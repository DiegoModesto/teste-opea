using Application.Users.GetById;
using Application.Users.Register;
using Application.Abstractions.Authentication;
using Domain.Users;
using Infra.Database;
using Microsoft.EntityFrameworkCore;
using Moq;
using SharedKernel;

namespace Application.Tests.Users;

public class RegisterUserCommandHandlerTests
{
   
    [Fact]
    public async Task Handle_ReturnsUserResponse_WhenUserCreated()
    {
        // Arrange
        var passwordHasherMock = new Mock<IPasswordHasher>();
        DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        await using var context = new ApplicationDbContext(options);
        passwordHasherMock
            .Setup(ph => ph.Hash(It.IsAny<string>()))
            .Returns("F8DA953696611D84A8A617AC71368CE292E3965A381B13B040A51B9ADC3578FF-C2D00B9B58A4B252C2D0D71C013A4D00");
        
        var handler = new RegisterUserCommandHandler(context, passwordHasherMock.Object);
        var command = new RegisterUserCommand(Email: "teste@teste.com", FirstName: "Diego1", LastName:  "Teste2", Password: "@123mudar");
        
        // Act
        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }
    
    [Fact]
    public async Task Handle_ReturnsUnsuccess_WhenUserAlredyExist()
    {
        // Arrange
        var passwordHasherMock = new Mock<IPasswordHasher>();
        DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        await using var context = new ApplicationDbContext(options);
        passwordHasherMock
            .Setup(ph => ph.Hash(It.IsAny<string>()))
            .Returns("F8DA953696611D84A8A617AC71368CE292E3965A381B13B040A51B9ADC3578FF-C2D00B9B58A4B252C2D0D71C013A4D00");
        
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Email = "teste@teste.com",
            FirstName = "Diego1",
            LastName = "Teste2",
            PasswordHash =
                "F8DA953696611D84A8A617AC71368CE292E3965A381B13B040A51B9ADC3578FF-C2D00B9B58A4B252C2D0D71C013A4D00"
        };
        
        
        
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new RegisterUserCommandHandler(context, passwordHasherMock.Object);
        var command = new RegisterUserCommand(Email: "teste@teste.com", FirstName: "Diego1", LastName:  "Teste2", Password: "@123mudar");
        

        // Act
        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
    }
}

