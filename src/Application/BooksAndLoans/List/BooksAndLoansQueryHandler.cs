using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.BooksAndLoans.List;

public sealed class BooksAndLoansQueryHandler(IReadDbContext context) : IQueryHandler<BooksAndLoansQuery, IEnumerable<BooksAndLoansResponse>>
{
    public async Task<Result<IEnumerable<BooksAndLoansResponse>>> Handle(BooksAndLoansQuery query, CancellationToken cancellationToken)
    {
        var books = await context.Books
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        if (books.Count == 0)
        {
            return Result.Failure<IEnumerable<BooksAndLoansResponse>>(Domain.BookErrors.TheresNoLoans);
        }

        var bookIds = books.Select(b => b.Id).ToList();

        var loans = await context.Loans
            .AsNoTracking()
            .Where(l => bookIds.Contains(l.BookId))
            .ToListAsync(cancellationToken);

        var loansByBook = loans
            .GroupBy(l => l.BookId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var response = books.Select(book =>
        {
            var bookLoans = loansByBook.TryGetValue(book.Id, out var bl) ? bl : new List<Domain.Loan>();
            var loanInfos = bookLoans.Select(loan => new LoanInfo(
                Id: loan.Id,
                Status: loan.Status,
                LoanDate: loan.CreatedAt.UtcDateTime,
                ReturnDate: loan.ReturnDate?.UtcDateTime
            )).ToList();

            return new BooksAndLoansResponse(
                BookId: book.Id,
                Title: book.Title,
                Author: book.Author,
                Publish: book.Publish.UtcDateTime,
                Loans: loanInfos
            );
        }).ToList();

        return Result.Success<IEnumerable<BooksAndLoansResponse>>(response);
    }
}