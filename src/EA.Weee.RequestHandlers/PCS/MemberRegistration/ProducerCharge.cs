namespace EA.Weee.RequestHandlers.PCS.MemberRegistration
{
    using Domain;

    public class ProducerCharge
    {
        public ChargeBandType ChargeBandType { get; set; }

        public decimal ChargeAmount { get; set; }
    }
}
