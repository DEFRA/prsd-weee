namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using System.Collections.Generic;
    using System.Linq;

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

            var filteredAatfList = RemoveOlderAatfs(source.AatfList);

            if (source.IsAE)
            {
                foreach (var aatf in filteredAatfList)
                {
                    if (aatf.FacilityType == Core.AatfReturn.FacilityType.Ae)
                    {
                        selectedAatfsOrAes.Add(aatf);
                    }
                }
            }
            else
            {
                foreach (var aatf in filteredAatfList)
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
            Model.AatfList = selectedAatfsOrAes.OrderBy(o => o.Name).ToList();

            return Model;
        }

        private List<AatfData> RemoveOlderAatfs(List<AatfData> source)
        {
            var listToBeReturned = new List<AatfData>();

            for (var i = 0; i < source.Count; i++)
            {
                var aatfListToSort = new List<AatfData>();

                for (var j = 0; j < source.Count; j++)
                {
                    if (source[i].AatfId == source[j].AatfId)
                    {
                        aatfListToSort.Add(source[j]);
                    }
                }

                var latestAatf = aatfListToSort.OrderByDescending(x => x.ApprovalDate).FirstOrDefault();

                if (!listToBeReturned.Contains(latestAatf))
                {
                    listToBeReturned.Add(latestAatf);
                }
            }

            return listToBeReturned;
        }
    }
}