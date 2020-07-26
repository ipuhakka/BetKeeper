namespace Betkeeper
{
    public static class Settings
    {
        public static string DatabasePath { get; set; }

        public static bool? UseForeignKeys { get; set; }

        /// <summary>
        /// SQL connection string
        /// </summary>
        public static string ConnectionString { get; set; }
    }
}
