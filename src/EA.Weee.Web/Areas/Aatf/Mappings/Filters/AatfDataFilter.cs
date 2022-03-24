namespace EA.Weee.Web.Areas.Aatf.Mappings.Filters
{
    using EA.Weee.Core.AatfReturn;
    using System.Collections.Generic;
    using System.Linq;

    public class AatfDataAatfDataFilter : IAatfDataFilter<List<AatfData>, FacilityType>
    {
        public List<AatfData> Filter(List<AatfData> source, FacilityType filter, bool displayStatus)
        {
            var filteredAatfList = RemoveOlderAatfs(source);

            var selectedAatfsOrAes = new List<AatfData>();

            selectedAatfsOrAes.AddRange(filteredAatfList.Where(m => m.FacilityType == filter));

            foreach (var aatf in selectedAatfsOrAes)
            {
                aatf.AatfContactDetailsName = displayStatus ? aatf.Name + " (" + aatf.ApprovalNumber + ") - " + aatf.AatfStatus : aatf.Name + " (" + aatf.ApprovalNumber + ")";
            }

            return selectedAatfsOrAes;
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