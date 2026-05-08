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
    public class JobConfiguration : BaseEntityConfiguration<Job>
    {
        public override void Configure(EntityTypeBuilder<Job> builder)
        {
            base.Configure(builder);

            builder.ToTable("Jobs");
            
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

            builder.HasOne(x => x.ClientProfile)
                .WithMany(x => x.Jobs)
                .HasForeignKey(x => x.ClientProfileId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.HasOne(x=>x.HandymanProfile)
                .WithMany(x=>x.Jobs)
                .HasForeignKey(x=>x.HandymanProfileId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(x=>x.ServiceCategory)
                .WithMany(x=>x.Jobs)
                .HasForeignKey(x=>x.ServiceCategoryId)
                .OnDelete(DeleteBehavior.Restrict);



            builder.HasOne(x=>x.City)
                .WithMany(x=>x.Jobs)
                .HasForeignKey(x=>x.CityId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(x=>x.JobStatus)
                .WithMany(x=>x.Jobs)
                .HasForeignKey(x=>x.JobStatusId)
                .OnDelete(DeleteBehavior.Restrict);




        }
    }
}
