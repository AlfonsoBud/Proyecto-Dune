using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.Entities;
using Shared;

namespace Infrastructure.Persistence
{
    public class JsonGameRepository : IGameRepository
    {
        private readonly string _dataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "data", "games");

        public JsonGameRepository()
        {
            if (!Directory.Exists(_dataDirectory))
            {
                Directory.CreateDirectory(_dataDirectory);
            }
        }

        public async Task SaveAsync(Game game)
        {
            var filePath = Path.Combine(_dataDirectory, $"{game.Id}.json");
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(game, options);
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task<Game> LoadAsync(Guid id)
        {
            var filePath = Path.Combine(_dataDirectory, $"{id}.json");
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Game not found", filePath);
            }

            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<Game>(json);
        }
    }
}