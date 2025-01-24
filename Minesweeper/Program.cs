using Minesweeper;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
builder.Services.AddSingleton<IMinessweeperService, MinessweeperService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("https://minesweeper-test.studiotg.ru") // Разрешаем только этот домен
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty;
    });
}
app.UseCors("AllowSpecificOrigin");
app.UseHttpsRedirection();
app.MapControllers();
app.Run();

