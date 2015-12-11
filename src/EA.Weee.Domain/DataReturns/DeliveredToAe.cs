namespace EA.Weee.Domain.DataReturns
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Domain;
    using Prsd.Core;

    public class DeliveredToAe : Entity
    {
        public string ApprovalNumber { get; private set; }

        public string OperatorName { get; private set; }

        public ICollection<ReturnItem> ReturnItems { get; private set; }

        public DeliveredToAe(string approvalNumber, string operatorName)
        {
            ApprovalNumber = approvalNumber;
            OperatorName = operatorName;
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
