namespace EA.Weee.Core.Organisations
{
    using System.Collections.Generic;
    using System.Linq;

    public class PartnerViewModel
    {
        public List<AdditionalContactModel> PartnerModels { get; set; } = new List<AdditionalContactModel>();
        public List<NotRequiredPartnerModel> NotRequiredPartnerModels { get; set; } = new List<NotRequiredPartnerModel>();
        public List<AdditionalContactModel> AllParterModels
        {
            get
            {
                return NotRequiredPartnerModels
                    .Select(x => new AdditionalContactModel { FirstName = x.FirstName, LastName = x.LastName })
                    .Concat(PartnerModels)
                    .ToList();
            }
        } 
    }
}