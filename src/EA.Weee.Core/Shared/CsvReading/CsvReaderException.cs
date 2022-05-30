namespace EA.Weee.Core.Shared.CsvReading
{
    using System;

    public class CsvReaderException : Exception
    {
        public CsvReaderException()
        {
        }

        public CsvReaderException(string message) : base(message)
        {
        }
    }
}
