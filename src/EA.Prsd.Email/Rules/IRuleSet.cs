using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EA.Prsd.Email.Rules
{
    public interface IRuleSet
    {
        RuleAction DefaultAction { get; }
        IEnumerable<IRule> Rules { get; }

        RuleAction Check(string emailAddress);
    }
}
