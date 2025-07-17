using Application.Abstractions.Messaging;

namespace Application.Loans;

public sealed record ApplyLoanCommand(Guid BookId) : ICommand<string>;
