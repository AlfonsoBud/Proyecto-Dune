using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using Shared;

namespace Application.Services
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;

        public GameService(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        public async Task<Game> CreateGameAsync(string playerAlias)
        {
            var game = new Game
            {
                PlayerAlias = playerAlias,
                Funds = 1000m
            };
            await _gameRepository.SaveAsync(game);
            return game;
        }

        public async Task<Game> GetGameAsync(Guid gameId)
        {
            return await _gameRepository.LoadAsync(gameId);
        }

        public async Task AddEventLogAsync(Guid gameId, string log, string type = "General")
        {
            var game = await GetGameAsync(gameId);
            if (game != null)
            {
                game.AddEvent(log, type);
                await _gameRepository.SaveAsync(game);
            }
        }
    }
}