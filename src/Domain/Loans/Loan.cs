using SharedKernel;

namespace Domain;

public sealed class Loan : Entity
{
    public Guid Id { get; set; }
    public DateTime? ReturnDate { get; set; }
    public LoanStatus Status { get; set; }
    
    public Guid BookId { get; set; }
    public Book Book { get; set; }
}
