namespace EA.Weee.Core.AatfEvidence
{
    using System;
    using System.ComponentModel.DataAnnotations;

    [Serializable]
    public enum Protocol
    {
        [Display(Name = "Actual")]
        Actual = 1,
        [Display(Name = "National protocol")]
        NationalProtocol = 2,
        [Display(Name = "Site protocol")]
        SiteProtocol = 3,
        [Display(Name = "Re-use network weights")]
        ReuseNetworkWeights = 4,
    }
}
