namespace EA.Weee.Core.Shared
{
    /// <summary>
    /// Creates instances of CsvWriter. Where a class needs to use
    /// a CsvWriter, dependecy injection should be used to first
    /// obtain a CsvWriterFactory.
    /// </summary>
    public class CsvWriterFactory
    {
        private readonly IExcelSanitizer excelSanitizer;

        public CsvWriterFactory(IExcelSanitizer excelSanitizer)
        {
            this.excelSanitizer = excelSanitizer;
        }

        public CsvWriter<T> Create<T>()
        {
            return new CsvWriter<T>(excelSanitizer);
        }
    }
}
