using System;
using System.Data.SQLite;
using System.Collections.Generic;
using System.IO;
using Betkeeper;
using NUnit.Framework;

namespace Test
{
    public static class Tools
    {
        const string testDatabaseName = "test.db";
        const string testSchemaPath = "files/database_schema_dump.sql";

        public static void CreateTestDatabase()
        {
            Directory.SetCurrentDirectory(
                TestContext.CurrentContext.TestDirectory);

            var dir = Directory.GetCurrentDirectory();

            SQLiteConnection.CreateFile(testDatabaseName);

            Settings.DatabasePath = testDatabaseName;
            Settings.UseForeignKeys = false;

            CreateSchemas();
        }

        /// <summary>
        /// Creates schemas from specified file.
        /// </summary>
        private static void CreateSchemas()
        {
            string commandText = File.ReadAllText(testSchemaPath);

            ExecuteNonQuery(commandText);
        }

        /// <summary>
        /// Clears all tables.
        /// </summary>
        /// <param name="tableNames"></param>
        public static void ClearTables(List<string> tableNames)
        {
            var commandText = "";

            foreach(var tableName in tableNames)
            {
                commandText += string.Format("DELETE FROM {0};", tableName);
            }

            ExecuteNonQuery(commandText);
        }

        /// <summary>
        /// Executes commands to database.
        /// </summary>
        /// <param name="commandText"></param>
        public static void ExecuteNonQuery(string commandText)
        {
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
