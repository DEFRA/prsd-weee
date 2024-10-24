﻿namespace EA.Weee.Web.Services.SubmissionService
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Organisations.Base;
    using EA.Weee.Web.Areas.Admin.ViewModels.Scheme.Overview;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class SubmissionService : ISubmissionService
    {
        private SmallProducerSubmissionData smallProducerSubmissionData;

        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly IMapper mapper;
        private bool isInternal;

        public SubmissionService(
            BreadcrumbService breadcrumb,
            IWeeeCache cache,
            IMapper mapper)
        {
            this.breadcrumb = breadcrumb;
            this.cache = cache;
            this.mapper = mapper;
        }

        public SubmissionService WithSubmissionData(SmallProducerSubmissionData data, bool isInternal = false)
        {
            smallProducerSubmissionData = data;
            this.isInternal = isInternal;

            return this;
        }

        public async Task<OrganisationDetailsTabsViewModel> Submissions(int? year = null)
        {
            await SetTabsCrumb(year);

            var years = YearsDropdownData(smallProducerSubmissionData);

            int? yearParam = year ?? (years.FirstOrDefault() == 0 ? (int?)null : years.First());

            return await OrganisationDetails(yearParam);
        }

        public async Task<OrganisationDetailsTabsViewModel> OrganisationDetails(int? year = null)
        {
            var tabModel = await GetSubmissionTabModel(OrganisationDetailsDisplayOption.OrganisationDetails, year);

            tabModel.OrganisationViewModel = MapDetailsSubmissionYearModel<OrganisationViewModel>(year);

            return tabModel;
        }

        public async Task<OrganisationDetailsTabsViewModel> ContactDetails(int? year = null)
        {
            var tabModel = await GetSubmissionTabModel(OrganisationDetailsDisplayOption.ContactDetails, year);

            tabModel.ContactDetailsViewModel = MapDetailsSubmissionYearModel<ContactDetailsViewModel>(year);

            return tabModel;
        }

        public async Task<OrganisationDetailsTabsViewModel> ServiceOfNoticeDetails(int? year = null)
        {
            var tabModel = await GetSubmissionTabModel(OrganisationDetailsDisplayOption.ServiceOfNoticeDetails, year);

            tabModel.ServiceOfNoticeViewModel = MapDetailsSubmissionYearModel<ServiceOfNoticeViewModel>(year);

            return tabModel;
        }

        public async Task<OrganisationDetailsTabsViewModel> RepresentedOrganisationDetails(int? year = null)
        {
            var tabModel = await GetSubmissionTabModel(OrganisationDetailsDisplayOption.RepresentedOrganisationDetails, year);

            tabModel.RepresentingCompanyDetailsViewModel = MapDetailsSubmissionYearModel<RepresentingCompanyDetailsViewModel>(year);

            return tabModel;
        }

        public async Task<OrganisationDetailsTabsViewModel> TotalEEEDetails(int? year = null)
        {
            var tabModel = await GetSubmissionTabModel(OrganisationDetailsDisplayOption.TotalEEEDetails, year);

            tabModel.EditEeeDataViewModel = MapDetailsSubmissionYearModel<EditEeeDataViewModel>(year);

            return tabModel;
        }

        private async Task<OrganisationDetailsTabsViewModel> GetSubmissionTabModel(OrganisationDetailsDisplayOption option, int? year = null)
        {
            await SetTabsCrumb(year);

            return new OrganisationDetailsTabsViewModel
            {
                Years = YearsDropdownData(smallProducerSubmissionData),
                Year = year,
                ActiveOption = option,
                SmallProducerSubmissionData = this.smallProducerSubmissionData,
                IsInternal = this.isInternal
            };
        }

        private async Task SetBreadcrumb(Guid organisationId, string activity)
        {
            var org = await cache.FetchOrganisationName(organisationId);

            if (isInternal)
            {
                breadcrumb.InternalActivity = activity;
            }
            else
            {
                breadcrumb.ExternalOrganisation = org;
                breadcrumb.ExternalActivity = activity;
                breadcrumb.OrganisationId = organisationId;
            }
        }

        private Task SetViewBreadcrumb() => SetBreadcrumb(smallProducerSubmissionData.OrganisationData.Id, ProducerSubmissionConstant.ViewOrganisation);
        private Task SetHistoricBreadcrumb() => SetBreadcrumb(smallProducerSubmissionData.OrganisationData.Id, ProducerSubmissionConstant.HistoricProducerRegistrationSubmission);

        private Task SetTabsCrumb(int? year = null) => year.HasValue ? SetHistoricBreadcrumb() : SetViewBreadcrumb();

        private IEnumerable<int> YearsDropdownData(SmallProducerSubmissionData data)
        {
            if (!isInternal)
            {
                return data.SubmissionHistory
                           .Where(x => x.Value.Status == SubmissionStatus.Submitted)
                           .OrderByDescending(x => x.Key)
                           .Select(x => x.Key);
            }

            return data.SubmissionHistory
                    .OrderByDescending(x => x.Key)
                    .Select(x => x.Key);
        }

        private T MapDetailsSubmissionYearModel<T>(int? year)
        {
            return mapper.Map<SubmissionsYearDetails, T>(
               new SubmissionsYearDetails
               {
                   Year = year,
                   SmallProducerSubmissionData = this.smallProducerSubmissionData
               });
        }
    }
}