namespace Betkeeper.Page.Components
{
    /// <summary>
    /// Component for displaying a label with no field attached to it.
    /// </summary>
    public class Label : Component
    {
        public string Text { get; set; }

        public bool IsDate { get; set; }

        public Label(string text, bool isDate = false)
            : base(ComponentType.Label)
        {
            Text = text;
            IsDate = isDate;
        }
    }
}
