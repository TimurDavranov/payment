using Common.Extentions;
using Microsoft.Extensions.Options;

using Common.Helpers;
using Common.Models;
using Data.DbContexts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPersistance(builder.Configuration);
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<PaymentDbContext>(
    (serviceProvider, dbContextOptionsBuilder) =>
    {
        var databaseOptions = serviceProvider.GetService<IOptions<DatabaseOptionsModel>>()!.Value;
        
        dbContextOptionsBuilder.UseSqlServer(databaseOptions.ConnectionString, sqlServiceAction =>
        {
            sqlServiceAction.EnableRetryOnFailure(databaseOptions.MaxRetryCount);
            
            sqlServiceAction.CommandTimeout(databaseOptions.CommandTimeOut);
        });
        
        dbContextOptionsBuilder.EnableDetailedErrors(databaseOptions.EnableDetaildeErrors);
        
        dbContextOptionsBuilder.EnableSensitiveDataLogging(databaseOptions.EnableSensitiveDataLogging);
    });

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
