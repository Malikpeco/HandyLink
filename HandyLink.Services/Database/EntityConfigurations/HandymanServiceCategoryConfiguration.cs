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
    
    public class HandymanServiceCategoryConfiguration : IEntityTypeConfiguration<HandymanServiceCategory>
    {
        public void Configure(EntityTypeBuilder<HandymanServiceCategory> builder)
        {
            builder.ToTable("HandymanServiceCategories");
            builder.HasKey(x => x.Id);


            builder.HasOne(x => x.HandymanProfile)
                .WithMany(x => x.HandymanServiceCategories)
                .HasForeignKey(x => x.HandymanProfileId);

            builder.HasOne(x => x.ServiceCategory)
                .WithMany(x => x.HandymanServiceCategories)
                .HasForeignKey(x => x.ServiceCategoryId);

        }
    }
}
