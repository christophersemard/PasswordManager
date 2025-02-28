using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class User
    {
        [Key] public int Id { get; set; }
        [Required] [EmailAddress] public string Email { get; set; } = string.Empty;
        [Required] public string Password { get; set; } = null!;
        [Required] public string Salt { get; set; }
    }
}