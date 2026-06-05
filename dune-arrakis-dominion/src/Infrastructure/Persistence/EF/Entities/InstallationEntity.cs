using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence.EF.Entities
{
    public class InstallationEntity
    {
        public Guid Id { get; set; }
        public Guid EnclaveId { get; set; }
        public EnclaveEntity Enclave { get; set; }

        public string Code { get; set; }
        public decimal Cost { get; set; }
        public int Capacity { get; set; }
        public int Environment { get; set; }
        public int Diet { get; set; }
        public int Supplies { get; set; }

        public List<CreatureEntity> Creatures { get; set; } = new();
    }
}