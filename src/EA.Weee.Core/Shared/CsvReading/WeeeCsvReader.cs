namespace EA.Weee.Core.Shared.CsvReading
{
    using System;
    using System.IO;
    using System.Linq;
    using CsvHelper;
    using CsvHelper.Configuration;

    public class WeeeCsvReader : IWeeeCsvReader
    {
        private readonly TextReader textReader;
        private readonly CsvReader csvReader;
        private bool disposed;

        public WeeeCsvReader(TextReader streamReader, CsvConfiguration config)
        {
            textReader = streamReader;
            csvReader = new CsvReader(textReader, config);
        }

        public void RegisterClassMap<T>() where T : ClassMap
        {
            csvReader.Context.RegisterClassMap<T>();
        }

        public bool Read()
        {
            try
            {
                return csvReader.Read();
            }
            catch (BadDataException ex)
            {
                throw new CsvReaderException(ex.Message);
            }
        }

        public bool ReadHeader()
        {
            try
            {
                csvReader.Read();
                return csvReader.ReadHeader();
            }
            catch (Exception re) when (re is ReaderException || re is HeaderValidationException)  
            {
                throw new CsvReaderException(re.Message);
            }
        }

        public void ValidateHeader<T>(int expectedColumnCount, string[] expectedOrder)
        {
            try
            {
                csvReader.ValidateHeader<T>();

                if (csvReader.HeaderRecord.Length != expectedColumnCount)
                {
                    throw new ReaderException(csvReader.Context, "Unexpected number of CSV columns");
                }

                if (!csvReader.HeaderRecord.SequenceEqual(expectedOrder))
                {
                    throw new ReaderException(csvReader.Context, "Unexpected CSV column order");
                }
            }
            catch (Exception ve) when (ve is ReaderException || ve is HeaderValidationException)
            {
                throw new CsvValidationException(ve.Message);
            }
        }

        public T GetRecord<T>()
        {
            return csvReader.GetRecord<T>();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                csvReader.Dispose();
                textReader.Dispose();
            }

            disposed = true;
        }

        ~WeeeCsvReader() => Dispose();
    }
}
