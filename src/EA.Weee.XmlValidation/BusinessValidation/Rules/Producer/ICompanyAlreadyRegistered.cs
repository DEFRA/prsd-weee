﻿namespace EA.Weee.XmlValidation.BusinessValidation.Rules.Producer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Weee.Xml.Schemas;

    public interface ICompanyAlreadyRegistered
    {
        RuleResult Evaluate(producerType element);
    }
}
