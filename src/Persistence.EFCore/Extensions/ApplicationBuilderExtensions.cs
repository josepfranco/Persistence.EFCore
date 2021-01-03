using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence.EFCore.Contracts;

namespace Persistence.EFCore.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// </summary>
        /// <param name="app"></param>
        public static void UseEfCoreMigrations<TContext>(this IApplicationBuilder app)
            where TContext : AbstractContext
        {
            MigrateDatabaseForContext<TContext>(app);
        }
        
        #region PRIVATE METHODS
        /// <summary>
        /// </summary>
        /// <param name="app"></param>
        /// <typeparam name="TContext"></typeparam>
        private static void MigrateDatabaseForContext<TContext>(IApplicationBuilder app)
            where TContext : AbstractContext
        {
            using var scope   = app.ApplicationServices.CreateScope();
            var       context = scope.ServiceProvider.GetRequiredService<TContext>();
            context.Database.Migrate();
        }
        #endregion
    }
}