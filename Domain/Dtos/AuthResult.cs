namespace Domain.Dtos
{
    public class AuthResult
    {
        public string AccessToken { get; set; } = null!;
        public bool IsAdmin { get; set; }
        public string? ApiKey { get; set; }
    }
}
