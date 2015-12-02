namespace EA.Weee.Tests.Core.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Domain;
    using Domain.Lookup;

    /// <summary>
    /// This class provides helper methods for deterministically seeding a database.
    /// It should be used during the "arrange" step of data access integration tests.
    /// All database entries created with these methods will be populated with unique
    /// valid values.
    /// </summary>
    public class ModelHelper
    {
        private readonly Entities model;
        private int currentId;
        private object currentIdLock = new object();

        public ModelHelper(Entities model)
        {
            this.model = model;
        }

        private int GetNextId()
        {
            lock (currentIdLock)
            {
                currentId++;
            }

            return currentId;
        }

        private Guid IntegerToGuid(int id)
        {
            return new Guid(id, 0, 0, new byte[8]);
        }

        /// <summary>
        /// Create user with userName
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public AspNetUser CreateUser(string userName, IdType idType = IdType.Integer)
        {
            var userId = string.Empty;

            switch (idType)
            {
                case IdType.Guid:
                    userId = Guid.NewGuid().ToString();
                    break;
                case IdType.Integer:
                default:
                    userId = GetNextId().ToString();
                    break;
            }

            AspNetUser user = new AspNetUser();
            user.Id = userId;
            user.FirstName = "Test";
            user.Surname = "LastName";
            user.Email = userName;
            user.EmailConfirmed = true;
            user.UserName = userName;
            model.AspNetUsers.Add(user);
            return user;
        }

        /// <summary>
        /// Gets the user with the specified username. The user is created if it does not exist.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public AspNetUser GetOrCreateUser(string userName)
        {
            var user = model.AspNetUsers.SingleOrDefault(u => u.UserName == userName);

            if (user == null)
            {
                user = CreateUser(userName);
                model.SaveChanges();
            }

            return user;
        }

        /// <summary>
        /// Creates an organisation and a scheme.
        /// </summary>
        /// <returns></returns>
        public Scheme CreateScheme()
        {
            var organisation = CreateOrganisation();

            int schemeId = GetNextId();
            Scheme scheme = new Scheme
            {
                Id = IntegerToGuid(schemeId),
                Organisation = organisation,
                OrganisationId = organisation.Id,
                SchemeName = "test scheme name",
                ApprovalNumber = schemeId.ToString()
            };
            model.Schemes.Add(scheme);

            return scheme;
        }

        /// <summary>
        /// Creates and organisation
        /// </summary>
        /// <returns></returns>
        public Organisation CreateOrganisation()
        {
            int organisationId = GetNextId();
            Organisation organisation = new Organisation
            {
                Id = IntegerToGuid(organisationId),
                TradingName = string.Format("Organisation {0} Trading Name", organisationId),
            };
            model.Organisations.Add(organisation);

            return organisation;
        }

        /// <summary>
        /// Associates a user with an organisation. The user is created if the specified username is not present. The status
        /// of the user is set to active by default.
        /// </summary>
        /// <param name="organisation"></param>
        /// <param name="username"></param>
        /// <param name="userStatus"></param>
        /// <returns></returns>
        public OrganisationUser CreateOrganisationUser(Organisation organisation, string username, int userStatus = 2)
        {
            var organisationUser = new OrganisationUser();
            organisationUser.Id = Guid.NewGuid();
            organisationUser.OrganisationId = organisation.Id;
            organisationUser.UserId = GetOrCreateUser(username).Id;
            organisationUser.UserStatus = userStatus;

            model.OrganisationUsers.Add(organisationUser);

            return organisationUser;
        }

        public Address CreateOrganisationAddress()
        {
            Country england = model.Countries.Single(c => c.Name == "UK - England");
            int addressId = GetNextId();
            return new Address
            {
                Id = IntegerToGuid(addressId),
                Address1 = string.Format("Address {0} Address1", addressId),
                Address2 = string.Format("Address {0} Address2", addressId),
                Postcode = "458 5256",
                TownOrCity = string.Format("Address {0} TownOrCity", addressId),
                CountyOrRegion = string.Format("Address {0} CountyOrRegion", addressId),
                Email = "test@test.com",
                Telephone = "123 456 7890",
                Country = england,
                CountryId = england.Id
            };
        }

        /// <summary>
        /// Cretates a member upload associated with the specified scheme.
        /// After creation, the ComplianceYear and IsSubmitted properties
        /// should be explicitly set by the test.
        /// </summary>
        /// <param name="scheme"></param>
        /// <returns></returns>
        public MemberUpload CreateMemberUpload(Scheme scheme)
        {
            int memberUploadId = GetNextId();
            MemberUpload memberUpload = new MemberUpload
            {
                Id = IntegerToGuid(memberUploadId),
                OrganisationId = scheme.OrganisationId,
                Scheme = scheme,
                SchemeId = scheme.Id,
                Data = string.Format("<memberUpload{0} />", memberUploadId),
                UserId = GetOrCreateUser("Testuser").Id,
                Date = DateTime.UtcNow,
                ProcessTime = new TimeSpan(0)
            };
            model.MemberUploads.Add(memberUpload);

            return memberUpload;
        }

        /// <summary>
        /// Creates memberupload errors and warnings
        /// </summary>
        /// <param name="memberUpload"></param>
        /// <returns></returns>
        public MemberUploadError CreateMemberUploadError(MemberUpload memberUpload)
        {
            int memberUploadErrorId = GetNextId();
            MemberUploadError memberUploadError = new MemberUploadError
            {
                Id = IntegerToGuid(memberUploadErrorId),
                MemberUploadId = memberUpload.Id,
                ErrorLevel = ErrorLevel.Warning.Value,
                ErrorType = UploadErrorType.Business.Value,
                Description = "Test Warning"
            };

            model.MemberUploadErrors.Add(memberUploadError);

            return memberUploadError;
        }

        public RegisteredProducer GerOrCreateRegisteredProducer(Scheme scheme, int complianceYear, string registrationNumber)
        {
            // Try to find a RegisteredProducer that has already been created, otherwise create a new one.
            RegisteredProducer registeredProducer =
                model.RegisteredProducers.Local
                .SingleOrDefault(rp => rp.ProducerRegistrationNumber == registrationNumber &&
                                       rp.ComplianceYear == complianceYear &&
                                       rp.SchemeId == scheme.Id);

            if (registeredProducer == null)
            {
                int registeredProducerId = GetNextId();

                registeredProducer = new RegisteredProducer
                {
                    Id = IntegerToGuid(registeredProducerId),
                    Scheme = scheme,
                    SchemeId = scheme.Id,
                    ComplianceYear = complianceYear,
                    ProducerRegistrationNumber = registrationNumber,
                    CurrentSubmissionId = null
                };
                model.RegisteredProducers.Add(registeredProducer);
            }

            return registeredProducer;
        }

        /// <summary>
        /// Creates a producer associated with the specified member upload.
        /// The producer's business will be populated with the details of a company.
        /// </summary>
        /// <param name="memberUpload"></param>
        /// <returns></returns>
        public ProducerSubmission CreateProducerAsCompany(MemberUpload memberUpload, string registrationNumber)
        {
            ProducerSubmission producerSubsmission = CreateProducerWithEmptyBusiness(memberUpload, registrationNumber);
            Company company = CreateCompany();

            producerSubsmission.Business.Company = company;
            producerSubsmission.Business.CompanyId = company.Id;

            return producerSubsmission;
        }

        /// <summary>
        /// Creates a producer associated with the specified member upload.
        /// The producer's business will be populated with the details of a partnership.
        /// </summary>
        /// <param name="memberUpload"></param>
        /// <returns></returns>
        public ProducerSubmission CreateProducerAsPartnership(MemberUpload memberUpload, string registrationNumber)
        {
            ProducerSubmission producerSubmission = CreateProducerWithEmptyBusiness(memberUpload, registrationNumber);
            Partnership partnership = CreatePartnership();

            producerSubmission.Business.Partnership = partnership;
            producerSubmission.Business.PartnershipId = partnership.Id;

            return producerSubmission;
        }

        /// <summary>
        /// Creates a producer associated with the specified member upload.
        /// The producer's business will not be populated. I.e. The producer is a sole trader.
        /// </summary>
        /// <param name="memberUpload"></param>
        /// <returns></returns>
        public ProducerSubmission CreateProducerAsSoleTrader(MemberUpload memberUpload, string registrationNumber)
        {
            return CreateProducerWithEmptyBusiness(memberUpload, registrationNumber);
        }

        private ProducerSubmission CreateProducerWithEmptyBusiness(MemberUpload memberUpload, string registrationNumber)
        {
            int businessId = GetNextId();
            Business business = new Business
            {
                Id = IntegerToGuid(businessId),
            };
            model.Businesses.Add(business);

            int producerSubmissionId = GetNextId();

            RegisteredProducer registeredProducer = GerOrCreateRegisteredProducer(memberUpload.Scheme, memberUpload.ComplianceYear.Value, registrationNumber);

            var chargeBandAmount = FetchChargeBandAmount(ChargeBand.A);
            ProducerSubmission producerSubmission = new ProducerSubmission
            {
                Id = IntegerToGuid(producerSubmissionId),
                RegisteredProducer = registeredProducer,
                RegisteredProducerId = registeredProducer.Id,
                MemberUpload = memberUpload,
                MemberUploadId = memberUpload.Id,
                TradingName = string.Format("Producer {0} Trading Name", producerSubmissionId),
                UpdatedDate = new DateTime(2015, 1, 1, 0, 0, 0),
                Business = business,
                ProducerBusinessId = business.Id,
                AuthorisedRepresentativeId = null,
                ChargeBandAmountId = chargeBandAmount.Id,
                ChargeThisUpdate = 0,
                ObligationType = "B2B"
            };
            model.ProducerSubmissions.Add(producerSubmission);

            if (memberUpload.IsSubmitted)
            {
                registeredProducer.CurrentSubmissionId = IntegerToGuid(producerSubmissionId);
                registeredProducer.CurrentSubmission = producerSubmission;
            }

            return producerSubmission;
        }

        private Company CreateCompany()
        {
            Contact1 contact = CreateContact();

            int companyId = GetNextId();
            Company company = new Company
            {
                Id = IntegerToGuid(companyId),
                Name = string.Format("Company {0} Name", companyId),
                CompanyNumber = string.Format("No. {0}", companyId),
                Contact1 = contact,
                RegisteredOfficeContactId = contact.Id,
            };
            model.Companies.Add(company);

            return company;
        }

        private Partnership CreatePartnership()
        {
            Contact1 contact = CreateContact();

            int partnershipId = GetNextId();
            Partnership partnership = new Partnership
            {
                Id = IntegerToGuid(partnershipId),
                Name = string.Format("Partnership {0} Name", partnershipId),
                Contact1 = contact,
                PrincipalPlaceOfBusinessId = contact.Id,
            };
            model.Partnerships.Add(partnership);

            return partnership;
        }

        private Contact1 CreateContact()
        {
            Country england = model.Countries.Single(c => c.Name == "UK - England");

            int addressId = GetNextId();
            Address1 address = new Address1
            {
                Id = IntegerToGuid(addressId),
                PrimaryName = string.Format("Address {0} Primary Name", addressId),
                SecondaryName = string.Format("Address {0} Secondary Name", addressId),
                Street = string.Format("Address {0} Street", addressId),
                Town = string.Format("Address {0} Town", addressId),
                Locality = string.Format("Address {0} Locality", addressId),
                AdministrativeArea = string.Format("Address {0} Admin Area", addressId),
                PostCode = string.Format("Address {0} Post Code", addressId),
                Country = england,
                CountryId = england.Id,
            };
            model.Address1.Add(address);

            int contactId = GetNextId();
            Contact1 contact = new Contact1
            {
                Id = IntegerToGuid(contactId),
                Title = string.Format("Contact {0} Title", contactId),
                Forename = string.Format("Contact {0} Forename", contactId),
                Surname = string.Format("Contact {0} Surname", contactId),
                Telephone = string.Format("Contact {0} Telephone", contactId),
                Mobile = string.Format("Contact {0} Mobile", contactId),
                Fax = string.Format("Contact {0} Fax", contactId),
                Email = string.Format("Contact {0} Email", contactId),
                Address1 = address,
                AddressId = address.Id,
            };
            model.Contact1.Add(contact);

            return contact;
        }

        public ChargeBandAmount FetchChargeBandAmount(ChargeBand chargeBand)
        {
            return model.ChargeBandAmounts.First(pcb => pcb.ChargeBand == (int)chargeBand);
        }
    }
}
