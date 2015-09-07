namespace EA.Weee.Core.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    
    public interface ITestInternalUserEmailDomains
    {
        bool Enabled { get; }
        IEnumerable<string> Domains { get; }
    }
}
