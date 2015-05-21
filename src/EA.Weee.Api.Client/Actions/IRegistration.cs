namespace EA.Weee.Api.Client.Actions
{
    using System.Threading.Tasks;
    using Entities;
    using Prsd.Core.Web.ApiClient;

    public interface IRegistration
    {
        Task<string> RegisterApplicantAsync(ApplicantRegistrationData applicantRegistrationData);
    }
}