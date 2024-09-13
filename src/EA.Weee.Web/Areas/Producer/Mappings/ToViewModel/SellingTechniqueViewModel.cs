//namespace EA.Weee.Web.Areas.Producer.Mappings.ToViewModel
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Linq;
//    using System.Web;

//    public class SellingTechniqueViewModel
//    {
//        [Display(Name = "Direct Selling to End User")]
//        public bool IsDirectSelling { get; set; }

//        [Display(Name = "Indirect Selling to End User")]
//        public bool IsIndirectSelling { get; set; }

//        public SellingTechniqueType ToSellingTechniqueType()
//        {
//            if (IsDirectSelling && IsIndirectSelling)
//                return SellingTechniqueType.Both;
//            if (IsDirectSelling)
//                return SellingTechniqueType.DirectSellingtoEndUser;
//            if (IsIndirectSelling)
//                return SellingTechniqueType.IndirectSellingtoEndUser;

//            // Default case if nothing is selected
//            return null;
//        }

//        public static SellingTechniqueViewModel FromSellingTechniqueType(SellingTechniqueType type)
//        {
//            return new SellingTechniqueViewModel
//            {
//                IsDirectSelling = type == SellingTechniqueType.DirectSellingtoEndUser || type == SellingTechniqueType.Both,
//                IsIndirectSelling = type == SellingTechniqueType.IndirectSellingtoEndUser || type == SellingTechniqueType.Both
//            };
//        }
//    }
//}