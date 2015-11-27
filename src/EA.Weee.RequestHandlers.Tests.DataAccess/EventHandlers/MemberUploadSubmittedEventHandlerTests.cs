namespace EA.Weee.RequestHandlers.Tests.DataAccess.EventHandlers
{
    using EA.Weee.DataAccess.EventHandlers;
    using EA.Weee.Domain.Events;
    using EA.Weee.Tests.Core.Model;
    using System;
    using System.Linq;
    using Xunit;

    public class MemberUploadSubmittedEventHandlerTests
    {
        [Fact]
        public async void HandleAsync_NewProducer_IsCurrentForComplianceYearTrue()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                int complianceYear = 2016;
                string registrationNumber = "WEE/11AAAA11";

                ProducerHelper helper = new ProducerHelper(db);

                // New DB record
                var memberUpload = helper.AddMemberUpload(complianceYear);
                var producerId = helper.AddProducer(memberUpload, registrationNumber).Id;

                db.Model.SaveChanges();

                var newMemberUpload = helper.FindMemberUpload(memberUpload.Id);

                MemberUploadSubmittedEventHandler handler = new MemberUploadSubmittedEventHandler(db.WeeeContext);
                await handler.HandleAsync(new MemberUploadSubmittedEvent(newMemberUpload));

                var producer1DbRecord = helper.FindProducer(producerId);

                Assert.True(producer1DbRecord.IsCurrentForComplianceYear);
            }
        }

        [Fact]
        public async void HandleAsync_UpdateProducer_IsCurrentForComplianceYear_TrueForNewProducer()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                int complianceYear = 2016;
                string registrationNumber = "WEE/11AAAA11";

                ProducerHelper helper = new ProducerHelper(db);

                // Existing DB records
                var memberUpload1 = helper.AddMemberUpload(complianceYear);
                var producer1Id = helper.AddProducer(memberUpload1, registrationNumber, isCurrentForComplianceYear: true).Id;

                db.Model.SaveChanges();

                // New DB record
                var memberUpload2 = helper.AddMemberUpload(complianceYear);
                var newProducer1 = helper.AddProducer(memberUpload2, registrationNumber);
                newProducer1.UpdatedDate = DateTime.Now;
                db.Model.SaveChanges();

                var newMemberUpload = helper.FindMemberUpload(memberUpload2.Id);

                MemberUploadSubmittedEventHandler handler = new MemberUploadSubmittedEventHandler(db.WeeeContext);
                await handler.HandleAsync(new MemberUploadSubmittedEvent(newMemberUpload));

                var producer1DbRecord = helper.FindProducer(producer1Id);
                var newProducer1DbRecord = helper.FindProducer(newProducer1.Id);

                Assert.False(producer1DbRecord.IsCurrentForComplianceYear);
                Assert.True(newProducer1DbRecord.IsCurrentForComplianceYear);
            }
        }

        [Fact]
        public async void HandleAsync_UpdateProducerSameScheme_IsCurrentForComplianceYearOtherProducersRemainsUnchanged()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                int complianceYear = 2016;
                string registrationNumber1 = "WEE/11AAAA11";
                string registrationNumber2 = "WEE/11AAAA12";

                ProducerHelper helper = new ProducerHelper(db);

                // Existing DB records
                var memberUpload1 = helper.AddMemberUpload(complianceYear);
                var producer1Id = helper.AddProducer(memberUpload1, registrationNumber1, isCurrentForComplianceYear: true).Id;
                var producer2Id = helper.AddProducer(memberUpload1, registrationNumber2, isCurrentForComplianceYear: true).Id;

                db.Model.SaveChanges();

                // New DB record
                var memberUpload2 = helper.AddMemberUpload(complianceYear);
                var newProducer1 = helper.AddProducer(memberUpload2, registrationNumber1);
                newProducer1.UpdatedDate = DateTime.Now;
                db.Model.SaveChanges();

                var newMemberUpload = helper.FindMemberUpload(memberUpload2.Id);

                MemberUploadSubmittedEventHandler handler = new MemberUploadSubmittedEventHandler(db.WeeeContext);
                await handler.HandleAsync(new MemberUploadSubmittedEvent(newMemberUpload));

                var producer1DbRecord = helper.FindProducer(producer1Id);
                var producer2DbRecord = helper.FindProducer(producer2Id);
                var newProducer1DbRecord = helper.FindProducer(newProducer1.Id);

                Assert.False(producer1DbRecord.IsCurrentForComplianceYear);
                Assert.True(producer2DbRecord.IsCurrentForComplianceYear);
                Assert.True(newProducer1DbRecord.IsCurrentForComplianceYear);
            }
        }

        [Fact]
        public async void HandleAsync_UpdateProducerSameScheme_DifferentComplianceYear_IsCurrentForComplianceYearRemainsUnchanged()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                int complianceYear1 = 2016;
                int complianceYear2 = 2017;
                string registrationNumber = "WEE/11AAAA11";

                ProducerHelper helper = new ProducerHelper(db);

                // Existing DB records
                var memberUpload1 = helper.AddMemberUpload(complianceYear1);
                var producer1Id = helper.AddProducer(memberUpload1, registrationNumber, isCurrentForComplianceYear: true).Id;

                db.Model.SaveChanges();

                // New DB record
                var memberUpload2 = helper.AddMemberUpload(complianceYear2);
                var newProducer1 = helper.AddProducer(memberUpload2, registrationNumber);
                newProducer1.UpdatedDate = DateTime.Now;
                db.Model.SaveChanges();

                var newMemberUpload = helper.FindMemberUpload(memberUpload2.Id);

                MemberUploadSubmittedEventHandler handler = new MemberUploadSubmittedEventHandler(db.WeeeContext);
                await handler.HandleAsync(new MemberUploadSubmittedEvent(newMemberUpload));

                var producer1DbRecord = helper.FindProducer(producer1Id);
                var newProducer1DbRecord = helper.FindProducer(newProducer1.Id);

                Assert.True(producer1DbRecord.IsCurrentForComplianceYear);
                Assert.True(newProducer1DbRecord.IsCurrentForComplianceYear);
            }
        }

        [Fact]
        public async void HandleAsync_UpdateProducerDifferentScheme_IsCurrentForComplianceYearRemainsUnchanged()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                int complianceYear = 2016;
                string registrationNumber = "WEE/11AAAA11";

                ProducerHelper helper = new ProducerHelper(db);

                // Existing DB records
                var memberUpload1 = helper.AddMemberUpload(complianceYear);
                var producer1Id = helper.AddProducer(memberUpload1, registrationNumber, 1, true).Id;

                db.Model.SaveChanges();

                // New DB record
                helper.NewScheme();
                var memberUpload2 = helper.AddMemberUpload(complianceYear);
                // Same producer in another scheme requires a different obligation type
                var newProducer1 = helper.AddProducer(memberUpload2, registrationNumber, 2);
                newProducer1.UpdatedDate = DateTime.Now;
                db.Model.SaveChanges();

                var newMemberUpload = helper.FindMemberUpload(memberUpload2.Id);

                MemberUploadSubmittedEventHandler handler = new MemberUploadSubmittedEventHandler(db.WeeeContext);
                await handler.HandleAsync(new MemberUploadSubmittedEvent(newMemberUpload));

                var producer1DbRecord = helper.FindProducer(producer1Id);
                var newProducer1DbRecord = helper.FindProducer(newProducer1.Id);

                Assert.True(producer1DbRecord.IsCurrentForComplianceYear);
                Assert.True(newProducer1DbRecord.IsCurrentForComplianceYear);
            }
        }

        private class ProducerHelper
        {
            private DatabaseWrapper dbWrapper;
            private ModelHelper helper;
            public Scheme Scheme { get; private set; }

            public ProducerHelper(DatabaseWrapper dbWrapper)
            {
                this.dbWrapper = dbWrapper;
                helper = new ModelHelper(dbWrapper.Model);
                Scheme = helper.CreateScheme();
            }

            public void NewScheme()
            {
                Scheme = helper.CreateScheme();
            }

            public MemberUpload AddMemberUpload(int complianceYear)
            {
                MemberUpload memberUpload = helper.CreateMemberUpload(Scheme);
                memberUpload.ComplianceYear = complianceYear;
                memberUpload.IsSubmitted = false;

                return memberUpload;
            }

            public Producer AddProducer(MemberUpload memberUpload, string registrationNumber, int obligationType = 1, bool isCurrentForComplianceYear = false)
            {
                Producer producer = helper.CreateProducerAsCompany(memberUpload, registrationNumber);
                producer.ObligationType = obligationType;
                producer.IsCurrentForComplianceYear = isCurrentForComplianceYear;

                return producer;
            }

            public Domain.Producer.Producer FindProducer(Guid producerId)
            {
                return dbWrapper.WeeeContext.Producers.First(p => p.Id == producerId);
            }

            public Domain.Scheme.MemberUpload FindMemberUpload(Guid memberUploadId)
            {
                return dbWrapper.WeeeContext.MemberUploads.First(m => m.Id == memberUploadId);
            }
        }
    }
}
