SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- Create date: 2026 Aug 28
-- Modified date: 2026 Sep 11
-- Description:	This stored procedure deletes data from multiple sources
--				which match a given SchemeId and ComplianceYear
-- =============================================
CREATE OR ALTER     PROCEDURE [PCS].[spgRemovePCSRecords]
	@SchemeId		UNIQUEIDENTIFIER,
    @ComplianceYear INT
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		BEGIN TRANSACTION;

		PRINT '@SchemeId = ' + CAST(@SchemeId AS NVARCHAR(40));
		PRINT '@ComplianceYear = ' + CAST(@ComplianceYear AS NVARCHAR(10));
		PRINT '';

		/*==================================================================*/
		/*	OBLIGATION SCHEME	*/
		/*==================================================================*/
		PRINT 'delete from ObligationSchemeAmount';
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
		PRINT 'Rows deleted: ' + CAST(@@ROWCOUNT AS VARCHAR(10));
		PRINT '';

		PRINT 'delete from ObligationScheme';
		DELETE 
		FROM
			[PCS].[ObligationScheme]
		WHERE
			SchemeId = @SchemeId
		AND
			ComplianceYear = @ComplianceYear;
		PRINT 'Rows deleted: ' + CAST(@@ROWCOUNT AS VARCHAR(10));
		PRINT '';


		/*==================================================================*/
		/*	MEMBER UPLOAD	*/
		/*==================================================================*/
		PRINT 'delete from AdditionalCompanyDetails';
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
		PRINT 'Rows deleted: ' + CAST(@@ROWCOUNT AS VARCHAR(10));
		PRINT '';

		PRINT 'drop FK_PaymentSession_DirectRegistrantId';
		ALTER TABLE [Producer].[PaymentSession] 
			DROP CONSTRAINT [FK_PaymentSession_DirectRegistrantId]
		PRINT '';

		PRINT 'delete from DirectRegistrant';
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
		PRINT 'Rows deleted: ' + CAST(@@ROWCOUNT AS VARCHAR(10));
		PRINT '';

		/*---------------------------------------------------------------------------------*/
		--NOT GOING TO DIRECT PRODUCER SUBMISSION 
		--AS IT SHOULD BE COVERED BY REGISTERED PRODUCER
		/*---------------------------------------------------------------------------------*/

		PRINT 'create FK_PaymentSession_DirectRegistrantId';
		ALTER TABLE [Producer].[PaymentSession] WITH NOCHECK
			ADD CONSTRAINT [FK_PaymentSession_DirectRegistrantId] 
			FOREIGN KEY([DirectRegistrantId])
			REFERENCES [Producer].[DirectRegistrant] ([Id])

		PRINT 'CHECK FK_PaymentSession_DirectRegistrantId';
		ALTER TABLE [Producer].[PaymentSession] 
			CHECK CONSTRAINT [FK_PaymentSession_DirectRegistrantId]
		PRINT '';

		PRINT 'drop FK_DirectProducerSubmission_CurrentSubmission';
		ALTER TABLE [Producer].[DirectProducerSubmission] 
			DROP CONSTRAINT [FK_DirectProducerSubmission_CurrentSubmission];
		PRINT '';

		PRINT 'delete from DirectProducerSubmissionHistory';
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
		PRINT 'Rows deleted: ' + CAST(@@ROWCOUNT AS VARCHAR(10));
		PRINT '';
	
		/*---------------------------------------------------------------------------------*/
		--NOT GOING TO DIRECT PRODUCER SUBMISSION 
		--AS IT SHOULD BE COVERED BY REGISTERED PRODUCER
		/*---------------------------------------------------------------------------------*/

		PRINT 'create FK_DirectProducerSubmission_CurrentSubmission';
		ALTER TABLE [Producer].[DirectProducerSubmission] WITH NOCHECK
			ADD CONSTRAINT [FK_DirectProducerSubmission_CurrentSubmission] 
			FOREIGN KEY([CurrentSubmissionId])
			REFERENCES [Producer].[DirectProducerSubmissionHistory] ([Id]);

		PRINT 'CHECK FK_DirectProducerSubmission_CurrentSubmission';
		ALTER TABLE [Producer].[DirectProducerSubmission] 
			CHECK CONSTRAINT [FK_DirectProducerSubmission_CurrentSubmission];
		PRINT '';

		PRINT 'delete from BrandName';
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
		PRINT 'Rows deleted: ' + CAST(@@ROWCOUNT AS VARCHAR(10));
		PRINT '';

		PRINT 'delete from SICCode';
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
		PRINT 'Rows deleted: ' + CAST(@@ROWCOUNT AS VARCHAR(10));
		PRINT '';

		PRINT 'drop FK_RegisteredProducer_ProducerSubmission';
		ALTER TABLE [Producer].[RegisteredProducer] 
			DROP CONSTRAINT [FK_RegisteredProducer_ProducerSubmission];
		PRINT '';

		PRINT 'delete from ProducerSubmission';
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
		PRINT 'Rows deleted: ' + CAST(@@ROWCOUNT AS VARCHAR(10));
		PRINT '';
	
		/*---------------------------------------------------------------------------------*/
		--NOT GOING TO REGISTERED PRODUCER 
		--AS IT HAS ITS OWN SCHEMEID AND COMPLIANCEYEAR
		/*---------------------------------------------------------------------------------*/

		PRINT 'create FK_RegisteredProducer_ProducerSubmission';
		ALTER TABLE [Producer].[RegisteredProducer] WITH NOCHECK
			ADD	CONSTRAINT [FK_RegisteredProducer_ProducerSubmission] 
			FOREIGN KEY([CurrentSubmissionId])
			REFERENCES [Producer].[ProducerSubmission] ([Id]);

		PRINT 'CHECK FK_RegisteredProducer_ProducerSubmission'
		ALTER TABLE [Producer].[RegisteredProducer] 
			CHECK CONSTRAINT [FK_RegisteredProducer_ProducerSubmission];
		PRINT '';

		PRINT 'delete from MemberUploadError';
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
		PRINT 'Rows deleted: ' + CAST(@@ROWCOUNT AS VARCHAR(10));

		PRINT 'delete from MemberUpload';
		DELETE FROM
			[PCS].[MemberUpload]
		WHERE
			SchemeId = @SchemeId
		AND
			ComplianceYear = @ComplianceYear;
		PRINT 'Rows deleted: ' + CAST(@@ROWCOUNT AS VARCHAR(10));
		PRINT '';


		/*==================================================================*/
		/*	DATA RETURN UPLOAD	*/
		/*==================================================================*/
		PRINT 'delete from DataReturnUploadError';
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
		PRINT 'Rows deleted: ' + CAST(@@ROWCOUNT AS VARCHAR(10));
		PRINT '';

		PRINT 'delete from DataReturnUpload';
		DELETE FROM
			[PCS].[DataReturnUpload]
		WHERE
			SchemeId = @SchemeId
		AND
			ComplianceYear = @ComplianceYear;
		PRINT 'Rows deleted: ' + CAST(@@ROWCOUNT AS VARCHAR(10));
		PRINT '';


		/*==================================================================*/
		/*	DATA RETURN	*/
		/*==================================================================*/
		PRINT 'drop FK_DataReturnUpload_DataReturnVersion';
		ALTER TABLE [PCS].[DataReturnUpload] 
			DROP CONSTRAINT [FK_DataReturnUpload_DataReturnVersion];

		PRINT 'drop FK_DataReturn_DataReturnVersion';
		ALTER TABLE [PCS].[DataReturn] 
			DROP CONSTRAINT [FK_DataReturn_DataReturnVersion];
		PRINT '';

		PRINT 'delete from DataReturnVersion';
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
		PRINT 'Rows deleted: ' + CAST(@@ROWCOUNT AS VARCHAR(10));
		PRINT '';

		/*---------------------------------------------------------------------------------*/
		--NOT GOING TO DataReturnUpload 
		--AS IT HAS ITS OWN SCHEMEID AND COMPLIANCEYEAR
		/*---------------------------------------------------------------------------------*/

		/*---------------------------------------------------------------------------------*/
		--NOT GOING TO DataReturn 
		--AS IT IS A BACKWARDS REFERENCE
		/*---------------------------------------------------------------------------------*/

		PRINT 'create FK_DataReturn_DataReturnVersion';
		ALTER TABLE [PCS].[DataReturn] WITH NOCHECK
			ADD CONSTRAINT [FK_DataReturn_DataReturnVersion] 
			FOREIGN KEY([CurrentDataReturnVersionId])
			REFERENCES [PCS].[DataReturnVersion] ([Id]);

		PRINT 'CHECK FK_DataReturn_DataReturnVersion';
		ALTER TABLE [PCS].[DataReturn] 
			CHECK CONSTRAINT [FK_DataReturn_DataReturnVersion];
		PRINT '';

		PRINT 'create FK_DataReturnUpload_DataReturnVersion';
		ALTER TABLE [PCS].[DataReturnUpload] WITH NOCHECK
			ADD CONSTRAINT [FK_DataReturnUpload_DataReturnVersion] 
			FOREIGN KEY([DataReturnVersionId])
			REFERENCES [PCS].[DataReturnVersion] ([Id]);

		PRINT 'CHECK FK_DataReturnUpload_DataReturnVersion';
		ALTER TABLE [PCS].[DataReturnUpload] 
			CHECK CONSTRAINT [FK_DataReturnUpload_DataReturnVersion];
		PRINT '';

		PRINT 'delete from DataReturn';
		DELETE FROM
			[PCS].[DataReturn]
		WHERE
			SchemeId = @SchemeId
		AND
			ComplianceYear = @ComplianceYear;
		PRINT 'Rows deleted: ' + CAST(@@ROWCOUNT AS VARCHAR(10));
		PRINT '';


		/*==================================================================*/
		/*	REGISTERED PRODUCER	*/
		/*==================================================================*/
		PRINT 'delete from EeeOutputReturnVersionAmount';
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
		PRINT 'Rows deleted: ' + CAST(@@ROWCOUNT AS VARCHAR(10));
		PRINT '';

		PRINT 'delete from EeeOutputAmount';
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
		PRINT 'Rows deleted: ' + CAST(@@ROWCOUNT AS VARCHAR(10));
		PRINT '';

		PRINT 'drop FK_DirectProducerSubmission_CurrentSubmission';
		ALTER TABLE [Producer].[DirectProducerSubmission] 
			DROP CONSTRAINT [FK_DirectProducerSubmission_CurrentSubmission];
		PRINT '';

		PRINT 'delete from DirectProducerSubmissionHistory';
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
		PRINT 'Rows deleted: ' + CAST(@@ROWCOUNT AS VARCHAR(10));
		PRINT '';

		--NOT GOING TO DirectProducerSubmission 
		--AS IT IS A BACKWARDS REFERENCE

		PRINT 'create FK_DirectProducerSubmission_CurrentSubmission';
		ALTER TABLE [Producer].[DirectProducerSubmission] WITH NOCHECK
			ADD CONSTRAINT [FK_DirectProducerSubmission_CurrentSubmission] 
			FOREIGN KEY([CurrentSubmissionId])
			REFERENCES [Producer].[DirectProducerSubmissionHistory] ([Id]);

		PRINT 'CHECK FK_DirectProducerSubmission_CurrentSubmission';
		ALTER TABLE [Producer].[DirectProducerSubmission] 
			CHECK CONSTRAINT [FK_DirectProducerSubmission_CurrentSubmission];
		PRINT '';

		PRINT 'drop FK_DirectProducerSubmission_FinalPaymentSessionId';
		ALTER TABLE [Producer].[DirectProducerSubmission] 
			DROP CONSTRAINT [FK_DirectProducerSubmission_FinalPaymentSessionId];
		PRINT '';

		PRINT 'delete from PaymentSession';
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
		PRINT 'Rows deleted: ' + CAST(@@ROWCOUNT AS VARCHAR(10));
		PRINT '';

		--NOT GOING TO DirectProducerSubmission 
		--AS IT IS A BACKWARDS REFERENCE

		PRINT 'create FK_DirectProducerSubmission_FinalPaymentSessionId';
		ALTER TABLE [Producer].[DirectProducerSubmission] WITH NOCHECK
			ADD CONSTRAINT [FK_DirectProducerSubmission_FinalPaymentSessionId] 
			FOREIGN KEY([FinalPaymentSessionId])
			REFERENCES [Producer].[PaymentSession] ([Id]);

		PRINT 'CHECK FK_DirectProducerSubmission_FinalPaymentSessionId';
		ALTER TABLE [Producer].[DirectProducerSubmission] 
			CHECK CONSTRAINT [FK_DirectProducerSubmission_FinalPaymentSessionId];
		PRINT '';

		PRINT 'delete from DirectProducerSubmission';
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
		PRINT 'Rows deleted: ' + CAST(@@ROWCOUNT AS VARCHAR(10));
		PRINT '';

		PRINT 'drop FK_DirectProducerSubmission_CurrentSubmission';
		ALTER TABLE [Producer].[DirectProducerSubmission] 
			DROP CONSTRAINT [FK_DirectProducerSubmission_CurrentSubmission];
		PRINT '';

		PRINT 'delete from DirectProducerSubmissionHistory';
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
		PRINT 'Rows deleted: ' + CAST(@@ROWCOUNT AS VARCHAR(10));
		PRINT '';

		PRINT 'create FK_DirectProducerSubmission_CurrentSubmission';
		ALTER TABLE [Producer].[DirectProducerSubmission] WITH NOCHECK
			ADD CONSTRAINT [FK_DirectProducerSubmission_CurrentSubmission] 
			FOREIGN KEY([CurrentSubmissionId])
			REFERENCES [Producer].[DirectProducerSubmissionHistory] ([Id]);

		PRINT 'CHECK FK_DirectProducerSubmission_CurrentSubmission';
		ALTER TABLE [Producer].[DirectProducerSubmission] 
			CHECK CONSTRAINT [FK_DirectProducerSubmission_CurrentSubmission];
		PRINT '';

		PRINT 'delete from AdditionalCompanyDetails';
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
		PRINT 'Rows deleted: ' + CAST(@@ROWCOUNT AS VARCHAR(10));
		PRINT '';

		PRINT 'drop FK_PaymentSession_DirectRegistrantId';
		ALTER TABLE [Producer].[PaymentSession] 
			DROP CONSTRAINT [FK_PaymentSession_DirectRegistrantId]
		PRINT '';

		PRINT 'delete from DirectRegistrant';
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
		PRINT 'Rows deleted: ' + CAST(@@ROWCOUNT AS VARCHAR(10));
		PRINT '';

		/*---------------------------------------------------------------------------------*/
		--NOT GOING TO DIRECT PRODUCER SUBMISSION 
		--AS IT SHOULD BE COVERED BY REGISTERED PRODUCER
		/*---------------------------------------------------------------------------------*/

		PRINT 'create FK_PaymentSession_DirectRegistrantId';
		ALTER TABLE [Producer].[PaymentSession] WITH NOCHECK
			ADD CONSTRAINT [FK_PaymentSession_DirectRegistrantId] 
			FOREIGN KEY([DirectRegistrantId])
			REFERENCES [Producer].[DirectRegistrant] ([Id])

		PRINT 'CHECK FK_PaymentSession_DirectRegistrantId';
		ALTER TABLE [Producer].[PaymentSession] 
			CHECK CONSTRAINT [FK_PaymentSession_DirectRegistrantId]
		PRINT '';

		PRINT 'delete from BrandName';
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
		PRINT 'Rows deleted: ' + CAST(@@ROWCOUNT AS VARCHAR(10));
		PRINT '';

		PRINT 'delete from SICCode';
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
		PRINT 'Rows deleted: ' + CAST(@@ROWCOUNT AS VARCHAR(10));
		PRINT '';

		PRINT 'drop FK_RegisteredProducer_ProducerSubmission';
		ALTER TABLE [Producer].[RegisteredProducer] 
			DROP CONSTRAINT [FK_RegisteredProducer_ProducerSubmission];
		PRINT '';

		PRINT 'delete from ProducerSubmission';
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
		PRINT 'Rows deleted: ' + CAST(@@ROWCOUNT AS VARCHAR(10));
		PRINT '';

		/*---------------------------------------------------------------------------------*/
		--NOT GOING TO RegisteredProducer 
		--AS IT IS A BACKWARDS REFERENCE
		/*---------------------------------------------------------------------------------*/

		PRINT 'create FK_RegisteredProducer_ProducerSubmission';
		ALTER TABLE [Producer].[RegisteredProducer] WITH NOCHECK
			ADD CONSTRAINT [FK_RegisteredProducer_ProducerSubmission] 
			FOREIGN KEY([CurrentSubmissionId])
			REFERENCES [Producer].[ProducerSubmission] ([Id]);

		PRINT 'CHECK FK_RegisteredProducer_ProducerSubmission';
		ALTER TABLE [Producer].[RegisteredProducer] 
			CHECK CONSTRAINT [FK_RegisteredProducer_ProducerSubmission];
		PRINT '';

		PRINT 'delete from RegisteredProducer';
		DELETE FROM
			[Producer].[RegisteredProducer]
		WHERE
			SchemeId = @SchemeId
		AND
			ComplianceYear = @ComplianceYear;
		PRINT 'Rows deleted: ' + CAST(@@ROWCOUNT AS VARCHAR(10));
		PRINT '';

		--ROLLBACK TRANSACTION;
		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION;

		PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS VARCHAR(10));
		PRINT 'Error Message: ' + ERROR_MESSAGE();
		PRINT 'Error Line: ' + CAST(ERROR_LINE() AS VARCHAR(10));

		RETURN -1;
	END CATCH

	RETURN 0;
END
GO
