using System;
using System.Linq;
using System.Threading.Tasks;
using DotNetEnv;
using DotNetEnv;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Shared;

namespace AdminClient
{
    class Program
    {
        private static IGameService _gameService;
        private static ISimulationService _simulationService;
        private static IPersistenceService _persistenceService;
        private static Game _currentGame;

        static async Task Main(string[] args)
        {
            // Load environment variables
            Env.Load("../../../.env");

            var apiUrl = Environment.GetEnvironmentVariable("API_URL");
            var bearerToken = Environment.GetEnvironmentVariable("BEARER_TOKEN");
            var userBearerToken = Environment.GetEnvironmentVariable("USER_BEARER_TOKEN");

            var apiClient = new ApiClient(apiUrl, bearerToken, userBearerToken);

            // Initialize services
            var repository = new JsonGameRepository();
            _gameService = new GameService(repository);
            _simulationService = new Application.Services.SimulationService();
            _persistenceService = new PersistenceService.PersistenceService(repository);

            Console.WriteLine("Welcome to Dune: Arrakis Dominion Admin Client");

            while (true)
            {
                ShowMenu();
                var choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            await CreateGameAsync();
                            break;
                        case "2":
                            ViewGameStatus();
                            break;
                        case "3":
                            await RunRoundAsync();
                            break;
                        case "4":
                            await SaveGameAsync();
                            break;
                        case "5":
                            await LoadGameAsync();
                            break;
                        case "6":
                            return;
                        default:
                            Console.WriteLine("Invalid choice. Try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }

        private static void ShowMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Dune: Arrakis Dominion ===");
            Console.WriteLine("1. Create new game");
            Console.WriteLine("2. View game status");
            Console.WriteLine("3. Execute round");
            Console.WriteLine("4. Save game");
            Console.WriteLine("5. Load game");
            Console.WriteLine("6. Exit");
            Console.Write("Choose an option: ");
        }

        private static async Task CreateGameAsync()
        {
            Console.Write("Enter player alias: ");
            var alias = Console.ReadLine();
            _currentGame = await _gameService.CreateGameAsync(alias);
            Console.WriteLine($"Game created for player {alias} with ID {_currentGame.Id}");
        }

        private static void ViewGameStatus()
        {
            if (_currentGame == null)
            {
                Console.WriteLine("No game loaded.");
                return;
            }

            Console.WriteLine($"Player: {_currentGame.PlayerAlias}");
            Console.WriteLine($"Funds: {_currentGame.Funds:C}");
            Console.WriteLine($"Enclaves: {_currentGame.Enclaves.Count}");

            foreach (var enclave in _currentGame.Enclaves)
            {
                Console.WriteLine($"- {enclave.Name} ({enclave.Type}): {enclave.Hectares} ha, {enclave.Visitors} visitors");
                foreach (var installation in enclave.Installations)
                {
                    Console.WriteLine($"  - Installation {installation.Code}: {installation.Creatures.Count} creatures");
                    foreach (var creature in installation.Creatures.OrderByDescending(c => c.Health))
                    {
                        Console.WriteLine($"    - {creature.Name}: Age {creature.Age}, Health {creature.Health}");
                    }
                }
            }

            Console.WriteLine("Recent Events:");
            foreach (var log in _currentGame.EventLog.TakeLast(5))
            {
                Console.WriteLine($"- {log.Timestamp}: {log.Message}");
            }
        }

        private static async Task RunRoundAsync()
        {
            if (_currentGame == null)
            {
                Console.WriteLine("No game loaded.");
                return;
            }

            _currentGame = await _simulationService.RunRoundAsync(_currentGame);
            Console.WriteLine("Round executed successfully.");
        }

        private static async Task SaveGameAsync()
        {
            if (_currentGame == null)
            {
                Console.WriteLine("No game loaded.");
                return;
            }

            await _persistenceService.SaveGameAsync(_currentGame);
            Console.WriteLine("Game saved.");
        }

        private static async Task LoadGameAsync()
        {
            Console.Write("Enter game ID: ");
            if (Guid.TryParse(Console.ReadLine(), out var id))
            {
                _currentGame = await _persistenceService.LoadGameAsync(id);
                Console.WriteLine("Game loaded.");
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }
    }
}