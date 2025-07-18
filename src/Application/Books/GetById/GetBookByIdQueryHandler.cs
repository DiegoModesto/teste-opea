using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using SharedKernel;

namespace Application.Books.GetById;

public sealed class GetBookByIdQueryHandler(
    IReadDbContext context)
: IQueryHandler<GetBookByIdQuery, BookResponse>
{
    public async Task<Result<BookResponse>> Handle(GetBookByIdQuery query, CancellationToken cancellationToken)
    {
        FilterDefinition<Book>? filter = Builders<Book>.Filter.Eq(l => l.Id, query.Id);

        BookResponse? book = await context.Books
            .Find(filter)
            .Project(b => new BookResponse{
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                Publish = b.Publish
            })
            .FirstOrDefaultAsync(cancellationToken);
            
        return book ?? Result.Failure<BookResponse>(error: BookErrors.NotFound(query.Id));
    }
}
