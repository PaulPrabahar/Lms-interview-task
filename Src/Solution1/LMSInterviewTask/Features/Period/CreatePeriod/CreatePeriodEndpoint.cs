using BuildingBlocks.CQRS;
using Carter;
using Mapster;
using MediatR;

namespace LMSInterviewTask.Api.Features.Period.CreatePeriod;
public record CreatePeriodRequest(string Name, DateTimeOffset StartDate, DateTimeOffset EndDate);
public record CreatePeriodResponse(int Id);
public class CreatePeriodEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/period", async (CreatePeriodRequest request, ISender sender) =>
        {
            var command = request.Adapt<CreatePeriodCommand>();
            var result = await sender.Send(command);
            var response = result.Adapt<CreatePeriodResponse>();
            return Results.Created($"/period/{response.Id}", response);
        });
    }
}
