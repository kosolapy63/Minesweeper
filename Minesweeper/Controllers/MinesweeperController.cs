using Microsoft.AspNetCore.Mvc;
using Minesweeper.DTO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
        // GET: api/<MinesweeperController>
        [HttpPost]
        [Route("/new")]
        public ActionResult<GameResponse> NewGame([FromBody] NewGameRequest request)
        {
            try
            {
                return Ok(_minessweeperService.StartNewGame(request.width, request.height, request.mines_count));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Произошла ошибка на сервере. Пожалуйста, попробуйте позже." });
            }
        }
        [HttpPost]
        [Route("/turn")]
        public ActionResult<GameResponse> Turn([FromBody] GameTurnRequest request)
        {
            try
            {
                return Ok(_minessweeperService.GameTurn(request.game_id, request.col, request.row));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new               {
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Произошла ошибка на сервере. Пожалуйста, попробуйте позже." });
            }
        }


    }
}
