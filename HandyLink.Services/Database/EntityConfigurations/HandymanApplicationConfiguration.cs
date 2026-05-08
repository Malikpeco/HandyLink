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
    public class HandymanApplicationConfiguration : BaseEntityConfiguration<HandymanApplication>
    {
        public override void Configure(EntityTypeBuilder<HandymanApplication> builder)
        {
            base.Configure(builder);
            builder.ToTable("HandymanApplications");
            builder.Property(x => x.ExperienceYears).IsRequired();
            builder.Property(x => x.WorkDescription).IsRequired().HasMaxLength(1000);

            builder.HasOne(x => x.User)
                .WithMany(x => x.HandymanApplications)
                .HasForeignKey(x => x.UserId);



        }
    }
}
