using BuildingBlocks.CQRS;
using LMSInterviewTask.Api.Data;
using Microsoft.EntityFrameworkCore;
using System.Windows.Input;

namespace LMSInterviewTask.Api.Features.LeaveApplication.GetAvailableBalanceById;

public record LeaveBalanceDto(
    int TypeId,
    string TypeCode,
    string TypeName,
    decimal AllocatedDays,
    decimal UsedDays,
    decimal AvailableDays
);

public record GetAvailableBalanceByIdCommand(int UserId, int PeriodId, int? TypeId) :ICommand<GetAvailableBalanceByIdResult>;
public record GetAvailableBalanceByIdResult(List<LeaveBalanceDto> Balances);
public class GetAvailableBalanceHandler(LmsContext context) : ICommandHandler<GetAvailableBalanceByIdCommand, GetAvailableBalanceByIdResult>
{
public async Task<GetAvailableBalanceByIdResult> Handle(GetAvailableBalanceByIdCommand request, CancellationToken ct)
    {
        if (request.UserId <= 0) throw new ArgumentOutOfRangeException(nameof(request.UserId));
        if (request.PeriodId <= 0) throw new ArgumentOutOfRangeException(nameof(request.PeriodId));

        // Step 1: Fetch all allocations for user + period (filtered by TypeId if given)
        var allocationsQuery = context.UserLeaveAllocations
            .AsNoTracking()
            .Include(x => x.Type)
            .Where(x => x.UserId == request.UserId && x.PeriodId == request.PeriodId);

        if (request.TypeId is not null)
            allocationsQuery = allocationsQuery.Where(x => x.TypeId == request.TypeId.Value);

        var allocations = await allocationsQuery.ToListAsync(ct);

        // Step 2: Get approved leave usage per type
        var usedQuery = context.LeaveApplications
            .AsNoTracking()
            .Where(x => x.UserId == request.UserId &&
                        x.PeriodId == request.PeriodId &&
                        x.Status == "Approved");

        if (request.TypeId is not null)
            usedQuery = usedQuery.Where(x => x.TypeId == request.TypeId.Value);

        var usedGroup = await usedQuery
            .GroupBy(x => x.TypeId)
            .Select(g => new { TypeId = g.Key, UsedDays = g.Sum(x => x.DaysRequested) })
            .ToListAsync(ct);

        // Step 3: Merge allocations + used to compute available balance
        var result = allocations
            .Select(a =>
            {
                var used = usedGroup.FirstOrDefault(u => u.TypeId == a.TypeId)?.UsedDays ?? 0;
                var available = a.DaysAllocated - used;
                return new LeaveBalanceDto(
                    a.TypeId,
                    a.Type.Code,
                    a.Type.Name,
                    a.DaysAllocated,
                    used,
                    available < 0 ? 0 : available
                );
            })
            .OrderBy(b => b.TypeCode)
            .ToList();

        return new GetAvailableBalanceByIdResult(result);
    }
}
