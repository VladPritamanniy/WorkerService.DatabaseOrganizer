using Quartz;
using DatabaseOrganizer.Jobs;
using Serilog;
using DatabaseOrganizer.Data;
using DatabaseOrganizer.HelperExtensions;
using Microsoft.EntityFrameworkCore;

namespace DatabaseOrganizer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("D:\\Log_Worker_DatabaseOrganizer.txt")
                .CreateLogger();

            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddWindowsService();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddQuartz(q =>
            {
                q.SchedulerName = "Delete_unused_databases_scheduler";
                q.SchedulerId = "AUTO";
                q.UsePersistentStore(s =>
                {
                    s.UseSqlServer(connectionString);
                    s.UseClustering();
                    s.UseProperties = true;
                    s.UseJsonSerializer();
                });

                var jobKey = new JobKey("DeleteUnusedDatabases");
                q.AddJob<DatabaseOrganizerJob>(opts => opts.WithIdentity(jobKey));
                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity(jobKey.Name + "_trigger")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithInterval(TimeSpan.FromDays(30))
                        .RepeatForever()
                    )
                );

            });
            builder.Services.AddQuartzHostedService();
            
            var host = builder.Build();
            await host.Services.MigrateDatabaseAsync();
            await host.RunAsync();
        }
    }
}