using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace SERVICESAPI.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private OFXContext _context;
        private Dictionary<string, dynamic> _repositories;
     
        public DbContext Context => _context;

        public UnitOfWork(OFXContext context)
        {
            _context = context;
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            if (_repositories == null)
                _repositories = new Dictionary<string, dynamic>();
            var type = typeof(TEntity).Name;
            if (_repositories.ContainsKey(type))
                return (IRepository<TEntity>)_repositories[type];

            var repositoryType = typeof(Repository<>);

            _repositories.Add(type, Activator.CreateInstance(
                repositoryType.MakeGenericType(typeof(TEntity)), this)
            ) ;
            return _repositories[type];
        }


        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                    _repositories = null;
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}