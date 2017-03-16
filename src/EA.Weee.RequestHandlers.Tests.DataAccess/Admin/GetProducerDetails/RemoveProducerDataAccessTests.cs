namespace EA.Weee.RequestHandlers.Tests.DataAccess.Admin.GetProducerDetails
{
    using System.Linq;
    using System.Threading.Tasks;
    using RequestHandlers.Admin.GetProducerDetails;
    using Weee.DataAccess.DataAccess;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class RemoveProducerDataAccessTests
    {
        [Fact]
        public async Task GetProducerSubmission_WithUnsubmittedProducerSubmission_AndSubmittedAndInvoicedSubmision_ReturnsOnlySubmittedProducerSubmission()
        {
            using (DatabaseWrapper databaseWrapper = new DatabaseWrapper())
            {
                //Arrange
                ModelHelper modelHelper = new ModelHelper(databaseWrapper.Model);

                int complianceYear = 2000;
                string registeredProducerNumber = "WEE/XX4567YY";

                var scheme = modelHelper.CreateScheme();

                // Create the registered producer to be removed
                var registeredProducer = modelHelper.GetOrCreateRegisteredProducer(scheme, complianceYear, registeredProducerNumber);

                // Create and upload that has not been submitted, but contains the registered producer
                var firstMemberUpload = modelHelper.CreateMemberUpload(scheme);
                firstMemberUpload.IsSubmitted = false;
                firstMemberUpload.ComplianceYear = complianceYear;
                var producerSubmission1 = modelHelper.CreateProducerAsCompany(firstMemberUpload, registeredProducerNumber);
                producerSubmission1.RegisteredProducer = registeredProducer;
                producerSubmission1.ChargeThisUpdate = 10;

                //Create the second upload that has been submitted and invoiced and contains the registered producer
                var invoiceRun = modelHelper.CreateInvoiceRun();
                var secondMemberUpload = modelHelper.CreateSubmittedMemberUpload(scheme);
                secondMemberUpload.ComplianceYear = complianceYear;
                secondMemberUpload.InvoiceRun = invoiceRun;
                var producerSubmission2 = modelHelper.CreateProducerAsCompany(secondMemberUpload, registeredProducerNumber);
                producerSubmission2.ChargeThisUpdate = 10;
                producerSubmission2.Invoiced = true;
                producerSubmission2.RegisteredProducer = registeredProducer;
                registeredProducer.CurrentSubmission = producerSubmission2;

                await databaseWrapper.Model.SaveChangesAsync();

                var producerId = databaseWrapper.WeeeContext.RegisteredProducers
                    .Single(p => p.ProducerRegistrationNumber == registeredProducerNumber)
                    .Id;

                RegisteredProducerDataAccess registeredProdDA = new RegisteredProducerDataAccess(databaseWrapper.WeeeContext);
                RemoveProducerDataAccess removeProdDA = new RemoveProducerDataAccess(registeredProdDA, databaseWrapper.WeeeContext);

                //Act
                var prodSubs = await removeProdDA.GetProducerSubmissionsForRegisteredProducer(producerId);

                //Assert
                Assert.NotNull(prodSubs);
                Assert.Equal(1, prodSubs.Count());
                Assert.Equal(true, prodSubs.First().MemberUpload.IsSubmitted);
            }
        }
    }
}
