namespace EA.Weee.Core.AatfReturn
{
    using System;
    using System.ComponentModel.DataAnnotations;
    
    public sealed class WeeeObligatedData
    {
        public Guid Id { get; set; }

        public Scheme Scheme { get; set; }

        public AatfData Aatf { get; set; }

        public Guid ReturnId { get; set; }

        public int CategoryId { get; set; }

        public decimal? B2B { get; set; }

        public decimal? B2C { get; set; }

        public WeeeObligatedData(Guid id, Scheme scheme, AatfData aatf, int categoryId, decimal? b2b, decimal? b2c)
        {
            this.Id = id;
            this.Scheme = scheme;
            this.Aatf = aatf;
            this.CategoryId = categoryId;
            this.B2B = b2b;
            this.B2C = b2c;
        }

        public WeeeObligatedData()
        {
        }
    }
}
