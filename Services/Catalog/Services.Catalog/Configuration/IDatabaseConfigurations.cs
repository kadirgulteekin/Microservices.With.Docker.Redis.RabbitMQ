namespace Services.Catalog.Configuration
{
    public interface IDatabaseConfigurations
    {
        public string? CourceCollectionName { get; set; }
        public string? CategoryCollectionName { get; set; }
        public string? ConnectionString { get; set; }
        public string? DatabaseName { get; set; }

    }
}
