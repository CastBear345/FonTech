using FonTech.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FonTech.DAL.Configurations;

public class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder
            .Property(u => u.Id)
            .ValueGeneratedOnAdd();

        builder
            .Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .Property(u => u.Description)
            .IsRequired()
            .HasMaxLength(2000);
    }
}
