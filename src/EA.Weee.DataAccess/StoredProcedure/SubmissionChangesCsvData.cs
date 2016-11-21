namespace EA.Weee.DataAccess.StoredProcedure
{
    using System;

    public class SubmissionChangesCsvData
    {
        public string ProducerRegistrationNumber { get; set; }

        public string ChangeType { get; set; }

        public DateTime SubmittedDate { get; set; }

        public string CompanyName { get; set; }

        public int ComplianceYear { get; set; }

        public string ProducerName { get; set; }

        public string Partners { get; set; }

        public string TradingName { get; set; }

        public string ChargeBandType { get; set; }

        public bool VATRegistered { get; set; }

        public decimal? AnnualTurnover { get; set; }

        public string AnnualTurnoverBandType { get; set; }

        public string EEEPlacedOnMarketBandType { get; set; }

        public string ObligationType { get; set; }

        public string SICCodes { get; set; }

        public string SellingTechniqueType { get; set; }

        public DateTime? CeaseToExist { get; set; }

        // Correspondence of Notices details
        public string CNTitle { get; set; }

        public string CNForename { get; set; }

        public string CNSurname { get; set; }

        public string CNTelephone { get; set; }

        public string CNMobile { get; set; }

        public string CNFax { get; set; }

        public string CNEmail { get; set; }

        public string CNPrimaryName { get; set; }

        public string CNSecondaryName { get; set; }

        public string CNStreet { get; set; }

        public string CNTown { get; set; }

        public string CNLocality { get; set; }

        public string CNAdministrativeArea { get; set; }

        public string CNPostcode { get; set; }

        public string CNCountry { get; set; }

        //Registered Office details
        public string CompanyNumber { get; set; }

        public string CompanyContactTitle { get; set; }

        public string CompanyContactForename { get; set; }

        public string CompanyContactSurname { get; set; }

        public string CompanyContactTelephone { get; set; }

        public string CompanyContactMobile { get; set; }

        public string CompanyContactFax { get; set; }

        public string CompanyContactEmail { get; set; }

        public string CompanyContactPrimaryName { get; set; }

        public string CompanyContactSecondaryName { get; set; }

        public string CompanyContactStreet { get; set; }

        public string CompanyContactTown { get; set; }

        public string CompanyContactLocality { get; set; }

        public string CompanyContactAdministrativeArea { get; set; }

        public string CompanyContactPostcode { get; set; }

        public string CompanyContactCountry { get; set; }

        //Principal place of business details

        public string PPOBContactTitle { get; set; }

        public string PPOBContactForename { get; set; }

        public string PPOBContactSurname { get; set; }

        public string PPOBContactTelephone { get; set; }

        public string PPOBContactMobile { get; set; }

        public string PPOBContactFax { get; set; }

        public string PPOBContactEmail { get; set; }

        public string PPOBContactPrimaryName { get; set; }

        public string PPOBContactSecondaryName { get; set; }

        public string PPOBContactStreet { get; set; }

        public string PPOBContactTown { get; set; }

        public string PPOBContactLocality { get; set; }

        public string PPOBContactAdministrativeArea { get; set; }

        public string PPOBContactPostcode { get; set; }

        public string PPOBContactCountry { get; set; }

        //Overseas contact details
        public string OverseasProducerName { get; set; }

        public string OverseasContactTitle { get; set; }

        public string OverseasContactForename { get; set; }

        public string OverseasContactSurname { get; set; }

        public string OverseasContactTelephone { get; set; }

        public string OverseasContactMobile { get; set; }

        public string OverseasContactFax { get; set; }

        public string OverseasContactEmail { get; set; }

        public string OverseasContactPrimaryName { get; set; }

        public string OverseasContactSecondaryName { get; set; }

        public string OverseasContactStreet { get; set; }

        public string OverseasContactTown { get; set; }

        public string OverseasContactLocality { get; set; }

        public string OverseasContactAdministrativeArea { get; set; }

        public string OverseasContactPostcode { get; set; }

        public string OverseasContactCountry { get; set; }
    }
}
