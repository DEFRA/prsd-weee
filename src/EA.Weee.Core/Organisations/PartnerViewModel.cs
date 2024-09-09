namespace EA.Weee.Core.Organisations
{
    using System.Collections.Generic;
    using System.Linq;

    public class PartnerViewModel
    {
        public List<PartnerModel> PartnerModels { get; set; } = new List<PartnerModel>();
        public List<NotRequiredPartnerModel> NotRequiredPartnerModels { get; set; } = new List<NotRequiredPartnerModel>();
        public List<PartnerModel> AllParterModels
        {
            get
            {
                return NotRequiredPartnerModels
                    .Select(x => new PartnerModel { FirstName = x.FirstName, LastName = x.LastName })
                    .Concat(PartnerModels)
                    .ToList();
            }
        } 
    }
}