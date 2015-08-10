namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Api.Client;
    using Core.Scheme;
    using Core.Scheme;
    using Core.Scheme.MemberUploadTesting;
    using Core.Shared;
    using FakeItEasy;
    using Prsd.Core.Mediator;
    using Services;
    using TestHelpers;
    using Web.Areas.Scheme.Controllers;
    using Web.Areas.Scheme.ViewModels;
    using Weee.Requests.Organisations;
    using Weee.Requests.Scheme.MemberRegistration;
    using Xunit;

    public class MemberRegistrationControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly IFileConverterService fileConverter;

        public MemberRegistrationControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            fileConverter = A.Fake<IFileConverterService>();
        }

        private MemberRegistrationController MemberRegistrationController()
        {
            var controller = new MemberRegistrationController(() => weeeClient, fileConverter);
            new HttpContextMocker().AttachToController(controller);

            return controller;
        }

        [Fact]
        public async void GetAddOrAmendMembers_ChecksForValidityOfOrganisation()
        {
            try
            {
                await MemberRegistrationController().AddOrAmendMembers(A<Guid>._);
            }
            catch (Exception)
            {
            }

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void GetAddOrAmendMembers_IdDoesNotBelongToAnExistingOrganisation_ThrowsException()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(false);

            await Assert.ThrowsAnyAsync<Exception>(() => MemberRegistrationController().AddOrAmendMembers(A<Guid>._));
        }

        [Fact]
        public async void GetAddOrAmendMembers_IdDoesBelongToAnExistingOrganisation_ReturnsView()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(true);

            var result = await MemberRegistrationController().AddOrAmendMembers(A<Guid>._);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void PostAddOrAmendMembers_ModelIsInvalid_ReturnsView()
        {
            var controller = MemberRegistrationController();
            controller.ModelState.AddModelError("ErrorKey", "Some kind of error goes here");

            var result = await controller.AddOrAmendMembers(A<Guid>._, new AddOrAmendMembersViewModel());

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void PostAddOrAmendMembers_ConvertsFileToString()
        {
            try
            {
                await MemberRegistrationController().AddOrAmendMembers(A<Guid>._, new AddOrAmendMembersViewModel());
            }
            catch (Exception)
            {
            }

            A.CallTo(() => fileConverter.Convert(A<HttpPostedFileBase>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void PostAddOrAmendMembers_FileIsConvertedSuccessfully_ValidateRequestSentWithConvertedFileDataAndOrganisationId()
        {
            var fileData = new byte[1];
            var organisationId = Guid.NewGuid();
            var request = new ProcessXMLFile(A<Guid>._, A<byte[]>._);

            A.CallTo(() => fileConverter.Convert(A<HttpPostedFileBase>._))
                .Returns(fileData);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<ProcessXMLFile>._))
                .Invokes((string token, IRequest<Guid> req) => request = (ProcessXMLFile)req);

            try
            {
                await MemberRegistrationController().AddOrAmendMembers(organisationId, new AddOrAmendMembersViewModel());
            }
            catch (Exception)
            {
            }

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<ProcessXMLFile>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            Assert.NotNull(request);
            Assert.Equal(fileData, request.Data);
            Assert.Equal(organisationId, request.OrganisationId);
        }

        [Fact]
        public async void PostAddOrAmendMembers_ValidateRequestIsProcessedSuccessfully_RedirectsToResults()
        {
            var validationId = Guid.NewGuid();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<ProcessXMLFile>._))
                .Returns(validationId);

            var result = await MemberRegistrationController().AddOrAmendMembers(A<Guid>._, new AddOrAmendMembersViewModel());
            var redirect = (RedirectToRouteResult)result;

            Assert.Equal("ViewErrorsAndWarnings", redirect.RouteValues["action"]);
            Assert.Equal("MemberRegistration", redirect.RouteValues["controller"]);
            Assert.Equal(validationId, redirect.RouteValues["memberUploadId"]);
        }

        private const string XmlHasErrorsViewName = "ViewErrorsAndWarnings";
        private const string XmlHasNoErrorsViewName = "XmlHasNoErrors";

        [Fact]
        public async Task GetViewErrorsOrWarnings_NoErrors_ShowsAcceptedPage()
        {
            Assert.Equal(XmlHasNoErrorsViewName, await ViewAfterClientReturns(new List<MemberUploadErrorData> { }));
        }

        [Fact]
        public async Task GetViewErrorsOrWarnings_ErrorsPresent_ShowsErrorPage()
        {
            Assert.Equal(XmlHasErrorsViewName, await ViewAfterClientReturns(new List<MemberUploadErrorData> { new MemberUploadErrorData { ErrorLevel = ErrorLevel.Error } }));
        }

        [Fact]
        public async Task GetViewErrorsOrWarnings_WarningPresent_ShowsAcceptedPage()
        {
            Assert.Equal(XmlHasNoErrorsViewName, await ViewAfterClientReturns(new List<MemberUploadErrorData> { new MemberUploadErrorData { ErrorLevel = ErrorLevel.Warning } }));
        }

        [Fact]
        public async Task GetViewErrorsOrWarnings_NoErrors_HasProvidedErrorData()
        {
            var errors = new List<MemberUploadErrorData>();

            var providedErrors = await ErrorsAfterClientReturns(errors);

            Assert.Equal(errors, providedErrors);
        }

        [Fact]
        public async Task GetViewErrorsOrWarnings_ErrorsPresent_HasProvidedErrorData()
        {
            var errors = new List<MemberUploadErrorData>
            {
                new MemberUploadErrorData
                {
                    ErrorLevel = ErrorLevel.Error
                }
            };

            var providedErrors = await ErrorsAfterClientReturns(errors);

            Assert.Equal(errors, providedErrors);
        }

        [Fact]
        public async void GetProducerCSV_ValidMemberUploadId_ReturnsCSVFile()
        {
            var testCSVData = new ProducerCSVFileData { FileContent = "Test, Test, Test", FileName = "test.csv" };
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetProducerCSVByMemberUploadId>._))
                .Returns(testCSVData);

            var result = await MemberRegistrationController().GetProducerCSV(A<Guid>._);

            Assert.IsType<FileContentResult>(result);
        }

        [Fact]
        public async void PostSubmitXml_ValidMemberUploadId_ReturnsSuccessfulSubmissionView()
        {
            var memberUploadId = Guid.NewGuid();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<MemberUploadSubmission>._))
                .Returns(memberUploadId);

            var result = await MemberRegistrationController().SubmitXml(A<Guid>._, new MemberUploadResultViewModel { ErrorData = new List<MemberUploadErrorData>(), MemberUploadId = memberUploadId });

            var redirect = (RedirectToRouteResult)result;

            Assert.Equal("SuccessfulSubmission", redirect.RouteValues["action"]);
            Assert.Equal(memberUploadId, redirect.RouteValues["memberUploadId"]);
        }

        [Fact]
        public async void GetEditScheme_ModelWithNoError_ReturnsView()
        {
            var controller = MemberRegistrationController();

            var viewResult = await controller.EditScheme();

            Assert.Equal("EditScheme", ((ViewResult)viewResult).ViewName);
        }

        [Fact]
        public async void PostEditScheme_ModelWithError_ReturnsViewWithError()
        {
            var controller = MemberRegistrationController();
            controller.ModelState.AddModelError("ErrorKey", "Some kind of error goes here");
            var viewResult = await controller.EditScheme(new SchemeViewModel());

            Assert.Equal("EditScheme", ((ViewResult)viewResult).ViewName);
            Assert.False(controller.ModelState.IsValid);
        }

        [Theory]
        [InlineData("Wee/AB1234CD/SCH")]
        [InlineData("WEE/AB1234CD/sch")]
        [InlineData("WEE/AB1234CD/123")]
        [InlineData("WEE/891234CD/SCH")]
        [InlineData("WEE/AB1DF4CD/SCH")]
        [InlineData("WEE/AB123482/SCH")]
        public async void PostEditScheme_ModelWithInCorrectApprovalNumber_ReturnsViewWithError(string approvalNumber)
        {
            var controller = MemberRegistrationController();
            var model = new SchemeViewModel
            {
                ApprovalNumber = approvalNumber,
                CompetentAuthorities = new List<UKCompetentAuthorityData>(),
                CompetentAuthorityId = new Guid(),
                CompetentAuthorityName = "Any name",
                IbisCustomerReference = "Any value",
                ObligationType = ObligationType.B2B,
                ObligationTypeSelectList = new List<SelectListItem>(),
                SchemeName = "Any value"
            };

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            var isModelStateValid = Validator.TryValidateObject(model, context, results, true);

            var viewResult = await controller.EditScheme(model);

            Assert.Equal("EditScheme", ((ViewResult)viewResult).ViewName);
            Assert.False(isModelStateValid);
        }

        [Theory]
        [InlineData("WEE/AB1234CD/SCH")]
        [InlineData("WEE/DE8562FG/SCH")]
        public async void PostEditScheme_ModelWithCorrectApprovalNumber_ReturnsView(string approvalNumber)
        {
            var controller = MemberRegistrationController();
            var model = new SchemeViewModel
            {
                ApprovalNumber = approvalNumber,
                CompetentAuthorities = new List<UKCompetentAuthorityData>(),
                CompetentAuthorityId = new Guid(),
                CompetentAuthorityName = "Any name",
                IbisCustomerReference = "Any value",
                ObligationType = ObligationType.B2B,
                ObligationTypeSelectList = new List<SelectListItem>(),
                SchemeName = "Any value"
            };

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            var isModelStateValid = Validator.TryValidateObject(model, context, results, true);

            var viewResult = await controller.EditScheme(model);

            Assert.Equal("EditScheme", ((ViewResult)viewResult).ViewName);
            Assert.True(isModelStateValid);
        }

        private async Task<List<MemberUploadErrorData>> ErrorsAfterClientReturns(List<MemberUploadErrorData> memberUploadErrorDatas)
        {
            var result = await GetViewResult(memberUploadErrorDatas);

            return ((MemberUploadResultViewModel)result.Model).ErrorData;
        }

        private async Task<string> ViewAfterClientReturns(List<MemberUploadErrorData> list)
        {
            return (await GetViewResult(list)).ViewName;
        }

        private async Task<ViewResult> GetViewResult(List<MemberUploadErrorData> memberUploadErrorDatas)
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetMemberUploadData>._))
                .Returns(memberUploadErrorDatas);

            return await MemberRegistrationController().ViewErrorsAndWarnings(A<Guid>._, A<Guid>._);
        }
    }
}
