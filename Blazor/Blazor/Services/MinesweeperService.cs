using Blazor.Data;
using Blazor.Models;
using Microsoft.EntityFrameworkCore;

namespace Blazor.Services
{
    public class MinesweeperService
    {
        readonly BlazorContext _context;

        public MinesweeperService(BlazorContext context)
        {
            _context = context;
        }

        //сохранить результат игры
        public async Task SaveResultAsync(string playerName, int seconds)
        {
            Minesweeper? result = new Minesweeper
            {
                PlayerName = playerName,
                Seconds = seconds, 
                CreatedAt = DateTime.Now
            };

            _context.Minesweepers.Add(result);
            await _context.SaveChangesAsync();
        }

        //Сегодня
        public async Task<List<Minesweeper>> GetTodayResultAsync()
        {
            DateTime today = DateTime.Now.Date;
            DateTime tomorrow = today.AddDays(1);
            return await _context.Minesweepers
                .Where(x => x.CreatedAt >= today && x.CreatedAt < tomorrow)
                .OrderBy(x => x.Seconds)
                .ToListAsync();
        }

        //Неделя
        public async Task<List<Minesweeper>> GetWeekResultAsync()
        {
            DateTime today = DateTime.Now.Date;
            DateTime startOfWeek = today.AddDays(-(int)today.DayOfWeek);
            DateTime endOfWeek = startOfWeek.AddDays(7);
            return await _context.Minesweepers
                .Where(x => x.CreatedAt >= startOfWeek && x.CreatedAt < endOfWeek)
                .OrderBy(x => x.Seconds)
                .ToListAsync();
        }

        //Месяц
        public async Task<List<Minesweeper>> GetMonthResultAsync()
        {
            DateTime today = DateTime.Now.Date;
            DateTime startOfMonth = new DateTime(today.Year, today.Month, 1);
            DateTime endOfMonth = startOfMonth.AddMonths(1);
            return await _context.Minesweepers
                .Where(x => x.CreatedAt >= startOfMonth && x.CreatedAt < endOfMonth)
                .OrderBy(x => x.Seconds)
                .ToListAsync();
        }

        //Top-50
        public async Task<List<Minesweeper>> GetTopResultAsync(int count = 50)
        {
            return await _context.Minesweepers
                .OrderBy(x => x.Seconds)
                .Take(count)
                .ToListAsync();
        }
    }
}
