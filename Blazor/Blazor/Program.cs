using Blazor;
using Blazor.Components;
using Blazor.Services; // <--- ЭТА СТРОКА ОБЯЗАНА БЫТЬ ЗДЕСЬ, В САМОМ НАЧАЛЕ
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Регистрируем сервис меню (ОДНОКРАТНО)
builder.Services.AddSingleton<MenuStateService>();

// Добавляем стандартный HttpClient
// builder.Services.AddHttpClient();

builder.Services.AddHostedService<ApiLauncherService>();

// Регистрируем отдельный клиент специально для MinesweeperApi
builder.Services.AddHttpClient("MinesweeperApi", client =>
{
	client.BaseAddress = new Uri("http://localhost:5000");
});

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

var app = builder.Build();

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