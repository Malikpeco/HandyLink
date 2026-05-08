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
    public class UserStatusConfiguration : BaseEntityConfiguration<UserStatus>
    {
        public override void Configure(EntityTypeBuilder<UserStatus> builder)
        {
            base.Configure(builder);

            builder.ToTable("UserStatuses");
           
            builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Description).IsRequired().HasMaxLength(500);
        }
    }
}
