using BookStore.EventObserver;
using Moq;
using UserService.Logout;

namespace UserService.UnitTest;

public class UserLogoutTest
{

    [Fact]
    public async Task LogoutAsync_WithValidRequest_ShouldReturnTrue()
    {
        // Arrange
        var request = new UserLogoutRequest
        {
            Id = "1",
            Token = "token",
            Email = "user@example.com"
        };
        
        var eventPublishObservant = new Mock<IEventPublishObservant>();
        
        var service = new UserLogoutService(eventPublishObservant.Object);
        
        // Act
        var result = await service.LogoutAsync(request, CancellationToken.None);
        
        // Assert
        Assert.True(result);
        
        eventPublishObservant.Verify(a => a.PublishAsync(It.IsAny<UserLoggedOutEvent>()), Times.Once);
    }
}