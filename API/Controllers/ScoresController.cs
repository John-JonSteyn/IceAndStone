using IceAndStone.API.Requests;
using IceAndStone.API.Responses;
using IceAndStone.API.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace IceAndStone.API.Controllers
{
    [ApiController]
    [Route("api/scores")]
    public class ScoresController : ControllerBase
    {
        private readonly IScoreService _service;
        public ScoresController(IScoreService service) => _service = service;

        /// <summary>Records a team’s score for the current round.</summary>
        [HttpPost]
        public async Task<ActionResult<ScoreResponse>> Post([FromBody] PostTeamScoreRequest request, CancellationToken cancellationToken)
            => Ok(await _service.PostTeamScoreAsync(request, cancellationToken));
    }
}
