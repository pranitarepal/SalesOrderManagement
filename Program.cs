using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddSingleton<SalesOrderManagement.Services.FileParserFactory>();
builder.Services.AddHttpClient<SalesOrderManagement.Services.AI.GeminiAIService>();
builder.Services.AddTransient<SalesOrderManagement.Services.AI.IAIService, SalesOrderManagement.Services.AI.GeminiAIService>();

// Register Entity Framework Core (SQL Server)
builder.Services.AddDbContext<SalesOrderManagement.DataAccess.SalesOrderDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Business Services
builder.Services.AddScoped<SalesOrderManagement.BusinessLogic.Interfaces.IPurchaseOrderService, SalesOrderManagement.BusinessLogic.Implementations.PurchaseOrderService>();
builder.Services.AddScoped<SalesOrderManagement.BusinessLogic.Interfaces.IOrderService, SalesOrderManagement.Services.Implementations.OrderService>();
builder.Services.AddScoped<SalesOrderManagement.BusinessLogic.Interfaces.IUserService, SalesOrderManagement.Services.Implementations.UserService>();

// builder.Services.AddOpenApi(); // Using Swashbuckle instead
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

// Ensure Database is Created
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SalesOrderManagement.DataAccess.SalesOrderDbContext>();
    db.Database.EnsureCreated();
}

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
