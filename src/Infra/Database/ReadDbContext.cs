using Application.Abstractions.Data;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infra.Database;

public sealed class ReadDbContext : DbContext, IReadDbContext
{
    public ReadDbContext(DbContextOptions<ReadDbContext> options)
        : base(options)
    {
    }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<Loan> Loans => Set<Loan>();
}