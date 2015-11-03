namespace EA.Weee.DataAccess.StoredProcedure
{
    using System;

    /// <summary>
    ///     This class maps to the results of [Producer].[spgCSVDataBySchemeComplianceYearAndAuthorisedAuthority].
    /// </summary>
    public class MembersDetailsCsvData
    {
        public string SchemeName { get; set; }

        public string ApprovalNumber { get; set; }

        public string OrganisationName { get; set; }

        public string TradingName { get; set; }

        public string PRN { get; set; }

        public DateTime DateRegistered { get; set; }

        public DateTime DateAmended { get; set; }

        public string SICCodes { get; set; }

        public bool VATRegisted { get; set; }

        public decimal AnnualTurnover { get; set; }

        public string AnnualTurnoverBandType { get; set; }

        public string EEPlacedOnMarketBandType { get; set; }

        public string ObligationType { get; set; }

        public string SellingTechniqueType { get; set; }

        public string ChargeBandType { get; set; }

        public decimal ChargeThisUpdate { get; set; }

        public DateTime CeaseToExist { get; set; }

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

        public string CNAdministravtiveArea { get; set; }

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

        public string CompanycontactSecondaryName { get; set; }

        public string CompanyContactStreet { get; set; }

        public string CompanyContactTown { get; set; }

        public string CompanyContactLocality { get; set; }

        public string CompanyContactAdministravtiveArea { get; set; }

        public string CompanyContactPostcode { get; set; }

        public string CompanyContactCountry { get; set; }

        //Principal place of business details
        public string Partners { get; set; }

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

        public string PPOBContactAdministravtiveArea { get; set; }

        public string PPOBContactPostcode { get; set; }

        public string PPOBContactCountry { get; set; }

        //Overseas contact details
        public string OCContactTitle { get; set; }

        public string OCContactForename { get; set; }

        public string OCContactSurname { get; set; }

        public string OCContactTelephone { get; set; }

        public string OCContactMobile { get; set; }

        public string OCContactFax { get; set; }

        public string OCContactEmail { get; set; }

        public string OCContactPrimaryName { get; set; }

        public string OCContactSecondaryName { get; set; }

        public string OCContactStreet { get; set; }

        public string OCContactTown { get; set; }

        public string OCContactLocality { get; set; }

        public string OCContactAdministravtiveArea { get; set; }

        public string OCContactPostcode { get; set; }

        public string OCContactCountry { get; set; }
    }
}
