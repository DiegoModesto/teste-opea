using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Users.Delete;

internal sealed class DeleteUserCommandHandler(IApplicationDbContext context) : ICommandHandler<DeleteUserCommand, Guid>
{
    public async Task<Result<Guid>> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        await context.Users
            .Where(u => u.Id == command.UserId)
            .ExecuteDeleteAsync(cancellationToken: cancellationToken);

        return command.UserId;
    }
}
