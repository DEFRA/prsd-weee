namespace EA.Weee.Web.Areas.Producer.ViewModels
{
    using EA.Weee.Core.DirectRegistrant;
    using System;
    using System.Collections.Generic;

    public class TaskListViewModel
    {
        public List<ProducerTaskModel> ProducerTaskModels { get; set; }

        public Guid OrganisationId { get; set; }

        public bool CheckAnswersEnabled
        {
            get
            {
                return ProducerTaskModels.TrueForAll(a => a.Complete);
            }
        }

        public decimal DirectRegistrantChargeAmount { get; set; }

        public bool HasPaid { get; set; }

        public SubmissionStatus Status { get; set; }
    }
}