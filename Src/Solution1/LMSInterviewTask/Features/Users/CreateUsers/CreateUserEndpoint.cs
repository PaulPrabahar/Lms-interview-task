using Carter;
using Mapster;
using MediatR;

namespace LMSInterviewTask.Api.Features.Users.CreateUsers;

public record CreateUserRequest(string EmpCode, string FullName, string Email);
public record CreateUserResponce(int Id);
public class CreateUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/users", async (CreateUserRequest request, ISender sender) =>
        {
            var command = request.Adapt<CreateUserCommand>();
            var result = await sender.Send(command);
            var responce = result.Adapt<CreateUserResponce>();
            return Results.Created($"/users/{responce.Id}",responce);
        });
    }
}
