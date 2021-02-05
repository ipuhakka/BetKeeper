using System;
using Betkeeper.Models;
using Api.Classes;

namespace Api.Models
{
    public class ClientError
    {
        public string StackTrace { get; set; }

        public int LineNumber { get; set; }

        public int ColumnNumber { get; set; }

        public string Message { get; set; }

        public string Url { get; set; }

        public void LogError()
        {
            var errorLog = new ErrorLog
            {
                Application = "Client",
                Message = $"Column {ColumnNumber}: Line{LineNumber}: {Message}",
                StackTrace = StackTrace,
                Url = Url,
                Time = DateTime.UtcNow
            };

            ErrorLogger.LogClientError(errorLog);
        }
    }
}