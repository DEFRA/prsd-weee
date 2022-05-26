namespace EA.Weee.Core.Shared.CsvReading
{
    using System.Globalization;
    using System.IO;
    using CsvHelper;
    using CsvHelper.Configuration;
    using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

    public class WeeeCsvReader : IWeeeCsvReader
    {
        private readonly TextReader textReader;
        private readonly CsvReader csvReader;

        public WeeeCsvReader(TextReader streamReader)
        {
            textReader = streamReader;
            csvReader = new CsvReader(textReader, CultureInfo.InvariantCulture);
        }

        public void RegisterClassMap<T>() where T : ClassMap
        {
            csvReader.Context.RegisterClassMap<T>();
        }

        public bool Read()
        {
            return csvReader.Read();
        }

        public bool ReadHeader()
        {
            try
            {
                csvReader.Read();
                return csvReader.ReadHeader();
            }
            catch (ReaderException re)
            {
                throw new CsvReaderException(re.Message);
            }
        }

        public void ValidateHeader<T>()
        {
            try
            {
                csvReader.ValidateHeader<T>();
            }
            catch (ValidationException vex)
            {
                throw new CsvValidationException(vex.Message);
            }
        }

        public T GetRecord<T>()
        {
            return csvReader.GetRecord<T>();
        }

        public void Dispose()
        {
            csvReader.Dispose();
            textReader.Dispose();
        }
    }
}
