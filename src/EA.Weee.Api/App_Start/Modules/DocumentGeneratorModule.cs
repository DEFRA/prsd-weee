namespace EA.Weee.Api.Modules
{
    using Autofac;
    using DocumentGeneration;
    using DocumentGeneration.DocumentGenerator;
    using DocumentGeneration.Mapper;
    using Domain;

    public class DocumentGeneratorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DocumentGenerator>().As<IDocumentGenerator>();
            builder.RegisterType<NotificationDocumentMerger>().AsSelf();
            builder.RegisterAssemblyTypes(typeof(DocumentGenerator).Assembly)
                .Where(t => typeof(INotificationMergeMapper).IsAssignableFrom(t)).As<INotificationMergeMapper>();
        }
    }
}