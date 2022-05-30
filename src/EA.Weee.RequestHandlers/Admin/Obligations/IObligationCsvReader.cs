namespace EA.Weee.RequestHandlers.Admin.Obligations
{
    public interface IObligationCsvReader
    {
        void ValidateHeader(byte[] data);
    }
}
