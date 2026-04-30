namespace Domain.Entities
{
    using System.Collections.Generic;
    using Domain.Enums;

    public class Installation
    {
        public string Code { get; set; }
        public decimal Cost { get; set; }
        public int Capacity { get; set; }
        public EnvironmentType Environment { get; set; }
        public DietType Diet { get; set; }
        public int Supplies { get; set; }
        public List<Creature> Creatures { get; set; }

        public Installation()
        {
            Creatures = new List<Creature>();
        }
    }
}