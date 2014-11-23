using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace SixDoFMouse.CameraDetection
{
    public class TimerStopwatch
    {
        private Timer timer;

        public TimerStopwatch(double interval)
        {
            timer = new Timer(interval);
            timer.Elapsed += new ElapsedEventHandler(this.Stopwatch_Tick);
            timer.AutoReset = true;
            _elapsedTime = 0;
        }

        public TimerStopwatch()
            : this(10)
        {
        }

        private double _elapsedTime;
        public double ElapsedTime
        {
            get
            {
                double retInterval = _elapsedTime;
                _elapsedTime = 0;
                return retInterval * timer.Interval;
            }
        }

        public bool Enabled
        {
            get { return timer.Enabled; }
            set { timer.Enabled = value; }
        }

        private void Stopwatch_Tick(object sender, ElapsedEventArgs e)
        {
            _elapsedTime++;
        }
    }
}
