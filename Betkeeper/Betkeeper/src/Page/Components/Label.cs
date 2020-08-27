using System;
using System.Collections.Generic;
using System.Text;

namespace Betkeeper.Page.Components
{
    /// <summary>
    /// Component for displaying a label with no field attached to it.
    /// </summary>
    public class Label : Component
    {
        public string Text { get; set; }

        public Label(string text)
            : base(ComponentType.Label)
        {
            Text = text;
        }
    }
}
