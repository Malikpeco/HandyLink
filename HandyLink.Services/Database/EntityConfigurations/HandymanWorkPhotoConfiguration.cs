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
    public class HandymanWorkPhotoConfiguration : IEntityTypeConfiguration<HandymanWorkPhoto>
    {
        public void Configure(EntityTypeBuilder<HandymanWorkPhoto> builder)
        {
            builder.ToTable("HandymanWorkPhotos");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ImageBase64).IsRequired();

            builder.HasOne(x => x.HandymanProfile)
                .WithMany(x => x.HandymanWorkPhotos)
                .HasForeignKey(x => x.HandymanProfileId);

        }
    }
}
