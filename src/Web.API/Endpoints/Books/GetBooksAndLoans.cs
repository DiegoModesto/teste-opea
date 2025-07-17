using Application.Abstractions.Messaging;
using Application.Books.GetBooksAndLoans;
using SharedKernel;
using Web.API.Extensions;
using Web.API.Infrastructure;

namespace Web.API.Endpoints.Books;

public sealed class GetBooksAndLoans : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(pattern: "books/loans", async (
            IQueryHandler<GetBooksAndLoansQuery, IEnumerable<BookAndLoansResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetBooksAndLoansQuery();
            Result<IEnumerable<BookAndLoansResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Books");
    }
}
