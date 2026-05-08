using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Lab04.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab04.Repositories
{
    public class GenericRepository<T> : IRepository<T>
        where T : class
    {
        private readonly UniDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(UniDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public IEnumerable<T> GetAll()
        {
            // Special-case: include Students when querying Departments so DTOs can access navigation
            if (typeof(T) == typeof(Models.Department))
            {
                var deps = _context
                    .Set<Models.Department>()
                    .Include(d => d.Students)
                    .ToList()
                    .Cast<T>()
                    .ToList();
                return deps;
            }

            return _dbSet.ToList();
        }

        public T GetByName(object name)
        {
            T entity = _dbSet.FirstOrDefault(e => EF.Property<string>(e, "Name") == (string)name);
            if (entity == null)
            {
                throw new InvalidOperationException(
                    $"No entity of type {typeof(T).Name} with Name '{name}' was found."
                );
            }
            return entity;
        }

        public T GetById(object id)
        {
            return _dbSet.Find(id);
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Edit(T entity)
        {
            // Edit behaves like Update but uses Attach+Modified
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }
    }
}
