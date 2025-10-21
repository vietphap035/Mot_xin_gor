
using System.ComponentModel.DataAnnotations;

namespace ShareModel
{
    public class User
    {
        [Key]
        public string UId { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public virtual ICollection<UserRoom> UserRooms { get; set; }

        public virtual ICollection<Messages> Messages { get; set; }
    }
}
