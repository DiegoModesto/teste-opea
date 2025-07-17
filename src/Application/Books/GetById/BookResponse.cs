namespace Application.Books;

public sealed record BookResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; }
    public string Author { get; init; }
    public DateTime Publish { get; init; }
}
