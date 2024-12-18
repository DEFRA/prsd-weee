﻿namespace EA.Weee.Tests.Core.Model
{
    using Domain.Error;
    using Domain.Lookup;
    using Domain.Obligation;
    using EA.Prsd.Core;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
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
        public AspNetUser CreateUser(string userName, IdType idType = IdType.Integer, string firstName = "Test", string surname = "LastName")
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
            user.FirstName = firstName;
            user.Surname = surname;
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
                user = CreateUser(userName, IdType.Guid);
                model.SaveChanges();
            }

            return user;
        }

        public CompetentAuthorityUser GetOrCreateCompetentAuthorityUser(string userName, int status = 2, Role role = null)
        {
            var competentAuthorityUser = model.CompetentAuthorityUsers.SingleOrDefault(c => c.AspNetUser.UserName == userName);
            if (competentAuthorityUser == null)
            {
                var user = CreateUser(userName);
                var competentAuthority = model.CompetentAuthorities.First();
                role = role ?? model.Roles.First();

                competentAuthorityUser = new CompetentAuthorityUser
                {
                    Id = IntegerToGuid(GetNextId()),
                    AspNetUser = user,
                    UserId = user.Id,
                    CompetentAuthority = competentAuthority,
                    CompetentAuthorityId = competentAuthority.Id,
                    Role = role,
                    RoleId = role.Id,
                    UserStatus = status
                };

                model.CompetentAuthorityUsers.Add(competentAuthorityUser);
            }

            return competentAuthorityUser;
        }

        /// <summary>
        /// Creates an organisation and a scheme.
        /// </summary>
        /// <returns></returns>
        public Scheme CreateScheme()
        {
            return CreateScheme(CreateOrganisation());
        }

        /// <summary>
        /// Creates a scheme and associates it to the specified organisation.
        /// </summary>
        /// <returns></returns>
        public Scheme CreateScheme(Organisation organisation, Guid? authorityId = null)
        {
            int schemeId = GetNextId();
            if (authorityId == null)
            {
                authorityId = new Guid("4EEE5942-01B2-4A4D-855A-34DEE1BBBF26");
            }
            Scheme scheme = new Scheme
            {
                Id = IntegerToGuid(schemeId),
                Organisation = organisation,
                OrganisationId = organisation.Id,
                SchemeName = "test scheme name",
                ApprovalNumber = schemeId.ToString(),
                CompetentAuthorityId = authorityId
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
        /// Creates a member upload associated with the specified scheme.
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
                Organisation = scheme.Organisation,
                Scheme = scheme,
                SchemeId = scheme.Id,
                Data = string.Format("<memberUpload{0} />", memberUploadId),
                CreatedById = GetOrCreateUser("Testuser").Id,
                CreatedDate = SystemTime.UtcNow,
                ProcessTime = new TimeSpan(0),
                ComplianceYear = 2016,
                HasAnnualCharge = false
            };

            model.MemberUploads.Add(memberUpload);

            return memberUpload;
        }

        /// <summary>
        /// Creates a member upload associated with the specified scheme and sets the member
        /// upload as being submitted. An invoice run can optionally be assigned to the submitted
        /// member upload.
        /// </summary>
        /// <param name="scheme"></param>
        /// <param name="invoiceRun"></param>
        /// <returns></returns>
        public MemberUpload CreateSubmittedMemberUpload(Scheme scheme, InvoiceRun invoiceRun = null)
        {
            var memberUpload = CreateMemberUpload(scheme);

            memberUpload.IsSubmitted = true;
            memberUpload.SubmittedDate = SystemTime.UtcNow;
            memberUpload.TotalCharges = 30;

            if (invoiceRun != null)
            {
                memberUpload.InvoiceRun = invoiceRun;
                memberUpload.InvoiceRunId = invoiceRun.Id;
            }

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

        public RegisteredProducer GetOrCreateRegisteredProducer(Scheme scheme, int complianceYear, string registrationNumber)
        {
            // Try to find a RegisteredProducer that has already been created, otherwise create a new one.
            // A removed producer is not reused.
            RegisteredProducer registeredProducer =
                model.RegisteredProducers.Local
                .SingleOrDefault(rp => rp.ProducerRegistrationNumber == registrationNumber &&
                                       rp.ComplianceYear == complianceYear &&
                                       rp.SchemeId == scheme.Id &&
                                       !rp.Removed);

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
                    CurrentSubmissionId = null,
                    Removed = false
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
            return CreateProducerAsCompany(memberUpload, registrationNumber, "B2B");
        }

        /// <summary>
        /// Creates a producer associated with the specified member upload.
        /// The producer's business will be populated with the details of a company.
        /// </summary>
        /// <param name="memberUpload"></param>
        /// <returns></returns>
        public ProducerSubmission CreateProducerAsCompany(MemberUpload memberUpload, string registrationNumber, string obligationType)
        {
            ProducerSubmission producerSubsmission = CreateProducerWithEmptyBusiness(memberUpload, registrationNumber, obligationType);
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
            return CreateProducerAsPartnership(memberUpload, registrationNumber, "B2B");
        }

        /// <summary>
        /// Creates a producer associated with the specified member upload.
        /// The producer's business will be populated with the details of a partnership.
        /// </summary>
        /// <param name="memberUpload"></param>
        /// <returns></returns>
        public ProducerSubmission CreateProducerAsPartnership(MemberUpload memberUpload, string registrationNumber, string obligationType)
        {
            ProducerSubmission producerSubmission = CreateProducerWithEmptyBusiness(memberUpload, registrationNumber, obligationType);
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
            return CreateProducerAsSoleTrader(memberUpload, registrationNumber, "B2B");
        }

        /// <summary>
        /// Creates a producer associated with the specified member upload.
        /// The producer's business will not be populated. I.e. The producer is a sole trader.
        /// </summary>
        /// <param name="memberUpload"></param>
        /// <returns></returns>
        public ProducerSubmission CreateProducerAsSoleTrader(MemberUpload memberUpload, string registrationNumber, string obligationType)
        {
            return CreateProducerWithEmptyBusiness(memberUpload, registrationNumber, "B2B");
        }

        public ProducerSubmission CreateInvoicedProducer(MemberUpload memberUpload, string registrationNumber)
        {
            int businessId = GetNextId();
            Business business = new Business
            {
                Id = IntegerToGuid(businessId),
            };
            model.Businesses.Add(business);

            int producerSubmissionId = GetNextId();

            RegisteredProducer registeredProducer = GetOrCreateRegisteredProducer(memberUpload.Scheme, memberUpload.ComplianceYear.Value, registrationNumber);

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
                ChargeThisUpdate = 30,
                ObligationType = "B2B",
                Invoiced = true
            };
            model.ProducerSubmissions.Add(producerSubmission);

            if (memberUpload.IsSubmitted)
            {
                registeredProducer.CurrentSubmissionId = IntegerToGuid(producerSubmissionId);
                registeredProducer.CurrentSubmission = producerSubmission;
            }

            return producerSubmission;
        }
        private ProducerSubmission CreateProducerWithEmptyBusiness(MemberUpload memberUpload, string registrationNumber, string obligationType)
        {
            int businessId = GetNextId();
            Business business = new Business
            {
                Id = IntegerToGuid(businessId),
            };
            model.Businesses.Add(business);

            int producerSubmissionId = GetNextId();

            RegisteredProducer registeredProducer = GetOrCreateRegisteredProducer(memberUpload.Scheme, memberUpload.ComplianceYear.Value, registrationNumber);

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
                ObligationType = obligationType,
                Invoiced = false
            };
            model.ProducerSubmissions.Add(producerSubmission);

            if (memberUpload.IsSubmitted)
            {
                registeredProducer.CurrentSubmissionId = IntegerToGuid(producerSubmissionId);
                registeredProducer.CurrentSubmission = producerSubmission;
            }

            return producerSubmission;
        }

        public BrandName_ CreateBrandName(ProducerSubmission producerSubmission, string brandName)
        {
            var result = new BrandName_
            {
                Id = IntegerToGuid(GetNextId()),
                Name = brandName,
                ProducerSubmissionId = producerSubmission.Id,
                ProducerSubmission = producerSubmission
            };

            model.BrandName_Set.Add(result);

            return result;
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

            var partnershipId = GetNextId();

            Partnership partnership = new Partnership
            {
                Id = IntegerToGuid(partnershipId),
                Name = string.Format("Partnership {0} Name", CreatePartnershipNameFromId(partnershipId)),
                Contact1 = contact,
                PrincipalPlaceOfBusinessId = contact.Id,
            };
            model.Partnerships.Add(partnership);

            return partnership;
        }

        private string CreatePartnershipNameFromId(int id)
        {
            var preceedingZeros = "0000000";

            if (id.ToString().Length > preceedingZeros.Length)
            {
                throw new ArgumentOutOfRangeException(string.Format("modulus of id must be less than {0} digits", id));
            }

            for (var i = 0; i < id.ToString().Length; i++)
            {
                preceedingZeros = preceedingZeros.Remove(0, 1);
            }

            return preceedingZeros + id;
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

        public DataReturnUpload CreateDataReturnUpload(Scheme scheme, DataReturnVersion dataReturnVersion = null)
        {
            int dataReturnUploadId = GetNextId();
            DataReturnUpload dataReturnUpload = new DataReturnUpload
            {
                Id = IntegerToGuid(dataReturnUploadId),
                SchemeId = scheme.Id,
                Scheme = scheme,
                Data = string.Format("<SchemeReturn{0} />", dataReturnUploadId),
                Date = SystemTime.UtcNow,
                ProcessTime = TimeSpan.Zero
            };

            if (dataReturnVersion != null)
            {
                dataReturnUpload.DataReturnVersionId = dataReturnVersion.Id;
                dataReturnUpload.DataReturnVersion = dataReturnVersion;

                if (dataReturnVersion.DataReturn != null)
                {
                    dataReturnUpload.ComplianceYear = dataReturnVersion.DataReturn.ComplianceYear;
                    dataReturnUpload.Quarter = dataReturnVersion.DataReturn.Quarter;
                }
            }

            model.DataReturnUploads.Add(dataReturnUpload);

            return dataReturnUpload;
        }

        public DataReturn GetOrCreateDataReturn(Scheme scheme, int complianceYear, int quarter)
        {
            // Try to find a DataReturn that has already been created, otherwise create a new one.
            var dataReturn =
                model.DataReturns.Local
                .SingleOrDefault(du => du.Scheme.Id == scheme.Id &&
                                       du.ComplianceYear == complianceYear &&
                                       du.Quarter == quarter);

            if (dataReturn == null)
            {
                dataReturn = CreateDataReturn(scheme, complianceYear, quarter);
            }

            return dataReturn;
        }

        public DataReturn CreateDataReturn(Scheme scheme, int complianceYear, int quarter)
        {
            int dataReturnId = GetNextId();

            var dataReturn = new DataReturn
            {
                Id = IntegerToGuid(dataReturnId),
                Scheme = scheme,
                SchemeId = scheme.Id,
                Quarter = quarter,
                ComplianceYear = complianceYear
            };
            model.DataReturns.Add(dataReturn);

            return dataReturn;
        }

        public Return CreateAatfReturn(Organisation organisation, int complianceYear, int quarter, string userId)
        {
            int dataReturnId = GetNextId();

            var aatfReturn = new Return
            {
                Id = IntegerToGuid(dataReturnId),
                Quarter = quarter,
                ComplianceYear = complianceYear,
                SubmittedDate = DateTime.Now,
                CreatedDate = DateTime.Now,
                CreatedById = userId,
                Organisation = organisation,
                OrganisationId = organisation.Id
            };
            model.Returns.Add(aatfReturn);

            return aatfReturn;
        }

        public DataReturnVersion CreateDataReturnVersion(Scheme scheme, int complianceYear, int quarter, bool isSubmitted = true, DataReturn dataReturn = null, WeeeCollectedReturnVersion weeeCollectedReturnVersion = null)
        {
            if (dataReturn == null)
            {
                dataReturn = GetOrCreateDataReturn(scheme, complianceYear, quarter);
            }

            Guid dataReturnVersionId = IntegerToGuid(GetNextId());

            var dataReturnVersion = new DataReturnVersion
            {
                Id = dataReturnVersionId,
                DataReturn = dataReturn,
                DataReturnId = dataReturn.Id,
                CreatedDate = SystemTime.UtcNow
            };

            if (isSubmitted)
            {
                dataReturnVersion.SubmittedDate = SystemTime.UtcNow;
                dataReturnVersion.SubmittingUserId = GetOrCreateUser("Testuser").Id;
                dataReturn.CurrentDataReturnVersionId = dataReturnVersionId;
                dataReturn.CurrentDataReturnVersion = dataReturnVersion;
            }

            if (weeeCollectedReturnVersion != null)
            {
                dataReturnVersion.WeeeCollectedReturnVersion = weeeCollectedReturnVersion;
            }

            model.DataReturnVersions.Add(dataReturnVersion);

            return dataReturnVersion;
        }

        public EeeOutputReturnVersion CreateEeeOutputReturnVersion()
        {
            var eeeOutputReturnVersion = new EeeOutputReturnVersion();
            eeeOutputReturnVersion.Id = IntegerToGuid(GetNextId());
            eeeOutputReturnVersion.EeeOutputReturnVersionAmounts = new List<EeeOutputReturnVersionAmount>();

            return eeeOutputReturnVersion;
        }

        public EeeOutputAmount CreateEeeOutputAmount(RegisteredProducer registeredProducer, string obligationType, int weeeCategory, decimal tonnage)
        {
            var eeeOutputAmount = new EeeOutputAmount();
            eeeOutputAmount.Id = IntegerToGuid(GetNextId());
            eeeOutputAmount.ObligationType = obligationType;
            eeeOutputAmount.WeeeCategory = weeeCategory;
            eeeOutputAmount.Tonnage = tonnage;
            eeeOutputAmount.RegisteredProducer = registeredProducer;

            return eeeOutputAmount;
        }

        public EeeOutputAmount CreateEeeOutputAmount(DataReturnVersion dataReturnVersion, RegisteredProducer registeredProducer, string obligationType, int weeeCategory, decimal tonnage)
        {
            var eeeOutputAmount = CreateEeeOutputAmount(registeredProducer, obligationType, weeeCategory, tonnage);
            AddEeeOutputAmount(dataReturnVersion, eeeOutputAmount);

            return eeeOutputAmount;
        }

        public void AddEeeOutputAmount(DataReturnVersion dataReturnVersion, EeeOutputAmount eeeOutputAmount)
        {
            if (dataReturnVersion.EeeOutputReturnVersion == null)
            {
                dataReturnVersion.EeeOutputReturnVersion = CreateEeeOutputReturnVersion();
            }

            AddEeeOutputAmount(dataReturnVersion.EeeOutputReturnVersion, eeeOutputAmount);
        }

        public void AddEeeOutputAmount(EeeOutputReturnVersion eeeOutputReturnVersion, EeeOutputAmount eeeOutputAmount)
        {
            if (eeeOutputReturnVersion.EeeOutputReturnVersionAmounts == null)
            {
                eeeOutputReturnVersion.EeeOutputReturnVersionAmounts = new List<EeeOutputReturnVersionAmount>();
            }

            eeeOutputReturnVersion
                .EeeOutputReturnVersionAmounts
                .Add(new EeeOutputReturnVersionAmount
                {
                    EeeOuputAmountId = eeeOutputAmount.Id,
                    EeeOutputAmount = eeeOutputAmount,
                    EeeOutputReturnVersionId = eeeOutputReturnVersion.Id,
                    EeeOutputReturnVersion = eeeOutputReturnVersion
                });
        }

        public InvoiceRun CreateInvoiceRun()
        {
            var compenentAuthority = model.CompetentAuthorities.First();

            var ibisFileData = new IbisFileData
            {
                Id = IntegerToGuid(GetNextId()),
                FileId = GetNextId(),
                CustomerFileData = "Customer file data",
                CustomerFileName = "Customer file name",
                TransactionFileData = "Transaction file data",
                TransactionFileName = "Transaction file name"
            };

            var user = GetOrCreateUser("Invoice Run User");

            var invoiceRun = new InvoiceRun
            {
                Id = IntegerToGuid(GetNextId()),
                IssuedDate = SystemTime.UtcNow,
                IssuedByUserId = user.Id,
                CompetentAuthority = compenentAuthority,
                CompetentAuthorityId = compenentAuthority.Id,
                IbisFileData = ibisFileData,
                IbisFileDataId = ibisFileData.Id
            };

            model.InvoiceRuns.Add(invoiceRun);

            return invoiceRun;
        }

        public AatfDeliveryLocation CreateAatfDeliveryLocation(string approvalNumber, string facilityName)
        {
            var aatfDeliveryLocation = new AatfDeliveryLocation
            {
                Id = IntegerToGuid(GetNextId()),
                ApprovalNumber = approvalNumber,
                FacilityName = facilityName
            };

            model.AatfDeliveryLocations.Add(aatfDeliveryLocation);

            return aatfDeliveryLocation;
        }

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "aeDeliveryLocation is valid.")]
        public AeDeliveryLocation CreateAeDeliveryLocation(string approvalNumber, string operatorName)
        {
            var aeDeliveryLocation = new AeDeliveryLocation
            {
                Id = IntegerToGuid(GetNextId()),
                ApprovalNumber = approvalNumber,
                OperatorName = operatorName
            };

            model.AeDeliveryLocations.Add(aeDeliveryLocation);

            return aeDeliveryLocation;
        }

        public WeeeCollectedAmount CreateWeeeCollectedAmount(DataReturnVersion dataReturnVersion, string obligationType, int weeeCategory, decimal tonnage)
        {
            var weeeCollectedAmount = new WeeeCollectedAmount
            {
                Id = IntegerToGuid(GetNextId()),
                ObligationType = obligationType,
                Tonnage = tonnage,
                WeeeCategory = weeeCategory
            };

            AddWeeeCollectedAmount(dataReturnVersion, weeeCollectedAmount);

            return weeeCollectedAmount;
        }

        public void AddWeeeCollectedAmount(DataReturnVersion dataReturnVersion, WeeeCollectedAmount weeeCollectedAmount)
        {
            if (dataReturnVersion.WeeeCollectedReturnVersion == null)
            {
                dataReturnVersion.WeeeCollectedReturnVersion = CreateWeeeCollectedReturnVersion();
            }

            AddWeeeCollectedAmount(dataReturnVersion.WeeeCollectedReturnVersion, weeeCollectedAmount);
        }

        public void AddWeeeCollectedAmount(WeeeCollectedReturnVersion weeeCollectedReturnVersion, WeeeCollectedAmount weeeCollectedAmount)
        {
            if (weeeCollectedReturnVersion.WeeeCollectedReturnVersionAmounts == null)
            {
                weeeCollectedReturnVersion.WeeeCollectedReturnVersionAmounts = new List<WeeeCollectedReturnVersionAmount>();
            }

            weeeCollectedReturnVersion
                .WeeeCollectedReturnVersionAmounts
                .Add(new WeeeCollectedReturnVersionAmount
                {
                    WeeeCollectedAmountId = weeeCollectedAmount.Id,
                    WeeeCollectedAmount = weeeCollectedAmount,
                    WeeeCollectedReturnVersionId = weeeCollectedReturnVersion.Id,
                    WeeeCollectedReturnVersion = weeeCollectedReturnVersion
                });
        }

        public WeeeCollectedAmount CreateWeeeCollectedAmount(ObligationType obligationType, decimal tonnage, WeeeCategory category)
        {
            var weeeCollectedAmount = new WeeeCollectedAmount
            {
                Id = IntegerToGuid(GetNextId()),
                ObligationType = obligationType.ToString(),
                Tonnage = tonnage,
                WeeeCategory = (int)category
            };

            model.WeeeCollectedAmounts.Add(weeeCollectedAmount);

            return weeeCollectedAmount;
        }

        public WeeeCollectedReturnVersionAmount CreateWeeeCollectedReturnVersionAmount(WeeeCollectedAmount weeeCollectedAmount, WeeeCollectedReturnVersion weeeCollectedReturnVersion)
        {
            var weeeCollectedReturnVersionAmount = new WeeeCollectedReturnVersionAmount
            {
                WeeeCollectedAmount = weeeCollectedAmount,
                WeeeCollectedReturnVersion = weeeCollectedReturnVersion
            };

            model.WeeeCollectedReturnVersionAmounts.Add(weeeCollectedReturnVersionAmount);

            return weeeCollectedReturnVersionAmount;
        }

        public WeeeCollectedReturnVersion CreateWeeeCollectedReturnVersion()
        {
            var weeeCollectedReturnVersion = new WeeeCollectedReturnVersion
            {
                Id = IntegerToGuid(GetNextId())
            };

            model.WeeeCollectedReturnVersions.Add(weeeCollectedReturnVersion);

            return weeeCollectedReturnVersion;
        }

        public WeeeDeliveredReturnVersion CreateWeeeDeliveredReturnVersion()
        {
            var weeeDeliveredReturnVersions = new WeeeDeliveredReturnVersion
            {
                Id = IntegerToGuid(GetNextId())
            };

            model.WeeeDeliveredReturnVersions.Add(weeeDeliveredReturnVersions);

            return weeeDeliveredReturnVersions;
        }

        public void AddWeeeDeliveredAmount(WeeeDeliveredReturnVersion weeeDeliveredReturnVersion, WeeeDeliveredAmount weeeDeliveredAmount)
        {
            if (weeeDeliveredReturnVersion.WeeeDeliveredReturnVersionAmounts == null)
            {
                weeeDeliveredReturnVersion.WeeeDeliveredReturnVersionAmounts = new List<WeeeDeliveredReturnVersionAmount>();
            }

            weeeDeliveredReturnVersion
                .WeeeDeliveredReturnVersionAmounts
                .Add(new WeeeDeliveredReturnVersionAmount
                {
                    WeeeDeliveredAmountId = weeeDeliveredAmount.Id,
                    WeeeDeliveredAmount = weeeDeliveredAmount,
                    WeeeDeliveredReturnVersionId = weeeDeliveredReturnVersion.Id,
                    WeeeDeliveredReturnVersion = weeeDeliveredReturnVersion
                });
        }

        public void AddWeeeDeliveredAmount(DataReturnVersion dataReturnVersion, WeeeDeliveredAmount weeeDeliveredAmount)
        {
            if (dataReturnVersion.WeeeDeliveredReturnVersion == null)
            {
                dataReturnVersion.WeeeDeliveredReturnVersion = CreateWeeeDeliveredReturnVersion();
            }

            AddWeeeDeliveredAmount(dataReturnVersion.WeeeDeliveredReturnVersion, weeeDeliveredAmount);
        }

        public WeeeDeliveredAmount CreateWeeeDeliveredAmount(DataReturnVersion dataReturnVersion, AatfDeliveryLocation location, string obligationType, int weeeCategory, decimal tonnage)
        {
            var weeeDeliveredAmount = new WeeeDeliveredAmount
            {
                Id = IntegerToGuid(GetNextId()),
                AatfDeliveryLocationId = location.Id,
                AatfDeliveryLocation = location,
                ObligationType = obligationType,
                Tonnage = tonnage,
                WeeeCategory = weeeCategory
            };

            AddWeeeDeliveredAmount(dataReturnVersion, weeeDeliveredAmount);

            return weeeDeliveredAmount;
        }

        public WeeeDeliveredAmount CreateWeeeDeliveredAmount(DataReturnVersion dataReturnVersion, AeDeliveryLocation location, string obligationType, int weeeCategory, decimal tonnage)
        {
            var weeeDeliveredAmount = new WeeeDeliveredAmount
            {
                Id = IntegerToGuid(GetNextId()),
                AeDeliveryLocationId = location.Id,
                AeDeliveryLocation = location,
                ObligationType = obligationType,
                Tonnage = tonnage,
                WeeeCategory = weeeCategory
            };

            AddWeeeDeliveredAmount(dataReturnVersion, weeeDeliveredAmount);

            return weeeDeliveredAmount;
        }

        public Return CreateReturn()
        {
            return new Return() { Organisation = CreateOrganisation(), Quarter = 1, Id = IntegerToGuid(1) };
        }

        public AATF CreateAatf()
        {
            return new AATF()
            {
                Id = Guid.NewGuid()
            };
        }
    }
}
