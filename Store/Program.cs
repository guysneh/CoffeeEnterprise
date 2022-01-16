using CoffeeStore.Constants;
using CoffeeStore.Data;
using CoffeeStore.Messaging;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<CoffeeStoreContext>(options => options
                                                .UseSqlServer(builder.Configuration
                                                .GetConnectionString(ConfigurationKeys.CoffeeStoreDB)));
builder.Services.AddHostedService<CoffeeFactoryListener>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;

    var dbContext = services.GetRequiredService<CoffeeStoreContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
