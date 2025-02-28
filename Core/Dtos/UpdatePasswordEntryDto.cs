namespace Core.Dtos
{
    public class UpdatePasswordEntryDto
    {
        public string? Title { get; set; }
        public string? Username { get; set; }
        public string? EncryptedPassword { get; set; }
        public string? Url { get; set; }
        public int? CategoryId { get; set; }
        public string? Notes { get; set; }
    }
}