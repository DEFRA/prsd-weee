namespace EA.Weee.Core.Shared
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CsvReader
    {
        private readonly TextReader _textReader;
        private readonly CsvReader _csvReader;

        public CsvReader(StreamReader streamReader)
        {
            _textReader = streamReader;
            _csvReader = new CsvReader(_textReader, CultureInfo.InvariantCulture);
        }

        public void RegisterClassMap<T>() where T : ClassMap
        {
            _csvReader.Context.RegisterClassMap<T>();
        }

        public bool Read()
        {
            return _csvReader.Read();
        }

        public bool ReadHeader()
        {
            try
            {
                _csvReader.Read();
                return _csvReader.ReadHeader();
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
                _csvReader.ValidateHeader<T>();
            }
            catch (ValidationException vex)
            {
                throw new CsvValidationException(vex.Message);
            }
        }

        public T GetRecord<T>()
        {
            return _csvReader.GetRecord<T>();
        }

        public void Dispose()
        {
            _csvReader.Dispose();
            _textReader.Dispose();
        }
    }
}
