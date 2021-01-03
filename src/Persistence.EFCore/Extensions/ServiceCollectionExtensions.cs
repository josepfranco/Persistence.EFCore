using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.EFCore.Configuration;
using Persistence.EFCore.Contracts;

namespace Persistence.EFCore.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddEfCore<TContext>(this IServiceCollection services, IConfiguration configuration)
            where TContext : AbstractContext
        {
            // load database & contexts
            var databaseConfig = AddDatabaseConfiguration(services, configuration);
            AddContextFor<TContext>(services, configuration, databaseConfig);
        }

        #region PRIVATE METHODS
        /// <summary>
        /// </summary>
        /// <param name="databaseConfig"></param>
        /// <param name="contextConfig"></param>
        /// <returns></returns>
        private static string GenerateConnectionString(DatabaseConfiguration databaseConfig,
                                                       ContextConfiguration contextConfig)
        {
            var keyValuePairs = new List<string>
            {
                $"{nameof(databaseConfig.Host)}={databaseConfig.Host}",
                $"{nameof(databaseConfig.Port)}={databaseConfig.Port}"
            };

            if (!string.IsNullOrEmpty(contextConfig.Username))
                keyValuePairs.Add($"{nameof(contextConfig.Username)}={contextConfig.Username}");

            if (!string.IsNullOrEmpty(contextConfig.Password))
                keyValuePairs.Add($"{nameof(contextConfig.Password)}={contextConfig.Password}");

            if (!string.IsNullOrEmpty(contextConfig.Database))
                keyValuePairs.Add($"{nameof(contextConfig.Database)}={contextConfig.Database}");

            var connectionString = string.Join(";", keyValuePairs);
            return connectionString;
        }

        /// <summary>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        private static DatabaseConfiguration AddDatabaseConfiguration(this IServiceCollection services,
                                                                      IConfiguration configuration)
        {
            var host = configuration[$"{nameof(DatabaseConfiguration)}:Host"];
            var port = configuration[$"{nameof(DatabaseConfiguration)}:Port"];

            return new DatabaseConfiguration
            {
                Host = host,
                Port = int.Parse(port)
            };
        }

        /// <summary>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="databaseConfig"></param>
        /// <typeparam name="TContext"></typeparam>
        private static void AddContextFor<TContext>(IServiceCollection services,
                                                    IConfiguration configuration,
                                                    DatabaseConfiguration databaseConfig)
            where TContext : DbContext
        {
            var user =
                configuration[$"ConnectionStrings:{typeof(TContext).Name}:{nameof(ContextConfiguration.Username)}"];
            var password =
                configuration[$"ConnectionStrings:{typeof(TContext).Name}:{nameof(ContextConfiguration.Password)}"];
            var database =
                configuration[$"ConnectionStrings:{typeof(TContext).Name}:{nameof(ContextConfiguration.Database)}"];

            services.AddDbContext<TContext>(b =>
                                                UseNpgsqlProvider(b, databaseConfig, new ContextConfiguration
                                                {
                                                    Database = database,
                                                    Username = user,
                                                    Password = password
                                                }));
        }

        /// <summary>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="databaseConfig"></param>
        /// <param name="contextConfig"></param>
        /// <returns></returns>
        private static void UseNpgsqlProvider(DbContextOptionsBuilder builder,
                                              DatabaseConfiguration databaseConfig,
                                              ContextConfiguration contextConfig)
        {
            var connectionString = GenerateConnectionString(databaseConfig, contextConfig);
            builder.UseNpgsql(connectionString);
        }
        #endregion
    }
}