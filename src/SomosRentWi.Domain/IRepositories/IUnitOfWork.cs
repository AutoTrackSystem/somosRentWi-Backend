namespace SomosRentWi.Domain.IRepositories;

public interface IUnitOfWork
{
    Task SaveChangesAsync();
}