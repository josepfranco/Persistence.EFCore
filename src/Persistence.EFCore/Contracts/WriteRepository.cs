using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Persistence.EFCore.Contracts
{
    public class WriteRepository<TEntity> : IWriteRepository<TEntity>
        where TEntity : class, IDomainEntity
    {
        private readonly AbstractContext _context;
        private readonly DbSet<TEntity> _repository;

        public WriteRepository(AbstractContext context)
        {
            _context    = context;
            _repository = _context.Set<TEntity>();
        }

        public Task InsertAsync(TEntity entity, CancellationToken token = default)
        {
            return Task.Run(() => _context.Entry(entity).State = EntityState.Added, token);
        }

        public Task InsertRangeAsync(IEnumerable<TEntity> entities, CancellationToken token = default)
        {
            return Task.Run(() =>
            {
                foreach (var entity in entities) _context.Entry(entity).State = EntityState.Added;
            }, token);
        }

        public Task UpdateAsync(TEntity entity, CancellationToken token = default)
        {
            return Task.Run(() => _context.Entry(entity).State = EntityState.Modified, token);
        }

        public Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken token = default)
        {
            return Task.Run(() =>
            {
                foreach (var entity in entities) _context.Entry(entity).State = EntityState.Modified;
            }, token);
        }

        public async Task MergeAsync(TEntity entity, CancellationToken token = default)
        {
            await _repository.AddAsync(entity, token);
        }

        public async Task MergeRangeAsync(IEnumerable<TEntity> entities, CancellationToken token = default)
        {
            await _repository.AddRangeAsync(entities, token);
        }

        public async Task DeleteAsync(TEntity entity, CancellationToken token = default)
        {
            await Task.Run(() => _repository.Remove(entity), token);
        }

        public async Task DeleteAsyncAsync(IEnumerable<TEntity> entities, CancellationToken token = default)
        {
            await Task.Run(() => _repository.RemoveRange(entities), token);
        }
    }
}