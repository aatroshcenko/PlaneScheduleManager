namespace IoTDevice.Client.Services.Interfaces
{
    public interface IHubMessageSender
    {
        Task SendHeartbeatAsync();
        Task BroadcastClusterLockAsync();
        Task BroadcastClusterReleaseAsync();
    }
}
