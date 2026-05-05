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
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.FirstName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.LastName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Email).IsRequired().HasMaxLength(200);
            builder.Property(x => x.PasswordHash).IsRequired().HasMaxLength(200);
            builder.Property(x => x.PasswordSalt).IsRequired().HasMaxLength(200);
            builder.Property(x => x.PhoneNumber).IsRequired().HasMaxLength(20);
            builder.Property(x => x.UserType).IsRequired();
            
            builder.HasOne(x => x.City)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.CityId);

            builder.HasOne(x => x.UserStatus)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.UserStatusId);

        }
    }
}
