namespace EA.Weee.RequestHandlers.Tests.Unit.DataReturns.ProcessDataReturnXmlFile
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Core.DataReturns;
    using Core.Shared;
    using Domain.DataReturns;
    using Domain.Lookup;
    using FakeItEasy;
    using RequestHandlers.DataReturns.ProcessDataReturnXmlFile;
    using RequestHandlers.DataReturns.ReturnVersionBuilder;
    using Xml.DataReturns;
    using Xunit;

    public class DataReturnFromXmlBuilderTests
    {
        [Fact]
        public void Build_NoProducers_DoesNotProcessProducerList()
        {
            var builder = new DataReturnFromXmlBuilderHelper();

            var schemeReturn = new SchemeReturn()
            {
                ProducerList = null
            };

            builder.Build().Build(schemeReturn);

            A.CallTo(() => builder.DataReturnVersionBuilder.AddEeeOutputAmount(A<string>._, A<string>._, A<WeeeCategory>._, A<ObligationType>._, A<decimal>._))
                 .MustNotHaveHappened();
        }

        [Fact]
        public void Build_ProcessesAllReturnsInProducerList()
        {
            var builder = new DataReturnFromXmlBuilderHelper();

            var schemeReturn = new SchemeReturn()
            {
                ProducerList = new SchemeReturnProducer[]
                {
                    new SchemeReturnProducer
                    {
                        Return = new[] { A.Dummy<TonnageReturnType>(), A.Dummy<TonnageReturnType>() }
                    },
                    new SchemeReturnProducer
                    {
                        Return = new[] { A.Dummy<TonnageReturnType>() }
                    }
                }
            };

            builder.Build().Build(schemeReturn);

            A.CallTo(() => builder.DataReturnVersionBuilder.AddEeeOutputAmount(A<string>._, A<string>._, A<WeeeCategory>._, A<ObligationType>._, A<decimal>._))
                 .MustHaveHappened(Repeated.Exactly.Times(3));
        }

        [Fact]
        public void Build_NoCollectedFromDcf_DoesNotProcessCollectedFromDcf()
        {
            var builder = new DataReturnFromXmlBuilderHelper();

            var schemeReturn = new SchemeReturn()
            {
                CollectedFromDCF = null
            };

            builder.Build().Build(schemeReturn);

            A.CallTo(() => builder.DataReturnVersionBuilder.AddWeeeCollectedAmount(WeeeCollectedAmountSourceType.Dcf, A<WeeeCategory>._, A<ObligationType>._, A<decimal>._))
                 .MustNotHaveHappened();
        }

        [Fact]
        public void Build_ProcessesAllReturnsInCollectedFromDcf()
        {
            var builder = new DataReturnFromXmlBuilderHelper();

            var schemeReturn = new SchemeReturn()
            {
                CollectedFromDCF = new[] { A.Dummy<TonnageReturnType>(), A.Dummy<TonnageReturnType>() }
            };

            builder.Build().Build(schemeReturn);

            A.CallTo(() => builder.DataReturnVersionBuilder.AddWeeeCollectedAmount(WeeeCollectedAmountSourceType.Dcf, A<WeeeCategory>._, A<ObligationType>._, A<decimal>._))
                 .MustHaveHappened(Repeated.Exactly.Times(2));
        }

        [Fact]
        public void Build_NoB2CWEEEFromDistributors_DoesNotProcessB2CWEEEFromDistributors()
        {
            var builder = new DataReturnFromXmlBuilderHelper();

            var schemeReturn = new SchemeReturn()
            {
                B2CWEEEFromDistributors = null
            };

            builder.Build().Build(schemeReturn);

            A.CallTo(() => builder.DataReturnVersionBuilder.AddWeeeCollectedAmount(WeeeCollectedAmountSourceType.Distributor, A<WeeeCategory>._, A<ObligationType>._, A<decimal>._))
                 .MustNotHaveHappened();
        }

        [Fact]
        public void Build_ProcessesAllReturnsInB2CWEEEFromDistributors()
        {
            var builder = new DataReturnFromXmlBuilderHelper();

            var schemeReturn = new SchemeReturn()
            {
                B2CWEEEFromDistributors = new[] { A.Dummy<TonnageReturnType>(), A.Dummy<TonnageReturnType>() }
            };

            builder.Build().Build(schemeReturn);

            A.CallTo(() => builder.DataReturnVersionBuilder.AddWeeeCollectedAmount(WeeeCollectedAmountSourceType.Distributor, A<WeeeCategory>._, A<ObligationType>._, A<decimal>._))
                 .MustHaveHappened(Repeated.Exactly.Times(2));
        }

        [Fact]
        public void Build_ReturnsResult()
        {
            var builder = new DataReturnFromXmlBuilderHelper();

            DataReturnVersionBuilderResult expectedResult = new DataReturnVersionBuilderResult(A.Dummy<DataReturnVersion>(), A.Dummy<List<ErrorData>>());

            A.CallTo(() => builder.DataReturnVersionBuilder.Build())
                 .Returns(expectedResult);

            var actualResult = builder.Build().Build(A.Dummy<SchemeReturn>());

            Assert.Equal(expectedResult, actualResult);
        }

        private class DataReturnFromXmlBuilderHelper
        {
            public IDataReturnVersionBuilder DataReturnVersionBuilder;

            public DataReturnFromXmlBuilderHelper()
            {
                DataReturnVersionBuilder = A.Fake<IDataReturnVersionBuilder>();
            }

            public DataReturnFromXmlBuilder Build()
            {
                return new DataReturnFromXmlBuilder(DataReturnVersionBuilder);
            }
        }
    }
}
