namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using Domain;

    public class ProducerCharge
    {
        public ChargeBandType ChargeBandType { get; set; }

        public decimal ChargeAmount { get; set; }
    }
}
