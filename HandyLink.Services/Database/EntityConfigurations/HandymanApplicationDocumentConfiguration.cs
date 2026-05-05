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
    public class HandymanApplicationDocumentConfiguration : IEntityTypeConfiguration<HandymanApplicationDocument>
    {
        public void Configure(EntityTypeBuilder<HandymanApplicationDocument> builder)
        {
            builder.ToTable("HandymanApplicationDocuments");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.FileUrl).IsRequired();
            builder.Property(x => x.FileName).IsRequired();
            builder.Property(x => x.ContentType).IsRequired();

            builder.HasOne(x => x.HandymanApplication)
                .WithMany(x => x.HandymanApplicationDocuments)
                .HasForeignKey(x => x.HandymanApplicationId);

        }
    }
}
