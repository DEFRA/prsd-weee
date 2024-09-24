namespace EA.Weee.Web.Services
{
    public interface IPaymentReferenceGenerator
    {
        string GeneratePaymentReference(int length = 20);
        string GeneratePaymentReferenceWithSeparators(int length = 20);
    }
}