using Domain;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Data;

public interface IApplicationDbContext
{
    DbSet<Book> Books { get; }
    DbSet<Loan> Loans { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
