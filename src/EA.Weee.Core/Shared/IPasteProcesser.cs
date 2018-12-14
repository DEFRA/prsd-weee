namespace EA.Weee.Core.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IPasteProcesser
    {
        CategoryValues BuildModel(object pasteValues);
    }
}
