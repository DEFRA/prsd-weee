namespace EA.Weee.RequestHandlers.Aatf
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.RequestHandlers.Factories;

    public class AatfDataAccess : IAatfDataAccess
    {
        private readonly WeeeContext context;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IQuarterWindowFactory quarterWindowFactory;

        public AatfDataAccess(WeeeContext context,
            IGenericDataAccess genericDataAccess,
            IQuarterWindowFactory quarterWindowFactory)
        {
            this.context = context;
            this.genericDataAccess = genericDataAccess;
            this.quarterWindowFactory = quarterWindowFactory;
        }

        public async Task<List<Aatf>> GetAatfsForOrganisation(Guid organisationId)
        {
            return await context.Aatfs.Where(a => a.Organisation.Id == organisationId).ToListAsync();
        }

        public async Task<Aatf> GetDetails(Guid id)
        {
            return await GetAatfById(id);
        }

        public Task UpdateDetails(Aatf oldDetails, Aatf newDetails)
        {
            oldDetails.UpdateDetails(
                newDetails.Name,
                newDetails.CompetentAuthority,
                newDetails.ApprovalNumber,
                newDetails.AatfStatus,
                newDetails.Organisation,
                newDetails.Size,
                newDetails.ApprovalDate,
                newDetails.LocalArea,
                newDetails.PanArea);

            return context.SaveChangesAsync();
        }

        public Task UpdateAddress(AatfAddress oldDetails, AatfAddress newDetails, Country country)
        {
            oldDetails.UpdateAddress(
                newDetails.Name,
                newDetails.Address1,
                newDetails.Address2,
                newDetails.TownOrCity,
                newDetails.CountyOrRegion,
                newDetails.Postcode,
                country);

            return context.SaveChangesAsync();
        }

        public async Task<AatfContact> GetContact(Guid aatfId)
        {
            return await context.AatfContacts.SingleOrDefaultAsync(a => a.Id == (context.Aatfs.FirstOrDefault(c => c.Id == aatfId)).Contact.Id);
        }

        public Task UpdateContact(AatfContact oldDetails, AatfContactData newDetails, Country country)
        {
            oldDetails.UpdateDetails(
                newDetails.FirstName,
                newDetails.LastName,
                newDetails.Position,
                newDetails.AddressData.Address1,
                newDetails.AddressData.Address2,
                newDetails.AddressData.TownOrCity,
                newDetails.AddressData.CountyOrRegion,
                newDetails.AddressData.Postcode,
                country,
                newDetails.Telephone,
                newDetails.Email);

            return context.SaveChangesAsync();
        }

        public async Task<bool> HasAatfData(Guid aatfId)
        {
            return await context.WeeeSentOn.AnyAsync(p => p.AatfId == aatfId)
                   || await context.WeeeReused.AnyAsync(p => p.AatfId == aatfId)
                   || await context.WeeeReceived.AnyAsync(p => p.AatfId == aatfId)
                    || await context.ReturnAatfs.AnyAsync(r => r.Aatf.Id == aatfId && r.Return.NilReturn);
        }

        public async Task<bool> HasAatfOrganisationOtherAeOrAatf(Aatf aatf)
        {
            var findAatf = await GetAatfById(aatf.Id);

            var organisationId = aatf.Organisation.Id;

            return await context.Aatfs.CountAsync(p => p.Organisation.Id == organisationId
                                                       && p.ComplianceYear == findAatf.ComplianceYear
                                                       && p.FacilityType.Value == findAatf.FacilityType.Value
                                                       && p.Id != findAatf.Id) > 0;
        }

        private async Task<Aatf> GetAatfById(Guid id)
        {
            var aatf = await context.Aatfs.FirstOrDefaultAsync(p => p.Id == id);

            if (aatf == null)
            {
                throw new ArgumentException($"Aatf with id {id} not found");
            }

            return aatf;
        }

        public async Task RemoveAatf(Guid aatfId)
        {
            var aatf = await GetAatfById(aatfId);

            genericDataAccess.Remove(aatf);

            foreach (var returnAatf in context.ReturnAatfs.Where(r => r.Aatf.Id == aatfId))
            {
                genericDataAccess.Remove(returnAatf);
            }

            await context.SaveChangesAsync();
        }

        public async Task RemoveAatfData(Aatf aatf, IEnumerable<int> quarters)
        {
            foreach (var quarter in quarters)
            {
                var quarterWindow = await quarterWindowFactory.GetAnnualQuarter(new Quarter(aatf.ComplianceYear, (QuarterType)quarter));

                var aatfCount = await context.Aatfs.CountAsync(a => a.Organisation.Id ==
                                                                    aatf.Organisation.Id
                                                                    && a.ComplianceYear == aatf.ComplianceYear
                                                                    && a.FacilityType.Value == aatf.FacilityType.Value
                                                                    && a.ApprovalDate.Value <= quarterWindow.EndDate);

                IEnumerable<WeeeSentOn> weeeSentOn;
                IEnumerable<WeeeReused> weeeReused;
                IEnumerable<WeeeReceived> weeeReceived;
                IEnumerable<ReturnAatf> returnAatfs;
                var weeeReceivedAmounts = new List<WeeeReceivedAmount>().AsEnumerable();
                var weeeReusedAmounts = new List<WeeeReusedAmount>().AsEnumerable();
                var weeeReusedSites = new List<WeeeReusedSite>().AsEnumerable();
                var weeeSentOnAmounts = new List<WeeeSentOnAmount>().AsEnumerable();

                if (aatfCount > 0)
                {
                    if (aatfCount == 1)
                    {
                        var returns = context.Returns.Where(r =>
                            r.Organisation.Id == aatf.Organisation.Id && (int)r.Quarter.Q == quarter && r.Quarter.Year == aatf.ComplianceYear && r.FacilityType.Value == aatf.FacilityType.Value);
                        var returnIds = returns.Select(r => r.Id).ToList();
                        returnAatfs = context.ReturnAatfs.Where(r => returnIds.Contains(r.Return.Id));
                        var returnReportsOn = context.ReturnReportOns.Where(r => returnIds.Contains(r.Return.Id));
                        var returnScheme = context.ReturnScheme.Where(r => returnIds.Contains(r.Return.Id));
                        var nonObligated = context.NonObligatedWeee.Where(r => returnIds.Contains(r.Return.Id));

                        weeeSentOn = context.WeeeSentOn.Where(w => returnIds.Contains(w.Return.Id)).Cast<WeeeSentOn>();
                        weeeReused = context.WeeeReused.Where(w => returnIds.Contains(w.Return.Id)).Cast<WeeeReused>();
                        weeeReceived = context.WeeeReceived.Where(w => returnIds.Contains(w.Return.Id)).Cast<WeeeReceived>();

                        foreach (var @return in returns)
                        {
                            context.Entry(@return).Entity.ParentId = null;
                            context.Entry(@return).State = EntityState.Modified;
                        }

                        await context.SaveChangesAsync();

                        context.ReturnScheme.RemoveRange(returnScheme);
                        context.ReturnReportOns.RemoveRange(returnReportsOn);
                        context.Returns.RemoveRange(returns);
                        context.NonObligatedWeee.RemoveRange(nonObligated);
                    }
                    else
                    {
                        weeeSentOn = context.WeeeSentOn.Where(ObligatedByAatfComplianceYearAndQuarter(aatf, quarter)).Cast<WeeeSentOn>();
                        weeeReused = context.WeeeReused.Where(ObligatedByAatfComplianceYearAndQuarter(aatf, quarter)).Cast<WeeeReused>();
                        weeeReceived = context.WeeeReceived.Where(ObligatedByAatfComplianceYearAndQuarter(aatf, quarter)).Cast<WeeeReceived>();

                        returnAatfs = context.ReturnAatfs.Where(r =>
                            r.Aatf.Id == aatf.Id && r.Return.Organisation.Id == aatf.Organisation.Id && r.Return.Quarter.Year == aatf.ComplianceYear &&
                            (int)r.Return.Quarter.Q == quarter);
                    }

                    weeeReceivedAmounts = context.WeeeReceivedAmount.Where(w => weeeReceived.Select(wr => wr.Id).Contains(w.WeeeReceived.Id));
                    weeeReusedAmounts = context.WeeeReusedAmount.Where(w => weeeReused.Select(wr => wr.Id).Contains(w.WeeeReused.Id));
                    weeeReusedSites = context.WeeeReusedSite.Where(w => weeeReused.Select(wr => wr.Id).Contains(w.WeeeReused.Id));
                    weeeSentOnAmounts = context.WeeeSentOnAmount.Where(w => weeeSentOn.Select(wr => wr.Id).Contains(w.WeeeSentOn.Id));

                    context.WeeeSentOnAmount.RemoveRange(weeeSentOnAmounts);
                    context.WeeeReusedSite.RemoveRange(weeeReusedSites);
                    context.WeeeReusedAmount.RemoveRange(weeeReusedAmounts);
                    context.WeeeReceivedAmount.RemoveRange(weeeReceivedAmounts);
                    context.WeeeReceived.RemoveRange(weeeReceived);
                    context.WeeeReused.RemoveRange(weeeReused);
                    context.WeeeSentOn.RemoveRange(weeeSentOn);
                    context.ReturnAatfs.RemoveRange(returnAatfs);
                }
            }

            await context.SaveChangesAsync();
        }

        private Expression<Func<AatfEntity, bool>> ObligatedByAatfComplianceYearAndQuarter(Aatf aatf, int quarter)
        {
            return w => w.AatfId == aatf.Id && w.Return.Quarter.Year == aatf.ComplianceYear && (int)w.Return.Quarter.Q == quarter;
        }

        public async Task<List<short>> GetComplianceYearsForAatfByAatfId(Guid aatfId)
        {
            return await context.Aatfs
                    .Where(r => r.AatfId == aatfId)
                    .Select(r => r.ComplianceYear)
                    .Distinct()
                    .OrderByDescending(year => year)
                    .ToListAsync();
        }

        public async Task<Aatf> GetAatfByAatfIdAndComplianceYear(Guid aatfId, int complianceYear)
        {
            return await context.Aatfs.FirstOrDefaultAsync(p => p.AatfId == aatfId && p.ComplianceYear == complianceYear);
        }

        public async Task<bool> IsLatestAatf(Guid id, Guid aatfId)
        {
            var latestAatf = await context.Aatfs
                .Where(r => r.AatfId == aatfId)
                .OrderByDescending(r => r.ComplianceYear).FirstOrDefaultAsync();                   

            return latestAatf != null && latestAatf.Id.Equals(id);
        }
    }
}
