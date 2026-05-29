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
    public class JobCompletionMarkConfiguration : BaseEntityConfiguration<JobCompletionMark>
    {
        public override void Configure(EntityTypeBuilder<JobCompletionMark> builder)
        {
            base.Configure(builder);

            builder.ToTable("JobCompletionMarks");

            builder.HasOne(x => x.MarkedByUser)
                .WithMany(x => x.JobCompletionMarks)
                .HasForeignKey(x => x.MarkedByUserId);
            builder.HasOne(x => x.Job)
                .WithMany(x => x.JobCompletionMarks)
                .HasForeignKey(x => x.JobId);
            builder.HasIndex(x => new { x.JobId, x.MarkedByUserId })
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");
        }

    }
}
