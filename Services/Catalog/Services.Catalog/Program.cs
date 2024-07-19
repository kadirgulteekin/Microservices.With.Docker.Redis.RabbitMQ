using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Services.Catalog.Configuration;
using Services.Catalog.Mapping;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(MappingProfile));

ServiceConfigurator.Configure(builder.Services);

//Database settings configuration
builder.Services.Configure<DatabaseConfigurations>(builder.Configuration.GetSection(DatabaseConfigurations.Key));
builder.Services.AddSingleton<IDatabaseConfigurations>(sp => sp.GetRequiredService<IOptions<DatabaseConfigurations>>().Value);

builder.Services.AddCors(x =>
{
    x.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();


        });
});

builder.Services.AddSwaggerGen(s =>
{
    s.SwaggerDoc("v1", new OpenApiInfo { Title = "Services.Catalog", Version = "V1" });
});

var app = builder.Build();


app.UseRouting();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
