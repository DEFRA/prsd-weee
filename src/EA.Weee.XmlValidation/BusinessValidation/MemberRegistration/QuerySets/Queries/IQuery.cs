namespace EA.Weee.XmlValidation.BusinessValidation.MemberRegistration.QuerySets.Queries
{
    public interface IQuery<out T>
    {
        T Run();
    }
}
