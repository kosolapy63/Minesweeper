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
        public MinesweeperController(IMinessweeperService minessweeperService ) {
            _minessweeperService= minessweeperService;
        }
        // GET: api/<MinesweeperController>
        [HttpPut]
        [Route("/new")]
        public GameResponse NewGame([FromBody] NewGameRequest request)
        {
            return _minessweeperService.StartNewGame(request.width, request.height, request.mines_count);
        }

        
    }
}
