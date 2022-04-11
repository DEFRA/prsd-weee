namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.AatfEvidence;
    using Core.AatfReturn;
    using Core.Helpers;
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using ViewModels;
    using Web.ViewModels.Returns.Mappings.ToViewModel;
    using Web.ViewModels.Shared.Utilities;

    public class ViewEvidenceNoteViewModelMap : IMap<ViewEvidenceNoteMapTransfer, ViewEvidenceNoteViewModel>
    {
        private readonly ITonnageUtilities tonnageUtilities;
        private readonly IAddressUtilities addressUtilities;

        public ViewEvidenceNoteViewModelMap(ITonnageUtilities tonnageUtilities, 
            IAddressUtilities addressUtilities)
        {
            this.tonnageUtilities = tonnageUtilities;
            this.addressUtilities = addressUtilities;
        }

        public ViewEvidenceNoteViewModel Map(ViewEvidenceNoteMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var model = new ViewEvidenceNoteViewModel
            {
                Id = source.EvidenceNoteData.Id,
                OrganisationId = source.EvidenceNoteData.OrganisationData.Id,
                AatfId = source.EvidenceNoteData.AatfData.Id,
                Reference = source.EvidenceNoteData.Reference,
                Status = source.EvidenceNoteData.Status,
                Type = source.EvidenceNoteData.Type,
                StartDate = source.EvidenceNoteData.StartDate,
                EndDate = source.EvidenceNoteData.EndDate,
                ProtocolValue = source.EvidenceNoteData.Protocol,
                WasteTypeValue = source.EvidenceNoteData.WasteType,
                OperatorAddress = addressUtilities.FormattedAddress(source.EvidenceNoteData.OrganisationData.OrganisationName,
                    source.EvidenceNoteData.OrganisationData.BusinessAddress.Address1,
                    source.EvidenceNoteData.OrganisationData.BusinessAddress.Address2,
                    source.EvidenceNoteData.OrganisationData.BusinessAddress.TownOrCity,
                    source.EvidenceNoteData.OrganisationData.BusinessAddress.CountyOrRegion,
                    source.EvidenceNoteData.OrganisationData.BusinessAddress.Postcode),
                SiteAddress = addressUtilities.FormattedAddress(source.EvidenceNoteData.AatfData.SiteAddress.Name,
                    source.EvidenceNoteData.AatfData.SiteAddress.Address1,
                    source.EvidenceNoteData.AatfData.SiteAddress.Address2,
                    source.EvidenceNoteData.AatfData.SiteAddress.TownOrCity,
                    source.EvidenceNoteData.AatfData.SiteAddress.CountyOrRegion,
                    source.EvidenceNoteData.AatfData.SiteAddress.Postcode,
                    source.EvidenceNoteData.AatfData.ApprovalNumber),
                RecipientAddress = addressUtilities.FormattedAddress(source.EvidenceNoteData.SchemeData.SchemeName,
                    source.EvidenceNoteData.SchemeData.Address.Address1,
                    source.EvidenceNoteData.SchemeData.Address.Address2,
                    source.EvidenceNoteData.SchemeData.Address.TownOrCity,
                    source.EvidenceNoteData.SchemeData.Address.CountyOrRegion,
                    source.EvidenceNoteData.SchemeData.Address.Postcode)
            };

            foreach (var tonnageData in source.EvidenceNoteData.EvidenceTonnageData)
            {
                var category = model.CategoryValues.FirstOrDefault(c => c.CategoryId == (int)tonnageData.CategoryId);

                if (category != null)
                {
                    category.Received = tonnageUtilities.CheckIfTonnageIsNull(tonnageData.Received);
                    category.Reused = tonnageUtilities.CheckIfTonnageIsNull(tonnageData.Reused);
                    category.Id = tonnageData.Id;
                }
            }

            return model;
        }
    }
}