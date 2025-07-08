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
}
