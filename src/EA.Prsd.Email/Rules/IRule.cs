using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EA.Prsd.Email.Rules
{
    public interface IRule
    {
        RuleAction Action { get; }
        RuleType Type { get; }
        string Value { get; }

        RuleAction? Check(string emailAddress);
    }
}
