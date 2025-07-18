using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain;
using SharedKernel;

namespace Application.Loans.ReturnLoan;

public sealed class ReturnLoanCommandHandler(IApplicationDbContext context) : ICommandHandler<ReturnLoanCommand, string>
{
    public async Task<Result<string>> Handle(
        ReturnLoanCommand command, 
        CancellationToken cancellationToken)
    {
        Loan? loan = await context
            .Loans
            .FindAsync(command.LoanId, cancellationToken);

        if (loan is null)
        {
            return Result.Failure<string>(error: LoanErrors.NotFound(command.LoanId));
        }

        switch (loan.Status)
        {
            case LoanStatus.Completed:
                return Result.Failure<string>(error: LoanErrors.AlreadyReturned(command.LoanId));
            case LoanStatus.Overdue:
                return Result.Failure<string>(error: LoanErrors.IsOverdued(command.LoanId));
            case LoanStatus.Pending:
            case LoanStatus.Approved:
            case LoanStatus.Rejected:
            default:
                break;
        }
        
        loan.Status = LoanStatus.Completed;
        
        context.Loans.Update(loan);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success(loan.Id.ToString());
    }
}
