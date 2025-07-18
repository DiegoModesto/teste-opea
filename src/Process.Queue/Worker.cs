using Application.Abstractions.Data;
using Domain;
using Infra.Database;
using MongoDB.Driver;
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

            switch (message!.Event)
            {
                case EventType.Created:
                    switch (message.Aggregate)
                    {
                        case AggregateType.Book:
                            {
                                Book book = System.Text.Json.JsonSerializer.Deserialize<Book>(message.Payload);
                                await AddBookAsync(book!, stoppingToken);
                                break;
                            }
                        case AggregateType.Loan:
                            {
                                Loan loan = System.Text.Json.JsonSerializer.Deserialize<Loan>(message.Payload);
                                await AddLoanAsync(loan!, stoppingToken);
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
                                await UpdateBookAsync(book!, stoppingToken);
                                break;
                            }
                        case AggregateType.Loan:
                            {
                                Loan loan = System.Text.Json.JsonSerializer.Deserialize<Loan>(message.Payload);
                                await UpdateLoanAsync(loan!, stoppingToken);
                                break;
                            }
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            await ((AsyncEventingBasicConsumer)sender).Channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false, stoppingToken);
            
            Console.WriteLine("Message processed successfully");
        };

        await channel.BasicConsumeAsync(queue: QueuesName.Outbox, autoAck: false, consumer, cancellationToken: stoppingToken);
    }

    private async Task AddBookAsync(Book book, CancellationToken stoppingToken)
    {
        try
        {
            var opt = new InsertOneOptions
            {
                BypassDocumentValidation = true,
                Comment = "Book added by Worker"
            };
            await _context.Books.InsertOneAsync(book!, opt, stoppingToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    private async Task AddLoanAsync(Loan loan, CancellationToken stoppingToken)
    {
        try
        {
            var opt = new InsertOneOptions
            {
                BypassDocumentValidation = true,
                Comment = "Loan of book added by Worker"
            };
            await _context.Loans.InsertOneAsync(document: loan, opt, stoppingToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    private async Task UpdateLoanAsync(Loan loan, CancellationToken stoppingToken)
    {
        FilterDefinition<Loan>? filter = Builders<Loan>.Filter.Eq(l => l.Id, loan.Id);
        await _context.Loans.ReplaceOneAsync(filter, loan, cancellationToken: stoppingToken);
    }
    private async Task UpdateBookAsync(Book book, CancellationToken stoppingToken)
    {
        FilterDefinition<Book>? filter = Builders<Book>.Filter.Eq(b => b.Id, book.Id);
        await _context.Books.ReplaceOneAsync(filter, book, cancellationToken: stoppingToken);
    }
}

