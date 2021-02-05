using Betkeeper;
using Betkeeper.Models;
using Microsoft.AspNetCore.Diagnostics;
using System;

namespace Api.Classes
{
    public class ErrorLogger
    {
        /// <summary>
        /// Log a backend exception
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requestUri"></param>
        public void LogError(IExceptionHandlerFeature context, string requestUri)
        {
            if (!Settings.LogErrors)
            {
                return;
            }

            var error = new ErrorLog
            {
                Message = context.Error.Message,
                StackTrace = context.Error.StackTrace,
                Url = requestUri,
                Application = "Api",
                Time = DateTime.UtcNow
            };

            new ErrorLogRepository().AddError(error);
        }

        /// <summary>
        /// Log client error
        /// </summary>
        /// <param name="errorLog"></param>
        public static void LogClientError(ErrorLog errorLog)
        {
            if (!Settings.LogErrors)
            {
                return;
            }

            new ErrorLogRepository().AddError(errorLog);
        }
    }
}