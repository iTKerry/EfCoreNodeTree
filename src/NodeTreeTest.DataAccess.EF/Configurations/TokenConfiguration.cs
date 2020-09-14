using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NodeTreeTest.DataAccess.Abstractions.Entities;
using NodeTreeTest.DataAccess.EF.Metadata;

namespace NodeTreeTest.DataAccess.EF.Configurations
{
    public class TokenConfiguration : IEntityTypeConfiguration<Token>
    {
        public void Configure(EntityTypeBuilder<Token> builder)
        {
            builder.ToTable(Tables.Token, Schemas.Dbo).HasKey(p => p.Id);

            builder.Property(p => p.Query).IsRequired();
            builder.Property(p => p.NodeType).IsRequired();
            
            builder.Property(p => p.QueryParameter);
            builder.Property(p => p.DocumentType);

            builder.OwnsOne(p => p.Name, x =>
            {
                x.Property(pp => pp.Value).HasColumnName("Name");
            });
            
            builder.HasOne(p => p.Application).WithMany();
            
            builder
                .HasMany(p => p.ParentTokens)
                .WithOne(p => p.Child)
                .OnDelete(DeleteBehavior.NoAction)
                .Metadata.PrincipalToDependent.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder
                .HasMany(p => p.ChildrenTokens)
                .WithOne(p => p.Parent)
                .OnDelete(DeleteBehavior.NoAction)
                .Metadata.PrincipalToDependent.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}