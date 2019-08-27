namespace EA.Weee.Api.Client.Entities
{
    using Core.Routing;
    using System;

    public class ActivatedUserAccountData
    {
        public Guid Id { get; set; }

        public string Code { get; set; }

        public ViewCompetentAuthorityUserRoute ViewUserRoute { get; set; }
    }
}