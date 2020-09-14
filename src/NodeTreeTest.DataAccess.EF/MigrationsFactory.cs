using Microsoft.EntityFrameworkCore.Design;
using NodeTreeTest.DataAccess.EF.Common;

namespace NodeTreeTest.DataAccess.EF
{
    public class MigrationsFactory: IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var connectionStrings = new DbConnectionString();
            return new AppDbContext(connectionStrings, null);
        }
    }
}