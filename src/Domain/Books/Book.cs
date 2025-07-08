using SharedKernel;

namespace Domain;

public sealed class Book : Entity
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public DateTime Publish { get; set; }
    public int TotalRemaining { get; set; }
    
    public Loan Loan { get; set; }
}
