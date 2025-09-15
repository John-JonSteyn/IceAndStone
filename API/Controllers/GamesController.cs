using IceAndStone.API.Requests;
using IceAndStone.API.Responses;
using IceAndStone.API.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace IceAndStone.API.Controllers
{
    [ApiController]
    [Route("api/games")]
    public class GamesController : ControllerBase
    {
        private readonly IGameService _service;
        public GamesController(IGameService service) => _service = service;

        /// <summary>Starts a new game within a session.</summary>
        [HttpPost("start")]
        public async Task<ActionResult<GameResponse>> Start([FromBody] StartGameRequest request, CancellationToken cancellationToken)
            => Ok(await _service.StartAsync(request, cancellationToken));

        /// <summary>Ends the current game and finalises results.</summary>
        [HttpPost("end")]
        public async Task<ActionResult<GameResponse>> End([FromBody] EndGameRequest request, CancellationToken cancellationToken)
            => Ok(await _service.EndAsync(request, cancellationToken));
    }
}
