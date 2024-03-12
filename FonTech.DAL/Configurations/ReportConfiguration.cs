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

        builder.HasData(new List<Report>()
        {
            new Report()
            {
                Id = 1,
                Name = "Report #1",
                Description = "Test description",
                UserId = 1,
                CreatedAt = DateTime.UtcNow,
            }
        });
    }
}
