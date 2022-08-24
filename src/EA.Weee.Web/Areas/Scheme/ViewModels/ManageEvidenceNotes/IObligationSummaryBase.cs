namespace EA.Weee.Web.Areas.Scheme.ViewModels.ManageEvidenceNotes
{
    using System.Collections.Generic;
    using EA.Weee.Core.Admin.Obligation;

    public interface IObligationSummaryBase
    {
         IList<ObligationEvidenceValue> ObligationEvidenceValues { get; set; }

         bool DisplayNoDataMessage { get; set; }

          string ObligationTotal { get; set; }

          string Obligation210Total { get; set; }

          string EvidenceTotal { get; set; }

          string Evidence210Total { get; set; }

          string ReuseTotal { get; set; }

          string Reuse210Total { get; set; }

          string TransferredOutTotal { get; set; }

          string TransferredOut210Total { get; set; }

          string TransferredInTotal { get; set; }

          string TransferredIn210Total { get; set; }

          string DifferenceTotal { get; set; }

          string Difference210Total { get; set; }
    }
}
