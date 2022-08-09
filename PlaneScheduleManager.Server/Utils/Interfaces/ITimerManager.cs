namespace PlaneScheduleManager.Server.Utils.Interfaces
{
    public interface ITimerManager
    {
        Guid SetTimeout(Func<Task> asyncFunc, double timeoutMilliseconds);
        Guid SetInterval(Action action, double intervalMilliseconds);

        void ClearInterval(Guid timerId);
    }
}
