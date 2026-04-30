using System;
using System.Threading.Tasks;
using Domain.Entities;
using Shared;

namespace PersistenceService
{
    public class PersistenceService : IPersistenceService
    {
        private readonly IGameRepository _gameRepository;

        public PersistenceService(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        public async Task SaveGameAsync(Game game)
        {
            await _gameRepository.SaveAsync(game);
        }

        public async Task<Game> LoadGameAsync(Guid id)
        {
            return await _gameRepository.LoadAsync(id);
        }
    }
}