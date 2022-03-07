namespace EA.Weee.Integration.Tests
{
    using Microsoft.Owin.Security.DataProtection;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TestDataProtectionProvider : IDataProtectionProvider
    {
        public IDataProtector Create(params string[] purposes)
        {
            return new TestDataProtector();
        }
    }
}