using FonTech.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FonTech.DAL.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder
            .Property(u => u.Id)
            .ValueGeneratedOnAdd();

        builder
            .Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasData(new List<Role>()
        {
            new Role()
            {
                Id = 1,
                Name = "User"
            },
            new Role()
            {
                Id = 2,
                Name = "Admin"
            },
            new Role()
            {
                Id = 3,
                Name = "Moderator"
            },
        });
    }
}
