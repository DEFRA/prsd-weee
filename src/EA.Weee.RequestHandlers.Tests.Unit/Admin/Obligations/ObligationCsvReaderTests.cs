namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Obligations
{
    using System.IO;
    using System.Linq;
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
        private readonly byte[] data;
        private readonly IWeeeCsvReader csvReader;

        public ObligationsCsvReaderTests()
        {
            fileHelper = A.Fake<IFileHelper>();
            csvReader = A.Fake<IWeeeCsvReader>();
            var fixture = new Fixture();
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
            A.CallTo(() => csvReader.ValidateHeader<ObligationCsvUpload>(16,
                A<string[]>.That.Matches(a => a.SequenceEqual(new[] 
                    { 
                    "Scheme Identifier", "Scheme Name", "Cat1 (t)", "Cat2 (t)", "Cat3 (t)", "Cat4 (t)", "Cat5 (t)", "Cat6 (t)", "Cat7 (t)", "Cat8 (t)",
                    "Cat9 (t)", "Cat10 (t)", "Cat11 (t)", "Cat12 (t)", "Cat13 (t)", "Cat14 (t)"
                })))).MustHaveHappenedOnceExactly();
        }
    }
}
