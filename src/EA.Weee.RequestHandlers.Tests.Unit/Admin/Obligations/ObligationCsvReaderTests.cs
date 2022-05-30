namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Obligations
{
    using System.IO;
    using AutoFixture;
    using Core.Shared;
    using Core.Shared.CsvReading;
    using FakeItEasy;
    using RequestHandlers.Admin.Obligations;
    using Xunit;

    public class ObligationsCsvReaderTests
    {
        private readonly ObligationCsvReader reader;
        private readonly IFileHelper fileHelper;
        private readonly Fixture fixture;
        private readonly byte[] data;
        private readonly IWeeeCsvReader csvReader;

        public ObligationsCsvReaderTests()
        {
            fileHelper = A.Fake<IFileHelper>();
            csvReader = A.Fake<IWeeeCsvReader>();
            fixture = new Fixture();
            data = fixture.Create<byte[]>();

            reader = new ObligationCsvReader(fileHelper);
        }

        [Fact]
        public void ValidateHeader_GivenData_GetStreamReaderShouldBeCalled()
        {
            //act
            reader.Read(data);

            //assert
            A.CallTo(() => fileHelper.GetStreamReader(data)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void ValidateHeader_GivenData_GetCsvReaderShouldBeCalled()
        {
            //arrange
            var stream = new StreamReader(new MemoryStream());
            A.CallTo(() => fileHelper.GetStreamReader(A<byte[]>._)).Returns(stream);

            //act
            reader.Read(data);

            //assert
            A.CallTo(() => fileHelper.GetCsvReader(stream)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void ValidateHeader_GivenData_RegisterClassMapShouldBeCalled()
        {
            //arrange
            A.CallTo(() => fileHelper.GetCsvReader(A<StreamReader>._)).Returns(csvReader);

            //act
            reader.Read(data);

            //assert
            A.CallTo(() => csvReader.RegisterClassMap<ObligationUploadClassMap>()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void ValidateHeader_GivenData_ReadHeaderShouldBeCalled()
        {
            //arrange
            A.CallTo(() => fileHelper.GetCsvReader(A<StreamReader>._)).Returns(csvReader);

            //act
            reader.Read(data);

            //assert
            A.CallTo(() => csvReader.ReadHeader()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void ValidateHeader_GivenData_ValidateHeaderShouldBeCalled()
        {
            //arrange
            A.CallTo(() => fileHelper.GetCsvReader(A<StreamReader>._)).Returns(csvReader);

            //act
            reader.Read(data);

            //assert
            A.CallTo(() => csvReader.ValidateHeader<ObligationCsvUpload>()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void ValidateHeader_GivenRegisterClassMapThrowsCsvValidationException_ErrorShouldBeReturned()
        {
        }
    }
}
