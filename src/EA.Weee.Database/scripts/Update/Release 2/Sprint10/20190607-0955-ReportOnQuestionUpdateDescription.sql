UPDATE [AATF].ReportOnQuestion
SET Description = 'Obligated WEEE sent to another AATF or ATF for treatment'
WHERE Question = 'SentToAnotherATF'

UPDATE [AATF].ReportOnQuestion
Set Description = 'Obligated WEEE reused as a whole appliance'
Where Question = 'WEEEReused'