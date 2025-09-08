using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using MiniBank.Auth.Application.Service;
using MiniBank.Auth.Core.Interface;
using MiniBank.Auth.Infrastructure.Data;
using MiniBank.Auth.Infrastructure.Repository;
using MiniBank.Auth.Infrastructure.Service;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        path: $"logs/auth-{DateTime.Now:yyyyMMdd-HHmmss}.log",
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
    )
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddControllers().AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IMessageBroker, RabbitMqService>();
    builder.Services.AddScoped<IMessageBrokerConsumer, MessageBrokerConsumer>();
    builder.Services.AddScoped<IMessageBrokerPublisher, MessageBrokerPublisher>();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseRouting();
    app.MapControllers();
    app.UseHttpsRedirection();

    app.Run();
}
catch (Exception e)
{
    Log.Fatal(e, "Fatal Error: " + e.Message);
}
finally
{
    Log.CloseAndFlush();
}
