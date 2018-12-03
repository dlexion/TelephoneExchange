using System.Collections.Generic;

namespace BillingSystem.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        ICollection<T> GetAll();

        void Add(T entity);

        void Remove(T entity);
    }
}