namespace EA.Weee.Core.AatfReturn
{
    using System;
    using System.ComponentModel.DataAnnotations;
    
    public sealed class WeeeReceivedObligatedData
    {
        public Scheme Scheme { get; set; }

        public AatfData Aatf { get; set; }

        public Guid ReturnId { get; set; }

        public int CategoryId { get; set; }

        public decimal? B2B { get; set; }

        public decimal? B2C { get; set; }

        public WeeeReceivedObligatedData(int categoryId, decimal? b2b, decimal? b2c)
        {
            this.CategoryId = categoryId;
            this.B2B = b2b;
            this.B2C = b2c;
        }

        public WeeeReceivedObligatedData(Scheme scheme, AatfData aatf, int categoryId, decimal? b2b, decimal? b2c)
        {
            this.Scheme = scheme;
            this.Aatf = aatf;
            this.CategoryId = categoryId;
            this.B2B = b2b;
            this.B2C = b2c;
        }

        public WeeeReceivedObligatedData()
        {
        }
    }
}
