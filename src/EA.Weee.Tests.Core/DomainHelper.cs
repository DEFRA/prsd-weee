﻿namespace EA.Weee.Tests.Core
{
    using Domain;
    using Domain.Admin;
    using Domain.DataReturns;
    using Domain.Security;
    using EA.Weee.DataAccess;
    using System;
    using System.Linq;
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

        public DataReturnVersion GetDataReturnVersion(Guid dataReturnVersionId)
        {
            return context.DataReturnVersions.Single(d => d.Id == dataReturnVersionId);
        }

        public CompetentAuthorityUser GetCompetentAuthorityUser(Guid competentAuthorityUserId)
        {
            return context.CompetentAuthorityUsers.Single(c => c.Id == competentAuthorityUserId);
        }

        public UKCompetentAuthority GetCompetentAuthority(Guid competentAuthorityId)
        {
            return context.UKCompetentAuthorities.Single(c => c.Id == competentAuthorityId);
        }

        public Role GetRole(string roleName)
        {
            return context.Roles.Single(r => r.Name == roleName);
        }
    }
}
