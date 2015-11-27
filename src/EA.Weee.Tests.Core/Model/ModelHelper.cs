namespace EA.Weee.Tests.Core.Model
{
    using Domain;
    using Domain.Lookup;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// This class provides helper methods for deterministically seeding a database.
    /// It should be used during the "arrange" step of data access integration tests.
    /// All database entries created with these methods will be populated with unique
    /// valid values.
    /// </summary>
    public class ModelHelper
    {
        private readonly Entities model;
        private Dictionary<Type, int> objectCount = new Dictionary<Type, int>();

        public ModelHelper(Entities model)
        {
            this.model = model;
        }

        private int GetNextId(Type type)
        {
            if (!objectCount.ContainsKey(type))
            {
                objectCount.Add(type, 1);
                return 1;
            }
            else
            {
                objectCount[type]++;
                return objectCount[type];
            }
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
                    userId = GetNextId(typeof(AspNetUser)).ToString();
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

            int schemeId = GetNextId(typeof(Scheme));
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
            int organisationId = GetNextId(typeof(Organisation));
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

        /// <summary>
        /// Cretates a member upload associated with the specified scheme.
        /// After creation, the ComplianceYear and IsSubmitted properties
        /// should be explicitly set by the test.
        /// </summary>
        /// <param name="scheme"></param>
        /// <returns></returns>
        public MemberUpload CreateMemberUpload(Scheme scheme)
        {
            int memberUploadId = GetNextId(typeof(MemberUpload));
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
            int memberUploadErrorId = GetNextId(typeof(MemberUploadError));
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

        /// <summary>
        /// Creates a producer associated with the specified member upload.
        /// The producer's business will be populated with the details of a company.
        /// </summary>
        /// <param name="memberUpload"></param>
        /// <returns></returns>
        public Producer CreateProducerAsCompany(MemberUpload memberUpload, string registrationNumber)
        {
            Producer producer = CreateProducerWithEmptyBusiness(memberUpload, registrationNumber);
            Company company = CreateCompany();

            producer.Business.Company = company;
            producer.Business.CompanyId = company.Id;

            return producer;
        }

        /// <summary>
        /// Creates a producer associated with the specified member upload.
        /// The producer's business will be populated with the details of a partnership.
        /// </summary>
        /// <param name="memberUpload"></param>
        /// <returns></returns>
        public Producer CreateProducerAsPartnership(MemberUpload memberUpload, string registrationNumber)
        {
            Producer producer = CreateProducerWithEmptyBusiness(memberUpload, registrationNumber);
            Partnership partnership = CreatePartnership();

            producer.Business.Partnership = partnership;
            producer.Business.PartnershipId = partnership.Id;

            return producer;
        }

        /// <summary>
        /// Creates a producer associated with the specified member upload.
        /// The producer's business will not be populated. I.e. The producer is a sole trader.
        /// </summary>
        /// <param name="memberUpload"></param>
        /// <returns></returns>
        public Producer CreateProducerAsSoleTrader(MemberUpload memberUpload, string registrationNumber)
        {
            return CreateProducerWithEmptyBusiness(memberUpload, registrationNumber);
        }

        private Producer CreateProducerWithEmptyBusiness(MemberUpload memberUpload, string registrationNumber)
        {
            int businessId = GetNextId(typeof(Business));
            Business business = new Business
            {
                Id = IntegerToGuid(businessId),
            };
            model.Businesses.Add(business);

            int producerId = GetNextId(typeof(Producer));
            var chargeBandAmount = FetchChargeBandAmount(ChargeBand.A);
            Producer producer = new Producer
            {
                Id = IntegerToGuid(producerId),
                MemberUpload = memberUpload,
                MemberUploadId = memberUpload.Id,
                RegistrationNumber = registrationNumber,
                TradingName = string.Format("Producer {0} Trading Name", producerId),
                UpdatedDate = new DateTime(2015, 1, 1, 0, 0, 0),
                Business = business,
                ProducerBusinessId = business.Id,
                Scheme = memberUpload.Scheme,
                SchemeId = memberUpload.Scheme.Id,
                AuthorisedRepresentativeId = null,
                ChargeBandAmountId = chargeBandAmount.Id,
                ChargeThisUpdate = 445
            };
            model.Producers.Add(producer);

            return producer;
        }

        private Company CreateCompany()
        {
            Contact1 contact = CreateContact();

            int companyId = GetNextId(typeof(Company));
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

            int partnershipId = GetNextId(typeof(Partnership));
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

            int addressId = GetNextId(typeof(Address));
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

            int contactId = GetNextId(typeof(Contact));
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
