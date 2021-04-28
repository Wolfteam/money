using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Money.Api.Models.Entities;

namespace Money.Api.Models.EntitiesConfiguration
{
    public class UserTypeConfiguration : BaseTypeConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);
            builder.Property(b => b.Name).HasMaxLength(100).IsRequired();
            builder.Property(b => b.LastName).HasMaxLength(100).IsRequired();
        }
    }
}
