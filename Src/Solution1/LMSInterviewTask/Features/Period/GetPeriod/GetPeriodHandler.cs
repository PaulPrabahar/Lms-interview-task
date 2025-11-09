using BuildingBlocks.CQRS;
using LMSInterviewTask.Api.Data;
using LMSInterviewTask.Api.Features.Users.GetUsers;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace LMSInterviewTask.Api.Features.Period.GetPeriod;
public record PeriodDto(
    int Id,
    string Name,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);
public record GetPeriodQuery():IQuery<GetPeriodResult>;
public record GetPeriodResult(List<PeriodDto> User);
public class GetPeriodHandler(LmsContext context) : IQueryHandler<GetPeriodQuery, GetPeriodResult>
{
    public async Task<GetPeriodResult> Handle(GetPeriodQuery request, CancellationToken cancellationToken)
    {
        var period = await context.LeavePeriods.ToListAsync(cancellationToken);
        var result = period.Adapt<List<PeriodDto>>();
        return new GetPeriodResult(result);
    }
}
