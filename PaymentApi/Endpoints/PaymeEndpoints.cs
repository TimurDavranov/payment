using System.Text.Json;
using Common.Payme.Exeptions;
using Microsoft.AspNetCore.Mvc;
using Common.Payme.Requests;
using Common.Payme.Requests.CancelTransaction;
using Common.Payme.Requests.CheckPerform;
using Common.Payme.Requests.CheckTransaction;
using Common.Payme.Requests.CreateTransaction;
using Common.Payme.Requests.PerformTransaction;
using Common.Payme.Responses;
using PaymentApi.Services;

namespace PaymentApi.Endpoints;

public static class PaymeEndpoints
{
    public static IApplicationBuilder AddPaymeEndpoints(this WebApplication app)
    {
        var paymeMapGroup = app.MapGroup("Payme");
        paymeMapGroup.MapPost("Pay/", async (dynamic request, [FromServices] PaymeService _paymeService) =>
            {
                try
                {
                    object response = null;
                    switch (request.RequestMethod)
                    {
                        case "CheckPerformTransaction":
                            response = await _paymeService.CheckPerformTransaction(
                                request as CheckPerformTransactionRequest);
                            return Results.Ok(JsonSerializer.Serialize(response));
                        case "CreateTransaction":
                            response = await _paymeService.CreateTransaction(request as CreateTransactionRequest);
                            return Results.Ok(JsonSerializer.Serialize(response));
                        case "PerformTransaction":
                            response = await _paymeService.PerformTransaction(request as PerformTransactionRequest);
                            return Results.Ok(JsonSerializer.Serialize(response));
                        case "CheckTransaction":
                            response = await _paymeService.CheckTransaction(request as CheckTransactionRequest);
                            return Results.Ok(JsonSerializer.Serialize(response));
                        case "CancelTransaction":
                            response = await _paymeService.CancelTransaction(request as CancelTransactionRequest);
                            return Results.Ok(JsonSerializer.Serialize(response));
                    }

                    return Results.Ok();
                }
                catch (PaymeMerchantException ex)
                {
                    return Results.Ok(new PaymeResponse()
                    {
                        ResultResponse = null,
                        ErrorResponse = new PaymeErrorResponse()
                        {
                            Error = ex.Error
                        }
                    });
                }
            })
            .WithName("Payme action")
            .WithOpenApi();

        return app;
    }
}