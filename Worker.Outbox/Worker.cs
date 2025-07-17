using System.Text;
using Domain;
using Microsoft.EntityFrameworkCore;
using Infra.Database;
using RabbitMQ.Client;

public sealed class Worker(
    ApplicationDbContext context
) : BackgroundService
{
    private readonly ConnectionFactory _fac = new ConnectionFactory
    {
        HostName = "localhost"
    };
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            OutboxEvent? record = await context
                .OutboxEvents
                .Where(x => !x.Processed)
                .FirstOrDefaultAsync(stoppingToken);

            if (record is not null)
            {
                IConnection connection = await _fac.CreateConnectionAsync(stoppingToken);
                IChannel channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);
                
                string message = System.Text.Json.JsonSerializer.Serialize(record);
                byte[] body = Encoding.UTF8.GetBytes(message);

                await channel.QueueDeclareAsync(cancellationToken: stoppingToken);
                await channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: "outbox",
                    body: body,
                    cancellationToken: stoppingToken
                );
                
                record.Processed = true;
                context.OutboxEvents.Update(record);
                await context.SaveChangesAsync(stoppingToken);
            }

            //Simulate a delay to avoid busy waiting
            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
        }
    }
}
