namespace EA.Weee.Web.ViewModels.JoinOrganisation
{
    using System;
    using System.Collections.Generic;
    using Shared;

    public class SelectOrganisationRadioButtons : KeyValueRadioButtons<string, string>
    {
         public SelectOrganisationRadioButtons()
        {
            this.SelectedValue = String.Empty;
        }

         public SelectOrganisationRadioButtons(IEnumerable<KeyValuePair<string, string>> keysAndValues)
            : base(keysAndValues)
        {
        }
    }
}