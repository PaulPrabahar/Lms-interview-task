using BuildingBlocks.CQRS;
using LMSInterviewTask.Api.Data;
using LMSInterviewTask.Api.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace LMSInterviewTask.Api.Features.Users.GetUsers;
public record UserDto(
    int Id,
    string EmpCode,
    string FullName,
    string Email,
    bool IsActive,
    DateTimeOffset CreatedAt
);
public record GetUserQuery():IQuery<GetUserResult>;
public record GetUserResult(List<UserDto> User);

public class GetUserHandler(LmsContext context) : IQueryHandler<GetUserQuery, GetUserResult>
{
    public async Task<GetUserResult> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await context.Users.ToListAsync(cancellationToken);

        var responce = user.Adapt<List<UserDto>>();
        return new GetUserResult(responce);
    }
}
