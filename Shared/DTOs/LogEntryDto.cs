namespace Shared.DTOs
{
    public class LogEntryDto
    {
        public string Service { get; set; } = string.Empty;   // ProductService, AuthService vs.
        public string Level { get; set; } = "Info";          // Info, Error, Warning
        public string Message { get; set; } = string.Empty;  // Örn: "Product created"
        public string? UserId { get; set; }                  // Kullanıcı id’si varsa
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
