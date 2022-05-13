namespace EA.Weee.RequestHandlers.Admin.Aatf
{
    using AatfReturn;
    using AatfReturn.Internal;
    using Core.AatfReturn;
    using Core.Admin;
    using DataAccess;
    using Domain.AatfReturn;
    using Domain.Lookup;
    using Factories;
    using Organisations;
    using Prsd.Core.Domain;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Admin.Aatf;
    using Security;
    using Shared;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using Weee.Security;
    using PanArea = Domain.Lookup.PanArea;

    internal class EditAatfDetailsRequestHandler : IRequestHandler<EditAatfDetails, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IAatfDataAccess aatfDataAccess;
        private readonly IMap<AatfAddressData, AatfAddress> addressMapper;
        private readonly IOrganisationDetailsDataAccess organisationDetailsDataAccess;
        private readonly ICommonDataAccess commonDataAccess;
        private readonly IGetAatfApprovalDateChangeStatus getAatfApprovalDateChangeStatus;
        private readonly IQuarterWindowFactory quarterWindowFactory;
        private readonly IWeeeTransactionAdapter context;

        public EditAatfDetailsRequestHandler(
            IWeeeAuthorization authorization,
            IAatfDataAccess aatfDataAccess,
            IGenericDataAccess genericDataAccess,
            IMap<AatfAddressData, AatfAddress> addressMapper,
            IOrganisationDetailsDataAccess organisationDetailsDataAccess,
            ICommonDataAccess commonDataAccess,
            IGetAatfApprovalDateChangeStatus getAatfApprovalDateChangeStatus,
            IQuarterWindowFactory quarterWindowFactory,
            IWeeeTransactionAdapter context)
        {
            this.authorization = authorization;
            this.genericDataAccess = genericDataAccess;
            this.aatfDataAccess = aatfDataAccess;
            this.addressMapper = addressMapper;
            this.organisationDetailsDataAccess = organisationDetailsDataAccess;
            this.commonDataAccess = commonDataAccess;
            this.getAatfApprovalDateChangeStatus = getAatfApprovalDateChangeStatus;
            this.quarterWindowFactory = quarterWindowFactory;
            this.context = context;
        }

        public async Task<bool> HandleAsync(EditAatfDetails message)
        {
            authorization.EnsureCanAccessInternalArea();
            authorization.EnsureUserInRole(Roles.InternalAdmin);

            using (var transaction = context.BeginTransaction())
            {
                try
                {
                    var updatedAddress = addressMapper.Map(message.Data.SiteAddress);

                    var existingAatf = await genericDataAccess.GetById<Aatf>(message.Data.Id);

                    var competentAuthority = await commonDataAccess.FetchCompetentAuthority(message.Data.CompetentAuthority.Abbreviation);

                    LocalArea localArea = null;
                    PanArea panArea = null;

                    if (message.Data.LocalAreaDataId.HasValue)
                    {
                        localArea = await commonDataAccess.FetchLookup<LocalArea>(message.Data.LocalAreaDataId.Value);
                    }

                    if (message.Data.PanAreaDataId.HasValue)
                    {
                        panArea = await commonDataAccess.FetchLookup<PanArea>(message.Data.PanAreaDataId.Value);
                    }

                    var updatedAatf = new Aatf(
                        message.Data.Name,
                        competentAuthority,
                        message.Data.ApprovalNumber,
                        Enumeration.FromValue<Domain.AatfReturn.AatfStatus>(message.Data.AatfStatusValue),
                        existingAatf.Organisation,
                        updatedAddress,
                        Enumeration.FromValue<Domain.AatfReturn.AatfSize>(message.Data.AatfSizeValue),
                        message.Data.ApprovalDate.GetValueOrDefault(),
                        existingAatf.Contact,
                        existingAatf.FacilityType,
                        existingAatf.ComplianceYear,
                        localArea,
                        panArea);

                    var existingAddress = await genericDataAccess.GetById<AatfAddress>(existingAatf.SiteAddress.Id);

                    var country = await organisationDetailsDataAccess.FetchCountryAsync(message.Data.SiteAddress.CountryId);

                    await aatfDataAccess.UpdateAddress(existingAddress, updatedAddress, country);

                    if (message.Data.ApprovalDate.HasValue && existingAatf.ApprovalDate.HasValue)
                    {
                        var flags = await getAatfApprovalDateChangeStatus.Validate(existingAatf, message.Data.ApprovalDate.Value);

                        if (flags.HasFlag(CanApprovalDateBeChangedFlags.DateChanged))
                        {
                            var existingQuarter = await quarterWindowFactory.GetAnnualQuarterForDate(existingAatf.ApprovalDate.Value);

                            var newQuarter = await quarterWindowFactory.GetAnnualQuarterForDate(message.Data.ApprovalDate.Value);

                            var range = Enumerable.Range((int)existingQuarter, (int)newQuarter - 1);

                            await aatfDataAccess.RemoveAatfData(existingAatf, range);
                        }
                    }

                    await aatfDataAccess.UpdateDetails(existingAatf, updatedAatf);

                    context.Commit(transaction);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    if (ex.InnerException != null)
                    {
                        throw ex.InnerException;
                    }
                }
                finally
                {
                    context.Dispose(transaction);
                }
            }

            return true;
        }
    }
}