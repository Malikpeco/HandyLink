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
    public class HandymanApplicationPhotoConfiguration : BaseEntityConfiguration<HandymanApplicationPhoto>
    {
        public override void Configure(EntityTypeBuilder<HandymanApplicationPhoto> builder)
        {
            base.Configure(builder);
            builder.ToTable("HandymanApplicationPhotos");
            builder.Property(x => x.ImageBase64).IsRequired();

            builder.HasOne(x => x.HandymanApplication)
                .WithMany(x => x.HandymanApplicationPhotos)
                .HasForeignKey(x => x.HandymanApplicationId);

        }
    }
}
