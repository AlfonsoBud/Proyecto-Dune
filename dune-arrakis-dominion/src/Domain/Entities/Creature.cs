namespace Domain.Entities
{
    using System;
    using Domain.Enums;

    public class Creature
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public EnvironmentType Environment { get; set; }
        public DietType Diet { get; set; }
        public int Age { get; set; }
        public int AdultAge { get; set; }
        public int Health { get; set; }
        public int TimesFavorite { get; set; }
        public int Appetite { get; set; }

        public Creature(string name, EnvironmentType environment, DietType diet, int adultAge, int appetite)
        {
            Id = Guid.NewGuid();
            Name = name;
            Environment = environment;
            Diet = diet;
            Age = 0;
            AdultAge = adultAge;
            Health = 100; // Default health
            TimesFavorite = 0;
            Appetite = appetite;
        }

        public void GrowOlder()
        {
            Age++;
            if (Age > AdultAge)
            {
                // Logic for adult creatures can be added here
            }
        }

        public void UpdateHealth(int healthChange)
        {
            Health = Math.Clamp(Health + healthChange, 0, 100);
        }
    }
}