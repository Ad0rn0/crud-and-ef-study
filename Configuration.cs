namespace Blog;

public static class Configuration
{
    // TOKEN - JWT = Json Web Token
    public static string JwtKey = "ZRQ3IazEV0KGlNTwh0wJYQ==";
    public static string ApiKeyName = "api_key";
    public static string ApiKey = "curso_api_IlTevUM/z0ey3NwCV/unWg==";
    public static SmptConfiguration Smtp = new();

    public class SmptConfiguration
    {
        public string Host { get; set; } = null!;
        public int Port { get; set; } = 25;
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}