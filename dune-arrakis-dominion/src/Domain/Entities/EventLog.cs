namespace Domain.Entities
{
    using System;

    public class EventLog
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }
        public string Type { get; set; } // e.g., "Creature", "Enclave", "Simulation"

        public EventLog(string message, string type = "General")
        {
            Id = Guid.NewGuid();
            Timestamp = DateTime.UtcNow;
            Message = message;
            Type = type;
        }
    }
}