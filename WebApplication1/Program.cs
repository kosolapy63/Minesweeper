using Minesweeper;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Добавляем Swagger
services.AddSwaggerGen();

// Добавляем контроллеры
services.AddControllers();

services.AddSingleton<IMinessweeperService, MinessweeperService>();
var app = builder.Build();

// Если приложение в режиме разработки, активируем Swagger и Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Генерация Swagger документации
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); // Путь к Swagger JSON
        c.RoutePrefix = string.Empty;  // Swagger UI будет доступен по корневому URL
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Маршруты контроллеров
app.MapControllers();

app.Run();