namespace LMSInterviewTask.Api.Models;

public class LeaveTypes
{
    public int Id { get; set; }
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public ICollection<UserLeaveAllocation> UserLeaveAllocation { get; set; }
    public ICollection<LeaveApplication> LeaveApplications { get; set; }
}
