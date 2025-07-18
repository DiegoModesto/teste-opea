using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Loans;

public sealed class ApplyLoanCommandHandler(IApplicationDbContext context) : ICommandHandler<ApplyLoanCommand, string>
{
    public async Task<Result<string>> Handle(ApplyLoanCommand command, CancellationToken cancellationToken)
    {
        Book? book = await context
            .Books
            .SingleOrDefaultAsync(b => b.Id == command.BookId, cancellationToken);
        
        if (book is null)
        {
            return Result.Failure<string>(error: BookErrors.NotFound(command.BookId));
        }
        
        if (book.TotalRemaining == 0)
        {
            return Result.Failure<string>(error: BookErrors.NoBookAvailable());
        }

        book.TotalRemaining -= 1;

        var loan = new Loan
        {
            BookId = book.Id,
            Status = LoanStatus.Approved
        };
        
        context.Books.Update(book);
        context.Loans.Add(loan);
        
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success(loan.Id.ToString());
    }
}
