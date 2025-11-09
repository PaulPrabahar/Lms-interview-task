namespace LMSInterviewTask.Api.Models;

public class LeavePeriod
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public ICollection<UserLeaveAllocation> UserLeaveAllocations { get; set; }
    public ICollection<LeaveApplication> LeaveApplications { get; set; }
}
