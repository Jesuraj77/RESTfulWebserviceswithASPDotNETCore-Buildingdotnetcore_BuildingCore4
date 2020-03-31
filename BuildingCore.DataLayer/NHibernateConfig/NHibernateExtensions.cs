using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;

namespace BuildingCore.DataLayer.NHibernateConfigurations
{

    public static class NHibernateExtensions
    {
        public static IServiceCollection AddNHibernate(this IServiceCollection services, string connectionString, string createDataBase)
        {
            string dataBaseName = GetDatabaseNameFromConnectionString(connectionString);
            CreateDatabase(dataBaseName, createDataBase);

            var sessionFactory = Fluently.Configure()
                 .Database(MySQLConfiguration.Standard
                 .ConnectionString(connectionString))
                 .Mappings(m => m.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly()))
                 .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Mappings>())
                 .CurrentSessionContext("call")
                 .ExposeConfiguration(cfg => BuildSchema(cfg, true, true)).BuildSessionFactory();

            services.AddSingleton(sessionFactory);
            services.AddScoped(factory => sessionFactory.OpenSession());
            services.AddScoped<IMapperSession, NHibernateMapperSession>();

            return services;
        }

        private static void BuildSchema(Configuration config, bool create = false, bool update = false)
        {

            if (create)
            {
                new SchemaExport(config).Create(false, true);
            }
            else
            {
                new SchemaUpdate(config).Execute(false, update);
            }
        }

        private static string GetDatabaseNameFromConnectionString(string connectionString)
        {
            string dbNameFetch = connectionString;
            int pFrom = dbNameFetch.IndexOf("Catalog=") + "Catalog=".Length;
            int pTo = dbNameFetch.LastIndexOf(";User ID");
            return dbNameFetch.Substring(pFrom, pTo - pFrom);
        }

        private static void CreateDatabase(string dataBaseName, string createDataBase)
        {
            MySqlConnection conn = new MySqlConnection(createDataBase);

            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand($"CREATE DATABASE IF NOT EXISTS {dataBaseName}", conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }
    }

}