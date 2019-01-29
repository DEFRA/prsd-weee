namespace EA.Weee.Core.Shared
{
    using Core.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    
    public interface IPasteProcesser
    {
        NonObligatedCategoryValues BuildModel(object pasteValues);
    }
}
