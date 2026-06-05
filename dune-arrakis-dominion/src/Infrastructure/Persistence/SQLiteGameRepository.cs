using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Shared;
using Infrastructure.Persistence.EF;
using Infrastructure.Persistence.EF.Entities;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence
{
    public class SQLiteGameRepository : IGameRepository
    {
        private readonly AppDbContext _db;
        private readonly ILogger<SQLiteGameRepository>? _logger;

        public SQLiteGameRepository(AppDbContext db, ILogger<SQLiteGameRepository>? logger = null)
        {
            _db = db;
            _logger = logger;
        }

        public async Task SaveAsync(Game game)
        {
            if (game == null) throw new ArgumentNullException(nameof(game));

            var existing = await _db.Games
                .Include(g => g.Enclaves).ThenInclude(e => e.Installations).ThenInclude(i => i.Creatures)
                .Include(g => g.Events)
                .FirstOrDefaultAsync(g => g.Id == game.Id);

            var entity = MapToEntity(game);

            if (existing == null)
            {
                _db.Games.Add(entity);
            }
            else
            {
                // Replace state: easiest approach is to remove existing and add entity,
                // or update properties and sync collections. Simpler: remove then add.
                _db.Remove(existing);
                _db.Games.Add(entity);
            }

            await _db.SaveChangesAsync();
            _logger?.LogInformation("Game {GameId} saved to SQLite", game.Id);
        }

        public async Task<Game> LoadAsync(Guid id)
        {
            var entity = await _db.Games
                .Include(g => g.Enclaves).ThenInclude(e => e.Installations).ThenInclude(i => i.Creatures)
                .Include(g => g.Events)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (entity == null) return null;

            return MapToDomain(entity);
        }

        #region Mapping

        private static GameEntity MapToEntity(Game game)
        {
            var ge = new GameEntity
            {
                Id = game.Id,
                PlayerAlias = game.PlayerAlias,
                Funds = game.Funds,
                Events = game.EventLog.Select(ev => new GameEventEntity
                {
                    Id = ev.Id,
                    GameId = game.Id,
                    Timestamp = ev.Timestamp,
                    Message = ev.Message,
                    Type = ev.Type
                }).ToList()
            };

            ge.Enclaves = game.Enclaves.Select(en => new EnclaveEntity
            {
                Id = Guid.NewGuid(),
                GameId = game.Id,
                Name = en.Name,
                Type = (int)en.Type,
                Hectares = en.Hectares,
                Supplies = en.Supplies,
                Visitors = en.Visitors,
                WealthLevel = (int)en.WealthLevel,
                Installations = en.Installations.Select(inst => new InstallationEntity
                {
                    Id = Guid.NewGuid(),
                    Code = inst.Code,
                    Cost = inst.Cost,
                    Capacity = inst.Capacity,
                    Environment = (int)inst.Environment,
                    Diet = (int)inst.Diet,
                    Supplies = inst.Supplies,
                    Creatures = inst.Creatures.Select(c => new CreatureEntity
                    {
                        Id = c.Id == Guid.Empty ? Guid.NewGuid() : c.Id,
                        Name = c.Name,
                        Environment = (int)c.Environment,
                        Diet = (int)c.Diet,
                        Age = c.Age,
                        AdultAge = c.AdultAge,
                        Health = c.Health,
                        TimesFavorite = c.TimesFavorite,
                        Appetite = c.Appetite
                    }).ToList()
                }).ToList()
            }).ToList();

            return ge;
        }

        private static Game MapToDomain(GameEntity ge)
        {
            var game = new Game
            {
                Id = ge.Id,
                PlayerAlias = ge.PlayerAlias,
                Funds = ge.Funds
            };

            // Map events
            foreach (var ev in ge.Events.OrderBy(e => e.Timestamp))
            {
                var evt = new EventLog(ev.Message, ev.Type) { Id = ev.Id, Timestamp = ev.Timestamp };
                game.EventLog.Add(evt);
            }

            // Map enclaves/installations/creatures
            foreach (var en in ge.Enclaves)
            {
                var enclave = new Enclave(en.Name, (Domain.Enums.EnclaveType)en.Type, en.Hectares, en.Supplies, (Domain.Enums.WealthLevel)en.WealthLevel)
                {
                    Visitors = en.Visitors
                };

                foreach (var inst in en.Installations)
                {
                    var installation = new Installation
                    {
                        Code = inst.Code,
                        Cost = inst.Cost,
                        Capacity = inst.Capacity,
                        Environment = (Domain.Enums.EnvironmentType)inst.Environment,
                        Diet = (Domain.Enums.DietType)inst.Diet,
                        Supplies = inst.Supplies
                    };

                    foreach (var c in inst.Creatures)
                    {
                        var creature = new Creature(c.Name, (Domain.Enums.EnvironmentType)c.Environment, (Domain.Enums.DietType)c.Diet, c.AdultAge, c.Appetite)
                        {
                            Id = c.Id,
                            Age = c.Age,
                            Health = c.Health,
                            TimesFavorite = c.TimesFavorite
                        };
                        installation.Creatures.Add(creature);
                    }

                    enclave.Installations.Add(installation);
                }

                game.Enclaves.Add(enclave);
            }

            return game;
        }

        #endregion
    }
}