namespace EA.Weee.RequestHandlers.Mappings
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Admin;
    using Core.DataReturns;
    using Core.Shared;
    using Domain.Producer;
    using Prsd.Core.Mapper;

    public class ProducerEeeMap : IMap<IEnumerable<ProducerEeeByQuarter>, ProducerEeeDetails>
    {
        public ProducerEeeDetails Map(IEnumerable<ProducerEeeByQuarter> source)
        {
            return new ProducerEeeDetails
            {
                Q1EEE = FilterEeeByQuarter(source, QuarterSelection.Q1).ToList(),
                Q2EEE = FilterEeeByQuarter(source, QuarterSelection.Q2).ToList(),
                Q3EEE = FilterEeeByQuarter(source, QuarterSelection.Q3).ToList(),
                Q4EEE = FilterEeeByQuarter(source, QuarterSelection.Q4).ToList(),
                TotalEee = FilterEeeByQuarter(source, QuarterSelection.All).ToList()
            };
        }

        private IEnumerable<Eee> FilterEeeByQuarter(IEnumerable<ProducerEeeByQuarter> source, QuarterSelection quarterSelection)
        {
            switch (quarterSelection)
            {
                case QuarterSelection.All:

                    if (source.Any())
                    {
                        return source
                        .Select(eee => eee.Eee)
                        .Aggregate((prev, curr) => prev.Concat(curr))
                        .GroupBy(eee => new { eee.ObligationType, eee.WeeeCategory })
                        .Select(g => new Eee(g.Sum(eee => eee.Tonnage),
                            (WeeeCategory)(int)g.Key.WeeeCategory,
                            (ObligationType)(int)g.Key.ObligationType));
                    }

                    return new List<Eee>();

                default:
                    source = source
                        .Where(s => (int)s.Quarter.Q == (int)quarterSelection);

                    if (source.Any())
                    {
                        return source.Select(eee => eee.Eee)
                        .Aggregate((prev, curr) => prev.Concat(curr))
                        .Select(eee => new Eee(eee.Tonnage,
                            (WeeeCategory)(int)eee.WeeeCategory,
                            (ObligationType)(int)eee.ObligationType));
                    }

                    return new List<Eee>();
            }      
        }
    }
}
