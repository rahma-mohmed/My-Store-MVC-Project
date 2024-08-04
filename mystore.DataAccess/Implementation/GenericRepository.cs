using Microsoft.EntityFrameworkCore;
using mystore.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace mystore.DataAccess.Implementation
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private DbSet<T> _dbset;

        public GenericRepository(ApplicationDbContext context)
        {
             _context = context;
             _dbset = context.Set<T>();
        }
        public void Add(T entity)
        {
            _dbset.Add(entity);
        } 

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> ?perdicate = null, string? Includeword = null)
        {
            IQueryable<T> query = _dbset;
            if(perdicate != null) //where
            {
                query = query.Where(perdicate);
            }
            if(Includeword != null) //include
            {
                //context.categories.include({logos , x})
                foreach (var item in Includeword.Split(new char[] {','} , StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }
            }
            return query.ToList();
        }

        public T GetFirstorDefault(Expression<Func<T, bool>> ?perdicate = null, string? Includeword = null)
        {
            IQueryable<T> query = _dbset;

            if(perdicate != null)
            {
                query = query.Where(perdicate);
            }
            if(Includeword != null)
            {
                foreach(var item in Includeword.Split(new char[] {','} , StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }
            }
            return query.SingleOrDefault();
        }

        public void Remove(T entity)
        {
            _dbset.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbset.RemoveRange(entities);
        }
    }
}
