using BuildingBlocks.CQRS;
using LMSInterviewTask.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace LMSInterviewTask.Api.Features.LeaveApplication;

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

public record CreateLeaveApplicationCommand(int UserId, int PeriodId, int TypeId, decimal DaysRequested)
    : ICommand<CreateLeaveApplicationResult>;

public record CreateLeaveApplicationResult(LeaveApplicationDto LeaveApplication);

public class CreateLeaveApplicationHandler(LmsContext context)
    : ICommandHandler<CreateLeaveApplicationCommand, CreateLeaveApplicationResult>
{
    public async Task<CreateLeaveApplicationResult> Handle(CreateLeaveApplicationCommand request, CancellationToken ct)
    {
        if (request.DaysRequested <= 0)
            throw new ArgumentException("DaysRequested must be greater than zero.", nameof(request.DaysRequested));

        var userExists = await context.Users.AsNoTracking().AnyAsync(u => u.Id == request.UserId, ct);
        var periodExists = await context.LeavePeriods.AsNoTracking().AnyAsync(p => p.Id == request.PeriodId, ct);
        var typeExists = await context.LeaveTypes.AsNoTracking().AnyAsync(t => t.Id == request.TypeId, ct);

        if (!userExists) throw new InvalidOperationException($"User {request.UserId} not found.");
        if (!periodExists) throw new InvalidOperationException($"LeavePeriod {request.PeriodId} not found.");
        if (!typeExists) throw new InvalidOperationException($"LeaveType {request.TypeId} not found.");

        // Load allocation row (must exist)
        var allocation = await context.UserLeaveAllocations
            .FirstOrDefaultAsync(a =>
                a.UserId == request.UserId &&
                a.PeriodId == request.PeriodId &&
                a.TypeId == request.TypeId, ct);

        if (allocation is null)
        {
            var ts = DateTimeOffset.UtcNow;
            return new CreateLeaveApplicationResult(
                new LeaveApplicationDto(0, request.UserId, request.PeriodId, request.TypeId, request.DaysRequested,
                    "Rejected", "No leave allocation found for the selected period/type.", ts, ts));
        }

        var alreadyApproved = await context.LeaveApplications
            .Where(l => l.UserId == request.UserId &&
                        l.PeriodId == request.PeriodId &&
                        l.TypeId == request.TypeId &&
                        l.Status == "Approved")
            .SumAsync(l => (decimal?)l.DaysRequested, ct) ?? 0m;

        var available = allocation.DaysAllocated - alreadyApproved;
        var now = DateTimeOffset.UtcNow;

        await using var tx = await context.Database.BeginTransactionAsync(ct);

        if (available >= request.DaysRequested)
        {
            allocation.DaysAllocated -= request.DaysRequested;
            allocation.UpdatedAt = now;

            var approved = new Models.LeaveApplication
            {
                UserId = request.UserId,
                PeriodId = request.PeriodId,
                TypeId = request.TypeId,
                DaysRequested = request.DaysRequested,
                Status = "Approved",
                DecisionNote = "Auto-approved by system (balance sufficient).",
                CreatedAt = now,
                UpdatedAt = now
            };

            context.LeaveApplications.Add(approved);
            await context.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            return new CreateLeaveApplicationResult(
                new LeaveApplicationDto(approved.Id, approved.UserId, approved.PeriodId, approved.TypeId,
                    approved.DaysRequested, approved.Status, approved.DecisionNote, approved.CreatedAt, approved.UpdatedAt));
        }
        else
        {
            var rejected = new Models.LeaveApplication
            {
                UserId = request.UserId,
                PeriodId = request.PeriodId,
                TypeId = request.TypeId,
                DaysRequested = request.DaysRequested,
                Status = "Rejected",
                DecisionNote = "Insufficient leave balance.",
                CreatedAt = now,
                UpdatedAt = now
            };

            context.LeaveApplications.Add(rejected);
            await context.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            return new CreateLeaveApplicationResult(
                new LeaveApplicationDto(rejected.Id, rejected.UserId, rejected.PeriodId, rejected.TypeId,
                    rejected.DaysRequested, rejected.Status, rejected.DecisionNote, rejected.CreatedAt, rejected.UpdatedAt));
        }
    }
}
