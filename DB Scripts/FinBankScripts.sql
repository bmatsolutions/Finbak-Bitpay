USE BITPAY

If Not Exists(Select * From SysSettings Where ItemName = 'BANK_NAME')
Insert into SysSettings(ItemName,ItemValue,Descr,Editable) values('BANK_NAME','FINBANK SPRL','Bank Name',1)
If Not Exists(Select * From SysSettings Where ItemName = 'BANK')
Insert into SysSettings(ItemName,ItemValue,Descr,Editable) values('BANK','FINBANK SA','Bank Name',1)
If Not Exists(Select * From SysSettings Where ItemName = 'CBS_UPLOAD')
Insert into SysSettings(ItemName,ItemValue,Descr,Editable) values('CBS_UPLOAD','1','Posting to CBS 0 - Manual,1 - System Upload',1)
If Not Exists(Select * From SysSettings Where ItemName = 'BANK_CODE')
Insert into SysSettings(ItemName,ItemValue,Descr,Editable) values('BANK_CODE','FLB','Bank Code',1)

if exists(select name from sys.objects where name = N'sp_GetSystemSetting')
	begin
		drop PROCEDURE dbo.sp_GetSystemSetting;
	end
Go

Create PROCEDURE [dbo].[sp_GetSystemSetting] 
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
	Declare	@TxnNoTable TABLE	
		(
			TxnNo Varchar(20) 
		)
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
				@Data4 = Case ItemName when 'OBR_GATEWAY_URL' then ItemValue else @Data4 end
		From SysSettings 
		Where ItemName in ('DOMESTIC_TAX_URL','DOMESTIC_TAX_Uname','DOMESTIC_TAX_Pass','OBR_GATEWAY_URL')
		
	End
	Else If(@SettingType = 6)
	Begin
		Select 	
				@Data1 = Case ItemName when 'FINBRIDGE_AUTH_URL' then ItemValue else @Data1 end,
				@Data2 = Case ItemName when 'FINBRIDGE_MiARIE_URL' then ItemValue else @Data2 end,
				@Data3 = Case ItemName when 'FINBRIDGE_APPID' then ItemValue else @Data3 end,
				@Data4 = Case ItemName when 'FINBRIDGE_APPKEY' then ItemValue else @Data4 end
		From SysSettings 
		Where ItemName in ('FINBRIDGE_AUTH_URL','FINBRIDGE_MiARIE_URL','FINBRIDGE_APPID','FINBRIDGE_APPKEY')
		
	End
	Else If(@SettingType =8)
	Begin
		Select 	
				@Data2 = Case ItemName when 'ORACLE_USER' then ItemValue else @Data2 end,
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
			coalesce(@Data6,'0') as Data6,
			@Data7 as Data7,
			@Data8 as Data8,
			@Data9 as Data9,
			@Data10 as Data10
END

Go

if exists(select name from sys.objects where name = N'sp_UserLogin')
	begin
		drop PROCEDURE dbo.sp_UserLogin;
	end
Go

-- =============================================
-- Author:		Alex Mugo
-- Create date: 01/06/2018
-- Description:	A stored procedure to update store user login attempt details
-- =============================================
CREATE PROCEDURE [dbo].[sp_UserLogin]
	@UserCode int,
	@LoginStatus  int
AS
BEGIN
	SET NOCOUNT ON;
	Declare @Code int,
			@RespStat int = 0,
			@RespMsg varchar(150) = '',
			@UserId int,
			@FullNames varchar(50),
			@Attempts int,
			@UserStatus int = 0,
			@ChangePwd bit = 0,
			@UserRole int,
			@Title varchar(30),
			@Bank varchar(30),
			@PostToCBS int=-1,
			@branchCode int=0

    BEGIN TRY
		---- Get user details
		Select	@UserId = Id, 
				@FullNames = FullNames, 
				@Attempts = Attempts, 
				@ChangePwd = ChangePwd,
				@UserRole = UserRole,
				@UserStatus = UserStatus,
				@branchCode = BranchCode
		From Users Where UserCode = @UserCode

		Select @Title =(CASE WHEN a.ItemName = 'BANK_NAME' THEN a.ItemValue ELSE @Title END),
				@Bank =(CASE WHEN a.ItemName = 'BANK' THEN a.ItemValue ELSE @Bank END),
				@PostToCBS=(CASE WHEN a.ItemName = 'CBS_UPLOAD' THEN a.ItemValue ELSE @PostToCBS END)
		From SysSettings a Where a.ItemName In('BANK_NAME','BANK','CBS_UPLOAD')


		If(@UserId Is Null)
		Begin
			Select  1 as RespStatus, 'Invalid user details!' as RespMessage
			Return
		End

		If(@LoginStatus = 0)
		Begin
			---- Update user details
			Update Users Set LastLogin = GETDATE(), Attempts = 0 Where UserCode = @UserCode
		End
		Else
		Begin
			Set @Attempts = @Attempts + 1
			If(@Attempts > 5)
				Set @UserStatus = 2

			Update Users Set Attempts = @Attempts, UserStatus = @UserStatus Where UserCode = @UserCode
		End

		Select  @RespStat as RespStatus, 
				@RespMsg as RespMessage, 
				@UserId as Data1, 
				@FullNames as Data2,
				@ChangePwd as Data3,
				@UserRole as Data4,
				@Title as Data5,
				@Bank as Data6,
				@PostToCBS as Data7,
				@branchCode as Data8


	END TRY
	BEGIN CATCH
		SELECT @RespMsg = ERROR_MESSAGE(), @RespStat = 2;
		Select  @RespStat as RespStatus, @RespMsg as RespMessage
	END CATCH
END
GO
if exists(select name from sys.objects where name = N'sp_CreateTaxFile')
	begin
		drop PROCEDURE dbo.sp_CreateTaxFile;
	end
Go

-- =============================================
-- Author:		Alex Mugo
-- Create date: 02/06/2018
-- Description:	A stored procedure to create a tax file record
-- =============================================
CREATE PROCEDURE [dbo].[sp_CreateTaxFile]
	@UserCode int,
	@TaxAmount decimal(18,2),
	@OfficeCode varchar(20),
	@OfficeName varchar(50),
	@DclntCode varchar(20),
	@DclntName varchar(50),
	@CompanyCode varchar(20),
	@CompanyName varchar(150),	
	@AsmtSerial varchar(20),
	@AsmtNumber varchar(20),
	@RegYear int,
	@RegSerial varchar(20),
	@RegNumber varchar(20),
	@AccHolder varchar(150),
	@PayerName varchar(150),
	@TranCode varchar(5),
	@TranRef varchar(150),
	@Currency varchar(1),
	@PayType int,
	@Extra1 varchar(150),
	@Extra2 varchar(150),
	@Extra3 varchar(150),
	@Extra4 varchar(150)
AS
BEGIN
	SET NOCOUNT ON;
	Declare @Code int,
			@RespStat int = 0,
			@RespMsg varchar(150) = '',
			@Charge varchar(10) = '0',
			@PostToCBS varchar(10)=''

    BEGIN TRY
		---- Validate
		If Not Exists(Select Id From Users Where UserCode = @UserCode)
		Begin
			Select  1 as RespStatus, 'Invalid user details!' as RespMessage
			Return
		End

		--- Generate Item code
		Declare	@RunNoTable TABLE	
		(
			RunNo Varchar(20) 
		)
		INSERT INTO @RunNoTable
		Exec sp_GenerateRunNo 'TXF1' 
		Select @Code = RunNo From @RunNoTable

		--- Create record now
		Insert Into TaxFiles(
			FileCode,
			CreateDate,
			UserCode,
			StatusCode,
			TaxAmount,
			OfficeCode,
			OfficeName,
			DclntCode,
			DclntName,
			CompanyCode,
			CompanyName,
			AsmtSerial,
			RegYear,
			AsmtNumber,
			Extra1,
			Extra2,
			Extra3,
			Extra4,
			RegSerial,
			RegNumber,
			AccountHolder,
			PayType,
			PayerName,
			TransactionCode,
			TransactionRef,
			Currency
		)
		Values(
			@Code,
			GETDATE(),
			@UserCode,
			0,
			@TaxAmount,
			@OfficeCode,
			@OfficeName,
			@DclntCode,
			@DclntName,
			@CompanyCode,
			@CompanyName,
			Coalesce(@AsmtSerial,''),
			@RegYear,
			Coalesce(@AsmtNumber,''),
			@Extra1,
			@Extra2,
			@Extra3,
			@Extra4,
			Coalesce(@RegSerial,''),
			Coalesce(@RegNumber,''),
			@AccHolder,
			@PayType,
			@PayerName,
			@TranCode,
			@TranRef,
			Coalesce(@Currency,'1')
		)

		---- Get charge
		If(@Currency = '1')
		Select @Charge = Charge From PaymentModes Where ModeCode = 0
		Else
		Select @Charge=ItemValue from SysSettings where ItemName='TAX_CHARGE_USD'

		Set @Charge = Coalesce(@Charge, '0')
		Select @PostToCBS=ItemValue from SysSettings where ItemName='CBS_UPLOAD'

		---- Create response
		Select  @RespStat as RespStatus, @RespMsg as RespMessage, @Code as Data1, @Charge as Data2,@PostToCBS as Data3

	END TRY
	BEGIN CATCH
		SELECT @RespMsg = ERROR_MESSAGE(), @RespStat = 2;
		Select  @RespStat as RespStatus, @RespMsg as RespMessage
	END CATCH
END
GO

if exists(select name from sys.objects where name = N'sp_ValidateTaxPayment')
	begin
		drop PROCEDURE dbo.sp_ValidateTaxPayment;
	end
Go

CREATE PROCEDURE [dbo].[sp_ValidateTaxPayment]
    @FileCode int,
    @Amount decimal(18,2),
    @ModeCode int,
    @AccountNo varchar(20),
    @ChequeNo varchar(20),
    @NoCharge bit,
    @SortCode varchar(10),
	@PostToCBS int,
	@CBSRef varchar(150)

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
            @CrAccountName varchar(50) = 'OBR TAX',
            @DefaultCharge varchar(15),
            @BalUrl varchar(200),
            @ChequeValidUrl varchar(200),
			@TaxAcc varchar(50),
			@Attempts int

    BEGIN TRY
        ---- Validate
		---Check posting reference
		If (@PostToCBS = 0)
		Begin
			Select @Attempts=b.StatusCode from Payments a Inner Join TaxFiles b on b.FileCode=a.FileCode where a.Extra3=@CBSRef 
			If (@Attempts = 6)
			Begin
				Select  1 as RespStatus, @CBSRef+' has already been used to make payment!!!' as RespMessage
				Return
			End
			Select @TaxAcc=ItemValue from SysSettings where ItemName='TAX_CR_ACCOUNT'
		End
		Else If(@PostToCBS=1)
		Begin
			--validate if the cheque is already used for clearing cheque Extra2 column is used to store the sortcode
			if((@SortCode!=null or @SortCode!=''))
			Begin
			if Exists(select Id from Payments where Extra1=@ChequeNo and Extra2=@SortCode)
			Begin
			Select  1 as RespStatus, 'This cheque: '+@ChequeNo +' has already been used to make payment!!!' as RespMessage
				Return
			End
			End
			Select @TaxAmount = TaxAmount From TaxFiles Where FileCode = @FileCode
			If (@TaxAmount Is Null)
			Begin
				Select  1 as RespStatus, 'Invalid tax file details!' as RespMessage
				Return
			End
			Select @PayMode = ModeName, @ChargeAmount = Charge From PaymentModes Where ModeCode = @ModeCode
			If(@PayMode Is Null)
			Begin
				Select  1 as RespStatus, 'Invalid payment mode!' as RespMessage
				Return
			End

			If(@NoCharge = 1)
				Set @ExpectedAmount = @TaxAmount 
			Else
			Begin
				---- Get charge
				Select  @BalUrl =(CASE WHEN a.ItemName = 'CBS_BALANCE_URL' THEN a.ItemValue ELSE @BalUrl END),                  
						--@DefaultCharge =(CASE WHEN a.ItemName = 'TAX_CHARGE_AMOUNT' THEN a.ItemValue ELSE @DefaultCharge END),
						@ChequeValidUrl =(CASE WHEN a.ItemName = 'CBS_CHQ_VALID_URL' THEN a.ItemValue ELSE @ChequeValidUrl END)
				From SysSettings a Where ItemName  in('TAX_CHARGE_AMOUNT','CBS_BALANCE_URL','CBS_CHQ_VALID_URL')

            
				select @DefaultCharge= p.Charge from PaymentModes p where p.ModeCode=@ModeCode;
				Set @ChargeAmount = Coalesce(@DefaultCharge,  Cast((Coalesce(@DefaultCharge, '0')) as Decimal(18,2)));
				Set @ExpectedAmount = @TaxAmount + @ChargeAmount
			End

			If(@ExpectedAmount <> @Amount)
			Begin
				Select  1 as RespStatus, 'Invalid amount! Expected amount is: ' + Cast(@ExpectedAmount as Varchar(20)) as RespMessage
				Return
			End

			--If(@ModeCode = 1)
			--Begin
			--  Select @CustName = AccountName From CustAccounts Where AccountNo = @AccountNo
			--  If (@CustName Is Null)
			--  Begin
			--      Select  1 as RespStatus, 'Invalid customer account!' as RespMessage
			--      Return
			--  End
			--End
		End
        ---- Create response
        Select  @RespStat as RespStatus, 
                @RespMsg as RespMessage, 
                @PayMode as Data1, 
                @CrAccountName as Data2,
                @CustName as Data3,
                @BalUrl as Data4,
                @ExpectedAmount as Data5,
                @ChequeValidUrl as Data6,
				@TaxAcc as Data7
    END TRY
    BEGIN CATCH
        SELECT @RespMsg = ERROR_MESSAGE(), @RespStat = 2;
        Select  @RespStat as RespStatus, @RespMsg as RespMessage
    END CATCH
END
GO

if exists(select name from sys.objects where name = N'sp_CreateDomesticTaxFile')
	begin
		drop PROCEDURE dbo.sp_CreateDomesticTaxFile;
	end
Go

-- =============================================
-- Author:		Brian Ogenge
-- Create date: 13/04/2020
-- Description:	A stored procedure to create a domestic tax file record
-- =============================================
CREATE PROCEDURE [dbo].[sp_CreateDomesticTaxFile]
	@UserCode int,
	@Amount decimal(18,2),
	@TransactionCode varchar(50),
	@TransactionType varchar(10),
	@CustomerName varchar(150),
	@TaxPeriod varchar(50),
	@DeclNo varchar(50),	
	@Tin varchar(20),
	@CommuneName varchar(150),
	@Delay varchar(50),
	@Adjustment varchar(50),
	@TaxAdjustment varchar(50),
	@Service varchar(50),
	@Chasis varchar(50),
	@Imma varchar(50),
	@CarOnwer varchar(50),
	@Contracavation varchar(50),
	@Document varchar(150),
	@DriverName varchar(150),
	@Education varchar(50),
	@Infraction varchar(50),
	@LicenceType varchar(50),
	@Copies  varchar(10),
	@Vehicle varchar(50),
	@Word varchar(50)
AS
BEGIN
	SET NOCOUNT ON;
	Declare @Code int,
			@RespStat int = 0,
			@RespMsg varchar(150) = '',
			@Charge varchar(10) = '0',
			@PostToCBS varchar(10)='',
			@TaxName varchar(250)=''

    BEGIN TRY
		---- Validate
		If Not Exists(Select Id From Users Where UserCode = @UserCode)
		Begin
			Select  1 as RespStatus, 'Invalid user details!' as RespMessage
			Return
		End

		--- Generate Item code
		Declare	@RunNoTable TABLE	
		(
			RunNo Varchar(20) 
		)
		INSERT INTO @RunNoTable
		Exec sp_GenerateRunNo 'TXF1' 
		Select @Code = RunNo From @RunNoTable

		--- Create record now
		Insert Into DomesticTaxFiles(
			FileCode,
			CreateDate,
			TransactionCode,
			TransactionType,
			UserCode,
			StatusCode,
			Amount,
			CustomerName,
			TaxPeriod,
			DeclNo,
			Tin,
			CommuneName,
			Delay,
			Adjustment,
			TaxAdjustment,
			Service,
			Chasis,
			Imma,
			CarOnwer,
			Contracavation,
			Document,
			DriverName,
			Education,
			Infraction,
			LicenceType,
			Copies,
			Vehicle,
			Word
		)
		Values(
			@Code,
			GETDATE(),
			@TransactionCode,
			@TransactionType,
			@UserCode,
			0,
			@Amount,
			@CustomerName,
			@TaxPeriod,
			@DeclNo,
			@Tin,
			@CommuneName,
			@Delay,
			@Adjustment,
			@TaxAdjustment,
			@Service,
			@Chasis,
			@Imma,
			@CarOnwer,
			@Contracavation,
			@Document,
			@DriverName,
			@Education,
			@Infraction,
			@LicenceType,
			@Copies,
			@Vehicle,
			@Word
		)

		---- Get charge
		Select @Charge = Charge From PaymentModes Where ModeCode = 0
		Set @Charge = Coalesce(@Charge, '0')
		Select @PostToCBS=ItemValue from SysSettings where ItemName='CBS_UPLOAD'
		Select @TaxName =dc.CatergoryName From DomesticTaxFiles df Inner Join DomesticTaxCatergory dc on dc.CatergoryCode=df.TransactionType Where df.FileCode = @Code
       
		---- Create response
		Select  @RespStat as RespStatus, @RespMsg as RespMessage, @Code as Data1, @Charge as Data2,@PostToCBS as Data3,@TaxName as Data4

	END TRY
	BEGIN CATCH
		SELECT @RespMsg = ERROR_MESSAGE(), @RespStat = 2;
		Select  @RespStat as RespStatus, @RespMsg as RespMessage
	END CATCH
END
GO

if exists(select name from sys.objects where name = N'sp_ValidateDomesticTaxPayment')
	begin
		drop PROCEDURE dbo.sp_ValidateDomesticTaxPayment;
	end
Go

CREATE PROCEDURE [dbo].[sp_ValidateDomesticTaxPayment]
    @FileCode int,
    @Amount decimal(18,2),
    @ModeCode int,
    @AccountNo varchar(20),
    @ChequeNo varchar(20),
    @NoCharge bit,
    @SortCode varchar(10),
	@PostToCBS int,
	@CBSRef varchar(150)
AS
BEGIN
    SET NOCOUNT ON;
    Declare @Code int,
            @RespStat int = 0,
            @RespMsg varchar(150) = '',
			@TaxName varchar(100)='',
            @TaxAmount decimal(18,2),
            @ExpectedAmount decimal(18,2),
            @ChargeAmount decimal(18,2) = 0,
            @PayMode varchar(50),
            @CustName varchar(50),
            @CrAccountName varchar(50) = 'OBR TAX',
            @DefaultCharge varchar(15),
            @BalUrl varchar(200),
			@ApprovalNeeded int,
			@TaxAcc varchar(50),
			@Attempts int,
            @ChequeValidUrl varchar(200)

    BEGIN TRY
        ---- Validate
		---Check posting reference
		If (@PostToCBS = 0)
		Begin
			Select @Attempts=b.StatusCode from DomesticTaxPayments a Inner Join TaxFiles b on b.FileCode=a.FileCode where a.Extra3=@CBSRef 
			If (@Attempts = 5)
			Begin
				Select  1 as RespStatus, @CBSRef+' has already been used to make payment!!!' as RespMessage
				Return
			End
			Select @TaxAcc=ItemValue from SysSettings where ItemName='DOMESTICTAX_CR_ACCOUNT'

		End
		Else If(@PostToCBS=1)
		Begin
			--validate if the cheque is already used for clearing cheque Extra2 column is used to store the sortcode
			if((@SortCode!=null or @SortCode!=''))
			Begin
			if Exists(select Id from DomesticTaxPayments where Extra1=@ChequeNo and Extra2=@SortCode)
			Begin
			Select  1 as RespStatus, 'This cheque: '+@ChequeNo +' has already been used to make payment!!!' as RespMessage
				Return
			End
			End
		
			Select @TaxAmount = df.Amount,@TaxName =dc.CatergoryName From DomesticTaxFiles df Inner Join DomesticTaxCatergory dc on dc.CatergoryCode=df.TransactionType Where df.FileCode = @FileCode
			If (@TaxAmount Is Null)
			Begin
				Select  1 as RespStatus, 'Invalid domestic tax file details!' as RespMessage
				Return
			End
			Select @PayMode = ModeName, @ChargeAmount = Charge From PaymentModes Where ModeCode = @ModeCode
			If(@PayMode Is Null)
			Begin
				Select  1 as RespStatus, 'Invalid payment mode!' as RespMessage
				Return
			End

			If(@NoCharge = 1)
			Begin
				---- Get charge
				Select  @BalUrl =(CASE WHEN a.ItemName = 'CBS_BALANCE_URL' THEN a.ItemValue ELSE @BalUrl END),                  
						--@DefaultCharge =(CASE WHEN a.ItemName = 'TAX_CHARGE_AMOUNT' THEN a.ItemValue ELSE @DefaultCharge END),
						@ChequeValidUrl =(CASE WHEN a.ItemName = 'CBS_CHQ_VALID_URL' THEN a.ItemValue ELSE @ChequeValidUrl END)
				From SysSettings a Where ItemName  in('TAX_CHARGE_AMOUNT','CBS_BALANCE_URL','CBS_CHQ_VALID_URL')
				Set @ExpectedAmount = @TaxAmount 
			End
			Else
			Begin
				---- Get charge
				Select  @BalUrl =(CASE WHEN a.ItemName = 'CBS_BALANCE_URL' THEN a.ItemValue ELSE @BalUrl END),                  
						--@DefaultCharge =(CASE WHEN a.ItemName = 'TAX_CHARGE_AMOUNT' THEN a.ItemValue ELSE @DefaultCharge END),
						@ChequeValidUrl =(CASE WHEN a.ItemName = 'CBS_CHQ_VALID_URL' THEN a.ItemValue ELSE @ChequeValidUrl END)
				From SysSettings a Where ItemName  in('TAX_CHARGE_AMOUNT','CBS_BALANCE_URL','CBS_CHQ_VALID_URL')

				select @DefaultCharge= p.Charge from PaymentModes p where p.ModeCode=@ModeCode;
				Set @ChargeAmount = Coalesce(@DefaultCharge,  Cast((Coalesce(@DefaultCharge, '0')) as Decimal(18,2)));
				Set @ExpectedAmount = @TaxAmount + @ChargeAmount
			End

			If(@ExpectedAmount <> @Amount)
			Begin
				Select  1 as RespStatus, 'Invalid amount! Expected amount is: ' + Cast(@ExpectedAmount as Varchar(20)) as RespMessage
				Return
			End
			Select @ApprovalNeeded = StatusCode From DomesticTaxPayments Where FileCode = @FileCode
			 If(@PayMode Is Null)
			Begin
				Select  1 as RespStatus, 'Payment created awaiting approval' as RespMessage
				Return
			End

			--If(@ModeCode = 1)
			--Begin
			--  Select @CustName = AccountName From CustAccounts Where AccountNo = @AccountNo
			--  If (@CustName Is Null)
			--  Begin
			--      Select  1 as RespStatus, 'Invalid customer account!' as RespMessage
			--      Return
			--  End
			--End
		End
        ---- Create response
        Select  @RespStat as RespStatus, 
                @RespMsg as RespMessage, 
                @PayMode as Data1, 
                @CrAccountName as Data2,
                @CustName as Data3,
                @BalUrl as Data4,
                @ExpectedAmount as Data5,
                @ChequeValidUrl as Data6,
				@TaxName as Data7,
				@TaxAcc as Data8
    END TRY
    BEGIN CATCH
        SELECT @RespMsg = ERROR_MESSAGE(), @RespStat = 2;
        Select  @RespStat as RespStatus, @RespMsg as RespMessage
    END CATCH
END
GO


if exists(select name from sys.objects where name = N'vw_GetOBRPendingPayments')
	begin
		drop VIEW dbo.vw_GetOBRPendingPayments;
	end
Go

CREATE view [dbo].[vw_GetOBRPendingPayments]
As
Select  
	a.FileCode,
	a.AsmtNumber as AssessmentNumber,
	a.AsmtSerial as AssessmentSerial,
	a.OfficeCode as OfficeCode,
	a.RegNumber as RegistrationNumber,
	a.RegSerial as RegistrationSerial,
	a.RegYear as RegistrationYear,
	a.DclntCode as DeclarantCode,
	a.CompanyCode as CompanyCode,
	a.TaxAmount as AmountToBePaid,
	11 as MeanOfPayment,
	'FLB' as BankCode,
	'' as CheckReference,
	a.TaxAmount as Amount,
	CONVERT(varchar(15),b.CreateDate,103) as PaymentDate,
	'false' as Refund
From TaxFiles a
Inner Join Payments b on a.FileCode = b.FileCode
Where a.StatusCode = 5 and a.FileCode not in(select filecode from TempTaxDets_Arch) 

GO


if exists(select name from sys.objects where name = N'CustomTax')
	begin
		drop Table dbo.CustomTax;
	end
Go

CREATE TABLE [dbo].[CustomTax](
	[TaxNo] [int] NOT NULL,
	[TaxName] [varchar](250) NOT NULL,
	[TaxCode] [int] NULL DEFAULT ((0)),
 CONSTRAINT [PK_CustomTax] PRIMARY KEY CLUSTERED 
(
	[TaxNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
INSERT [dbo].[CustomTax] ([TaxNo], [TaxName], [TaxCode]) VALUES (1, N'Paiement des déclarations au comptant', 0)
INSERT [dbo].[CustomTax] ([TaxNo], [TaxName], [TaxCode]) VALUES (2, N'Paiement des déclarations à crédit', 0)
INSERT [dbo].[CustomTax] ([TaxNo], [TaxName], [TaxCode]) VALUES (3, N'Paiement par Bordereau de crédit', 0)
INSERT [dbo].[CustomTax] ([TaxNo], [TaxName], [TaxCode]) VALUES (4, N'Paiement des autres taxes et accises', 0)


if exists(select name from sys.objects where name = N'sp_GetListModel')
	begin
		drop Procedure dbo.sp_GetListModel;
	end
Go

CREATE PROCEDURE [dbo].[sp_GetListModel]
	@Type int
AS
BEGIN
	SET NOCOUNT ON;

    If(@Type = 0)
		Select a.ModeName as Text, a.ModeCode as Value From PaymentModes a
	Else if(@Type = 1)
		Select a.StatusName as Text, a.StatusCode as Value From FileStatus a
	Else if(@Type = 2)
		Select a.BranchName as Text, a.BranchCode as Value From Branches a
	Else if(@Type = 3)
		Select a.UserName as Text, a.UserCode as Value From Users a
	Else if(@Type = 4)
		Select b.BankName as Text, b.BankCode as Value From Banks b
	Else if(@Type = 5)
         Select  o.OfficeName as Text,o.obrofficeCode as Value From ObrOffices o
    Else if(@Type = 6)
         Select  o.ItemName as Text,o.ItemValue as Value From SysSettings o where o.Descr='Report'
	Else if(@Type = 7)
         Select a.ModeName as Text, a.ModeCode as Value From PaymentModes a where ModeCode !=4
    Else if(@Type = 8)
		Select a.TaxName as Text,a.TaxNo as Value from DomesticTax a
	Else if(@Type = 9)
		Select a.MiarieName as Text,a.MiarieNo as Value from MiarieTax a
	Else if(@Type = 10)
		Select a.TaxName as Text,a.TaxNo as Value from CustomTax a where a.TaxCode=0
	Else if(@Type = 11)
		Select a.TaxName as Text,a.TaxNo as Value from CustomTax a where a.TaxCode=4
END

GO

If not exists(select * from sys.columns where name=N'AccountHolder' and object_id=object_id(N'dbo.TaxFiles'))
	Alter Table TaxFiles add  AccountHolder varchar(150) null
If not exists(select * from sys.columns where name=N'PayType' and object_id=object_id(N'dbo.TaxFiles'))
	Alter Table TaxFiles add  AccountHolder int default(1) not null
If not exists(select * from sys.columns where name=N'PayerName' and object_id=object_id(N'dbo.TaxFiles'))
	Alter Table TaxFiles add  PayerName varchar(150) null
If not exists(select * from sys.columns where name=N'TransactionCode' and object_id=object_id(N'dbo.TaxFiles'))
	Alter Table TaxFiles add  TransactionCode varchar(5) null
If not exists(select * from sys.columns where name=N'TransactionRef' and object_id=object_id(N'dbo.TaxFiles'))
	Alter Table TaxFiles add  TransactionRef varchar(150) null
If exists(select * from sys.columns where name=N'DclntName' and object_id=object_id(N'dbo.TaxFiles'))
	Alter Table TaxFiles alter column  DclntName varchar(50) null
If exists(select * from sys.columns where name=N'CompanyCode' and object_id=object_id(N'dbo.TaxFiles'))
	Alter Table TaxFiles alter column  CompanyCode varchar(20) null
If exists(select * from sys.columns where name=N'CompanyName' and object_id=object_id(N'dbo.TaxFiles'))	
	Alter Table TaxFiles alter column  CompanyName varchar(150) null
If exists(select * from sys.columns where name=N'AsmtSerial' and object_id=object_id(N'dbo.TaxFiles'))
	Alter Table TaxFiles alter column  AsmtSerial varchar(20) null
If exists(select * from sys.columns where name=N'AsmtNumber' and object_id=object_id(N'dbo.TaxFiles'))	
	Alter Table TaxFiles alter column  AsmtNumber varchar(20) null
If exists(select * from sys.columns where name=N'OfficeName' and object_id=object_id(N'dbo.TaxFiles'))	
	Alter Table TaxFiles alter column  OfficeName varchar(50) null
If exists(select * from sys.columns where name=N'DclntCode' and object_id=object_id(N'dbo.TaxFiles'))	
	Alter Table TaxFiles alter column  DclntCode varchar(20) null
If not exists(select * from sys.columns where name=N'Currency' and object_id=object_id(N'dbo.TaxFiles'))	
	Alter Table TaxFiles add   Currency int default(1) not null
	
if exists(select name from sys.objects where name = N'sp_CreateTaxFile')
	begin
		drop Procedure dbo.sp_CreateTaxFile;
	end
Go

-- =============================================
-- Author:		Alex Mugo
-- Create date: 02/06/2018
-- Description:	A stored procedure to create a tax file record
-- =============================================
Create PROCEDURE [dbo].[sp_CreateTaxFile]
	@UserCode int,
	@TaxAmount decimal(18,2),
	@OfficeCode varchar(20),
	@OfficeName varchar(50),
	@DclntCode varchar(20),
	@DclntName varchar(50),
	@CompanyCode varchar(20),
	@CompanyName varchar(150),	
	@AsmtSerial varchar(20),
	@AsmtNumber varchar(20),
	@RegYear int,
	@RegSerial varchar(20),
	@RegNumber varchar(20),
	@AccHolder varchar(150),
	@PayerName varchar(150),
	@TranCode varchar(5),
	@TranRef varchar(150),
	@Currency varchar(1),
	@PayType int,
	@Extra1 varchar(150),
	@Extra2 varchar(150),
	@Extra3 varchar(150),
	@Extra4 varchar(150)
AS
BEGIN
	SET NOCOUNT ON;
	Declare @Code int,
			@RespStat int = 0,
			@RespMsg varchar(150) = '',
			@Charge varchar(10) = '0',
			@PostToCBS varchar(10)=''

    BEGIN TRY
		---- Validate
		If Not Exists(Select Id From Users Where UserCode = @UserCode)
		Begin
			Select  1 as RespStatus, 'Invalid user details!' as RespMessage
			Return
		End
		
		If (@TaxAmount<0)
		Begin
			Select  1 as RespStatus, 'Invalid tax amount!' as RespMessage
			Return
		End
		--- Generate Item code
		Declare	@RunNoTable TABLE	
		(
			RunNo Varchar(20) 
		)
		INSERT INTO @RunNoTable
		Exec sp_GenerateRunNo 'TXF1' 
		Select @Code = RunNo From @RunNoTable

		--- Create record now
		Insert Into TaxFiles(
			FileCode,
			CreateDate,
			UserCode,
			StatusCode,
			TaxAmount,
			OfficeCode,
			OfficeName,
			DclntCode,
			DclntName,
			CompanyCode,
			CompanyName,
			AsmtSerial,
			RegYear,
			AsmtNumber,
			Extra1,
			Extra2,
			Extra3,
			Extra4,
			RegSerial,
			RegNumber,
			AccountHolder,
			PayType,
			PayerName,
			TransactionCode,
			TransactionRef,
			Currency
		)
		Values(
			@Code,
			GETDATE(),
			@UserCode,
			0,
			@TaxAmount,
			@OfficeCode,
			@OfficeName,
			@DclntCode,
			@DclntName,
			@CompanyCode,
			@CompanyName,
			Coalesce(@AsmtSerial,''),
			@RegYear,
			Coalesce(@AsmtNumber,''),
			@Extra1,
			@Extra2,
			@Extra3,
			@Extra4,
			Coalesce(@RegSerial,''),
			Coalesce(@RegNumber,''),
			@AccHolder,
			@PayType,
			@PayerName,
			@TranCode,
			@TranRef,
			@Currency
		)

		---- Get charge
		Select @Charge = Charge From PaymentModes Where ModeCode = 0
		Set @Charge = Coalesce(@Charge, '0')
		Select @PostToCBS=ItemValue from SysSettings where ItemName='CBS_UPLOAD'

		---- Create response
		Select  @RespStat as RespStatus, @RespMsg as RespMessage, @Code as Data1, @Charge as Data2,@PostToCBS as Data3

	END TRY
	BEGIN CATCH
		SELECT @RespMsg = ERROR_MESSAGE(), @RespStat = 2;
		Select  @RespStat as RespStatus, @RespMsg as RespMessage
	END CATCH
END
Go


if exists(select name from sys.objects where name = N'vw_GetOBRPendingMultiplePayments')
	begin
		drop View dbo.vw_GetOBRPendingMultiplePayments;
	end
Go

Create view [dbo].[vw_GetOBRPendingMultiplePayments]
As
Select  
	a.FileCode,
	(select ItemValue from SysSettings where ItemName='DOM_BANK_CODE')+Cast(a.FileCode as varchar) as TranID,
	a.AsmtNumber as AssessmentNumber,
	a.AsmtSerial as AssessmentSerial,
	a.OfficeCode as OfficeCode,
	a.RegNumber as RegistrationNumber,
	a.RegSerial as RegistrationSerial,
	a.RegYear as RegistrationYear,
	a.DclntCode as DeclarantCode,
	a.CompanyCode as CompanyCode,
	a.TaxAmount as AmountToBePaid,
	11 as MeanOfPayment,
	(Select ItemValue From SysSettings Where ItemName = 'BANK_CODE') as BankCode,
	'' as CheckReference,
	a.TaxAmount as Amount,
	CONVERT(varchar(15),b.CreateDate,103) as PaymentDate,
	(Select ItemName From SysSettings Where ItemName = 'Report' and ItemValue=a.Currency) as Currency,
	'false' as Refund,
	a.PayType as Pay,
	a.AccountHolder,
	a.RegYear as ReferenceYear,
	a.RegNumber as ReferenceNumber,
	a.RegSerial as AccountReference,
	a.PayerName as TaxPayerName,
	a.TransactionCode,
	a.TransactionRef
From TaxFiles a
Inner Join Payments b on a.FileCode = b.FileCode
Where a.StatusCode = 5 and a.FileCode not in(select filecode from TempTaxDets_Arch) 
GO

if exists(select name from sys.objects where name = N'vw_GetOBRPendingPayments')
	begin
		drop View dbo.vw_GetOBRPendingPayments;
	end
Go

CREATE view [dbo].[vw_GetOBRPendingPayments]
As
Select  
	a.FileCode,
	(select ItemValue from SysSettings where ItemName='DOM_BANK_CODE')+Cast(a.FileCode as varchar) as TranID,
	a.AsmtNumber as AssessmentNumber,
	a.AsmtSerial as AssessmentSerial,
	a.OfficeCode as OfficeCode,
	a.RegNumber as RegistrationNumber,
	a.RegSerial as RegistrationSerial,
	a.RegYear as RegistrationYear,
	a.DclntCode as DeclarantCode,
	a.CompanyCode as CompanyCode,
	a.TaxAmount as AmountToBePaid,
	11 as MeanOfPayment,
	(Select ItemValue From SysSettings Where ItemName = 'BANK_CODE') as BankCode,
	'' as CheckReference,
	a.TaxAmount as Amount,
	CONVERT(varchar(15),b.CreateDate,103) as PaymentDate,
	(Select ItemName from SysSettings where ItemValue=a.Currency and Descr='Report')  as Currency,
	'false' as Refund,
	a.PayType as Pay,
	a.AccountHolder,
	a.RegYear as ReferenceYear,
	a.RegNumber as ReferenceNumber,
	a.RegSerial as AccountReference,
	a.PayerName as TaxPayerName,
	a.TransactionCode,
	a.TransactionRef as TransactionReference
From TaxFiles a
Inner Join Payments b on a.FileCode = b.FileCode
Where a.StatusCode = 5 and a.FileCode not in(select filecode from TempTaxDets_Arch) 

GO


if exists(select name from sys.objects where name = N'CreditStatementData')
	begin
		drop TYPE dbo.CreditStatementData;
	end
Go

CREATE TYPE [dbo].[CreditStatementData] AS TABLE(
	[Office] [varchar](20) NULL,
	[AssessmentYear] [varchar](20) NULL,
	[AssessmentSerial] [varchar](20) NOT NULL,
	[AssessmentNumber] [varchar](20) NOT NULL,
	[Amount] [decimal](12, 2) NOT NULL DEFAULT ((0))
)
GO

if exists(select name from sys.objects where name = N'sp_InsertCreditState')
	begin
		drop Procedure dbo.sp_InsertCreditState;
	end
Go

-- =============================================
-- Author:		Brian Ogenge
-- Create date: 03/03/2021
-- Description:	Stored Procedure To Store Credit Statement
-- =============================================
CREATE PROCEDURE [dbo].[sp_InsertCreditState]
	-- Add the parameters for the stored procedure here
	@FileCode int,
	@FileData dbo.CreditStatementData ReadOnly
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	Declare @RespStat int = 0,
			@RespMsg varchar(150) = ''

	BEGIN TRY
	If Not Exists(Select * From TaxFiles Where FileCode=@FileCode)
		Begin
			Select 1 as RespStatus, 'The payment does not exist!' as RespMessage
		Return
	End

	Declare @MyFileData as Table
	(
	Office varchar(20),
	AssessmentYear varchar(20),
	AssessmentSerial varchar(20),
	AssessmentNumber varchar(20),
	Amount decimal(12,2)
	)
	
	Insert Into @MyFileData(
	Office ,
	AssessmentYear ,
	AssessmentSerial ,
	AssessmentNumber ,
	Amount
	)
	Select a.Office,
	a.AssessmentYear ,
	a.AssessmentSerial,
	a.AssessmentNumber,
	a.Amount
	From @FileData a

	Insert Into CreditSlipPaymentsData(
	FileCode,
	Office ,
	AssessmentYear ,
	AssessmentSerial ,
	AssessmentNumber ,
	Amount
	)
	Select @FileCode,
	a.Office,
	a.AssessmentYear ,
	a.AssessmentSerial,
	a.AssessmentNumber,
	a.Amount
	From @MyFileData a

	Select @RespStat as RespStatus,
			@RespMsg as RespMessage
	END TRY
	BEGIN CATCH
	ROLLBACK
		SELECT @RespMsg = ERROR_MESSAGE(), @RespStat = 2;
		Select @RespStat as RespStatus, @RespMsg as RespMessage
	END CATCH
END
GO

if exists(select name from sys.objects where name = N'CreditSlipPaymentsData')
	begin
		drop TABLE dbo.CreditSlipPaymentsData;
	end
Go

CREATE TABLE [dbo].[CreditSlipPaymentsData](
	[FileCode] [int] NOT NULL,
	[Office] [varchar](20) NULL,
	[AssessmentYear] [varchar](20) NULL,
	[AssessmentSerial] [varchar](20) NULL,
	[AssessmentNumber] [varchar](20) NULL,
	[Amount] [decimal](12, 2) NULL
) ON [PRIMARY]

GO



if exists(select name from sys.objects where name = N'vw_PaymentReceipts')
	begin
		drop View dbo.vw_PaymentReceipts;
	end
Go

CREATE view [dbo].[vw_PaymentReceipts]
as
Select 
	a.PaymentCode,
	a.ReceiptNo,
	a.CreateDate as ReceiptDate,
	a.UserCode,
	a.Amount,
	a.Extra4 as ReceivedFrom,
	b.OfficeCode,
	b.OfficeName,
	b.CompanyCode,
	b.CompanyName,
	b.DclntName as DeclarantName,
	b.DclntCode as DeclarantCode,
	b.RegYear,
	b.RegSerial,
	b.AsmtSerial,
	b.AsmtNumber,
	c.UserName,
	a.Extra3,
	e.BranchName,
	'Thank you for choosing '+(select ItemValue from SysSettings where ItemName='BANK')+'.' as CustomMessage,
	'OBR Tax Declaration(' + b.OfficeCode + ')' as PaymentFor,
	'Paiement Electronique par '+d.ModeName as PaymentMode,
	b.StatusCode,
	b.StatusMsg,
	Cast(b.RegYear as varchar)+'-'+b.OfficeCode+'-'+b.Extra1 as OBRReceiptNo,
	b.PayType,
	Coalesce(b.TransactionCode,'') TransactionCode,
	Coalesce(b.TransactionRef,'') TransactionRef,
	b.RegNumber as RegNo,
	b.PayerName,
	case b.Currency when 1 then 'BIF' when 2 then 'USD' else ''  end as Currency,
	case b.PayType when 1 then 'PAIEMENT DE DÉCLARATION' when 2 then 'PAIEMENT DE CREDIT'
	when 3 then 'PAIEMENT PAR BORDEREAU DE CRÉDIT' when 6 then 'PAIEMENT DE DÉCLARATION' 
	when 7 then 'PAIEMENT DE CREDIT' else 'AUTRES PAIEMENTS'  end as PaymentType
From Payments a
Inner Join TaxFiles b on a.FileCode = b.FileCode
Inner Join Users c on c.UserCode = a.UserCode
Inner Join PaymentModes d on d.ModeCode = a.ModeCode
Inner Join Branches e on e.BranchCode = c.BranchCode
GO


if exists(select name from sys.objects where name = N'sp_UpdateTaxFileStatus')
	begin
		drop procedure dbo.sp_UpdateTaxFileStatus;
	end
Go

CREATE PROCEDURE [dbo].[sp_UpdateTaxFileStatus] 
	@FileCode int,
	@Stat int,
	@Msg varchar(150),
	@RecNo varchar(20),
	@RecDate varchar(20),
	@cmpName varchar(150),
	@dclntName varchar(50)
AS
BEGIN
	SET NOCOUNT ON;
	Declare @Code int,
			@RespStat int = 0,
			@RespMsg varchar(150) = '',
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
				Update TaxFiles Set StatusCode = 6, Extra1 = @RecNo, Extra2 = @RecDate Where FileCode = @FileCode
				--if the file has multiple payments update the TempTaxDets_Arch individual files
				if exists(select Filecode from TempTaxDets_Arch where Filecode=@FileCode)
				Begin
				Update TempTaxDets_Arch Set StatusCode = 6 Where FileCode = @FileCode
				End
				If(@PayType = 9 or @PayTpe=4)
				Begin
				Update TaxFiles set DclntName=@dclntName and CompanyName=@cmpName where FileCode = @FileCode
				End
			End
			Else if(@Msg like '%The declaration was already paid%')--this means notification to obr has already been succcessful and response has obr receipt numbr as per the API documentation 
		        Begin
		            Update TaxFiles Set StatusCode = 6, Extra1 = @RecNo, Extra2 = @RecDate Where FileCode = @FileCode

			        --if the file has multiple payments update the TempTaxDets_Arch individual files
				if exists(select Filecode from TempTaxDets_Arch where Filecode=@FileCode)
				Begin
				Update TempTaxDets_Arch Set StatusCode = 6 Where FileCode = @FileCode
				End
		        End
			Else if(@Msg like ('%cannot be found%'))--this means notification to obr has already been paid and customer should check with OBR 
		        Begin
		            Update TaxFiles Set StatusCode = 7 , StatusMsg = @Msg Where FileCode = @FileCode

			        --if the file has multiple payments update the TempTaxDets_Arch individual files
				if exists(select Filecode from TempTaxDets_Arch where Filecode=@FileCode)
				Begin
				Update TempTaxDets_Arch Set StatusCode = 7 Where FileCode = @FileCode
				End
		       End
			Else
			Begin
				--- Tax file notification failed, make 5 attempts and marks it as failed
				Set @Attempts = @Attempts + 1
				If(@Attempts > @MaxAttempts)
				Begin
					--- Update file notification failed
					Update TaxFiles Set StatusCode = 7, StatusMsg = @Msg Where FileCode = @FileCode
					--if the file has multiple payments update the TempTaxDets_Arch individual files
					if exists(select Filecode from TempTaxDets_Arch where Filecode=@FileCode)
				Begin
				Update TempTaxDets_Arch Set StatusCode = 7 Where FileCode = @FileCode
				End
				End
				Else
				Begin
					--- Update file attempts
					Update TaxFiles Set NotifAttempts = @Attempts, StatusMsg = @Msg Where FileCode = @FileCode
				End
			End
		End
		Else
		Begin
			Select  1 as RespStatus, 'Invalid tax file status!' as RespMessage
			Return
		End

		---- Create response
		Select  @RespStat as RespStatus, @RespMsg as RespMessage
	END TRY
	BEGIN CATCH
		SELECT @RespMsg = ERROR_MESSAGE(), @RespStat = 2;
		Select  @RespStat as RespStatus, @RespMsg as RespMessage
	END CATCH
END

GO

If not exists(select * from sys.columns where name=N'payType' and object_id=object_id(N'dbo.TempTaxDets'))	
	Alter Table TempTaxDets add  payType int default(1) not null
If not exists(select * from sys.columns where name=N'payType' and object_id=object_id(N'dbo.TempTaxDets_Arch'))	
	Alter Table TempTaxDets_Arch add  payType int default(1) not null
If not exists(select * from sys.columns where name=N'payName' and object_id=object_id(N'dbo.TempTaxDets'))	
	Alter Table TempTaxDets add  payName varchar(150) null
If not exists(select * from sys.columns where name=N'payName' and object_id=object_id(N'dbo.TempTaxDets_Arch'))	
	Alter Table TempTaxDets_Arch add  payName varchar(150) null
If not exists(select * from sys.columns where name=N'TranCode' and object_id=object_id(N'dbo.TempTaxDets'))	
	Alter Table TempTaxDets add  TranCode varchar(10) null
If not exists(select * from sys.columns where name=N'TranCode' and object_id=object_id(N'dbo.TempTaxDets_Arch'))	
	Alter Table TempTaxDets_Arch add  TranCode varchar(10) null
If not exists(select * from sys.columns where name=N'TranRef' and object_id=object_id(N'dbo.TempTaxDets'))	
	Alter Table TempTaxDets add  TranRef varchar(150) null
If not exists(select * from sys.columns where name=N'TranRef' and object_id=object_id(N'dbo.TempTaxDets_Arch'))	
	Alter Table TempTaxDets_Arch add  TranRef varchar(150) null
If not exists(select * from sys.columns where name=N'currency' and object_id=object_id(N'dbo.TempTaxDets'))	
	Alter Table TempTaxDets add  currency int default(1) not null
If not exists(select * from sys.columns where name=N'currency' and object_id=object_id(N'dbo.TempTaxDets_Arch'))	
	Alter Table TempTaxDets_Arch add  currency int default(1) not null
	
if exists(select name from sys.objects where name = N'sp_TempTaxDecl')
	begin
		drop procedure dbo.sp_TempTaxDecl;
	end
Go

CREATE PROCEDURE [dbo].[sp_TempTaxDecl]
    @mode int=0,
    @userCode int=0,
    @declCode int=0,
	@PayType int,
    @officeCode varchar(150)='' ,
    @officeName varchar(150)='',
    @declarantCode varchar(150)='',
    @declarantName varchar(150)='',
    @companyCode varchar(150)='',
    @companyName varchar(150)='',
    @assessmentYear varchar(150)='',
    @assessmentSerial varchar(150)='',
    @assessmentNumber varchar(150)='',
    @RegistrationNumber varchar(150)='',
    @RegistrationSerial varchar(150)='',
    @amountToBePaid varchar(150)='',
    @receiptSerial varchar(150)='',
    @receiptNumber varchar(150)='',
    @receiptDate varchar(150)='',
	@payerName varchar(150)='',
	@tranCode varchar(150)='',
	@tranRef varchar(150)='',
	@currency varchar(150)=''
AS

BEGIN
    SET NOCOUNT ON;
    Declare @Code int,
            @RespStat int = 0,
            @RespMsg varchar(150) = ''

    Begin Try
            --mode 0 is create-create temp the declaration record
        if(@mode=0)
        Begin
		
            --- Generate Item code
        Declare @RunNoTable TABLE(RunNo Varchar(20))
        INSERT INTO @RunNoTable Exec sp_GenerateRunNo 'TMP' 
        Select @Code = RunNo From @RunNoTable
           
			--Begin
        ---- Create record
        Insert Into TempTaxDets(
            declCode,
            usercode,
            officeCode,
            officeName,
            declarantCode,
            declarantName,
            companyCode,
            companyName,
            assessmentYear,
            assessmentSerial,
            assessmentNumber,
            RegistrationNumber,
            RegistrationSerial,
            amountToBePaid,
            receiptSerial,
            receiptNumber,
            receiptDate,
			payType,
			payName,
			TranCode,
			TranRef,
			Currency
        )
        Values(
            @Code,
            @userCode,
            @officeCode,
            @officeName,
            @declarantCode,
            @declarantName,
            @companyCode,
            @companyName,
            @assessmentYear,
            @assessmentSerial,
            @assessmentNumber,
            @RegistrationNumber,
            @RegistrationSerial,
            @amountToBePaid,
            @receiptSerial,
            @receiptNumber,
            @receiptDate,
			@PayType,
			@payerName,
			@tranCode,
			@tranRef,
			@currency
        )
        -- Create response
            Select  @RespStat as RespStatus, ' TEMP declaration   record inserted successfully.' as RespMessage
     End

    --mode 1 is delete the temp record
    if(@mode=1)
        Begin
            delete from  TempTaxDets where declCode=@declCode;
            
            -- Create response
        Select  @RespStat as RespStatus, ' TEMP declaration   record deleted successfully.' as RespMessage
            End

    END TRY
    BEGIN CATCH
        SELECT @RespMsg = ERROR_MESSAGE(), @RespStat = 2;
        ----- Create a response
        Select  @RespStat as RespStatus, @RespMsg as RespMessage
    END CATCH
END
GO

if exists(select name from sys.objects where name = N'sp_ArchTempDecl')
	begin
		drop procedure dbo.sp_ArchTempDecl;
	end
Go
Create PROCEDURE [dbo].[sp_ArchTempDecl]
	@Filecode int
AS
BEGIN
	SET NOCOUNT ON;
	Declare @Code int,
			@RespStat int = 0,
			@RespMsg varchar(150) = ''

    BEGIN TRY
		--- Validate if the filecode exists
		If Not Exists(Select declCode From TempTaxDets Where FileCode = @Filecode)
		Begin
			Select	0 as RespStatus, 'not exist' as RespMessage
			Return; 
		End
		--Archive temp declarations to match file in tax file tables
	       Insert Into TempTaxDets_Arch(
		    declCode,
			officeCode,
			officeName,
			declarantCode,
			declarantName,
			companyCode,
			companyName,
			assessmentYear,
			assessmentSerial,
			assessmentNumber,
			RegistrationNumber,
			RegistrationSerial,
			amountToBePaid,
			receiptSerial,
			receiptNumber,
			receiptDate,
			usercode,
			Filecode,
			statusCode,
			payType,
			payName,
			TranCode,
			TranRef,
			Currency
		)
		select * from TempTaxDets where Filecode=@Filecode;

		      --update the archived data
		      update TempTaxDets_Arch set statusCode=5 where Filecode=@Filecode;
		--delete records from TempTaxDets
		delete from TempTaxDets where Filecode=@Filecode;
		
		-- Create response
		Select	@RespStat as RespStatus, @RespMsg as RespMessage

	END TRY
	BEGIN CATCH
		SELECT @RespMsg = ERROR_MESSAGE(), @RespStat = 2;
		----- Create a response
		Select	@RespStat as RespStatus, @RespMsg as RespMessage
	END CATCH
END
Go

if exists(select name from sys.objects where name = N'vw_GetOBRPendingBulkPayments')
	begin
		drop View dbo.vw_GetOBRPendingBulkPayments;
	end
Go

CREATE view [dbo].[vw_GetOBRPendingBulkPayments]
As
Select  
	t.FileCode,
	t.assessmentNumber as AssessmentNumber,
	t.assessmentSerial as AssessmentSerial,
	t.OfficeCode as OfficeCode,
	t.assessmentNumber as RegistrationNumber,
	t.assessmentSerial as RegistrationSerial,
	t.declarantCode as DeclarantCode,
	t.CompanyCode as CompanyCode,
	t.amountToBePaid as AmountToBePaid,
	t.assessmentYear as RegistrationYear,
	(Select a.ItemName from SysSettings a where a.ItemValue=t.Currency and a.Descr='Report') as Currency,
	t.tranCode as TransactionCode,
	t.tranRef as TransactionReference
From TempTaxDets_Arch t
Inner Join TaxFiles a on t.FileCode = a.FileCode
Inner Join Payments b on t.FileCode = b.FileCode
Where a.StatusCode = 5 and t.statuscode=5

GO

if exists(select name from sys.objects where name = N'sp_GetTempTaxDets')
	begin
		drop PROCEDURE dbo.sp_GetTempTaxDets;
	end
Go

Create PROCEDURE [dbo].[sp_GetTempTaxDets] 
@mode int=0,
@Code varchar(50)='',
@userCode int =0
AS
BEGIN
	SET NOCOUNT ON;
	Declare 
			@RespStat int = 0,
			@RespMsg varchar(150) = '',
			@Data1 decimal=0

			--this sp get the multiple payments as captured by user
	if(@mode=0)
		Begin
		Select officeCode,officeName,declCode,DeclarantName,companyCode,
			companyName,assessmentYear,assessmentSerial,
            assessmentNumber,receiptNumber,receiptSerial, amountToBePaid,
			receiptSerial,receiptNumber,receiptDate,payType,payName TaxPayerName,TranCode TransactionCode,TranRef TransactionReference,Currency from TempTaxDets   where usercode=@userCode;
        End
	if(@mode=1)
		Begin
            Select top 1 * from TempTaxDets where declCode=@Code;
        End
	if(@mode=2)
		Begin
		   Select top 1 officeCode,officeName, declarantCode,DeclarantName,companyCode,
			companyName,assessmentYear,assessmentSerial,
            assessmentNumber,receiptNumber,receiptSerial,@Data1 as amountToBePaid,
			receiptSerial,receiptNumber,receiptDate,payType,payName TaxPayerName,TranCode TransactionCode,TranRef TransactionReference,Currency from TempTaxDets
            where usercode=@userCode
        End
END
Go

if exists(select name from sys.objects where name = N'vw_PaymentReport')
	begin
		drop View dbo.vw_PaymentReport;
	end
Go

CREATE View [dbo].[vw_PaymentReport]
As
Select 
	a.Amount,
	a.Cr_Account,
	a.Dr_Account,
	a.CreateDate as PaymentDate,
	a.UserCode,
	a.StatusCode,
	Coalesce(a.Reason,'') + '|' + Coalesce(b.StatusMsg,'') as Reason,
	a.Extra3 as RefNo,
	b.DclntName as DeclarantName,
	b.DclntCode as DeclarantNo,
	b.StatusCode as FileStatus,
	b.Extra1 as ReceiptNo,
	b.Extra2 as ReceiptDate,
	c.ModeName as PaymentMode,
	c.ModeCode,
	(select username from users where UserCode=a.UserCode ) as MakerID,
	d.BranchCode,
	a.Extra4 as DepositorName,
	a.Checker as CheckerId,
   (select username from users where UserCode=a.Checker)as Checker_Id,
	a.PaymentCode as Code,
	a.ReceiptNo as Receipt_No,
	B.StatusCode as StatusCode2,
	f.StatusName  as Status,
	b.OfficeName,
	b.OfficeCode,
	b.CompanyCode,
	b.CompanyName,
	a.Extra3 CbsResp,
	b.OfficeCode+'-' + b.RegSerial+'-' + b.RegNumber +'-' +convert(varchar(4),b.RegYear) as delcarationRef,
	o.obrofficeCode,
	b.PayType,
	case b.PayType when 1 then 'PAIEMENT DE DÉCLARATION' when 2 then 'PAIEMENT DE CREDIT'
	when 3 then 'PAIEMENT PAR BORDEREAU DE CRÉDIT' when 4 then 'AUTRES PAIEMENTS'
	when 6 then 'PAIEMENT DE DÉCLARATION EN VRAC'when 7 then 'PAIEMENT DE CREDIT EN VRAC'
	when 9 then 'PAIEMENT D’AUTRES EN VRAC' else '' end as PaymentType
From Payments a 
Inner join TaxFiles b on a.FileCode = b.FileCode
Inner join FileStatus f on b.StatusCode = f.StatusCode
Inner join PaymentModes c on c.ModeCode = a.ModeCode
Inner join Users d on d.UserCode = a.UserCode
Inner join ObrOffices o on b.OfficeCode = o.code
GO

if exists(select name from sys.objects where name = N'sp_ApprovedPayments')
	begin
		drop Procedure dbo.sp_ApprovedPayments;
	end
Go
Create PROCEDURE [dbo].[sp_ApprovedPayments] 
	@usercode int =0,
	@assesNo varchar(20)=''
AS
BEGIN
	SET NOCOUNT ON;
	Declare  
             
			@RespStat int = 0,
			@RespMsg varchar(150) = '',
			@Data1 varchar(250) = ''

    	Begin
		if(@assesNo!='')
		begin
		select * from vw_PaymentReport where UserCode=@usercode and StatusCode2>5 and Receipt_No=''+ @assesNo+ ''  order by Receipt_No desc;
		end
		else
		
		select * from vw_PaymentReport where UserCode=@usercode and StatusCode2>5  order by Receipt_No desc;
	End
	
END
Go

If Not Exists(Select * From SysSettings Where ItemName = 'DOM_BANK_CODE')
Insert into SysSettings(ItemName,ItemValue,Descr,Editable) values('DOM_BANK_CODE','FNB','Domestic Bank Code',1)

if exists(select name from sys.objects where name = N'sp_GetSystemSetting')
	begin
		drop PROCEDURE dbo.sp_GetSystemSetting;
	end
Go

CREATE PROCEDURE [dbo].[sp_GetSystemSetting] 
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
	Declare	@TxnNoTable TABLE	
		(
			TxnNo Varchar(20) 
		)
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
		Select 	
				@Data1 = Case ItemName when 'FINBRIDGE_AUTH_URL' then ItemValue else @Data1 end,
				@Data2 = Case ItemName when 'FINBRIDGE_MiARIE_URL' then ItemValue else @Data2 end,
				@Data3 = Case ItemName when 'FINBRIDGE_APPID' then ItemValue else @Data3 end,
				@Data4 = Case ItemName when 'FINBRIDGE_APPKEY' then ItemValue else @Data4 end
		From SysSettings 
		Where ItemName in ('FINBRIDGE_AUTH_URL','FINBRIDGE_MiARIE_URL','FINBRIDGE_APPID','FINBRIDGE_APPKEY')
		
	End
	Else If(@SettingType =8)
	Begin
		Select 	
				@Data2 = Case ItemName when 'ORACLE_USER' then ItemValue else @Data2 end,
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
			coalesce(@Data6,'0') as Data6,
			@Data7 as Data7,
			@Data8 as Data8,
			@Data9 as Data9,
			@Data10 as Data10
END
Go

if exists(select name from sys.objects where name = N'vw_DomesticTaxReceipts')
	begin
		drop View dbo.vw_DomesticTaxReceipts;
	end
Go
Create VIEW [dbo].[vw_DomesticTaxReceipts]
AS
SELECT      f.PayerName as CustomerName,
			a.Extra3 as ReferenceNo,
			(Select ItemValue From SysSettings Where ItemName = 'DOM_BANK_CODE')+a.FileCode as ReceiptNo,
			a.PostDate as ReceiptDate,
			a.PostDate,
			f.Tin as Nif,
			f.DeclNo as DeclarantNo,
			f.TaxPeriod as Period,
			a.Cr_Account as AccountCredit,
			a.Dr_Account as AccDebit,
			--f.Tin +' - '+
			 f.CustomerName as DeclarantDetails,
			a.FileCode,
			c.UserName,
			e.BranchName,
			h.CatergoryName as TaxDetails,
			h.CatergoryName as TaxName,
			f.PayerName as ReceivedFrom,
			f.DeclNo as DeclarationNo,
			a.Amount,
			d.ModeName,
			a.StatusCode,
			a.Remarks,
			'Paiement Electronique par ' + d.ModeName AS PaymentMode,
			a.UserCode,
			a.PaymentCode,
			a.ObrNo as OBRRefNo,
			a.ObrNo 
			FROM  dbo.DomesticTaxPayments AS a INNER JOIN
            dbo.Users AS c ON c.UserCode = a.UserCode INNER JOIN
            dbo.PaymentModes AS d ON d.ModeCode = a.ModeCode INNER JOIN
            dbo.Branches AS e ON e.BranchCode = c.BranchCode INNER JOIN
			dbo.DomesticTaxFiles AS f ON F.FileCode = a.FileCode INNER JOIN
			dbo.DomesticTaxCatergory AS h ON f.TransactionType=h.CatergoryCode
	 where a.StatusCode=1
GO

if exists(select name from sys.objects where name = N'sp_MakeDomesticTaxPayment')
	begin
		drop Procedure dbo.sp_MakeDomesticTaxPayment;
	end
Go
Create  PROCEDURE [dbo].[sp_MakeDomesticTaxPayment] 
	@FileCode int,
	@UserCode int,
	@Amount decimal(18,2),
	@ModeCode int,
	@Dr_Account varchar(20),
	@Remarks varchar(150),
	@Extra1 varchar(150),
	@Extra2 varchar(150),
	@Extra3 varchar(150),
	@Extra4 varchar(150),
	@Reason varchar(150) = '',
	@ApplyCharge bit

	
AS
BEGIN
	SET NOCOUNT ON;
		Declare @Code int,
		@ReceiptNo varchar(150),
		@RespStat int = 0,
		@RespMsg varchar(150) = '',
		@ChargeCode int,
		@Cr_Account varchar(150),
		@TaxAmount decimal(18,2),
		@CrAccount varchar(20) = '',
		@PostAttempts int,
		@PostDate datetime,
		@Checker int,
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
		@Declarant varchar(50) ='',
		@TaxName varchar(50) ='',
		@DefaultCharge varchar(20) ='0',
		@Approval int,
		@ModeId int,
		@GL_Account varchar(20),
		@GL_Acc varchar(20),
		@StatusCode int =0,
		@TxnCharge decimal(18,2),
		@BankCode varchar(20),
		@branchCode int

		BEGIN TRY
			---- Validate
			If Not Exists(Select Id From Users Where UserCode = @UserCode)
				Begin
					Select 1 as RespStatus, 'Invalid user details!' as RespMessage
					Return
				End	
			If Not Exists(Select Id From DomesticTaxFiles Where FileCode = @FileCode)
				Begin
					Select 1 as RespStatus, 'Invalid domestic tax file details!' as RespMessage
					Return
				End
			--validate sortcode
			if((@Extra2!=null or @Extra2!=''))
			Begin
				If Not Exists(Select Id From Banks Where BankCode = @Extra2)
					Begin
						Select 1 as RespStatus, 'Invalid Bank!' as RespMessage
						Return
					End	
			End

			select @Declarant= Coalesce(dtf.PayerName,dtf.DriverName),@TaxName=dtc.CatergoryName from DomesticTaxFiles dtf inner Join DomesticTaxCatergory dtc on dtc.CatergoryCode=dtf.TransactionType where dtf.FileCode=@FileCode

			Select	@ModeId = Id, 
					@Approval = Approval, 
					--@GL_Account = GL_Account,
					@TxnCharge = Charge, 
					@CBSTxnCode = CBS_Txn_Code 
			From PaymentModes Where ModeCode = @ModeCode
			If (@ModeId Is Null)
				Begin
					Select 1 as RespStatus, 'Invalid payment mode!' as RespMessage
					Return	
				End
			---- Set auto-approval items
			If(@Approval = 0) 
				Set @StatusCode = 1;
			
			--- Get settings
				Select	@CashAccount =(CASE WHEN a.ItemName = 'DOMESTICTAX_CASH' THEN a.ItemValue ELSE @CashAccount END ),
						@CrAccount =(CASE WHEN a.ItemName = 'DOMESTICTAX_CR_ACCOUNT' THEN a.ItemValue ELSE @CrAccount END),
						@GL_Account =(CASE WHEN a.ItemName = 'DOMESTICTAX_CERTIFIED_CHEQUE' THEN a.ItemValue ELSE @GL_Account END),
						@GL_Acc =(CASE WHEN a.ItemName = 'DOMESTICTAX_CLEARING' THEN a.ItemValue ELSE @GL_Acc END),
						@PostUrl =(CASE WHEN a.ItemName = 'CBS_POST_URL' THEN a.ItemValue ELSE @PostUrl END),
						@Officer =(CASE WHEN a.ItemName = 'CBS_OFFICER' THEN a.ItemValue ELSE @Officer END),
						@DefaultTxnCode =(CASE WHEN a.ItemName = 'CBS_TXN_CODE' THEN a.ItemValue ELSE @DefaultTxnCode END),
						@TxnType =(CASE WHEN a.ItemName = 'CBS_TXN_TYPE' THEN a.ItemValue ELSE @TxnType END),
						@DDName =(CASE WHEN a.ItemName = 'CBS_TNCTR_NAME' THEN a.ItemValue ELSE @DDName END),
						@BankCode = (Case When a.ItemName='DOM_BANK_CODE' Then a.ItemValue Else @BankCode END),
						@DefaultCharge =(CASE WHEN a.ItemName = 'TAX_CHARGE_AMOUNT' THEN a.ItemValue ELSE @DefaultCharge END)		
			From SysSettings a 
			Where a.ItemName In('DOMESTICTAX_CASH','DOMESTICTAX_CR_ACCOUNT','DOMESTICTAX_CERTIFIED_CHEQUE','DOMESTICTAX_CLEARING','CBS_POST_URL','CBS_OFFICER','CBS_TXN_CODE',
			'CBS_TXN_TYPE','CBS_TNCTR_NAME','CBS_DOM_NARRATION','DOM_BANK_CODE','TAX_CHARGE_AMOUNT')

			set @Narration=@BankCode+Cast(@FileCode as Varchar(15))+' Tax Paid: '+@TaxName+' Declarant: '+Coalesce(@Declarant,'')
			If(@ModeCode = 0)
			Begin
				--- Get user cash account
				--Select @CashAccount = a.Cash_Account From Users a Inner Join Branches b on a.BranchCode = b.BranchCode Where UserCode = @UserCode
				--Set @CashAccount = Coalesce(@CashAccount,'')
				Set @Dr_Account = @CashAccount	

			---- Validate account
			If(Len(@Dr_Account) = 0)
				Begin
					Select 1 as RespStatus, 'Cash account is not set!' as RespMessage
					Return
				End
			End

			---- Validate GL account for certified cheque
			If(@ModeCode = 3)
				Begin
					Set @Dr_Account = Coalesce(@GL_Account,'')
			If(Len(@Dr_Account) = 0)
				Begin
					Select 1 as RespStatus, 'GL account for certified cheque NOT set!' as RespMessage
					Return
				End
			End

			---- Validate GL account for clearing cheque
			If(@ModeCode = 4)
				Begin
					Set @Dr_Account = Coalesce(@GL_Acc,'')
			If(Len(@Dr_Account) = 0)
				Begin
					Select 1 as RespStatus, 'GL account for certified cheque NOT set!' as RespMessage
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
				Set @TxnCharge =0;

			--- Generate Item code
			Declare	@RunNoTable TABLE(RunNo Varchar(20))
			INSERT INTO @RunNoTable	Exec sp_GenerateRunNo 'PYT1' 
			Select @Code = RunNo From @RunNoTable
			--- Create record now
			Insert Into DomesticTaxPayments(
			PaymentCode,
			FileCode,
			ReceiptNo,
			UserCode,
			Amount,
			StatusCode,
			ModeCode,
			Cr_Account,
			Dr_Account,
			Remarks,
			PostAttempts,
			PostDate,
			Checker,
			Reason,
			Extra1,
			Extra2,
			Extra3,
			Extra4,
			ApplyCharge
			)
			Values(
			@Code,
			@FileCode,
			@BankCode + Cast(@FileCode as Varchar(15)),
			@UserCode,
			@Amount,
			@StatusCode,
			@ModeCode,
			@CrAccount,
			@Dr_Account,
			@Remarks,
			1,
			GETDATE(),
			1,
			@Reason,
			@Extra1,
			@Extra2,
			@Extra3,
			@Extra4,
			@ApplyCharge
			)

			--- Create tax commission
			If(@ApplyCharge = 1)
			Begin
			--- Get charge settings
			Select	@BalUrl =(CASE WHEN a.ItemName = 'CBS_BALANCE_URL' THEN a.ItemValue ELSE @BalUrl END),	
					@ChargeCrAccount =(CASE WHEN a.ItemName = 'CHARGE_CR_ACCOUNT' THEN a.ItemValue ELSE @ChargeCrAccount END),
					@ChargeTxnCode =(CASE WHEN a.ItemName = 'CHARGE_CBS_TXN_CODE' THEN a.ItemValue ELSE @ChargeTxnCode END),
					@ChargeTxnType =(CASE WHEN a.ItemName = 'CHARGE_CBS_TXN_TYPE' THEN a.ItemValue ELSE @ChargeTxnType END)
					--@ChargeNarration =(CASE WHEN a.ItemName = 'CHARGE_CBS_DOM_NARRATION' THEN a.ItemValue ELSE @ChargeNarration END)
			From SysSettings a 
			Where a.ItemName In('CBS_BALANCE_URL','CHARGE_CR_ACCOUNT','CHARGE_CBS_TXN_CODE','CHARGE_CBS_TXN_TYPE','CHARGE_CBS_DOM_NARRATION')

			--- Generate code
			Delete From @RunNoTable
			INSERT INTO @RunNoTable
			Exec sp_GenerateRunNo 'CHG1' 
			Select @ChargeCode = RunNo From @RunNoTable

			Insert Into DomesticTaxCharges(
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
			select @branchCode= CBS_Code from Branches where BranchCode=(select BranchCode from Users where UserCode=@UserCode)
			set @ChargeNarration=@BankCode+Cast(@FileCode as Varchar(15))+'/'+Cast(@ChargeCode as Varchar(15))+' '+@TaxName+' '+Coalesce(@Declarant,'')


			---- Create response
			Select @RespStat as RespStatus, 
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
			@BankCode + Cast(@FileCode as Varchar(15)) as MainRefNo,
			'0' as MainFlag,
			@Dr_Account as MainDrAccount,
			@CrAccount as MainCrAccount,
			@Narration as MainNarration,
			@ChargeTxnCode as ChargeTxnCode,
			@ChargeTxnType as ChargeTxnType,
			@BankCode + Cast(@FileCode as Varchar(15)) + '/' + Cast(@ChargeCode as Varchar(15)) as ChargeRefNo,
			'0' as ChargeFlag,
			@Dr_Account as ChargeDrAccount,
			@ChargeCrAccount as ChargeCrAccount,
			@ChargeNarration as ChargeNarration,
			@TxnCharge as ChargeAmount,
			@ChargeCode as ChargeCode,
			@Approval as ApprovalNeeded,
			@Extra1 as ChequeNo,
			@branchCode as brachCode
		END TRY
	BEGIN CATCH
		SELECT @RespMsg = ERROR_MESSAGE(), @RespStat = 2;
		Select @RespStat as RespStatus, @RespMsg as RespMessage	
	END CATCH
	END

Go



if exists(select name from sys.objects where name = N'sp_GetFailedPayments')
	begin
		drop Procedure dbo.sp_GetFailedPayments;
	end
Go

Create PROCEDURE [dbo].[sp_GetFailedPayments] 
	@usercode int =0,
	@DateFrom varchar(20)='',
	@DateTo varchar(20)=''
AS
BEGIN
	SET NOCOUNT ON;
	Declare  
             
			@RespStat int = 0,
			@RespMsg varchar(150) = '',
			@Data1 varchar(250) = '',
			@userRole int=0
     Begin
		 if(@DateFrom!='' and @DateTo!='')
		    begin
		         select RegYear Year, RegNumber RegNo, RegSerial,TaxAmount Amount,case when a.statuscode =7 then 'Failed' when a.statuscode= 6 then 'successful' else 'Pending' end status,a.StatusMsg,
				 OfficeCode, OfficeName, DclntCode DeclCode,DclntName DeclName,companyCode NIF,a.FileCode, CompanyName,b.CreateDate,b.Cr_Account CrAccount,b.Dr_Account DrAccount,b.Remarks, b.Extra3 CBSref
			     from TaxFiles a left join payments b on a.FileCode=b.FileCode where a.StatusCode=7 and b.StatusCode=1
				  and cast(b.CreateDate as date) >= cast(@DateFrom as date) and cast(b.CreateDate as date) <= cast(@DateTo as date) order by a.id desc
		    end
				 select RegYear Year, RegNumber RegNo, RegSerial,TaxAmount Amount,case when a.statuscode =7 then 'Failed' when a.statuscode= 6 then 'successful' else 'Pending' end status,a.StatusMsg,
				 OfficeCode, OfficeName, DclntCode DeclCode,DclntName DeclName,companyCode NIF,a.FileCode, CompanyName,b.CreateDate,b.Cr_Account CrAccount,b.Dr_Account DrAccount,b.Remarks, b.Extra3 CBSref
			     from TaxFiles a left join payments b on a.FileCode=b.FileCode where a.StatusCode=7 and b.StatusCode=1 order by a.id desc
		End
	 End
GO

if exists(select name from sys.objects where name = N'sp_UpdateFailedPayments')
	begin
		drop Procedure dbo.sp_UpdateFailedPayments;
	end
Go

Create PROCEDURE [dbo].[sp_UpdateFailedPayments] 
	@user int,
	@filecode int
AS
BEGIN
	SET NOCOUNT ON;
	Declare @Code int,
			@RespStat int = 0,
			@RespMsg varchar(150) = '',
			@UserRole int
    BEGIN TRY
		---- Validate
		Select @UserRole = UserRole From Users Where UserCode = @user
		If(@UserRole Is Null)
		Begin
			Select  1 as RespStatus, 'Invalid user details!' as RespMessage
			Return
		End
		If(@UserRole <> 1)
		Begin
			Select  1 as RespStatus, 'This action can ONLY be done by user with role checker!' as RespMessage
			Return
		End
		If Not Exists(Select Id From TaxFiles Where FileCode = @filecode)
		Begin
			Select  1 as RespStatus, 'Invalid Payment Details!' as RespMessage
			Return
		End
		update TempTaxDets_Arch set StatusCode=5 where FileCode=@filecode
		Update TaxFiles Set StatusCode = 5 Where FileCode = @filecode
		---- Create response
		Select  @RespStat as RespStatus, @RespMsg as RespMessage

	END TRY
	BEGIN CATCH
		SELECT @RespMsg = ERROR_MESSAGE(), @RespStat = 2;
		Select  @RespStat as RespStatus, @RespMsg as RespMessage
	END CATCH
END
GO

if exists(select name from sys.objects where name = N'vw_GetOBRPendingMultiplePayments')
	begin
		drop View dbo.vw_GetOBRPendingMultiplePayments;
	end
Go

Create view [dbo].[vw_GetOBRPendingMultiplePayments]
As
Select  
	a.FileCode,
	(select ItemValue from SysSettings where ItemName='DOM_BANK_CODE')+Cast(a.FileCode as varchar) as TranID,
	a.AsmtNumber as AssessmentNumber,
	a.AsmtSerial as AssessmentSerial,
	a.OfficeCode as OfficeCode,
	a.RegNumber as RegistrationNumber,
	a.RegSerial as RegistrationSerial,
	a.RegYear as RegistrationYear,
	a.DclntCode as DeclarantCode,
	a.CompanyCode as CompanyCode,
	a.TaxAmount as AmountToBePaid,
	11 as MeanOfPayment,
	(Select ItemValue From SysSettings Where ItemName = 'BANK_CODE') as BankCode,
	'' as CheckReference,
	a.TaxAmount as Amount,
	CONVERT(varchar(15),b.CreateDate,103) as PaymentDate,
	(Select ItemName From SysSettings Where ItemName = 'Report' and ItemValue=a.Currency) as Currency,
	'false' as Refund,
	a.PayType as Pay,
	a.AccountHolder,
	a.RegYear as ReferenceYear,
	a.RegNumber as ReferenceNumber,
	a.RegSerial as AccountReference,
	a.PayerName as TaxPayerName,
	a.TransactionCode,
	a.TransactionRef
From TaxFiles a
Inner Join Payments b on a.FileCode = b.FileCode
Where a.StatusCode = 5 and a.FileCode  in(select filecode from TempTaxDets_Arch) 
GO


if exists(select name from sys.objects where name = N'vw_GetOBRPendingPayments')
	begin
		drop View dbo.vw_GetOBRPendingPayments;
	end
Go

CREATE view [dbo].[vw_GetOBRPendingPayments]
As
Select  
	a.FileCode,
	(select ItemValue from SysSettings where ItemName='DOM_BANK_CODE')+Cast(a.FileCode as varchar) as TranID,
	a.AsmtNumber as AssessmentNumber,
	a.AsmtSerial as AssessmentSerial,
	a.OfficeCode as OfficeCode,
	a.RegNumber as RegistrationNumber,
	a.RegSerial as RegistrationSerial,
	a.RegYear as RegistrationYear,
	a.DclntCode as DeclarantCode,
	a.CompanyCode as CompanyCode,
	a.TaxAmount as AmountToBePaid,
	11 as MeanOfPayment,
	(Select ItemValue From SysSettings Where ItemName = 'BANK_CODE') as BankCode,
	'' as CheckReference,
	a.TaxAmount as Amount,
	CONVERT(varchar(15),b.CreateDate,103) as PaymentDate,
	(Select ItemName from SysSettings where ItemValue=a.Currency and Descr='Report')  as Currency,
	'false' as Refund,
	a.PayType as Pay,
	a.AccountHolder,
	a.RegYear as ReferenceYear,
	a.RegNumber as ReferenceNumber,
	a.RegSerial as AccountReference,
	a.PayerName as TaxPayerName,
	a.TransactionCode,
	a.TransactionRef as TransactionReference
From TaxFiles a
Inner Join Payments b on a.FileCode = b.FileCode
Where a.StatusCode = 5 and a.FileCode not in(select filecode from TempTaxDets_Arch) 
GO

if exists(select name from sys.objects where name = N'vw_GetOBRPendingBulkPayments')
	begin
		drop View dbo.vw_GetOBRPendingBulkPayments;
	end
Go

CREATE view [dbo].[vw_GetOBRPendingBulkPayments]
As
Select  
	t.FileCode,
	(select ItemValue from SysSettings where ItemName='DOM_BANK_CODE')+cast(t.FileCode as varchar) as TranID,
	t.assessmentNumber as AssessmentNumber,
	t.assessmentSerial as AssessmentSerial,
	t.OfficeCode as OfficeCode,
	t.RegistrationNumber as RegistrationNumber,
	t.RegistrationSerial as RegistrationSerial,
	t.declarantCode as DeclarantCode,
	t.CompanyCode as CompanyCode,
	t.amountToBePaid as AmountToBePaid,
	t.assessmentYear as RegistrationYear,
	t.TranCode as TransactionCode,
	t.TranRef as TransactionReference,
	(Select ItemName from SysSettings where ItemValue=a.Currency and Descr='Report')  as Currency
From TempTaxDets_Arch t
Inner Join TaxFiles a on t.FileCode = a.FileCode
Inner Join Payments b on t.FileCode = b.FileCode
GO


if exists(select name from sys.objects where name = N'vw_PaymentReceipts')
	begin
		drop View dbo.vw_PaymentReceipts;
	end
Go

CREATE view [dbo].[vw_PaymentReceipts]
as
Select 
	a.PaymentCode,
	a.ReceiptNo,
	a.CreateDate as ReceiptDate,
	a.UserCode,
	a.Amount,
	a.Extra4 as ReceivedFrom,
	b.OfficeCode,
	b.OfficeName,
	b.CompanyCode,
	b.CompanyName,
	b.DclntName as DeclarantName,
	b.DclntCode as DeclarantCode,
	b.RegYear,
	b.RegSerial,
	b.AsmtSerial,
	b.AsmtNumber,
	c.UserName,
	a.Extra3,
	e.BranchName,
	'Thank you for choosing '+(select ItemValue from SysSettings where ItemName='BANK')+'.' as CustomMessage,
	'OBR Tax Declaration(' + b.OfficeCode + ')' as PaymentFor,
	'Paiement Electronique par '+d.ModeName as PaymentMode,
	b.StatusCode,
	b.StatusMsg,
	Cast(b.RegYear as varchar)+'-'+b.OfficeCode+'-'+b.Extra1 as OBRReceiptNo,
	b.PayType,
	Coalesce(b.TransactionCode,'') TransactionCode,
	Coalesce(b.TransactionRef,'') TransactionRef,
	Coalesce(b.RegNumber,'') as RegNo,
	Coalesce(b.PayerName,'') PayerName,
	case b.Currency when 1 then 'BIF' when 2 then 'USD' else ''  end as Currency,
	case b.PayType when 1 then 'PAIEMENT DE DÉCLARATION' when 2 then 'PAIEMENT DE CREDIT'
	when 3 then 'PAIEMENT PAR BORDEREAU DE CRÉDIT' when 6 then 'PAIEMENT DE DÉCLARATION' 
	when 7 then 'PAIEMENT DE CREDIT' else 'AUTRES PAIEMENTS'  end as PaymentType
From Payments a
Inner Join TaxFiles b on a.FileCode = b.FileCode
Inner Join Users c on c.UserCode = a.UserCode
Inner Join PaymentModes d on d.ModeCode = a.ModeCode
Inner Join Branches e on e.BranchCode = c.BranchCode
GO
