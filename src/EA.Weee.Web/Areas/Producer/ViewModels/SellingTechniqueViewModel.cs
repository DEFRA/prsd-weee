﻿namespace EA.Weee.Web.Areas.Producer.ViewModels
{
    using EA.Weee.Core.DirectRegistrant;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class SellingTechniqueViewModel
    {
        [Display(Name = "Direct selling to end user (mail, order, internet etc)")]
        public bool IsDirectSelling { get; set; }

        [Display(Name = "Indirect selling (other)")]
        public bool IsIndirectSelling { get; set; }

        public SellingTechniqueType ToSellingTechniqueType()
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

            throw new InvalidOperationException();
        }

        public string ToSellingTechniqueTypeString() 
        {
            switch (ToSellingTechniqueType())
            {
                case SellingTechniqueType.DirectSellingToEndUser:
                    return "Direct selling to end user (mail, order, internet etc)";
                case SellingTechniqueType.IndirectSellingToEndUser:
                    return "Indirect selling (other)";
                case SellingTechniqueType.Both:
                    return "Direct selling to end user (mail, order, internet etc) and Indirect selling (other)";
                default:
                    return string.Empty;
            }
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