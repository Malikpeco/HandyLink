using HandyLink.Services.Database.Entities;
using Microsoft.EntityFrameworkCore;


namespace HandyLink.Services.Database
{
    public partial class HandyLinkDbContext : DbContext
    {
        public HandyLinkDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Country> Countries {  get; set; }
        public DbSet<City> Cities {  get; set; }
        public DbSet<ServiceCategory> ServiceCategories {  get; set; }
        public DbSet<UserStatus> UserStatuses {  get; set; }
        public DbSet<User> Users {  get; set; }
        public DbSet<HandymanProfile> HandymanProfiles {  get; set; }
        public DbSet<HandymanServiceCategory> HandymanServiceCategories {  get; set; }
        public DbSet<Job> Jobs {  get; set; }


    }
}
