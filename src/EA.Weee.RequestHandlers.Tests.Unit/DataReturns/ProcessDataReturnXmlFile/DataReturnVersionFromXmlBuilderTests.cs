namespace EA.Weee.RequestHandlers.Tests.Unit.DataReturns.ProcessDataReturnXmlFile
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Shared;
    using Domain.DataReturns;
    using Domain.Lookup;
    using FakeItEasy;
    using RequestHandlers.DataReturns.ProcessDataReturnXmlFile;
    using RequestHandlers.DataReturns.ReturnVersionBuilder;
    using Xml.DataReturns;
    using Xunit;
    using ObligationType = Domain.ObligationType;

    public class DataReturnVersionFromXmlBuilderTests
    {
        [Fact]
        public async Task Build_NoProducers_DoesNotProcessProducerList()
        {
            var builder = new DataReturnVersionFromXmlBuilderHelper();

            var schemeReturn = new SchemeReturn()
            {
                ProducerList = null
            };

            await builder.Create().Build(schemeReturn);

            A.CallTo(() => builder.DataReturnVersionBuilder.AddEeeOutputAmount(A<string>._, A<string>._, A<WeeeCategory>._, A<ObligationType>._, A<decimal>._))
                 .MustNotHaveHappened();
        }

        [Fact]
        public async Task Build_ProcessesAllReturnsInProducerList()
        {
            var builder = new DataReturnVersionFromXmlBuilderHelper();

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

            await builder.Create().Build(schemeReturn);

            A.CallTo(() => builder.DataReturnVersionBuilder.AddEeeOutputAmount(A<string>._, A<string>._, A<WeeeCategory>._, A<ObligationType>._, A<decimal>._))
                 .MustHaveHappened(Repeated.Exactly.Times(3));
        }

        [Fact]
        public async Task Build_NoCollectedFromDcf_DoesNotProcessCollectedFromDcf()
        {
            var builder = new DataReturnVersionFromXmlBuilderHelper();

            var schemeReturn = new SchemeReturn()
            {
                CollectedFromDCF = null
            };

            await builder.Create().Build(schemeReturn);

            A.CallTo(() => builder.DataReturnVersionBuilder.AddWeeeCollectedAmount(WeeeCollectedAmountSourceType.Dcf, A<WeeeCategory>._, A<ObligationType>._, A<decimal>._))
                 .MustNotHaveHappened();
        }

        [Fact]
        public async Task Build_ProcessesAllReturnsInCollectedFromDcf()
        {
            var builder = new DataReturnVersionFromXmlBuilderHelper();

            var schemeReturn = new SchemeReturn()
            {
                CollectedFromDCF = new[] { A.Dummy<TonnageReturnType>(), A.Dummy<TonnageReturnType>() }
            };

            await builder.Create().Build(schemeReturn);

            A.CallTo(() => builder.DataReturnVersionBuilder.AddWeeeCollectedAmount(WeeeCollectedAmountSourceType.Dcf, A<WeeeCategory>._, A<ObligationType>._, A<decimal>._))
                 .MustHaveHappened(Repeated.Exactly.Times(2));
        }

        [Fact]
        public async Task Build_NoB2CWEEEFromDistributors_DoesNotProcessB2CWEEEFromDistributors()
        {
            var builder = new DataReturnVersionFromXmlBuilderHelper();

            var schemeReturn = new SchemeReturn()
            {
                B2CWEEEFromDistributors = null
            };

            await builder.Create().Build(schemeReturn);

            A.CallTo(() => builder.DataReturnVersionBuilder.AddWeeeCollectedAmount(WeeeCollectedAmountSourceType.Distributor, A<WeeeCategory>._, A<ObligationType>._, A<decimal>._))
                 .MustNotHaveHappened();
        }

        [Fact]
        public async Task Build_ProcessesAllReturnsInB2CWEEEFromDistributors()
        {
            var builder = new DataReturnVersionFromXmlBuilderHelper();

            var schemeReturn = new SchemeReturn()
            {
                B2CWEEEFromDistributors = new[] { A.Dummy<TonnageReturnType>(), A.Dummy<TonnageReturnType>() }
            };

            await builder.Create().Build(schemeReturn);

            A.CallTo(() => builder.DataReturnVersionBuilder.AddWeeeCollectedAmount(WeeeCollectedAmountSourceType.Distributor, A<WeeeCategory>._, A<ObligationType>._, A<decimal>._))
                 .MustHaveHappened(Repeated.Exactly.Times(2));
        }

        [Fact]
        public async Task Build_NoB2CWEEEFromFinalHolders_DoesNotProcessB2CWEEEFromFinalHolders()
        {
            var builder = new DataReturnVersionFromXmlBuilderHelper();

            var schemeReturn = new SchemeReturn()
            {
                B2CWEEEFromFinalHolders = null
            };

            await builder.Create().Build(schemeReturn);

            A.CallTo(() => builder.DataReturnVersionBuilder.AddWeeeCollectedAmount(WeeeCollectedAmountSourceType.FinalHolder, A<WeeeCategory>._, A<ObligationType>._, A<decimal>._))
                 .MustNotHaveHappened();
        }

        [Fact]
        public async Task Build_ProcessesAllReturnsInB2CWEEEFromFinalHolders()
        {
            var builder = new DataReturnVersionFromXmlBuilderHelper();

            var schemeReturn = new SchemeReturn()
            {
                B2CWEEEFromFinalHolders = new[] { A.Dummy<TonnageReturnType>(), A.Dummy<TonnageReturnType>() }
            };

            await builder.Create().Build(schemeReturn);

            A.CallTo(() => builder.DataReturnVersionBuilder.AddWeeeCollectedAmount(WeeeCollectedAmountSourceType.FinalHolder, A<WeeeCategory>._, A<ObligationType>._, A<decimal>._))
                 .MustHaveHappened(Repeated.Exactly.Times(2));
        }

        [Fact]
        public async Task Build_NoDeliveredToAaTF_DoesNotProcessDeliveredToAaTF()
        {
            var builder = new DataReturnVersionFromXmlBuilderHelper();

            var schemeReturn = new SchemeReturn()
            {
                DeliveredToATF = null
            };

            await builder.Create().Build(schemeReturn);

            A.CallTo(() => builder.DataReturnVersionBuilder.AddAatfDeliveredAmount(A<string>._, A<string>._, A<WeeeCategory>._, A<ObligationType>._, A<decimal>._))
                 .MustNotHaveHappened();
        }

        [Fact]
        public async Task Build_ProcessesAllReturnsInDeliveredToAaTF()
        {
            var builder = new DataReturnVersionFromXmlBuilderHelper();

            var schemeReturn = new SchemeReturn()
            {
                DeliveredToATF = new[]
                {
                    new SchemeReturnDeliveredToATF
                    {
                        DeliveredToFacility = A.Dummy<SchemeReturnDeliveredToATFDeliveredToFacility>(),
                        Return = new[] { A.Dummy<TonnageReturnType>(), A.Dummy<TonnageReturnType>() }
                    },
                    new SchemeReturnDeliveredToATF
                    {
                        DeliveredToFacility = A.Dummy<SchemeReturnDeliveredToATFDeliveredToFacility>(),
                        Return = new[] { A.Dummy<TonnageReturnType>() }
                    },
                }
            };

            await builder.Create().Build(schemeReturn);

            A.CallTo(() => builder.DataReturnVersionBuilder.AddAatfDeliveredAmount(A<string>._, A<string>._, A<WeeeCategory>._, A<ObligationType>._, A<decimal>._))
                 .MustHaveHappened(Repeated.Exactly.Times(3));
        }

        [Fact]
        public async Task Build_NoDeliveredToAe_DoesNotProcessDeliveredToAe()
        {
            var builder = new DataReturnVersionFromXmlBuilderHelper();

            var schemeReturn = new SchemeReturn()
            {
                DeliveredToAE = null
            };

            await builder.Create().Build(schemeReturn);

            A.CallTo(() => builder.DataReturnVersionBuilder.AddAeDeliveredAmount(A<string>._, A<string>._, A<WeeeCategory>._, A<ObligationType>._, A<decimal>._))
                 .MustNotHaveHappened();
        }

        [Fact]
        public async Task Build_ProcessesAllReturnsInDeliveredToAe()
        {
            var builder = new DataReturnVersionFromXmlBuilderHelper();

            var schemeReturn = new SchemeReturn()
            {
                DeliveredToAE = new[]
                {
                    new SchemeReturnDeliveredToAE
                    {
                        DeliveredToOperator = A.Dummy<SchemeReturnDeliveredToAEDeliveredToOperator>(),
                        Return = new[] { A.Dummy<TonnageReturnType>(), A.Dummy<TonnageReturnType>() }
                    },
                    new SchemeReturnDeliveredToAE
                    {
                        DeliveredToOperator = A.Dummy<SchemeReturnDeliveredToAEDeliveredToOperator>(),
                        Return = new[] { A.Dummy<TonnageReturnType>() }
                    },
                }
            };

            await builder.Create().Build(schemeReturn);

            A.CallTo(() => builder.DataReturnVersionBuilder.AddAeDeliveredAmount(A<string>._, A<string>._, A<WeeeCategory>._, A<ObligationType>._, A<decimal>._))
                 .MustHaveHappened(Repeated.Exactly.Times(3));
        }

        [Fact]
        public async Task Build_ReturnsResult()
        {
            var builder = new DataReturnVersionFromXmlBuilderHelper();

            var expectedResult = new DataReturnVersionBuilderResult(A.Dummy<DataReturnVersion>(), A.Dummy<List<ErrorData>>());

            A.CallTo(() => builder.DataReturnVersionBuilder.Build())
                 .Returns(expectedResult);

            var actualResult = await builder.Create().Build(A.Dummy<SchemeReturn>());

            Assert.Equal(expectedResult, actualResult);
        }

        private class DataReturnVersionFromXmlBuilderHelper
        {
            public IDataReturnVersionBuilder DataReturnVersionBuilder;

            public DataReturnVersionFromXmlBuilderHelper()
            {
                DataReturnVersionBuilder = A.Fake<IDataReturnVersionBuilder>();
            }

            public DataReturnVersionFromXmlBuilder Create()
            {
                return new DataReturnVersionFromXmlBuilder(DataReturnVersionBuilder);
            }
        }
    }
}
