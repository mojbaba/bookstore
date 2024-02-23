using System.Text;
using BookStore.Authentication.Jwt;
using BookStore.Authentication.Jwt.Redis;
using BookStore.EventLog.Kafka;
using BookStore.EventObserver;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using UserService;
using UserService.KafkaObserversForProducer;
using UserService.Login;
using UserService.Logout;
using UserService.Register;

namespace UserService;

public class Program
{
    public static int Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Configuration.AddJsonFile("appsettings.json");
        builder.Configuration.AddEnvironmentVariables();

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description =
                    "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
        });

        builder.Services.AddDbContext<UserServiceDbContext>((p, options) =>
        {
            var configuration = p.GetRequiredService<IConfiguration>();
            options.UseNpgsql(configuration.GetConnectionString("PostgreSqlConnection"));
        });

        builder.Services.AddSingleton(p =>
        {
            var configuration = p.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("RedisConnection");
            return ConnectionMultiplexer.Connect(connectionString);
        });

        builder.Services.AddTransient<IDatabase>(provider =>
        {
            var multiplexer = provider.GetRequiredService<ConnectionMultiplexer>();
            return multiplexer.GetDatabase();
        });

        builder.Services.AddScoped<IUserRegisterService, UserRegisterService>();
        builder.Services.AddScoped<IUserLoginService, UserLoginService>();
        builder.Services.AddScoped<IUserLogoutService, UserLogoutService>();
        builder.Services.AddTransient<ITokenService, JwtTokenService>();
        builder.Services.RegisterEventSourceObservant();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddRedisTokenValidationService();

        builder.Services.AddSingleton<IEventLogProducer, KafkaEventLogProducer>();
        builder.Services.AddTransient<Confluent.Kafka.ProducerConfig>(p =>
        {
            var configuration = p.GetRequiredService<IConfiguration>();
            return new Confluent.Kafka.ProducerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"]
            };
        });

        builder.Services.AddTransient(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            KafkaOptions kafkaOptions = new KafkaOptions();
            configuration.GetSection("Kafka").Bind(kafkaOptions);
            return kafkaOptions;
        });

        builder.Services.AddSingleton<IEventPublishObserver, UserLoggedOutObserver>();
        builder.Services.AddSingleton<IEventPublishObserver, UserRegisteredKafkaObserver>();
        builder.Services.AddSingleton<IEventPublishObserver, UserLoggedInKafkaObserver>();
        builder.Services.AddSingleton<IEventPublishObserver, UserLoggedOutKafkaObserver>();

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            var configuration = builder.Configuration;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]))
            };
        });

        builder.Services.AddControllers();

        var app = builder.Build();

        app.SubscribeObservers();

        app.UseMiddleware<JwtValidationMiddleware>();

        app.MapControllers();

        app.UseAuthentication();
        app.UseAuthorization();


        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();


        app.Run();

        return 0;
    }
}