using Carter;
using Mapster;
using MediatR;

namespace LMSInterviewTask.Api.Features.Period.GetPeriod;
public record GetPeriodResponse(List<PeriodDto> User);
public class GetPeriodEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/period", async (ISender sender) =>
        {
            var result = await sender.Send(new GetPeriodQuery { });
            var response = result.Adapt<GetPeriodResponse>();
            return Results.Ok(response);
        });
    }
}
