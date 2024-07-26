using Microsoft.Extensions.Options;
using Services.Basket.Configuration;
using Services.Basket.Services;
using Shared.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ISharedIdentityService, SharedIdentityService>();
builder.Services.AddScoped<IBasketService, BasketService>();
//Database settings configuration
builder.Services.Configure<RedisConfiguration>(builder.Configuration.GetSection(RedisConfiguration.Key));
builder.Services.AddSingleton<RedisService>(sp =>
{
    var redisConfig = sp.GetRequiredService<IOptions<RedisConfiguration>>().Value;

    var redis = new RedisService(redisConfig.Host ?? throw new InvalidOperationException("Redis host is not provided."), redisConfig.Port);
    redis.Connect();

    return redis;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
