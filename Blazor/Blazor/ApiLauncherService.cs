using System.Diagnostics;

namespace Blazor;

public class ApiLauncherService : IHostedService
{
    private Process? _apiProcess;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        string exePath = @"C:\Users\Admin\source\repos\ASP\MinesweeperApi\out\MinesweeperAPI.exe";

        if (!File.Exists(exePath))
        {
            Console.WriteLine($"API exe не найден: {exePath}");
            return Task.CompletedTask;
        }

        KillAllMinesweeperApiProcesses();

        _apiProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = exePath,
                WorkingDirectory = Path.GetDirectoryName(exePath)!,
                UseShellExecute = false,
                CreateNoWindow = false
            },
            EnableRaisingEvents = true
        };

        // Собираем ключи без LINQ
        List<string> keysToRemove = new List<string>();
        foreach (string? key in _apiProcess.StartInfo.EnvironmentVariables.Keys)
        {
            if (key == null) continue;
            if (key.StartsWith("ASPNETCORE_", StringComparison.OrdinalIgnoreCase) ||
                key.StartsWith("DOTNET_", StringComparison.OrdinalIgnoreCase))
            {
                keysToRemove.Add(key);
            }
        }

        foreach (string? key in keysToRemove)
        {
            _apiProcess.StartInfo.EnvironmentVariables.Remove(key);
            Console.WriteLine($"Удалена переменная: {key}");
        }

        _apiProcess.StartInfo.EnvironmentVariables["ASPNETCORE_ENVIRONMENT"] = "Production";
        _apiProcess.StartInfo.EnvironmentVariables["ASPNETCORE_URLS"] = "http://localhost:5000";

        try
        {
            _apiProcess.Start();
            Console.WriteLine($"API запущен, PID: {_apiProcess.Id}");
            Thread.Sleep(1000);

            if (_apiProcess.HasExited)
                Console.WriteLine($"API упал! Код: {_apiProcess.ExitCode}");
            else
                Console.WriteLine($"API живёт, PID: {_apiProcess.Id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка запуска API: {ex.Message}");
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        try { if (_apiProcess != null && !_apiProcess.HasExited) _apiProcess.Kill(); } catch { }
        return Task.CompletedTask;
    }

    private void KillAllMinesweeperApiProcesses()
    {
        foreach (Process? p in Process.GetProcessesByName("MinesweeperApi"))
        {
            try { p.Kill(); p.WaitForExit(1000); } catch { }
        }
    }
}