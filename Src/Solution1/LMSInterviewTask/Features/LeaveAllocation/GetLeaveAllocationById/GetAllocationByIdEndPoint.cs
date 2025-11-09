using BuildingBlocks.CQRS;
using Carter;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMSInterviewTask.Api.Features.LeaveAllocation.GetLeaveAllocationById;
public record GetAllocationRequest(int UserId, int PeriodId);
public record GetAllocationResponse(List<UserLeaveAllocationDto> UserLeave);

public class GetAllocationByIdEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/allocations",async ([FromQuery] int userId,
    [FromQuery] int periodId, ISender sender) =>
        {
            var query = new GetAllocationQuery(userId, periodId);
            var result = await sender.Send(query);
            var response = result.Adapt<GetAllocationResponse>();
            return Results.Ok(response);
        });
    }
}
