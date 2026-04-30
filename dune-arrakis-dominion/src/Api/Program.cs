using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Api.Hubs;
using System.Text.Json.Serialization;

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

// Nota: registra tus servicios concretos según tus proyectos
// Ejemplo (ajusta nombres de clases y namespaces a tu solución):
// builder.Services.AddSingleton<Domain.Repositories.IGameRepository, Infrastructure.Persistence.JsonGameRepository>();
// builder.Services.AddScoped<Application.Services.GameService>();
// builder.Services.AddScoped<Application.Services.SimulationService>();
// builder.Services.AddScoped<Infrastructure.Persistence.PersistenceService>();

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