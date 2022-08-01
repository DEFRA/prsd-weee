namespace EA.Weee.Web.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ViewModels.Shared;

    public class ComplianceYearHelper
    {
        public static List<int> FetchCurrentComplianceYears(DateTime systemDateTime, bool forLinks = false)
        {
            var currentYear = systemDateTime.Year;

            var list = Enumerable.Range(currentYear, 2).ToList();

            if (forLinks)
            {
                list.Add(systemDateTime.AddYears(-1).Year);
            }
            else
            {
                //Until end of Jan show previous year
                DateTime endOfMonth = new DateTime(currentYear, 1, DateTime.DaysInMonth(currentYear, 1));

                if (systemDateTime <= endOfMonth)
                {
                    list.Add(systemDateTime.AddYears(-1).Year);
                }
            }

            return list.OrderByDescending(x => x).ToList();
        }

        public static List<int> FetchCurrentComplianceYearsForEvidence(DateTime systemDateTime)
        {
            var currentYear = systemDateTime.Year;

            var listYears = new List<int>() { currentYear, currentYear - 1, currentYear - 2 };

            return listYears.OrderByDescending(x => x).ToList();
        }

        public static int GetSelectedComplianceYear(ManageEvidenceNoteViewModel manageEvidenceNoteViewModel, object selectedComplianceYear, DateTime currentDateTime)
        {
            var complianceYear = manageEvidenceNoteViewModel != null && manageEvidenceNoteViewModel.SelectedComplianceYear > 0 ? manageEvidenceNoteViewModel.SelectedComplianceYear : (selectedComplianceYear == null ? currentDateTime.Year : (int)selectedComplianceYear);

            return complianceYear;
        }
    }
}