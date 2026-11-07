using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaSystem.Models
{
    public class ApplicationUserOTP
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser applicationUser { get; set; }
        public string OTP { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpirationTime { get; set; }
        public bool IsUsed { get; set; }
        public ApplicationUserOTP()
        {

        }
        public ApplicationUserOTP(string ApplicationUserId, string Otp)
        {
            this.ApplicationUserId = ApplicationUserId;
            this.OTP = Otp;
            this.CreatedAt = DateTime.Now;
            this.ExpirationTime = DateTime.Now.AddMinutes(5);
            this.IsUsed = false;
        }
    }
    
}
