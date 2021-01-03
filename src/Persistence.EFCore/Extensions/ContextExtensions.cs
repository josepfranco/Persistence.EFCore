using System.Linq;
using Abstractions.Persistence;
using Persistence.EFCore.Contracts;
using Persistence.EFCore.Contracts.Seeding;

namespace Persistence.EFCore.Extensions
{
    public static class ContextExtensions
    {
        public static void ApplyContextualMigrationsFor<TEntity>(this AbstractContext context,
                                                                 IUnitOfWork unitOfWork,
                                                                 IFileSeedingService<TEntity> seedingService,
                                                                 string fileName)
            where TEntity : class, IDomainEntity
        {
            var fileEntities = seedingService.ReadFrom(fileName).ToList();
            var dbSet        = context.Set<TEntity>();

            var existingEntitiesOnDatabase = dbSet
                                            .Where(x => fileEntities
                                                       .Select(y => y.Id)
                                                       .Contains(x.Id))
                                            .ToList();

            var toBeAddedEntities = fileEntities
               .Where(x => !existingEntitiesOnDatabase
                           .Select(y => y.Id)
                           .Contains(x.Id));

            unitOfWork.Begin("SEEDED");
            unitOfWork.GetRepository<TEntity>().InsertRangeAsync(toBeAddedEntities).GetAwaiter().GetResult();
            unitOfWork.Commit();
        }
    }
}