using Betkeeper.Enums;
using System;

namespace Betkeeper.Actions
{
    /// <summary>
    /// Action exception
    /// </summary>
    public class ActionException : Exception
    {
        public ActionResultType ActionExceptionType { get; set; }
        public string ErrorMessage { get; set; }

        public ActionException(
            ActionResultType actionExceptionType, 
            string errorMessage)
        {
            ActionExceptionType = actionExceptionType;
            ErrorMessage = errorMessage;
        }
    }
}
