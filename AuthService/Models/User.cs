namespace AuthService.Models
{
    public class User
    {
        public int Id { get; set; }

        // Kullanıcı adı (zorunlu değil ama login için faydalı olabilir)
        public string Username { get; set; } = string.Empty;

        // Email (unique olacak)
        public string Email { get; set; } = string.Empty;

        // Hashlenmiş şifre (plaintext asla saklanmaz!)
        public string PasswordHash { get; set; } = string.Empty;

        // Rol (örn: "User", "Admin")
        public string Role { get; set; } = "User";
    }
}
