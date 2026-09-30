using F1Telemetry.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using F1Telemetry.Domain.Interfaces;
using F1Telemetry.Infrastructure.Repositories;
using F1Telemetry.API.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configuração do Banco de Dados PostgreSQL
builder.Services.AddDbContext<F1TelemetryDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuração do Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
    options.InstanceName = "F1Telemetry_"; // Prefixo para as chaves
});

builder.Services.AddScoped<ILapTimeRepository, LapTimeRepository>();

// Adiciona os controllers e Swagger
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddSignalR();

// Configuração do CORS para permitir o Front-end (Live Server) conversar com o SignalR
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .SetIsOriginAllowed(_ => true) // Permite qualquer origem (como o seu 127.0.0.1:5500)
              .AllowCredentials(); // Obrigatório para conexões SignalR/WebSockets
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registra o Worker que ficará lendo a fila em segundo plano
builder.Services.AddHostedService<LapTimeConsumerService>();

var app = builder.Build();

app.UseCors("CorsPolicy");
// Configura o pipeline HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
// Mapeia a rota do WebSocket
app.MapHub<F1Telemetry.API.Hubs.F1TelemetryHub>("/hubs/telemetry");

app.Run();

// Expõe a classe Program para o projeto de testes de integração
public partial class Program { }
