using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence.EF.Entities
{
    public class GameEntity
    {
        public Guid Id { get; set; }
        public string PlayerAlias { get; set; }
        public decimal Funds { get; set; }

        public List<EnclaveEntity> Enclaves { get; set; } = new();
        public List<GameEventEntity> Events { get; set; } = new();
    }
}