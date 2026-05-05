using HandyLink.Services.Database.Enums;

namespace HandyLink.Services.Database.Entities
{
    public class User : BaseEntity
    {
        public string FirstName{ get; set; } = string.Empty;
        public string LastName{ get; set; } = string.Empty;
        public string Email{ get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string PasswordSalt { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? ProfileImageBase64 { get; set; }
        public UserType UserType { get; set; }
        public int CityId { get; set; }
        public City City { get; set; } = null!;
        public int UserStatusId { get; set; }
        public UserStatus UserStatus { get; set; } = null!;
        public ICollection<HandymanApplication> HandymanApplications { get; set; } = new List<HandymanApplication>();
        public HandymanProfile? HandymanProfile { get; set; }
        public ClientProfile? ClientProfile { get; set; }
        public AdminProfile? AdminProfile { get; set; }
        public ICollection<JobProposal> JobProposals { get; set; } = new List<JobProposal>();
        

    }
}
