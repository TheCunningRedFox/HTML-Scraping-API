using Npgsql;
using Dapper;
using TestAssignment;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();

// TODO Убрать в отдельный класс
// НАЧАЛО БЛОКА: Автоматическая инициализация структуры БД через Dapper
try
{
    var dataSource = app.Services.GetRequiredService<NpgsqlDataSource>();
    await using var connection = await dataSource.OpenConnectionAsync();

    const string createTableSql = """
        CREATE TABLE IF NOT EXISTS elements (
            id BIGSERIAL PRIMARY KEY,
            value TEXT,
            html_code TEXT
        );
    """;

    await connection.ExecuteAsync(createTableSql);
    Console.WriteLine("---- База данных успешно инициализирована: таблица 'elements' готова ----");
}
catch (Exception ex)
{
    Console.WriteLine($"---- ОШИБКА ИНИЦИАЛИЗАЦИИ БД: {ex.Message} ----");
}
// КОНЕЦ БЛОКА

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "Web Scraping & Cryptography API v1");

        options.RoutePrefix = "api/swagger";
    });
}

app.MapGet("/", () => Results.Redirect("api/swagger"))
    .ExcludeFromDescription();

app.UseAuthorization();

app.MapControllers();

app.Run();
