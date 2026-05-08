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
    public class HandymanApplicationServiceCategoryConfiguration : BaseEntityConfiguration<HandymanApplicationServiceCategory>
    {
        public override void Configure(EntityTypeBuilder<HandymanApplicationServiceCategory> builder)
        {
            base.Configure(builder);

            builder.ToTable("HandymanApplicationServiceCategories");


            builder.HasOne(x => x.HandymanApplication)
                .WithMany(x => x.HandymanApplicationServiceCategories)
                .HasForeignKey(x => x.HandymanApplicationId);

            builder.HasOne(x => x.ServiceCategory)
                .WithMany(x => x.HandymanApplicationServiceCategories)
                .HasForeignKey(x => x.ServiceCategoryId);


        }
    }
}
