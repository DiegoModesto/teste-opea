using Application.Abstractions.Data;
using Application.Books.Create;
using Domain;
using SharedKernel;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Application.Tests.Books;

public sealed class BooksTests
{
    private Mock<IApplicationDbContext> Context { get; } = new();

    [Fact]
    public async Task Handler_CreateBook_ShouldReturnErrorWhenCreateBookWithoutTitle()
    {
        // Arrange
        var command = new CreateBookCommand(
            Title: string.Empty,
            Author: "Author Name", 
            Publish: DateTime.Now, 
            Remaining: 10
        );
        var handler = new CreateBookCommandHandler(Context.Object);

        // Act
        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(BookErrors.BookTitleRequired.Code, result.Error?.Code);
    }
    
    [Fact]
    public async Task Handler_CreateBook_ShouldReturnErrorWhenCreateBookWithoutAuthor()
    {
        // Arrange
        var command = new CreateBookCommand(
            Title: "Book Title",
            Author: string.Empty, 
            Publish: DateTime.Now, 
            Remaining: 10
        );
        var handler = new CreateBookCommandHandler(Context.Object);

        // Act
        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(BookErrors.BookAuthorRequired.Code, result.Error?.Code);
    }
    
    [Fact]
    public async Task Handler_CreateBook_ShouldReturnSuccessWhenCreateABook()
    {
        // Arrange
        Context.Setup(c => c.Books)
            .Returns(new Mock<DbSet<Book>>().Object);
        
        var command = new CreateBookCommand(
            Title: "Book Title",
            Author: "Author Name", 
            Publish: DateTime.Now, 
            Remaining: 10
        );
        var handler = new CreateBookCommandHandler(Context.Object);

        // Act
        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }
}
