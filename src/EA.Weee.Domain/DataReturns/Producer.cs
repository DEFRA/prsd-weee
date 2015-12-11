namespace EA.Weee.Domain.DataReturns
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Domain.Producer;
    using EA.Prsd.Core.Domain;
    using Prsd.Core;

    public class Producer : Entity
    {
        public RegisteredProducer RegisteredProducer { get; private set; }

        public ICollection<ReturnItem> ReturnItems { get; private set; }

        public Producer(RegisteredProducer registeredProducer)
        {
            Guard.ArgumentNotNull(() => registeredProducer, registeredProducer);

            RegisteredProducer = registeredProducer;
            ReturnItems = new List<ReturnItem>();
        }

        public void AddReturnItem(ReturnItem returnItem)
        {
            Guard.ArgumentNotNull(() => returnItem, returnItem);

            if ((returnItem.ObligationType & RegisteredProducer.CurrentSubmission.ObligationType) == ObligationType.None)
            {
                string errorMessage = string.Format(
                    "A return item with obligation type {0} cannot be added to a producer " +
                    "that is registered with obligation type of {1}.",
                    returnItem.ObligationType,
                    RegisteredProducer.CurrentSubmission.ObligationType);
                throw new InvalidOperationException(errorMessage);
            }

            if (ReturnItems
                .Where(r => r.Category == returnItem.Category)
                .Where(r => (r.ObligationType & returnItem.ObligationType) != ObligationType.None)
                .Any())
            {
                string errorMessage = "A return item with this obligation type and category has already been added.";
                throw new InvalidOperationException(errorMessage);
            }

            ReturnItems.Add(returnItem);
        }
    }
}
