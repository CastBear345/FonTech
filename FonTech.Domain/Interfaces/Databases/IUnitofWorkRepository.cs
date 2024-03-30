using FonTech.Domain.Entities;
using FonTech.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace FonTech.Domain.Interfaces.Databases;

public interface IUnitofWorkRepository : IStateOfChanges
{
    Task<IDbContextTransaction> BeginTransactionAsync();

    IBaseRepository<User> Users { get; set; }

    IBaseRepository<Role> Roles { get; set; }

    IBaseRepository<UserRole> UserRoles { get; set; }
}
