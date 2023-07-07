using Common.Click.Requests;
using Microsoft.AspNetCore.Mvc;
using PaymentApi.Services;
using Microsoft.AspNetCore.OpenApi;

namespace PaymentApi.Endpoints;

public static class ClickEndpoints
{
    public static IApplicationBuilder AddClickEndpoints(this WebApplication app)
    {
        var clickMapGroup = app.MapGroup("Click");
        clickMapGroup
            .MapPost("Prepare/", async (PrepareClickRequest request, [FromServices]ClickService _clickService) => 
                Results.Ok(await _clickService.Prepare(request)))
            .WithName("Click first action")
            .WithOpenApi();

        clickMapGroup
            .MapPost("Complete/", async (CompleteClickRequest request, [FromServices]ClickService _clickService) => 
                Results.Ok(await _clickService.Complete(request)))
            .WithName("Click second action")
            .WithOpenApi();;
        
        return app;
    }
}