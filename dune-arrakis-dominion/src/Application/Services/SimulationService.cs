using System;
using System.Linq;
using System.Threading.Tasks;
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

        private async Task SimulateInstallationAsync(Game game, Installation installation)
        {
            double totalSuppliesNeeded = 0;
            double availableSupplies = installation.Supplies;

            // Calculate feeding for each creature
            foreach (var creature in installation.Creatures)
            {
                double needed = CalculateFoodNeeded(creature);
                totalSuppliesNeeded += needed;
            }

            // Feed creatures based on available supplies
            double supplyRatio = availableSupplies / totalSuppliesNeeded;
            if (supplyRatio > 1) supplyRatio = 1;

            foreach (var creature in installation.Creatures)
            {
                double fedAmount = CalculateFoodNeeded(creature) * supplyRatio;
                double fedPercentage = fedAmount / CalculateFoodNeeded(creature) * 100;

                // Update health based on feeding
                int healthChange = CalculateHealthChange(fedPercentage);
                creature.UpdateHealth(healthChange);

                // Age the creature
                creature.GrowOlder();

                // Reproduction
                if (creature.Age >= creature.AdultAge && new Random().Next(100) < 20)
                {
                    // Create offspring
                    var offspring = new Creature(
                        $"{creature.Name} Jr",
                        creature.Environment,
                        creature.Diet,
                        creature.AdultAge,
                        creature.Appetite
                    );
                    installation.Creatures.Add(offspring);
                    game.AddEvent($"Creature {creature.Name} reproduced", "Creature");
                }

                game.AddEvent($"Creature {creature.Name} fed {fedPercentage:F1}%, health now {creature.Health}", "Creature");
            }

            // Update supplies
            installation.Supplies -= (int)(totalSuppliesNeeded * supplyRatio);
            if (installation.Supplies < 0) installation.Supplies = 0;
        }

        private double CalculateFoodNeeded(Creature creature)
        {
            if (creature.Age < creature.AdultAge)
            {
                return creature.Appetite * creature.Age;
            }
            else
            {
                return creature.Appetite * Math.Pow(2, creature.Age - creature.AdultAge);
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
            double totalHectares = enclave.Installations.Sum(i => i.Capacity); // Assuming capacity represents hectares per installation
            double instHectares = enclave.Hectares; // This might need adjustment
            double avgHealth = enclave.Installations
                .SelectMany(i => i.Creatures)
                .Average(c => c.Health);

            // Simplified formula
            return (int)(enclave.Visitors * (instHectares / totalHectares) * (avgHealth / 100));
        }

        private decimal CalculateDonations(Enclave enclave)
        {
            double sigma = enclave.Installations
                .SelectMany(i => i.Creatures)
                .Average(c => c.Health / 100.0);

            decimal totalDonations = 0;
            foreach (var installation in enclave.Installations)
            {
                foreach (var creature in installation.Creatures)
                {
                    double donation = 10 * (creature.Health / 100.0) * (creature.Age / (double)creature.AdultAge) * sigma;
                    totalDonations += (decimal)donation;
                }
            }
            return totalDonations;
        }
    }
}