namespace EA.Weee.Domain.Tests.Unit.Scheme
{
    using System;
    using System.Collections.Generic;
    using Domain.Scheme;
    using Xunit;

    public class MemberUploadTests
    {
        private const string orgGuid = "2AE69682-E9D8-4AC5-991D-A4CF00C42F14";

        [Fact]
        public void MemberUploadSubmission_MemberUploadNotSubmitted_ReturnsSubmittedMemberUpload()
        {
            var memberUpload = new MemberUpload(new Guid(orgGuid), "Test data", new List<MemberUploadError>(), 0, 2016, Guid.NewGuid(), "File name");
            memberUpload.Submit();

            Assert.True(memberUpload.IsSubmitted);
        }

        [Fact]
        public void MemberUploadSubmission_MemberUploadAlreadySubmitted_ThrowInvalidOperationException()
        {
            var memberUpload = new MemberUpload(new Guid(orgGuid), "Test data", new List<MemberUploadError>(), 0, 2016, Guid.NewGuid(), "File name");
            memberUpload.Submit();

            Assert.Throws<InvalidOperationException>(() => memberUpload.Submit());
        }

        [Fact]
        public void MemberUpload_SetProcessTimeMoreThanOnce_ThrowInvalidOperationException()
        {
            var memberUpload = new MemberUpload(new Guid(orgGuid), "Test data", new List<MemberUploadError>(), 0, 2016, Guid.NewGuid(), "File name");
            memberUpload.SetProcessTime(new TimeSpan(1));

            Assert.Throws<InvalidOperationException>(() => memberUpload.SetProcessTime(new TimeSpan(2)));
        }
    }
}
