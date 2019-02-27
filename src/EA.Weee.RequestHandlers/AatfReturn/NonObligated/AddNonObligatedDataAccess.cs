﻿namespace EA.Weee.RequestHandlers.AatfReturn.NonObligated
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.AatfReturn;

    public class AddNonObligatedDataAccess : IAddNonObligatedDataAccess
    {
        private readonly WeeeContext context;

        public AddNonObligatedDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public Task Submit(IEnumerable<NonObligatedWeee> nonObligated)
        {
            foreach (var nonObligatedWeee in nonObligated)
            {
                context.NonObligatedWeee.Add(nonObligatedWeee);
            }

            return context.SaveChangesAsync();
        }
    }
}
