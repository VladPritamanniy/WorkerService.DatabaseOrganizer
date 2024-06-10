using DatabaseOrganizer.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DatabaseOrganizer.HelperExtensions
{
    public static class DatabaseExtensionHelper
    {
        public static async Task MigrateDatabaseAsync(this IServiceProvider provider)
        {
            using (IServiceScope scope = provider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<AppDbContext>();
                try
                {
                    await context.Database.MigrateAsync();
                }
                catch (Exception ex)
                {
                    Log.Error($"Exception while context.Database.MigrateAsync(): {ex}");
                }
            }
        }
    }
}
