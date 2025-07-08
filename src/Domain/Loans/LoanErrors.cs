using SharedKernel;

namespace Domain;

public static class LoanErrors
{
    public static Error NotFound(Guid bookId) => Error.NotFound(
        code: "Loans.NotFound",
        description: $"The loan with the Book Id = '{bookId}' was not found");

    public static Error Unavailable() => Error.Failure(
        code: "Loans.Unavailable",
        description: "You can't make a loan because the book is unavailable.");
}
