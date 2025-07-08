using Application.Users.GetById;
using Domain.Users;
using Infra.Database;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Tests.Users;

public class GetUserByIdQueryHandlerTests
{
   
    [Fact]
    public async Task Handle_ReturnsUserResponse_WhenUserExists()
    {
        // Arrange
        DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        await using var context = new ApplicationDbContext(options);
        var userId = Guid.NewGuid();
        context.Users.Add(new User
        {
            Id = userId,
            Email = "teste@teste.com",
            FirstName = "Diego1",
            LastName = "Teste2",
            PasswordHash = "F8DA953696611D84A8A617AC71368CE292E3965A381B13B040A51B9ADC3578FF-C2D00B9B58A4B252C2D0D71C013A4D00"
        });
        await context.SaveChangesAsync();

        var handler = new GetUserByIdQueryHandler(context);
        var query = new GetUserByIdQuery(userId);

        // Act
        Result<UserResponse> result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }
    
    [Fact]
    public async Task Handle_ReturnsUserResponse_WhenUserDoesNotExists()
    {
        // Arrange
        DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        await using var context = new ApplicationDbContext(options);
        var userId = Guid.NewGuid();
        context.Users.Add(new User
        {
            Id = userId,
            Email = "teste@teste.com",
            FirstName = "Diego1",
            LastName = "Teste2",
            PasswordHash = "F8DA953696611D84A8A617AC71368CE292E3965A381B13B040A51B9ADC3578FF-C2D00B9B58A4B252C2D0D71C013A4D00"
        });
        await context.SaveChangesAsync();

        var handler = new GetUserByIdQueryHandler(context);
        var query = new GetUserByIdQuery(Guid.NewGuid());

        // Act
        Result<UserResponse> result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
    }
}
