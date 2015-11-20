namespace EA.Weee.Web.Infrastructure
{
    public static class CsvFilenameFormat
    {
        public static string FormatFileName(string filename)
        {
            return filename.Replace("/", string.Empty);
        }
    }
}