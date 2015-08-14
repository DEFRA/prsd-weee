GO
PRINT N'Updating data in [Admin].[CompetentAuthorityUser]...';

GO
DECLARE @competentauthorityId uniqueidentifier

select @competentauthorityId = Id from [Lookup].CompetentAuthority where Abbreviation = 'EA'
	
INSERT into [Admin].[CompetentAuthorityUser] ([Id], [UserId], [CompetentAuthorityId], [UserStatus]) 
 Select  
 newid(), 
 cast(U.Id as uniqueidentifier),
 @competentauthorityId, 
 1 
from [Identity].[AspNetUsers] U 
 Left join [Admin].[CompetentAuthorityUser] ca on U.Id = ca.UserId
 inner join [Identity].AspNetUserClaims C on U.Id = C.UserId 
	where C.ClaimValue = 'can_access_internal_area' and ca.UserId is NULL