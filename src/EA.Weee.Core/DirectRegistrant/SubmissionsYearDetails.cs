﻿namespace EA.Weee.Core.DirectRegistrant
{
    public class SubmissionsYearDetails
    {
       public SmallProducerSubmissionData SmallProducerSubmissionData { get; set; }

       public int? Year { get; set; }

       public bool DisplayRegistrationDetails { get; set; } = false;
    }
}