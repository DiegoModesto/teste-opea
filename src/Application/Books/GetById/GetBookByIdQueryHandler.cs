using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Books.GetById;

public sealed class GetBookByIdQueryHandler(
    IReadDbContext context)
    : IQueryHandler<GetBookByIdQuery, BookResponse>
{
    public async Task<Result<BookResponse>> Handle(GetBookByIdQuery query, CancellationToken cancellationToken)
    {
        var book = await context.Books
            .AsNoTracking()
            .Where(b => b.Id == query.Id)
            .Select(b => new BookResponse
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                Publish = b.Publish
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (book is null)
        {
            return Result.Failure<BookResponse>(BookErrors.NotFound(query.Id));
        }

        return Result.Success(book);
    }
}