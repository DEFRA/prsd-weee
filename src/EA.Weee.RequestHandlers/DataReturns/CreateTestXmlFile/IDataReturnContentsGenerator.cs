namespace EA.Weee.RequestHandlers.DataReturns.CreateTestXmlFile
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Domain.DataReturns;

    public interface IDataReturnContentsGenerator
    {
        Task<DataReturnContents> GenerateAsync(TestFileSettings settings);
    }
}
