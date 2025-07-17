using Application.Abstractions.Messaging;

namespace Application.Books.GetById;

public sealed record GetBookByIdQuery(Guid Id) : IQuery<BookResponse>;
