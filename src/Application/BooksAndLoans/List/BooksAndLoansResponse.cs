using SharedKernel;

namespace Application.BooksAndLoans.List;

public record BooksAndLoansResponse(
    Guid BookId,
    string Title,
    string Author,
    DateTime Publish,
    List<LoanInfo> Loans
);

public record LoanInfo(
    Guid Id,
    LoanStatus Status,
    DateTime LoanDate,
    DateTime? ReturnDate
);
