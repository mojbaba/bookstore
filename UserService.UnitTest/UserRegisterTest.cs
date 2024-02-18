using BookStore.EventObserver;
using Moq;
using UserService.Register;
using UserService.Register.Events;

namespace UserService.UnitTest;

public class UserRegisterTest
{
    [Fact]
    public async Task RegisterAsync_WithValidRequest_ShouldReturnTrue()
    {
        // Arrange
        var request = new UserRegisterationRequest
        {
            Email = "user@example.com",
            Password = "password"
        };

        var repository = new Mock<IUserRepository>();
        var eventPublishObservant = new Mock<IEventPublishObservant>();
        

        var service = new UserRegisterService(repository.Object, eventPublishObservant.Object);

        // Act
        var result = await service.RegisterAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Email, result.Email);
        
        eventPublishObservant.Verify(a => a.PublishAsync(It.IsAny<UserRegisteredEvent>()), Times.Once);
        repository.Verify(repo => repo.CreateUserAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}