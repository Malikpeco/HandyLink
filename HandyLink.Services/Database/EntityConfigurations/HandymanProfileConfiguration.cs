using HandyLink.Services.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Database.EntityConfigurations
{
    public class HandymanProfileConfiguration : BaseEntityConfiguration<HandymanProfile>
    {
        public override void Configure(EntityTypeBuilder<HandymanProfile> builder)
        {
            base.Configure(builder);

            builder.ToTable("HandymanProfiles");
            builder.Property(x => x.Bio).IsRequired().HasMaxLength(2500);
            
            builder.HasOne(x => x.User)
                .WithOne(x => x.HandymanProfile)
                .HasForeignKey<HandymanProfile>(x => x.UserId);

        }
    }
}
