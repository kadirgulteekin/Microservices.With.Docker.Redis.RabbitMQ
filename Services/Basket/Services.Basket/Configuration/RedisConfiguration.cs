namespace Services.Basket.Configuration
{
    public class RedisConfiguration
    {
        public const string Key = "ReddisSettings";
        public string? Host { get; set; }
        public int Port { get; set; }
    }
}
