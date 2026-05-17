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
    public class ServiceCategoryConfiguration : BaseEntityConfiguration<ServiceCategory>
    {
        public override void Configure(EntityTypeBuilder<ServiceCategory> builder)
        {
            base.Configure(builder);

            builder.ToTable("ServiceCategories");
            
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.HasIndex(x => x.Name).IsUnique().HasFilter("[IsDeleted] = 0");
            builder.Property(x => x.Description).HasMaxLength(200);
            builder.Property(x=>x.IsActive).HasDefaultValue(true);

            


        }
    }
}
