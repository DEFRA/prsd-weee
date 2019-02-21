namespace EA.Weee.Core.AatfReturn
{
    using System;

    public class WeeeObligatedData
    {
        public WeeeObligatedData(Scheme scheme, Aatf aatf, int categoryId, decimal? b2b, decimal? b2c)
        {
            this.Scheme = scheme;
            this.Aatf = aatf;
            this.CategoryId = categoryId;
            this.B2B = b2b;
            this.B2C = b2c;
        }

        public WeeeObligatedData()
        {
        }
        
        public virtual Scheme Scheme { get; set; }

        public virtual Aatf Aatf { get; set; }

        public Guid ReturnId { get; set; }

        public int CategoryId { get; set; }

        public decimal? B2B { get; set; }

        public decimal? B2C { get; set; }
    }
}
