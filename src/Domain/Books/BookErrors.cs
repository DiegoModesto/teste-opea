using SharedKernel;

namespace Domain;

public static class BookErrors
{
    public static Error NotFound(Guid bookId) => Error.NotFound(
        code: "Books.NotFound",
        description: $"The book with the Id = '{bookId}' was not found");

    public static Error Unauthorized() => Error.Failure(
        code: "Books.Unauthorized",
        description: "You are not authorized to perform this action.");
    
    public static Error NoBookAvailable() => Error.Failure(
        code: "Books.NoBookAvailable",
        description: "There are no books available for loan at the moment.");
}
