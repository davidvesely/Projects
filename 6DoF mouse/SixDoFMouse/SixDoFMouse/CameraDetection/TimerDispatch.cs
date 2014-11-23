using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//using System.Timers;

namespace SixDoFMouse.CameraDetection
{
    public delegate void StopwatchCallEventHandler(object sender, EventArgs e);

    /// <summary>
    /// Timer, which activates the subscribed
    /// method at regular intervals
    /// </summary>
    public class TimerDispatch
    {
        private Timer timer;

        public event StopwatchCallEventHandler TickMethod;
        private int _interval;
        public int Interval 
        {
            get { return _interval; }
        }

        protected virtual void OnTick()
        {
            if (TickMethod != null)
                TickMethod(this, EventArgs.Empty);
        }

        public TimerDispatch(int interval)
        {
            timer = new Timer();
            timer.Interval = interval;
            timer.Tick += new EventHandler(this.Stopwatch_Tick);
            _interval = interval;
        }

        public TimerDispatch() : this(10)
        {
        }

        public bool Enabled
        {
            get { return timer.Enabled; }
            set { timer.Enabled = value; }
        }

        private void Stopwatch_Tick(object sender, EventArgs e)
        {
            OnTick();
        }
    }
}