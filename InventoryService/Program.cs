using System.Text;
using BookStore.Authentication.Jwt;
using BookStore.Authentication.Jwt.KafkaLoggedOut;
using BookStore.Authentication.Jwt.Redis;
using BookStore.EventLog.Kafka;
using BookStore.EventObserver;
using BookStore.RedisLock;
using InventoryService.AdminOperations;
using InventoryService.Entities;
using InventoryService.KafkaOrderEventsConsumer;
using InventoryService.QueryBooks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

namespace InventoryService;

public class Program
{
    public static void Main(string[] args)
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

        builder.Services.AddEntites();

        builder.Services.AddSingleton<IEventLogProducer, KafkaEventLogProducer>();

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

        builder.Services.AddTransient<IBookRepository, BookRepository>();
        builder.Services.AddTransient<IAdminOperationsService, AdminOperationsService>();
        builder.Services.AddTransient<IDistributedLock, RedisDistributedLock>();
        builder.Services.AddRedisTokenValidationService();

        builder.Services.AddKafkaUserLoggedOutHandler(p =>
        {
            var configuration = p.GetRequiredService<IConfiguration>();
            return new KafkaUserLoggedOutOptions
            {
                GroupId = configuration["Kafka:GroupId"],
                BootstrapServers = configuration["Kafka:BootstrapServers"],
                Topic = configuration["Kafka:Topics:UserLogoutTopic"]
            };
        });
        
        builder.Services.AddTransient(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            KafkaOptions kafkaOptions = new KafkaOptions();
            configuration.GetSection("Kafka").Bind(kafkaOptions);
            return kafkaOptions;
        });

        builder.Services.AddHostedService<KafkaOrderCreatedEventConsumer>();

        builder.Services.AddTransient<IBookQueryHandler, BookQueryHandler>();
        builder.Services.AddSingleton<IEventPublishObserver, OrderCreatedEventObserver>();
        builder.Services.AddTransient<OrderCreatedEventHandler>();

        builder.Services.AddControllers();
        
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

        builder.Services.RegisterEventSourceObservant();
        

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (true)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseMiddleware<JwtValidationMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();

        app.SubscribeObservers();

        app.MapControllers();

        app.Run();
    }
}