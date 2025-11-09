using BuildingBlocks.CQRS;
using LMSInterviewTask.Api.Data;
using LMSInterviewTask.Api.Models;

namespace LMSInterviewTask.Api.Features.Users.CreateUsers;

public record CreateUserCommand(string EmpCode, string FullName, string Email) : ICommand<CreateUserResult>;
public record CreateUserResult(int Id);

internal class CreateUserCommandHandler(LmsContext context) : ICommandHandler<CreateUserCommand, CreateUserResult>
{
    public async Task<CreateUserResult> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var user = new User
        {
            EmpCode = command.EmpCode,
            FullName = command.FullName,
            Email = command.Email,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        };
        context.Users.Add(user);
        await context.SaveChangesAsync(cancellationToken);

        return new CreateUserResult(user.Id);
    }
}
