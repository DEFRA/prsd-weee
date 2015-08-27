namespace EA.Weee.Core.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Writes a set of data to a string of comma separated variables.
    /// All values will be encoded where required to ensure the output is valid.
    /// </summary>
    /// <typeparam name="T">The type of data items that will be written.</typeparam>
    public class CsvWriter<T>
    {
        private List<CsvColumn> columns = new List<CsvColumn>();

        /// <summary>
        /// Defines a column that will appear in the output.
        /// </summary>
        /// <param name="title">The title that will appear on the first row of the output.</param>
        /// <param name="func">A Functhat defines how the data value will be extracted from each item.</param>
        public void DefineColumn(string title, Func<T, object> func)
        {
            columns.Add(new CsvColumn(title, func));
        }

        public string Write(IEnumerable<T> items)
        {
            StringBuilder sb = new StringBuilder();

            string[] titles = columns.Select(c => Encode(c.Title)).ToArray();
            string titleString = string.Join(",", titles);
            sb.AppendLine(titleString);

            foreach (T item in items)
            {
                string[] values = columns.Select(c => Encode(c.GetData(item))).ToArray();
                string valuesString = string.Join(",", values);
                sb.AppendLine(valuesString);
            }

            return sb.ToString();
        }

        public static string Encode(string value)
        {
            if (value.Contains(","))
            {
                value = string.Concat("\"", value, "\"");
            }

            value = value.Replace("\r\n", " ");
            value = value.Replace("\n\n", " ");
            value = value.Replace("\r", " ");
            value = value.Replace("\n", " ");

            value = value.Trim();

            return value;
        }

        private class CsvColumn
        {
            public string Title { get; private set; }
            private readonly Func<T, object> func;

            public CsvColumn(string title, Func<T, object> func)
            {
                Title = title;
                this.func = func;
            }

            public string GetData(T item)
            {
                object data = func(item);
                if (data == null)
                {
                    return string.Empty;
                }
                else
                {
                    return data.ToString();
                }
            }
        }
    }
}
