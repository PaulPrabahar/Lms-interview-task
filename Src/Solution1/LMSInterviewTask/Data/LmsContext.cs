using LMSInterviewTask.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LMSInterviewTask.Api.Data;

public class LmsContext:DbContext
{

    public LmsContext(DbContextOptions<LmsContext> options) : base(options)
    {

    }
    public DbSet<User> Users {  get; set; }
    public DbSet<LeavePeriod> LeavePeriods {  get; set; }
    public DbSet<LeaveTypes> LeaveTypes {  get; set; }
    public DbSet<UserLeaveAllocation> UserLeaveAllocations {  get; set; }
    public DbSet<LeaveApplication> LeaveApplications {  get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LeaveTypes>(e =>
        {
            e.Property(x => x.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            e.Property(x => x.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            e.HasData(
                new LeaveTypes { Id = 1, Code = "RH", Name = "Restricted Holiday" },
                new LeaveTypes { Id = 2, Code = "CL", Name = "Casual Leave" },
                new LeaveTypes { Id = 3, Code = "BONUS", Name = "Bonus Leave" },
                new LeaveTypes { Id = 4, Code = "AL", Name = "Annual Leave" }
            );
        });
    }
}
