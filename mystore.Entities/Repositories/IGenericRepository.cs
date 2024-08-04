﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace mystore.Entities.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        //Include.Tolist
        //Where.Tolist
        IEnumerable<T> GetAll(Expression<Func<T , bool>>?perdicate = null , string? Includeword = null);

        //
        T GetFirstorDefault(Expression<Func<T, bool>>? perdicate = null, string? Includeword = null);

        void Add(T entity);

        void Remove(T entity);

        void RemoveRange(IEnumerable<T> entities);
    }
}
