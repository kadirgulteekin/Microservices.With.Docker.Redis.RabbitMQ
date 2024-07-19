namespace Services.Catalog.Configuration
{
    public class DatabaseConfigurations : IDatabaseConfigurations
    {
        public const string Key = "DatabaseSettings";
        public string? CourceCollectionName { get; set; }
        public string? CategoryCollectionName { get; set; }
        public string? ConnectionString { get; set; }
        public string? DatabaseName { get; set; }
    }
}
