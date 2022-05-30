namespace EA.Weee.Core.Shared.CsvReading
{
    using System;

    public class CsvValidationException : Exception
    {
        public CsvValidationException()
        {
        }

        public CsvValidationException(string message) : base(message)
        {
        }
    }
}