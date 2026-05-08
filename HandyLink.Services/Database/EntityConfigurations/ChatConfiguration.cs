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
    public class ChatConfiguration : BaseEntityConfiguration<Chat>
    {
        public override void Configure(EntityTypeBuilder<Chat> builder)
        {
            base.Configure(builder);

            builder.ToTable("Chats");

            builder.HasOne(x => x.Job)
                .WithOne(x => x.Chat)
                .HasForeignKey<Chat>(x => x.JobId);

        }
    }
}
