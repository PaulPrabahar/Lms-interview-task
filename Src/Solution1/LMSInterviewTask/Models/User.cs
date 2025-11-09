namespace LMSInterviewTask.Api.Models;

public class User
{
    public int Id { get; set; }
    public string EmpCode { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public ICollection<UserLeaveAllocation> UserLeaveAllocation { get; set; }
    public ICollection<LeaveApplication> LeaveApplications { get; set; }
}
