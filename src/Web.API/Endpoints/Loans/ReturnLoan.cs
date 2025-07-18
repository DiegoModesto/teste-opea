using Application.Abstractions.Messaging;
using Application.Loans.ReturnLoan;
using SharedKernel;
using Web.API.Extensions;
using Web.API.Infrastructure;

namespace Web.API.Endpoints.Loans;

public sealed class ReturnLoan : IEndpoint
{
    public sealed record ReturnLoanRequest(Guid LoanId);
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(pattern: "loans/return", async (
                Guid loanId,
                ICommandHandler<ReturnLoanCommand, string> handler,
                CancellationToken cancellationToken
            ) =>
            {
                var command = new ReturnLoanCommand(loanId);
                Result<string> result = await handler.Handle(command, cancellationToken);

                return result.Match(
                    Results.Ok,
                    CustomResults.Problem
                );
            })
            .WithTags(Tags.Loans);
    }
}
