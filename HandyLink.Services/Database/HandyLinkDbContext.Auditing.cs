using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Database
{
    public partial class HandyLinkDbContext
    {
        private DateTime UtcNow => DateTime.UtcNow;

        public override int SaveChanges()
        {
            ApplyAuditAndSoftDelete();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditAndSoftDelete();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyAuditAndSoftDelete()
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>()) 
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAtUtc = UtcNow;
                        entry.Entity.ModifiedAtUtc = null;
                        entry.Entity.IsDeleted = false;
                        break;

                    case EntityState.Modified:
                        entry.Entity.ModifiedAtUtc= UtcNow;
                        entry.Property(x=>x.CreatedAtUtc).IsModified = false;
                        break;

                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.Entity.ModifiedAtUtc = UtcNow;
                        entry.Entity.IsDeleted = true;
                        entry.Property(x => x.CreatedAtUtc).IsModified = false;
                        break;
                }
            }
        }

        private void ApplySoftDeleteQueryFilters(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");

                    var property = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));

                    var compare = Expression.Equal(property, Expression.Constant(false));

                    var lambda = Expression.Lambda(compare, parameter);

                    modelBuilder.Entity(entityType.ClrType)
                        .HasQueryFilter(lambda);
                }
            }
        }
    }
}
