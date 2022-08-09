namespace IoTDevice.Client.Domain
{
    public class Device
    {
        private readonly HashSet<int> _additionalGates = new HashSet<int>();
        public IReadOnlyList<int> AdditionalGates => _additionalGates.ToList();
        public string Identifier { get; }

        public string Area { get; }

        public int Gate { get; }

        public Device(
            string identifier,
            string area,
            int gate)
        {
            Identifier = identifier;
            Area = area;
            Gate = gate;
        }

        public void AddGate(int gate)
        {
            _additionalGates.Add(gate);
        }

        public void RemoveGate(int gate)
        {
            _additionalGates.Remove(gate);
        }
    }
}
