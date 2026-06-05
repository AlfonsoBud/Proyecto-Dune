using System;

namespace Infrastructure.Persistence.EF.Entities
{
    public class GameEventEntity
    {
        public Guid Id { get; set; }
        public Guid GameId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public GameEntity Game { get; set; }
    }
}