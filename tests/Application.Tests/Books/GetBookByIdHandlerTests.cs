using Application.Books.GetById;
using Domain;
using MongoDB.Driver;
using Moq;
using SharedKernel;

namespace Application.Tests.Books;

public sealed class GetBookByIdHandlerTests : PartialBooksTests
{
    [Fact]
    public async Task Handle_ShouldReturnBook_WhenBookExists()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var book = new Book
        {
            Id = bookId,
            Title = "Test Book",
            Author = "Test Author",
            Publish = DateTimeOffset.UtcNow,
            TotalRemaining = 5
        };

        // Primeiro insere o livro no contexto de escrita
        WriteContext.Books.Add(book);
        await WriteContext.SaveChangesAsync();

        // Mock do cursor para retornar o resultado esperado
        var expectedResponse = new BookResponse
        {
            Id = bookId,
            Title = "Test Book",
            Author = "Test Author",
            Publish = book.Publish
        };

        var books = new List<Book> { book };
        var mockCursor = new Mock<IAsyncCursor<Book>>();
        mockCursor.Setup(x => x.Current).Returns(books);
        mockCursor.SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);

        var mockFind = new Mock<IFindFluent<Book, Book>>();
        mockFind.Setup(x => x.ToCursorAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockCursor.Object);

        MockBooksCollection.Setup(x => x.Find(
                It.IsAny<FilterDefinition<Book>>(),
                It.IsAny<FindOptions>()))
            .Returns(mockFind.Object);

        var query = new GetBookByIdQuery(bookId);
        var handler = new GetBookByIdQueryHandler(MockReadContext.Object);

        // Act
        Result<BookResponse> result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(bookId, result.Value.Id);
        Assert.Equal("Test Book", result.Value.Title);
        Assert.Equal("Test Author", result.Value.Author);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenBookNotFound()
    {
        // Arrange
        var bookId = Guid.NewGuid();

        var mockCursor = new Mock<IAsyncCursor<Book>>();
        mockCursor.Setup(x => x.Current).Returns(new List<Book>());
        mockCursor.Setup(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var mockFind = new Mock<IFindFluent<Book, Book>>();
        mockFind.Setup(x => x.ToCursorAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockCursor.Object);

        MockBooksCollection.Setup(x => x.Find(
                It.IsAny<FilterDefinition<Book>>(),
                It.IsAny<FindOptions>()))
            .Returns(mockFind.Object);

        var query = new GetBookByIdQuery(bookId);
        var handler = new GetBookByIdQueryHandler(MockReadContext.Object);

        // Act
        Result<BookResponse> result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
    }
}
