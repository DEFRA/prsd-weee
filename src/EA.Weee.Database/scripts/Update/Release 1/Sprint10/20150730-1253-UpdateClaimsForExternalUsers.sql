INSERT INTO [Identity].[AspNetUserClaims] (UserId, ClaimType, ClaimValue)
SELECT Id, 'http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationmethod', 'can_access_external_area'
FROM [Identity].[AspNetUsers]
WHERE Id NOT IN 
(
  SELECT UserId FROM [Identity].[AspNetUserClaims]
  WHERE ClaimType = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationmethod'
  AND ClaimValue IN ('can_access_internal_area', 'can_access_external_area')
)
  