namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using System.Collections.Generic;

    public class AatfDataToHomeViewModelMap : IMap<AatfDataToHomeViewModelMapTransfer, HomeViewModel>
    {
        public HomeViewModel Model = new HomeViewModel();

        public AatfDataToHomeViewModelMap()
        {
        }

        public HomeViewModel Map(AatfDataToHomeViewModelMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var selectedAatfsOrAes = new List<AatfData>();

            if (source.IsAE)
            {
                foreach (var aatf in source.AatfList)
                {
                    if (aatf.FacilityType == Core.AatfReturn.FacilityType.Ae)
                    {
                        selectedAatfsOrAes.Add(aatf);
                    }
                }
            }
            else
            {
                foreach (var aatf in source.AatfList)
                {
                    if (aatf.FacilityType == Core.AatfReturn.FacilityType.Aatf)
                    {
                        selectedAatfsOrAes.Add(aatf);
                    }
                }
            }

            foreach (var aatf in selectedAatfsOrAes)
            {
                aatf.AatfContactDetailsName = aatf.Name + " (" + aatf.ApprovalNumber + ") - " + aatf.AatfStatus;
            }

            Model.IsAE = source.IsAE;
            Model.OrganisationId = source.OrganisationId;
            Model.AatfList = selectedAatfsOrAes;

            return Model;
        }
    }
}