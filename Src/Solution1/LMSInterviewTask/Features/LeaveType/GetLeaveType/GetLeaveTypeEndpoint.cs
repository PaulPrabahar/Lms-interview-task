using Carter;
using Mapster;
using MediatR;

namespace LMSInterviewTask.Api.Features.LeaveType.GetLeaveType;
public record GetLeaveTypeResponse(List<LeaveTypeDto> LeaveTypes);
public class GetLeaveTypeEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/leavetypes", async (ISender sender) =>
        {
            var result = await sender.Send(new GetLeaveTypeQuery { });
            var response = result.Adapt<GetLeaveTypeResponse>();
            return Results.Ok(response);
        });
    }
}
