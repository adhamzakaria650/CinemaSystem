using System.ComponentModel.DataAnnotations;

namespace CinemaSystem.ViewModels.Identity
{
    public class ResetPasswordVM
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password),Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
