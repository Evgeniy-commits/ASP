var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddControllers();
builder.Services.AddControllers().AddJsonOptions(options =>
{
	options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
	options.JsonSerializerOptions.PropertyNamingPolicy = null; // Отключаем автоматическую конвертацию в camelCase
});

//Разрешаем CORS для Blazor проекта
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
    {
        policy.WithOrigins("https://localhost:7102").AllowAnyHeader().AllowAnyMethod();
    });
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowBlazor");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
