using FonTech.Domain.Entities;
using FonTech.Domain.Interfaces.Databases;
using FonTech.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace FonTech.DAL.Repositories;

public class UnitOfWork : IUnitofWorkRepository
{
    private readonly ApplicationDbContext _dbContext;
    public IBaseRepository<User> Users { get; set; }
    public IBaseRepository<Role> Roles { get; set; }
    public IBaseRepository<UserRole> UserRoles { get; set; }

    public UnitOfWork(ApplicationDbContext dbContext, IBaseRepository<User> users, IBaseRepository<Role> roles, IBaseRepository<UserRole> userRoles)
    {
        _dbContext = dbContext;
        UserRoles = userRoles;
        Users = users;
        Roles = roles;
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _dbContext.Database.BeginTransactionAsync();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }
}
