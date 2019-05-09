namespace EA.Weee.Web.Areas.Admin.Requests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Weee.Requests.AatfReturn.Internal;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using EA.Weee.Web.Requests.Base;

    public interface IEditAatfContactRequestCreator : IRequestCreator<AatfEditContactAddressViewModel, EditAatfContact>
    {
    }
}
