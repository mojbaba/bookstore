using System.Net.Http.Headers;
using System.Net.Http.Json;
using UserService.Login;
using UserService.Register;

namespace UserService.IntegrationTest;

[Collection("default collection")]
public class UserServiceTest
{
    private readonly UserServiceHostFixture _userServiceHostFixture;

    public UserServiceTest(UserServiceHostFixture userServiceHostFixture)
    {
        _userServiceHostFixture = userServiceHostFixture;
    }

    [Fact]
    public async Task RegisterLoginLogout_ShouldSuccess()
    {
        // Arrange
        var client = _userServiceHostFixture.CreateClient();
        var request = new UserRegisterationRequest
        {
            Email = "user@example.com",
            Password = "super_secret_password"
        };

        // Act
        var registerResponse = await client.PostAsJsonAsync("/api/user/register", request, CancellationToken.None);
        var registerResult = await registerResponse.Content.ReadFromJsonAsync<UserRegisterationResponse>();

        // Assert
        Assert.NotNull(registerResult);
        Assert.True(registerResponse.IsSuccessStatusCode);
        Assert.Equal(request.Email, registerResult.Email);


        // Arrange
        var loginRequest = new UserLoginRequest
        {
            Email = request.Email,
            Password = request.Password
        };

        // Act
        var loginResponse = await client.PostAsJsonAsync("/api/user/login", loginRequest, CancellationToken.None);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<UserLoginResponse>();

        // Assert
        Assert.NotNull(loginResult);
        Assert.True(loginResponse.IsSuccessStatusCode);
        Assert.Equal(request.Email, loginResult.Email);
        Assert.NotNull(loginResult.Token);

        // Arrange
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", loginResult.Token.Replace("Bearer ", ""));

        // Act
        var logoutResponse = await client.PostAsync("/api/user/logout", null, CancellationToken.None);

        // Assert
        Assert.True(logoutResponse.IsSuccessStatusCode);

        // Act
        var logoutResponse2 = await client.PostAsync("/api/user/logout", null, CancellationToken.None);

        // Assert
        Assert.False(logoutResponse2.IsSuccessStatusCode);
    }
}