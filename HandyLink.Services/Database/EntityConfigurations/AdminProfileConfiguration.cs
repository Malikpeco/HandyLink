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
    public class AdminProfileConfiguration : BaseEntityConfiguration<AdminProfile>
    {
        public override void Configure(EntityTypeBuilder<AdminProfile> builder)
        {
            base.Configure(builder);

            builder.ToTable("AdminProfiles");

            builder.HasOne(x => x.User)
            .WithOne(x => x.AdminProfile)
            .HasForeignKey<AdminProfile>(x => x.UserId);
        }
    }
}
