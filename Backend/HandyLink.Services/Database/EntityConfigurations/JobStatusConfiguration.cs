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
    public class JobStatusConfiguration : BaseEntityConfiguration<JobStatus>
    {
        public override void Configure(EntityTypeBuilder<JobStatus> builder)
        {
            base.Configure(builder);

            builder.ToTable("JobStatuses");
            builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
            builder.HasIndex(x => x.Name).IsUnique().HasFilter("[IsDeleted] = 0");
            builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
            builder.HasIndex(x => x.Code).IsUnique().HasFilter("[IsDeleted] = 0");
            builder.Property(x => x.Description).IsRequired().HasMaxLength(500);
        }
    }
}
