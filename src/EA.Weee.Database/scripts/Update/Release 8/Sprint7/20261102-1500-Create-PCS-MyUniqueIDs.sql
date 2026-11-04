-- ================================
-- Create User-defined Table Type
-- ================================

-- Create the data type
CREATE TYPE [PCS].[MyUniqueIDs] AS TABLE 
(
	Id	UNIQUEIDENTIFIER NOT NULL, 
    PRIMARY KEY (Id)
)
GO
