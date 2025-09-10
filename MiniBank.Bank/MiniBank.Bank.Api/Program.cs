using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using MiniBank.Bank.Application.Service;
using MiniBank.Bank.Core.Interface;
using MiniBank.Bank.Infrastructure.Data;
using MiniBank.Bank.Infrastructure.Repository;
using MiniBank.Bank.Infrastructure.Service;
using Serilog;
using StackExchange.Redis;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        path: $"logs/bank-{DateTime.Now:yyyyMMdd-HHmmss}.log",
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
    
    builder.Services.AddSingleton<IConnectionMultiplexer>(
        ConnectionMultiplexer.Connect("localhost:6379"));

    builder.Services.AddControllers().AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

    builder.Services.AddScoped<IAccountRepository, AccountRepository>();
    builder.Services.AddScoped<IAccountService, AccountService>();
    builder.Services.AddScoped<ICacheService, RedisCacheService>();
    builder.Services.AddScoped<IMessageBroker, RabbitMqService>();
    builder.Services.AddScoped<IMessageBrokerConsumer, MessageBrokerConsumer>();
    builder.Services.AddScoped<IMessageBrokerPublisher, MessageBrokerPublisher>();

    var app = builder.Build();
    
    using (var scope = app.Services.CreateScope())
    {
        var consumer = scope.ServiceProvider.GetRequiredService<IMessageBrokerConsumer>();
        _ = consumer.StartEmailConfirmedConsumer();
    }

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