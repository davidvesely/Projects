namespace SixDoFMouse.CameraDetection
{
    partial class FormChart
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            this.chartCoordinates = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.timerGraph = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.chartCoordinates)).BeginInit();
            this.SuspendLayout();
            // 
            // chartCoordinates
            // 
            chartArea1.Name = "Coordinates";
            this.chartCoordinates.ChartAreas.Add(chartArea1);
            this.chartCoordinates.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.chartCoordinates.Legends.Add(legend1);
            this.chartCoordinates.Location = new System.Drawing.Point(0, 0);
            this.chartCoordinates.Name = "chartCoordinates";
            this.chartCoordinates.Size = new System.Drawing.Size(1346, 543);
            this.chartCoordinates.TabIndex = 0;
            this.chartCoordinates.Text = "chart1";
            // 
            // timerGraph
            // 
            this.timerGraph.Tick += new System.EventHandler(this.timerGraph_Tick);
            // 
            // FormChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1346, 543);
            this.Controls.Add(this.chartCoordinates);
            this.Name = "FormChart";
            this.Text = "FormChart";
            ((System.ComponentModel.ISupportInitialize)(this.chartCoordinates)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chartCoordinates;
        private System.Windows.Forms.Timer timerGraph;
    }
}