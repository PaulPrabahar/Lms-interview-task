using BuildingBlocks.CQRS;
using Carter;
using Mapster;
using MediatR;

namespace LMSInterviewTask.Api.Features.LeaveAllocation.CreateAllocationById;
public record CreateAllocationRequest(int UserId, int PeriodId, int TypeId, decimal DaysAllocated);
public record CreateAllocationResponse(int Id);

public class CreateAllocationByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/allocations", async (CreateAllocationRequest request, ISender sender) =>
        {
            var command = request.Adapt<CreateAllocationCommand>();
            var result = await sender.Send(command);
            var response = result.Adapt<CreateAllocationResponse>();
            return Results.Created($"/allocations/{response.Id}", response);

        });
    }
}
