using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Api.Hubs;
using System.Text.Json.Serialization;
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
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(conn));

// Registrar repository concreto que implementa IGameRepository
builder.Services.AddScoped<IGameRepository, SQLiteGameRepository>();

// Registrar servicios de aplicación ya existentes
builder.Services.AddScoped<Application.Services.IGameService, Application.Services.GameService>();
builder.Services.AddScoped<Application.Services.ISimulationService, Application.Services.SimulationService>();

// Registrar controllers y SignalR
builder.Services.AddControllers().AddJsonOptions(opts =>
{
    opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});
builder.Services.AddSignalR();
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
app.MapHub<GameHub>("/gamehub");

app.Run();