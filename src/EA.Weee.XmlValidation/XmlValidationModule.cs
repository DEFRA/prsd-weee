﻿namespace EA.Weee.XmlValidation
{
    using Autofac;
    using BusinessValidation;
    using BusinessValidation.Helpers;
    using BusinessValidation.QuerySets;
    using BusinessValidation.Rules.Producer;
    using BusinessValidation.Rules.Scheme;

    public class XmlValidationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // TODO: Autoscan

            builder.RegisterType<XmlBusinessValidator>()
                .As<IXmlBusinessValidator>()
                .InstancePerRequest();

            builder.RegisterType<MigratedProducerQuerySet>()
                .As<IMigratedProducerQuerySet>()
                .InstancePerRequest();

            builder.RegisterType<ProducerQuerySet>()
                .As<IProducerQuerySet>()
                .InstancePerRequest();

            builder.RegisterType<SchemeQuerySet>()
                .As<ISchemeQuerySet>()
                .InstancePerRequest();

            builder.RegisterType<AmendmentHasNoProducerRegistrationNumber>()
                .As<IAmendmentHasNoProducerRegistrationNumber>()
                .InstancePerRequest();

            builder.RegisterType<AnnualTurnoverMismatch>()
                .As<IAnnualTurnoverMismatch>()
                .InstancePerRequest();

            builder.RegisterType<InsertHasProducerRegistrationNumber>()
                .As<IInsertHasProducerRegistrationNumber>()
                .InstancePerRequest();

            builder.RegisterType<ProducerAlreadyRegistered>()
                .As<IProducerAlreadyRegistered>()
                .InstancePerRequest();

            builder.RegisterType<ProducerNameAlreadyRegistered>()
                .As<IProducerNameAlreadyRegistered>()
                .InstancePerRequest();

            builder.RegisterType<ProducerNameChange>()
                .As<IProducerNameChange>()
                .InstancePerRequest();

            builder.RegisterType<ProducerRegistrationNumberValidity>()
                .As<IProducerRegistrationNumberValidity>()
                .InstancePerRequest();

            builder.RegisterType<UkBasedAuthorisedRepresentative>()
                .As<IUkBasedAuthorisedRepresentative>()
                .InstancePerRequest();

            builder.RegisterType<CorrectSchemeApprovalNumber>()
                .As<ICorrectSchemeApprovalNumber>()
                .InstancePerRequest();

            builder.RegisterType<DuplicateProducerNames>()
                .As<IDuplicateProducerNames>()
                .InstancePerRequest();

            builder.RegisterType<DuplicateProducerRegistrationNumbers>()
                .As<IDuplicateProducerRegistrationNumbers>()
                .InstancePerRequest();

            builder.RegisterType<SearchMatcher>()
                .As<ISearchMatcher>()
                .InstancePerRequest();

            builder.RegisterType<EnsureAnOverseasProducerIsNotBasedInTheUK>()
                .As<IEnsureAnOverseasProducerIsNotBasedInTheUK>()
                .InstancePerRequest();

            builder.RegisterType<ProducerChargeBandChange>()
                .As<IProducerChargeBandChange>()
                .InstancePerRequest();

            builder.RegisterType<CompanyAlreadyRegistered>()
                .As<ICompanyAlreadyRegistered>()
                .InstancePerRequest();                

            //builder.RegisterAssemblyTypes(GetType().Assembly)
            //    .Where(t => t.Name.StartsWith("BusinessValidation"))
            //    .AsImplementedInterfaces();
            //    .InstancePerRequest();

            //builder.RegisterType<XmlBusinessValidator>()
            //    .As<IXmlBusinessValidator>()
            //    .InstancePerRequest();
        }
    }
}
