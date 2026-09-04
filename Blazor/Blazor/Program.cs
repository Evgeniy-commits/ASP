using Blazor;
using Blazor.Components;
using Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Diagnostics;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Регистрируем сервис меню (ОДНОКРАТНО)
builder.Services.AddSingleton<MenuStateService>();

// Добавляем стандартный HttpClient
// builder.Services.AddHttpClient();

//builder.Services.AddHostedService<ApiLauncherService>();

//// Регистрируем отдельный клиент специально для MinesweeperApi
//builder.Services.AddHttpClient("MinesweeperApi", client =>
//{
//	//client.BaseAddress = new Uri("http://localhost:5000");
//	client.BaseAddress = new Uri("http://192.168.1.105:5000");
//});

// --- Проверка удалённого API ---
string apiUrl = "http://localhost:5000";
bool useRemoteApi = false;

try
{
    using HttpClient testClient = new HttpClient();
    testClient.Timeout = TimeSpan.FromSeconds(3);
    HttpResponseMessage response = testClient.GetAsync("http://192.168.1.105:5000/api/game/state").Result;
    // Любой HTTP-ответ (даже 404/400) = API жив
    useRemoteApi = true;
    apiUrl = "http://192.168.1.105:5000";
    Console.WriteLine($"Удалённый API доступен: {apiUrl} (статус {response.StatusCode})");
}
catch (Exception ex)
{
    Console.WriteLine($"Удалённый API недоступен: {ex.Message}");
    Console.WriteLine("Запускаем локальный...");
}

builder.Services.AddHttpClient("MinesweeperApi", client =>
{
    client.BaseAddress = new Uri(apiUrl);
});

if (!useRemoteApi)
{
    builder.Services.AddHostedService<ApiLauncherService>();
}

// --- Настройки ---



//string remoteIp = "192.168.1.105";
//string remoteExePath = @"C:\Users\User\Desktop\out\MinesweeperAPI.exe";

//string remoteApiUrl = $"http://{remoteIp}:5000";
//string localApiUrl = "http://localhost:5000";

//string apiUrl = localApiUrl;
//bool useRemoteApi = false;

//// 1. Проверяем — может, API уже запущен на удалённом ПК
//if (RemoteApiLauncher.IsApiAlive(remoteApiUrl))
//{
//    apiUrl = remoteApiUrl;
//    useRemoteApi = true;
//    Console.WriteLine($"Удалённый API уже запущен: {remoteApiUrl}");
//}
//else
//{
//    Console.WriteLine("Удалённый API не отвечает. Пробуем запустить...");

//    // 2. Пробуем запустить .exe на удалённом ПК через WMI
//    bool started = RemoteApiLauncher.TryStartRemoteProcess(
//        remoteIp, remoteExePath);

//    if (started)
//    {
//        // Ждём, пока API поднимется
//        Console.WriteLine("Ждём загрузки удалённого API (3 сек)...");
//        Thread.Sleep(3000);

//        // 3. Проверяем снова
//        if (RemoteApiLauncher.IsApiAlive(remoteApiUrl))
//        {
//            apiUrl = remoteApiUrl;
//            useRemoteApi = true;
//            Console.WriteLine($"Удалённый API успешно запущен: {remoteApiUrl}");
//        }
//        else
//        {
//            Console.WriteLine("WMI запустил процесс, но API не отвечает. Используем локальный.");
//        }
//    }
//    else
//    {
//        Console.WriteLine("Не удалось запустить удалённо (комп выключен или WMI недоступен).");
//    }
//}

//// 4. Регистрируем HttpClient
//builder.Services.AddHttpClient("MinesweeperApi", client =>
//{
//    client.BaseAddress = new Uri(apiUrl);
//});

//// 5. Локальный .exe — только если удалённый не сработал
//if (!useRemoteApi)
//{
//    builder.Services.AddHostedService<ApiLauncherService>();
//}

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

WebApplication app = builder.Build();

// Принудительно говорим Kestrel слушать все интерфейсы
//app.Urls.Add("http://0.0.0.0:5000");
//app.Urls.Add("https://0.0.0.0:7102");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

app.Run();