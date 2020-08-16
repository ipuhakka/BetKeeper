﻿using System;

namespace Betkeeper.Actions
{
    /// <summary>
    /// Action exception types
    /// </summary>
    public enum ActionExceptionType
    {
        InvalidInput = 400,
        Unauthorized = 401,
        Conflict = 409,
    }

    /// <summary>
    /// Action exception
    /// </summary>
    public class ActionException : Exception
    {
        public ActionExceptionType ActionExceptionType { get; set; }
        public string ErrorMessage { get; set; }

        public ActionException(
            ActionExceptionType actionExceptionType, 
            string errorMessage)
        {
            ActionExceptionType = actionExceptionType;
            ErrorMessage = errorMessage;
        }
    }
}
