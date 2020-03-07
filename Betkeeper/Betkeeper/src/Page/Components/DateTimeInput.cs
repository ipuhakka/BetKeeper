using System;
using System.Collections.Generic;
using System.Text;

namespace Betkeeper.Page.Components
{
    public class DateTimeInput : Field
    {
        public DateTime? MinimumDateTime { get; }

        public DateTimeInput(string key, string label, DateTime? minimumDateTime = null)
            : base(key, label, FieldType.DateTime)
        {
            MinimumDateTime = minimumDateTime;
        }
    }
}
