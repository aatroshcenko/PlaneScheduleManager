using PlaneScheduleManager.Server.Utils.Interfaces;
using System.Collections.Concurrent;

namespace PlaneScheduleManager.Server.Utils
{
    public class TimerManager: ITimerManager, IDisposable
    {
        private readonly ConcurrentDictionary<Guid, System.Timers.Timer> _timeoutTimers =
            new ConcurrentDictionary<Guid, System.Timers.Timer>();
        private readonly ConcurrentDictionary<Guid, System.Timers.Timer> _intervalTimers =
            new ConcurrentDictionary<Guid, System.Timers.Timer>();

        public Guid SetTimeout(Func<Task> asyncFunc, double timeoutMilliseconds)
        {
            var timer = new System.Timers.Timer(timeoutMilliseconds);
            var timerId = Guid.NewGuid();
            _timeoutTimers.TryAdd(timerId, timer);
            timer.Elapsed += async (state, e) =>
            {
                await asyncFunc.Invoke();
                if (_timeoutTimers.Remove(timerId, out var savedTimer))
                {
                    savedTimer.Dispose();
                };
            };
            timer.AutoReset = false;
            timer.Enabled = true;
            return timerId;
        }

        public Guid SetInterval(Action action, double intervalMilliseconds)
        {
            var timer = new System.Timers.Timer(intervalMilliseconds);
            var timerId = Guid.NewGuid();
            _intervalTimers.TryAdd(timerId, timer);
            timer.Elapsed += (state, e) =>
            {
                action.Invoke();
            };
            timer.AutoReset = true;
            timer.Enabled = true;
            return timerId;
        }

        public void ClearInterval(Guid timerId)
        {
            if (_intervalTimers.TryRemove(timerId, out var savedTimer))
            {
                savedTimer.Dispose();
            };
        }

        public void Dispose()
        {
            foreach(var timer in _timeoutTimers.Values)
            {
                timer.Dispose();
            }

            foreach(var timer in _intervalTimers.Values)
            {
                timer.Dispose();
            }
        }
    }
}
