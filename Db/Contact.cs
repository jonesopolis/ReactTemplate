using System.ComponentModel.DataAnnotations;

namespace Db
{
    public sealed class Contact
    {
        public long Id { get; set; }
        public User User { get; set; }

        [Required]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Phone]
        public string Phone { get; set; }
    }
}