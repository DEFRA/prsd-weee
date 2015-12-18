namespace EA.Weee.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Weee.DataAccess;

    public class DomainHelper
    {
        private readonly WeeeContext context;

        public DomainHelper(WeeeContext context)
        {
            this.context = context;
        }

        public Domain.Scheme.Scheme GetScheme(Guid schemeId)
        {
            return context.Schemes.Single(s => s.Id == schemeId);
        }
    }
}
