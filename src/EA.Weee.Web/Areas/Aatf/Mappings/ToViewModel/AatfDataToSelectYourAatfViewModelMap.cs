namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.AatfReturn;
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using ViewModels;

    public class AatfDataToSelectYourAatfViewModelMap : IMap<AatfDataToSelectYourAatfViewModelMapTransfer, SelectYourAatfViewModel>
    {
        public SelectYourAatfViewModel Map(AatfDataToSelectYourAatfViewModelMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var filteredAatfList = RemoveOlderAatfs(source.AatfList);

            var selectedAatfsOrAes = new List<AatfData>();

            selectedAatfsOrAes.AddRange(filteredAatfList.Where(m => m.FacilityType == source.FacilityType));

            foreach (var aatf in selectedAatfsOrAes)
            {
                aatf.AatfContactDetailsName = aatf.Name + " (" + aatf.ApprovalNumber + ") - " + aatf.AatfStatus;
            }

            var model = new SelectYourAatfViewModel
            {
                FacilityType = source.FacilityType,
                OrganisationId = source.OrganisationId,
                AatfList = selectedAatfsOrAes.OrderBy(o => o.Name).ToList()
            };

            return model;
        }

        private IEnumerable<AatfData> RemoveOlderAatfs(IReadOnlyCollection<AatfData> source)
        {
            var listToBeReturned = new List<AatfData>();

            foreach (var aatf in source)
            {
                if (source.Any(m => m.AatfId == aatf.AatfId && m.Id != aatf.Id))
                {
                    var latestAatf = source.Where(m => m.AatfId == aatf.AatfId).OrderByDescending(m => m.ApprovalDate).First();

                    if (!listToBeReturned.Contains(latestAatf))
                    {
                        listToBeReturned.Add(latestAatf);
                    }
                }
                else
                {
                    listToBeReturned.Add(aatf);
                }
            }
            return listToBeReturned;
        }
    }
}