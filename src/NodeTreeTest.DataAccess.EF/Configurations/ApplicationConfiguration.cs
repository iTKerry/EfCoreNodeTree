using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NodeTreeTest.DataAccess.Abstractions.Entities;
using NodeTreeTest.DataAccess.EF.Metadata;

namespace NodeTreeTest.DataAccess.EF.Configurations
{
    public class ApplicationConfiguration : IEntityTypeConfiguration<Application>
    {
        public void Configure(EntityTypeBuilder<Application> builder)
        {
            builder.ToTable(Tables.Application, Schemas.Dbo).HasKey(p => p.Id);

            builder.Property(p => p.Id).ValueGeneratedNever();
            builder.Property(p => p.Name);
        }
    }
}