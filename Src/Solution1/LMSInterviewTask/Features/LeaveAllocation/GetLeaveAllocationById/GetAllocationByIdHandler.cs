using BuildingBlocks.CQRS;
using LMSInterviewTask.Api.Data;
using LMSInterviewTask.Api.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace LMSInterviewTask.Api.Features.LeaveAllocation.GetLeaveAllocationById;
public record UserLeaveAllocationDto(
    int Id,
    int UserId,
    int PeriodId,
    int TypeId,
    decimal DaysAllocated,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);
public record GetAllocationQuery(int UserId, int PeriodId) :IQuery<GetAllocationResult>;
public record GetAllocationResult(List<UserLeaveAllocationDto> UserLeave);
public class GetAllocationByIdHandler(LmsContext context) : IQueryHandler<GetAllocationQuery, GetAllocationResult>
{
    public async Task<GetAllocationResult> Handle(GetAllocationQuery request, CancellationToken cancellationToken)
    {
        var rows = await context.UserLeaveAllocations
            .AsNoTracking()
            .Where(x => x.UserId == request.UserId && x.PeriodId == request.PeriodId)
            .OrderBy(x => x.TypeId)
            .Select(x => new UserLeaveAllocationDto(
                x.Id,
                x.UserId,
                x.PeriodId,
                x.TypeId,
                x.DaysAllocated,
                x.CreatedAt,
                x.UpdatedAt
            ))
            .ToListAsync(cancellationToken);

        return new GetAllocationResult(rows);
    }
}
