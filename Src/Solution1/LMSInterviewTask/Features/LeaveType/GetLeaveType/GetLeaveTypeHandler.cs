using BuildingBlocks.CQRS;
using LMSInterviewTask.Api.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace LMSInterviewTask.Api.Features.LeaveType.GetLeaveType;
public record LeaveTypeDto(
    int Id,
    string Code,
    string Name,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);
public record GetLeaveTypeQuery():IQuery<GetLeaveTypeResult>;
public record GetLeaveTypeResult(List<LeaveTypeDto> LeaveTypes);

public class GetLeaveTypeHandler(LmsContext context) : IQueryHandler<GetLeaveTypeQuery, GetLeaveTypeResult>
{
    public async Task<GetLeaveTypeResult> Handle(GetLeaveTypeQuery request, CancellationToken cancellationToken)
    {
        var leaveType = await context.LeaveTypes.ToListAsync(cancellationToken);
        var result = leaveType.Adapt<List<LeaveTypeDto>>();
        return new GetLeaveTypeResult(result);
    }
}
