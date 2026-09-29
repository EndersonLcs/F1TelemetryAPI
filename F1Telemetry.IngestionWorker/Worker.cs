using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace F1Telemetry.IngestionWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ConnectionFactory _factory;
    private const string QueueName = "f1_laptimes_queue";

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
        _factory = new ConnectionFactory { HostName = "localhost" };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Conecta ao RabbitMQ
        using var connection = _factory.CreateConnection();
        using var channel = connection.CreateModel();

        // Declara a fila (se ela não existir, o RabbitMQ cria)
        channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

        _logger.LogInformation("Worker de Ingestão iniciado. Conectado ao RabbitMQ.");

        int simulatedLap = 1;

        // Loop infinito simulando a corrida em tempo real
        while (!stoppingToken.IsCancellationRequested)
        {
            // 1. Simula os dados recebidos da pista
            var newLap = new
            {
                SessionKey = 9158,
                DriverNumber = 44,
                LapNumber = simulatedLap,
                LapDurationMs = new Random().Next(80000, 85000), // Tempo aleatório entre 1:20 e 1:25
                TireCompound = "SOFT",
                TireAge = simulatedLap,
                Timestamp = DateTime.UtcNow
            };

            // 2. Converte para JSON e depois para Bytes (formato exigido pelo RabbitMQ)
            string message = JsonSerializer.Serialize(newLap);
            var body = Encoding.UTF8.GetBytes(message);

            // 3. Publica na fila
            channel.BasicPublish(exchange: "", routingKey: QueueName, basicProperties: null, body: body);

            _logger.LogInformation("Volta {Lap} do Piloto {Driver} enviada para a fila! Tempo: {Time}ms", 
                newLap.LapNumber, newLap.DriverNumber, newLap.LapDurationMs);

            simulatedLap++;

            // Aguarda 5 segundos antes de mandar a próxima volta (simulando o tempo de pista)
            await Task.Delay(5000, stoppingToken);
        }
    }
}
