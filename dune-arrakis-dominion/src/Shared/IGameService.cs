using System;
using System.Threading.Tasks;
using Domain.Entities;

namespace Shared
{
    public interface IGameService
    {
        Task<Game> CreateGameAsync(string playerAlias);
        Task<Game> GetGameAsync(Guid gameId);
        Task AddEventLogAsync(Guid gameId, string log, string type = "General");
    }
}
