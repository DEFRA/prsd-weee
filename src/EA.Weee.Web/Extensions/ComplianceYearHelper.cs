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

        public static List<int> FetchCurrentComplianceYearsForEvidence(DateTime evidenceDate, DateTime systemDateTime)
        {
            return Enumerable.Range(evidenceDate.Year, (systemDateTime.Year - evidenceDate.Year) + 1).OrderByDescending(x => x).ToList();
        }

        public static int GetSelectedComplianceYear(ManageEvidenceNoteViewModel manageEvidenceNoteViewModel, DateTime currentDateTime)
        {
            var complianceYear =
                manageEvidenceNoteViewModel != null && manageEvidenceNoteViewModel.SelectedComplianceYear > 0
                    ? manageEvidenceNoteViewModel.SelectedComplianceYear
                    : GetDefaultComplianceYear();

            return complianceYear;

            int GetDefaultComplianceYear()
            {
                int currentYear = currentDateTime.Year;
                int currentMonth = currentDateTime.Month;

                return currentMonth == 1 ? currentYear - 1 : currentYear;
            }
        }
    }
}