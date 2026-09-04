using Blazor.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;

namespace Blazor.Pages
{
	public partial class Minesweeper : ComponentBase
	{
		[Inject] IHttpClientFactory ClientFactory { get; set; } = default!;

		HttpClient _httpClient = default!;

		GameStateResponse? gameState;
		bool isLoading = false;
		string errorMessage = "";

		int Rows = 10;
		int Cols = 10;
		int Mines = 15;

		protected override void OnInitialized()
		{
			_httpClient = ClientFactory.CreateClient("MinesweeperApi");
		}

		async Task StartGame()
		{
			errorMessage = "";
			Console.WriteLine("Start");
			isLoading = true;
			var request = new { Rows, Cols, Mines };

			try
			{
				var response = await _httpClient.PostAsJsonAsync("api/game/start", request);

				if (!response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					throw new Exception($"API error: {response.StatusCode} - {content}");
				}

				gameState = await response.Content.ReadFromJsonAsync<GameStateResponse>();
				StateHasChanged();
			}
			catch (HttpRequestException ex)
			{
				errorMessage = $"Net error (CORS или недоступен API): + {ex.Message}";
				Console.WriteLine(errorMessage);
			}
			catch (Exception ex)
			{
				errorMessage = ex.Message;
				Console.WriteLine(errorMessage);
			}
			finally
			{
				isLoading = false;
			}
		}

		async Task RestartGame()
		{
			errorMessage = "";
			isLoading = true;
			try
			{
				if (gameState == null)
				{
					await StartGame();
					return;
				}

				HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/game/restart", new { });
				gameState = await response.Content.ReadFromJsonAsync<GameStateResponse>();
				StateHasChanged();
			}
			catch (Exception ex)
			{
				errorMessage = "Restart error: " + ex.Message;
			}
			finally
			{
				isLoading = false;
			}
		}

		async Task FetchState()
		{
			try
			{
				gameState = await _httpClient.GetFromJsonAsync<GameStateResponse>("api/game/state");
				StateHasChanged();
			}
			catch (Exception ex)
			{
				errorMessage = "State error: " + ex.Message;
			}
		}

		async Task HandleClick(int r, int c, bool isChord)
		{
			if (gameState?.IsGameOver == true || gameState?.IsWon == true) return;

			CellStateDto cell = gameState!.Grid[r][c];

			try
			{
				if (cell.IsRevealed && !cell.IsMine)
				{
					await _httpClient.PostAsJsonAsync("api/game/chord", new { Row = r, Col = c });
				}
				else
				{
					await _httpClient.PostAsJsonAsync("api/game/reveal", new { Row = r, Col = c });
				}
				await FetchState();
			}
			catch (Exception ex)
			{
				errorMessage = "Move error" + ex.Message;
			}
		}

		async Task HandleRightClick(MouseEventArgs e, int r, int c)
		{
			if (gameState?.IsGameOver == true || gameState?.IsWon == true) return;

			try
			{
				await _httpClient.PostAsJsonAsync("api/game/toggle-flag", new { Row = r, Col = c });
				await FetchState();
			}
			catch (Exception ex)
			{
				errorMessage = "Ошибка флага: " + ex.Message;
			}
		}

		void ClearError() => errorMessage = "";

		// --- UI МЕТОДЫ (ОБНОВЛЕНЫ ДЛЯ РАБОТЫ С КЛАССАМИ) ---

		string GetGridStyle()
		{
			int cellSize = 20;
			int gap = 2;
			// Расчет общей ширины для центрирования и ограничения роста
			int totalWidth = (Cols * cellSize) + ((Cols - 1) * 2 * gap);
			int totalHeight = (Rows * cellSize) + ((Rows - 1) * 2 * gap);

			return $"display: grid; " +
                   $"grid-template-columns: repeat({Cols}, {cellSize}px); " +
				   $"grid-template-rows: repeat({Rows}, {cellSize}px); " +
				   $"gap: {gap}px; " +
                   $"border: solid 2px #333; " +
                   $"padding: 10px; " +
				   $"width: {totalWidth}px; " +
				   $"height: {totalHeight}px; " +
				   $"max-width: 100%; " +
                   $"overflow-x: hidden; " +
				   $"box-sizing: border-box; " +
				   $"justify-content: center; " +
                   $"align-content: center;" + 
				   $"overflow: visible";
		}

		string GetCellStyle(CellStateDto cell)
		{
			if (gameState == null) return "background-color: gray; width: 20px; height: 20px; display: flex; justify-content: center; align-items: center; cursor: pointer; user-select: none;";

			string baseStyle = "width: 20px; height: 20px; display: flex; justify-content: center; align-items: center; cursor: pointer; user-select: none; border-radius: 4px;";

			if (cell.IsRevealed)
			{
				// Эффект вдавленной кнопки для открытой ячейки
				baseStyle += "background-color: white; color: #333; " +
							 "box-shadow: inset 2px 2px 3px rgba(0,0,0,0.2), inset -2px -2px 3px rgba(255,255,255,0.8);";

				    if (!gameState.IsWon)
					{
						baseStyle += " background-color: lightgreen;"; // Зеленый, если победа
					}
					else
					{
						baseStyle += " background-color: red;";
					} 
				
			}
			else if (cell.IsFlagged)
			{
				// Эффект выпуклости для флага
				baseStyle += "background-color: green; color: black; " +
							 "box-shadow: 2px 2px 2px rgba(0,0,0,0.3), -1px -1px 2px rgba(255,255,255,0.5);";
			}
			else
			{
				// Эффект выпуклости для закрытой ячейки
				baseStyle += "background-color: #e0e0e0; color: #555; " +
					 "box-shadow: 2px 2px 2px rgba(0,0,0,0.4), -1px -1px 2px rgba(255,255,255,0.6);";
			}

			return baseStyle;
		}

		string GetNumberColor(int number)
		{
			return number switch
			{
				1 => "blue",
				2 => "green",
				3 => "red",
				4 => "darkcyan",
				5 => "brown",
				6 => "teal",
				7 => "black",
				8 => "darkgray",
				_ => "black"
			};
		}

		int CalculateMines()
		{
			if (gameState == null) return Mines;
			int flags = 0;
			foreach (var row in gameState.Grid)
				foreach (var cell in row)
					if (cell.IsFlagged) flags++;

			return Mines - flags;
		}
	}
}