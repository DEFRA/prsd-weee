namespace EA.Weee.Domain.DataReturns
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Domain;
    using Prsd.Core;

    public class DataReturnContents : Entity
    {
        public DataReturn DataReturn { get; private set; }

        public ICollection<ReturnItem> ReturnItemsCollectedFromDcf { get; private set; }

        public ICollection<DeliveredToAtf> DeliveredToAatf { get; private set; }

        public ICollection<DeliveredToAe> DeliveredToAe { get; private set; }

        public ICollection<ReturnItem> B2cWeeeFromDistributors { get; private set; }

        public ICollection<ReturnItem> B2cWeeeFromFinalHolders { get; private set; }

        public ICollection<Producer> Producers { get; private set; }

        public DataReturnContents(DataReturn dataReturn)
        {
            Guard.ArgumentNotNull(() => dataReturn, dataReturn);

            DataReturn = dataReturn;
            ReturnItemsCollectedFromDcf = new List<ReturnItem>();
            DeliveredToAatf = new List<DeliveredToAtf>();
            DeliveredToAe = new List<DeliveredToAe>();
            B2cWeeeFromDistributors = new List<ReturnItem>();
            B2cWeeeFromFinalHolders = new List<ReturnItem>();
            Producers = new List<Producer>();
        }

        public void AddReturnItemCollectedFromDcf(ReturnItem returnItem)
        {
            Guard.ArgumentNotNull(() => returnItem, returnItem);

            if (ReturnItemsCollectedFromDcf
                .Where(r => r.Category == returnItem.Category)
                .Where(r => (r.ObligationType & returnItem.ObligationType) != ObligationType.None)
                .Any())
            {
                string errorMessage = "A return item with this obligation type and category has already been added.";
                throw new InvalidOperationException(errorMessage);
            }

            ReturnItemsCollectedFromDcf.Add(returnItem);
        }

        public void AddB2cWeeeFromDistributor(ReturnItem returnItem)
        {
            Guard.ArgumentNotNull(() => returnItem, returnItem);

            if (returnItem.ObligationType != ObligationType.B2C)
            {
                string errorMessage = "Only return items with an obligation type of B2C can be added under \"B2C WEEE from distributors\".";
                throw new InvalidOperationException(errorMessage);
            }

            if (B2cWeeeFromDistributors
                .Where(r => r.Category == returnItem.Category)
                .Where(r => (r.ObligationType & returnItem.ObligationType) != ObligationType.None)
                .Any())
            {
                string errorMessage = "A return item with this obligation type and category has already been added.";
                throw new InvalidOperationException(errorMessage);
            }

            B2cWeeeFromDistributors.Add(returnItem);
        }

        public void AddB2cWeeeFromFinalHolder(ReturnItem returnItem)
        {
            Guard.ArgumentNotNull(() => returnItem, returnItem);

            if (returnItem.ObligationType != ObligationType.B2C)
            {
                string errorMessage = "Only return items with an obligation type of B2C can be added under \"B2C WEEE from final holders\".";
                throw new InvalidOperationException(errorMessage);
            }

            if (B2cWeeeFromFinalHolders
                .Where(r => r.Category == returnItem.Category)
                .Where(r => (r.ObligationType & returnItem.ObligationType) != ObligationType.None)
                .Any())
            {
                string errorMessage = "A return item with this obligation type and category has already been added.";
                throw new InvalidOperationException(errorMessage);
            }

            B2cWeeeFromFinalHolders.Add(returnItem);
        }

        public void AddProducer(Producer producer)
        {
            Guard.ArgumentNotNull(() => producer, producer);

            if (producer.RegisteredProducer.Scheme != DataReturn.Scheme)
            {
                string errorMesage = "The specified producer was registered in a different scheme to this data return.";
                throw new InvalidOperationException(errorMesage);
            }

            if (producer.RegisteredProducer.ComplianceYear != DataReturn.Quarter.Year)
            {
                string errorMesage = "The specified producer was registered in a different year to this data return.";
                throw new InvalidOperationException(errorMesage);
            }

            Producers.Add(producer);
        }
    }
}
