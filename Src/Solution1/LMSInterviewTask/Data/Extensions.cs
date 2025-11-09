using Microsoft.EntityFrameworkCore;

namespace LMSInterviewTask.Api.Data;

public static class Extensions
{
    public static IApplicationBuilder UseMigration(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<LmsContext>();
        dbContext.Database.MigrateAsync();
        return app;
    }
}
