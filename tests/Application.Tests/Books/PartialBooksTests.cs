using Infra.Database;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions.Data;
using Domain;
using MongoDB.Driver;
using Moq;

namespace Application.Tests.Books;

public abstract class PartialBooksTests : IDisposable
{
    protected DbContextOptions<ApplicationDbContext> WriteContextOptions { get; }
    protected ApplicationDbContext WriteContext { get; }
    
    // Mocks para MongoDB
    protected Mock<IReadDbContext> MockReadContext { get; }
    protected Mock<IMongoCollection<Book>> MockBooksCollection { get; }
    protected Mock<IMongoCollection<Loan>> MockLoansCollection { get; }

    protected PartialBooksTests()
    {
        // Configuração do contexto de escrita (PostgreSQL)
        WriteContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        WriteContext = new ApplicationDbContext(WriteContextOptions);

        // Configuração dos mocks para MongoDB
        MockBooksCollection = new Mock<IMongoCollection<Book>>();
        MockLoansCollection = new Mock<IMongoCollection<Loan>>();
        MockReadContext = new Mock<IReadDbContext>();

        MockReadContext.Setup(x => x.Books).Returns(MockBooksCollection.Object);
        MockReadContext.Setup(x => x.Loans).Returns(MockLoansCollection.Object);
    }

    public void Dispose()
    {
        WriteContext?.Dispose();
    }
}
