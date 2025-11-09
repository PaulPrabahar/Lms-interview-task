using BuildingBlocks.CQRS;
using Carter;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMSInterviewTask.Api.Features.LeaveApplication.GetLeaveApplicationById;

public record GetLeaveApplicationByIdResponse(int UserId, int PeriodId);
public record CreateLeaveApplicationByIdResponse(List<LeaveApplicationDto> LeaveApplication);
public class GetLeaveApplicationByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("leaveapplication", async ([FromQuery] int userId,
    [FromQuery] int periodId, ISender sender ) =>
        {
            var query = new GetLeaveApplicationByIdQuery(userId, periodId);
            var result = await sender.Send(query);
            var response = result.Adapt<CreateLeaveApplicationByIdResponse>();
            return Results.Ok(response);
        });
    }
}
