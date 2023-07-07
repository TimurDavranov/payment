using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

using Common.Models;
using Data.DbContexts;
using PaymentApi.Endpoints;
using PaymentApi.Extentions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPersistance(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.AddClickEndpoints();
app.AddPaymeEndpoints();
app.AppPaymentEndpoints();

app.Run();
