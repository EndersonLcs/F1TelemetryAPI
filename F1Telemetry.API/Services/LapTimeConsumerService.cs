using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using F1Telemetry.Domain.Entities;
using F1Telemetry.Infrastructure.Data;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.AspNetCore.SignalR;
using F1Telemetry.API.Hubs;

namespace F1Telemetry.API.Services;

public class LapTimeConsumerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<LapTimeConsumerService> _logger;
    private readonly IHubContext<F1TelemetryHub> _hubContext;
    private IConnection _connection = null!;
    private IModel _channel = null!;
    private const string QueueName = "f1_laptimes_queue";
    
    // Caches em memória: thread-safe e extremamente rápidos
    private readonly ConcurrentDictionary<int, bool> _knownDrivers = new();
    private readonly ConcurrentDictionary<int, bool> _knownSessions = new();
    
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public LapTimeConsumerService(IServiceProvider serviceProvider, ILogger<LapTimeConsumerService> logger, IHubContext<F1TelemetryHub> hubContext)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _hubContext = hubContext;
        InitRabbitMQ();
    }

    private void InitRabbitMQ()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            
            try
            {
                var lapData = JsonSerializer.Deserialize<LapTime>(message);
                if (lapData != null)
                {
                    lapData.Id = Guid.NewGuid();

                    using var scope = _serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<F1TelemetryDbContext>();

                    // 1. Verificação de Piloto
                    if (!_knownDrivers.ContainsKey(lapData.DriverNumber))
                    {
                        await _semaphore.WaitAsync();
                        try
                        {
                            if (!_knownDrivers.ContainsKey(lapData.DriverNumber))
                            {
                                var driver = await dbContext.Drivers.FindAsync(lapData.DriverNumber);
                                if (driver == null)
                                {
                                    dbContext.Drivers.Add(new Driver { DriverNumber = lapData.DriverNumber, FullName = "Piloto " + lapData.DriverNumber, TeamName = "Equipe Simulada" });
                                    await dbContext.SaveChangesAsync();
                                }
                                _knownDrivers.TryAdd(lapData.DriverNumber, true);
                            }
                        }
                        finally
                        {
                            _semaphore.Release();
                        }
                    }

                    // 2. Verificação de Sessão
                    if (!_knownSessions.ContainsKey(lapData.SessionKey))
                    {
                        await _semaphore.WaitAsync();
                        try
                        {
                            if (!_knownSessions.ContainsKey(lapData.SessionKey))
                            {
                                var session = await dbContext.Sessions.FindAsync(lapData.SessionKey);
                                if (session == null)
                                {
                                    dbContext.Sessions.Add(new Session { SessionKey = lapData.SessionKey, MeetingKey = 1, SessionName = "Simulação", DateStart = DateTime.UtcNow });
                                    await dbContext.SaveChangesAsync();
                                }
                                _knownSessions.TryAdd(lapData.SessionKey, true);
                            }
                        }
                        finally
                        {
                            _semaphore.Release();
                        }
                    }

                    // 3. Inserção Totalmente Livre!
                    dbContext.LapTimes.Add(lapData);
                    await dbContext.SaveChangesAsync();

                    // Envia a volta em tempo real para todos os clientes conectados
                    await _hubContext.Clients.All.SendAsync("ReceiveNewLap", lapData);

                    _logger.LogInformation("✅ Volta {Lap} do Piloto {Driver} processada e transmitida ao vivo!", lapData.LapNumber, lapData.DriverNumber);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erro ao processar mensagem da fila.");
            }
            
            _channel.BasicAck(ea.DeliveryTag, false);
        };

        _channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}
