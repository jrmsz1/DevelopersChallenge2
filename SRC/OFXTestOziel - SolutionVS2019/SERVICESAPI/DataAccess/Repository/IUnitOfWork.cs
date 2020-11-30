using Microsoft.EntityFrameworkCore;
using System;

namespace SERVICESAPI.DataAccess.Repository
{

    //https://garywoodfine.com/generic-repository-pattern-net-core/

    public interface IUnitOfWork : IDisposable
    {

        //DbContext Context { get; }

        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

        void Commit();
    }

    public interface IUnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
    {
        TContext Context { get; }
    }
}
