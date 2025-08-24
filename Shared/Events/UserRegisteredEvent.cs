namespace Shared.Events
{
    public class UserRegisteredEvent
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
    }
}
