CREATE FUNCTION Organisation.fnCheckOrganisationIsSchemeOrBalancingScheme(	
	@OrganisationId UNIQUEIDENTIFIER
)
RETURNS TINYINT
AS
BEGIN
	DECLARE @Result TINYINT
	IF EXISTS (SELECT OrganisationId FROM [Organisation].ProducerBalancingScheme WHERE OrganisationId = @OrganisationId) OR
	 EXISTS (SELECT Id FROM [PCS].Scheme WHERE OrganisationId = @OrganisationId)
			SET @Result = 1
	ELSE
		SET @Result = 0

	RETURN @Result
END
GO

ALTER TABLE [Evidence].[Note] DROP CONSTRAINT [FK_Note_RecipientId]
GO

UPDATE
	n
SET
	n.RecipientId = s.OrganisationId
FROM
	[Evidence].Note n
	INNER JOIN [PCS].Scheme s ON s.Id = n.RecipientId

ALTER TABLE [Evidence].[Note] WITH CHECK ADD  CONSTRAINT [FK_RecipientOrganisation_OrganisationId] FOREIGN KEY([RecipientId])
REFERENCES [Organisation].[Organisation] ([Id])
GO

ALTER TABLE [Evidence].[Note] CHECK CONSTRAINT [FK_RecipientOrganisation_OrganisationId]
GO

CREATE NONCLUSTERED INDEX [IDX_RecipientOrganisation_OrganisationId] ON [Evidence].[Note]
(
	[OrganisationId] ASC
)
GO

ALTER TABLE [Evidence].[Note] 
  ADD CONSTRAINT CHK_Check_Recipient_Scheme_Or_Balancing_Scheme 
  CHECK (Organisation.fnCheckOrganisationIsSchemeOrBalancingScheme(RecipientId) = 1); 
GO
