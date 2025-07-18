namespace Application.Books.GetById;

public sealed record BookResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; }
    public string Author { get; init; }
    public DateTimeOffset Publish { get; init; }
}
