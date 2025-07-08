using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Users.Register;

public sealed class RegisterUserCommandHandler(IApplicationDbContext context, IPasswordHasher passHasher)
    : ICommandHandler<RegisterUserCommand, Guid>
{
    public async Task<Result<Guid>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        if (await context.Users.AnyAsync(u => u.Email == command.Email, cancellationToken))
        {
            return Result.Failure<Guid>(error: UserErrors.EmailNotUnique);
        }
        
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = command.Email,
            FirstName = command.FirstName,
            LastName = command.LastName,
            PasswordHash = passHasher.Hash(command.Password),
        };

        //TODO: Criar evento de registro de usuário
        //user.Raise(new UserRegisterDomainEvent(user.id));

        context.Users.Add(user);
        
        await context.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
