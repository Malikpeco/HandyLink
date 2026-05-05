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
    public class JobConfiguration : IEntityTypeConfiguration<Job>
    {
        public void Configure(EntityTypeBuilder<Job> builder)
        {
            builder.ToTable("Jobs");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Title).IsRequired().HasMaxLength(150);
            builder.Property(x => x.Description).IsRequired().HasMaxLength(2000);
            builder.Property(x => x.Address).HasMaxLength(200);
            builder.Property(x => x.JobCreationType).IsRequired();
            builder.Property(x => x.InitialPrice).HasPrecision(18,2);
            builder.Property(x => x.CurrentPrice).HasPrecision(18,2);
            builder.Property(x => x.InitialPriceOnArrangement).IsRequired();
            builder.Property(x => x.CurrentPriceOnArrangement).IsRequired();
            builder.Property(x => x.InitialScheduledAtUtc).IsRequired();
            builder.Property(x => x.CurrentScheduledAtUtc).IsRequired();
            builder.Property(x => x.InitialTimeFlexible).IsRequired();
            builder.Property(x => x.CurrentTimeFlexible).IsRequired();
            
            builder.HasOne(x=>x.ClientProfile)
                .WithMany(x=>x.Jobs)
                .HasForeignKey(x=>x.ClientProfileId);
            
            builder.HasOne(x=>x.HandymanProfile)
                .WithMany(x=>x.Jobs)
                .HasForeignKey(x=>x.HandymanProfileId);

            builder.HasOne(x=>x.ServiceCategory)
                .WithMany(x=>x.Jobs)
                .HasForeignKey(x=>x.ServiceCategoryId);

            builder.HasOne(x=>x.City)
                .WithMany(x=>x.Jobs)
                .HasForeignKey(x=>x.CityId);

            builder.HasOne(x=>x.JobStatus)
                .WithMany(x=>x.Jobs)
                .HasForeignKey(x=>x.JobStatusId);



        }
    }
}
