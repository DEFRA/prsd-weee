namespace EA.Weee.Web.Infrastructure
{
    public static class CsvFilenameFormat
    {
        public static string RemoveSlash(this string filename)
        {
            return filename.Replace("/", string.Empty);
        }
    }
}