namespace EA.Weee.XmlValidation.BusinessValidation.QuerySets
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using DataAccess;
    using DataAccess.Extensions;
    using Domain;
    using Domain.Producer;
    using Xml.Schemas;

    public class ProducerQuerySet : IProducerQuerySet
    {
        private readonly WeeeContext context;

        private bool dataFetched = false;
        private List<string> existingProducerNames;
        private List<string> existingProducerRegistrationNumbers;
        private Dictionary<string, List<Producer>> currentProducersByRegistrationNumber;

        public ProducerQuerySet(WeeeContext context)
        {
            this.context = context;
        }

        private void FetchData()
        {
            currentProducersByRegistrationNumber = context
                .Producers
                .Include(p => p.MemberUpload)
                .Include(p => p.Scheme)
                .Include(p => p.ProducerBusiness)
                .Include(p => p.ProducerBusiness.CompanyDetails)
                .Include(p => p.ProducerBusiness.Partnership)
                .Where(p => p.IsCurrentForComplianceYear)
                .AsNoTracking()
                .GroupBy(p => p.RegistrationNumber)
                .ToDictionary(g => g.Key, p => p.ToList());

            existingProducerNames = context
                .Producers
                .ProducerNames()
                .ToList();

            existingProducerRegistrationNumbers = context
                .Producers
                .Select(p => p.RegistrationNumber)
                .Distinct()
                .ToList();
        }

        private void EnsureDataFetched()
        {
            if (!dataFetched)
            {
                FetchData();
                dataFetched = true;
            }
        }

        public Producer GetLatestProducerForComplianceYearAndScheme(string registrationNo, string schemeComplianceYear, Guid schemeOrgId)
        {
            EnsureDataFetched();

            int complianceYear = int.Parse(schemeComplianceYear);

            if (!currentProducersByRegistrationNumber.ContainsKey(registrationNo))
            {
                return null;
            }

            return currentProducersByRegistrationNumber[registrationNo]
                .Where(p => p.MemberUpload.ComplianceYear == complianceYear)
                .Where(p => p.Scheme.OrganisationId == schemeOrgId)
                .SingleOrDefault();
        }

        public Producer GetLatestProducerFromPreviousComplianceYears(string registrationNo)
        {
            EnsureDataFetched();

            if (!currentProducersByRegistrationNumber.ContainsKey(registrationNo))
            {
                return null;
            }

            return currentProducersByRegistrationNumber[registrationNo]
                .OrderByDescending(p => p.MemberUpload.ComplianceYear)
                .FirstOrDefault();
        }

        public Producer GetProducerForOtherSchemeAndObligationType(string registrationNo, string schemeComplianceYear, Guid schemeOrgId, int obligationType)
        {
            EnsureDataFetched();

            int complianceYear = int.Parse(schemeComplianceYear);

            if (!currentProducersByRegistrationNumber.ContainsKey(registrationNo))
            {
                return null;
            }

            return currentProducersByRegistrationNumber[registrationNo]
                .Where(p => p.Scheme.OrganisationId != schemeOrgId)
                .Where(p => p.MemberUpload.ComplianceYear == complianceYear)
                .Where(p => (
                    p.ObligationType == obligationType || 
                    p.ObligationType == (int)ObligationType.Both ||
                    obligationType == (int)obligationTypeType.Both))
                .SingleOrDefault();
        }

        public bool ProducerNameAlreadyRegisteredForComplianceYear(string producerName, string schemeComplianceYear)
        {
            EnsureDataFetched();

            if (string.IsNullOrEmpty(producerName) || string.IsNullOrEmpty(schemeComplianceYear))
            {
                return false;
            }

            return existingProducerNames
                .Any(pn => string.Equals(pn.Trim(), producerName.Trim(), StringComparison.InvariantCultureIgnoreCase));
        }

        public List<string> GetAllRegistrationNumbers()
        {
            EnsureDataFetched();

            return existingProducerRegistrationNumbers;
        }
    }
}
