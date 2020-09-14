using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodeTreeTest.DataAccess.Abstractions.Entities;
using NodeTreeTest.DataAccess.EF.Common;

namespace NodeTreeTest.DataAccess.EF
{
    public class AppDbContext : DbContext
    {
        private static readonly Type[] EnumerationTypes =
        {
            typeof(Application)
        };

        private readonly DbConnectionString _connectionString;
        private readonly ILoggerFactory _loggerFactory;

        public AppDbContext(DbConnectionString connectionString, ILoggerFactory loggerFactory)
        {
            _connectionString = connectionString;
            _loggerFactory = loggerFactory;
        }

        public DbSet<Token> Tokens { get; set; }
        public DbSet<TokenNode> TokenNodes { get; set; }
        public DbSet<Application> Applications { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLoggerFactory(_loggerFactory)
                .EnableSensitiveDataLogging()
                .UseSqlServer(_connectionString.Value, builder => builder
                    .EnableRetryOnFailure(5, TimeSpan.FromSeconds(5), default)
                    .MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            ChangeTracker.Entries()
                .Where(x => EnumerationTypes.Contains(x.Entity.GetType()))
                .ToList()
                .ForEach(entity => entity.State = EntityState.Unchanged);

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}