using Application.Abstractions.Messaging;

namespace Application.BooksAndLoans.List;

public sealed record BooksAndLoansQuery : IQuery<IEnumerable<BooksAndLoansResponse>>;
