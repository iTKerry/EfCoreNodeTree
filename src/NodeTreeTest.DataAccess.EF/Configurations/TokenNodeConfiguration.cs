using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NodeTreeTest.DataAccess.Abstractions.Entities;
using NodeTreeTest.DataAccess.EF.Metadata;

namespace NodeTreeTest.DataAccess.EF.Configurations
{
    public class TokenNodeConfiguration : IEntityTypeConfiguration<TokenNode>
    {
        public void Configure(EntityTypeBuilder<TokenNode> builder)
        {
            builder.ToTable(Tables.TokenNode, Schemas.Dbo).HasKey(p => p.Id);
            
            builder
                .HasOne(p => p.Parent)
                .WithMany(p => p.ChildrenTokens)
                .IsRequired();

            builder
                .HasOne(p => p.Child)
                .WithMany(p => p.ParentTokens)
                .IsRequired();
        }
    }
}