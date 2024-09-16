namespace EA.Weee.Web.Areas.Producer.ViewModels
{
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
    }
}