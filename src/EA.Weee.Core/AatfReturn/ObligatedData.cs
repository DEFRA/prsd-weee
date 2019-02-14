namespace EA.Weee.Core.AatfReturn
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Domain.AatfReturn;

    public class ObligatedData
    {
        public ObligatedData(int categoryId, decimal? b2b, decimal? b2c)
        {
            this.CategoryId = categoryId;
            this.B2B = b2b;
            this.B2C = b2c;
        }

        public ObligatedData(WeeeReceived weeeReceived, int categoryId, decimal? b2b, decimal? b2c)
        {
            this.WeeeReceived = weeeReceived;
            this.CategoryId = categoryId;
            this.B2B = b2b;
            this.B2C = b2c;
        }

        public ObligatedData(Guid schemeId, Guid aatfId, int categoryId, decimal? b2b, decimal? b2c)
        {
            this.SchemeId = schemeId;
            this.AatfId = aatfId;
            this.CategoryId = categoryId;
            this.B2B = b2b;
            this.B2C = b2c;
        }

        public ObligatedData()
        {
        }

        public virtual WeeeReceived WeeeReceived { get;set;}

        public Guid SchemeId { get; private set; }

        public Guid AatfId { get; private set; }

        public Guid ReturnId { get; private set; }

        public int CategoryId { get; set; }

        public decimal? B2B { get; set; }

        public decimal? B2C { get; set; }
    }
}
