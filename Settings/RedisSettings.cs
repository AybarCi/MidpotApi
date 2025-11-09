namespace DatingWeb.Settings
{
    public class RedisSettings
    {
        public string ConnectionString { get; set; } = "localhost:6379";
        public int DefaultExpiryMinutes { get; set; } = 60;
        public int UserProfileExpiryMinutes { get; set; } = 120;
        public int MatchResultsExpiryMinutes { get; set; } = 30;
        public int LocationDataExpiryMinutes { get; set; } = 15;
    }
}