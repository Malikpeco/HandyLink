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
    public class AdminProfileConfiguration : IEntityTypeConfiguration<AdminProfile>
    {
        public void Configure(EntityTypeBuilder<AdminProfile> builder)
        {
            builder.ToTable("AdminProfiles");
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.User)
            .WithOne(x => x.AdminProfile)
            .HasForeignKey<AdminProfile>(x => x.UserId);
        }
    }
}
