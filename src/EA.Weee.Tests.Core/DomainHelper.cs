namespace EA.Weee.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Domain.DataReturns;
    using EA.Weee.DataAccess;
    using Scheme = Domain.Scheme.Scheme;

    public class DomainHelper
    {
        private readonly WeeeContext context;

        public DomainHelper(WeeeContext context)
        {
            this.context = context;
        }

        public Scheme GetScheme(Guid schemeId)
        {
            return context.Schemes.Single(s => s.Id == schemeId);
        }

        public DataReturnUpload GetDataReturnUpload(Guid dataReturnUploadId)
        {
            return context.DataReturnsUploads.Single(d => d.Id == dataReturnUploadId);
        }
    }
}
