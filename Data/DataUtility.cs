namespace AeonRegistryAPI.Data
{
    public static class DataUtility
    {
        public static string GetConnectionstring(IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DbConnection");

            return connectionString!;
        }
    }
}