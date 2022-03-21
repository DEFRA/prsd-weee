namespace EA.Weee.Core.AatfReturn
{
    using System;

    [Serializable]
    public class ReturnAatfAddressData
    {
        public ReturnAatfAddressData()
        {            
        }
        public Guid SearchTermId { get; set; }

        public string SearchTermName { get; set; }
    }
}
