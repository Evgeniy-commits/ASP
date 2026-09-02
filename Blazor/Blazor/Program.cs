using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Blazor.Components;
using Blazor.Services; // <--- ÝÒÀ ÑÒÐÎÊÀ ÎÁßÇÀÍÀ ÁÛÒÜ ÇÄÅÑÜ, Â ÑÀÌÎÌ ÍÀ×ÀËÅ

var builder = WebApplication.CreateBuilder(args);

// Ðåãèñòðèðóåì ñåðâèñ ìåíþ (ÎÄÍÎÊÐÀÒÍÎ)
builder.Services.AddSingleton<MenuStateService>();

// Äîáàâëÿåì ñòàíäàðòíûé HttpClient
// builder.Services.AddHttpClient();

// Ðåãèñòðèðóåì îòäåëüíûé êëèåíò ñïåöèàëüíî äëÿ MinesweeperApi
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