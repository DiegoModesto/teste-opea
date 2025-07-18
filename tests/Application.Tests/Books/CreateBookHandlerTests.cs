using Application.Abstractions.Data;
using Application.Books.Create;
using Domain;
using SharedKernel;
using Infra.Database;
using Microsoft.EntityFrameworkCore;

namespace Application.Tests.Books;

public sealed class CreateBookHandlerTests : IDisposable
{
    private readonly DbContextOptions<ApplicationDbContext> _contextOptions;
    private readonly ApplicationDbContext _context;

    public CreateBookHandlerTests()
    {
        _contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(_contextOptions);
    }

    [Fact]
    public async Task Handle_ShouldCreateBook_WhenValidRequest()
    {
        // Arrange
        var command = new CreateBookCommand(
            Title: "Test Book",
            Author: "Test Author",
            Publish: DateTime.UtcNow,
            Remaining: 5
        );

        var handler = new CreateBookCommandHandler(_context);

        // Act
        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);

        // Verifica se o livro foi criado no banco
        Book? book = await _context.Books.FirstOrDefaultAsync(b => b.Id == result.Value);
        Assert.NotNull(book);
        Assert.Equal("Test Book", book.Title);
        Assert.Equal("Test Author", book.Author);
        Assert.Equal(5, book.TotalRemaining);

        // Verifica se o evento do outbox foi criado
        OutboxEvent? outboxEvent = await _context.Set<OutboxEvent>()
            .FirstOrDefaultAsync(e => e.Id == result.Value);
        Assert.NotNull(outboxEvent);
        Assert.Equal(EventType.Created, outboxEvent.Event);
        Assert.Equal(AggregateType.Book, outboxEvent.Aggregate);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenTitleIsEmpty()
    {
        // Arrange
        var command = new CreateBookCommand(
            Title: string.Empty,
            Author: "Test Author",
            Publish: DateTime.UtcNow, 
            Remaining: 5
        );

        var handler = new CreateBookCommandHandler(_context);

        // Act
        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);

        // Verifica que nenhum livro foi criado
        int bookCount = await _context.Books.CountAsync();
        Assert.Equal(0, bookCount);

        // Verifica que nenhum evento do outbox foi criado
        int outboxEventCount = await _context.Set<OutboxEvent>().CountAsync();
        Assert.Equal(0, outboxEventCount);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenAuthorIsEmpty()
    {
        // Arrange
        var command = new CreateBookCommand(
            Title: "Test Book",
            Author: string.Empty,
            Publish: DateTime.UtcNow, 
            Remaining: 5
        );

        var handler = new CreateBookCommandHandler(_context);

        // Act
        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);

        // Verifica que nenhum livro foi criado
        int bookCount = await _context.Books.CountAsync();
        Assert.Equal(0, bookCount);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenTotalRemainingIsZero()
    {
        // Arrange
        var command = new CreateBookCommand(
            Title: "Test Book",
            Author: "Test Author",
            Publish: DateTime.UtcNow,
            Remaining: 0
        );

        var handler = new CreateBookCommandHandler(_context);

        // Act
        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);

        // Verifica que nenhum livro foi criado
        int bookCount = await _context.Books.CountAsync();
        Assert.Equal(0, bookCount);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}
