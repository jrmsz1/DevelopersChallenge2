using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SERVICESAPI.DataAccess.Repository
{
    //Better
    //https://medium.com/@adlerpagliarini/c-net-core-criando-uma-aplica%C3%A7%C3%A3o-utilizando-repository-pattern-com-dois-orms-diferentes-dapper-a821d501e317
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly DbContext _dbContext;
        private readonly DbSet<TEntity> _dbSet;
   

        public Repository(IUnitOfWork<OFXContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _dbContext = unitOfWork.Context;            
            _dbSet = _dbContext.Set<TEntity>();
        }

        public Repository(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _dbContext = unitOfWork.Context;
            _dbSet = _dbContext.Set<TEntity>();
        }

        public void Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public TEntity Find(params object[] keyValues)
        {
            return _dbSet.Find(keyValues);
        }

        public IEnumerable<TEntity> FindAll()
        {
            return _dbSet.AsEnumerable<TEntity>();
        }

        public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.Where(predicate).AsEnumerable<TEntity>();
        }

        public void Insert(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public void InsertMany(IEnumerable<TEntity> entities)
        {
            _dbSet.AddRange(entities);          
        }

        public IQueryable<TEntity> Queryable()
        {
            return _dbSet;
        }


        public void Update(TEntity entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            
        }

     
    }
}