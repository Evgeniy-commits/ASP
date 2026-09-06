using Blazor.Services;
using Blazor.Models;


namespace Blazor.Components.Pages
{
    public partial class Leaderboard
    {
        private List<Minesweeper>? _results;
        private string _activeTab = "today";
        private bool _loading = true;

        protected override async Task OnInitializedAsync() => await LoadData();

        private async Task LoadData()
        {
            _loading = true;
            try
            {
                switch (_activeTab)
                {
                    case "today": _results = await Service.GetTodayResultAsync(); break;
                    case "week": _results = await Service.GetWeekResultAsync(); break;
                    case "month": _results = await Service.GetMonthResultAsync(); break;
                    default: _results = await Service.GetTopResultAsync(50); break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                _results = new List<Minesweeper>();
            }
            _loading = false;
        }

        private async Task SwitchTab(string tab)
        {
            _activeTab = tab;
            await LoadData();
        }
    }
}
