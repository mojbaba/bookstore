using System.Text;
using BookStore.Authentication.Jwt;
using BookStore.Authentication.Jwt.KafkaLoggedOut;
using BookStore.Authentication.Jwt.Redis;
using BookStore.EventLog.Kafka;
using BookStore.RedisLock;
using InventoryService.AdminOperations;
using InventoryService.Entities;
using InventoryService.QueryBooks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace InventoryService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddEntites();

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

        builder.Services.AddTransient<IBookQueryHandler, BookQueryHandler>();

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

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseMiddleware<JwtValidationMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}