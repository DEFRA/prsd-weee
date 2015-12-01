namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using Domain.Lookup;

    public class ProducerCharge
    {
        public ChargeBandAmount ChargeBandAmount { get; set; }

        public decimal Amount { get; set; }
    }
}
