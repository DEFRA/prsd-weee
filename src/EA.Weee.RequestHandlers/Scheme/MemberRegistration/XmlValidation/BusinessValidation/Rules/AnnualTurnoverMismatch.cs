namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration.XmlValidation.BusinessValidation.Rules
{
    public class AnnualTurnoverMismatch
    {
        public producerType Producer { get; set; }

        public AnnualTurnoverMismatch(producerType producer)
        {
            Producer = producer;
        }
    }
}
