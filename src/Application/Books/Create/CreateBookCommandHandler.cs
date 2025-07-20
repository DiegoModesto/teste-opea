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
        if (string.IsNullOrEmpty(command.Title))
        {
            return Result.Failure<Guid>(BookErrors.BookTitleRequired);
        }

        if (string.IsNullOrEmpty(command.Author))
        {
            return Result.Failure<Guid>(BookErrors.BookAuthorRequired);
        }
        
        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = command.Title,
            Author = command.Author,
            Publish = command.Publish,
            TotalRemaining = command.Remaining > 0 ? command.Remaining : 10,
        };
        
        context.Books.Add(book);
        
        await context.SaveChangesAsync(cancellationToken);

        return book.Id;
    }
}
