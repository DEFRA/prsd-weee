namespace EA.Weee.RequestHandlers.Shared
{
    using System.Data;
    using System.Diagnostics;
    using System.Text;
    using EA.Weee.Core.Shared;

    public static class DataTableCsvHelper
    {        
        public static string DataTableToCSV(this DataTable datatable)
        {
            NoFormulaeExcelSanitizer excelSanitizer = new NoFormulaeExcelSanitizer();
            char seperator = ',';
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < datatable.Columns.Count; i++)
            {
                sb.Append(datatable.Columns[i]);
                if (i < datatable.Columns.Count - 1)
                {
                    sb.Append(seperator);
                }
            }
            sb.AppendLine();
            if (datatable.Rows.Count > 0)
            {
                foreach (DataRow dr in datatable.Rows)
                {
                    for (int i = 0; i < datatable.Columns.Count; i++)
                    {                       
                        sb.Append(EncodeAndCheck(dr[i].ToString(), excelSanitizer));

                        if (i < datatable.Columns.Count - 1)
                        {
                            sb.Append(seperator);
                        }
                    }
                    sb.AppendLine();
                }
            }
            return sb.ToString();
        }

        public static string EncodeAndCheck(string value, NoFormulaeExcelSanitizer excelSanitizer)
        {                       
            string result;
            if (value == null)
            {
                result = string.Empty;
            }
            else
            {
                result = value.ToString();
            }

            if (excelSanitizer.IsThreat(result))
            {
                string message = string.Format(
                    "A potentially dangerous string was identified and sanitised when writing CSV data. The value was \"{0}\".",
                    result);
                Trace.TraceWarning(message);
                result = excelSanitizer.Sanitize(result);
            }

            if (result.Contains(","))
            {
                result = string.Concat("\"", value, "\"");
            }

            result = result.Replace("\r\n", " ");
            result = result.Replace("\n\n", " ");
            result = result.Replace("\r", " ");
            result = result.Replace("\n", " ");

            result = result.Trim();

            return result;
        }
    }
}
