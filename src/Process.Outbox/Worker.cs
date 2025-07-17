using System.Text;
using Domain;
using Infra.Database;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

namespace Process.Outbox;

public class Worker(IServiceProvider provider) : BackgroundService
{
    private readonly ConnectionFactory _factory = new ConnectionFactory
    {
        HostName = "localhost",
        Port = 5672,
        UserName = "rabbitmq",
        Password = "rabbitmq",
    };
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using IConnection connection = await _factory.CreateConnectionAsync(stoppingToken);
        using IChannel channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);
        using IServiceScope scope = provider.CreateScope();
        
        ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await channel.QueueDeclareAsync(
            queue: "outbox-queue",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: stoppingToken
        );

        while (!stoppingToken.IsCancellationRequested)
        {
            OutboxEvent? record = await context
                .OutboxEvents
                .Where(x => !x.Processed)
                .FirstOrDefaultAsync(stoppingToken);

            if (record is not null)
            {
                byte[] body = Encoding.UTF8.GetBytes(record.Payload);
            
                await channel.BasicPublishAsync(
                    exchange: string.Empty,
                    routingKey: "outbox-queue",
                    mandatory: true,
                    basicProperties: new BasicProperties { Persistent = true },
                    body: body,
                    cancellationToken: stoppingToken
                );

                Console.WriteLine($"Mensagem publicada: {record.Payload}");
                
                record.Processed = true;
                context.OutboxEvents.Update(record);
                await context.SaveChangesAsync(stoppingToken);
                
                Console.WriteLine($"Registro atualizado como processado: {record.Id}");
            }
            
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
