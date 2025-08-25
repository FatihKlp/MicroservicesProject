using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Repositories;
using ProductService.Services;
using ProductService.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ProductService.CQRS.Queries.Categories;
using ProductService.CQRS.Queries.Products;
using ProductService.Middleware;
using Shared.Interfaces;
using Shared.Services;

// .env dosyasını yükle
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Log Service
builder.Services.AddHttpClient<ILogServiceClient, LogServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://logservice:8080");
});

// DbContext (PostgreSQL)
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
        ?? builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Database connection string is not configured");
    opt.UseNpgsql(connectionString);
});

// Redis Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    var redisConnection = Environment.GetEnvironmentVariable("REDIS_CONNECTION")
        ?? builder.Configuration.GetConnectionString("Redis")
        ?? "redis:6379";
    // Bağlantıyı toleranslı hale getir
    if (!redisConnection.Contains("abortConnect", StringComparison.OrdinalIgnoreCase))
    {
        redisConnection += ",abortConnect=false,connectRetry=3,connectTimeout=5000";
    }
    options.Configuration = redisConnection;
    options.InstanceName = "ProductService_";
});

// MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

// Authentication (JWT)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY")
            ?? builder.Configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT Key is not configured");

        var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
            ?? builder.Configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("JWT Issuer is not configured");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// Dependency Injection
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService.Services.ProductService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProductService API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "JWT Bearer token ekle (örnek: 'Bearer {token}')",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Exception Handling Middleware
app.UseExceptionHandlingMiddleware();

// Swagger'ı her ortamda aç
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProductService API v1");
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Otomatik migrasyon
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.MapControllers();
app.Run();