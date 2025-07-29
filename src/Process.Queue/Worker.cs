using Application.Abstractions.Data;
using Domain;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedKernel;
using SharedKernel.Constants;

namespace Process.Queue;

public class Worker : BackgroundService
{
    private readonly IReadDbContext _context;
    private readonly ConnectionFactory _factory = new ConnectionFactory
    {
        HostName = "localhost",
        Port = 5672,
        UserName = "rabbitmq",
        Password = "rabbitmq",
    };

    public Worker(IServiceProvider provider)
    {
        using IServiceScope scope = provider.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<IReadDbContext>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using IConnection connection = await _factory.CreateConnectionAsync(stoppingToken);
        using IChannel channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync(
            queue: QueuesName.Outbox,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: stoppingToken
        );

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (sender, eventArgs) =>
        {
            byte[] body = eventArgs.Body.ToArray();
            OutboxEvent? message = System.Text.Json.JsonSerializer.Deserialize<OutboxEvent>(body);

            // As this worker is now only for reading/sync, we just log the event or process as needed.
            // No MongoDB operations, just simulate the sync or log.
            switch (message!.Event)
            {
                case EventType.Created:
                    switch (message.Aggregate)
                    {
                        case AggregateType.Book:
                            {
                                Book book = System.Text.Json.JsonSerializer.Deserialize<Book>(message.Payload);
                                Console.WriteLine($"[SYNC] Book created: {book!.Id} - {book.Title}");
                                break;
                            }
                        case AggregateType.Loan:
                            {
                                Loan loan = System.Text.Json.JsonSerializer.Deserialize<Loan>(message.Payload);
                                Console.WriteLine($"[SYNC] Loan created: {loan!.Id} for Book {loan.BookId}");
                                break;
                            }
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case EventType.Updated:
                    switch (message.Aggregate)
                    {
                        case AggregateType.Book:
                            {
                                Book book = System.Text.Json.JsonSerializer.Deserialize<Book>(message.Payload);
                                Console.WriteLine($"[SYNC] Book updated: {book!.Id} - {book.Title}");
                                break;
                            }
                        case AggregateType.Loan:
                            {
                                Loan loan = System.Text.Json.JsonSerializer.Deserialize<Loan>(message.Payload);
                                Console.WriteLine($"[SYNC] Loan updated: {loan!.Id} for Book {loan.BookId}");
                                break;
                            }
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Console.WriteLine("Message processed successfully");
        };

        await channel.BasicConsumeAsync(queue: QueuesName.Outbox, autoAck: true, consumer, cancellationToken: CancellationToken.None);
    }
}