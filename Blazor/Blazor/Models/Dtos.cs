namespace Blazor.Models
{
	public class StartGameRequest
	{
		public int Rows { get; set; }
		public int Cols { get; set; }
		public int Mines { get; set; }
	}

	public class CoordinateRequest
	{
		public int Row { get; set; }
		public int Col { get; set; }
	}

	// Это то, что видит Blazor! Структура должна совпадать 1 в 1
	public class GameStateResponse
	{
		public bool IsWon { get; set; }
		public bool IsGameOver { get; set; }
		// Важно: именно Grid (с большой буквы), так как в C# принято PascalCase
		public List<List<CellStateDto>> Grid { get; set; } = new();
	}

	public class CellStateDto
	{
		public int Row { get; set; }
		public int Col { get; set; }
		public bool IsRevealed { get; set; }
		public bool IsFlagged { get; set; }
		public bool IsMine { get; set; }
		public int NeighborCount { get; set; }
	}
}
