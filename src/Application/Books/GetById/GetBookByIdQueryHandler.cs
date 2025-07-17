using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Books.GetById;

public sealed class GetBookByIdQueryHandler(
    IApplicationDbContext context)
: IQueryHandler<GetBookByIdQuery, BookResponse>
{
    public async Task<Result<BookResponse>> Handle(GetBookByIdQuery query, CancellationToken cancellationToken)
    {
        BookResponse? book = await context.Books
            .Where(b => b.Id == query.Id)
            .Select(b => new BookResponse
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                Publish = b.Publish
            })
            .SingleOrDefaultAsync(cancellationToken);

        return book ?? Result.Failure<BookResponse>(error: UserErrors.NotFound(query.Id));
    }
}
