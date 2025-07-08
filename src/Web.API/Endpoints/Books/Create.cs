using Application.Abstractions.Messaging;
using Application.Books.Create;
using SharedKernel;
using Web.API.Extensions;
using Web.API.Infrastructure;

namespace Web.API.Endpoints.Books;

public sealed class Create: IEndpoint
{
    public sealed record Request
    {
        public required string Title { get; set; }
        public required string Author { get; set; }
        public DateTime Publish { get; set; } = DateTime.Now;
        public int Remaining { get; set; } = 0;
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(pattern: "books", async (
            Request request,
            ICommandHandler<CreateBookCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateBookCommand(
                Title: request.Title, 
                Author: request.Author, 
                Publish: request.Publish, 
                Remaining: request.Remaining
            );
            
            Result result = await handler.Handle(command, cancellationToken);
            
            return result.Match(Results.Created, CustomResults.Problem);
        })
        .WithTags(Tags.Books);
    }
}
