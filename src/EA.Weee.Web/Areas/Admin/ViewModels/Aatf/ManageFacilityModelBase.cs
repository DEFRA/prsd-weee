namespace EA.Weee.Web.Areas.Admin.ViewModels.Aatf
{
    using EA.Weee.Core.AatfReturn;
    using System;

    public abstract class ManageFacilityModelBase
    {
        public virtual Guid? Selected { get; set; }
        public virtual FacilityType FacilityType { get; set; }
    }
}