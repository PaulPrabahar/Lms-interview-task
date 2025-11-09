using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using LMSInterviewTask.Api.Data;
using LMSInterviewTask.Api.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Windows.Input;

namespace LMSInterviewTask.Api.Features.LeaveAllocation.CreateAllocationById;

public record CreateAllocationCommand(int UserId, int PeriodId, int TypeId, decimal DaysAllocated) :ICommand<CreateAllocationResult>;
public record CreateAllocationResult(int Id);

public class CreateAllocationByIdHandler(LmsContext context) : ICommandHandler<CreateAllocationCommand, CreateAllocationResult>
{
     public async Task<CreateAllocationResult> Handle(CreateAllocationCommand request, CancellationToken ct)
     {
        // 1) Basic FK validation (optional but safer)
        var userExists = await context.Users.AnyAsync(u => u.Id == request.UserId, ct);
        var periodExists = await context.LeavePeriods.AnyAsync(p => p.Id == request.PeriodId, ct);
        var typeExists = await context.LeaveTypes.AnyAsync(t => t.Id == request.TypeId, ct);

        if (!userExists) throw new NotFoundException("User not found", nameof(request.UserId));
        if (!periodExists) throw new NotFoundException("Leave period not found", nameof(request.PeriodId));
        if (!typeExists) throw new NotFoundException("Leave type not found", nameof(request.TypeId));
        if (request.DaysAllocated < 0) throw new ArgumentOutOfRangeException(nameof(request.DaysAllocated), "DaysAllocated must be >= 0");

        // 2) Try fetch existing allocation for (User, Period, Type)
        var existing = await context.UserLeaveAllocations
            .FirstOrDefaultAsync(x =>
                x.UserId == request.UserId &&
                x.PeriodId == request.PeriodId &&
                x.TypeId == request.TypeId, ct);

        if (existing is not null)
        {
            // UPDATE path
            existing.DaysAllocated = request.DaysAllocated;
            existing.UpdatedAt = DateTimeOffset.UtcNow;

            await context.SaveChangesAsync(ct);
            // return new CreateAllocationResult(existing.Id, Updated: true);
            return new CreateAllocationResult(existing.Id);
        }

        // 3) CREATE path
        var allocation = new UserLeaveAllocation
        {
            UserId = request.UserId,
            PeriodId = request.PeriodId,
            TypeId = request.TypeId,
            DaysAllocated = request.DaysAllocated,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        context.UserLeaveAllocations.Add(allocation);

        try
        {
            await context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            // In case of a race with another request creating the same tuple,
            // re-fetch and switch to UPDATE (idempotent upsert behavior).
            var concurrent = await context.UserLeaveAllocations
                .FirstOrDefaultAsync(x =>
                    x.UserId == request.UserId &&
                    x.PeriodId == request.PeriodId &&
                    x.TypeId == request.TypeId, ct);

            if (concurrent is null) throw; // real error

            concurrent.DaysAllocated = request.DaysAllocated;
            concurrent.UpdatedAt = DateTimeOffset.UtcNow;
            await context.SaveChangesAsync(ct);

            // return new CreateAllocationResult(concurrent.Id, Updated: true);
            return new CreateAllocationResult(concurrent.Id);
        }

        var result = allocation.Adapt<CreateAllocationResult>();
        return result;
     }
}
