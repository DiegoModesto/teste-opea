using SharedKernel;

namespace Domain;

public static class LoanErrors
{
    public static Error NotFound(Guid loanId) => Error.NotFound(
        code: "Loans.NotFound",
        description: $"The loan with the Ticket Id = '{loanId}' was not found");

    public static Error Unavailable => Error.Failure(
        code: "Loans.Unavailable",
        description: "You can't make a loan because the book is unavailable.");

    public static Error AlreadyReturned(Guid loanId) => Error.Conflict(
        code: "Loans.AlreadyReturned",
        description: $"The loan with the Ticket Id = '{loanId}' has already been returned.");
    
    public static Error IsOverdued(Guid loanId) => Error.Conflict(
        code: "Loans.AlreadyReturned",
        description: $"The loan with the Ticket Id = '{loanId}' is overdued.");
}
