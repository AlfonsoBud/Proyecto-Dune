using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Services;
using Api.DTOs;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly IGameService _gameService;
        private readonly ISimulationService _simulationService;
        private readonly IGameRepository _repo;

        public GamesController(IGameService gameService, ISimulationService simulationService, IGameRepository repo)
        {
            _gameService = gameService;
            _simulationService = simulationService;
            _repo = repo;
        }

        [HttpPost("create")]
        public async Task<ActionResult<GameDto>> Create([FromBody] CreateGameRequest req)
        {
            var game = await _gameService.CreateGameAsync(req.PlayerAlias ?? "Player");
            // game already saved by GameService.CreateGameAsync (calls repository.SaveAsync)
            var dto = MapToDto(game);
            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<GameDto>> Get(Guid id)
        {
            var game = await _gameService.GetGameAsync(id);
            if (game == null) return NotFound();
            return Ok(MapToDto(game));
        }

        [HttpPost("{id:guid}/round")]
        public async Task<ActionResult<GameDto>> ExecuteRound(Guid id)
        {
            var game = await _gameService.GetGameAsync(id);
            if (game == null) return NotFound();

            var updated = await _simulationService.RunRoundAsync(game);
            await _repo.SaveAsync(updated);
            return Ok(MapToDto(updated));
        }

        #region Helpers and request types

        public record CreateGameRequest(string PlayerAlias);

        private static GameDto MapToDto(Domain.Entities.Game game)
        {
            var dto = new GameDto
            {
                Id = game.Id,
                PlayerAlias = game.PlayerAlias,
                Funds = game.Funds,
                Events = game.EventLog.Select(e => new GameEventDto { Id = e.Id, Timestamp = e.Timestamp, Message = e.Message, Type = e.Type }).ToList()
            };

            foreach (var en in game.Enclaves)
            {
                var enDto = new EnclaveDto
                {
                    Name = en.Name,
                    Type = en.Type.ToString(),
                    Hectares = en.Hectares,
                    Supplies = en.Supplies,
                    Visitors = en.Visitors,
                    WealthLevel = en.WealthLevel.ToString()
                };

                foreach (var inst in en.Installations)
                {
                    var instDto = new InstallationDto
                    {
                        Code = inst.Code,
                        Cost = inst.Cost,
                        Capacity = inst.Capacity,
                        Environment = inst.Environment.ToString(),
                        Diet = inst.Diet.ToString(),
                        Supplies = inst.Supplies
                    };

                    instDto.Creatures.AddRange(inst.Creatures.Select(c => new CreatureDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Environment = c.Environment.ToString(),
                        Diet = c.Diet.ToString(),
                        Age = c.Age,
                        AdultAge = c.AdultAge,
                        Health = c.Health,
                        Appetite = c.Appetite
                    }));

                    enDto.Installations.Add(instDto);
                }

                dto.Enclaves.Add(enDto);
            }

            return dto;
        }

        #endregion
    }
}