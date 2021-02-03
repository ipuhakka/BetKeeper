using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Betkeeper.Page.Components
{
    public class StaticTable : Component
    {
        public List<Row> Rows { get; set; }

        public Row Header { get; set; }

        public StaticTable() : base(ComponentType.StaticTable)
        {
            Rows = new List<Row>();
        }

        public StaticTable(Row header) : this()
        {
            Header = header;
        }
    }

    public class Row
    {
        public List<Cell> Cells { get; set; }

        public Row()
        {
            Cells = new List<Cell>();
        }

        /// <summary>
        /// Constructor for creating a row with same styles
        /// </summary>
        /// <param name="values"></param>
        /// <param name="style"></param>
        public Row(List<string> values, CellStyle style = CellStyle.Normal)
        {
            Cells = new List<Cell>();

            values.ForEach(value =>
            {
                Cells.Add(new Cell
                {
                    Value = value,
                    Style = style
                });
            });
        }

        /// <summary>
        /// Constructor for creating a table row with specific coloring scheme based on a value
        /// </summary>
        /// <param name="valueTuples"></param>
        /// <param name="style"></param>
        /// <param name="determineColor"></param>
        public Row(
            List<Tuple<string, object>> valueTuples, 
            CellStyle style = CellStyle.Normal, 
            Func<object, Color> determineColor = null)
        {
            Cells = new List<Cell>();

            Cells.AddRange(
                valueTuples
                    .Select(tuple => new Cell
                    {
                        Value = tuple.Item1,
                        Style = style,
                        Color = determineColor(tuple.Item2)
                    }));
        }
    }

    /// <summary>
    /// Class representing an individual cell in table
    /// </summary>
    public class Cell
    {
        /// <summary>
        /// Text color
        /// </summary>
        public Color Color { get; set; } = Color.Black;

        [JsonConverter(typeof(StringEnumConverter))]
        public CellStyle Style { get; set; }

        public string Value { get; set; }
    }

    public enum CellStyle
    {
        Normal,
        Bold
    }
}
