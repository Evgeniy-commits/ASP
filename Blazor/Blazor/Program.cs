using Blazor.Components;

var builder = WebApplication.CreateBuilder(args);

//1.Добавляем стандартный HttpClient
builder.Services.AddHttpClient();

////2.Регистрируем отдельный клиент специально для MinesweeperApi
builder.Services.AddHttpClient("MinesweeperApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7273");
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
