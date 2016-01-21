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
        private readonly IMapper mapper;

        public ProducerEeeMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public ProducerEeeDetails Map(IEnumerable<ProducerEeeByQuarter> source)
        {
            return new ProducerEeeDetails
            {
                Q1EEE = FilterEeeByQuarter(source, QuarterSelection.Q1).ToList(),
                Q2EEE = FilterEeeByQuarter(source, QuarterSelection.Q2).ToList(),
                Q3EEE = FilterEeeByQuarter(source, QuarterSelection.Q3).ToList(),
                Q4EEE = FilterEeeByQuarter(source, QuarterSelection.Q4).ToList(),
                TotalEEE = FilterEeeByQuarter(source, QuarterSelection.All).ToList()
            };
        }

        private IEnumerable<Eee> FilterEeeByQuarter(IEnumerable<ProducerEeeByQuarter> source, QuarterSelection quarterSelection)
        {
            switch (quarterSelection)
            {
                case QuarterSelection.All:
                    break;
                default:
                   source = source
                        .Where(s => (int)s.Quarter.Q == (int)quarterSelection)
                        .ToList();
                    break;
            }

            if (!source.Any())
            {
                return new List<Eee>();
            }

            return source
                .Select(eee => eee.Eee)
                .Aggregate((prev, curr) => prev.Concat(curr))
                .Select(
                    eee =>
                        new Eee(eee.Tonnage, mapper.Map<Domain.Lookup.WeeeCategory, WeeeCategory>(eee.WeeeCategory),
                            mapper.Map<Domain.ObligationType, ObligationType>(eee.ObligationType)));
        }
    }
}
