using Microsoft.AspNetCore.Mvc;
using Minesweeper.DTO;
using Minesweeper.Services;

namespace Minesweeper.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MinesweeperController : ControllerBase
    {
        private readonly IMinessweeperService _minessweeperService;
        public MinesweeperController(IMinessweeperService minessweeperService)
        {
            _minessweeperService = minessweeperService;
        }

        [HttpPost]
        [Route("/new")]
        public ActionResult<GameResponse> NewGame([FromBody] NewGameRequest request)
        {
            try
            {
                return Ok(_minessweeperService.StartNewGame(request.Width, request.Height, request.MinesCount));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost]
        [Route("/turn")]
        public ActionResult<GameResponse> Turn([FromBody] GameTurnRequest request)
        {
            try
            {
                return Ok(_minessweeperService.GameTurn(request.GameId, request.Collumn, request.Row));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
