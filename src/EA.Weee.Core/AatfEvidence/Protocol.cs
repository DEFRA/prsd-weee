namespace EA.Weee.Core.AatfEvidence
{
    using System;
    using System.ComponentModel.DataAnnotations;

    [Serializable]
    public enum Protocol
    {
        [Display(Name = "Actual protocol")]
        Actual = 1,
        [Display(Name = "LDA protocol")]
        LdaProtocol = 2,
        [Display(Name = "SMW protocol")]
        SmwProtocol = 3,
        [Display(Name = "Site specific protocol")]
        SiteSpecificProtocol = 4,
        [Display(Name = "Reuse network PWP")]
        ReuseNetworkPwp = 5,
        [Display(Name = "Light iron protocol")]
        LightIronProtocol = 6
    }
}
