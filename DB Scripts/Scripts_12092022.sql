
ALTER PROCEDURE [dbo].[sp_GetSystemSetting] 
	@SettingType int
AS
BEGIN
	SET NOCOUNT ON;
	Declare @Code int,
			@RespStat int = 0,
			@RespMsg varchar(150) = '',
			@Data1 varchar(250) = '',
			@Data2 varchar(250) = '',
			@Data3 varchar(250) = '',
			@Data4 varchar(250) = '',
			@Data5 varchar(250) = '',
			@Data6 varchar(250) = '',
			@Data7 varchar(250) = '',
			@Data8 varchar(250)='',
			@Data9 varchar(250)='',
			@Data10 varchar(250)='',
			@PCode varchar(250)=''

	Declare	@TxnNoTable TABLE(TxnNo Varchar(20))

    If(@SettingType = 0)
	Begin
		Select 	@Data1 = Case ItemName when 'TAX_QUERY_URL' then ItemValue else @Data1 end,
				@Data2 = Case ItemName when 'OBR_USER_NAME' then ItemValue else @Data2 end,
				@Data3 = Case ItemName when 'OBR_USER_PASS' then ItemValue else @Data3 end,
				@Data4 = Case ItemName when 'OBR_GATEWAY_URL' then ItemValue else @Data4 end,
				@Data6 = Case ItemName when 'ENV_MODE' then ItemValue else @Data6 end,
				@Data7 = Case ItemName when 'DUMMY_TAX_FILE' then ItemValue else @Data7 end
		From SysSettings 
		Where ItemName in ('TAX_QUERY_URL','OBR_USER_NAME','OBR_USER_PASS','OBR_GATEWAY_URL', 'ENV_MODE','DUMMY_TAX_FILE')
	End
	Else If(@SettingType = 1)
	Begin
		Select 	@Data1 = Case ItemName when 'TAX_PAY_URL' then ItemValue else @Data1 end,
				@Data2 = Case ItemName when 'OBR_USER_NAME' then ItemValue else @Data2 end,
				@Data3 = Case ItemName when 'OBR_USER_PASS' then ItemValue else @Data3 end,
				@Data4 = Case ItemName when 'OBR_GATEWAY_URL' then ItemValue else @Data4 end,
				@Data6 = Case ItemName when 'ENV_MODE' then ItemValue else @Data6 end
				
		From SysSettings 
		Where ItemName in ('TAX_PAY_URL','OBR_USER_NAME','OBR_USER_PASS','OBR_GATEWAY_URL')
	End

	Else If(@SettingType = 2)
	Begin
		Select 	@Data1 = Case ItemName when 'PayWayBuyTokenUname' then ItemValue else @Data1 end,
				@Data2 = Case ItemName when 'PayWayBuyTokenPass' then ItemValue else @Data2 end,
				@Data3 = Case ItemName when 'AppId' then ItemValue else @Data3 end,
				@Data4 = Case ItemName when 'AppToken' then ItemValue else @Data4 end,
				@Data5 = Case ItemName when 'BuyTokenProvider' then ItemValue else @Data5 end,
				@Data7 = Case ItemName when 'PaywayUrl' then ItemValue else @Data7 end,
				@Data9= Case ItemName when 'PaywayBuyTokenMin' then ItemValue else @Data9 end,
				@Data10= Case ItemName when 'PaywayBuyTokenMax' then ItemValue else @Data10 end,
				@Data6 = Case ItemName when 'ENV_MODE' then ItemValue else @Data6 end
		From SysSettings 
		Where ItemName in ('PayWayBuyTokenUname','PayWayBuyTokenPass','AppId','AppToken','BuyTokenProvider','PaywayUrl','PaywayBuyTokenMin','PaywayBuyTokenMax')
		
		INSERT INTO @TxnNoTable
		Exec sp_GenerateTxnNo 
		Select @PCode = TxnNo From @TxnNoTable
		set @Data8=@PCode
	End
	Else If(@SettingType = 3)
	Begin
		Select 	@Data1 = Case ItemName when 'PayWayRetailerTopUpUname' then ItemValue else @Data1 end,
				@Data2 = Case ItemName when 'PayWayRetailerTopUpPass' then ItemValue else @Data2 end,
				@Data3 = Case ItemName when 'AppId' then ItemValue else @Data3 end,
				@Data4 = Case ItemName when 'AppToken' then ItemValue else @Data4 end,
				@Data5 = Case ItemName when 'TopUpProvider' then ItemValue else @Data5 end,
				@Data7 = Case ItemName when 'PaywayUrl' then ItemValue else @Data7 end,
				@Data9= Case ItemName when 'PaywayTopUpMin' then ItemValue else @Data9 end,
				@Data10= Case ItemName when 'PaywayTopUpMax' then ItemValue else @Data10 end,
				@Data6 = Case ItemName when 'ENV_MODE' then ItemValue else @Data6 end
		From SysSettings 
		Where ItemName in ('PayWayRetailerTopUpUname','PayWayRetailerTopUpPass','TopUpProvider','AppId','AppToken','BuyTokenProvider','PaywayUrl','PaywayTopUpMin','PaywayTopUpMax')
		INSERT INTO @TxnNoTable
		Exec sp_GenerateTxnNo 
		Select @PCode = TxnNo From @TxnNoTable
		set @Data8=@PCode
	End
	Else If(@SettingType = 4)
	Begin
		Select 	
				@Data1 = Case ItemName when 'DOMESTIC_TAX_URL' then ItemValue else @Data1 end,
				@Data2 = Case ItemName when 'DOMESTIC_TAX_Uname' then ItemValue else @Data2 end,
				@Data3 = Case ItemName when 'DOMESTIC_TAX_Pass' then ItemValue else @Data3 end,
				@Data4 = Case ItemName when 'OBR_GATEWAY_URL' then ItemValue else @Data4 end,
				@Data5 = Case ItemName when 'DOM_BANK_CODE' then ItemValue else @Data5 end
		From SysSettings 
		Where ItemName in ('DOMESTIC_TAX_URL','DOMESTIC_TAX_Uname','DOMESTIC_TAX_Pass','OBR_GATEWAY_URL','DOM_BANK_CODE')
		
	End
	Else If(@SettingType = 6)
	Begin
		Select 	@Data1 = Case ItemName when 'FINBRIDGE_BASE_URL' then ItemValue else @Data1 end,
				@Data2 = Case ItemName when 'FINBRIDGE_APPID' then ItemValue else @Data2 end,
				@Data3 = Case ItemName when 'FINBRIDGE_APPKEY' then ItemValue else @Data3 end
		From SysSettings 
		Where ItemName in ('FINBRIDGE_BASE_URL','FINBRIDGE_APPID','FINBRIDGE_APPKEY')
		
	End
	Else If(@SettingType =8)
	Begin
		Select 	@Data2 = Case ItemName when 'ORACLE_USER' then ItemValue else @Data2 end,
				@Data3 = Case ItemName when 'ORACLE_PASS' then ItemValue else @Data3 end,
				@Data4 = Case ItemName when 'ORACLE_HOST' then ItemValue else @Data4 end,
				@Data5 = Case ItemName when 'ORACLE_PORT' then ItemValue else @Data5 end,
				@Data6 = Case ItemName when 'ORACLE_SID' then ItemValue else @Data6 end,
				@Data7 = Case ItemName when 'ORACLE_SCHEMA' then ItemValue else @Data7 end
		From SysSettings 
		Where ItemName in ('ORACLE_USER','ORACLE_PASS','ORACLE_HOST','ORACLE_PORT','ORACLE_SID','ORACLE_SCHEMA')

		Set @Data1='Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST='+@Data4+')(PORT='+@Data5+')))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME='+@Data6+')));User ID='++@Data2+';Password='+@Data3+''
	End
	Else
	Begin
		Select  1 as RespStatus, 'Invalid setting type!' as RespMessage
		Return
	End

	Select  0 as RespStatus, 
			'' as RespMessage, 
			@Data1 as Data1, 
			@Data2 As Data2,
			@Data3 as Data3,
			@Data4 as Data4,
			@Data5 as Data5,
			Coalesce(@Data6,'0') as Data6,
			@Data7 as Data7,
			@Data8 as Data8,
			@Data9 as Data9,
			@Data10 as Data10
END
Go

Insert Into SysSettings(ItemName, ItemValue, Descr) Values('FINBRIDGE_BASE_URL','http://154.117.212.146/FinBridge/api/v1','FinBridge base url')
Go

-- =============================================
-- Author:		Alex Mugo
-- Create date: 13/09/2022
-- Description:	A stored procedure to create Miarie tax record
-- =============================================
CREATE PROCEDURE sp_CreateMiarieFile
	@UserCode int,
	@TaxAmount decimal(18,2),
	@NoteNo varchar(20),
	@NoteType int,
	@Payer varchar(150),
	@Period varchar(50),
	@RefNo varchar(50),
	@Descr varchar(150)
AS
BEGIN
	SET NOCOUNT ON;
	Declare @Code int,
			@RespStat int = 0,
			@RespMsg varchar(250) = '',
			@Charge decimal(18,2) = 0

    BEGIN TRY
		---- Validate
		If Not Exists(Select Id From Users Where UserCode = @UserCode)
		Begin
			Select  1 as RespStatus, 'Invalid user details!' as RespMessage
			Return
		End

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
			Period
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
			@Period
		)

		---- Get charge

		---- Create response
		Select  @RespStat as RespStatus, 
				@RespMsg as RespMessage, 
				@Code as Data1, 
				@Charge as Data2
	END TRY
	BEGIN CATCH
		SELECT @RespMsg = ERROR_MESSAGE(), @RespStat = 2;
		Select  @RespStat as RespStatus, @RespMsg as RespMessage
	END CATCH
END
Go

-- =============================================
-- Author:		Author Unknown! Modified by Alex Mugo
-- Create date: 14/09/2022
-- Description:	A stored procedure to validate Miarie tax payment
-- =============================================
ALTER PROCEDURE [dbo].[sp_ValidateMiariePayment]
    @FileCode int,
    @Amount decimal(18,2),
    @ModeCode int,
    @AccountNo varchar(20),
    @ChequeNo varchar(20),
    @NoCharge bit,
    @SortCode varchar(10)
AS
BEGIN
    SET NOCOUNT ON;
    Declare @Code int,
            @RespStat int = 0,
            @RespMsg varchar(150) = '',
            @TaxAmount decimal(18,2),
            @ExpectedAmount decimal(18,2),
            @ChargeAmount decimal(18,2) = 0,
            @PayMode varchar(50),
            @CustName varchar(50),
            @CrAccountName varchar(50) = 'MIARIE TAX',
            @DefaultCharge varchar(15) = '0',
            @BalUrl varchar(200),
            @ChequeValidUrl varchar(200)

    BEGIN TRY
        ---- Validate
		Select @TaxAmount = Amount From MiarieTaxFiles Where FileCode = @FileCode
        If (@TaxAmount Is Null)
        Begin
            Select  1 as RespStatus, 'Invalid tax file details!' as RespMessage
            Return
        End

        ---- Validate if the cheque is already used for clearing cheque Extra2 column is used to store the sortcode
        If(Coalesce(@SortCode,'') <> '')
        Begin
			If Exists(Select Id From MiariePayments where Extra1 = @ChequeNo and Extra2 = @SortCode)
			Begin
				Select  1 as RespStatus, 'This cheque: '+ @ChequeNo +' has already been used to make payment!!!' as RespMessage
				Return
			End
        End
        
        Select @PayMode = ModeName From PaymentModes Where ModeCode = @ModeCode
        If(@PayMode Is Null)
        Begin
            Select  1 as RespStatus, 'Invalid payment mode!' as RespMessage
            Return
        End

        ---- Get charge
        Select  @BalUrl =(CASE WHEN a.ItemName = 'CBS_BALANCE_URL' THEN a.ItemValue ELSE @BalUrl END),                  
                @ChequeValidUrl =(CASE WHEN a.ItemName = 'CBS_CHQ_VALID_URL' THEN a.ItemValue ELSE @ChequeValidUrl END)
        From SysSettings a Where ItemName  In('CBS_BALANCE_URL','CBS_CHQ_VALID_URL')

        Set @ExpectedAmount = @TaxAmount + @ChargeAmount

        ---- Create response
        Select  @RespStat as RespStatus, 
                @RespMsg as RespMessage, 
                @PayMode as Data1, 
                @CrAccountName as Data2,
                @CustName as Data3,
                @BalUrl as Data4,
                @ExpectedAmount as Data5,
                @ChequeValidUrl as Data6,
				@Amount as Data7
    END TRY
    BEGIN CATCH
        SELECT @RespMsg = ERROR_MESSAGE(), @RespStat = 2;
        Select  @RespStat as RespStatus, @RespMsg as RespMessage
    END CATCH
END
Go

-- =============================================
-- Author:		Author Unknown! Modified by Alex Mugo
-- Create date: 14/09/2022
-- Description:	A stored procedure to create Miarie tax payment
-- =============================================
ALTER PROCEDURE [dbo].[sp_MakeMiariePayment] 
	@FileCode int,
	@UserCode int,
	@Amount decimal(18,2),
	@ModeCode int,
	@Remarks varchar(150),
	@Extra1 varchar(150),
	@Extra2 varchar(150),
	@Extra3 varchar(150),
	@Extra4 varchar(150),
	@Dr_Account varchar(20),
	@ApplyCharge bit
AS
BEGIN
	SET NOCOUNT ON;
	Declare @Code int,
			@RespStat int = 0,
			@RespMsg varchar(150) = '',
			@ChargeCode int,
			@TaxAmount decimal(18,2),
			@CrAccount varchar(20) = '',
			@ChargeCrAccount varchar(20) = '',
			@PostUrl varchar(150) ='',
			@BalUrl varchar(150) ='',
			@Currency varchar(10) ='BIF',
			@DefaultTxnCode varchar(10) ='',
			@CBSTxnCode varchar(10) ='',
			@TxnType varchar(10) ='',
			@Officer varchar(10) ='',
			@DDName varchar(35) ='',
			@Narration varchar(150) ='',
			@CashAccount varchar(20),
			@ChargeTxnCode varchar(10) ='',
			@ChargeTxnType varchar(10) ='',
			@ChargeNarration varchar(150) ='',
			@DefaultCharge varchar(20) ='0',
			@Approval int,
			@ModeId int,
			@GL_Account varchar(20),
			@StatusCode int = 0,
			@TxnCharge decimal(18,2),
			@branchCode int,
			@Data1 varchar(250) = '',
			@Data2 varchar(250) = '',
			@Data3 varchar(250) = '',
			@Data4 varchar(250) = '',
			@Data5 varchar(250) = '',
			@Data6 varchar(250) = ''

    BEGIN TRY
		---- Validate
		If Not Exists(Select Id From Users Where UserCode = @UserCode)
		Begin
			Select  1 as RespStatus, 'Invalid user details!' as RespMessage
			Return
		End
		If Not Exists(Select Id From MiarieTaxFiles Where FileCode = @FileCode)
		Begin
			Select  1 as RespStatus, 'Invalid tax file details!' as RespMessage
			Return
		End
		-- Validate sortcode
		If(Coalesce(@Extra2, '') <> '')
		Begin
			If Not Exists(Select Id From Banks Where BankCode = @Extra2)
			Begin
				Select  1 as RespStatus, 'Invalid Bank!' as RespMessage
				Return
			End
		End

		Select	@ModeId = Id, 
				@Approval = Approval, 
				@GL_Account = GL_Account,
				@TxnCharge = Charge, 
				@CBSTxnCode = CBS_Txn_Code 
		From PaymentModes Where ModeCode = @ModeCode
		If (@ModeId Is Null)
		Begin
			Select  1 as RespStatus, 'Invalid payment mode!' as RespMessage
			Return
		End
		---- Set auto-approval items
		If(@Approval = 0) 
			Set @StatusCode = 1

		--- Get settings
		Select	@CashAccount =(CASE WHEN a.ItemName = 'CASH_DR_ACCOUNT' THEN a.ItemValue ELSE @CashAccount END ),
				@CrAccount =(CASE WHEN a.ItemName = 'TAX_CR_ACCOUNT' THEN a.ItemValue ELSE @CrAccount END),
				@PostUrl =(CASE WHEN a.ItemName = 'CBS_POST_URL' THEN a.ItemValue ELSE @PostUrl END),
				@Officer =(CASE WHEN a.ItemName = 'CBS_OFFICER' THEN a.ItemValue ELSE @Officer END),
				@DefaultTxnCode =(CASE WHEN a.ItemName = 'CBS_TXN_CODE' THEN a.ItemValue ELSE @DefaultTxnCode END),
				@TxnType =(CASE WHEN a.ItemName = 'CBS_TXN_TYPE' THEN a.ItemValue ELSE @TxnType END),
				@DDName =(CASE WHEN a.ItemName = 'CBS_TNCTR_NAME' THEN a.ItemValue ELSE @DDName END),
				@Narration =(CASE WHEN a.ItemName = 'CBS_NARRATION' THEN a.ItemValue ELSE @Narration END),
				@DefaultCharge =(CASE WHEN a.ItemName = 'TAX_CHARGE_AMOUNT' THEN a.ItemValue ELSE @DefaultCharge END),
				@Data1 = Case ItemName when 'FINBRIDGE_BASE_URL' then ItemValue else @Data1 end,
				@Data2 = Case ItemName when 'FINBRIDGE_APPID' then ItemValue else @Data2 end,
				@Data3 = Case ItemName when 'FINBRIDGE_APPKEY' then ItemValue else @Data3 end
		From SysSettings a 
		Where a.ItemName In('CASH_DR_ACCOUNT','TAX_CR_ACCOUNT','CBS_POST_URL','CBS_OFFICER','CBS_TXN_CODE','CBS_TXN_TYPE',
		'CBS_TNCTR_NAME','CBS_NARRATION','TAX_CHARGE_AMOUNT','FINBRIDGE_BASE_URL','FINBRIDGE_APPID','FINBRIDGE_APPKEY')

		If(@ModeCode = 0)
		Begin
			--- Get user cash account
			Select @CashAccount = a.Cash_Account From Users a Inner Join Branches b on a.BranchCode = b.BranchCode Where UserCode = @UserCode
			Set @CashAccount = Coalesce(@CashAccount,'')
			Set @Dr_Account = @CashAccount

			---- Validate account
			If(Len(@Dr_Account) = 0)
			Begin
				Select  1 as RespStatus, 'User cash account is not set!' as RespMessage
				Return
			End
		End

		---- Validate GL account for certified cheque
		If(@ModeCode = 3)
		Begin
			Set @Dr_Account = Coalesce(@GL_Account,'')
			If(Len(@Dr_Account) = 0)
			Begin
				Select  1 as RespStatus, 'GL account for certified cheque NOT set!' as RespMessage
				Return
			End
		End

		---- Validate GL account for clearing cheque
		If(@ModeCode = 4)
		Begin
			Set @Dr_Account = Coalesce(@GL_Account,'')
			If(Len(@Dr_Account) = 0)
			Begin
				Select  1 as RespStatus, 'GL account for certified cheque NOT set!' as RespMessage
				Return
			End
		End

		--- Process charge
		If(@TxnCharge = 0)
			Set @TxnCharge = Cast(@DefaultCharge as Decimal(18,2))

		Set @TaxAmount = @Amount
		If(@ApplyCharge = 1)
			Set @TaxAmount = @Amount - @TxnCharge
		else
		Set @TxnCharge  =0;

		--- Generate Item code
		Declare	@RunNoTable TABLE(RunNo Varchar(20))
		INSERT INTO @RunNoTable	Exec sp_GenerateRunNo 'PYT1' 
		Select @Code = RunNo From @RunNoTable

		--- Create record now
		Insert Into MiariePayments(
			PaymentCode,
			FileCode,
			ReceiptNo,
			CreateDate,
			UserCode,
			Amount,
			StatusCode,
			ModeCode,
			Remarks,
			Extra1,
			Extra2,
			Extra3,
			Extra4,
			Dr_Account,
			Cr_Account,
			ApplyCharge
		)
		Values(
			@Code,
			@FileCode,
			'MTX' + Cast(@Code as Varchar(15)),
			GETDATE(),
			@UserCode,
			@TaxAmount,
			@StatusCode,
			@ModeCode,
			@Remarks,
			@Extra1,
			@Extra2,
			@Extra3,
			@Extra4,
			@Dr_Account,
			@CrAccount,
			@ApplyCharge
		)

		--- Create tax commission
		If(@ApplyCharge = 1)
		Begin
			--- Get charge settings
			Select	@BalUrl =(CASE WHEN a.ItemName = 'CBS_BALANCE_URL' THEN a.ItemValue ELSE @BalUrl END),					
					@ChargeCrAccount =(CASE WHEN a.ItemName = 'CHARGE_CR_ACCOUNT' THEN a.ItemValue ELSE @ChargeCrAccount END),
					@ChargeTxnCode =(CASE WHEN a.ItemName = 'CHARGE_CBS_TXN_CODE' THEN a.ItemValue ELSE @ChargeTxnCode END),
					@ChargeTxnType =(CASE WHEN a.ItemName = 'CHARGE_CBS_TXN_TYPE' THEN a.ItemValue ELSE @ChargeTxnType END),
					@ChargeNarration =(CASE WHEN a.ItemName = 'CHARGE_CBS_NARRATION' THEN a.ItemValue ELSE @ChargeNarration END)
			From SysSettings a 
			Where a.ItemName In('CBS_BALANCE_URL','CHARGE_CR_ACCOUNT','CHARGE_CBS_TXN_CODE','CHARGE_CBS_TXN_TYPE','CHARGE_CBS_NARRATION')

			--- Generate code
			Delete From @RunNoTable
			INSERT INTO @RunNoTable
			Exec sp_GenerateRunNo 'CHG1' 
			Select @ChargeCode = RunNo From @RunNoTable

			Insert Into MiarieTaxCharges(
				ChargeCode,
				PaymentCode,
				Amount,
				DrAccount,
				CrAccount,
				ChargeStatus,
				PostAttempts
			)
			Values(
				@ChargeCode,
				@Code,
				@TxnCharge,
				@Dr_Account,
				@ChargeCrAccount,
				0,
				0
			)

		End	
		
		Set @CBSTxnCode = Coalesce(@CBSTxnCode, @DefaultTxnCode)
		Select @branchCode= CBS_Code From Branches where BranchCode=(select BranchCode from Users where UserCode=@UserCode)

		---- Create response
		Select  @RespStat as RespStatus, 
				@RespMsg as RespMessage, 
				@Code as PaymentCode,
				@ModeCode as PaymentMode,
				@ApplyCharge as ApplyCharge,
				@PostUrl as PostUrl,
				@BalUrl as BalanceUrl,
				'BIF' as CurrencyCode,
				@TaxAmount as MainAmount,
				@CBSTxnCode as MainTxnCode,
				@TxnType as MainTxnType,
				@Officer as CBSOfficer,
				@DDName as TransactorName,
				'MTX' + Cast(@Code as Varchar(15)) as MainRefNo,
				'0' as MainFlag,
				@Dr_Account as MainDrAccount,
				@CrAccount as MainCrAccount,
				Coalesce(@Remarks,@Narration) as MainNarration,
				@ChargeTxnCode as ChargeTxnCode,
				@ChargeTxnType as ChargeTxnType,
				'MTX' + Cast(@Code as Varchar(15)) + '/' + Cast(@ChargeCode as Varchar(15)) as ChargeRefNo,
				'0' as ChargeFlag,
				@Dr_Account as ChargeDrAccount,
				@ChargeCrAccount as ChargeCrAccount,
				@ChargeNarration as ChargeNarration,
				@TxnCharge as ChargeAmount,
				@ChargeCode as ChargeCode,
				@Approval as ApprovalNeeded,
				@Extra1 as ChequeNo,
				@branchCode as BrachCode,
				@Data1 as Data1,
				@Data2 as Data2,
				@Data3 as Data3,
				@Data4 as Data4,
				@Data5 as Data5,
				@Data6 as Data6
	END TRY
	BEGIN CATCH
		SELECT @RespMsg = ERROR_MESSAGE(), @RespStat = 2;
		Select  @RespStat as RespStatus, @RespMsg as RespMessage
	END CATCH
END
Go

-- =============================================
-- Author:		Alex Mugo
-- Create date: 15/09/2022
-- Description:	A stored procedure to update Miarie tax payment
-- =============================================
Create PROCEDURE sp_UpdateMiarieFileStatus
	@FileCode int,
	@Stat int,
	@Msg varchar(250)
AS
BEGIN
	SET NOCOUNT ON;
	Declare @Code int,
			@RespStat int = 0,
			@RespMsg varchar(250) = '',
			@Attempts int,
			@FileStat int,
			@MaxAttempts int = 5,
			@PayType int

    BEGIN TRY
		---- Validate
		Select @Attempts = NotifAttempts, @FileStat = StatusCode,@PayType = PayType From TaxFiles Where FileCode = @FileCode
		If(@FileStat Is Null)
		Begin
			Select  1 as RespStatus, 'Invalid tax file!' as RespMessage
			Return
		End

		If(@FileStat = 5)
		Begin
			If(@Stat = 0)
			Begin
				--- Tax file processing complete
				Update MiarieTaxFiles Set StatusCode = 6 Where FileCode = @FileCode

			End
			Else
			Begin
				--- Tax file notification failed, make 5 attempts and marks it as failed
				Set @Attempts = @Attempts + 1
				If(@Attempts > @MaxAttempts)
				Begin
					--- Update file notification failed
					Update TaxFiles Set StatusCode = 7, StatusMsg = @Msg Where FileCode = @FileCode
				End
			End
		End
		Else
		Begin
			Select  1 as RespStatus, 'Invalid tax file status!' as RespMessage
			Return
		End

		---- Create response
		Select  @RespStat as RespStatus, 
				@RespMsg as RespMessage
	END TRY
	BEGIN CATCH
		SELECT @RespMsg = ERROR_MESSAGE(), @RespStat = 2;
		Select  @RespStat as RespStatus, @RespMsg as RespMessage
	END CATCH
END
Go

