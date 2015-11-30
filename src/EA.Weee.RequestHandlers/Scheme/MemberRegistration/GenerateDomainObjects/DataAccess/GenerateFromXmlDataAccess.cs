namespace EA.Weee.RequestHandlers.Scheme.MemberRegistration.GenerateDomainObjects.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Runtime.ExceptionServices;
    using System.Threading.Tasks;
    using Core.Helpers.PrnGeneration;
    using Domain;
    using Domain.Producer;
    using Weee.DataAccess;

    public class GenerateFromXmlDataAccess : IGenerateFromXmlDataAccess
    {
        private WeeeContext context;

        public GenerateFromXmlDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Generates unique, pseudorandom PRNs with minimal database interaction.
        /// Works by:
        /// a) uniquely mapping each unsigned integer to another pseudorandom unsigned integer
        /// b) uniquely mapping each unsigned integer to a specific PRN
        /// Combining those two mappings, and using a sequential seed, we can obtain pseudorandom PRNs
        /// with assurance that we will not repeat ourselves for a very, very long time.
        /// </summary>
        /// <param name="context">The database context</param>
        /// <param name="numberOfPrnsNeeded">A non-negative integer</param>
        /// <returns></returns>
        public async Task<Queue<string>> ComputePrns(int numberOfPrnsNeeded)
        {
            bool succeeded = false;
            bool retry = false;
            IEnumerable<DbEntityEntry> staleValues = null;
            List<PrnAsComponents> generatedPrns = new List<PrnAsComponents>();
            ExceptionDispatchInfo exceptionDispatchInfo = null;

            var prnHelper = new PrnHelper(new QuadraticResidueHelper());

            try
            {
                succeeded = false;
                retry = false;

                // to avoid concurrency issues, we want to read the latest seed, 'reserve' some PRNs (figuring
                // out the resulting final seed as we go), and write the final seed back as quickly as possible
                uint originalLatestSeed = (uint)context.SystemData.Select(sd => sd.LatestPrnSeed).First();

                uint currentSeed = originalLatestSeed;
                for (int ii = 0; ii < numberOfPrnsNeeded; ii++)
                {
                    var prnFromSeed = new PrnAsComponents(currentSeed + 1);
                    generatedPrns.Add(prnFromSeed);
                    currentSeed = prnFromSeed.ToSeedValue();
                }

                // we write back the next acceptable seed to the database, for next time
                // since there are some mathematical constraints on the acceptable values
                context.SystemData.First().LatestPrnSeed = currentSeed;
                await context.SaveChangesAsync();

                succeeded = true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                staleValues = ex.Entries;
                retry = true;
            }
            catch (Exception ex)
            {
                // In .NET 4.5 it is not allowed to use "await" within catch blocks; this forces us to put
                // code after the catch block. As a result of that, we don't want to throw un-handled exceptions
                // here, so we capture the dispatch info and throw it at the end of this method.
                exceptionDispatchInfo = System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex);
            }

            if (succeeded)
            {
                // now we're done with the fairly time sensitive database read/write,
                // we can 'randomise' the results at our leisure
                return new Queue<string>(generatedPrns.Select(p => prnHelper.CreateUniqueRandomVersionOfPrn(p)));
            }
            else if (retry)
            {
                // If we need to retry, we are probably in a race condition with another thread.
                // To avoid retrying indefinitely, we'll wait a few milliseconds to get out of sync
                // with the other thread.
                await Task.Delay(TimeSpan.FromMilliseconds(new Random().Next(100)));

                // If the database value for [LatestPrnSeed] was updated between the time we fetched the value
                // tried to update it with our new value, we will get a DbConcurrencyException.
                // To handle this we will just call this method again until it succeeds.
                // However, as dependency injection forces us to reuse the same WeeeContext, the SystemData
                // entity will already be attached to the context giving us the same stale value from when it
                // was first fetched.
                // The DbUpdateConcurrencyException gives us the ability to reload this entity from the database.
                foreach (var entry in staleValues)
                {
                    if (entry.Entity is SystemData)
                    {
                        await entry.ReloadAsync();
                    }
                }

                // Now that we have the latest value loaded, we'll try calling this method again.
                return await ComputePrns(numberOfPrnsNeeded);
            }
            else
            {
                // Something else bad happened and it's not possible to fix that here.
                exceptionDispatchInfo.Throw();
                throw new Exception("This will never be thrown.");
            }
        }

        public async Task<Country> GetCountry(string countryName)
        {
            Country country = null;

            if (!string.IsNullOrEmpty(countryName))
            {
                country = await context.Countries.SingleAsync(c => c.Name == countryName);
            }

            return country;
        }

        public Task<Producer> GetLatestProducerRecord(Guid schemeId, string producerRegistrationNumber)
        {
            return GetLatestProducerRecord(schemeId, producerRegistrationNumber, false);
        }

        public Task<Producer> GetLatestProducerRecordExcludeScheme(Guid schemeId, string producerRegistrationNumber)
        {
            return GetLatestProducerRecord(schemeId, producerRegistrationNumber, true);
        }

        private Task<Producer> GetLatestProducerRecord(Guid schemeId, string producerRegistrationNumber, bool excludeSpecifiedSchemeId)
        {
            // Get the producers for scheme based on producer->prn and producer->lastsubmitted
            // is latest date and memberupload ->IsSubmitted is true.
            return context.MemberUploads
                  .Where(member => member.IsSubmitted && ((member.SchemeId == schemeId) != excludeSpecifiedSchemeId))
                  .SelectMany(p => p.Producers)
                  .Where(p => p.RegistrationNumber == producerRegistrationNumber)
                  .OrderByDescending(p => p.UpdatedDate)
                  .FirstOrDefaultAsync();
        }

        public Task<MigratedProducer> GetMigratedProducer(string producerRegistrationNumber)
        {
            return context.MigratedProducers.FirstOrDefaultAsync(m => m.ProducerRegistrationNumber == producerRegistrationNumber);
        }
    }
}
