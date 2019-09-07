using System;
using System.Data.SQLite;
using System.IO;
using Betkeeper;

namespace Test
{
    public static class Tools
    {
        const string testDatabaseName = "test.db";
        const string testSchemaPath = "files/database_schema_dump.sql";

        public static void CreateTestDatabase()
        {
            SQLiteConnection.CreateFile(testDatabaseName);

            Settings.DatabasePath = testDatabaseName;
            CreateSchemas();
        }

        /// <summary>
        /// Creates schemas from specified file.
        /// </summary>
        private static void CreateSchemas()
        {
            string commandText = File.ReadAllText(testSchemaPath);

            SQLiteConnection con = new SQLiteConnection(Settings.GetConnectionString());
            con.Open();

            SQLiteCommand command = new SQLiteCommand(commandText, con);
            command.ExecuteNonQuery();

            con.Close();
        }

        public static void DeleteTestDatabase()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            File.Delete(testDatabaseName);
        }
    }
}
