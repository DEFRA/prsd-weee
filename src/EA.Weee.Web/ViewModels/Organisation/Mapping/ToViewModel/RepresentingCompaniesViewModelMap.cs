namespace EA.Weee.Web.ViewModels.Organisation.Mapping.ToViewModel
{
    using System.Collections.Generic;
    using System.Linq;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Shared;

    public class RepresentingCompaniesViewModelMap : IMap<RepresentingCompaniesViewModelMapSource, RepresentingCompaniesViewModel>
    {
        public RepresentingCompaniesViewModel Map(RepresentingCompaniesViewModelMapSource source)
        {
            var accessibleOrganisations = source.OrganisationsData  
                .Where(o => o.UserStatus == UserStatus.Active)
                .ToList();

            var model = new RepresentingCompaniesViewModel()
            {
                OrganisationId = source.OrganisationData.Id,
                Organisations = new List<RepresentingCompany>(),
                ShowBackButton = accessibleOrganisations.Count > 1
            };

            foreach (var directRegistrant in source.OrganisationData.DirectRegistrants.Where(a =>
                         !string.IsNullOrWhiteSpace(a.RepresentedCompanyName)))
            {
                model.Organisations.Add(new RepresentingCompany()
                {
                    DirectRegistrantId = directRegistrant.DirectRegistrantId,
                    Name = directRegistrant.RepresentedCompanyName
                });
            }

            return model;
        }
    }
}