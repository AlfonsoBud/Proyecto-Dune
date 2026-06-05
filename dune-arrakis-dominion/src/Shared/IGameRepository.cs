using System;
using System.Threading.Tasks;
using Domain.Entities;

namespace Shared
{
    public interface IGameRepository
    {
        Task SaveAsync(Game game);
        Task<Game> LoadAsync(Guid id);
    }
}
