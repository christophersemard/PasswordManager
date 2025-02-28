using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class PasswordEntry
    {
        [Key] public int Id { get; set; }
        [Required] public string Title { get; set; } = string.Empty;
        [Required] public string Username { get; set; } = string.Empty;
        public string EncryptedPassword { get; set; } = string.Empty;
        [NotMapped] [Required] public string DecryptedPassword { get; set; } = string.Empty;
        public string Url { get; set; }  = null!;
        
        [ForeignKey("Category")] public int CategoryId { get; set; }
        public Category Category { get; set; }  = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string Notes { get; set; } 

        
        [ForeignKey("User")] public int UserId { get; set; }
        public User User { get; set; }  = null!;
    }
}