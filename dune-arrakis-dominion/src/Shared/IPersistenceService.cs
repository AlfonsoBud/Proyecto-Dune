using System;
using System.Threading.Tasks;
using Domain.Entities;

namespace Shared
{
    public interface IPersistenceService
    {
        Task SaveGameAsync(Game game);
        Task<Game> LoadGameAsync(Guid id);
    }
}
