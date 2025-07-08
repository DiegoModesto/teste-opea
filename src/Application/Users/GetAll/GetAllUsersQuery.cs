using Application.Abstractions.Messaging;
using Application.Users.GetById;

namespace Application.Users.GetAll;

public sealed record GetAllUsersQuery : IQuery<IEnumerable<UserResponse>>;
