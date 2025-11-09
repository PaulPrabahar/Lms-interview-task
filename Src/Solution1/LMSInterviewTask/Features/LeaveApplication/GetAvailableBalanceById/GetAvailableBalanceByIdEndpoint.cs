using BuildingBlocks.CQRS;
using Carter;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMSInterviewTask.Api.Features.LeaveApplication.GetAvailableBalanceById;

public record GetAvailableBalanceByIdRequest(int UserId, int PeriodId, int? TypeId);
public record GetAvailableBalanceByIdResponse(List<LeaveBalanceDto> Balances);
public class GetAvailableBalanceByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/balanceleave", async([FromQuery] int userId,
    [FromQuery] int periodId,[FromQuery] int? TypeId, ISender sender) =>
        {
            var query = new GetAvailableBalanceByIdCommand(userId, periodId,TypeId);
            var result = await sender.Send(query);
            var response = result.Adapt<GetAvailableBalanceByIdResponse>();
            return Results.Ok(response);
        });
    }
}
