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
    public class JobCancellationMarkConfiguration : BaseEntityConfiguration<JobCancellationMark>
    {
        public override void Configure(EntityTypeBuilder<JobCancellationMark> builder)
        {
            base.Configure(builder);
            builder.ToTable("JobCancellationMarks");


            builder.HasOne(x => x.MarkedByUser)
                .WithMany(x => x.JobCancellationMarks)
                .HasForeignKey(x => x.MarkedByUserId);
            builder.HasOne(x => x.Job)
                .WithMany(x => x.JobCancellationMarks)
                .HasForeignKey(x => x.JobId);
            builder.HasIndex(x => new { x.JobId, x.MarkedByUserId })
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");
        }

    }
}
