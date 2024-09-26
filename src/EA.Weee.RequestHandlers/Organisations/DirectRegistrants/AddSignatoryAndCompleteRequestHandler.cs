﻿namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using DataAccess;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Producer;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using Mappings;
    using Security;
    using System.Data.Entity;
    using System.Threading.Tasks;

    internal class AddSignatoryAndCompleteRequestHandler : SubmissionRequestHandlerBase, IRequestHandler<AddSignatoryAndCompleteRequest, bool>
    {
        private readonly WeeeContext weeeContext;
        private readonly ISystemDataDataAccess systemDataAccess;

        public AddSignatoryAndCompleteRequestHandler(IWeeeAuthorization authorization,
            IGenericDataAccess genericDataAccess, WeeeContext weeeContext, ISystemDataDataAccess systemDataAccess) : base(authorization, genericDataAccess, systemDataAccess)
        {
            this.weeeContext = weeeContext;
            this.systemDataAccess = systemDataAccess;
        }

        public async Task<bool> HandleAsync(AddSignatoryAndCompleteRequest request)
        {
            var currentYearSubmission = await Get(request.DirectRegistrantId);

            var contact = ValueObjectInitializer.CreateContact(request.ContactData);
            currentYearSubmission.CurrentSubmission.AddOrUpdateAppropriateSignatory(contact);

            currentYearSubmission.DirectRegistrant.Organisation.AddOrUpdateAddress(AddressType.RegisteredOrPPBAddress, currentYearSubmission.CurrentSubmission.BusinessAddress);

            currentYearSubmission.DirectRegistrant.Organisation.UpdateDirectRegistrantDetails(currentYearSubmission.CurrentSubmission.CompanyName,
                currentYearSubmission.CurrentSubmission.CompanyRegistrationNumber,
                currentYearSubmission.CurrentSubmission.TradingName);

            currentYearSubmission.DirectRegistrant.AddOrUpdateMainContactPerson(currentYearSubmission.CurrentSubmission.Contact);

            currentYearSubmission.DirectRegistrant.AddOrUpdateAddress(currentYearSubmission.CurrentSubmission.BusinessAddress);

            currentYearSubmission.DirectRegistrant.BrandName.OverwriteWhereNull(currentYearSubmission.CurrentSubmission.BrandName);

            if (currentYearSubmission.CurrentSubmission.AuthorisedRepresentative != null)
            {
                currentYearSubmission.DirectRegistrant.AuthorisedRepresentative.OverwriteWhereNull(currentYearSubmission.CurrentSubmission.AuthorisedRepresentative);
            }

            var systemDateTime = await systemDataAccess.GetSystemDateTime();

            currentYearSubmission.CurrentSubmission.SubmittedDate = systemDateTime.Date;
            currentYearSubmission.CurrentSubmission.DirectProducerSubmissionStatus = DirectProducerSubmissionStatus.Complete;

            await weeeContext.SaveChangesAsync();

            return true;
        }
    }
}