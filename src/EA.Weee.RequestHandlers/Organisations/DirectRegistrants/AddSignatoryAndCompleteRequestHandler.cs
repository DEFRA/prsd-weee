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

        public AddSignatoryAndCompleteRequestHandler(IWeeeAuthorization authorization,
            IGenericDataAccess genericDataAccess, WeeeContext weeeContext, ISystemDataDataAccess systemDataAccess) : base(authorization, genericDataAccess, systemDataAccess)
        {
            this.weeeContext = weeeContext;
        }

        public async Task<bool> HandleAsync(AddSignatoryAndCompleteRequest request)
        {
            var currentYearSubmission = await Get(request.DirectRegistrantId);

            var contact = ValueObjectInitializer.CreateContact(request.ContactData);

            currentYearSubmission.DirectRegistrant.Organisation.AddOrUpdateAddress(AddressType.RegisteredOrPPBAddress, currentYearSubmission.CurrentSubmission.BusinessAddress);
            currentYearSubmission.DirectRegistrant.Organisation.UpdateCompanyDetails(currentYearSubmission.CurrentSubmission.CompanyName, currentYearSubmission.CurrentSubmission.TradingName);

            currentYearSubmission.CurrentSubmission.AddOrUpdateAppropriateSignatory(contact);

            currentYearSubmission.DirectRegistrant.AddOrUpdateMainContactPerson(currentYearSubmission.CurrentSubmission.Contact);
            currentYearSubmission.DirectRegistrant.AddOrUpdateAddress(currentYearSubmission.CurrentSubmission.BusinessAddress);
            currentYearSubmission.DirectRegistrant.BrandName.OverwriteWhereNull(currentYearSubmission.CurrentSubmission.BrandName);
            currentYearSubmission.DirectRegistrant.AuthorisedRepresentative.OverwriteWhereNull(currentYearSubmission.CurrentSubmission.AuthorisedRepresentative);

            await weeeContext.SaveChangesAsync();

            return true;
        }
    }
}
