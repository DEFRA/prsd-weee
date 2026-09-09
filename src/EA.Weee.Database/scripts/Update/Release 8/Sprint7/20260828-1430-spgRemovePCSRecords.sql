SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- Create date: 2026 Aug 28
-- Modified date: 2026 Aug 28
-- Description:	This stored procedure deletes data from multiple sources
--				which match a given SchemeId and ComplianceYear
-- =============================================
CREATE PROCEDURE [PCS].[spgRemovePCSRecords]
	@SchemeId		UNIQUEIDENTIFIER NULL,
    @ComplianceYear INT NULL
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		BEGIN TRANSACTION;

		/*==================================================================*/
		/*	OBLIGATION SCHEME	*/
		/*==================================================================*/
		--* delete from ObligationSchemeAmount
		DELETE osa
		FROM
			[PCS].[ObligationSchemeAmount] osa
		INNER JOIN
			[PCS].[ObligationScheme] os
				ON os.Id = osa.ObligationSchemeId
		WHERE
			os.SchemeId = @SchemeId
		AND
			os.ComplianceYear = @ComplianceYear;

		--* delete from ObligationScheme
		DELETE FROM
			[PCS].[ObligationScheme]
		WHERE
			SchemeId = @SchemeId
		AND
			ComplianceYear = @ComplianceYear;


		/*==================================================================*/
		/*	MEMBER UPLOAD	*/
		/*==================================================================*/
		--* delete from AdditionalCompanyDetails
		DELETE acd
		FROM
			[Organisation].[AdditionalCompanyDetails] acd
		INNER JOIN
			[Producer].[DirectRegistrant] dr
				ON dr.Id = acd.DirectRegistrantId
		INNER JOIN
			[Producer].[BrandName] bn
				ON bn.Id = dr.BrandNameId
		INNER JOIN
			[Producer].[ProducerSubmission] ps
				ON ps.Id = bn.ProducerSubmissionId
		INNER JOIN
			[PCS].[MemberUpload] mu
				ON mu.Id = ps.MemberUploadId
		WHERE
			mu.SchemeId = @SchemeId
		AND
			mu.ComplianceYear = @ComplianceYear;

		--* drop FK_PaymentSession_DirectRegistrantId
		ALTER TABLE [Producer].[PaymentSession] 
			DROP CONSTRAINT [FK_PaymentSession_DirectRegistrantId]

		--* delete from DirectRegistrant
		DELETE dr
		FROM
			[Producer].[DirectRegistrant] dr
		INNER JOIN
			[Producer].[BrandName] bn
				ON bn.Id = dr.BrandNameId
		INNER JOIN
			[Producer].[ProducerSubmission] ps
				ON ps.Id = bn.ProducerSubmissionId
		INNER JOIN
			[PCS].[MemberUpload] mu
				ON mu.Id = ps.MemberUploadId
		WHERE
			mu.SchemeId = @SchemeId
		AND
			mu.ComplianceYear = @ComplianceYear;

		/*---------------------------------------------------------------------------------*/
		--NOT GOING TO DIRECT PRODUCER SUBMISSION 
		--AS IT SHOULD BE COVERED BY REGISTERED PRODUCER
		/*---------------------------------------------------------------------------------*/

		--* create FK_PaymentSession_DirectRegistrantId
		ALTER TABLE [Producer].[PaymentSession] 
			ADD CONSTRAINT [FK_PaymentSession_DirectRegistrantId] 
			FOREIGN KEY([DirectRegistrantId])
			REFERENCES [Producer].[DirectRegistrant] ([Id])

		ALTER TABLE [Producer].[PaymentSession] 
			CHECK CONSTRAINT [FK_PaymentSession_DirectRegistrantId]

		--* drop FK_DirectProducerSubmission_CurrentSubmission
		ALTER TABLE [Producer].[DirectProducerSubmission] 
			DROP CONSTRAINT [FK_DirectProducerSubmission_CurrentSubmission];

		--* delete from DirectProducerSubmissionHistory
		DELETE dp
		FROM
			[Producer].[DirectProducerSubmissionHistory] dp
		INNER JOIN
			[Producer].[BrandName] bn
				ON bn.Id = dp.BrandNameId
		INNER JOIN
			[Producer].[ProducerSubmission] ps
				ON ps.Id = bn.ProducerSubmissionId
		INNER JOIN
			[PCS].[MemberUpload] mu
				ON mu.Id = ps.MemberUploadId
		WHERE
			mu.SchemeId = @SchemeId
		AND
			mu.ComplianceYear = @ComplianceYear;
	
		/*---------------------------------------------------------------------------------*/
		--NOT GOING TO DIRECT PRODUCER SUBMISSION 
		--AS IT SHOULD BE COVERED BY REGISTERED PRODUCER
		/*---------------------------------------------------------------------------------*/

		--* create FK_DirectProducerSubmission_CurrentSubmission
		ALTER TABLE [Producer].[DirectProducerSubmission] 
			ADD CONSTRAINT [FK_DirectProducerSubmission_CurrentSubmission] 
			FOREIGN KEY([CurrentSubmissionId])
			REFERENCES [Producer].[DirectProducerSubmissionHistory] ([Id]);

		ALTER TABLE [Producer].[DirectProducerSubmission] 
			CHECK CONSTRAINT [FK_DirectProducerSubmission_CurrentSubmission];

		--* delete from BrandName
		DELETE bn
		FROM
			[Producer].[BrandName] bn
		INNER JOIN
			[Producer].[ProducerSubmission] ps
				ON ps.Id = bn.ProducerSubmissionId
		INNER JOIN
			[PCS].[MemberUpload] mu
				ON mu.Id = ps.MemberUploadId
		WHERE
			mu.SchemeId = @SchemeId
		AND
			mu.ComplianceYear = @ComplianceYear;

		--* delete from SICCode
		DELETE sc
		FROM
			[Producer].[SICCode] sc
		INNER JOIN
			[Producer].[ProducerSubmission] ps
				ON ps.Id = sc.ProducerSubmissionId
		INNER JOIN
			[PCS].[MemberUpload] mu
				ON mu.Id = ps.MemberUploadId
		WHERE
			mu.SchemeId = @SchemeId
		AND
			mu.ComplianceYear = @ComplianceYear;

		--* drop FK_RegisteredProducer_ProducerSubmission
		ALTER TABLE [Producer].[RegisteredProducer] 
			DROP CONSTRAINT [FK_RegisteredProducer_ProducerSubmission];

		--* delete from ProducerSubmission
		DELETE ps
		FROM
			[Producer].[ProducerSubmission] ps
		INNER JOIN
			[PCS].[MemberUpload] mu
				ON mu.Id = ps.MemberUploadId
		WHERE
			mu.SchemeId = @SchemeId
		AND
			mu.ComplianceYear = @ComplianceYear;
	
		--NOT GOING TO REGISTERED PRODUCER 
		--AS IT HAS ITS OWN SCHEMEID AND COMPLIANCEYEAR

		--* create FK_RegisteredProducer_ProducerSubmission
		ALTER TABLE [Producer].[RegisteredProducer] 
			ADD	CONSTRAINT [FK_RegisteredProducer_ProducerSubmission] 
			FOREIGN KEY([CurrentSubmissionId])
			REFERENCES [Producer].[ProducerSubmission] ([Id]);

		ALTER TABLE [Producer].[RegisteredProducer] 
			CHECK CONSTRAINT [FK_RegisteredProducer_ProducerSubmission];

		--* delete from MemberUploadError
		DELETE mue
		FROM
			[PCS].[MemberUploadError] mue
		INNER JOIN
			[PCS].[MemberUpload] mu
				ON mu.Id = mue.MemberUploadId
		WHERE
			mu.SchemeId = @SchemeId
		AND
			mu.ComplianceYear = @ComplianceYear;

		--* delete from MemberUpload
		DELETE FROM
			[PCS].[MemberUpload]
		WHERE
			SchemeId = @SchemeId
		AND
			ComplianceYear = @ComplianceYear;


		/*==================================================================*/
		/*	DATA RETURN UPLOAD	*/
		/*==================================================================*/
		--* delete from DataReturnUploadError
		DELETE drue
		FROM
			[PCS].[DataReturnUploadError] drue
		INNER JOIN
			[PCS].[DataReturnUpload] dru
				ON dru.Id = drue.DataReturnUploadId
		WHERE
			dru.SchemeId = @SchemeId
		AND
			dru.ComplianceYear = @ComplianceYear;

		--* delete from DataReturnUpload
		DELETE FROM
			[PCS].[DataReturnUpload]
		WHERE
			SchemeId = @SchemeId
		AND
			ComplianceYear = @ComplianceYear;


		/*==================================================================*/
		/*	DATA RETURN	*/
		/*==================================================================*/
		--* drop FK_DataReturnUpload_DataReturnVersion
		ALTER TABLE [PCS].[DataReturnUpload] 
			DROP CONSTRAINT [FK_DataReturnUpload_DataReturnVersion];

		--* drop FK_DataReturn_DataReturnVersion
		ALTER TABLE [PCS].[DataReturn] 
			DROP CONSTRAINT [FK_DataReturn_DataReturnVersion];

		--* delete from DataReturnVersion
		DELETE drv
		FROM
			[PCS].[DataReturnVersion] drv
		INNER JOIN
			[PCS].[DataReturn] dr
				ON dr.Id = drv.DataReturnId
		WHERE
			dr.SchemeId = @SchemeId
		AND
			dr.ComplianceYear = @ComplianceYear;

		--NOT GOING TO DataReturnUpload 
		--AS IT HAS ITS OWN SCHEMEID AND COMPLIANCEYEAR

		--NOT GOING TO DataReturn 
		--AS IT IS A BACKWARDS REFERENCE

		--* create FK_DataReturn_DataReturnVersion
		ALTER TABLE [PCS].[DataReturn] 
			ADD CONSTRAINT [FK_DataReturn_DataReturnVersion] 
			FOREIGN KEY([CurrentDataReturnVersionId])
			REFERENCES [PCS].[DataReturnVersion] ([Id]);

		ALTER TABLE [PCS].[DataReturn] 
			CHECK CONSTRAINT [FK_DataReturn_DataReturnVersion];

		--* create FK_DataReturnUpload_DataReturnVersion
		ALTER TABLE [PCS].[DataReturnUpload] 
			ADD CONSTRAINT [FK_DataReturnUpload_DataReturnVersion] 
			FOREIGN KEY([DataReturnVersionId])
			REFERENCES [PCS].[DataReturnVersion] ([Id]);

		ALTER TABLE [PCS].[DataReturnUpload] 
			CHECK CONSTRAINT [FK_DataReturnUpload_DataReturnVersion];

		--* delete from DataReturn
		DELETE FROM
			[PCS].[DataReturn]
		WHERE
			SchemeId = @SchemeId
		AND
			ComplianceYear = @ComplianceYear;


		/*==================================================================*/
		/*	REGISTERED PRODUCER	*/
		/*==================================================================*/
		--* delete from EeeOutputReturnVersionAmount
		DELETE eorva
		FROM
			[PCS].[EeeOutputReturnVersionAmount] eorva
		INNER JOIN
			[PCS].[EeeOutputAmount] eoa
				ON eoa.Id = eorva.EeeOuputAmountId
		INNER JOIN
			[Producer].[RegisteredProducer] rp
				ON rp.Id = eoa.RegisteredProducerId
		WHERE
			rp.SchemeId = @SchemeId
		AND
			rp.ComplianceYear = @ComplianceYear;

		--* delete from EeeOutputAmount
		DELETE eoa
		FROM
			[PCS].[EeeOutputAmount] eoa
		INNER JOIN
			[Producer].[RegisteredProducer] rp
				ON rp.Id = eoa.RegisteredProducerId
		WHERE
			rp.SchemeId = @SchemeId
		AND
			rp.ComplianceYear = @ComplianceYear;

		--* drop FK_DirectProducerSubmission_CurrentSubmission
		ALTER TABLE [Producer].[DirectProducerSubmission] 
			DROP CONSTRAINT [FK_DirectProducerSubmission_CurrentSubmission];

		--* delete from DirectProducerSubmissionHistory
		DELETE dpsh
		FROM
			[Producer].[DirectProducerSubmissionHistory] dpsh
		INNER JOIN
			[Producer].[DirectProducerSubmission] dps
				ON dps.Id = dpsh.DirectProducerSubmissionId
		INNER JOIN
			[Producer].[RegisteredProducer] rp
				ON rp.Id = dps.RegisteredProducerId
		WHERE
			rp.SchemeId = @SchemeId
		AND
			rp.ComplianceYear = @ComplianceYear;

		--NOT GOING TO DirectProducerSubmission 
		--AS IT IS A BACKWARDS REFERENCE

		--* create FK_DirectProducerSubmission_CurrentSubmission
		ALTER TABLE [Producer].[DirectProducerSubmission] 
			ADD CONSTRAINT [FK_DirectProducerSubmission_CurrentSubmission] 
			FOREIGN KEY([CurrentSubmissionId])
			REFERENCES [Producer].[DirectProducerSubmissionHistory] ([Id]);

		ALTER TABLE [Producer].[DirectProducerSubmission] 
			CHECK CONSTRAINT [FK_DirectProducerSubmission_CurrentSubmission];

		--* drop FK_DirectProducerSubmission_FinalPaymentSessionId
		ALTER TABLE [Producer].[DirectProducerSubmission] 
			DROP CONSTRAINT [FK_DirectProducerSubmission_FinalPaymentSessionId];

		--* delete from PaymentSession
		DELETE ps
		FROM
			[Producer].[PaymentSession] ps
		INNER JOIN
			[Producer].[DirectProducerSubmission] dps
				ON dps.Id = ps.DirectProducerSubmissionId
		INNER JOIN
			[Producer].[RegisteredProducer] rp
				ON rp.Id = dps.RegisteredProducerId
		WHERE
			rp.SchemeId = @SchemeId
		AND
			rp.ComplianceYear = @ComplianceYear;

		--NOT GOING TO DirectProducerSubmission 
		--AS IT IS A BACKWARDS REFERENCE

		--* create FK_DirectProducerSubmission_FinalPaymentSessionId
		ALTER TABLE [Producer].[DirectProducerSubmission] 
			ADD CONSTRAINT [FK_DirectProducerSubmission_FinalPaymentSessionId] 
			FOREIGN KEY([FinalPaymentSessionId])
			REFERENCES [Producer].[PaymentSession] ([Id]);

		ALTER TABLE [Producer].[DirectProducerSubmission] 
			CHECK CONSTRAINT [FK_DirectProducerSubmission_FinalPaymentSessionId];

		--* delete from DirectProducerSubmission
		DELETE dps
		FROM
			[Producer].[DirectProducerSubmission] dps
		INNER JOIN
			[Producer].[RegisteredProducer] rp
				ON rp.Id = dps.RegisteredProducerId
		WHERE
			rp.SchemeId = @SchemeId
		AND
			rp.ComplianceYear = @ComplianceYear;

		--* drop FK_DirectProducerSubmission_CurrentSubmission
		ALTER TABLE [Producer].[DirectProducerSubmission] 
			DROP CONSTRAINT [FK_DirectProducerSubmission_CurrentSubmission];

		--* delete from DirectProducerSubmissionHistory
		DELETE dpsh
		FROM
			[Producer].[DirectProducerSubmissionHistory] dpsh
		INNER JOIN
			[Producer].[BrandName] bn
				ON bn.Id = dpsh.BrandNameId
		INNER JOIN
			[Producer].[ProducerSubmission] ps
				ON ps.Id = bn.ProducerSubmissionId
		INNER JOIN
			[Producer].[RegisteredProducer] rp
				ON rp.Id = ps.RegisteredProducerId
		WHERE
			rp.SchemeId = @SchemeId
		AND
			rp.ComplianceYear = @ComplianceYear;

		--* create FK_DirectProducerSubmission_CurrentSubmission
		ALTER TABLE [Producer].[DirectProducerSubmission] 
			ADD CONSTRAINT [FK_DirectProducerSubmission_CurrentSubmission] 
			FOREIGN KEY([CurrentSubmissionId])
			REFERENCES [Producer].[DirectProducerSubmissionHistory] ([Id]);

		ALTER TABLE [Producer].[DirectProducerSubmission] 
			CHECK CONSTRAINT [FK_DirectProducerSubmission_CurrentSubmission];

		--* delete from AdditionalCompanyDetails
		DELETE acd
		FROM
			[Organisation].[AdditionalCompanyDetails] acd
		INNER JOIN
			[Producer].[DirectRegistrant] dr
				ON dr.Id = acd.DirectRegistrantId
		INNER JOIN
			[Producer].[BrandName] bn
				ON bn.Id = dr.BrandNameId
		INNER JOIN
			[Producer].[ProducerSubmission] ps
				ON ps.Id = bn.ProducerSubmissionId
		INNER JOIN
			[Producer].[RegisteredProducer] rp
				ON rp.Id = ps.RegisteredProducerId
		WHERE
			rp.SchemeId = @SchemeId
		AND
			rp.ComplianceYear = @ComplianceYear;

		--* drop FK_PaymentSession_DirectRegistrantId
		ALTER TABLE [Producer].[PaymentSession] 
			DROP CONSTRAINT [FK_PaymentSession_DirectRegistrantId]

		--* delete from DirectRegistrant
		DELETE dr
		FROM
			[Producer].[DirectRegistrant] dr
		INNER JOIN
			[Producer].[BrandName] bn
				ON bn.Id = dr.BrandNameId
		INNER JOIN
			[Producer].[ProducerSubmission] ps
				ON ps.Id = bn.ProducerSubmissionId
		INNER JOIN
			[Producer].[RegisteredProducer] rp
				ON rp.Id = ps.RegisteredProducerId
		WHERE
			rp.SchemeId = @SchemeId
		AND
			rp.ComplianceYear = @ComplianceYear;

		/*---------------------------------------------------------------------------------*/
		--NOT GOING TO DIRECT PRODUCER SUBMISSION 
		--AS IT SHOULD BE COVERED BY REGISTERED PRODUCER
		/*---------------------------------------------------------------------------------*/

		--* create FK_PaymentSession_DirectRegistrantId
		ALTER TABLE [Producer].[PaymentSession] 
			ADD CONSTRAINT [FK_PaymentSession_DirectRegistrantId] 
			FOREIGN KEY([DirectRegistrantId])
			REFERENCES [Producer].[DirectRegistrant] ([Id])

		ALTER TABLE [Producer].[PaymentSession] 
			CHECK CONSTRAINT [FK_PaymentSession_DirectRegistrantId]

		--* delete from BrandName
		DELETE bn
		FROM
			[Producer].[BrandName] bn
		INNER JOIN
			[Producer].[ProducerSubmission] ps
				ON ps.Id = bn.ProducerSubmissionId
		INNER JOIN
			[Producer].[RegisteredProducer] rp
				ON rp.Id = ps.RegisteredProducerId
		WHERE
			rp.SchemeId = @SchemeId
		AND
			rp.ComplianceYear = @ComplianceYear;

		--* delete from SICCode
		DELETE sc
		FROM
			[Producer].[SICCode] sc
		INNER JOIN
			[Producer].[ProducerSubmission] ps
				ON ps.Id = sc.ProducerSubmissionId
		INNER JOIN
			[Producer].[RegisteredProducer] rp
				ON rp.Id = ps.RegisteredProducerId
		WHERE
			rp.SchemeId = @SchemeId
		AND
			rp.ComplianceYear = @ComplianceYear;

		--* drop FK_RegisteredProducer_ProducerSubmission
		ALTER TABLE [Producer].[RegisteredProducer] 
			DROP CONSTRAINT [FK_RegisteredProducer_ProducerSubmission];

		--* delete from ProducerSubmission
		DELETE ps
		FROM
			[Producer].[ProducerSubmission] ps
		INNER JOIN
			[Producer].[RegisteredProducer] rp
				ON rp.Id = ps.RegisteredProducerId
		WHERE
			rp.SchemeId = @SchemeId
		AND
			rp.ComplianceYear = @ComplianceYear;

		--NOT GOING TO RegisteredProducer 
		--AS IT IS A BACKWARDS REFERENCE

		--* create FK_RegisteredProducer_ProducerSubmission
		ALTER TABLE [Producer].[RegisteredProducer] 
			ADD CONSTRAINT [FK_RegisteredProducer_ProducerSubmission] 
			FOREIGN KEY([CurrentSubmissionId])
			REFERENCES [Producer].[ProducerSubmission] ([Id]);

		ALTER TABLE [Producer].[RegisteredProducer] 
			CHECK CONSTRAINT [FK_RegisteredProducer_ProducerSubmission];

		--* delete from RegisteredProducer
		DELETE FROM
			[Producer].[RegisteredProducer]
		WHERE
			SchemeId = @SchemeId
		AND
			ComplianceYear = @ComplianceYear;

		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION;

		RETURN -1;
	END CATCH

	RETURN 0;
END
