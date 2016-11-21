namespace EA.Weee.RequestHandlers.Admin.GetDataReturnSubmissionChanges
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain;
    using Domain.DataReturns;
    using Domain.Obligation;
    using Domain.Producer;

    public class GetDataReturnSubmissionEeeChangesCsvDataAccess : IGetDataReturnSubmissionEeeChangesCsvDataAccess
    {
        private readonly WeeeContext context;

        public GetDataReturnSubmissionEeeChangesCsvDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<DataReturnSubmissionEeeChanges> GetChanges(Guid currentDataReturnVersionId, Guid previousDataReturnVersionId)
        {
            if (currentDataReturnVersionId == previousDataReturnVersionId)
            {
                throw new ArgumentException("Current and previous submissions IDs cannot be the same.");
            }

            var currentSubmission = await context.DataReturnVersions
                .Include(d => d.DataReturn.Scheme)
                .Include(d => d.EeeOutputReturnVersion.EeeOutputAmounts
                              .Select(a => a.RegisteredProducer.CurrentSubmission.ProducerBusiness))
                .SingleOrDefaultAsync(x => x.Id == currentDataReturnVersionId);

            if (currentSubmission == null)
            {
                throw new InvalidOperationException("Current submission record not found.");
            }

            var previousSubmission = await context.DataReturnVersions
                .Include(d => d.DataReturn.Scheme)
                .Include(d => d.EeeOutputReturnVersion.EeeOutputAmounts
                              .Select(a => a.RegisteredProducer.CurrentSubmission.ProducerBusiness))
                .SingleOrDefaultAsync(x => x.Id == previousDataReturnVersionId);

            if (previousSubmission == null)
            {
                throw new InvalidOperationException("Previous submission record not found.");
            }

            if (currentSubmission.DataReturn.Scheme.Id != previousSubmission.DataReturn.Scheme.Id ||
                currentSubmission.DataReturn.Quarter.Year != previousSubmission.DataReturn.Quarter.Year ||
                currentSubmission.DataReturn.Quarter.Q != previousSubmission.DataReturn.Quarter.Q)
            {
                throw new InvalidOperationException("The submissions must be for the same scheme, compliance year and quarter.");
            }

            int year = currentSubmission.DataReturn.Quarter.Year;
            int quarter = (int)currentSubmission.DataReturn.Quarter.Q;

            var currentAmounts = currentSubmission.EeeOutputReturnVersion != null ?
                   currentSubmission
                      .EeeOutputReturnVersion
                      .EeeOutputAmounts
                      .ToList()
                   : null;

            var previousAmounts = previousSubmission.EeeOutputReturnVersion != null ?
                   previousSubmission
                     .EeeOutputReturnVersion
                     .EeeOutputAmounts
                     .ToList()
                   : null;

            List<DataReturnSubmissionEeeChangesCsvData> csvData;

            if (currentAmounts == null && previousAmounts == null)
            {
                csvData = new List<DataReturnSubmissionEeeChangesCsvData>();
            }
            else if (currentAmounts != null && previousAmounts != null)
            {
                var currentAmountsByProducer = GroupByProducer(currentAmounts);
                var previousAmountsByProducer = GroupByProducer(previousAmounts);

                csvData = new List<DataReturnSubmissionEeeChangesCsvData>();

                foreach (var currentProducerAmounts in currentAmountsByProducer)
                {
                    List<EeeOutputAmount> previousProducerAmounts;
                    if (previousAmountsByProducer.TryGetValue(currentProducerAmounts.Key, out previousProducerAmounts))
                    {
                        if (!currentProducerAmounts.Value.UnorderedEqual(previousProducerAmounts))
                        {
                            csvData.Add(ExtractProducerData(currentProducerAmounts.Key, currentProducerAmounts.Value, year, quarter, currentSubmission.SubmittedDate.Value, DataReturnSubmissionChangeType.Amended));
                            csvData.Add(ExtractProducerData(currentProducerAmounts.Key, previousProducerAmounts, year, quarter, previousSubmission.SubmittedDate.Value));
                        }
                    }
                    else
                    {
                        csvData.Add(ExtractProducerData(currentProducerAmounts.Key, currentProducerAmounts.Value, year, quarter, currentSubmission.SubmittedDate.Value, DataReturnSubmissionChangeType.New));
                    }
                }

                foreach (var previousProducerAmount in previousAmountsByProducer)
                {
                    if (!currentAmountsByProducer.ContainsKey(previousProducerAmount.Key))
                    {
                        csvData.Add(ExtractProducerData(previousProducerAmount.Key, null, year, quarter, currentSubmission.SubmittedDate.Value, DataReturnSubmissionChangeType.Removed));
                    }
                }
            }
            else if (previousAmounts != null)
            {
                csvData =
                    previousAmounts
                    .Select(x => x.RegisteredProducer)
                    .Distinct(new ProducerRecordComparer())
                    .Select(x => ExtractProducerData(x, null, year, quarter, currentSubmission.SubmittedDate.Value, DataReturnSubmissionChangeType.Removed))
                    .ToList();
            }
            else
            {
                csvData =
                    GroupByProducer(currentAmounts)
                    .Select(x => ExtractProducerData(x.Key, x.Value, year, quarter, currentSubmission.SubmittedDate.Value, DataReturnSubmissionChangeType.New))
                    .ToList();
            }

            var orderedCsvData = csvData
                .OrderBy(x => x.ProducerName)
                .ThenByDescending(x => x.SubmissionDate)
                .ToList();

            return new DataReturnSubmissionEeeChanges
            {
                SchemeApprovalNumber = currentSubmission.DataReturn.Scheme.ApprovalNumber,
                ComplianceYear = year,
                Quarter = quarter,
                CurrentSubmissionDate = currentSubmission.SubmittedDate.Value,
                CsvData = orderedCsvData
            };
        }

        private Dictionary<RegisteredProducer, List<EeeOutputAmount>> GroupByProducer(List<EeeOutputAmount> outputAmounts)
        {
            return outputAmounts
                .GroupBy(x => x.RegisteredProducer)
                .ToDictionary(x => x.Key, x => x.ToList(), new ProducerRecordComparer());
        }

        private DataReturnSubmissionEeeChangesCsvData ExtractProducerData(RegisteredProducer producer, List<EeeOutputAmount> producerAmounts,
            int year, int quarter, DateTime submittedDate, DataReturnSubmissionChangeType? changeType = null)
        {
            var data = new DataReturnSubmissionEeeChangesCsvData
            {
                ProducerName = producer.CurrentSubmission.OrganisationName,
                ProducerRegistrationNumber = producer.ProducerRegistrationNumber,
                ChangeType = changeType,
                ComplianceYear = year,
                Quarter = quarter,
                SubmissionDate = submittedDate,
            };

            if (producerAmounts != null)
            {
                data.Cat1B2B = GetEeeDataTonnes(producerAmounts, ObligationType.B2B, 1);
                data.Cat2B2B = GetEeeDataTonnes(producerAmounts, ObligationType.B2B, 2);
                data.Cat3B2B = GetEeeDataTonnes(producerAmounts, ObligationType.B2B, 3);
                data.Cat4B2B = GetEeeDataTonnes(producerAmounts, ObligationType.B2B, 4);
                data.Cat5B2B = GetEeeDataTonnes(producerAmounts, ObligationType.B2B, 5);
                data.Cat6B2B = GetEeeDataTonnes(producerAmounts, ObligationType.B2B, 6);
                data.Cat7B2B = GetEeeDataTonnes(producerAmounts, ObligationType.B2B, 7);
                data.Cat8B2B = GetEeeDataTonnes(producerAmounts, ObligationType.B2B, 8);
                data.Cat9B2B = GetEeeDataTonnes(producerAmounts, ObligationType.B2B, 9);
                data.Cat10B2B = GetEeeDataTonnes(producerAmounts, ObligationType.B2B, 10);
                data.Cat11B2B = GetEeeDataTonnes(producerAmounts, ObligationType.B2B, 11);
                data.Cat12B2B = GetEeeDataTonnes(producerAmounts, ObligationType.B2B, 12);
                data.Cat13B2B = GetEeeDataTonnes(producerAmounts, ObligationType.B2B, 13);
                data.Cat14B2B = GetEeeDataTonnes(producerAmounts, ObligationType.B2B, 14);

                data.Cat1B2C = GetEeeDataTonnes(producerAmounts, ObligationType.B2C, 1);
                data.Cat2B2C = GetEeeDataTonnes(producerAmounts, ObligationType.B2C, 2);
                data.Cat3B2C = GetEeeDataTonnes(producerAmounts, ObligationType.B2C, 3);
                data.Cat4B2C = GetEeeDataTonnes(producerAmounts, ObligationType.B2C, 4);
                data.Cat5B2C = GetEeeDataTonnes(producerAmounts, ObligationType.B2C, 5);
                data.Cat6B2C = GetEeeDataTonnes(producerAmounts, ObligationType.B2C, 6);
                data.Cat7B2C = GetEeeDataTonnes(producerAmounts, ObligationType.B2C, 7);
                data.Cat8B2C = GetEeeDataTonnes(producerAmounts, ObligationType.B2C, 8);
                data.Cat9B2C = GetEeeDataTonnes(producerAmounts, ObligationType.B2C, 9);
                data.Cat10B2C = GetEeeDataTonnes(producerAmounts, ObligationType.B2C, 10);
                data.Cat11B2C = GetEeeDataTonnes(producerAmounts, ObligationType.B2C, 11);
                data.Cat12B2C = GetEeeDataTonnes(producerAmounts, ObligationType.B2C, 12);
                data.Cat13B2C = GetEeeDataTonnes(producerAmounts, ObligationType.B2C, 13);
                data.Cat14B2C = GetEeeDataTonnes(producerAmounts, ObligationType.B2C, 14);
            }

            return data;
        }

        private decimal? GetEeeDataTonnes(List<EeeOutputAmount> amounts, ObligationType obligationType, int category)
        {
            var amount = amounts
                .Where(a => a.ObligationType == obligationType)
                .Where(a => (int)a.WeeeCategory == category)
                .SingleOrDefault();

            return amount != null ? (decimal?)amount.Tonnage : null;
        }

        private class ProducerRecordComparer : IEqualityComparer<RegisteredProducer>
        {
            public bool Equals(RegisteredProducer x, RegisteredProducer y)
            {
                return x.Id == y.Id;
            }

            public int GetHashCode(RegisteredProducer obj)
            {
                return obj.Id.GetHashCode();
            }
        }
    }
}
