using HandyLink.Services.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Xml.Schema;


namespace HandyLink.Services.Database.EntityConfigurations
{
    public class CityConfiguration : BaseEntityConfiguration<City>
    {
        public override void Configure(EntityTypeBuilder<City> builder)
        {
            base.Configure(builder);
            builder.ToTable("Cities");
            builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
            builder.Property(x => x.CountryId).IsRequired();
            builder.HasIndex(x => new { x.Name,x.CountryId }).IsUnique().HasFilter("[IsDeleted] = 0");

            builder.HasOne(x => x.Country)
                .WithMany(x => x.Cities)
                .HasForeignKey(x => x.CountryId);

        }
    }
}
