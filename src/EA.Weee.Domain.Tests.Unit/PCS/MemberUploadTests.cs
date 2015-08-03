namespace EA.Weee.Domain.Tests.Unit.PCS
{
    using System;
    using System.Collections.Generic;
    using Domain.PCS;
    using Xunit;

    public class MemberUploadTests
    {
        private const string orgGuid = "2AE69682-E9D8-4AC5-991D-A4CF00C42F14";

        [Fact]
        public void MemberUploadSubmission_MemberUploadNotSubmitted_ReturnsSubmittedMemberUpload()
        {
            var memberUpload = TestMemberUpload();
            memberUpload.Submit();

            Assert.True(memberUpload.IsSubmitted);
        }

        [Fact]
        public void MemberUploadSubmission_MemberUploadAlreadySubmitted_ThrowInvalidOperationException()
        {
            var memberUpload = TestMemberUpload();
            memberUpload.Submit();

            Assert.Throws<InvalidOperationException>(() => memberUpload.Submit());
        }

        private MemberUpload TestMemberUpload()
        {
            return new MemberUpload(new Guid(orgGuid), "Test data", new List<MemberUploadError>(), 0);
        }
    }
}
