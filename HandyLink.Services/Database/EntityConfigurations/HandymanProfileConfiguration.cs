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
    public class HandymanProfileConfiguration : IEntityTypeConfiguration<HandymanProfile>
    {
        public void Configure(EntityTypeBuilder<HandymanProfile> builder)
        {
            builder.ToTable("HandymanProfiles");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Bio).IsRequired().HasMaxLength(2500);
            
            builder.HasOne(x => x.User)
                .WithOne(x => x.HandymanProfile)
                .HasForeignKey<HandymanProfile>(x => x.UserId);

        }
    }
}
