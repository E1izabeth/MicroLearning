using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MicroLearningSvc.Util
{
    interface ITimer : IDisposable
    {
        event EventHandler Elapsed;

        TimeSpan Interval { get; set; }

        void Start();
        void Stop();
    }

    class TimerImpl : ITimer
    {
        public event EventHandler Elapsed = delegate { };

        readonly Timer _timer = new Timer() { AutoReset = false, Enabled = false };

        public TimeSpan Interval
        {
            get { return TimeSpan.FromMilliseconds(_timer.Interval); }
            set { _timer.Interval = value.TotalMilliseconds; }
        }

        public TimerImpl()
        {
            _timer.Elapsed += (sender, ea) => this.Elapsed.Invoke(this, ea);
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        public void Dispose()
        {
            _timer.SafeDispose();
        }

    }
}
