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
    public class HandymanApplicationServiceCategoryConfiguration : IEntityTypeConfiguration<HandymanApplicationServiceCategory>
    {
        public void Configure(EntityTypeBuilder<HandymanApplicationServiceCategory> builder)
        {
            builder.ToTable("HandymanApplicationServiceCategories");
            builder.HasKey(x => x.Id);


            builder.HasOne(x => x.HandymanApplication)
                .WithMany(x => x.HandymanApplicationServiceCategories)
                .HasForeignKey(x => x.HandymanApplicationId);

            builder.HasOne(x => x.ServiceCategory)
                .WithMany(x => x.HandymanApplicationServiceCategories)
                .HasForeignKey(x => x.ServiceCategoryId);


        }
    }
}
