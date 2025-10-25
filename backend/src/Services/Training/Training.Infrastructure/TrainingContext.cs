using HumanResources.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Training.Domain;

namespace Training.Infrastructure
{
    internal class TrainingContext : DbContext
    {
        public DbSet<Apprentice> Apprentices { get; set; }
        public DbSet<Coach> Coaches { get; set; }
        public DbSet<Domain.Training> Trainings { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Participation> Participations { get; set; } = null!;


        public TrainingContext(DbContextOptions<TrainingContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            new ApprenticeConfiguration().Configure(modelBuilder.Entity<Apprentice>());
            new CoachConfiguration().Configure(modelBuilder.Entity<Coach>());
            new TrainingConfiguration().Configure(modelBuilder.Entity<Domain.Training>());
            new RoomConfiguration().Configure(modelBuilder.Entity<Room>());
            new ParticipationConfiguration().Configure(modelBuilder.Entity<Participation>());
        }
    }
}
