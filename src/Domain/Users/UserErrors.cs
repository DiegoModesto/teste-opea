using SharedKernel;

namespace Domain.Users;

public static class UserErrors
{
    public static Error NotFound(Guid userId) => Error.NotFound(
        code: "Users.NotFound",
        description: $"The user with the Id = '{userId}' was not found");

    public static Error Unauthorized() => Error.Failure(
        code: "Users.Unauthorized",
        description: "You are not authorized to perform this action.");

    public static readonly Error NotFoundByEmail = Error.NotFound(
        code: "Users.NotFoundByEmail",
        description: "The user with the specified email was not found");

    public static readonly Error EmailNotUnique = Error.Conflict(
        code: "Users.EmailNotUnique",
        description: "The provided email is not unique");
}
