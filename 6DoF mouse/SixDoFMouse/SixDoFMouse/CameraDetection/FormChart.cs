using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SixDoFMouse.CameraDetection
{
    /// <summary>
    /// Modes for updating the chart control
    /// </summary>
    public enum ChartUpdatingMode { Manual, Timed }

    /// <summary>
    /// Form with dynamically updatable chart and included timer for Timed updating
    /// </summary>
    public partial class FormChart : Form
    {
        /// <summary>
        /// Delegate used for safe-thread call of UpdateChartInternal method
        /// </summary>
        /// <param name="data">Values that should be visualized on chart</param>
        private delegate void UpdateChartInternalCallback(double[] data);

        // Counter used to count and move the data on X axis in time
        private int FrameCounter = 1;

        private ChartUpdatingMode _updatingMode;

        /// <summary>
        /// Updating mode of the chart, manual or timed.
        /// The timer is automatically started or stopped according the choosed mode
        /// </summary>
        public ChartUpdatingMode UpdatingMode
        {
            get
            {
                return _updatingMode;
            }
            set
            {
                _updatingMode = value;
                switch (_updatingMode)
                {
                    case ChartUpdatingMode.Manual:
                        timerGraph.Enabled = false;
                        break;
                    case ChartUpdatingMode.Timed:
                        timerGraph.Enabled = true;
                        break;
                    default:
                        break;
                }
            }
        }

        // Data array used during Timed update mode
        // On every tick of internal timer the data is read
        // and then nulled, which prevents visualizing the same data twice
        private double[] dataArr;

        /// <summary>
        /// Interval of the internal timer, used in Timed mode
        /// </summary>
        public int TimerInterval
        {
            get { return timerGraph.Interval; }
            set { timerGraph.Interval = value; }
        }

        /// <summary>
        /// The minimum value of Y axis. It's auto increased when greater
        /// value is passed for visualizing, but isn't auto decreased
        /// </summary>
        public double MinCoordinate
        {
            get { return chartCoordinates.ChartAreas[0].AxisY.Minimum; }
            set { chartCoordinates.ChartAreas[0].AxisY.Minimum = Math.Round(value, 0); }
        }

        /// <summary>
        /// The maximum value of Y axis. It's auto increased when greater
        /// value is passed for visualizing, but isn't auto decreased
        /// </summary>
        public double MaxCoordinate
        {
            get { return chartCoordinates.ChartAreas[0].AxisY.Maximum; }
            set { chartCoordinates.ChartAreas[0].AxisY.Maximum = Math.Round(value, 0); }
        }

        /// <summary>
        /// Insert a curve which will be updated with values from UpdateChart method.
        /// For every double value passed to UpdateChart, a curve needs to be added on initialization
        /// </summary>
        /// <param name="Name">String which will be added to the chart legend</param>
        public void AddCurve(string Name)
        {
            chartCoordinates.Series.Add(Name);
            chartCoordinates.Series[Name].ChartType = SeriesChartType.Line;
            chartCoordinates.Series[Name].ChartArea = "Coordinates";
            chartCoordinates.Series[Name].BorderWidth = 2;
        }

        /// <summary>
        /// Count of points which will be visualized on X axis
        /// </summary>
        public int PointCount { get; set; }

        /// <summary>
        /// Sets or gets whether the chart will automatically 
        /// check and update for min and max value of Y axis
        /// </summary>
        public bool AutoCheckBoundaries { get; set; }

        /// <summary>
        /// Constructor with default settings
        /// </summary>
        public FormChart()
            : this(50)
        {
        }

        /// <summary>
        /// Set the following settings, remaining are set to default
        /// </summary>
        /// <param name="aPtCount">Set PointCount</param>
        public FormChart(int aPtCount)
            : this(aPtCount, -1000, 1000)
        {
        }

        /// <summary>
        /// Set the following settings, remaining are set to default
        /// </summary>
        /// <param name="aPtCount">Set PointCount</param>
        /// <param name="aMinCoordinate">Set MinCoordinate</param>
        /// <param name="aMaxCoordinate">Set MaxCoordinate</param>
        public FormChart(int aPtCount, double aMinCoordinate, double aMaxCoordinate)
            : this(aPtCount, aMinCoordinate, aMaxCoordinate, "FormChart")
        {
        }

        /// <summary>
        /// Set the following settings, remaining are set to default
        /// </summary>
        /// <param name="aPtCount">Set PointCount</param>
        /// <param name="aMinCoordinate">Set MinCoordinate</param>
        /// <param name="aMaxCoordinate">Set MaxCoordinate</param>
        /// <param name="aTitle">Title of the chart form</param>
        public FormChart(int aPtCount, double aMinCoordinate, double aMaxCoordinate, string aTitle)
            : this(aPtCount, aMinCoordinate, aMaxCoordinate, aTitle, ChartUpdatingMode.Manual)
        {
        }

        /// <summary>
        /// Set the following settings, remaining are set to default
        /// </summary>
        /// <param name="aPtCount">Set PointCount</param>
        /// <param name="aMinCoordinate">Set MinCoordinate</param>
        /// <param name="aMaxCoordinate">Set MaxCoordinate</param>
        /// <param name="aTitle">Title of the chart form</param>
        /// <param name="aMode">Mode which will be used for updating values</param>
        public FormChart(int aPtCount, double aMinCoordinate, double aMaxCoordinate, string aTitle, ChartUpdatingMode aMode)
            : this(aPtCount, aMinCoordinate, aMaxCoordinate, aTitle, aMode, 100)
        {
        }

        /// <summary>
        /// Set the following settings
        /// </summary>
        /// <param name="aPtCount">Set PointCount</param>
        /// <param name="aMinCoordinate">Set MinCoordinate</param>
        /// <param name="aMaxCoordinate">Set MaxCoordinate</param>
        /// <param name="aTitle">Title of the chart form</param>
        /// <param name="aMode">Mode which will be used for updating values</param>
        /// <param name="aTimerInterval">If mode is set to "Timed", the chart will be updated every given milliseconds</param>
        public FormChart(int aPtCount, double aMinCoordinate, double aMaxCoordinate, string aTitle, ChartUpdatingMode aMode, int aTimerInterval)
        {
            InitializeComponent();
            // That is needed for smoothing the dynamic chart
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint, true);
            PointCount = aPtCount;
            MinCoordinate = aMinCoordinate;
            MaxCoordinate = aMaxCoordinate;
            Text = aTitle;
            AutoCheckBoundaries = true;
            UpdatingMode = aMode;
            TimerInterval = aTimerInterval;

            chartCoordinates.ChartAreas["Coordinates"].AxisX.Minimum = 1;
            chartCoordinates.ChartAreas["Coordinates"].AxisX.Maximum = aPtCount;
            chartCoordinates.ChartAreas["Coordinates"].AxisX.Interval = 2;
            chartCoordinates.ChartAreas["Coordinates"].AxisX.Title = "Frame";
            chartCoordinates.ChartAreas["Coordinates"].AxisY.Title = "Coordinate";

            // That strip indicates the Zero value on Y axis with 2 pixels width
            StripLine stripLine = new StripLine();
            stripLine.BackColor = Color.Black;
            stripLine.IntervalOffset = 0;
            stripLine.Interval = 0;
            stripLine.BorderColor = Color.Black;
            stripLine.BorderWidth = 2; // in pixels
            chartCoordinates.ChartAreas["Coordinates"].AxisY.StripLines.Add(stripLine);
        }

        /// <summary>
        /// Send the given variable count of double values to the chart
        /// </summary>
        /// <param name="data">Variable count of double values</param>
        public void UpdateChart(params double[] data)
        {
            // Depending on the set mode the values are sent for visualizing
            switch (UpdatingMode)
            {
                // In case of manual mode, each call to this
                // method updates immediately the chart
                case ChartUpdatingMode.Manual:
                    UpdateChartInternal(data);
                    break;
                // Independently how often this method is called
                // the timer dispatch the data array in intervals set by TimerInterval
                case ChartUpdatingMode.Timed:
                    dataArr = data;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Visualizes the given double values on each added curve(Series) to the chart.
        /// If the values are more or less than the count of curves, the method displays
        /// only these variables, for which a cruve exists
        /// </summary>
        /// <param name="data">Variable count of double values, which
        /// are added on Y axis, and displayed in time by X axis</param>
        private void UpdateChartInternal(double[] data)
        {
            if (chartCoordinates.InvokeRequired)
            {
                // Create a delegate for safe-thread call to this method
                UpdateChartInternalCallback d = new UpdateChartInternalCallback(UpdateChartInternal);
                this.Invoke(d, new object[] { data });
            }
            else
            {
                if (AutoCheckBoundaries)
                    CheckBoundaries(data);

                // Provides safety when count of passed double values is different than existing curves
                int count = Math.Min(data.Length, chartCoordinates.Series.Count);
                if (chartCoordinates.Series[0].Points.Count == PointCount)
                {
                    // If all possible points are filled (which is set by PointCount property)
                    // the first point is removed and X axis interval moves with 1 value ahead
                    for (int i = 0; i < count; i++)
                        chartCoordinates.Series[i].Points.RemoveAt(0);
                    // Moving the range of X makes the chart moving ahead
                    chartCoordinates.ChartAreas["Coordinates"].AxisX.Minimum++;
                    chartCoordinates.ChartAreas["Coordinates"].AxisX.Maximum++;
                }
                // Separate every double value on its corresponding curve(Series)
                // and asign it to Y value of the new point
                for (int i = 0; i < count; i++)
                    chartCoordinates.Series[i].Points.AddXY(FrameCounter, data[i]);
                // This is the X coordinate of the axis
                FrameCounter++;
                // Reduces the flickering while chart is updated
                Refresh();
            }
        }

        /// <summary>
        /// Check if any of the passed double values exeeds the current
        /// min and max assigned range and resize it if needed
        /// </summary>
        /// <param name="data">Passed double values for visualizing</param>
        private void CheckBoundaries(double[] data)
        {
            if (data.Length == 0)
                return;

            double min = data[0];
            double max = data[0];
            foreach (double val in data)
            {
                if (min > val)
                    min = val;
                if (max < val)
                    max = val;
            }
            
            // Update the min displayed value of Y axis if needed
            if (min < MinCoordinate)
                MinCoordinate = min;
            // Update the max displayed value of Y axis if needed
            if (max > MaxCoordinate)
                MaxCoordinate = max;
        }

        /// <summary>
        /// The timer which is used in Timed mode
        /// </summary>
        private void timerGraph_Tick(object sender, EventArgs e)
        {
            if (dataArr != null)
            {
                UpdateChartInternal(dataArr);
                // Nulling the internal data array will ensure that every values are displayed
                // only once, in case that calling method is slower than the timer interval
                dataArr = null;
            }
        }
    }
}
