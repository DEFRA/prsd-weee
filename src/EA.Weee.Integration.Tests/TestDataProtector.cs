namespace EA.Weee.Integration.Tests
{
    public class TestDataProtector : Microsoft.Owin.Security.DataProtection.IDataProtector
    {
        public byte[] Protect(byte[] userData)
        {
            return userData;
        }

        public byte[] Unprotect(byte[] protectedData)
        {
            return protectedData;
        }
    }
}
