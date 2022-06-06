namespace EA.Weee.Core.Shared
{
    using System.Globalization;
    using System.IO;
    using System.Text;
    using CsvReading;

    public class FileHelper : IFileHelper
    {
        public StreamReader GetStreamReader(byte[] data)
        {
            return new StreamReader(new MemoryStream(data), Encoding.UTF8);
        }

        public IWeeeCsvReader GetCsvReader(StreamReader streamReader)
        {
            var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
            {
                DetectColumnCountChanges = true
            };

            return new WeeeCsvReader(streamReader, config);
        }
    }
}
