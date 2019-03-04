namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration
{
    using EA.Weee.Domain.Scheme;
    using EA.Weee.Requests.Scheme.MemberRegistration;
    using System.Collections.Generic;

    public interface ITotalChargeCalculatorDataAccess
    {
        bool CheckForNotSubmitted(ProcessXmlFile message, Scheme scheme, int deserializedcomplianceYear, List<MemberUpload> memberUploadsCheckAgainstNotSubmitted);

        bool CheckSchemeHasAnnualCharge(Scheme scheme, int deserializedcomplianceYear);

        List<MemberUpload> GetMemberUploads(ProcessXmlFile message, bool hasAnnualCharge, bool isSubmitted, int deserializedcomplianceYear);
    }
}
