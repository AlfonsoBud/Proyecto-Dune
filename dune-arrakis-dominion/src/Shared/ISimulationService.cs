using System.Threading.Tasks;
using Domain.Entities;

namespace Shared
{
    public interface ISimulationService
    {
        Task<Game> RunRoundAsync(Game game);
    }
}
