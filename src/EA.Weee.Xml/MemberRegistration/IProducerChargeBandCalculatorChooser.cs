namespace EA.Weee.Xml.MemberRegistration
{
    public interface IProducerChargeBandCalculatorChooser
    {
        IProducerChargeBandCalculator GetCalculator(schemeType scheme, producerType producer, int complianceYear);
    }
}