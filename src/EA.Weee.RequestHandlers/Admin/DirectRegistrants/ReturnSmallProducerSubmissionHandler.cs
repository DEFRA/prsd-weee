namespace EA.Weee.RequestHandlers.Admin.DirectRegistrants
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Domain.Producer;
    using EA.Weee.Requests.Admin.DirectRegistrants;
    using EA.Weee.Security;
    using Security;
    using System;
    using System.Threading.Tasks;

    internal class ReturnSmallProducerSubmissionHandler : IRequestHandler<ReturnSmallProducerSubmission, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly ISmallProducerDataAccess smallProducerDataAccess;
        private readonly WeeeContext weeeContext;

        public ReturnSmallProducerSubmissionHandler(
            IWeeeAuthorization authorization,
            ISmallProducerDataAccess smallProducerDataAccess,
            WeeeContext weeeContext)
        {
            this.authorization = authorization;
            this.smallProducerDataAccess = smallProducerDataAccess;
            this.weeeContext = weeeContext;
        }

        public async Task<Guid> HandleAsync(ReturnSmallProducerSubmission request)
        {
            ValidateRequest(request);
            EnsureAuthorisation();

            var submission = await GetAndValidateSubmission(request.DirectProducerSubmissionId);
            var newSubmissionHistory = CreateNewSubmissionHistory(submission);

            await SaveSubmissionHistory(newSubmissionHistory, submission);

            return submission.Id;
        }

        private static void ValidateRequest(ReturnSmallProducerSubmission request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
        }

        private void EnsureAuthorisation()
        {
            authorization.EnsureCanAccessInternalArea();
            authorization.EnsureUserInRole(Roles.InternalUser);
        }

        private async Task<DirectProducerSubmission> GetAndValidateSubmission(Guid submissionId)
        {
            var submission = await smallProducerDataAccess.GetCurrentDirectRegistrantSubmissionById(submissionId);

            if (submission == null || submission.DirectProducerSubmissionStatus != DirectProducerSubmissionStatus.Complete)
            {
                throw new InvalidOperationException("Submission status invalid");
            }

            return submission;
        }

        private static DirectProducerSubmissionHistory CreateNewSubmissionHistory(DirectProducerSubmission submission)
        {
            var newSubmissionHistory = new DirectProducerSubmissionHistory(submission);
            var currentSubmission = submission.CurrentSubmission;

            CopyAddresses(currentSubmission, newSubmissionHistory);
            CopyContacts(currentSubmission, newSubmissionHistory);
            CopyAuthorisedRepresentative(currentSubmission, newSubmissionHistory);
            CopyEeeOutputData(currentSubmission, newSubmissionHistory);
            CopyCompanyDetails(currentSubmission, newSubmissionHistory);

            return newSubmissionHistory;
        }

        private static void CopyAddresses(DirectProducerSubmissionHistory source, DirectProducerSubmissionHistory target)
        {
            if (source.ContactAddress != null)
            {
                target.AddOrUpdateContactAddress(CreateAddressFrom(source.ContactAddress));
            }

            if (source.BusinessAddress != null)
            {
                target.AddOrUpdateBusinessAddress(CreateAddressFrom(source.BusinessAddress));
            }

            if (source.ServiceOfNoticeAddress != null)
            {
                target.AddOrUpdateServiceOfNotice(CreateAddressFrom(source.ServiceOfNoticeAddress));
            }
        }

        private static Address CreateAddressFrom(Address address) =>
            new Address(address.Address1,
                address.Address2,
                address.TownOrCity,
                address.CountyOrRegion,
                address.Postcode,
                address.Country,
                address.Telephone,
                address.Email,
                address.WebAddress);

        private static void CopyContacts(DirectProducerSubmissionHistory source, DirectProducerSubmissionHistory target)
        {
            if (source.Contact != null)
            {
                target.AddOrUpdateContact(CreateContactFrom(source.Contact));
            }

            if (source.AppropriateSignatory != null)
            {
                target.AddOrUpdateAppropriateSignatory(CreateContactFrom(source.AppropriateSignatory));
            }
        }

        private static Contact CreateContactFrom(Contact contact) =>
            new Contact(contact.FirstName, contact.LastName, contact.Position);

        private static void CopyAuthorisedRepresentative(DirectProducerSubmissionHistory source, DirectProducerSubmissionHistory target)
        {
            if (source.AuthorisedRepresentative != null)
            {
                var authRep = source.AuthorisedRepresentative;
                var authRepAddress = authRep.OverseasContact.Address;
                var authRepContact = authRep.OverseasContact;

                var producerAddress = new ProducerAddress(
                    authRepAddress.PrimaryName,
                    authRepAddress.SecondaryName,
                    authRepAddress.Street,
                    authRepAddress.Town,
                    authRepAddress.Locality,
                    authRepAddress.AdministrativeArea,
                    authRepAddress.Country,
                    authRepAddress.PostCode);

                var producerContact = new ProducerContact(
                    authRepContact.Title,
                    authRepContact.ForeName,
                    authRepContact.SurName,
                    authRepContact.Telephone,
                    authRepContact.Mobile,
                    authRepContact.Fax,
                    authRepContact.Email,
                    producerAddress);

                target.AddOrUpdateAuthorisedRepresentative(
                    new AuthorisedRepresentative(
                        authRep.OverseasProducerName,
                        authRep.OverseasProducerTradingName,
                        producerContact));
            }
        }

        private static void CopyEeeOutputData(DirectProducerSubmissionHistory source, DirectProducerSubmissionHistory target)
        {
            if (source.EeeOutputReturnVersion != null)
            {
                target.EeeOutputReturnVersion = new EeeOutputReturnVersion();
                foreach (var amount in source.EeeOutputReturnVersion.EeeOutputAmounts)
                {
                    target.EeeOutputReturnVersion.EeeOutputAmounts.Add(
                        new EeeOutputAmount(
                            amount.ObligationType,
                            amount.WeeeCategory,
                            amount.Tonnage,
                            amount.RegisteredProducer));
                }
            }
        }

        private static void CopyCompanyDetails(DirectProducerSubmissionHistory source, DirectProducerSubmissionHistory target)
        {
            if (source.BrandName != null)
            {
                target.AddOrUpdateBrandName(source.BrandName);
            }

            target.CompanyName = source.CompanyName;
            target.TradingName = source.TradingName;
            target.CompanyRegistrationNumber = source.CompanyRegistrationNumber;
            target.SellingTechniqueType = source.SellingTechniqueType;
        }

        private async Task SaveSubmissionHistory(DirectProducerSubmissionHistory newSubmissionHistory, DirectProducerSubmission submission)
        {
            weeeContext.DirectProducerSubmissionHistories.Add(newSubmissionHistory);
            await weeeContext.SaveChangesAsync();

            submission.DirectProducerSubmissionStatus = DirectProducerSubmissionStatus.Returned;
            submission.SetCurrentSubmission(newSubmissionHistory);

            await weeeContext.SaveChangesAsync();
        }
    }
}