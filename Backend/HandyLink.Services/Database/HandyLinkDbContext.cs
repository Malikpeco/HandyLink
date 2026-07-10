using HandyLink.Services.Database.Entities;
using Microsoft.EntityFrameworkCore;


namespace HandyLink.Services.Database
{
    public partial class HandyLinkDbContext : DbContext
    {
        public HandyLinkDbContext(DbContextOptions<HandyLinkDbContext> options) : base(options)
        {
        }

        public DbSet<AdminProfile> AdminProfiles {  get; set; }
        public DbSet<Chat> Chats {  get; set; }
        public DbSet<City> Cities {  get; set; }
        public DbSet<ClientProfile> ClientProfiles {  get; set; }
        public DbSet<Country> Countries {  get; set; }
        public DbSet<HandymanApplication> HandymanApplications {  get; set; }
        public DbSet<HandymanApplicationDocument> HandymanApplicationDocuments {  get; set; }
        public DbSet<HandymanApplicationPhoto> HandymanApplicationPhotos {  get; set; }
        public DbSet<HandymanApplicationReference> HandymanApplicationReferences {  get; set; }
        public DbSet<HandymanApplicationServiceCategory> HandymanApplicationServiceCategories {  get; set; }
        public DbSet<HandymanProfile> HandymanProfiles {  get; set; }
        public DbSet<HandymanServiceCategory> HandymanServiceCategories {  get; set; }
        public DbSet<HandymanWorkPhoto> HandymanWorkPhotos {  get; set; }
        public DbSet<Job> Jobs {  get; set; }
        public DbSet<JobCancellationMark> JobCancellationMarks {  get; set; }
        public DbSet<JobCompletionMark> JobCompletionMarks {  get; set; }
        public DbSet<JobProposal> JobProposals {  get; set; }
        public DbSet<JobStatus> JobStatuses {  get; set; }
        public DbSet<Message> Messages {  get; set; }
        public DbSet<Notification> Notifications {  get; set; }
        public DbSet<Review> Reviews {  get; set; }
        public DbSet<ServiceCategory> ServiceCategories {  get; set; }
        public DbSet<User> Users {  get; set; }
        public DbSet<UserStatus> UserStatuses {  get; set; }
        public DbSet<RefreshToken> RefreshTokens {  get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(HandyLinkDbContext).Assembly);

            ApplySoftDeleteQueryFilters(modelBuilder);

            CreateSeed(modelBuilder);   


        }

    }
}
