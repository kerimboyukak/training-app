using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Training.Domain;

namespace Training.Infrastructure
{
    internal class TrainingConfiguration : IEntityTypeConfiguration<Domain.Training>
    {
        public void Configure(EntityTypeBuilder<Domain.Training> builder)
        {
            builder.ToTable("Trainings");
            builder.HasKey(t => t.TrainingCode);
            builder.Property(t => t.TrainingCode)
                .HasMaxLength(15)
                .HasConversion(
                    c => c.ToString(), 
                    s => new Code(s)
                );

            builder.Property(t => t.Name).IsRequired();
            builder.Property(t => t.Description).IsRequired();
            builder.Property(t => t.MaximumCapacity).IsRequired();

            builder.Property(t => t.RoomCode)
               .HasMaxLength(15)
               .HasConversion(
                   c => c.ToString(),
                   s => new Code(s)
               )
               .IsRequired();

            builder.HasOne(t => t.Room)
                .WithMany()
                .HasForeignKey(t => t.RoomCode)
                .IsRequired();

            builder.HasOne(t => t.Coach)
                .WithMany()
                .HasForeignKey(t => t.CoachId)
                .IsRequired();

            builder.HasMany(t => t.Participations)
                 .WithOne()
                 .HasForeignKey(p => p.TrainingCode)
                 .OnDelete(DeleteBehavior.Cascade);

            builder.OwnsOne(t => t.TimeWindow, tw =>
            {
                tw.Property(tw => tw.Start).HasColumnName("StartTime");
                tw.Property(tw => tw.End).HasColumnName("EndTime");
            });
        }
    }
}