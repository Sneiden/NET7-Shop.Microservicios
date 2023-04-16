using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Common;
using Ordering.Domain.Entities;
using System.Linq.Expressions;
using System.Reflection;

namespace Ordering.Infrastructure.Persistence
{
    public class OrderContext : DbContext
    {
        public OrderContext(DbContextOptions<OrderContext> contextOptions) : base(contextOptions)
        {

        }

        public DbSet<Order> Orders { get; set; }
        private Guid TendantId { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<EntityBase>()) 
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.Entity.CreatedBy = "User";
                        entry.Entity.CreatedDate = DateTime.UtcNow;
                        break;
                    case EntityState.Added: 
                        entry.Entity.LastModifiedBy = "User";
                        entry.Entity.LastModifiedDate = DateTime.UtcNow;
                        break;
                }
            }

            foreach (var item in ChangeTracker.Entries().Where(e=> e.State == EntityState.Added 
            && e.Entity is IMultiTenant))
            {
                var entity = item.Entity as IMultiTenant;
                entity!.TenantId = Guid.Empty;
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //iteramos todas las creeaciones de los objetos que se han hecho
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                //obtenemos el tipo de cada uno de ellos
                var entityType = entity.ClrType;

                //si ellos no son multiTenant, continuamos
                if (!typeof(IMultiTenant).IsAssignableFrom(entityType)) continue;

                //si es multiTenant se le aplicara el filtro
                //obtenemos de manera generica el methodo, usando reflexion
                var method = typeof(OrderContext).GetMethod(nameof(MultiTenantExpression),
                    BindingFlags.NonPublic | BindingFlags.Static)?.MakeGenericMethod(entityType);

                //hacemos un filtro
                var filter = method?.Invoke(null, new object[] { this });

                //se agrega el filtro al querty que se desea ejecutar
                entity.SetQueryFilter((LambdaExpression)filter!);

                //se crea un idex al TenantId para no afectar el performance
                entity.AddIndex(entity.FindProperty(nameof(IMultiTenant.TenantId))!);
            }
        }

        private static LambdaExpression MultiTenantExpression<T>(OrderContext context)
            where T: EntityBase, IMultiTenant
        {
            Expression<Func<T, bool>> tenantFilter = x => x.TenantId == context.TendantId;
            return tenantFilter;
        }
    }
}
