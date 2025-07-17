using Application.Abstractions.Messaging;
using Application.Books;
using Application.Books.GetById;
using SharedKernel;
using Web.API.Extensions;
using Web.API.Infrastructure;

namespace Web.API.Endpoints.Books;

public sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(pattern: "/books/{id:guid}", async (
            Guid id,
            IQueryHandler<GetBookByIdQuery, BookResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetBookByIdQuery(id);
            Result<BookResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(
                book => Results.Ok(book),
                CustomResults.Problem
            );
        })
        .WithTags("Books");
    }
}
