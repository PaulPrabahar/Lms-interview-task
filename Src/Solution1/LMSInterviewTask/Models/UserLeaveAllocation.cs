namespace LMSInterviewTask.Api.Models;

public class UserLeaveAllocation
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int PeriodId { get; set; }
    public int TypeId { get; set; }
    public decimal DaysAllocated { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public User User { get; set; } = default!;
    public LeavePeriod Period { get; set; } = default!;
    public LeaveTypes Type { get; set; } = default!;
}
