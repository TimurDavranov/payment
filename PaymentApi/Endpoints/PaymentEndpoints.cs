using Common.Enums;
using Common.Models;
using Common.Payme.Extentions;
using Microsoft.AspNetCore.Mvc;
using PaymentApi.Services;

namespace PaymentApi.Endpoints;

public static class PaymentEndpoints
{
    public static IApplicationBuilder AppPaymentEndpoints(this WebApplication app)
    {
        var paymentMapGroup = app.MapGroup("Payment");
        paymentMapGroup.MapPost("Generate/", async (PaymentRequest request, [FromServices] ChequeService _chequeService) =>
            {
                await _chequeService.Generate(request);

                switch (request.System)
                {
                    case EpsSystem.Click:
                        return Results.Ok();
                    case EpsSystem.Oson:
                        return Results.Ok();
                    case EpsSystem.Payme:
                        return Results.Ok(PaymeChequeGenerator.Generate(request));
                    default:
                    {
                        await _chequeService.Remove(request.UniqueId);
                        throw new Exception("Данная платежная система не найдена");
                    }
                }
            })
            .WithName("Payment generate cheque action")
            .WithOpenApi();

        return app;
    }
}