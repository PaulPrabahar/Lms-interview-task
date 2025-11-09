using BuildingBlocks.CQRS;
using Carter;
using Mapster;
using MediatR;

namespace LMSInterviewTask.Api.Features.LeaveApplication;

public record CreateLeaveApplicationRequest(int UserId, int PeriodId, int TypeId, decimal DaysRequested) : ICommand<CreateLeaveApplicationResult>;
public record CreateLeaveApplicationResponse(LeaveApplicationDto LeaveApplication);
public class CreateLeaveApplicationEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/leaveapplication", async (CreateLeaveApplicationRequest request, ISender sender) =>
        {
            var command = request.Adapt<CreateLeaveApplicationCommand>();
            var result = await sender.Send(command);
            var response = result.Adapt<CreateLeaveApplicationResponse>();
            return Results.Created($"/leaveapplication/{response.LeaveApplication.Id}",response);
        });
    }
}
