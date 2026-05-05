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
    public class HandymanApplicationConfiguration : IEntityTypeConfiguration<HandymanApplication>
    {
        public void Configure(EntityTypeBuilder<HandymanApplication> builder)
        {
            builder.ToTable("HandymanApplications");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ExperienceYears).IsRequired();
            builder.Property(x => x.WorkDescription).IsRequired().HasMaxLength(1000);

            builder.HasOne(x => x.User)
                .WithMany(x => x.HandymanApplications)
                .HasForeignKey(x => x.UserId);



        }
    }
}
