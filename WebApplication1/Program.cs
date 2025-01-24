using Minesweeper;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// ��������� Swagger
services.AddSwaggerGen();

// ��������� �����������
services.AddControllers();

services.AddSingleton<IMinessweeperService, MinessweeperService>();
var app = builder.Build();

// ���� ���������� � ������ ����������, ���������� Swagger � Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // ��������� Swagger ������������
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); // ���� � Swagger JSON
        c.RoutePrefix = string.Empty;  // Swagger UI ����� �������� �� ��������� URL
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();

// �������� ������������
app.MapControllers();

app.Run();