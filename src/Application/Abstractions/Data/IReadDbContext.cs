using Domain;
using MongoDB.Driver;

namespace Application.Abstractions.Data;

public interface IReadDbContext
{
    IMongoCollection<Book> Books { get; }
    IMongoCollection<Loan> Loans { get; }
}
