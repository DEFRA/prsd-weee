namespace EA.Weee.DataAccess.Tests.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Weee.Tests.Core.Model;
    using Xunit;

    public class ConstraintTests
    {
        /// <summary>
        /// IX_RegisteredProducer_Unique_SchemeId_ProducerRegistrationNumber_ComplianceYear_WhereNotRemoved
        /// 
        /// The test ensures the above unique index will prevent two producers being registered with the
        /// same scheme, compliance year and PRN if both registrations are marked as non-removed.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task InsertNonRemovedRegisteredProducer_WithExistingNonRemovedProducer_ThrowsDbUpdateException()
        {
            using (DatabaseWrapper wrapper = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(wrapper.Model);
                Scheme scheme = helper.CreateScheme();

                RegisteredProducer registeredProducer1 = new RegisteredProducer()
                {
                    Id = new Guid("834635D7-876D-47BD-B3E3-570E481B2C15"),
                    Scheme = scheme,
                    ComplianceYear = 2099,
                    ProducerRegistrationNumber = "WEE/HD6483TF",
                    Removed = false,
                };

                wrapper.Model.RegisteredProducers.Add(registeredProducer1);

                await wrapper.Model.SaveChangesAsync();

                // Act
                RegisteredProducer registeredProducer2 = new RegisteredProducer()
                {
                    Id = new Guid("65C1B132-2573-409C-B18E-E7C3AEC777AF"),
                    Scheme = scheme,
                    ComplianceYear = 2099,
                    ProducerRegistrationNumber = "WEE/HD6483TF",
                    Removed = false,
                };

                wrapper.Model.RegisteredProducers.Add(registeredProducer2);

                Func<Task> testCode = async () => await wrapper.Model.SaveChangesAsync();

                // Assert
                await Assert.ThrowsAsync<DbUpdateException>(testCode);
            }
        }

        /// <summary>
        /// IX_RegisteredProducer_Unique_SchemeId_ProducerRegistrationNumber_ComplianceYear_WhereNotRemoved
        /// 
        /// The test ensures the above unique index will not prevent two producers being registered with the
        /// same scheme, compliance year and PRN if the first is marked as removed.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task InsertNonRemovedRegisteredProducer_WithExistingRemovedProducer_DoesntThrowAnException()
        {
            using (DatabaseWrapper wrapper = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(wrapper.Model);
                Scheme scheme = helper.CreateScheme();

                RegisteredProducer registeredProducer1 = new RegisteredProducer()
                {
                    Id = new Guid("834635D7-876D-47BD-B3E3-570E481B2C15"),
                    Scheme = scheme,
                    ComplianceYear = 2099,
                    ProducerRegistrationNumber = "WEE/HD6483TF",
                    Removed = true,
                };

                wrapper.Model.RegisteredProducers.Add(registeredProducer1);

                await wrapper.Model.SaveChangesAsync();

                // Act
                RegisteredProducer registeredProducer2 = new RegisteredProducer()
                {
                    Id = new Guid("65C1B132-2573-409C-B18E-E7C3AEC777AF"),
                    Scheme = scheme,
                    ComplianceYear = 2099,
                    ProducerRegistrationNumber = "WEE/HD6483TF",
                    Removed = false,
                };

                wrapper.Model.RegisteredProducers.Add(registeredProducer2);

                await wrapper.Model.SaveChangesAsync();

                // Assert
                // No exception
            }
        }

        /// <summary>
        /// IX_RegisteredProducer_Unique_SchemeId_ProducerRegistrationNumber_ComplianceYear_WhereNotRemoved
        /// 
        /// The test ensures the above unique index will not prevent two producers being registered with the
        /// same scheme, compliance year and PRN if the both are marked as removed.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task UpdateRegisteredProducerAsRemoved_WithExistingRemovedProducer_DoesntThrowAnException()
        {
            using (DatabaseWrapper wrapper = new DatabaseWrapper())
            {
                // Arrange
                ModelHelper helper = new ModelHelper(wrapper.Model);
                Scheme scheme = helper.CreateScheme();

                RegisteredProducer registeredProducer1 = new RegisteredProducer()
                {
                    Id = new Guid("834635D7-876D-47BD-B3E3-570E481B2C15"),
                    Scheme = scheme,
                    ComplianceYear = 2099,
                    ProducerRegistrationNumber = "WEE/HD6483TF",
                    Removed = true,
                };

                wrapper.Model.RegisteredProducers.Add(registeredProducer1);

                RegisteredProducer registeredProducer2 = new RegisteredProducer()
                {
                    Id = new Guid("65C1B132-2573-409C-B18E-E7C3AEC777AF"),
                    Scheme = scheme,
                    ComplianceYear = 2099,
                    ProducerRegistrationNumber = "WEE/HD6483TF",
                    Removed = false,
                };

                wrapper.Model.RegisteredProducers.Add(registeredProducer2);

                await wrapper.Model.SaveChangesAsync();

                // Act
                registeredProducer2.Removed = true;
                await wrapper.Model.SaveChangesAsync();

                // Assert
                // No exception
            }
        }
    }
}
