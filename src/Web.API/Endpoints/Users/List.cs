using Application.Abstractions.Messaging;
using Application.Users.GetAll;
using Application.Users.GetById;
using SharedKernel;
using Web.API.Extensions;
using Web.API.Infrastructure;

namespace Web.API.Endpoints.Users;

internal sealed class List : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(pattern: "users", async (
            IQueryHandler<GetAllUsersQuery, IEnumerable<UserResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllUsersQuery();

            Result<IEnumerable<UserResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Users);
    }
}
