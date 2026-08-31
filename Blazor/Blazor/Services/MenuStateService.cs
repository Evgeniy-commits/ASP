using Microsoft.AspNetCore.Components;

namespace Blazor.Services
{	public class MenuStateService
	{
		private bool _isOpen = true;
		public bool IsOpen => _isOpen;

		// Событие, на которое подпишется MainLayout
		public event Action? OnChange;

		public void Toggle()
		{
			_isOpen = !_isOpen;
			System.Console.WriteLine($"[Service] Состояние изменено на: {_isOpen}");
			OnChange?.Invoke(); // Уведомляем всех подписчиков
		}

		public void Open() { _isOpen = true; OnChange?.Invoke(); }
		public void Close() { _isOpen = false; OnChange?.Invoke(); }
	}
}
