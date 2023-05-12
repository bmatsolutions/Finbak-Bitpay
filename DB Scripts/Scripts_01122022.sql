-- =============================================
-- Author:		Alex Mugo
-- Create date: 13/09/2022
-- Description:	A stored procedure to create Miarie tax record
-- =============================================
ALTER PROCEDURE [dbo].[sp_CreateMiarieFile]
	@UserCode int,
	@TaxAmount decimal(18,2),
	@NoteNo varchar(20),
	@NoteType int,
	@Payer varchar(150),
	@Period varchar(50),
	@RefNo varchar(50),
	@Descr varchar(150),
	@Title varchar(250),
	@TypeName varchar(250)
AS
BEGIN
	SET NOCOUNT ON;
	Declare @Code int,
			@RespStat int = 0,
			@RespMsg varchar(250) = '',
			@Charge decimal(18,2) = 0,
			@PayPartial bit = 0,
			@PostToCBS varchar(10) = ''

    BEGIN TRY
		---- Validate
		If Not Exists(Select Id From Users Where UserCode = @UserCode)
		Begin
			Select  1 as RespStatus, 'Invalid user details!' as RespMessage
			Return
		End

		--- Check if exists and not complete
		If Exists(Select Id From MiarieTaxFiles Where NoteNo = @NoteNo and StatusCode In(5,7,9))
		Begin
			Select  1 as RespStatus, 'Transaction with a similar reference was processed but not completed!' as RespMessage
			Return
		End

		--- Check partial payment
		If(Coalesce(@TypeName,'') In ('foncier','activites-professionnelles'))
			Set @PayPartial = 1

		--- Generate Item code
		Declare	@RunNoTable TABLE(RunNo Varchar(20))
		INSERT INTO @RunNoTable Exec sp_GenerateRunNo 'TXF1' 
		Select @Code = RunNo From @RunNoTable

		--- Create record now
		Insert Into MiarieTaxFiles(
			FileCode,
			CreateDate,
			UserCode,
			StatusCode,
			Amount,
			NoteType,
			NoteNo,
			RefNo,
			PayerName,
			Descr,
			Period,
			Title,
			PayPartial,
			TypeName
		)
		Values(
			@Code,
			GETDATE(),
			@UserCode,
			0,
			@TaxAmount,
			@NoteType,
			@NoteNo,
			@RefNo,
			@Payer,
			@Descr,
			@Period,
			@Title,
			@PayPartial,
			@TypeName
		)

		---- Get settings
		Select @PostToCBS = ItemValue From SysSettings where ItemName = 'CBS_UPLOAD'

		---- Create response
		Select  @RespStat as RespStatus, 
				@RespMsg as RespMessage, 
				@Code as Data1, 
				@Charge as Data2,
				@PayPartial as Data3,
				@PostToCBS as Data4
	END TRY
	BEGIN CATCH
		SELECT @RespMsg = ERROR_MESSAGE(), @RespStat = 2;
		Select  @RespStat as RespStatus, @RespMsg as RespMessage
	END CATCH
END
Go
