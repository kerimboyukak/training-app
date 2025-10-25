using HumanResources.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

internal class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        // defining table name
        builder.ToTable("Employees");

        // defining primary key -> Number that is max 11 characters long
        builder.HasKey(e => e.Number);
        builder.Property(e => e.Number).HasMaxLength(11)

            // converting EmployeeNumber to string and vice versa when read from or written to the database
            .HasConversion(n => n.ToString(), s => new EmployeeNumber(s));

        builder.Property(e => e.LastName).IsRequired();
        builder.Property(e => e.FirstName).IsRequired();
    }
}