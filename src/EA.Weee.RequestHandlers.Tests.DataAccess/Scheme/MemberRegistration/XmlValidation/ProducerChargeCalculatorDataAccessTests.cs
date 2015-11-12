namespace EA.Weee.RequestHandlers.Tests.DataAccess.Scheme.MemberRegistration.XmlValidation
{
    using Domain.Lookup;
    using EA.Weee.Domain;
    using EA.Weee.RequestHandlers.Scheme.MemberRegistration;
    using EA.Weee.Tests.Core.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class ProducerChargeCalculatorDataAccessTests
    {
        /// <summary>
        /// This test ensures that the charge band amount can be fetched from the database by type.
        /// </summary>
        [Fact]
        public void FetchCurrentChargeBandAmount_WithChargeBandTypeA_ReturnsChargeBandAmountWithTypeA()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ProducerChargeCalculatorDataAccess dataAccess = new ProducerChargeCalculatorDataAccess(database.WeeeContext);

                // Act
                var result = dataAccess.FetchCurrentChargeBandAmount(ChargeBand.A);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(ChargeBand.A, result.ChargeBand);
            }
        }

        /// <summary>
        /// This test ensures that the sum of existing charges will be returned as zero where the
        /// producer has no existing charges.
        /// </summary>
        [Fact]
        public void FetchSumOfExistingCharges_WithUnknownProducer_ReturnsZero()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ProducerChargeCalculatorDataAccess dataAccess = new ProducerChargeCalculatorDataAccess(database.WeeeContext);

                // Act
                decimal result = dataAccess.FetchSumOfExistingCharges("XXXXXXX", 2016);

                // Assert
                Assert.Equal(0, result);
            }
        }

        /// <summary>
        /// This test ensures that the sum of existing charges will be returned as zero where the
        /// producer has only charges in other compliance years than the one specified.
        /// </summary>
        [Fact]
        public void FetchSumOfExistingCharges_WithProducerWithNoChargesInSpecifiedYear_ReturnsZero()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(database.Model);

                Scheme scheme = helper.CreateScheme();

                MemberUpload memberUpload = helper.CreateMemberUpload(scheme);
                memberUpload.ComplianceYear = 2015;
                memberUpload.IsSubmitted = true;

                Producer producer = helper.CreateProducerAsCompany(memberUpload, "AAAAA");
                producer.IsCurrentForComplianceYear = true;
                producer.ChargeThisUpdate = 1;

                database.Model.SaveChanges();

                ProducerChargeCalculatorDataAccess dataAccess = new ProducerChargeCalculatorDataAccess(database.WeeeContext);

                // Act
                decimal result = dataAccess.FetchSumOfExistingCharges("AAAAA", 2016);

                // Assert
                Assert.Equal(0, result);
            }
        }

        /// <summary>
        /// This test ensures that the sum of existing charges only considers charges from the specified
        /// compliance year.
        /// </summary>
        [Fact]
        public void FetchSumOfExistingCharges_WithProducerWithChargesInDifferentYears_ReturnsOnlyChargeAmountForSpecifiedYear()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(database.Model);

                Scheme scheme = helper.CreateScheme();

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme);
                memberUpload1.ComplianceYear = 2015;
                memberUpload1.IsSubmitted = true;

                Producer producer1 = helper.CreateProducerAsCompany(memberUpload1, "AAAAA");
                producer1.IsCurrentForComplianceYear = true;
                producer1.ChargeThisUpdate = 1;

                MemberUpload memberUpload2 = helper.CreateMemberUpload(scheme);
                memberUpload2.ComplianceYear = 2016;
                memberUpload2.IsSubmitted = true;

                Producer producer2 = helper.CreateProducerAsCompany(memberUpload2, "AAAAA");
                producer2.IsCurrentForComplianceYear = true;
                producer2.ChargeThisUpdate = 2;

                database.Model.SaveChanges();

                ProducerChargeCalculatorDataAccess dataAccess = new ProducerChargeCalculatorDataAccess(database.WeeeContext);

                // Act
                decimal result = dataAccess.FetchSumOfExistingCharges("AAAAA", 2016);

                // Assert
                Assert.Equal(2, result);
            }
        }

        /// <summary>
        /// This test ensures that the sum of existing charges will be correctly calculated 
        /// </summary>
        [Fact]
        public void FetchSumOfExistingCharges_WithProducerWithSeveralCharges_ReturnsSumOfChargeAmounts()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(database.Model);

                Scheme scheme = helper.CreateScheme();

                MemberUpload memberUpload1 = helper.CreateMemberUpload(scheme);
                memberUpload1.ComplianceYear = 2016;
                memberUpload1.IsSubmitted = true;

                Producer producer1 = helper.CreateProducerAsCompany(memberUpload1, "AAAAA");
                producer1.IsCurrentForComplianceYear = false;
                producer1.ChargeThisUpdate = 1;

                MemberUpload memberUpload2 = helper.CreateMemberUpload(scheme);
                memberUpload2.ComplianceYear = 2016;
                memberUpload2.IsSubmitted = true;

                Producer producer2 = helper.CreateProducerAsCompany(memberUpload2, "AAAAA");
                producer2.IsCurrentForComplianceYear = false;
                producer2.ChargeThisUpdate = 2;

                MemberUpload memberUpload3 = helper.CreateMemberUpload(scheme);
                memberUpload3.ComplianceYear = 2016;
                memberUpload3.IsSubmitted = true;

                Producer producer3 = helper.CreateProducerAsCompany(memberUpload3, "AAAAA");
                producer3.IsCurrentForComplianceYear = true;
                producer3.ChargeThisUpdate = 3;

                database.Model.SaveChanges();

                ProducerChargeCalculatorDataAccess dataAccess = new ProducerChargeCalculatorDataAccess(database.WeeeContext);

                // Act
                decimal result = dataAccess.FetchSumOfExistingCharges("AAAAA", 2016);

                // Assert
                Assert.Equal(6, result);
            }
        }
    }
}
