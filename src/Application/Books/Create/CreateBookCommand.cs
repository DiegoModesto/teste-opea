using Application.Abstractions.Messaging;

namespace Application.Books.Create;

public sealed record CreateBookCommand(string Title, string Author, DateTime Publish, int Remaining) : ICommand<Guid>;
