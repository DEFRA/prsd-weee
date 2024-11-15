UPDATE [Organisation].Organisation SET IsRepresentingCompany = 1 WHERE Id in (
SELECT OrganisationId FROM Producer.DirectRegistrant WHERE AuthorisedRepresentativeId IS NOT NULL)