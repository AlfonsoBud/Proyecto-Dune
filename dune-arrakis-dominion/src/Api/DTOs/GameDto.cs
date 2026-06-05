using System;
using System.Collections.Generic;

namespace Api.DTOs
{
    public class GameDto
    {
        public Guid Id { get; set; }
        public string PlayerAlias { get; set; }
        public decimal Funds { get; set; }
        public List<EnclaveDto> Enclaves { get; set; } = new();
        public List<GameEventDto> Events { get; set; } = new();
    }

    public class EnclaveDto
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public double Hectares { get; set; }
        public double Supplies { get; set; }
        public int Visitors { get; set; }
        public string WealthLevel { get; set; }
        public List<InstallationDto> Installations { get; set; } = new();
    }

    public class InstallationDto
    {
        public string Code { get; set; }
        public decimal Cost { get; set; }
        public int Capacity { get; set; }
        public string Environment { get; set; }
        public string Diet { get; set; }
        public int Supplies { get; set; }
        public List<CreatureDto> Creatures { get; set; } = new();
    }

    public class CreatureDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Environment { get; set; }
        public string Diet { get; set; }
        public int Age { get; set; }
        public int AdultAge { get; set; }
        public int Health { get; set; }
        public int Appetite { get; set; }
    }

    public class GameEventDto
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
    }
}