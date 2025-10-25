using HumanResources.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Training.Domain;

namespace Training.Infrastructure
{
    internal class ApprenticeConfiguration : IEntityTypeConfiguration<Apprentice>
    {
        public void Configure(EntityTypeBuilder<Apprentice> builder)
        {
            builder.ToTable("Apprentices");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Id).HasMaxLength(11);
            builder.Property(d => d.FirstName).IsRequired();
            builder.Property(d => d.LastName).IsRequired();
            builder.Property(d => d.Company).IsRequired();
        }
    }
}
