namespace EA.Weee.RequestHandlers.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using EA.Weee.Core.Shared;

    public static class DataTableCsvHelper
    {        
        public static string DataTableToCsv(this DataTable dataTable)
        {
            var excelSanitizer = new NoFormulaeExcelSanitizer();
            const char separator = ',';
            var sb = new StringBuilder();
            for (var i = 0; i < dataTable.Columns.Count; i++)
            {
                sb.Append(dataTable.Columns[i]);
                if (i < dataTable.Columns.Count - 1)
                {
                    sb.Append(separator);
                }
            }
            sb.AppendLine();
            if (dataTable.Rows.Count > 0)
            {
                foreach (DataRow dr in dataTable.Rows)
                {
                    for (var i = 0; i < dataTable.Columns.Count; i++)
                    {                       
                        sb.Append(EncodeAndCheck(dr[i].ToString(), excelSanitizer));

                        if (i < dataTable.Columns.Count - 1)
                        {
                            sb.Append(separator);
                        }
                    }
                    sb.AppendLine();
                }
            }
            return sb.ToString();
        }

        public static string DataTableToCsv(this DataTable dataTable, List<string> columnsToRemove)
        {
            foreach (var columnName in columnsToRemove)
            {
                for (var columnCount = dataTable.Columns.Count - 1; columnCount >= 0; columnCount--) 
                {
                    if (dataTable.Columns[columnCount].ColumnName.Contains(columnName))
                    {
                        dataTable.Columns.RemoveAt(columnCount);
                    }
                }
            }

            var excelSanitizer = new NoFormulaeExcelSanitizer();
            const char separator = ',';
            var sb = new StringBuilder();
            for (var i = 0; i < dataTable.Columns.Count; i++)
            {
                sb.Append(dataTable.Columns[i]);
                if (i < dataTable.Columns.Count - 1)
                {
                    sb.Append(separator);
                }
            }
            sb.AppendLine();
            if (dataTable.Rows.Count > 0)
            {
                foreach (DataRow dr in dataTable.Rows)
                {
                    for (var i = 0; i < dataTable.Columns.Count; i++)
                    {
                        sb.Append(EncodeAndCheck(dr[i].ToString(), excelSanitizer));

                        if (i < dataTable.Columns.Count - 1)
                        {
                            sb.Append(separator);
                        }
                    }
                    sb.AppendLine();
                }
            }
            return sb.ToString();
        }

        public static string DataSetSentOnToCsv(this DataTable datatable, DataTable columnNameDataTable)
        {
            var excelSanitizer = new NoFormulaeExcelSanitizer();
            var seperator = ',';
            var sb = new StringBuilder();  
            
            //Remove Column 0 from table for nil returns
            if (datatable.Columns.Contains("0"))
            {
                datatable.Columns.Remove("0");
            }

            for (var i = 0; i < datatable.Columns.Count; i++)
            {
                //Replace columnnames from number 1 starting from 14th column
                if (columnNameDataTable != null && columnNameDataTable.Rows.Count > 0)
                {
                    var matchingRow = columnNameDataTable.AsEnumerable().FirstOrDefault(
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
                    for (var i = 0; i < datatable.Columns.Count; i++)
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

        public static void SetColumnsOrder(this DataTable datatable, params String[] columnNames)
        {
            var columnIndex = 0;
            foreach (var columnName in columnNames)
            {
                datatable.Columns[columnName].SetOrdinal(columnIndex);
                columnIndex++;
            }
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
                var message = string.Format(
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
