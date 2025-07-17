using Application.Abstractions.Messaging;

namespace Application.Books.GetBooksAndLoans;

public record GetBooksAndLoansQuery : IQuery<IEnumerable<BookAndLoansResponse>>;
