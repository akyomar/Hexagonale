using Hexagonal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hexagonal.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Email)
            .HasConversion(
                e => e.Value,
                s => Hexagonal.Domain.ValueObjects.Email.Create(s))
            .IsRequired()
            .HasMaxLength(320);
        builder.OwnsOne(c => c.Address, a =>
        {
            a.Property(x => x.Street).HasMaxLength(300);
            a.Property(x => x.City).HasMaxLength(100);
            a.Property(x => x.PostalCode).HasMaxLength(20);
            a.Property(x => x.Country).HasMaxLength(100);
        });
        builder.Ignore(c => c.Orders);
    }
}
