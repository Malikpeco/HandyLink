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
    public class HandymanApplicationReferenceConfiguration : IEntityTypeConfiguration<HandymanApplicationReference>
    {
        public void Configure(EntityTypeBuilder<HandymanApplicationReference> builder)
        {
            builder.ToTable("HandymanApplicationReferences");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.FirstName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.LastName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.PhoneNumber).HasMaxLength(20);
            builder.Property(x => x.Email).IsRequired().HasMaxLength(200);
            builder.Property(x => x.ReferenceNote).IsRequired().HasMaxLength(2000);

            builder.HasOne(x => x.HandymanApplication)
                .WithMany(x => x.HandymanApplicationReferences)
                .HasForeignKey(x => x.HandymanApplicationId);

        }
    }
}
