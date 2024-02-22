using System.Net.Http.Headers;
using System.Net.Http.Json;
using BookStore.TestingTools;
using UserService.Login;
using UserService.Logout;
using UserService.Register;
using UserService.Register.Events;

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
        
        var configuration = _userServiceHostFixture.Configuration;
        
        var userRegisteredFastConsumer = new KafkaFastConsumer<UserRegisteredEvent>(
            configuration["Kafka:BootstrapServers"],
            configuration["Kafka:Topics:UserRegisterTopic"],
            "UserServiceTest");
        
        

        // Act
        var cancelationTokenSource = new CancellationTokenSource();
        var kafkaUserRegisterMessagesTask = userRegisteredFastConsumer.ConsumeAsync(cancelationTokenSource.Token);
        var registerResponse = await client.PostAsJsonAsync("/api/user/register", request, CancellationToken.None);
        var registerResult = await registerResponse.Content.ReadFromJsonAsync<UserRegisterationResponse>();
        cancelationTokenSource.Cancel();
        var kafkaUserRegisterMessages = await kafkaUserRegisterMessagesTask;
        
        // Assert
        Assert.NotNull(registerResult);
        Assert.True(registerResponse.IsSuccessStatusCode);
        Assert.Equal(request.Email, registerResult.Email);
        Assert.NotEmpty(kafkaUserRegisterMessages);
        var registeredEvent = kafkaUserRegisterMessages.First();
        Assert.Equal(request.Email, registeredEvent.Email);


        // Arrange
        var loginRequest = new UserLoginRequest
        {
            Email = request.Email,
            Password = request.Password
        };
        
        var userLoggedInFastConsumer = new KafkaFastConsumer<UserLoggedinEvent>(
            configuration["Kafka:BootstrapServers"],
            configuration["Kafka:Topics:UserLoginTopic"],
            "UserServiceTest");

        cancelationTokenSource = new CancellationTokenSource();

        // Act
        var kafkaUserLoginMessagesTask = userLoggedInFastConsumer.ConsumeAsync(cancelationTokenSource.Token);
        var loginResponse = await client.PostAsJsonAsync("/api/user/login", loginRequest, CancellationToken.None);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<UserLoginResponse>();
        await Task.Delay(150);
        cancelationTokenSource.Cancel();
        var kafkaUserLoginMessages = await kafkaUserLoginMessagesTask;

        // Assert
        Assert.NotNull(loginResult);
        Assert.True(loginResponse.IsSuccessStatusCode);
        Assert.Equal(request.Email, loginResult.Email);
        Assert.NotNull(loginResult.Token);
        Assert.NotEmpty(kafkaUserLoginMessages);
        var loggedInEvent = kafkaUserLoginMessages.First();
        Assert.Equal(request.Email, loggedInEvent.Email);
        

        // Arrange
        var userLoggedOutFastConsumer = new KafkaFastConsumer<UserLoggedOutEvent>(
            configuration["Kafka:BootstrapServers"],
            configuration["Kafka:Topics:UserLogoutTopic"],
            "UserServiceTest");
        
        cancelationTokenSource = new CancellationTokenSource();
        
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", loginResult.Token.Replace("Bearer ", ""));

        // Act
        var kafkaUserLogoutMessagesTask = userLoggedOutFastConsumer.ConsumeAsync(cancelationTokenSource.Token);
        var logoutResponse = await client.PostAsync("/api/user/logout", null, CancellationToken.None);
        await Task.Delay(150);
        cancelationTokenSource.Cancel();
        var kafkaUserLogoutMessages = await kafkaUserLogoutMessagesTask;

        // Assert
        Assert.True(logoutResponse.IsSuccessStatusCode);
        Assert.NotEmpty(kafkaUserLogoutMessages);
        var loggedOutEvent = kafkaUserLogoutMessages.First();
        Assert.Equal(request.Email, loggedOutEvent.Email);

        // Act
        var logoutResponse2 = await client.PostAsync("/api/user/logout", null, CancellationToken.None);

        // Assert
        Assert.False(logoutResponse2.IsSuccessStatusCode);
    }
}