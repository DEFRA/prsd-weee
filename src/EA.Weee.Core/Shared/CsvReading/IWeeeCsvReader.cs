namespace EA.Weee.Core.Shared.CsvReading
{
    using System;
    using CsvHelper.Configuration;

    public interface IWeeeCsvReader : IDisposable
    {
        void RegisterClassMap<T>() where T : ClassMap;
        bool Read();
        bool ReadHeader();
        void ValidateHeader<T>(int expectedColumnCount, string[] expectedOrder);
        T GetRecord<T>();
    }
}