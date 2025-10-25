using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Training.Domain;

namespace Training.Infrastructure
{
    internal class ParticipationConfiguration : IEntityTypeConfiguration<Participation>
    {
        public void Configure(EntityTypeBuilder<Participation> builder)
        {
            builder.ToTable("Participations");
            builder.HasKey(p => new { p.TrainingCode, p.ApprenticeId });
            builder.Property(p => p.TrainingCode)
                .HasMaxLength(15)
                .HasConversion(
                    c => c.ToString(),
                    s => new Code(s))
                .IsRequired();
            builder.Property(p => p.ApprenticeId)
                .HasMaxLength(11)
                .IsRequired();
            builder.Property(p => p.IsFinished)
                .IsRequired();

            builder.HasOne(p => p.Training)
                .WithMany(t => t.Participations)
                .HasForeignKey(p => p.TrainingCode)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Apprentice)
                .WithMany()
                .HasForeignKey(p => p.ApprenticeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}