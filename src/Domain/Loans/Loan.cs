using SharedKernel;

namespace Domain;

public sealed class Loan : Entity
{
    public Guid Id { get; set; }
    public DateTimeOffset? ReturnDate { get; set; }
    public LoanStatus Status { get; set; }
    
    public Guid BookId { get; set; }
    public Book Book { get; set; }
}
