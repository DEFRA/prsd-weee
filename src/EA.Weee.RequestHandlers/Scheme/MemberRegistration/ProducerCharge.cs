namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using Domain.Lookup;
    using Xml.MemberRegistration;

    public class ProducerCharge
    {
        public ChargeBandAmount ChargeBandAmount { get; set; }

        public decimal Amount { get; set; }
    }
}
