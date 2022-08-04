using PlaneScheduleManager.Server.Utils.Interfaces;
using System.Collections.Concurrent;

namespace PlaneScheduleManager.Server.Utils
{
    public class TimerManager: ITimerManager
    {
        private readonly ConcurrentDictionary<Guid, System.Timers.Timer> _timers =
            new ConcurrentDictionary<Guid, System.Timers.Timer>();

        public void SetTimeout(Func<Task> asyncAction, double timeoutMilliseconds)
        {
            var timer = new System.Timers.Timer(timeoutMilliseconds);
            var timerIdentifer = Guid.NewGuid();
            _timers.TryAdd(timerIdentifer, timer);
            timer.Elapsed += async (state, e) =>
            {
                await asyncAction.Invoke();
                if (_timers.Remove(timerIdentifer, out var savedTimer))
                {
                    savedTimer.Dispose();
                };
            };
            timer.AutoReset = false;
            timer.Enabled = true;
        }
    }
}
