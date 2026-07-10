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
    public class ClientProfileConfiguration : BaseEntityConfiguration<ClientProfile>
    {
        public override void Configure(EntityTypeBuilder<ClientProfile> builder)
        {
            base.Configure(builder);
            builder.ToTable("ClientProfiles");

            builder.HasOne(x => x.User)
            .WithOne(x => x.ClientProfile)
            .HasForeignKey<ClientProfile>(x => x.UserId);
        }
    }
}
