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
    public class ReviewConfiguration : BaseEntityConfiguration<Review>
    {
        public override void Configure(EntityTypeBuilder<Review> builder)
        {
            base.Configure(builder);

            builder.ToTable("Reviews");
            
            builder.Property(x => x.Comment).HasMaxLength(1000);
            builder.Property(x => x.Rating).IsRequired();
            builder.ToTable(t => t.HasCheckConstraint("CK_Reviews_Rating", "[Rating] >=1 AND [Rating] <=5"));


            builder.HasIndex(x => x.JobId).IsUnique().HasFilter("[IsDeleted] = 0");

            builder.HasOne(x => x.Job)
                .WithOne(x => x.Review)
                .HasForeignKey<Review>(x => x.JobId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(x => x.ClientProfile)
                .WithMany(x => x.Reviews)
                .HasForeignKey(x => x.ClientProfileId)
                .OnDelete(DeleteBehavior.Restrict);



            builder.HasOne(x => x.HandymanProfile)
                .WithMany(x => x.Reviews)
                .HasForeignKey(x => x.HandymanProfileId)
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
