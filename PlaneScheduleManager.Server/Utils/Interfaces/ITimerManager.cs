namespace PlaneScheduleManager.Server.Utils.Interfaces
{
    public interface ITimerManager
    {
        public void SetTimeout(Func<Task> asyncAction, double timeoutMilliseconds);
    }
}
