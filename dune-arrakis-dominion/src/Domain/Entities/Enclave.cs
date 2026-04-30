namespace Domain.Entities
{
    using System.Collections.Generic;
    using Domain.Enums;

    public class Enclave
    {
        public string Name { get; set; }
        public EnclaveType Type { get; set; }
        public double Hectares { get; set; }
        public double Supplies { get; set; }
        public List<Installation> Installations { get; set; }
        public int Visitors { get; set; }
        public WealthLevel WealthLevel { get; set; }

        public Enclave(string name, EnclaveType type, double hectares, double supplies, WealthLevel wealthLevel)
        {
            Name = name;
            Type = type;
            Hectares = hectares;
            Supplies = supplies;
            Installations = new List<Installation>();
            Visitors = 0;
            WealthLevel = wealthLevel;
        }

        public void AddInstallation(Installation installation)
        {
            Installations.Add(installation);
        }

        public void RemoveInstallation(Installation installation)
        {
            Installations.Remove(installation);
        }
    }
}