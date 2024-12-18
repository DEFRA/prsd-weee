namespace EA.Weee.Core.Organisations
{
    using System.Collections.Generic;
    using System.Linq;

    public class PartnerViewModel
    {
        public List<AdditionalContactModel> PartnerModels { get; set; } = new List<AdditionalContactModel>();
        public List<NotRequiredPartnerModel> NotRequiredPartnerModels { get; set; } = new List<NotRequiredPartnerModel>();
        public List<AdditionalContactModel> AllPartnerModels
        {
            get
            {
                var allPartners = new List<AdditionalContactModel>();

                allPartners.AddRange(PartnerModels.Select((p, index) =>
                    new AdditionalContactModel
                    {
                        FirstName = p.FirstName,
                        LastName = p.LastName,
                        Order = index
                    }));

                var startOrder = allPartners.Count;
                allPartners.AddRange(NotRequiredPartnerModels.Select((p, index) =>
                    new AdditionalContactModel
                    {
                        FirstName = p.FirstName,
                        LastName = p.LastName,
                        Order = startOrder + index
                    }));

                return allPartners;
            }
        } 
    }
}