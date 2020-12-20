using System;
using Abstractions.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace Persistence.EFCore.Contracts
{
    public abstract class AbstractUnitOfWork<TContext> : IUnitOfWork
        where TContext : AbstractContext
    {
        private readonly TContext _context;

        protected AbstractUnitOfWork(TContext context)
        {
            _context = context;
        }

        private IDbContextTransaction Transaction { get; set; }
        private string OwnerName { get; set; }

        public IWriteRepository<TEntity> GetRepository<TEntity>()
            where TEntity : class, IDomainEntity
        {
            return new WriteRepository<TEntity>(_context);
        }

        public void Begin(string auditOwner)
        {
            Transaction = _context.Database.BeginTransaction();
            OwnerName   = auditOwner;
        }

        public void Commit()
        {
            try
            {
                _context.SaveChanges(OwnerName);
                Transaction?.Commit();
            }
            catch (Exception)
            {
                Transaction?.Rollback();
                throw;
            }
            finally
            {
                Transaction?.Dispose();
            }
        }
    }
}