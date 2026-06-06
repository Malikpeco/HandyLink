using HandyLink.Services.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace HandyLink.Services.Database.EntityConfigurations
{
    public class RefreshTokenConfiguration : BaseEntityConfiguration<RefreshToken>
    {
        public override void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            base.Configure(builder);
            builder.ToTable("RefreshTokens");
            builder.Property(x => x.Token).IsRequired().HasMaxLength(500);
            builder.Property(x => x.ExpiresAt).IsRequired();
            builder.Property(x => x.UserId).IsRequired();

            builder.HasOne(x => x.User)
            .WithMany(x => x.RefreshTokens)
            .HasForeignKey(x => x.UserId);
        }
    }
}
