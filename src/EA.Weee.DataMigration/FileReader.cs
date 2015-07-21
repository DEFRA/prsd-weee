namespace EA.Weee.DataMigration
{
    using System;
    using LinqToExcel;

    public class FileReader
    {
        private const string excelDirectory = ".\\DataFiles\\Excel\\";

        public T ReadExcelFile<T>(string fileName)
        {
            var excel = new ExcelQueryFactory(excelDirectory + fileName);
        }
    }
}
