namespace IoTDevice.Client.Domain
{
    public class Device
    {
        public Guid Identifier { get; }

        public string Area { get; }

        public Device(Guid identifier, string area)
        {
            Identifier = identifier;
            Area = area;
        }
    }
}
