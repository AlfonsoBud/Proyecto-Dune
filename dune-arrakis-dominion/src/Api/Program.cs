using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text.Json.Serialization;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence.EF;
using Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Configuración CORS: sustituye por la URL de tu cliente Unity (WebGL o editor)
var unityOrigin = builder.Configuration["Unity:Origin"] ?? "http://localhost:8080";
builder.Services.AddCors(options =>
{
    options.AddPolicy("UnityClient", policy =>
    {
        policy.WithOrigins(unityOrigin)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Connection string desde appsettings.json
var conn = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=dune_game.db";

// Extraer ruta de fichero SQLite (format: Data Source=path)
var dataSourceKey = "Data Source=";
string dbPath = conn;
if (conn.IndexOf(dataSourceKey, System.StringComparison.OrdinalIgnoreCase) >= 0)
{
    dbPath = conn.Substring(conn.IndexOf(dataSourceKey, System.StringComparison.OrdinalIgnoreCase) + dataSourceKey.Length).Trim().Trim('"');
}

// Si el fichero no existe, fallar con mensaje claro (no crear DB ni tablas)
if (!File.Exists(dbPath))
{
    var msg = $"SQLite database not found at '{dbPath}'.\nPlease start Unity to allow it to create the database before running the API.";
    Console.Error.WriteLine(msg);
    throw new FileNotFoundException(msg, dbPath);
}

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(conn));

// Registrar repository concreto que implementa IGameRepository (Shared)
builder.Services.AddScoped<Shared.IGameRepository, Infrastructure.Persistence.SQLiteGameRepository>();

// Registrar servicios de aplicación ya existentes (implementaciones en Application)
builder.Services.AddScoped<Shared.IGameService, Application.Services.GameService>();
builder.Services.AddScoped<Shared.ISimulationService, Application.Services.SimulationService>();

// Registrar servicio de persistencia
builder.Services.AddScoped<Shared.IPersistenceService, PersistenceService.PersistenceService>();

// Registrar controllers y SignalR
builder.Services.AddControllers().AddJsonOptions(opts =>
{
    opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Autenticación (esqueleto) - configurar en producción
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Configura validación de tokens: Authority, Audience, TokenValidationParameters...
        options.Events ??= new JwtBearerEvents();
        options.RequireHttpsMetadata = false; // En entornos de desarrollo. En producción, true.
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("UnityClient");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();