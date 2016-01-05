
ALTER TABLE [Producer].[RegisteredProducer] 
	DROP CONSTRAINT [CN_RegisteredProducer_Unique_SchemeId_ProducerRegistrationNumber_ComplianceYear]
Go

/****** Object:  Index [CN_RegisteredProducer_Unique_SchemeId_ProducerRegistrationNumber_ComplianceYear_IsAligned]    Script Date: 05/01/2016 13:23:27 ******/
ALTER TABLE [Producer].[RegisteredProducer] ADD  CONSTRAINT [CN_RegisteredProducer_Unique_SchemeId_ProducerRegistrationNumber_ComplianceYear_IsAligned] UNIQUE NONCLUSTERED 
(
	[SchemeId] ASC,
	[ProducerRegistrationNumber] ASC,
	[ComplianceYear] ASC,
	[IsAligned] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO