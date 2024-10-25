namespace EA.Weee.RequestHandlers.Admin.DirectRegistrants
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Domain.Producer;
    using EA.Weee.RequestHandlers.Shared;
    using EA.Weee.Requests.Admin.DirectRegistrants;
    using EA.Weee.Security;
    using Security;
    using System;
    using System.Threading.Tasks;

    internal class ReturnSmallProducerSubmissionHandler : IRequestHandler<ReturnSmallProducerSubmission, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IRegisteredProducerDataAccess registeredProducerDataAccess;
        private readonly ISmallProducerSubmissionService smallProducerSubmissionService;
        private readonly ISmallProducerDataAccess smallProducerDataAccess;
        private readonly WeeeContext weeeContext;

        public ReturnSmallProducerSubmissionHandler(
            IWeeeAuthorization authorization,
            IRegisteredProducerDataAccess registeredProducerDataAccess,
            ISmallProducerSubmissionService smallProducerSubmissionService, ISmallProducerDataAccess smallProducerDataAccess, WeeeContext weeeContext)
        {
            this.authorization = authorization;
            this.registeredProducerDataAccess = registeredProducerDataAccess;
            this.smallProducerSubmissionService = smallProducerSubmissionService;
            this.smallProducerDataAccess = smallProducerDataAccess;
            this.weeeContext = weeeContext;
        }

        public async Task<Guid> HandleAsync(ReturnSmallProducerSubmission request)
        {
            authorization.EnsureCanAccessInternalArea();
            authorization.EnsureUserInRole(Roles.InternalAdmin);

            var submission =
                await smallProducerDataAccess.GetCurrentDirectRegistrantSubmissionById(
                    request.DirectProducerSubmissionId);

            if (submission == null || submission.DirectProducerSubmissionStatus != DirectProducerSubmissionStatus.Complete)
            {
                throw new InvalidOperationException("submission status invalid");
            }

            var newSubmissionHistory = new DirectProducerSubmissionHistory(submission);

            var currentSubmissionHistory = submission.CurrentSubmission;
            var contactAddress = currentSubmissionHistory.ContactAddress;
            var businessAddress = currentSubmissionHistory.BusinessAddress;
            var contact = currentSubmissionHistory.Contact;
            var appropriateSig = currentSubmissionHistory.AppropriateSignatory;
            var serviceOfNoticeAddress = currentSubmissionHistory.ServiceOfNoticeAddress;
            var authorisedRep = currentSubmissionHistory.AuthorisedRepresentative;

            if (contactAddress != null)
            {
                newSubmissionHistory.AddOrUpdateContactAddress(new Address(contactAddress.Address1,
                    contactAddress.Address2,
                    contactAddress.TownOrCity,
                    contactAddress.CountyOrRegion,
                    contactAddress.Postcode,
                    contactAddress.Country,
                    contactAddress.Telephone,
                    contactAddress.Email,
                    contactAddress.WebAddress));
            }

            if (businessAddress != null)
            {
                newSubmissionHistory.AddOrUpdateBusinessAddress(new Address(businessAddress.Address1,
                    businessAddress.Address2,
                    businessAddress.TownOrCity,
                    businessAddress.CountyOrRegion,
                    businessAddress.Postcode,
                    businessAddress.Country,
                    businessAddress.Telephone,
                    businessAddress.Email,
                    businessAddress.WebAddress));
            }

            if (contact != null)
            {
                newSubmissionHistory.AddOrUpdateContact(new Contact(contact.FirstName, contact.LastName,
                    contact.Position));
            }

            if (appropriateSig != null)
            {
                newSubmissionHistory.AddOrUpdateAppropriateSignatory(new Contact(appropriateSig.FirstName, appropriateSig.LastName, appropriateSig.Position));
            }

            if (serviceOfNoticeAddress != null)
            {
                newSubmissionHistory.AddOrUpdateServiceOfNotice(new Address(serviceOfNoticeAddress.Address1,
                    serviceOfNoticeAddress.Address2,
                    serviceOfNoticeAddress.TownOrCity,
                    serviceOfNoticeAddress.CountyOrRegion,
                    serviceOfNoticeAddress.Postcode,
                    serviceOfNoticeAddress.Country,
                    serviceOfNoticeAddress.Telephone,
                    serviceOfNoticeAddress.Email,
                    serviceOfNoticeAddress.WebAddress));
            }

            if (authorisedRep != null)
            {
                var authRepAddress = authorisedRep.OverseasContact.Address;
                var authRepContact = authorisedRep.OverseasContact;
                var producerAddress = new ProducerAddress(
                     authRepAddress.PrimaryName,
                    authRepAddress.SecondaryName,
                    authRepAddress.Street,
                    authRepAddress.Town,
                    authRepAddress.Locality,
                    authRepAddress.AdministrativeArea,
                    authRepAddress.Country,
                    authRepAddress.PostCode);

                var producerContact = new ProducerContact(authRepContact.Title,
                    authRepContact.ForeName,
                    authRepContact.SurName,
                    authRepContact.Telephone,
                    authRepContact.Mobile,
                    authRepContact.Fax,
                    authRepContact.Email,
                    producerAddress);

                var newAuthorisedRepresentative = new AuthorisedRepresentative(authorisedRep.OverseasProducerName, authorisedRep.OverseasProducerTradingName, producerContact);

                newSubmissionHistory.AddOrUpdateAuthorisedRepresentative(newAuthorisedRepresentative);
            }

            if (currentSubmissionHistory.EeeOutputReturnVersion != null)
            {
                foreach (var eeeOutputAmount in currentSubmissionHistory.EeeOutputReturnVersion.EeeOutputAmounts)
                {
                    newSubmissionHistory.EeeOutputReturnVersion = new EeeOutputReturnVersion();
                    newSubmissionHistory.EeeOutputReturnVersion.EeeOutputAmounts.Add(new EeeOutputAmount(eeeOutputAmount.ObligationType, eeeOutputAmount.WeeeCategory, eeeOutputAmount.Tonnage, eeeOutputAmount.RegisteredProducer));
                }
            }

            if (currentSubmissionHistory.BrandName != null)
            {
                newSubmissionHistory.AddOrUpdateBrandName(currentSubmissionHistory.BrandName);
            }

            newSubmissionHistory.CompanyName = currentSubmissionHistory.CompanyName;
            newSubmissionHistory.TradingName = currentSubmissionHistory.TradingName;
            newSubmissionHistory.CompanyRegistrationNumber = currentSubmissionHistory.CompanyRegistrationNumber;
            newSubmissionHistory.SellingTechniqueType = currentSubmissionHistory.SellingTechniqueType;

            weeeContext.DirectProducerSubmissionHistories.Add(newSubmissionHistory);

            await weeeContext.SaveChangesAsync();

            submission.DirectProducerSubmissionStatus = DirectProducerSubmissionStatus.Returned;
            submission.SetCurrentSubmission(newSubmissionHistory);

            await weeeContext.SaveChangesAsync();

            return submission.Id;
        }
    }
}
