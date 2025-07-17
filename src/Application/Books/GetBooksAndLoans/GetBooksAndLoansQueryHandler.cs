using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Books.GetBooksAndLoans;

public sealed class GetBooksAndLoansQueryHandler(IApplicationDbContext context)
    : IQueryHandler<GetBooksAndLoansQuery, IEnumerable<BookAndLoansResponse>>
{
    public async Task<Result<IEnumerable<BookAndLoansResponse>>> Handle(GetBooksAndLoansQuery query, CancellationToken cancellationToken)
    {
        /*IEnumerable<BookAndLoansResponse> booksAndLoans = await context
            .Books
            .ToListAsync(cancellationToken);

        return Result.Success(booksAndLoans);*/

        //TODO: Converter para chamada de mongoDB
        return null;
    }
}
