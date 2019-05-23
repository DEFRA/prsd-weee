using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EA.Weee.Migration.Organisation
{
    public enum Mode
    {
        LocalValidation = 1,
        DatabaseValidation = 2,
        DataMigration = 3
    }
}