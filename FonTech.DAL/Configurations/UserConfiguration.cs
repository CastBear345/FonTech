using FonTech.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FonTech.DAL.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .Property(u => u.Id)
            .ValueGeneratedOnAdd();

        builder
            .Property(u => u.Login)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .Property(u => u.Password)
            .IsRequired();


        builder
            .HasMany<Report>(u => u.Reports)
            .WithOne(u => u.User)
            .HasForeignKey(u => u.UserId)
            .HasPrincipalKey(u => u.Id);

        throw new NotImplementedException();
    }
}
