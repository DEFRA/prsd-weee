namespace EA.Weee.RequestHandlers.Scheme.UpdateSchemeInformation
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Helpers;
    using Core.Scheme;
    using DataAccess;
    using Domain;
    using Domain.Scheme;
    using EA.Weee.RequestHandlers.Security;
    using Mappings;
    using Prsd.Core.Mediator;
    using Requests.Scheme;

    internal class UpdateSchemeInformationHandler : IRequestHandler<UpdateSchemeInformation, UpdateSchemeInformationResult>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IUpdateSchemeInformationDataAccess dataAccess;

        public UpdateSchemeInformationHandler(
            IWeeeAuthorization authorization,
            IUpdateSchemeInformationDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
        }

        public async Task<UpdateSchemeInformationResult> HandleAsync(UpdateSchemeInformation message)
        {
            authorization.EnsureCanAccessInternalArea();

            Scheme scheme = await dataAccess.FetchSchemeAsync(message.SchemeId);

            /*
             * Check the uniqueness of the approval number if the value is being changed.
             */
            if (scheme.ApprovalNumber != message.ApprovalNumber)
            {
                if (await dataAccess.CheckSchemeApprovalNumberInUseAsync(message.ApprovalNumber))
                {
                    return new UpdateSchemeInformationResult()
                    {
                        Result = UpdateSchemeInformationResult.ResultType.ApprovalNumberUniquenessFailure
                    };
                }
            }

            /*
             * Check the uniqueness of the 1B1S customer reference if
             * - the value is being changed,
             * - a new value has been provided, 
             * - the scheme is in (or being moved to) the Environment Agency.
             */
            if (message.IbisCustomerReference != scheme.IbisCustomerReference)
            {
                if (!string.IsNullOrEmpty(message.IbisCustomerReference))
                {
                    UKCompetentAuthority environmentAgency = await dataAccess.FetchEnvironmentAgencyAsync();

                    if (environmentAgency.Id == message.CompetentAuthorityId)
                    {
                        /*
                         * Try and find another non-rejected scheme for the Environment Agency with the same
                         * 1B1S customer reference. In production, this should at most only ever return one result.
                         * 
                         * As the check for uniqueness has not always existed, it is possible that other
                         * environments may contain multiple schemes with the same 1B1S customer reference
                         * so we are using FirstOrDefault rather than SingleOrDefault.
                         */
                        List<Scheme> nonRejectedEnvironmentAgencySchemes = await dataAccess.FetchNonRejectedEnvironmentAgencySchemesAsync();
                        Scheme otherScheme = nonRejectedEnvironmentAgencySchemes
                            .Where(s => s.Id != scheme.Id)
                            .Where(s => s.IbisCustomerReference == message.IbisCustomerReference)
                            .FirstOrDefault();

                        if (otherScheme != null)
                        {
                            return new UpdateSchemeInformationResult()
                            {
                                Result = UpdateSchemeInformationResult.ResultType.IbisCustomerReferenceUniquenessFailure,
                                IbisCustomerReferenceUniquenessFailure = new UpdateSchemeInformationResult.IbisCustomerReferenceUniquenessFailureInfo()
                                {
                                    IbisCustomerReference = message.IbisCustomerReference,
                                    OtherSchemeApprovalNumber = otherScheme.ApprovalNumber,
                                    OtherSchemeName = otherScheme.SchemeName
                                }
                            };
                        }
                    }
                }
            }

            Domain.Obligation.ObligationType obligationType = ValueObjectInitializer.GetObligationType(message.ObligationType);
            scheme.UpdateScheme(
                message.SchemeName,
                message.ApprovalNumber,
                message.IbisCustomerReference,
                obligationType,
                message.CompetentAuthorityId);

            SchemeStatus status = message.Status.ToDomainEnumeration<SchemeStatus>();
            scheme.SetStatus(status);

            await dataAccess.SaveAsync();

            return new UpdateSchemeInformationResult()
            {
                Result = UpdateSchemeInformationResult.ResultType.Success
            };
        }
    }
}
