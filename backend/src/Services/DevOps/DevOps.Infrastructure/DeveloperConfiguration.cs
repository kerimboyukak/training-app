using DevOps.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevOps.Infrastructure
{
    internal class DeveloperConfiguration : IEntityTypeConfiguration<Developer>
    {
        public void Configure(EntityTypeBuilder<Developer> builder)
        {
            builder.ToTable("Developers");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Id).HasMaxLength(11);
            builder.Property(d => d.FirstName).IsRequired();
            builder.Property(d => d.LastName).IsRequired();
            builder.Property(d => d.Rating)
                .HasConversion(
                    v => (double)v,
                    v => new Percentage(v)
                    // used to convert the Percentage type to a double when saving to the database
                    // and back to a Percentage when reading from the database.
                );
        }
    }
}
