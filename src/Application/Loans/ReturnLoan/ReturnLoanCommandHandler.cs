using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain;
using Microsoft.EntityFrameworkCore;
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
            .SingleOrDefaultAsync(l => l.Id == command.LoanId, cancellationToken);

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
        
        Book? book = await context
            .Books
            .SingleOrDefaultAsync(b => b.Id == loan.BookId, cancellationToken);

        if (book is null)
        {
            return Result.Failure<string>(error: BookErrors.NotFound(loan.BookId));
        }
        
        loan.Status = LoanStatus.Completed;
        book.TotalRemaining += 1;
        
        context.Loans.Update(loan);
        context.Books.Update(book);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success(loan.Id.ToString());
    }
}
