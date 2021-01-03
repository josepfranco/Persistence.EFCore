using System;
using System.Collections.Generic;
using Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Persistence.EFCore.Contracts
{
    public abstract class AbstractContext : DbContext
    {
        protected AbstractContext(DbContextOptions options) : base(options)
        {
            Options = options;
        }

        public DbContextOptions Options { get; }

        public void SaveChanges(string auditOwner)
        {
            var entries = ChangeTracker.Entries();
            PopulateAuditFields(auditOwner, entries);
            base.SaveChanges();
        }

        private static void PopulateAuditFields(string auditOwner, IEnumerable<EntityEntry> entries)
        {
            var now = DateTime.UtcNow;
            foreach (var entry in entries)
            {
                var errorLog = $"Processed entity is inheriting {nameof(IAuditable)}: [{entry.Entity.GetType().Name}]";
                var entity   = entry.Entity as IAuditable ?? throw new InvalidOperationException(errorLog);
                switch (entry.State)
                {
                    case EntityState.Added:
                        UpdateCreateAudit(auditOwner, entity, now);
                        UpdateModifiedAudit(auditOwner, entity, now);
                        break;
                    case EntityState.Modified:
                        UpdateModifiedAudit(auditOwner, entity, now);
                        break;
                }
            }
        }

        private static void UpdateModifiedAudit(string auditOwner, IAuditable entity, DateTime now)
        {
            entity.ModifiedAt = now;
            entity.ModifiedBy = auditOwner;
        }

        private static void UpdateCreateAudit(string auditOwner, IAuditable entity, DateTime now)
        {
            entity.CreatedAt = now;
            entity.CreatedBy = auditOwner;
        }
    }
}