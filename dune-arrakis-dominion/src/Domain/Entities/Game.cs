namespace Domain.Entities
{
    using System;
    using System.Collections.Generic;

    public class Game
    {
        public Guid Id { get; set; }
        public string PlayerAlias { get; set; }
        public decimal Funds { get; set; }
        public List<Enclave> Enclaves { get; set; }
        public List<EventLog> EventLog { get; set; }

        public Game()
        {
            Id = Guid.NewGuid();
            Enclaves = new List<Enclave>();
            EventLog = new List<EventLog>();
        }

        public void AddEvent(string eventMessage, string type = "General")
        {
            EventLog.Add(new EventLog(eventMessage, type));
        }
    }
}