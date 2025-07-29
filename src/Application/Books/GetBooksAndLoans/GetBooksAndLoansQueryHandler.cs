using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Books.GetBooksAndLoans;

public sealed class GetBooksAndLoansQueryHandler(IReadDbContext context)
    : IQueryHandler<GetBooksAndLoansQuery, IEnumerable<BookAndLoansResponse>>
{
    public async Task<Result<IEnumerable<BookAndLoansResponse>>> Handle(GetBooksAndLoansQuery query, CancellationToken cancellationToken)
    {
        var books = await context.Books
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        if (books.Count == 0)
        {
            return Result.Failure<IEnumerable<BookAndLoansResponse>>(Domain.BookErrors.TheresNoLoans);
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
            // The BookAndLoansResponse is empty in the context, but assuming it should be similar to BooksAndLoansResponse in BooksAndLoans/List
            // If not, adjust accordingly.
            return new BookAndLoansResponse(); // Placeholder, as BookAndLoansResponse has no properties.
        }).ToList();

        return Result.Success<IEnumerable<BookAndLoansResponse>>(response);
    }
}