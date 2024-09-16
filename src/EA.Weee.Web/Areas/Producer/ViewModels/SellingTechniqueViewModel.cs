namespace EA.Weee.Web.Areas.Producer.ViewModels
{
    using EA.Weee.Core.DirectRegistrant;
    using System.ComponentModel.DataAnnotations;

    public class SellingTechniqueViewModel
    {
        [Display(Name = "Direct Selling to End User")]
        public bool IsDirectSelling { get; set; }

        [Display(Name = "Indirect Selling to End User")]
        public bool IsIndirectSelling { get; set; }

        public SellingTechniqueType? ToSellingTechniqueType()
        {
            switch (IsDirectSelling)
            {
                case true when IsIndirectSelling:
                    return SellingTechniqueType.Both;
                case true:
                    return SellingTechniqueType.DirectSellingToEndUser;
            }

            if (IsIndirectSelling)
            {
                return SellingTechniqueType.IndirectSellingToEndUser;
            }
                
            return null;
        }

        public static SellingTechniqueViewModel FromSellingTechniqueType(SellingTechniqueType? type)
        {
            return new SellingTechniqueViewModel
            {
                IsDirectSelling = type == SellingTechniqueType.DirectSellingToEndUser || type == SellingTechniqueType.Both,
                IsIndirectSelling = type == SellingTechniqueType.IndirectSellingToEndUser || type == SellingTechniqueType.Both
            };
        }
    }
}