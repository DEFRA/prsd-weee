namespace EA.Weee.Domain.Producer
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;
    using EA.Weee.Domain.Organisation;

    public partial class DirectRegistrant
    {
        public bool HasBrandName => BrandName != null;

        public void AddOrUpdateBrandName(BrandName brandName)
        {
            Guard.ArgumentNotNull(() => brandName, brandName);

            BrandName = brandName.OverwriteWhereNull(BrandName);
        }
    }
}
