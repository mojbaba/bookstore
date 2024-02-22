using BookStore.EventObserver;
using Moq;
using UserService.Login;

namespace UserService.UnitTest;

public class UserLoginTest
{
    [Fact]
    public async Task LoginAsync_WithValidRequest_ShouldReturnUserLoginResponse()
    {
        // Arrange
        var request = new UserLoginRequest
        {
            Email = "user@example.com",
            Password = "password"
        };

        var repository = new Mock<IUserRepository>();
        var eventPublishObservant = new Mock<IEventPublishObservant>();
        var tokenService = new Mock<ITokenService>();

        repository.Setup(repo => repo.GetAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserEntity
            {
                Id = Guid.NewGuid().ToString(),
                Email = request.Email,
                Password = "AQAAAAIAAYagAAAAEIaOlj5Bec2qAd1As9vC0Bcauv2U/hzW1rdOpedL6n047fetFBFgCD7uIBG7A44wRg==",
                Salt = "salt"
            });

        tokenService.Setup(a => a.GenerateToken(It.IsAny<string>(), It.IsAny<string>())).Returns("token");

        var service = new UserLoginService(repository.Object, tokenService.Object, eventPublishObservant.Object);

        // Act
        var result = await service.LoginAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Email, result.Email);
        Assert.NotNull(result.Token);

        eventPublishObservant.Verify(a => a.PublishAsync(It.IsAny<UserLoggedinEvent>()), Times.Once);
    }
}