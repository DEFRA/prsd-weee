namespace EA.Weee.Core.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Writes a set of data to a string of comma separated variables.
    /// All values will be encoded where required to ensure the output is valid.
    /// </summary>
    /// <typeparam name="T">The type of data items that will be written.</typeparam>
    public class CsvWriter<T>
    {
        private readonly IExcelSanitizer excelSanitizer;
        private List<CsvColumn> columns = new List<CsvColumn>();

        public CsvWriter()
        {
            this.excelSanitizer = null;
        }

        /// <summary>
        /// Create a CSV writer where values are sanitized to ensure they are
        /// Excel-friendly before being written.
        /// </summary>
        /// <param name="excelSanitizer"></param>
        public CsvWriter(IExcelSanitizer excelSanitizer)
        {
            this.excelSanitizer = excelSanitizer;
        }

        /// <summary>
        /// Defines a column that will appear in the output.
        /// </summary>
        /// <param name="title">The title that will appear on the first row of the output.</param>
        /// <param name="func">A Func that defines how the data value will be extracted from each item.</param>
        /// <param name="formatAsText">Forces the value to be output as a string value. This should be used for
        /// text columns which are likely to be misinterpreted by Excel as numberic, e.g. phone numbers.</param>
        public void DefineColumn(string title, Func<T, object> func, bool formatAsText = false)
        {
            columns.Add(new CsvColumn(title, func, formatAsText, excelSanitizer));
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

        public IEnumerable<string> ColumnTitles
        {
            get
            {
                return columns.Select(c => c.Title);
            }
        }

        private class CsvColumn
        {
            public string Title { get; private set; }
            private readonly Func<T, object> func;
            private readonly bool formatAsText;
            private readonly IExcelSanitizer excelSanitizer;

            public CsvColumn(string title, Func<T, object> func, bool formatAsText, IExcelSanitizer excelSanitizer)
            {
                Title = title;
                this.func = func;
                this.formatAsText = formatAsText;
                this.excelSanitizer = excelSanitizer;
            }

            public string GetData(T item)
            {
                string result;
                
                object data = func(item);
                if (data == null)
                {
                    result = string.Empty;
                }
                else
                {
                    result = data.ToString();
                }

                if (excelSanitizer != null && excelSanitizer.IsThreat(result))
                {
                    string message = string.Format(
                        "A potentially dangerous string was identified and santised when writing CSV data. The value was \"{0}\".",
                        result);
                    Trace.TraceWarning(message);
                    result = excelSanitizer.Sanitize(result);
                }

                if (formatAsText)
                {
                    // Assuming the CSV file will be opened in Excel, write the value as a formula. E.g. ="The value".
                    // Any double quotes already present in the string are escaped with double double-quotes.
                    result = string.Format("=\"{0}\"", result.Replace("\"", "\"\""));
                }

                return result;
            }
        }
    }
}
