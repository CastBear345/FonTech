namespace FonTech.Domain.Interfaces.Databases;

public interface IStateOfChanges
{
    Task<int> SaveChangesAsync();
}
