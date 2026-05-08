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
    public class HandymanWorkPhotoConfiguration : BaseEntityConfiguration<HandymanWorkPhoto>
    {
        public override void Configure(EntityTypeBuilder<HandymanWorkPhoto> builder)
        {
            base.Configure(builder);
            builder.ToTable("HandymanWorkPhotos");

            builder.Property(x => x.ImageBase64).IsRequired();

            builder.HasOne(x => x.HandymanProfile)
                .WithMany(x => x.HandymanWorkPhotos)
                .HasForeignKey(x => x.HandymanProfileId);

        }
    }
}
