namespace EA.Weee.Core.Configuration
{
    using System.Collections.Generic;
  
    public interface ITestUserEmailDomains
    {
        bool UserTestModeEnabled { get; }
        IEnumerable<string> Domains { get; }
    }
}
