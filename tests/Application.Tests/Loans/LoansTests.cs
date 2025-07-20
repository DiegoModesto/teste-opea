using Application.Abstractions.Data;
using Application.Loans;
using Application.Loans.ReturnLoan;
using Domain;
using Microsoft.EntityFrameworkCore;
using Moq;
using MockQueryable.Moq;
using SharedKernel;

namespace Application.Tests.Loans;

public sealed class LoansTests
{
    private Mock<IApplicationDbContext> Context { get; } = new();

    [Fact]
    public async Task Handler_ApplyLoan_ShouldReturnErrorWhenTheBookIsUnavailable()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var book = new Book
        {
            Id = bookId,
            Title = "Test Book",
            Author = "Test Author",
            Publish = DateTimeOffset.Now,
            TotalRemaining = 0 // Livro indisponível
        };

        var books = new List<Book> { book };
        Mock<DbSet<Book>>? mockDbSet = books.AsQueryable().BuildMockDbSet();

        Context.Setup(x => x.Books).Returns(mockDbSet.Object);

        var command = new ApplyLoanCommand(BookId: bookId);
        var handler = new ApplyLoanCommandHandler(Context.Object);

        // Act
        Result<string> result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(BookErrors.NoBookAvailable().Code, result.Error?.Code);
    }

    [Fact]
    public async Task Handler_ReturnLoan_ShouldReturnErrorWhenTheBookAlreadyReturned()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var loanId = Guid.NewGuid();
        var book = new Book
        {
            Id = bookId,
            Title = "Test Book",
            Author = "Test Author",
            Publish = DateTimeOffset.Now,
            TotalRemaining = 1
        };
        var loan = new Loan
        {
            Id = loanId,
            BookId = bookId,
            Status = LoanStatus.Completed // Livro já devolvido
        };
        
        var books = new List<Book> { book };
        var loans = new List<Loan> { loan };
        Mock<DbSet<Book>>? mockBooksDbSet = books.AsQueryable().BuildMockDbSet();
        Mock<DbSet<Loan>>? mockLoansDbSet = loans.AsQueryable().BuildMockDbSet();
        
        Context.Setup(x => x.Books).Returns(mockBooksDbSet.Object);
        Context.Setup(x => x.Loans).Returns(mockLoansDbSet.Object);
        
        var command = new ReturnLoanCommand(loanId);
        var handler = new ReturnLoanCommandHandler(Context.Object);
        
        // Act
        Result<string> result = await handler.Handle(command, CancellationToken.None);
        
        // Asset 
        Assert.False(result.IsSuccess);
        Assert.Equal(LoanErrors.AlreadyReturned(loanId).Code, result.Error?.Code);
    }
    
    [Fact]
    public async Task Handler_ReturnLoan_ShouldReturnErrorWhenTheBookIsOverdued()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var loanId = Guid.NewGuid();
        var book = new Book
        {
            Id = bookId,
            Title = "Test Book",
            Author = "Test Author",
            Publish = DateTimeOffset.Now,
            TotalRemaining = 1
        };
        var loan = new Loan
        {
            Id = loanId,
            BookId = bookId,
            Status = LoanStatus.Overdue // Livro em atraso
        };
        
        var books = new List<Book> { book };
        var loans = new List<Loan> { loan };
        Mock<DbSet<Book>>? mockBooksDbSet = books.AsQueryable().BuildMockDbSet();
        Mock<DbSet<Loan>>? mockLoansDbSet = loans.AsQueryable().BuildMockDbSet();
        
        Context.Setup(x => x.Books).Returns(mockBooksDbSet.Object);
        Context.Setup(x => x.Loans).Returns(mockLoansDbSet.Object);
        
        var command = new ReturnLoanCommand(loanId);
        var handler = new ReturnLoanCommandHandler(Context.Object);
        
        // Act
        Result<string> result = await handler.Handle(command, CancellationToken.None);
        
        // Asset 
        Assert.False(result.IsSuccess);
        Assert.Equal(LoanErrors.IsOverdued(loanId).Code, result.Error?.Code);
    }

    [Fact]
    public async Task Handler_ApplyLoan_ShouldReturnSuccessWhenApplyLoan()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var book = new Book
        {
            Id = bookId,
            Title = "Test Book",
            Author = "Test Author",
            Publish = DateTimeOffset.Now,
            TotalRemaining = 5 // Livro disponível
        };
    
        var books = new List<Book> { book };
        var loans = new List<Loan>();
        Mock<DbSet<Book>> mockBooksDbSet = books.AsQueryable().BuildMockDbSet();
        Mock<DbSet<Loan>> mockLoansDbSet = loans.AsQueryable().BuildMockDbSet();
    
        Context.Setup(x => x.Books).Returns(mockBooksDbSet.Object);
        Context.Setup(x => x.Loans).Returns(mockLoansDbSet.Object);
    
        var command = new ApplyLoanCommand(bookId);
        var handler = new ApplyLoanCommandHandler(Context.Object);
    
        // Act
        Result<string> result = await handler.Handle(command, CancellationToken.None);
    
        // Assert
        Assert.True(result.IsSuccess);
    }
}
