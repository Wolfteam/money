using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Money.Api.Models.Entities;

namespace Money.Api.Models.EntitiesConfiguration
{
    public class BaseTypeConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property(b => b.CreatedBy).IsRequired().HasMaxLength(150);
            builder.Property(b => b.UpdatedBy).IsRequired(false).HasMaxLength(150);
        }
    }
}
