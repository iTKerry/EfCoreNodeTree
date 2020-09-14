using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NodeTreeTest.DataAccess.EF;
using NodeTreeTest.DataAccess.EF.Common;

namespace NodeTreeTest.ConsoleApp
{
    public sealed class Startup
    {
        private readonly IServiceCollection _services;
        
        public Startup(IServiceCollection services) => 
            _services = services;

        public IServiceProvider BuildProvider() => 
            ConfigureServices(_services)
                .BuildServiceProvider();

        private IServiceCollection ConfigureServices(IServiceCollection services) =>
            services
                .AddTransient<DbConnectionString>()
                .AddLogging(builder => builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("Microsoft.EntityFrameworkCore.Database", LogLevel.Trace))
                .AddDbContext<AppDbContext>();
    }
}