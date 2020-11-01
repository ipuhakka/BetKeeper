using Betkeeper.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Web.Http.ExceptionHandling;

namespace Betkeeper.Classes
{
    public class ErrorLogger : ExceptionLogger
    {
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

        public static void LogClientError()
        {
            // TODO: Joku luokkamalli parameterinä
        }
    }
}
