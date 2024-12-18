namespace EA.Weee.XmlValidation
{
    using Autofac;
    using BusinessValidation.MemberRegistration;
    using BusinessValidation.MemberRegistration.Helpers;
    using BusinessValidation.MemberRegistration.QuerySets;
    using BusinessValidation.MemberRegistration.QuerySets.Queries.Producer;
    using BusinessValidation.MemberRegistration.Rules.Producer;
    using BusinessValidation.MemberRegistration.Rules.Scheme;
    using EA.Prsd.Core.Autofac;
    using EA.Weee.Xml.MemberRegistration;
    using Errors;
    using SchemaValidation;

    public class XmlValidationModule : Module
    {
        private readonly EnvironmentResolver environment;

        public XmlValidationModule(EnvironmentResolver environment)
        {
            this.environment = environment;
        }

        public XmlValidationModule()
        {
            this.environment = new EnvironmentResolver()
            {
                HostEnvironment = HostEnvironmentType.Owin,
                IocApplication = IocApplication.RequestHandler,
                IsTestRun = false
            };
        }

        protected override void Load(ContainerBuilder builder)
        {
            // TODO: Autoscan
            builder.RegisterTypeByEnvironment<MemberRegistrationBusinessValidator, IMemberRegistrationBusinessValidator>(environment);
            builder.RegisterTypeByEnvironment<MigratedProducerQuerySet, IMigratedProducerQuerySet>(environment);
            builder.RegisterTypeByEnvironment<ProducerQuerySet, IProducerQuerySet>(environment);
            builder.RegisterTypeByEnvironment<SchemeQuerySet, ISchemeQuerySet>(environment);
            builder.RegisterTypeByEnvironment<SchemeEeeDataQuerySet, ISchemeEeeDataQuerySet>(environment);
            builder.RegisterTypeByEnvironment<AmendmentHasNoProducerRegistrationNumber, IAmendmentHasNoProducerRegistrationNumber>(environment);
            builder.RegisterTypeByEnvironment<AnnualTurnoverMismatch, IAnnualTurnoverMismatch>(environment);
            builder.RegisterTypeByEnvironment<InsertHasProducerRegistrationNumber, IInsertHasProducerRegistrationNumber>(environment);
            builder.RegisterTypeByEnvironment<ProducerAlreadyRegistered, IProducerAlreadyRegistered>(environment);
            builder.RegisterTypeByEnvironment<ProducerNameAlreadyRegistered, IProducerNameAlreadyRegistered>(environment);
            builder.RegisterTypeByEnvironment<ProducerNameChange, IProducerNameChange>(environment);
            builder.RegisterTypeByEnvironment<ProducerRegistrationNumberValidity, IProducerRegistrationNumberValidity>(environment);
            builder.RegisterTypeByEnvironment<UkBasedAuthorisedRepresentative, IUkBasedAuthorisedRepresentative>(environment);
            builder.RegisterTypeByEnvironment<CorrectSchemeApprovalNumber, ICorrectSchemeApprovalNumber>(environment);
            builder.RegisterTypeByEnvironment<DuplicateProducerNames, IDuplicateProducerNames>(environment);
            builder.RegisterTypeByEnvironment<DuplicateProducerRegistrationNumbers, IDuplicateProducerRegistrationNumbers>(environment);
            builder.RegisterTypeByEnvironment<SearchMatcher, ISearchMatcher>(environment);
            builder.RegisterTypeByEnvironment<EnsureAnOverseasProducerIsNotBasedInTheUK, IEnsureAnOverseasProducerIsNotBasedInTheUK>(environment);
            builder.RegisterTypeByEnvironment<ProducerChargeBandChange, IProducerChargeBandChange>(environment);
            builder.RegisterTypeByEnvironment<CompanyAlreadyRegistered, ICompanyAlreadyRegistered>(environment);
            builder.RegisterTypeByEnvironment<CompanyRegistrationNumberChange, ICompanyRegistrationNumberChange>(environment);
            builder.RegisterTypeByEnvironment<ProducerObligationTypeChange, IProducerObligationTypeChange>(environment);
            builder.RegisterTypeByEnvironment<ExistingProducerNames, IExistingProducerNames>(environment);
            builder.RegisterTypeByEnvironment<CurrentCompanyProducers, ICurrentCompanyProducers>(environment);
            builder.RegisterTypeByEnvironment<ExistingProducerRegistrationNumbers, IExistingProducerRegistrationNumbers>(environment);
            builder.RegisterTypeByEnvironment<CurrentProducersByRegistrationNumber, ICurrentProducersByRegistrationNumber>(environment);
            builder.RegisterTypeByEnvironment<XmlErrorTranslator, IXmlErrorTranslator>(environment);
            builder.RegisterTypeByEnvironment<SchemaValidator, ISchemaValidator>(environment);
            builder.RegisterTypeByEnvironment<NamespaceValidator, INamespaceValidator>(environment);
        }
    }
}
