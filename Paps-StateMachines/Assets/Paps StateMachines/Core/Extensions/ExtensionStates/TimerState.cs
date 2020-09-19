using System;
using System.Timers;

namespace Paps.StateMachines.Extensions
{
    public class TimerState : IState
    {
        private Timer _timer;
        private Action _onTimerElapsed;

        public TimerState(TimeSpan time, Action onTimerElapsed)
        {
            _onTimerElapsed = onTimerElapsed;

            _timer = new Timer();
            _timer.AutoReset = false;
            _timer.Interval = time.TotalMilliseconds;
        }

        public void Enter()
        {
            _timer.Start();
        }

        public void Exit()
        {
            _timer.Stop();
        }

        public void Update()
        {
            if (_timer.Enabled == false)
            {
                _onTimerElapsed();
            }
        }
    }
}