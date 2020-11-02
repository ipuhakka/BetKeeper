using Betkeeper.Models;
using System;
using System.Configuration;
using System.Web.Http.ExceptionHandling;

namespace Betkeeper.Services
{
    public class ErrorLogger : ExceptionLogger
    {
        /// <summary>
        /// Log a backend exception
        /// </summary>
        /// <param name="context"></param>
        public override void Log(ExceptionLoggerContext context)
        {
            base.Log(context);

            if (!bool.Parse(ConfigurationManager.AppSettings.Get("logErrors")))
            {
                return;
            }

            var error = new ErrorLog
            {
                Message = context.Exception.Message,
                StackTrace = context.Exception.StackTrace,
                Url = context.Request.RequestUri.ToString(),
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
            if (!bool.Parse(ConfigurationManager.AppSettings.Get("logErrors")))
            {
                return;
            }

            new ErrorLogRepository().AddError(errorLog);
        }
    }
}
