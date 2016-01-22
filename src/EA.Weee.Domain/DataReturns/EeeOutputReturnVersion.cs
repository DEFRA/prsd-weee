namespace EA.Weee.Domain.DataReturns
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Prsd.Core;
    using Prsd.Core.Domain;

    public class EeeOutputReturnVersion : Entity
    {
        public virtual ICollection<DataReturnVersion> DataReturnVersions { get; private set; }

        public virtual ICollection<EeeOutputAmount> EeeOutputAmounts { get; private set; }

        public EeeOutputReturnVersion()
        {
            DataReturnVersions = new List<DataReturnVersion>();
            EeeOutputAmounts = new List<EeeOutputAmount>();
        }

        public void AddEeeOutputAmount(EeeOutputAmount eeeOutputAmount)
        {
            Guard.ArgumentNotNull(() => eeeOutputAmount, eeeOutputAmount);

            //if (eeeOutputAmount.RegisteredProducer.Scheme.Id != dataReturnVersion.DataReturn.Scheme.Id)
            //{
            //    string errorMesage = "The specified producer was registered in a different scheme to this data return.";
            //    throw new InvalidOperationException(errorMesage);
            //}

            EeeOutputAmounts.Add(eeeOutputAmount);
        }
    }
}
