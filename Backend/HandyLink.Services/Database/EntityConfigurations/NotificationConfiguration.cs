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
    public class NotificationConfiguration : BaseEntityConfiguration<Notification>
    {
        public override void Configure(EntityTypeBuilder<Notification> builder)
        {
            base.Configure(builder);

            builder.ToTable("Notifications");
            builder.Property(x => x.Title).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Content).IsRequired().HasMaxLength(200);
            builder.Property(x => x.IsRead).IsRequired();


            builder.HasOne(x => x.User)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.UserId);

            builder.HasOne(x => x.Job)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.JobId);
            
            builder.HasOne(x => x.Message)
                .WithOne(x => x.Notification)
                .HasForeignKey<Notification>(x => x.MessageId);

        }
    }
}
