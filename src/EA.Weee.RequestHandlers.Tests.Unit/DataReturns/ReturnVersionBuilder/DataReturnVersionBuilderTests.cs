namespace EA.Weee.RequestHandlers.Tests.Unit.DataReturns.ReturnVersionBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Shared;
    using Domain.DataReturns;
    using Domain.Lookup;
    using Domain.Producer;
    using FakeItEasy;
    using RequestHandlers.DataReturns.BusinessValidation;
    using RequestHandlers.DataReturns.BusinessValidation.Rules;
    using RequestHandlers.DataReturns.ReturnVersionBuilder;
    using Xunit;
    using ObligationType = Domain.Obligation.ObligationType;
    using Scheme = Domain.Scheme.Scheme;

    public class DataReturnVersionBuilderTests
    {
        [Fact]
        public async Task Build_ReturnsDataReturnVersionWhenNoErrors()
        {
            var builder = new DataReturnVersionBuilderHelper().CreateWithErrorData(new List<ErrorData>());
            await builder.AddAatfDeliveredAmount("Approval Number", "Facility name", A<WeeeCategory>._, ObligationType.B2C, A<decimal>._);

            var result = await builder.Build();

            Assert.NotNull(result.DataReturnVersion);
        }

        [Fact]
        public async Task Build_ReturnsDataReturnVersionAndWarnings_WhenNoErrorsButWithWarnings()
        {
            var warnings = new List<ErrorData> { new ErrorData("A warning", ErrorLevel.Warning) };

            var builder = new DataReturnVersionBuilderHelper().CreateWithErrorData(warnings);
            await builder.AddAatfDeliveredAmount("Approval Number", "Facility name", A<WeeeCategory>._, ObligationType.B2C, A<decimal>._);

            var result = await builder.Build();

            Assert.NotNull(result.DataReturnVersion);
            Assert.Equal(warnings, result.ErrorData);
        }

        [Fact]
        public async Task Build_ReturnsNullDataReturnVersionWhenContainsErrors_AndReturnsErrors()
        {
            var errors = new List<ErrorData> { new ErrorData("An Error", ErrorLevel.Error) };
            var builder = new DataReturnVersionBuilderHelper().CreateWithErrorData(errors);
            await builder.AddAatfDeliveredAmount("Approval Number", "Facility name", A<WeeeCategory>._, ObligationType.B2C, A<decimal>._);

            var result = await builder.Build();

            Assert.Null(result.DataReturnVersion);
            Assert.Equal(errors, result.ErrorData);
        }

        /// <summary>
        /// This test ensures that a <see cref="DataReturn"/> is created if none is currently available
        /// for the same scheme, compliance year and quarter.
        /// </summary>
        [Fact]
        public async Task Build_NoExistingDataReturn_CreatesNewDataReturn()
        {
            var helper = new DataReturnVersionBuilderHelper();

            A.CallTo(() => helper.DataAccess.FetchDataReturnOrDefault())
                .Returns((DataReturn)null);

            var builder = helper.Create();
            await builder.AddAatfDeliveredAmount("Approval Number", "Facility name", A<WeeeCategory>._, ObligationType.B2C, A<decimal>._);

            var result = await builder.Build();

            Assert.NotNull(result.DataReturnVersion.DataReturn);
        }

        /// <summary>
        /// This test ensures that an existing <see cref="DataReturn"/> is reused if
        /// it has the matching scheme, compliance year and quarter for the <see cref="DataReturnVersion"/>
        /// being built.
        /// </summary>
        [Fact]
        public async Task Build_ExistingDataReturn_ReturnsDataReturnVersionWithExistingDataReturn()
        {
            var dataReturn = new DataReturn(A.Dummy<Scheme>(), A.Dummy<Quarter>());

            var helper = new DataReturnVersionBuilderHelper();

            A.CallTo(() => helper.DataAccess.FetchDataReturnOrDefault())
                .Returns(dataReturn);

            var builder = helper.Create();
            await builder.AddAatfDeliveredAmount("Approval Number", "Facility name", A<WeeeCategory>._, ObligationType.B2C, A<decimal>._);

            var result = await builder.Build();

            Assert.Equal(dataReturn, result.DataReturnVersion.DataReturn);
        }

        /// <summary>
        /// This test ensures that new data properties (WeeeCollectedReturnVersion, WeeeDeliveredReturnVersion, EeeOutputReturnVersion)
        /// are created when no <see cref="DataReturnVersion"/> currently exists in the database.
        /// </summary>
        [Fact]
        public async Task Build_NoExistingLatestDataReturnVersion_ReturnsDataReturnVersionWithNewData()
        {
            var helper = new DataReturnVersionBuilderHelper();

            A.CallTo(() => helper.DataAccess.GetLatestDataReturnVersionOrDefault())
                .Returns((DataReturnVersion)null);

            var builder = helper.Create();
            await builder.AddEeeOutputAmount("PRN", "ProducerName", WeeeCategory.ConsumerEquipment, ObligationType.B2C, 100);
            await builder.AddAatfDeliveredAmount("ApprovalNumber", "FacilityName", WeeeCategory.ConsumerEquipment, ObligationType.B2C, 100);
            await builder.AddWeeeCollectedAmount(WeeeCollectedAmountSourceType.Dcf, WeeeCategory.ConsumerEquipment, ObligationType.B2C, 100);

            var result = await builder.Build();

            Assert.NotNull(result.DataReturnVersion);
            Assert.NotNull(result.DataReturnVersion.EeeOutputReturnVersion);
            Assert.NotNull(result.DataReturnVersion.WeeeCollectedReturnVersion);
            Assert.NotNull(result.DataReturnVersion.WeeeDeliveredReturnVersion);
        }

        /// <summary>
        /// This test ensures that a new <see cref="WeeeCollectedReturnVersion"/> is created when the existing <see cref="DataReturnVersion"/>
        /// from the database has a null value for the same property.
        /// </summary>
        [Fact]
        public async Task Build_ExistingLatestDataReturnVersion_WithNoExistingWeeeCollectedReturnVersion_ReturnsDataReturnVersionWithNewWeeeCollectedReturnVersion()
        {
            var dataReturn = new DataReturn(A.Dummy<Scheme>(), A.Dummy<Quarter>());
            var dataReturnVersion = new DataReturnVersion(dataReturn, null,
                A.Dummy<WeeeDeliveredReturnVersion>(), A.Dummy<EeeOutputReturnVersion>());

            var helper = new DataReturnVersionBuilderHelper();

            A.CallTo(() => helper.DataAccess.GetLatestDataReturnVersionOrDefault())
                .Returns(dataReturnVersion);

            var builder = helper.Create();
            await builder.AddWeeeCollectedAmount(WeeeCollectedAmountSourceType.Dcf, WeeeCategory.ConsumerEquipment, ObligationType.B2C, 100);

            var result = await builder.Build();

            Assert.NotNull(result.DataReturnVersion.WeeeCollectedReturnVersion);
            Assert.Equal(1, result.DataReturnVersion.WeeeCollectedReturnVersion.WeeeCollectedAmounts.Count);
        }

        /// <summary>
        /// This test ensures that a new <see cref="WeeeDeliveredReturnVersion"/> is created when the existing <see cref="DataReturnVersion"/> 
        /// from the database has a null value for the same property.
        /// </summary>
        [Fact]
        public async Task Build_ExistingLatestDataReturnVersion_WithNoExistingWeeeDeliveredReturnVersion_ReturnsDataReturnVersionWithNewWeeeDeliveredReturnVersion()
        {
            var dataReturn = new DataReturn(A.Dummy<Scheme>(), A.Dummy<Quarter>());
            var dataReturnVersion = new DataReturnVersion(dataReturn, A.Dummy<WeeeCollectedReturnVersion>(),
                null, A.Dummy<EeeOutputReturnVersion>());

            var helper = new DataReturnVersionBuilderHelper();

            A.CallTo(() => helper.DataAccess.GetLatestDataReturnVersionOrDefault())
                .Returns(dataReturnVersion);

            var builder = helper.Create();
            await builder.AddAatfDeliveredAmount("ApprovalNumber", "FacilityName", WeeeCategory.ConsumerEquipment, ObligationType.B2C, 100);

            var result = await builder.Build();

            Assert.NotNull(result.DataReturnVersion.WeeeDeliveredReturnVersion);
            Assert.Equal(1, result.DataReturnVersion.WeeeDeliveredReturnVersion.WeeeDeliveredAmounts.Count);
        }

        /// <summary>
        /// This test ensures that a new <see cref="EeeOutputReturnVersion"/> is created when the existing <see cref="DataReturnVersion"/>
        /// from the database has a null value for the same property.
        /// </summary>
        [Fact]
        public async Task Build_ExistingLatestDataReturnVersion_WithNoExistingEeeOutputReturnVersion_ReturnsDataReturnVersionWithNewEeeOutputReturnVersion()
        {
            var dataReturn = new DataReturn(A.Dummy<Scheme>(), A.Dummy<Quarter>());
            var dataReturnVersion = new DataReturnVersion(dataReturn, A.Dummy<WeeeCollectedReturnVersion>(),
                A.Dummy<WeeeDeliveredReturnVersion>(), null);

            var helper = new DataReturnVersionBuilderHelper();

            A.CallTo(() => helper.DataAccess.GetLatestDataReturnVersionOrDefault())
                .Returns(dataReturnVersion);

            var registeredProducer = A.Fake<RegisteredProducer>();
            A.CallTo(() => helper.DataAccess.GetRegisteredProducer(A<string>._))
                .Returns(registeredProducer);

            var builder = helper.Create();
            await builder.AddEeeOutputAmount("PRN", "ProducerName", WeeeCategory.ConsumerEquipment, ObligationType.B2C, 100);

            var result = await builder.Build();

            Assert.NotNull(result.DataReturnVersion.EeeOutputReturnVersion);
            Assert.Equal(1, result.DataReturnVersion.EeeOutputReturnVersion.EeeOutputAmounts.Count);
        }

        /// <summary>
        /// This test ensures that the <see cref="WeeeCollectedReturnVersion"/> of the existing <see cref="DataReturnVersion"/> is reused when the <see cref="WeeeCollectedAmount"/>
        /// items for the new and existing data return versions are all equal. 
        /// </summary>
        [Fact]
        public async Task Build_ExistingLatestDataReturnVersion_WithAllExistingWeeeCollectedAmounts_ReturnsDataReturnVersionWithExistingWeeeCollectedReturnVersion()
        {
            var weeeCollectedReturnVersion = new WeeeCollectedReturnVersion();
            weeeCollectedReturnVersion.AddWeeeCollectedAmount(
                new WeeeCollectedAmount(WeeeCollectedAmountSourceType.Dcf, ObligationType.B2C, WeeeCategory.ConsumerEquipment, 100));

            var dataReturn = new DataReturn(A.Dummy<Scheme>(), A.Dummy<Quarter>());
            var dataReturnVersion = new DataReturnVersion(dataReturn, weeeCollectedReturnVersion,
                A.Dummy<WeeeDeliveredReturnVersion>(), A.Dummy<EeeOutputReturnVersion>());

            var helper = new DataReturnVersionBuilderHelper();

            A.CallTo(() => helper.DataAccess.GetLatestDataReturnVersionOrDefault())
                .Returns(dataReturnVersion);

            var builder = helper.Create();
            await builder.AddWeeeCollectedAmount(WeeeCollectedAmountSourceType.Dcf, WeeeCategory.ConsumerEquipment, ObligationType.B2C, 100);

            var result = await builder.Build();

            Assert.Same(weeeCollectedReturnVersion, result.DataReturnVersion.WeeeCollectedReturnVersion);
        }

        /// <summary>
        /// This test ensures that the <see cref="WeeeDeliveredReturnVersion"/> of the existing <see cref="DataReturnVersion"/> is reused when the <see cref="WeeeDeliveredAmount"/>
        /// items for the new and existing data return versions are all equal. 
        /// </summary>
        [Fact]
        public async Task Build_ExistingLatestDataReturnVersion_WithAllExistingWeeeDeliveredAmounts_ReturnsDataReturnVersionWithExistingWeeeDeliveredReturnVersion()
        {
            var aatfDeliveryLocation = new AatfDeliveryLocation("ApprovalNumber", "FacilityName");

            var weeeDeliveredReturnVersion = new WeeeDeliveredReturnVersion();
            weeeDeliveredReturnVersion.AddWeeeDeliveredAmount(
                new WeeeDeliveredAmount(ObligationType.B2C, WeeeCategory.ConsumerEquipment, 100, aatfDeliveryLocation));

            var dataReturn = new DataReturn(A.Dummy<Scheme>(), A.Dummy<Quarter>());
            var dataReturnVersion = new DataReturnVersion(dataReturn, A.Dummy<WeeeCollectedReturnVersion>(),
                weeeDeliveredReturnVersion, A.Dummy<EeeOutputReturnVersion>());

            var helper = new DataReturnVersionBuilderHelper();

            A.CallTo(() => helper.DataAccess.GetLatestDataReturnVersionOrDefault())
                .Returns(dataReturnVersion);

            A.CallTo(() => helper.DataAccess.GetOrAddAatfDeliveryLocation(A<string>._, A<string>._))
                .Returns(aatfDeliveryLocation);

            var builder = helper.Create();
            await builder.AddAatfDeliveredAmount("ApprovalNumber", "FacilityName", WeeeCategory.ConsumerEquipment, ObligationType.B2C, 100);

            var result = await builder.Build();

            Assert.Same(weeeDeliveredReturnVersion, result.DataReturnVersion.WeeeDeliveredReturnVersion);
        }

        /// <summary>
        /// This test ensures that the <see cref="EeeOutputReturnVersion"/> of the existing <see cref="DataReturnVersion"/> is reused when the <see cref="EeeOutputAmount"/>
        /// items for the new and existing data return versions are all equal. 
        /// </summary>
        [Fact]
        public async Task Build_ExistingLatestDataReturnVersion_WithAllExistingEeeOutputAmounts_ReturnsDataReturnVersionWithExistingEeeOutputReturnVersion()
        {
            var registeredProducer = A.Fake<RegisteredProducer>();
            A.CallTo(() => registeredProducer.Equals(A<RegisteredProducer>._))
                .Returns(true);

            var eeeOutputReturnVersion = new EeeOutputReturnVersion();
            eeeOutputReturnVersion.AddEeeOutputAmount(
                new EeeOutputAmount(ObligationType.B2C, WeeeCategory.ConsumerEquipment, 100, registeredProducer));

            var dataReturn = new DataReturn(A.Dummy<Scheme>(), A.Dummy<Quarter>());
            var dataReturnVersion = new DataReturnVersion(dataReturn, A.Dummy<WeeeCollectedReturnVersion>(),
                A.Dummy<WeeeDeliveredReturnVersion>(), eeeOutputReturnVersion);

            var helper = new DataReturnVersionBuilderHelper();

            A.CallTo(() => helper.DataAccess.GetLatestDataReturnVersionOrDefault())
                .Returns(dataReturnVersion);

            A.CallTo(() => helper.DataAccess.GetRegisteredProducer(A<string>._))
                .Returns(registeredProducer);

            var builder = helper.Create();
            await builder.AddEeeOutputAmount("PRN", "ProducerName", WeeeCategory.ConsumerEquipment, ObligationType.B2C, 100);

            var result = await builder.Build();

            Assert.Same(eeeOutputReturnVersion, result.DataReturnVersion.EeeOutputReturnVersion);
        }

        /// <summary>
        /// This test ensures that equal <see cref="WeeeCollectedAmount"/> items from the existing <see cref="DataReturnVersion"/> are reused.
        /// </summary>
        [Fact]
        public async Task Build_ExistingLatestDataReturnVersion_WithSomeExistingWeeeCollectedAmounts_ReturnsDataReturnVersionWithExistingWeeeCollectedAmounts()
        {
            var weeeCollectedAmount1 = new WeeeCollectedAmount(WeeeCollectedAmountSourceType.Dcf, ObligationType.B2C, WeeeCategory.DisplayEquipment, 100);
            var weeeCollectedAmount2 = new WeeeCollectedAmount(WeeeCollectedAmountSourceType.Dcf, ObligationType.B2C, WeeeCategory.ConsumerEquipment, 100);
            var weeeCollectedAmount3 = new WeeeCollectedAmount(WeeeCollectedAmountSourceType.Distributor, ObligationType.B2C, WeeeCategory.ConsumerEquipment, 100);
            var weeeCollectedAmount4 = new WeeeCollectedAmount(WeeeCollectedAmountSourceType.FinalHolder, ObligationType.B2C, WeeeCategory.ConsumerEquipment, 100);

            var weeeCollectedReturnVersion = new WeeeCollectedReturnVersion();
            weeeCollectedReturnVersion.AddWeeeCollectedAmount(weeeCollectedAmount1);
            weeeCollectedReturnVersion.AddWeeeCollectedAmount(weeeCollectedAmount2);
            weeeCollectedReturnVersion.AddWeeeCollectedAmount(weeeCollectedAmount3);
            weeeCollectedReturnVersion.AddWeeeCollectedAmount(weeeCollectedAmount4);

            var dataReturn = new DataReturn(A.Dummy<Scheme>(), A.Dummy<Quarter>());
            var dataReturnVersion = new DataReturnVersion(dataReturn, weeeCollectedReturnVersion,
                A.Dummy<WeeeDeliveredReturnVersion>(), A.Dummy<EeeOutputReturnVersion>());

            var helper = new DataReturnVersionBuilderHelper();

            A.CallTo(() => helper.DataAccess.GetLatestDataReturnVersionOrDefault())
                .Returns(dataReturnVersion);

            var builder = helper.Create();
            await builder.AddWeeeCollectedAmount(WeeeCollectedAmountSourceType.Dcf, WeeeCategory.ConsumerEquipment, ObligationType.B2C, 100);
            await builder.AddWeeeCollectedAmount(WeeeCollectedAmountSourceType.Distributor, WeeeCategory.ConsumerEquipment, ObligationType.B2C, 100);

            var result = await builder.Build();

            Assert.NotSame(weeeCollectedReturnVersion, result.DataReturnVersion.WeeeCollectedReturnVersion);
            Assert.Collection(result.DataReturnVersion.WeeeCollectedReturnVersion.WeeeCollectedAmounts,
                r1 => Assert.Same(weeeCollectedAmount2, r1),
                r2 => Assert.Same(weeeCollectedAmount3, r2));
        }

        /// <summary>
        /// This test ensures that equal <see cref="WeeeDeliveredAmount"/> items from the existing <see cref="DataReturnVersion"/> are reused.
        /// </summary>
        [Fact]
        public async Task Build_ExistingLatestDataReturnVersion_WithSomeExistingWeeeDeliveredAmounts_ReturnsDataReturnVersionWithExistingWeeeDeliveredAmounts()
        {
            var weeeDeliveredAmount1 = new WeeeDeliveredAmount(ObligationType.B2C, WeeeCategory.ConsumerEquipment, 100, new AatfDeliveryLocation("ApprovalNumber", "FacilityName"));
            var weeeDeliveredAmount2 = new WeeeDeliveredAmount(ObligationType.B2C, WeeeCategory.ITAndTelecommsEquipment, 200, new AatfDeliveryLocation("ApprovalNumber", "FacilityName"));
            var weeeDeliveredAmount3 = new WeeeDeliveredAmount(ObligationType.B2C, WeeeCategory.DisplayEquipment, 100, new AeDeliveryLocation("ApprovalNumber", "FacilityName"));
            var weeeDeliveredAmount4 = new WeeeDeliveredAmount(ObligationType.B2C, WeeeCategory.LargeHouseholdAppliances, 300, new AeDeliveryLocation("ApprovalNumber", "OperatorName"));

            var weeeDeliveredReturnVersion = new WeeeDeliveredReturnVersion();
            weeeDeliveredReturnVersion.AddWeeeDeliveredAmount(weeeDeliveredAmount1);
            weeeDeliveredReturnVersion.AddWeeeDeliveredAmount(weeeDeliveredAmount2);
            weeeDeliveredReturnVersion.AddWeeeDeliveredAmount(weeeDeliveredAmount3);
            weeeDeliveredReturnVersion.AddWeeeDeliveredAmount(weeeDeliveredAmount4);

            var dataReturn = new DataReturn(A.Dummy<Scheme>(), A.Dummy<Quarter>());
            var dataReturnVersion = new DataReturnVersion(dataReturn, A.Dummy<WeeeCollectedReturnVersion>(),
                weeeDeliveredReturnVersion, A.Dummy<EeeOutputReturnVersion>());

            var helper = new DataReturnVersionBuilderHelper();

            A.CallTo(() => helper.DataAccess.GetLatestDataReturnVersionOrDefault())
                .Returns(dataReturnVersion);

            A.CallTo(() => helper.DataAccess.GetOrAddAatfDeliveryLocation(A<string>._, A<string>._))
                .Returns(new AatfDeliveryLocation("ApprovalNumber", "FacilityName"));
            A.CallTo(() => helper.DataAccess.GetOrAddAeDeliveryLocation(A<string>._, A<string>._))
                .Returns(new AeDeliveryLocation("ApprovalNumber", "OperatorName"));

            var builder = helper.Create();
            await builder.AddAatfDeliveredAmount("ApprovalNumber", "FacilityName", WeeeCategory.ConsumerEquipment, ObligationType.B2C, 100);
            await builder.AddAeDeliveredAmount("ApprovalNumber", "OperatorName", WeeeCategory.LargeHouseholdAppliances, ObligationType.B2C, 300);

            var result = await builder.Build();

            Assert.NotSame(weeeDeliveredReturnVersion, result.DataReturnVersion.WeeeDeliveredReturnVersion);
            Assert.Collection(result.DataReturnVersion.WeeeDeliveredReturnVersion.WeeeDeliveredAmounts,
                r1 => Assert.Same(weeeDeliveredAmount1, r1),
                r2 => Assert.Same(weeeDeliveredAmount4, r2));
        }

        /// <summary>
        /// This test ensures that equal <see cref="EeeOutputAmount"/> items from the existing <see cref="DataReturnVersion"/> are reused.
        /// </summary>
        [Fact]
        public async Task Build_ExistingLatestDataReturnVersion_WithSomeExistingEeeOutputAmounts_ReturnsDataReturnVersionWithExistingEeeOutputAmounts()
        {
            var registeredProducer1 = A.Fake<RegisteredProducer>();
            A.CallTo(() => registeredProducer1.ProducerRegistrationNumber)
                .Returns("Producer1");
            A.CallTo(() => registeredProducer1.Equals(A<RegisteredProducer>._))
                .Returns(true);

            var registeredProducer2 = A.Fake<RegisteredProducer>();
            A.CallTo(() => registeredProducer2.ProducerRegistrationNumber)
                .Returns("Producer2");
            A.CallTo(() => registeredProducer2.Equals(A<RegisteredProducer>._))
                .Returns(true);

            var eeeOutputAmount1 = new EeeOutputAmount(ObligationType.B2C, WeeeCategory.ConsumerEquipment, 100, registeredProducer1);
            var eeeOutputAmount2 = new EeeOutputAmount(ObligationType.B2C, WeeeCategory.MedicalDevices, 200, registeredProducer1);
            var eeeOutputAmount3 = new EeeOutputAmount(ObligationType.B2C, WeeeCategory.ToysLeisureAndSports, 100, registeredProducer2);
            var eeeOutputAmount4 = new EeeOutputAmount(ObligationType.B2C, WeeeCategory.ConsumerEquipment, 150, registeredProducer2);

            var eeeOutputReturnVersion = new EeeOutputReturnVersion();
            eeeOutputReturnVersion.AddEeeOutputAmount(eeeOutputAmount1);
            eeeOutputReturnVersion.AddEeeOutputAmount(eeeOutputAmount2);
            eeeOutputReturnVersion.AddEeeOutputAmount(eeeOutputAmount3);
            eeeOutputReturnVersion.AddEeeOutputAmount(eeeOutputAmount4);

            var dataReturn = new DataReturn(A.Dummy<Scheme>(), A.Dummy<Quarter>());
            var dataReturnVersion = new DataReturnVersion(dataReturn, A.Dummy<WeeeCollectedReturnVersion>(),
                A.Dummy<WeeeDeliveredReturnVersion>(), eeeOutputReturnVersion);

            var helper = new DataReturnVersionBuilderHelper();

            A.CallTo(() => helper.DataAccess.GetLatestDataReturnVersionOrDefault())
                .Returns(dataReturnVersion);

            A.CallTo(() => helper.DataAccess.GetRegisteredProducer("Producer1"))
                .Returns(registeredProducer1);

            A.CallTo(() => helper.DataAccess.GetRegisteredProducer("Producer2"))
                .Returns(registeredProducer2);

            var builder = helper.Create();
            await builder.AddEeeOutputAmount("Producer1", "ProducerName", WeeeCategory.MedicalDevices, ObligationType.B2C, 200);
            await builder.AddEeeOutputAmount("Producer2", "ProducerName", WeeeCategory.ToysLeisureAndSports, ObligationType.B2C, 100);

            var result = await builder.Build();

            Assert.NotSame(eeeOutputReturnVersion, result.DataReturnVersion.EeeOutputReturnVersion);
            Assert.Collection(result.DataReturnVersion.EeeOutputReturnVersion.EeeOutputAmounts,
                r1 => Assert.Same(eeeOutputAmount2, r1),
                r2 => Assert.Same(eeeOutputAmount3, r2));
        }

        /// <summary>
        /// Consider the following scenario: A <see cref="DataReturnVersion"/> exists in the database with all of the properties
        /// (WeeeCollectedReturnVersion, WeeeDeliveredReturnVersion and EeeOutputReturnVersion) having values (i.e. they are not null).
        /// The <see cref="DataReturnVersionBuilder"/> is used to build a new <see cref="DataReturnVersion"/> but values are not specified for any of the
        /// return version properties. The test ensures that the properties of the generated object will have null values.
        /// </summary>
        [Fact]
        public async Task Build_ExistingLatestDataReturnVersion_ContainsReturnData_WithNoDataForNewReturn_ReturnsNullReturnVersions()
        {
            var dataReturn = new DataReturn(A.Dummy<Scheme>(), A.Dummy<Quarter>());
            var dataReturnVersion = new DataReturnVersion(dataReturn, A.Dummy<WeeeCollectedReturnVersion>(),
                A.Dummy<WeeeDeliveredReturnVersion>(), A.Dummy<EeeOutputReturnVersion>());

            var helper = new DataReturnVersionBuilderHelper();

            A.CallTo(() => helper.DataAccess.GetLatestDataReturnVersionOrDefault())
                .Returns(dataReturnVersion);

            var builder = helper.Create();

            var result = await builder.Build();

            Assert.Null(result.DataReturnVersion.WeeeCollectedReturnVersion);
            Assert.Null(result.DataReturnVersion.WeeeDeliveredReturnVersion);
            Assert.Null(result.DataReturnVersion.EeeOutputReturnVersion);
        }

        /// <summary>
        /// Consider the following scenario: A <see cref="DataReturnVersion"/> exists in the database with null values for
        /// its WeeeCollectedReturnVersion, WeeeDeliveredReturnVersion and EeeOutputReturnVersion. The <see cref="DataReturnVersionBuilder"/>
        /// is used to build a new <see cref="DataReturnVersion"/> but values are not specified for any of the return version properties. The test ensures
        /// that the properties of the generated object will have null values.
        /// </summary>
        [Fact]
        public async Task Build_ExistingLatestDataReturnVersion_DoesNotContainReturnData_WithNoDataForNewReturn_ReturnsNullReturnVersions()
        {
            var dataReturn = new DataReturn(A.Dummy<Scheme>(), A.Dummy<Quarter>());
            var dataReturnVersion = new DataReturnVersion(dataReturn, null, null, null);

            var helper = new DataReturnVersionBuilderHelper();

            A.CallTo(() => helper.DataAccess.GetLatestDataReturnVersionOrDefault())
                .Returns(dataReturnVersion);

            var builder = helper.Create();

            var result = await builder.Build();

            Assert.Null(result.DataReturnVersion.WeeeCollectedReturnVersion);
            Assert.Null(result.DataReturnVersion.WeeeDeliveredReturnVersion);
            Assert.Null(result.DataReturnVersion.EeeOutputReturnVersion);
        }

        /// <summary>
        /// Consider the following scenario: A <see cref="DataReturnVersion"/> does not exist. The <see cref="DataReturnVersionBuilder"/>
        /// is used to build a new <see cref="DataReturnVersion"/> but values are not specified for any of the return version properties. The test ensures
        /// that the properties of the generated object will have null values.
        /// </summary>
        [Fact]
        public async Task Build_NoExistingLatestDataReturnVersion_WithNoDataForNewReturn_ReturnsNullReturnVersions()
        {
            var helper = new DataReturnVersionBuilderHelper();

            A.CallTo(() => helper.DataAccess.GetLatestDataReturnVersionOrDefault())
                .Returns((DataReturnVersion)null);

            var builder = helper.Create();

            var result = await builder.Build();

            Assert.Null(result.DataReturnVersion.WeeeCollectedReturnVersion);
            Assert.Null(result.DataReturnVersion.WeeeDeliveredReturnVersion);
            Assert.Null(result.DataReturnVersion.EeeOutputReturnVersion);
        }

        [Fact]
        public async Task AddAatfDeliveredAmount_CreatesAatfDeliveredAmountDomainObject()
        {
            var helper = new DataReturnVersionBuilderHelper();
            A.CallTo(() => helper.DataAccess.GetLatestDataReturnVersionOrDefault())
                .Returns((DataReturnVersion)null);

            var aatfDeliveryLocation = new AatfDeliveryLocation("Approval Number", "Facility name");
            A.CallTo(() => helper.DataAccess.GetOrAddAatfDeliveryLocation(A<string>._, A<string>._))
                .Returns(aatfDeliveryLocation);

            var builder = helper.Create();
            await builder.AddAatfDeliveredAmount("Approval Number", "Facility name", A<WeeeCategory>._, ObligationType.B2C, A<decimal>._);

            var result = await builder.Build();

            Assert.Equal(1, result.DataReturnVersion.WeeeDeliveredReturnVersion.WeeeDeliveredAmounts.Count);
            Assert.Collection(result.DataReturnVersion.WeeeDeliveredReturnVersion.WeeeDeliveredAmounts,
                r => Assert.Equal("Approval Number", r.AatfDeliveryLocation.ApprovalNumber));
            Assert.Same(aatfDeliveryLocation, result.DataReturnVersion.WeeeDeliveredReturnVersion.WeeeDeliveredAmounts.Single().AatfDeliveryLocation);
        }

        [Fact]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "aeDeliveryLocation is valid.")]
        public async Task AddAeDeliveredAmount_CreatesAeDeliveredAmountDomainObject()
        {
            var helper = new DataReturnVersionBuilderHelper();
            A.CallTo(() => helper.DataAccess.GetLatestDataReturnVersionOrDefault())
                .Returns((DataReturnVersion)null);

            var aeDeliveryLocation = new AeDeliveryLocation("Approval Number", "Operator name");
            A.CallTo(() => helper.DataAccess.GetOrAddAeDeliveryLocation(A<string>._, A<string>._))
                .Returns(aeDeliveryLocation);

            var builder = helper.Create();
            await builder.AddAeDeliveredAmount("Approval Number", "Operator name", A<WeeeCategory>._, ObligationType.B2C, A<decimal>._);

            var result = await builder.Build();

            Assert.Equal(1, result.DataReturnVersion.WeeeDeliveredReturnVersion.WeeeDeliveredAmounts.Count);
            Assert.Collection(result.DataReturnVersion.WeeeDeliveredReturnVersion.WeeeDeliveredAmounts,
                r => Assert.Equal("Approval Number", r.AeDeliveryLocation.ApprovalNumber));
            Assert.Same(aeDeliveryLocation, result.DataReturnVersion.WeeeDeliveredReturnVersion.WeeeDeliveredAmounts.Single().AeDeliveryLocation);
        }

        [Fact]
        public async Task AddWeeeCollectedAmount_CreatesWeeeCollectedAmountDomainObject()
        {
            var type = WeeeCollectedAmountSourceType.Distributor;

            var helper = new DataReturnVersionBuilderHelper();
            A.CallTo(() => helper.DataAccess.GetLatestDataReturnVersionOrDefault())
                .Returns((DataReturnVersion)null);

            var builder = helper.Create();
            await builder.AddWeeeCollectedAmount(type, A<WeeeCategory>._, ObligationType.B2C, A<decimal>._);

            var result = await builder.Build();

            Assert.Equal(1, result.DataReturnVersion.WeeeCollectedReturnVersion.WeeeCollectedAmounts.Count);

            Assert.Collection(result.DataReturnVersion.WeeeCollectedReturnVersion.WeeeCollectedAmounts,
                r => Assert.Equal(type, r.SourceType));
        }

        [Fact]
        public async Task AddEeeOutputAmount_CreatesEeeOutputAmountDomainObject_IfNoErrors()
        {
            var helper = new DataReturnVersionBuilderHelper();
            A.CallTo(() => helper.DataAccess.GetLatestDataReturnVersionOrDefault())
                .Returns((DataReturnVersion)null);

            A.CallTo(() => helper.EeeValidator.Validate(A<string>._, A<string>._, A<WeeeCategory>._, A<ObligationType>._, A<decimal>._))
                .Returns(new List<ErrorData>());

            A.CallTo(() => helper.DataAccess.GetRegisteredProducer(A<string>._))
                .Returns(new RegisteredProducer("Registration Number", 2016, A.Dummy<Scheme>()));

            var builder = helper.Create();
            await builder.AddEeeOutputAmount("Registration Number", A<string>._, A<WeeeCategory>._, ObligationType.B2C, A<decimal>._);

            var result = await builder.Build();

            Assert.Equal(1, result.DataReturnVersion.EeeOutputReturnVersion.EeeOutputAmounts.Count);
            Assert.Collection(result.DataReturnVersion.EeeOutputReturnVersion.EeeOutputAmounts,
                r => Assert.Equal("Registration Number", r.RegisteredProducer.ProducerRegistrationNumber));
        }

        [Fact]
        public async Task AddEeeOutputAmount_CreatesEeeOutputAmountDomainObject_IfWarningsOnly()
        {
            var helper = new DataReturnVersionBuilderHelper();
            A.CallTo(() => helper.DataAccess.GetLatestDataReturnVersionOrDefault())
                .Returns((DataReturnVersion)null);

            A.CallTo(() => helper.EeeValidator.Validate(A<string>._, A<string>._, A<WeeeCategory>._, A<ObligationType>._, A<decimal>._))
                .Returns(new List<ErrorData> { new ErrorData("A warning", ErrorLevel.Warning) });

            A.CallTo(() => helper.DataAccess.GetRegisteredProducer(A<string>._))
                .Returns(new RegisteredProducer("Registration Number", 2016, A.Dummy<Scheme>()));

            var builder = helper.Create();
            await builder.AddEeeOutputAmount("Registration Number", A<string>._, A<WeeeCategory>._, ObligationType.B2C, A<decimal>._);

            var result = await builder.Build();

            Assert.Equal(1, result.DataReturnVersion.EeeOutputReturnVersion.EeeOutputAmounts.Count);
            Assert.Collection(result.DataReturnVersion.EeeOutputReturnVersion.EeeOutputAmounts,
                r => Assert.Equal("Registration Number", r.RegisteredProducer.ProducerRegistrationNumber));
        }

        [Fact]
        public async Task AddEeeOutputAmount_DoesNotCreateEeeOutputAmountDomainObject_IfValidationError()
        {
            var helper = new DataReturnVersionBuilderHelper();

            A.CallTo(() => helper.EeeValidator.Validate(A<string>._, A<string>._, A<WeeeCategory>._, A<ObligationType>._, A<decimal>._))
                .Returns(new List<ErrorData> { new ErrorData("An Error", ErrorLevel.Error) });

            var builder = helper.Create();
            await builder.AddEeeOutputAmount("Registration Number", A<string>._, A<WeeeCategory>._, ObligationType.B2C, A<decimal>._);

            await builder.Build();

            A.CallTo(() => helper.DataAccess.GetRegisteredProducer(A<string>._))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task AddEeeOutputAmount_WithIfValidationErrorsandWarnings_CapturesErrorsAndWarnings()
        {
            var errorsAndWarnings = new List<ErrorData> { new ErrorData("An Error", ErrorLevel.Error), new ErrorData("A warning", ErrorLevel.Warning) };

            var helper = new DataReturnVersionBuilderHelper();

            A.CallTo(() => helper.EeeValidator.Validate(A<string>._, A<string>._, A<WeeeCategory>._, A<ObligationType>._, A<decimal>._))
                .Returns(errorsAndWarnings);

            var builder = helper.Create();
            await builder.AddEeeOutputAmount("Registration Number", A<string>._, A<WeeeCategory>._, ObligationType.B2C, A<decimal>._);

            var result = await builder.Build();

            Assert.Equal(2, result.ErrorData.Count);
            Assert.Contains(result.ErrorData, r => r.ErrorLevel == ErrorLevel.Warning);
            Assert.Contains(result.ErrorData, r => r.ErrorLevel == ErrorLevel.Error);
        }

        /// <summary>
        /// This test ensures that the error message list resulting from a DataReturnVersionBuilder Build() call does not contain
        /// duplicate errors in the case where the same validation error has occurred multiple times. This check was inserted because
        /// CheckProducerIsRegisteredWithSchemeForYear(prn) validation adds the same error for every EEE amount added for the invalid
        /// producer.
        /// </summary>
        [Fact]
        public async Task AddMultipleEeeOutputAmounts_WithDuplicateValidationErrorsAndWarnings_DoesNotOutputDuplicateErrorsOrWarnings()
        {
            String reg1Prn = "Registration Number 1";
            String reg2Prn = "Registration Number 2";

            var reg1ErrorsAndWarnings = new List<ErrorData>
            {
                new ErrorData("Error being compared", ErrorLevel.Error),
                new ErrorData("Warning being compared", ErrorLevel.Warning)
            };

            var reg2ErrorsAndWarnings = new List<ErrorData>
            {
                new ErrorData("Error being compared", ErrorLevel.Error),
                new ErrorData("Warning being compared", ErrorLevel.Warning)
            };

            var helper = new DataReturnVersionBuilderHelper();

            A.CallTo(() => helper.EeeValidator.Validate(reg1Prn, A<string>._, A<WeeeCategory>._, A<ObligationType>._, A<decimal>._))
                .Returns(reg1ErrorsAndWarnings);
            A.CallTo(() => helper.EeeValidator.Validate(reg2Prn, A<string>._, A<WeeeCategory>._, A<ObligationType>._, A<decimal>._))
                .Returns(reg2ErrorsAndWarnings);

            // Add five records, all contain the same error and warning
            var builder = helper.Create();
            await builder.AddEeeOutputAmount(reg1Prn, A<string>._, A<WeeeCategory>._, ObligationType.B2C, A<decimal>._);
            await builder.AddEeeOutputAmount(reg1Prn, A<string>._, A<WeeeCategory>._, ObligationType.B2B, A<decimal>._);
            await builder.AddEeeOutputAmount(reg1Prn, A<string>._, A<WeeeCategory>._, ObligationType.B2B, A<decimal>._);
            await builder.AddEeeOutputAmount(reg1Prn, A<string>._, A<WeeeCategory>._, ObligationType.B2B, A<decimal>._);
            await builder.AddEeeOutputAmount(reg2Prn, A<string>._, A<WeeeCategory>._, ObligationType.B2C, A<decimal>._);

            // Act
            var result = await builder.Build();

            // Assert
            // Only outputs one of each duplicate error/warning
            Assert.Equal(2, result.ErrorData.Count);
        }

        private class DataReturnVersionBuilderHelper
        {
            public readonly Scheme Scheme;

            public readonly Quarter Quarter;

            public readonly IDataReturnVersionBuilderDataAccess DataAccess;

            public readonly ISubmissionWindowClosed SubmissionWindowClosed;

            private readonly Func<Scheme, Quarter, IDataReturnVersionBuilderDataAccess> dataAccessDelegate;

            public readonly IEeeValidator EeeValidator;

            private readonly Func<Scheme, Quarter,
                Func<Scheme, Quarter, IDataReturnVersionBuilderDataAccess>, IEeeValidator> eeeValidatorDelegate;

            public DataReturnVersionBuilderHelper()
            {
                Scheme = A.Dummy<Scheme>();
                Quarter = A.Dummy<Quarter>();
                EeeValidator = A.Fake<IEeeValidator>();
                DataAccess = A.Fake<IDataReturnVersionBuilderDataAccess>();
                SubmissionWindowClosed = A.Fake<ISubmissionWindowClosed>();

                dataAccessDelegate = (x, y) => DataAccess;
                eeeValidatorDelegate = (s, q, z) => EeeValidator;
            }

            public DataReturnVersionBuilder Create()
            {
                return new DataReturnVersionBuilder(Scheme, Quarter, eeeValidatorDelegate, dataAccessDelegate, SubmissionWindowClosed);
            }

            public DataReturnVersionBuilder CreateWithErrorData(List<ErrorData> errorData)
            {
                return new DataReturnVersionBuilderExtension(Scheme, Quarter, eeeValidatorDelegate, dataAccessDelegate, SubmissionWindowClosed, errorData);
            }
        }

        private class DataReturnVersionBuilderExtension : DataReturnVersionBuilder
        {
            public DataReturnVersionBuilderExtension(Scheme scheme, Quarter quarter,
            Func<Scheme, Quarter, Func<Scheme, Quarter, IDataReturnVersionBuilderDataAccess>, IEeeValidator> eeeValidatorDelegate,
            Func<Scheme, Quarter, IDataReturnVersionBuilderDataAccess> dataAccessDelegate, ISubmissionWindowClosed submissionWindowClosed, List<ErrorData> errorData)
                : base(scheme, quarter, eeeValidatorDelegate, dataAccessDelegate, submissionWindowClosed)
            {
                Errors = errorData;
            }
        }
    }
}
