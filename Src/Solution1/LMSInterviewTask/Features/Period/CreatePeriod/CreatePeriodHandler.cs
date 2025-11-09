using BuildingBlocks.CQRS;
using LMSInterviewTask.Api.Data;
using LMSInterviewTask.Api.Models;
using Mapster;
using System.Windows.Input;

namespace LMSInterviewTask.Api.Features.Period.CreatePeriod;

public record CreatePeriodCommand(string Name, DateTimeOffset StartDate, DateTimeOffset EndDate) :ICommand<CreatePeriodResult>;
public record CreatePeriodResult(int Id);
public class CreatePeriodHandler(LmsContext context) : ICommandHandler<CreatePeriodCommand, CreatePeriodResult>
{
    public async Task<CreatePeriodResult> Handle(CreatePeriodCommand command, CancellationToken cancellationToken)
    {
        var period = new LeavePeriod
        {
            Name = command.Name,
            StartDate = command.StartDate,
            EndDate = command.EndDate,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        context.LeavePeriods.Add(period);
        await context.SaveChangesAsync(cancellationToken);

        var result = period.Adapt<CreatePeriodResult>();
        return result;
    }
}
