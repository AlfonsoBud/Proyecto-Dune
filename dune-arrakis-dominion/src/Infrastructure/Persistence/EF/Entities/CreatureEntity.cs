using System;

namespace Infrastructure.Persistence.EF.Entities
{
    public class CreatureEntity
    {
        public Guid Id { get; set; }
        public Guid InstallationId { get; set; }
        public InstallationEntity Installation { get; set; }

        public string Name { get; set; }
        public int Environment { get; set; }
        public int Diet { get; set; }
        public int Age { get; set; }
        public int AdultAge { get; set; }
        public int Health { get; set; }
        public int TimesFavorite { get; set; }
        public int Appetite { get; set; }
    }
}