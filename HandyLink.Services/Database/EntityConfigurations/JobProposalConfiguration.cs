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
    public class JobProposalConfiguration : BaseEntityConfiguration<JobProposal>
    {
        public override void Configure(EntityTypeBuilder<JobProposal> builder)
        {
            base.Configure(builder);

            builder.ToTable("JobProposals");
            builder.Property(x => x.ProposedPrice).HasPrecision(18, 2);
            builder.Property(x => x.ProposedPriceOnArrangement).IsRequired();
            builder.Property(x => x.ProposedScheduledAtUtc).IsRequired();
            builder.Property(x => x.ProposedTimeFlexible).IsRequired();
            builder.Property(x => x.JobProposalStatus).IsRequired();


            builder.HasOne(x => x.Job)
                .WithMany(x => x.JobProposals)
                .HasForeignKey(x => x.JobId);

            builder.HasOne(x => x.ProposedByUser)
                .WithMany(x => x.JobProposals)
                .HasForeignKey(x => x.ProposedByUserId);
        }
    }
}
