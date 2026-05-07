using System.Collections.Generic;

namespace Lab03.Repositories
{
    public interface IRepository<T>
        where T : class
    {
        IEnumerable<T> GetAll();
        T GetById(object id);

        T GetByName(object name);
        void Add(T entity);
        void Edit(T entity);
        void Update(T entity);
        void Remove(T entity);
    }
}
