using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain;
using SharedKernel;

namespace Application.Books.Create;

public sealed class CreateBookCommandHandler(IApplicationDbContext context)
    : ICommandHandler<CreateBookCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateBookCommand command, CancellationToken cancellationToken)
    {
        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = command.Title,
            Author = command.Author,
            Publish = command.Publish,
            TotalRemaining = command.Remaining
        };
        
        context.Books.Add(book);
        
        await context.SaveChangesAsync(cancellationToken);

        return book.Id;
    }
}
