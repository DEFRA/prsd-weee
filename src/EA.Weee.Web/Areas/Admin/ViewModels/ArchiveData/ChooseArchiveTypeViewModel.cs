namespace EA.Weee.Web.Areas.Admin.ViewModels.ArchiveData
{
    using EA.Weee.Web.ViewModels.Shared;
    using System.Collections.Generic;

    public class ChooseArchiveTypeViewModel : RadioButtonStringCollectionViewModel
    {
        public ChooseArchiveTypeViewModel() : base(new List<string> { ArchiveDataType.PCSMemberDetails, ArchiveDataType.AATFData })
        {
        }
    }
}