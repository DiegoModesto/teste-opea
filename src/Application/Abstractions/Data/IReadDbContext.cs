using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Data;

public interface IReadDbContext
{
    DbSet<Book> Books { get; }
    DbSet<Loan> Loans { get; }
}