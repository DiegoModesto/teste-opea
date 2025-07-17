using Application.Abstractions.Messaging;
using Application.Loans;
using SharedKernel;
using Web.API.Extensions;
using Web.API.Infrastructure;

namespace Web.API.Endpoints.Loans;

public sealed class RequestLoan : IEndpoint
{
    public sealed record ApplyLoan(Guid BookId);
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(pattern: "loans/apply", async (
                ApplyLoan request,
                ICommandHandler<ApplyLoanCommand, string> handler,
                CancellationToken cancellationToken
            ) =>
            {
                var command = new ApplyLoanCommand(request.BookId);
                Result<string> result = await handler.Handle(command, cancellationToken);

                return result.Match(
                    Results.Ok,
                    CustomResults.Problem
                );
            })
            .WithTags(Tags.Loans);
    }
}
