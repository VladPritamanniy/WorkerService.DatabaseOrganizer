using Quartz;
using Serilog;
using System.Data.SqlClient;

namespace DatabaseOrganizer.Jobs
{
    [DisallowConcurrentExecution]
    public class DatabaseOrganizerJob : IJob
    {
        private readonly IConfiguration _configuration;

        public DatabaseOrganizerJob(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            Log.Information("Start job executing");

            string queryString = @"
                DECLARE @dbName SYSNAME
                WHILE EXISTS (
                    SELECT name
                    FROM sys.databases
                    WHERE name NOT IN ('master', 'tempdb', 'model', 'msdb') AND
                        DATEDIFF(DAY, create_date, GETDATE()) > 30
                )
                BEGIN
                    SELECT TOP 1 @dbName = name
                    FROM sys.databases
                    WHERE name NOT IN ('master', 'tempdb', 'model', 'msdb') AND
                        DATEDIFF(DAY, create_date, GETDATE()) > 30

                    EXEC('DROP DATABASE [' + @dbName + ']')
                END
            ";

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand command = new SqlCommand(queryString, connection))
                {
                    try
                    {
                        command.Connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Exception while job executing{ex}");
                    }
                }
            }

            Log.Information("End job executing");
        }
    }
}
