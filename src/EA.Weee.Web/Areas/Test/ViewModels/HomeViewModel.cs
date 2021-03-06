﻿namespace EA.Weee.Web.Areas.Test.ViewModels
{
    using EA.Weee.Web.ViewModels.Shared;
    using System.Collections.Generic;

    public class HomeViewModel : RadioButtonStringCollectionViewModel
    {
        public const string CreatePcsMemberXmlFile = "Create PCS Member XML File";
        public const string CreatePcsDataReturnXmlFile = "Create PCS Data Return XML File";
        public const string ManageCache = "Manage Cache";
        public const string ManagePcsReturnsSubmissionWindow = "Manage Returns Submission Window";
        public const string CopyAndPaste = "Copy and paste";

        public HomeViewModel() : base(new List<string>
            {
                CreatePcsMemberXmlFile,
                CreatePcsDataReturnXmlFile,
                ManageCache,
                ManagePcsReturnsSubmissionWindow
            })
        {
        }
    }
}