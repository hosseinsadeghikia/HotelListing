﻿using System.Linq.Expressions;
using HotelListing.Models;
using Microsoft.EntityFrameworkCore.Query;
using X.PagedList;

namespace HotelListing.IRepository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IList<T>> GetAll(
            Expression<Func<T, bool>>? expression = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);

        Task<IPagedList<T>> GetAllPagedList(RequestParams requestParams,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);

        Task<T?> Get(Expression<Func<T, bool>>? expression,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);

        Task Insert(T entity);
        Task InsertRange(IEnumerable<T> entities);
        Task Delete(int id);
        void DeleteRange(IEnumerable<T> entities);
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);
    }
}
