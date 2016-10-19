namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.QuerySets
{
    using System.Threading.Tasks;
    using Domain.Obligation;

    public interface ISchemeEeeDataQuerySet
    {
        Task<bool> HasProducerEeeDataForObligationType(string registrationNumber, ObligationType obligationType);
    }
}
