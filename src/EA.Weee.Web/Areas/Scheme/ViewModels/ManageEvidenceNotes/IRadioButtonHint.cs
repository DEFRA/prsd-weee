namespace EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public interface IRadioButtonHint
    {
        Dictionary<string, string> HintItems { get; }
    }
}