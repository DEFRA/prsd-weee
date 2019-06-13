namespace EA.Weee.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class PanArea
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public Guid CompetentAuthorityId { get; private set; }
        
        protected PanArea()
        {
        }

        public PanArea(Guid id, string name, Guid competentauthorityid)
        {
            this.Id = id;
            this.Name = name;
            this.CompetentAuthorityId = competentauthorityid;
        }
    }
}
