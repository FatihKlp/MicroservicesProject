using LogService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

// .env dosyasını yükle
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// DbContext (PostgreSQL) - Environment Variable'dan al
builder.Services.AddDbContext<LogDbContext>(options =>
{
    var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
        ?? throw new InvalidOperationException("CONNECTION_STRING environment variable is not set");
    options.UseNpgsql(connectionString);
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var serviceName = Environment.GetEnvironmentVariable("SERVICE_NAME") ?? "LogService";
    var serviceVersion = Environment.GetEnvironmentVariable("SERVICE_VERSION") ?? "v1";

    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = $"{serviceName} API",
        Version = serviceVersion,
        Description = "Mikroservis log yönetimi için API"
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "LogService API v1");
        c.RoutePrefix = string.Empty; // Swagger UI'ı root'da açmak için
    });
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();