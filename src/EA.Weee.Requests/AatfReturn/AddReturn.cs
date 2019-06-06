﻿namespace EA.Weee.Requests.AatfReturn
{
    using System;
    using Core.DataReturns;
    using Prsd.Core.Mediator;

    public class AddReturn : IRequest<Guid>
    {
        public Guid OrganisationId { get; set; }

        public int Year { get; set; }

        public QuarterType Quarter { get; set; }
    }
}
