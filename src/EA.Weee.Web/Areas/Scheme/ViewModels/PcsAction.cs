namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    public static class PcsAction
    {
        // PCS menu items
        public static string ManagePcsEvidenceNotes = "Manage PCS evidence notes";  // CCCI_00007 in catalogue has not been updated but this has been confirmed
        public static string ManagePcsMembers = "Manage PCS members";
        public static string ManageEeeWeeeData = "Manage PCS EEE/WEEE data";
        public static string ViewSubmissionHistory = "View PCS submissions history";
        public static string ManagePcsContactDetails = "Manage PCS contact details";

        // AATF menu items
        public static string ManageAatfEvidenceNotes = "Manage AATF evidence notes";
        public static string ManageAatfReturns = "Manage AATF returns";
        public static string ManageAeReturns = "Manage AE returns";
        public static string ManageAatfContactDetails = "Manage AATF contact details";
        public static string ManageAeContactDetails = "Manage AE contact details";

        // PBS menu items
        public static string ManagePBSEvidenceNotes = "Manage PBS evidence notes";

        // these belong to both PCS and AATF menus
        public static string ManageOrganisationUsers = "Manage organisation users";
        public static string ViewOrganisationDetails = "View organisation details";
    }
}