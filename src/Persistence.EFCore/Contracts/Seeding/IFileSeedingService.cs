using System.Collections.Generic;
using Abstractions.Persistence;

namespace Persistence.EFCore.Contracts.Seeding
{
    public interface IFileSeedingService<out TEntity> where TEntity : class, IDomainEntity
    {
        /// <summary>
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        IEnumerable<TEntity> ReadFrom(string fileName);
    }
}