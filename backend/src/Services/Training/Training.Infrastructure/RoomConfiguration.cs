using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Training.Domain;

namespace Training.Infrastructure
{
    internal class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.ToTable("Rooms");
            builder.HasKey(r => r.RoomCode);
            builder.Property(r => r.RoomCode)
                .HasMaxLength(15)
                .HasConversion(c => c.ToString(), s => new Code(s));

            builder.Property(r => r.Name).IsRequired();
        }
    }
}