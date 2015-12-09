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
