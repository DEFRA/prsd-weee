namespace EA.Prsd.Core.DataAccess.Conventions
{
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.ModelConfiguration.Conventions;

    /// <summary>
    /// By default EF6 uses an underscore naming convention for foreign keys (ForeignKey_Id).
    /// If we aren't using this we need to generate a different convention for foreign key naming.
    /// </summary>
    /// <remarks>
    /// See: https://msdn.microsoft.com/en-us/data/dn469439
    /// </remarks>
    internal class ForeignKeyNaming : IStoreModelConvention<AssociationType>
    {
        public void Apply(AssociationType item, DbModel model)
        {
            if (item.IsForeignKey)
            {
                var constraint = item.Constraint;

                if (PropertiesHaveDefaultNames(constraint.FromProperties, constraint.ToProperties))
                {
                    NormalizeForeignKeyNames(constraint.FromProperties);
                }

                if (PropertiesHaveDefaultNames(constraint.ToProperties, constraint.FromProperties))
                {
                    NormalizeForeignKeyNames(constraint.ToProperties);
                }
            }
        }

        private bool PropertiesHaveDefaultNames(ReadOnlyMetadataCollection<EdmProperty> properties,
            ReadOnlyMetadataCollection<EdmProperty> otherEndProperties)
        {
            bool result = false;

            if (properties.Count == otherEndProperties.Count)
            {
                result = true;

                for (int i = 0; i < properties.Count; i++)
                {
                    if (!properties[i].Name.EndsWith("_" + otherEndProperties[i].Name))
                    {
                        result = false;
                        break;
                    }
                }
            }

            return result;
        }

        private void NormalizeForeignKeyNames(ReadOnlyMetadataCollection<EdmProperty> properties)
        {
            for (int i = 0; i < properties.Count; i++)
            {
                int underscoreLocation = properties[i].Name.IndexOf('_');

                if (underscoreLocation > 0)
                {
                    properties[i].Name = properties[i].Name.Replace("_", string.Empty);
                }
            }
        }
    }
}
