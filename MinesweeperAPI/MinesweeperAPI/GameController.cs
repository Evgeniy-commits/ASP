using Microsoft.AspNetCore.Mvc;
using MinesweeperProxyLib;
using System.Collections.Generic;
using MinesweeperAPI.Models;

namespace MinesweeperAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]

	public class GameController : ControllerBase
	{
		//Состояние игры в памяти
		private static GameModel? _game;

		[HttpPost("start")]
		public IActionResult StartGame([FromBody] StartGameRequest request) 
		{
			try
			{
				_game = new GameModel(request.Rows, request.Cols, request.Mines);
				return GetState();
			}
			catch (Exception ex)
			{
				return BadRequest(new { success = false, error = ex.Message });
			}
		}

		[HttpGet("state")]
		public IActionResult GetState()
		{
			if (_game == null) return NotFound(new { success = false, error = "Game not started" });

			//var grid = new List<List<CellStateDto>>();

			GameStateResponse response = new GameStateResponse
			{
				IsWon = _game.IsWonProperty,
				IsGameOver = _game.IsGameOverProperty,
				Grid = new List<List<CellDto>>()
			};

			for (int r = 0; r < _game.RowsProperty; r++)
			{
				List<CellDto> row = new List<CellDto>();
				for (int c = 0;  c < _game.ColsProperty; c++)
				{
					var state = _game.GetState(r, c);
					row.Add(new CellDto
					{
						Row = r,
						Col = c,
						IsRevealed = state == CellState.Revealed,
						IsFlagged = state == CellState.Flagged,
						IsMine = _game.IsMine(r, c),
						NeighborCount = _game.GetNeighborCount(r, c),
						//IsGameOver = _game.IsGameOverProperty,
						//IsWon = _game.IsWonProperty
					});
				}
				response.Grid.Add(row);
			}
			return Ok(response);
		}

		[HttpPost("reveal")]
		public IActionResult Reveal([FromBody] CoordinateRequest req)
		{
			if (_game == null) return NotFound();
			_game.Reveal(req.Row, req.Col);
			return GetState();
		}

		[HttpPost("chord")]
		public IActionResult Chord([FromBody] CoordinateRequest req)
		{
			if (_game == null) return NotFound();
			_game.Chord(req.Row, req.Col);
			return GetState();
		}

		[HttpPost("toggle-flag")]
		public IActionResult ToggleFlag([FromBody] CoordinateRequest req)
		{
			if (_game == null) return NotFound();
			_game.ToggleFlag(req.Row, req.Col);
			return GetState();
		}

		[HttpPost("restart")]
		public IActionResult Restart()
		{
			if (_game == null) return NotFound();
			_game.Restart();
			return GetState();
		}

		//public class StartGameRequest
		//{
		//	public int Rows { get; set; }
		//	public int Cols { get; set; }
		//	public int Mines { get; set; }
		//}

		//public class CoordinateRequest
		//{
		//	public int Row { get; set; }
		//	public int Col { get; set; }
		//}

		//public class CellStateDto
		//{
		//	public int Row { get; set; }
		//	public int Col { get; set; }
		//	public bool IsRevealed { get; set; }
		//	public bool IsFlagged { get; set; }
		//	public bool IsMine { get; set; }
		//	public int NeighborCount { get; set; }
		//	public bool IsGameOver { get; set; }
		//	public bool IsWon {  get; set; }
		//}
	}
}
