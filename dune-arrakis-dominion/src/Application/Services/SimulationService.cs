using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Domain.Entities;
using Domain.Enums;
using Shared;

namespace Application.Services
{
    public class SimulationService : ISimulationService
    {
        public async Task<Game> RunRoundAsync(Game game)
        {
            // Simulate one month
            game.AddEvent("Starting new round simulation", "Simulation");

            // Process each enclave
            foreach (var enclave in game.Enclaves)
            {
                await SimulateEnclaveAsync(game, enclave);
            }

            // Calculate donations and update funds
            decimal totalDonations = 0;
            foreach (var enclave in game.Enclaves)
            {
                totalDonations += CalculateDonations(enclave);
            }
            game.Funds += totalDonations;
            game.AddEvent($"Total donations received: {totalDonations:C}", "Finance");

            return game;
        }

        private async Task SimulateEnclaveAsync(Game game, Enclave enclave)
        {
            // Generate visitors
            int visitors = CalculateVisitors(enclave);
            enclave.Visitors = visitors;
            game.AddEvent($"Enclave {enclave.Name} received {visitors} visitors", "Enclave");

            // Process each installation
            foreach (var installation in enclave.Installations)
            {
                await SimulateInstallationAsync(game, installation);
            }
        }

        private Task SimulateInstallationAsync(Game game, Installation installation)
        {
            // Evitar modificar la colección original durante la iteración
            var creatures = installation.Creatures.ToList();

            double totalSuppliesNeeded = 0;
            foreach (var creature in creatures)
            {
                totalSuppliesNeeded += CalculateFoodNeeded(creature);
            }

            double availableSupplies = installation.Supplies;
            // Evitar división por cero
            double supplyRatio = totalSuppliesNeeded <= 0 ? 0 : Math.Min(1, availableSupplies / totalSuppliesNeeded);

            var offspringToAdd = new List<Creature>();
            var rnd = Random.Shared;

            foreach (var creature in creatures)
            {
                double needed = CalculateFoodNeeded(creature);
                double fedAmount = needed * supplyRatio;
                double fedPercentage = needed <= 0 ? 0 : (fedAmount / needed) * 100.0;

                // Update health based on feeding
                int healthChange = CalculateHealthChange(fedPercentage);
                creature.UpdateHealth(healthChange);

                // Age the creature
                creature.GrowOlder();

                // Reproduction (usar Random.Shared)
                if (creature.AdultAge > 0 && creature.Age >= creature.AdultAge && rnd.Next(100) < 20)
                {
                    // Create offspring
                    var offspring = new Creature(
                        $"{creature.Name} Jr",
                        creature.Environment,
                        creature.Diet,
                        creature.AdultAge,
                        creature.Appetite
                    );
                    offspringToAdd.Add(offspring);
                    game.AddEvent($"Creature {creature.Name} reproduced", "Creature");
                }

                game.AddEvent($"Creature {creature.Name} fed {fedPercentage:F1}%, health now {creature.Health}", "Creature");
            }

            // Añadir descendientes después de iterar
            if (offspringToAdd.Count > 0)
            {
                installation.Creatures.AddRange(offspringToAdd);
            }

            // Actualizar suministros
            installation.Supplies -= (int)(totalSuppliesNeeded * supplyRatio);
            if (installation.Supplies < 0) installation.Supplies = 0;

            return Task.CompletedTask;
        }

        private double CalculateFoodNeeded(Creature creature)
        {
            if (creature.Age < creature.AdultAge)
            {
                return creature.Appetite * creature.Age;
            }
            else
            {
                // Proteger contra exponentes muy altos
                var exponent = Math.Max(0, creature.Age - creature.AdultAge);
                var factor = Math.Min(32.0, Math.Pow(2, exponent)); // tope razonable
                return creature.Appetite * factor;
            }
        }

        private int CalculateHealthChange(double fedPercentage)
        {
            if (fedPercentage < 25) return -30;
            else if (fedPercentage < 75) return -20;
            else if (fedPercentage < 100) return -10;
            else return 5;
        }

        private int CalculateVisitors(Enclave enclave)
        {
            // Protección contra listas vacías y división por cero.
            var allCreatures = enclave.Installations.SelectMany(i => i.Creatures).ToList();
            if (!allCreatures.Any()) return 0;

            double avgHealth = allCreatures.Average(c => c.Health);
            // Fórmula sencilla y determinista: visitantes por hectárea ponderado por salud media
            const double baseVisitorsPerHectare = 10.0;
            var visitors = (int)(enclave.Hectares * baseVisitorsPerHectare * (avgHealth / 100.0));
            return Math.Max(0, visitors);
        }

        private decimal CalculateDonations(Enclave enclave)
        {
            var creatures = enclave.Installations.SelectMany(i => i.Creatures).ToList();
            if (!creatures.Any()) return 0m;

            double sigma = creatures.Average(c => c.Health / 100.0);

            decimal totalDonations = 0;
            foreach (var installation in enclave.Installations)
            {
                foreach (var creature in installation.Creatures)
                {
                    var adultDenom = creature.AdultAge > 0 ? creature.AdultAge : 1;
                    double donation = 10 * (creature.Health / 100.0) * (creature.Age / (double)adultDenom) * sigma;
                    totalDonations += (decimal)donation;
                }
            }
            return totalDonations;
        }
    }
}