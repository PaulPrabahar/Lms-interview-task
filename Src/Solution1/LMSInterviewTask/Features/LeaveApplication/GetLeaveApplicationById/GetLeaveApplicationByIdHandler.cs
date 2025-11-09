using BuildingBlocks.CQRS;
using LMSInterviewTask.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace LMSInterviewTask.Api.Features.LeaveApplication.GetLeaveApplicationById;

public record LeaveApplicationDto(
    int Id,
    int UserId,
    int PeriodId,
    int TypeId,
    decimal DaysRequested,
    string Status,
    string DecisionNote,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);
public record GetLeaveApplicationByIdQuery(int UserId, int PeriodId) :IQuery<CreateLeaveApplicationByIdResult>;
public record CreateLeaveApplicationByIdResult(List<LeaveApplicationDto> LeaveApplication);

public class GetLeaveApplicationByIdHandler(LmsContext context) : IQueryHandler<GetLeaveApplicationByIdQuery, CreateLeaveApplicationByIdResult>
{
    public async Task<CreateLeaveApplicationByIdResult> Handle(GetLeaveApplicationByIdQuery request, CancellationToken cancellationToken)
    {
        // Optional guards (avoid default/zero ids)
        if (request.UserId <= 0) throw new ArgumentOutOfRangeException(nameof(request.UserId));
        if (request.PeriodId <= 0) throw new ArgumentOutOfRangeException(nameof(request.PeriodId));

        // Read-only list of all applications for this user & period
        var rows = await context.LeaveApplications
    .AsNoTracking()
    .Where(x => x.UserId == request.UserId && x.PeriodId == request.PeriodId)
    .Select(x => new LeaveApplicationDto(
        x.Id,
        x.UserId,
        x.PeriodId,
        x.TypeId,
        x.DaysRequested,
        x.Status,
        x.DecisionNote,
        x.CreatedAt,
        x.UpdatedAt
    ))
    .ToListAsync(cancellationToken);

        // Order client-side (safe for SQLite)
        rows = rows.OrderByDescending(x => x.CreatedAt).ToList();

        return new CreateLeaveApplicationByIdResult(rows);
    }
}
