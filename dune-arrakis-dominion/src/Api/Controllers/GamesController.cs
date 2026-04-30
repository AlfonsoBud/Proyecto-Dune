csharp src/Api/Controllers/GamesController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using Api.Hubs;
using Domain.Entities;
using Application.Services;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly IHubContext<GameHub> _hub;

        // Inyecta aquí tus servicios reales (IGameService, ISimulationService); si no tienes interfaces, inyecta las clases concretas.
        public GamesController(IHubContext<GameHub> hub)
        {
            _hub = hub;
        }

        // Ejemplo: broadcast manual (más adelante conecta con tu GameService)
        [HttpPost("broadcast")]
        public async Task<IActionResult> Broadcast([FromBody] JsonElement payload)
        {
            var json = JsonSerializer.Serialize(payload);
            await _hub.Clients.All.SendAsync("ReceiveGameUpdate", json);
            return Ok();
        }

        // Añade endpoints reales: Get game, Create game, Execute round, Save/Load etc.
    }
}