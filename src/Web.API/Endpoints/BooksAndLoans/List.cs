using Application.Abstractions.Messaging;
using Application.BooksAndLoans.List;
using SharedKernel;
using Web.API.Extensions;
using Web.API.Infrastructure;

namespace Web.API.Endpoints.BooksAndLoans;

public sealed class List : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/books-and-loans", async (
            IQueryHandler<BooksAndLoansQuery, IEnumerable<BooksAndLoansResponse>> handler,
            CancellationToken cancellationToken
            ) =>
        {
            var query = new BooksAndLoansQuery();
            Result<IEnumerable<BooksAndLoansResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(
                onSuccess: result => Results.Ok(result),
                CustomResults.Problem
            );
        })
        .WithTags(Tags.BooksAndLoans);
    }
}
