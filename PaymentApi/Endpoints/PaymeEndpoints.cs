using System.Text.Json;
using Common.Payme.Exeptions;
using Microsoft.AspNetCore.Mvc;
using Common.Payme.Requests;
using Common.Payme.Responses;
using PaymentApi.Services;

namespace PaymentApi.Endpoints;

public static class PaymeEndpoints
{
    public static void Endpoints(this WebApplication app)
    {
        app.MapPost("payme/", async ([FromBody] dynamic request, PaymeService service) =>
        {
            try
            {
                object response = null;
                switch (request.RequestMethod)
                {
                    case "CheckPerformTransaction":
                        response = await service.CheckPerformTransaction(request as PaymeRequest);
                        return Results.Ok(JsonSerializer.Serialize(response));
                    case "CreateTransaction":
                        response = await service.CreateTransaction(request as CreateTransactionRequest);
                        return Results.Ok(JsonSerializer.Serialize(response));
                    case "PerformTransaction":
                        response = await service.PerformTransaction(request.ToString());
                        return Results.Ok(JsonSerializer.Serialize(response));
                    case "CheckTransaction":
                        response = await service.CheckTransaction(request);
                        return Results.Ok(JsonSerializer.Serialize(response));
                    case "CancelTransaction":
                        response = await service.CancelTransaction(request);
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
        });
    }
}