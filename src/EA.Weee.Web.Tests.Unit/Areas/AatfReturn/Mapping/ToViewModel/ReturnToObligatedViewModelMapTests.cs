﻿namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Mapping.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.AatfReturn;
    using Core.Scheme;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Core.Shared;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Common;
    using Prsd.Core.Mapper;
    using Services.Caching;
    using Web.Areas.AatfReturn.Mappings.ToViewModel;
    using Xunit;

    public class ReturnToObligatedViewModelMapTests
    {
        private readonly ReturnToObligatedViewModelMap mapper;
        private readonly IWeeeCache cache;
        private readonly IMap<ObligatedDataToObligatedValueMapTransfer, IList<ObligatedCategoryValue>> categoryMap;
        private readonly ICategoryValueTotalCalculator calculator;
        private readonly IPasteProcessor pasteProcessor;

        public ReturnToObligatedViewModelMapTests()
        {
            cache = A.Fake<IWeeeCache>();
            categoryMap = A.Fake<IMap<ObligatedDataToObligatedValueMapTransfer, IList<ObligatedCategoryValue>>>();
            calculator = A.Fake<ICategoryValueTotalCalculator>();
            pasteProcessor = A.Fake<IPasteProcessor>();

            mapper = new ReturnToObligatedViewModelMap(cache, categoryMap, calculator, pasteProcessor);
        }

        [Fact]
        public void Map_GivenNullSource_ShouldThrowArgumentNullException()
        {
            Action call = () => mapper.Map(null);

            call.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenOrganisationId_CachedSchemeNameShouldBeSet()
        {
            const string expected = "scheme";
            var schemeData = A.Fake<SchemePublicInfo>();

            A.CallTo(() => schemeData.Name).Returns(expected);

            var transfer = new ReturnToObligatedViewModelTransfer() { OrganisationId = Guid.NewGuid() };

            A.CallTo(() => cache.FetchSchemePublicInfo(transfer.OrganisationId)).Returns(schemeData);

            var result = mapper.Map(transfer);

            result.SchemeName.Should().Be(expected);
        }

        [Fact]
        public void Map_GivenOrganisationAndAatfId_CachedAatfNameShouldBeSet()
        {
            const string expected = "aatf";
            var aatfData = A.Fake<AatfData>();

            A.CallTo(() => aatfData.Name).Returns(expected);

            var transfer = new ReturnToObligatedViewModelTransfer() { OrganisationId = Guid.NewGuid(), AatfId = Guid.NewGuid() };

            A.CallTo(() => cache.FetchAatfData(transfer.OrganisationId, transfer.AatfId)).Returns(aatfData);

            var result = mapper.Map(transfer);

            result.AatfName.Should().Be(expected);
        }

        [Fact]
        public void Map_GivenEntityIds_IdPropertiesShouldBeSet()
        {
            var transfer = new ReturnToObligatedViewModelTransfer() { OrganisationId = Guid.NewGuid(), AatfId = Guid.NewGuid(), ReturnId = Guid.NewGuid(), SchemeId = Guid.NewGuid() };

            var result = mapper.Map(transfer);

            result.AatfId.Should().Be(transfer.AatfId);
            result.OrganisationId.Should().Be(transfer.OrganisationId);
            result.ReturnId.Should().Be(transfer.ReturnId);
            result.SchemeId.Should().Be(transfer.SchemeId);
        }

        [Fact]
        public void Map_GivenPastedData_PasteProcessorShouldBeCalled()
        {
            var pastedList = new List<ObligatedCategoryValue>();
            for (var i = 0; i < pastedList.Count; i++)
            {
                pastedList[i].B2B = i.ToString();
                pastedList[i].B2C = i.ToString();
            }

            var transfer = new ReturnToObligatedViewModelTransfer() { AatfId = Guid.NewGuid(), SchemeId = Guid.NewGuid(), PastedData = A.Fake<ObligatedCategoryValue>() };
            var obligatedValues = new ObligatedCategoryValues();
            var returnList = new List<ObligatedCategoryValue>();

            A.CallTo(() => pasteProcessor.ParseObligatedPastedValues(A<ObligatedPastedValues>._, A<IList<ObligatedCategoryValue>>._)).Returns(pastedList);

            var result = mapper.Map(transfer);

            result.CategoryValues.Should().BeEquivalentTo(pastedList);
        }

        [Fact]
        public void Map_GivenObligatedAndCategoryValues_ObligatedMapperShouldBeCalled()
        {
            var transfer = new ReturnToObligatedViewModelTransfer() { AatfId = Guid.NewGuid(), SchemeId = Guid.NewGuid() };
            var obligatedValues = new ObligatedCategoryValues();
            var returnList = new List<ObligatedCategoryValue>();

            var weeeDataValues = new List<WeeeObligatedData>()
            {
                new WeeeObligatedData()
                {
                    Aatf = new AatfData(transfer.AatfId, A.Dummy<string>(), A.Dummy<string>()),
                    Scheme = new Scheme(transfer.SchemeId, A.Dummy<string>())
                },
                new WeeeObligatedData()
                {
                    Aatf = new AatfData(transfer.AatfId, A.Dummy<string>(), A.Dummy<string>()),
                    Scheme = new Scheme(transfer.SchemeId, A.Dummy<string>())
                },
                new WeeeObligatedData()
                {
                    Aatf = new AatfData(transfer.AatfId, A.Dummy<string>(), A.Dummy<string>()),
                    Scheme = new Scheme(Guid.NewGuid(), A.Dummy<string>())
                },
                new WeeeObligatedData()
                {
                    Aatf = new AatfData(Guid.NewGuid(), A.Dummy<string>(), A.Dummy<string>()),
                    Scheme = new Scheme(Guid.NewGuid(), A.Dummy<string>())
                },
            };

            A.CallTo(() => categoryMap.Map(A<ObligatedDataToObligatedValueMapTransfer>.That.Matches(
                c => c.WeeeDataValues.IsSameOrEqualTo(weeeDataValues.Where(w => w.Aatf.Id == transfer.AatfId && w.Scheme.Id == transfer.SchemeId)) &&
                     c.ObligatedCategoryValues.IsSameOrEqualTo(obligatedValues)))).Returns(returnList);

            var result = mapper.Map(transfer);

            result.CategoryValues.Should().BeEquivalentTo(returnList);
        }
    }
}
