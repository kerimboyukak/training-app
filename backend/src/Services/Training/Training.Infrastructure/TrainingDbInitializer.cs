using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Training.Domain;

namespace Training.Infrastructure
{
    internal class TrainingDbInitializer
    {
        private readonly TrainingContext _context;
        private readonly ILogger<TrainingDbInitializer> _logger;

        public TrainingDbInitializer(TrainingContext context, ILogger<TrainingDbInitializer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void MigrateDatabase()
        {
            _logger.LogInformation("Migrating database associated with TrainingContext");

            try
            {
                //if the sql server container is not created on run docker compose this migration can't fail for network related exception.
                var retry = Policy.Handle<SqlException>()       // retry policies with Polly
                    .WaitAndRetry(new TimeSpan[]
                    {
                        TimeSpan.FromSeconds(3),
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(8),
                    });
                retry.Execute(() => _context.Database.Migrate());

                _logger.LogInformation("Migrated database associated with TrainingContext");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while migrating the database used on TrainingContext");
            }
        }
        public void SeedData()
        {
            if (_context.Rooms.Any())
            {
                return;
            }

            _context.Rooms.AddRange(new[]
            {
                Room.CreateNew("Hermanus"),
                Room.CreateNew("Henricus")
            });

            _context.SaveChanges();
        }
    }
}
