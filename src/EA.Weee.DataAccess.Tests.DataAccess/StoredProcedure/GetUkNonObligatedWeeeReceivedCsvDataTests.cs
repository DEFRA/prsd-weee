namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.DataReturns;
    using Core.Helpers;
    using Domain.AatfReturn;
    using FluentAssertions;
    using Prsd.Core;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;
    using NonObligatedWeee = Domain.AatfReturn.NonObligatedWeee;
    using QuarterType = Domain.DataReturns.QuarterType;
    using Return = Domain.AatfReturn.Return;
    using WeeeCategory = Domain.Lookup.WeeeCategory;

    public class GetUkNonObligatedWeeeReceivedCsvDataTests
    {
        private readonly Domain.Organisation.Organisation organisation;
        private readonly DateTime date;

        public GetUkNonObligatedWeeeReceivedCsvDataTests()
        {
            date = new DateTime(2019, 08, 09, 11, 12, 00);

            organisation = Domain.Organisation.Organisation.CreateSoleTrader("company");
        }

        [Fact]
        public async Task Execute_GivenComplianceYearWithNoData_DefaultDataShouldBeReturned()
        {
            using (var db = new DatabaseWrapper())
            {
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(db, organisation);

                db.WeeeContext.NonObligatedWeee.RemoveRange(db.WeeeContext.NonObligatedWeee);

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetUkNonObligatedWeeeReceivedByComplianceYear(2019);

                results.Count.Should().Be(70);

                var values = CategoryValues();
                var index = 0;
                foreach (var quarter in Quarters())
                {
                    for (var countValue = 0; countValue < values.Count(); countValue++)
                    {
                        var value = values.ElementAt(countValue);
                        results.ElementAt(index).Quarter.Should().Be(quarter <= 4 ? $"Q{quarter}" : "2019");
                        results.ElementAt(index).Category.Should().Be($"{(int)value}. {value.ToDisplayString()}");
                        results.ElementAt(index).TotalNonObligatedWeeeReceived.Should().BeNull();
                        results.ElementAt(index).TotalNonObligatedWeeeReceivedFromDcf.Should().BeNull();
                        index++;
                    }
                }
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Execute_GivenComplianceYearWithData_ExpectedReceivedDataShouldBeReturned(bool dcf)
        {
            using (var db = new DatabaseWrapper())
            {
                var returnQuarter1 = SetupSubmittedReturn(db, QuarterType.Q1);
                var return2Quarter1 = SetupSubmittedReturn(db, QuarterType.Q1);
                var returnQuarter2 = SetupSubmittedReturn(db, QuarterType.Q2);
                var return2Quarter2 = SetupSubmittedReturn(db, QuarterType.Q2);
                var returnQuarter3 = SetupSubmittedReturn(db, QuarterType.Q3);
                var return2Quarter3 = SetupSubmittedReturn(db, QuarterType.Q3);
                var returnQuarter4 = SetupSubmittedReturn(db, QuarterType.Q4);
                var return2Quarter4 = SetupSubmittedReturn(db, QuarterType.Q4);

                var values = CategoryValues();

                db.WeeeContext.NonObligatedWeee.RemoveRange(db.WeeeContext.NonObligatedWeee);

                await db.WeeeContext.SaveChangesAsync();
 
                foreach (var quarter in Quarters())
                {
                    if (quarter <= 4)
                    {
                        foreach (var weeeCategory in values)
                        {
                            db.WeeeContext.NonObligatedWeee.Add(new NonObligatedWeee(returnQuarter1, (int)weeeCategory, dcf, (int)weeeCategory));
                            db.WeeeContext.NonObligatedWeee.Add(new NonObligatedWeee(return2Quarter1, (int)weeeCategory, dcf, (int)weeeCategory));
                            db.WeeeContext.NonObligatedWeee.Add(new NonObligatedWeee(returnQuarter2, (int)weeeCategory, dcf, (int)weeeCategory));
                            db.WeeeContext.NonObligatedWeee.Add(new NonObligatedWeee(return2Quarter2, (int)weeeCategory, dcf, (int)weeeCategory));
                            db.WeeeContext.NonObligatedWeee.Add(new NonObligatedWeee(returnQuarter3, (int)weeeCategory, dcf, (int)weeeCategory));
                            db.WeeeContext.NonObligatedWeee.Add(new NonObligatedWeee(return2Quarter3, (int)weeeCategory, dcf, (int)weeeCategory));
                            db.WeeeContext.NonObligatedWeee.Add(new NonObligatedWeee(returnQuarter4, (int)weeeCategory, dcf, (int)weeeCategory));
                            db.WeeeContext.NonObligatedWeee.Add(new NonObligatedWeee(return2Quarter4, (int)weeeCategory, dcf, (int)weeeCategory));
                        }
                    }
                }

                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetUkNonObligatedWeeeReceivedByComplianceYear(2019);
                results.Count.Should().Be(70);

                var index = 0;
                foreach (var quarter in Quarters())
                {
                    for (var countValue = 0; countValue < values.Count(); countValue++)
                    {
                        var value = values.ElementAt(countValue);
                        if (quarter == 5)
                        {
                            results.ElementAt(index).Quarter.Should().Be($"2019");
                            results.ElementAt(index).Category.Should().Be($"{(int)value}. {value.ToDisplayString()}");
                            if (dcf)
                            {
                                results.ElementAt(index).TotalNonObligatedWeeeReceivedFromDcf.Should().Be((int)value * 4);
                            }
                            else
                            {
                                results.ElementAt(index).TotalNonObligatedWeeeReceived.Should().Be((int)value * 4);
                            }
                        }
                        else
                        {
                            results.ElementAt(index).Quarter.Should().Be($"Q{quarter}");
                            results.ElementAt(index).Category.Should().Be($"{(int)value}. {value.ToDisplayString()}");
                            if (dcf)
                            {
                                results.ElementAt(index).TotalNonObligatedWeeeReceivedFromDcf.Should().Be((int)value * 2 * 2);
                            }
                            else
                            {
                                results.ElementAt(index).TotalNonObligatedWeeeReceived.Should().Be((int)value * 2 * 2);
                            }
                        }
                        
                        index++;
                    }
                }
            }
        }

        private Return SetupSubmittedReturn(DatabaseWrapper db, QuarterType quarterType)
        {
            SystemTime.Freeze(date);
            var @return = ObligatedWeeeIntegrationCommon.CreateReturn(organisation, db.Model.AspNetUsers.First().Id, FacilityType.Aatf, 2019, quarterType);
            @return.UpdateSubmitted(db.Model.AspNetUsers.First().Id, false);
            SystemTime.Unfreeze();
            return @return;
        }

        private IEnumerable<WeeeCategory> CategoryValues()
        {
            return Enum.GetValues(typeof(WeeeCategory)).Cast<WeeeCategory>();
        }

        private IEnumerable<int> Quarters()
        {
            return new List<int>() { 1, 2, 3, 4, 5 };
        }
    }
}
