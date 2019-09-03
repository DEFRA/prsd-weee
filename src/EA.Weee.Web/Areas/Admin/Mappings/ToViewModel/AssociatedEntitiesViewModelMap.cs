namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.AatfReturn;
    using Core.Scheme;
    using Prsd.Core;
    using Prsd.Core.Mapper;
    using ViewModels.Shared;

    public class AssociatedEntitiesViewModelMap : IMap<AssociatedEntitiesViewModelTransfer, AssociatedEntitiesViewModel>
    {
        public AssociatedEntitiesViewModel Map(AssociatedEntitiesViewModelTransfer source)
        {
            var model = new AssociatedEntitiesViewModel();

            if (source.AssociatedAatfs != null)
            {
                var filteredList = RemoveOlderAatfs(source.AssociatedAatfs);
                var aatfs = filteredList.Where(a => a.FacilityType == FacilityType.Aatf);
                var aes = filteredList.Where(a => a.FacilityType == FacilityType.Ae);

                if (source.AatfId.HasValue)
                {
                    model.AssociatedAatfs = aatfs.Where(a => a.AatfId != source.AatfId.Value).ToList();
                    model.AssociatedAes = aes.Where(a => a.AatfId != source.AatfId.Value).ToList();
                }
                else
                {
                    model.AssociatedAatfs = aatfs.ToList();
                    model.AssociatedAes = aes.ToList();
                }
            }

            if (source.AssociatedSchemes != null)
            {
                model.AssociatedSchemes = source.SchemeId.HasValue ? source.AssociatedSchemes.Where(s => s.Id != source.SchemeId.Value).ToList() : source.AssociatedSchemes;
            }

            return model;
        }

        private List<AatfDataList> RemoveOlderAatfs(List<AatfDataList> source)
        {
            var listToBeReturned = new List<AatfDataList>();

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