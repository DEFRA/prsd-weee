namespace EA.Weee.RequestHandlers.DataReturns.ProcessDataReturnXmlFile
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Domain.DataReturns;
    using ReturnVersionBuilder;
    using Shared;
    using Xml.DataReturns;

    public class DataReturnFromXmlBuilder : IDataReturnFromXmlBuilder
    {
        private readonly IDataReturnVersionBuilder dataReturnVersionBuilder;

        public DataReturnFromXmlBuilder(IDataReturnVersionBuilder dataReturnVersionBuilder)
        {
            this.dataReturnVersionBuilder = dataReturnVersionBuilder;
        }

        public DataReturnVersionBuilderResult Build(SchemeReturn schemeReturn)
        {
            if (schemeReturn.ProducerList != null)
            {
                foreach (var producer in schemeReturn.ProducerList)
                {
                    foreach (var tonnageReturn in producer.Return)
                    {
                        dataReturnVersionBuilder.AddEeeOutputAmount(producer.RegistrationNo,
                            producer.ProducerCompanyName, tonnageReturn.CategoryName.ToDomainWeeeCategory(),
                            tonnageReturn.ObligationType.ToCoreObligationType(), tonnageReturn.TonnesReturnValue);
                    }
                }
            }

            if (schemeReturn.CollectedFromDCF != null)
            {
                foreach (var collectedFromDcf in schemeReturn.CollectedFromDCF)
                {
                    dataReturnVersionBuilder.AddWeeeCollectedAmount(WeeeCollectedAmountSourceType.Dcf,
                        collectedFromDcf.CategoryName.ToDomainWeeeCategory(), collectedFromDcf.ObligationType.ToCoreObligationType(), collectedFromDcf.TonnesReturnValue);
                }
            }

            if (schemeReturn.B2CWEEEFromDistributors != null)
            {
                foreach (var weeeFromDistributor in schemeReturn.B2CWEEEFromDistributors)
                {
                    dataReturnVersionBuilder.AddWeeeCollectedAmount(WeeeCollectedAmountSourceType.Distributor,
                        weeeFromDistributor.CategoryName.ToDomainWeeeCategory(), weeeFromDistributor.ObligationType.ToCoreObligationType(), weeeFromDistributor.TonnesReturnValue);
                }
            }

            return dataReturnVersionBuilder.Build();
        }
    }
}
