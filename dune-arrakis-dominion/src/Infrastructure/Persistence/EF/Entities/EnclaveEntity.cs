using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence.EF.Entities
{
    public class EnclaveEntity
    {
        public Guid Id { get; set; }
        public Guid GameId { get; set; }
        public GameEntity Game { get; set; }

        public string Name { get; set; }
        public int Type { get; set; }
        public double Hectares { get; set; }
        public double Supplies { get; set; }
        public int Visitors { get; set; }
        public int WealthLevel { get; set; }

        public List<InstallationEntity> Installations { get; set; } = new();
    }
}