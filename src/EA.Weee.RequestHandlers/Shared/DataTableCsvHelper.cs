namespace EA.Weee.RequestHandlers.Shared
{
    using System.Data;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using EA.Weee.Core.Shared;

    public static class DataTableCsvHelper
    {        
        public static string DataTableToCsv(this DataTable datatable)
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

        public static string DataSetSentOnToCsv(this DataTable datatable, DataTable columnNameDataTable)
        {
            NoFormulaeExcelSanitizer excelSanitizer = new NoFormulaeExcelSanitizer();
            char seperator = ',';
            StringBuilder sb = new StringBuilder();  
            
            //Remove Column 0 from table for nil returns
            if (datatable.Columns.Contains("0"))
            {
                datatable.Columns.Remove("0");
            }

            for (int i = 0; i < datatable.Columns.Count; i++)
            {
                //Replace columnnames from number 1 starting from 14th column
                if (columnNameDataTable != null && columnNameDataTable.Rows.Count > 0)
                {
                    DataRow matchingRow = columnNameDataTable.AsEnumerable().FirstOrDefault(
                        x => x.Field<int>("SiteOperatorId").ToString() == datatable.Columns[i].ColumnName);
                    if (matchingRow != null)
                    {
                        sb.Append("\"");
                        sb.Append(matchingRow.Field<string>("SiteOperatorData"));
                        sb.Append("\"");
                    }
                    else
                    {
                        sb.Append(datatable.Columns[i]);
                    }
                }
                else
                {
                    sb.Append(datatable.Columns[i]);
                }
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
                result = value.Equals("0.000") ? string.Empty : value.ToString();
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
