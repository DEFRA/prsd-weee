namespace EA.Weee.Core.Shared
{
    using System.IO;
    using CsvReading;

    public interface IFileHelper
    {
        StreamReader GetStreamReader(byte[] data);
        IWeeeCsvReader GetCsvReader(StreamReader streamReader);
    }
}