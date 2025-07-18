using Application.Abstractions.Messaging;

namespace Application.Loans.ReturnLoan;

public sealed record ReturnLoanCommand(Guid LoanId) : ICommand<string>;
