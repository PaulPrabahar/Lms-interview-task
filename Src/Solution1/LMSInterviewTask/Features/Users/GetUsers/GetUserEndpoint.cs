using BuildingBlocks.CQRS;
using Carter;
using LMSInterviewTask.Api.Models;
using Mapster;
using MediatR;

namespace LMSInterviewTask.Api.Features.Users.GetUsers;
//public record GetUserRequest();
public record GetUserResponce(List<UserDto> User);
public class GetUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/users", async (ISender sender) =>
        {
            var request = await sender.Send(new GetUserQuery { });
            var result = request.Adapt<GetUserResponce>();
            return Results.Ok(result);

        });
    }
}
