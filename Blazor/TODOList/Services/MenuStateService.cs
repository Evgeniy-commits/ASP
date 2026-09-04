using Microsoft.AspNetCore.Components;

namespace TODOList.Services
{
    public class MenuStateService
    {
        bool _isOpen = false;
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
