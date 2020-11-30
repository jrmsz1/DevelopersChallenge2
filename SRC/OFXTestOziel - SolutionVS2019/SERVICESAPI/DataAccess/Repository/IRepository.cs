using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SERVICESAPI.DataAccess.Repository
{
   
        public interface IRepository<TEntity> where TEntity : class
    {
            IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> predicate);
            TEntity Find(params object[] keyValues);
            IEnumerable<TEntity> FindAll();
            void Insert(TEntity entity);
            void InsertMany(IEnumerable<TEntity> entity);
            void Update(TEntity entity);
            void Delete(TEntity entity);
            IQueryable<TEntity> Queryable();
                
          
    }
    
}