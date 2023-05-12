
Create Table TaxTypes(
	Id int not null identity,
	TypeCode int not null,
	TypeName varchar(50) not null,
	Constraint pk_TaxTypes primary key(Id),
	Constraint fk_TaxTypes_TypeCode unique(TypeCode)
)
Go

Insert Into TaxTypes(TypeCode, TypeName) Values(10, 'OBR CUSTOM')
Insert Into TaxTypes(TypeCode, TypeName) Values(11, 'OBR DOMESTIC')
Insert Into TaxTypes(TypeCode, TypeName) Values(12, 'MIARIE')
Go

Alter Table Payments Add TaxType int not null default 10
Go

Alter Table Payments Add Constraint fk_Payments_TaxType foreign key(TaxType) references TaxTypes(TypeCode)
Go

Alter Table Payments Drop Constraint fk_Payments_FileCode
Go

-- =============================================
-- Author:		Modified By Alex Mugo
-- Create date: 17/09/2022
-- Description:	A stored procedure to make tax payment
-- =============================================
ALTER PROCEDURE [dbo].[sp_MakeTaxPayment] 
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
	@ApplyCharge bit,
	@TaxType int
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
			@CurrencyCode int =0,
			@Currency varchar(10) ='',
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
			@Decl varchar(150)='',
			@branchCode int

	BEGIN TRY	
		---- Validate
		If Not Exists(Select Id From Users Where UserCode = @UserCode)
		Begin
			Select  1 as RespStatus, 'Invalid user details!' as RespMessage
			Return
		End
		If Not Exists(Select Id From TaxTypes Where TypeCode = @TaxType)
		Begin
			Select  1 as RespStatus, 'Invalid tax details!' as RespMessage
			Return
		End

		If(@TaxType = 10)
		Begin
			If Not Exists(Select Id From TaxFiles Where FileCode = @FileCode)
			Begin
				Select  1 as RespStatus, 'Invalid tax file details!' as RespMessage
				Return
			End
		End
		If(@TaxType = 11)
		Begin
			If Not Exists(Select Id From TaxFiles Where FileCode = @FileCode)
			Begin
				Select  1 as RespStatus, 'Invalid tax file details!' as RespMessage
				Return
			End
		End
		If(@TaxType = 12)
		Begin
			If Not Exists(Select Id From MiarieTaxFiles Where FileCode = @FileCode)
			Begin
				Select  1 as RespStatus, 'Invalid tax file details!' as RespMessage
				Return
			End
		End
		
		--- Validate sortcode
		If((@Extra2 != null or @Extra2 != ''))
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
				@DefaultCharge =(CASE WHEN a.ItemName = 'TAX_CHARGE_AMOUNT' THEN a.ItemValue ELSE @DefaultCharge END)
		From SysSettings a 
		Where a.ItemName In('CASH_DR_ACCOUNT','TAX_CR_ACCOUNT','CBS_POST_URL','CBS_OFFICER','CBS_TXN_CODE',
							'CBS_TXN_TYPE','CBS_TNCTR_NAME','CBS_NARRATION','TAX_CHARGE_AMOUNT')

		Select @CurrencyCode = Currency From TaxFiles Where FileCode = @FileCode
		Select @Currency = a.ItemName from SysSettings a where a.ItemValue = @CurrencyCode and a.Descr = 'Report'		

		If(@ModeCode = 0)
		Begin
			--- Get user cash account
			Select @CashAccount = b.Cash_GL_Account From Users a Inner Join Branches b on a.BranchCode = b.BranchCode Where UserCode = @UserCode
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

		If(@CurrencyCode = 2)
		Begin
			Select	@CashAccount =(CASE WHEN a.ItemName = 'CASH_USD_DR_ACCOUNT' THEN a.ItemValue ELSE @CashAccount END ),
					@CrAccount =(CASE WHEN a.ItemName = 'TAX__USD_CR_ACCOUNT' THEN a.ItemValue ELSE @CrAccount END)
			From SysSettings a Where a.ItemName In('CASH_USD_DR_ACCOUNT','TAX__USD_CR_ACCOUNT')
			Set @Dr_Account = @CashAccount
		End

		--- Process charge
		If(@TxnCharge = 0)
			Set @TxnCharge = Cast(@DefaultCharge as Decimal(18,2))

		Set @TaxAmount = @Amount
		If(@ApplyCharge = 1)
			Set @TaxAmount = @Amount - @TxnCharge
		else
			Set @TxnCharge = 0;

		--- Generate Item code
		Declare	@RunNoTable TABLE(RunNo Varchar(20))
		INSERT INTO @RunNoTable	Exec sp_GenerateRunNo 'PYT1' 
		Select @Code = RunNo From @RunNoTable

		--- Create record now
		Insert Into Payments(
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
			ApplyCharge,
			TaxType
		)
		Values(
			@Code,
			@FileCode,
			'TX' + Cast(@Code as Varchar(15)),
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
			@ApplyCharge,
			@TaxType
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
			INSERT INTO @RunNoTable	Exec sp_GenerateRunNo 'CHG1' 
			Select @ChargeCode = RunNo From @RunNoTable

			Insert Into TaxCharges(
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
		select @branchCode = CBS_Code from Branches where BranchCode=(select BranchCode from Users where UserCode = @UserCode)
		select @Decl = DclntName from TaxFiles where FileCode = @FileCode

		---- Create response
		Select  @RespStat as RespStatus, 
				@RespMsg as RespMessage, 
				@Code as PaymentCode,
				@ModeCode as PaymentMode,
				@ApplyCharge as ApplyCharge,
				@PostUrl as PostUrl,
				@BalUrl as BalanceUrl,
				@Currency as CurrencyCode,
				@TaxAmount as MainAmount,
				@CBSTxnCode as MainTxnCode,
				@TxnType as MainTxnType,
				@Officer as CBSOfficer,
				@DDName as TransactorName,
				'TX' + Cast(@Code as Varchar(15)) as MainRefNo,
				'0' as MainFlag,
				@Dr_Account as MainDrAccount,
				@CrAccount as MainCrAccount,
				Coalesce(@Remarks,@Narration,@Decl) as MainNarration,
				@ChargeTxnCode as ChargeTxnCode,
				@ChargeTxnType as ChargeTxnType,
				'TX' + Cast(@Code as Varchar(15)) + '/' + Cast(@ChargeCode as Varchar(15)) as ChargeRefNo,
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
		Select  @RespStat as RespStatus, @RespMsg as RespMessage
	END CATCH
END
Go


-- =============================================
-- Author:		Modified By Alex Mugo
-- Create date: 20/09/2022
-- Description:	A stored procedure to validate tax payment
-- =============================================
ALTER PROCEDURE [dbo].[sp_ValidateTaxPayment]
    @FileCode int,
    @Amount decimal(18,2),
    @ModeCode int,
    @AccountNo varchar(20),
    @ChequeNo varchar(20),
    @NoCharge bit,
    @SortCode varchar(10),
	@PostToCBS int,
	@CBSRef varchar(150),
	@TaxType int
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
            @CrAccountName varchar(50) = 'TAX ACCOUNT',
            @DefaultCharge varchar(15),
            @BalUrl varchar(200),
            @ChequeValidUrl varchar(200),
			@TaxAcc varchar(50),
			@Currency varchar(1),
			@Attempts int

    BEGIN TRY
        ---- Validate
		---- Check posting reference
		If (@PostToCBS = 0)
		Begin
			If(@TaxType = 10)
			Begin
				Select @Attempts = b.StatusCode, @Currency = b.Currency From Payments a Inner Join TaxFiles b on b.FileCode = a.FileCode where a.Extra3 =  @CBSRef 
				If (@Attempts = 6)
				Begin
					Select  1 as RespStatus, @CBSRef+' has already been used to make payment!!!' as RespMessage
					Return
				End
			End
			Select @TaxAcc =ItemValue From SysSettings where ItemName ='TAX_CR_ACCOUNT'
		End
		Else If(@PostToCBS = 1)
		Begin
			-- validate if the cheque is already used for clearing cheque Extra2 column is used to store the sortcode
			If(Coalesce(@SortCode, '') <> '')
			Begin
				If Exists(Select Id From Payments where Extra1 = @ChequeNo and Extra2 = @SortCode)
				Begin
					Select  1 as RespStatus, 'This cheque: ' + @ChequeNo +' has already been used to make payment!!!' as RespMessage
					Return
				End
			End

			If(@TaxType = 10)
				Select @TaxAmount = TaxAmount, @Currency = Currency From TaxFiles Where FileCode = @FileCode
			Else If(@TaxType = 11)
				Select @TaxAmount = Amount, @Currency = 1 From DomesticTaxFiles Where FileCode = @FileCode
			Else If(@TaxType = 12)
				Select @TaxAmount = Amount, @Currency = 1 From MiarieTaxFiles Where FileCode = @FileCode

			If (@TaxAmount Is Null)
			Begin
				Select  1 as RespStatus, 'Invalid tax record details!' as RespMessage
				Return
			End

			Select @PayMode = ModeName, @ChargeAmount = Charge From PaymentModes Where ModeCode = @ModeCode
			If(@PayMode Is Null)
			Begin
				Select  1 as RespStatus, 'Invalid payment mode!' as RespMessage
				Return
			End

			Select  @BalUrl =(CASE WHEN a.ItemName = 'CBS_BALANCE_URL' THEN a.ItemValue ELSE @BalUrl END),                  
					@DefaultCharge =(CASE WHEN a.ItemName = 'TAX_CHARGE_USD' THEN a.ItemValue ELSE @DefaultCharge END),
					@ChequeValidUrl =(CASE WHEN a.ItemName = 'CBS_CHQ_VALID_URL' THEN a.ItemValue ELSE @ChequeValidUrl END)
			From SysSettings a Where ItemName  in('TAX_CHARGE_USD','CBS_BALANCE_URL','CBS_CHQ_VALID_URL')

			If(@TaxType = 12)
				Set @NoCharge = 1

			If(@NoCharge = 1)
				Set @ExpectedAmount = @TaxAmount 
			Else
			Begin
				---- Get charge
				If(@Currency = '1')
					Select @DefaultCharge = p.Charge From PaymentModes p Where p.ModeCode=@ModeCode;
				
				Set @ChargeAmount = Coalesce(@DefaultCharge,  Cast((Coalesce(@DefaultCharge, '0')) as Decimal(18,2)));
				Set @ExpectedAmount = @TaxAmount + @ChargeAmount
			End

			If(@ExpectedAmount <> @Amount)
			Begin
				Select  1 as RespStatus, 'Invalid amount! Expected amount is: ' + Cast(@ExpectedAmount as Varchar(20)) as RespMessage
				Return
			End
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
Go

-- =============================================
-- Author:		Modified By Alex Mugo
-- Create date: 17/09/2022
-- Description:	A stored procedure to make tax payment
-- =============================================
ALTER PROCEDURE [dbo].[sp_MakeTaxPayment] 
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
	@ApplyCharge bit,
	@TaxType int
AS
BEGIN
	SET NOCOUNT ON;
	Declare @Code int,
			@RespStat int = 0,
			@RespMsg varchar(150) = '',
			@ChargeCode int,
			@TaxId int,
			@TaxAmount decimal(18,2),
			@CrAccount varchar(20) = '',
			@ChargeCrAccount varchar(20) = '',
			@PostUrl varchar(150) ='',
			@BalUrl varchar(150) ='',
			@CurrencyCode int =0,
			@Currency varchar(10) ='',
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
			@Decl varchar(150)='',
			@BranchCode int,
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
		If Not Exists(Select Id From TaxTypes Where TypeCode = @TaxType)
		Begin
			Select  1 as RespStatus, 'Invalid tax type!' as RespMessage
			Return
		End

		If(@TaxType = 10)
			Select @TaxId = Id From TaxFiles Where FileCode = @FileCode
		If(@TaxType = 11)
			Select @TaxId = Id From DomesticTaxFiles Where FileCode = @FileCode		
		If(@TaxType = 12)
			Select @TaxId = Id From MiarieTaxFiles Where FileCode = @FileCode		

		If(@TaxId Is Null)
		Begin
			Select  1 as RespStatus, 'Invalid tax record details!' as RespMessage
			Return
		End
		
		--- Validate sortcode
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
				@DefaultCharge =(CASE WHEN a.ItemName = 'TAX_CHARGE_AMOUNT' THEN a.ItemValue ELSE @DefaultCharge END)
		From SysSettings a 
		Where a.ItemName In('CASH_DR_ACCOUNT','TAX_CR_ACCOUNT','CBS_POST_URL','CBS_OFFICER','CBS_TXN_CODE',
							'CBS_TXN_TYPE','CBS_TNCTR_NAME','CBS_NARRATION','TAX_CHARGE_AMOUNT')

		If(@TaxType = 12)
		Begin
			Select	@Data1 = Case ItemName when 'FINBRIDGE_BASE_URL' then ItemValue else @Data1 end,
					@Data2 = Case ItemName when 'FINBRIDGE_APPID' then ItemValue else @Data2 end,
					@Data3 = Case ItemName when 'FINBRIDGE_APPKEY' then ItemValue else @Data3 end
			From SysSettings a 
			Where a.ItemName In('FINBRIDGE_BASE_URL','FINBRIDGE_APPID','FINBRIDGE_APPKEY')
		End

		Select @CurrencyCode = Currency From TaxFiles Where FileCode = @FileCode
		Select @Currency = a.ItemName from SysSettings a where a.ItemValue = @CurrencyCode and a.Descr = 'Report'		

		If(@ModeCode = 0)
		Begin
			--- Get user cash account
			Select @CashAccount = b.Cash_GL_Account From Users a Inner Join Branches b on a.BranchCode = b.BranchCode Where UserCode = @UserCode
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

		If(@CurrencyCode = 2)
		Begin
			Select	@CashAccount =(CASE WHEN a.ItemName = 'CASH_USD_DR_ACCOUNT' THEN a.ItemValue ELSE @CashAccount END ),
					@CrAccount =(CASE WHEN a.ItemName = 'TAX__USD_CR_ACCOUNT' THEN a.ItemValue ELSE @CrAccount END)
			From SysSettings a Where a.ItemName In('CASH_USD_DR_ACCOUNT','TAX__USD_CR_ACCOUNT')
			Set @Dr_Account = @CashAccount
		End

		--- Process charge
		If(@TxnCharge = 0)
			Set @TxnCharge = Cast(@DefaultCharge as Decimal(18,2))

		Set @TaxAmount = @Amount
		If(@ApplyCharge = 1)
			Set @TaxAmount = @Amount - @TxnCharge
		else
			Set @TxnCharge = 0;

		If(@TaxType = 12)
			Set @TxnCharge = 0

		--- Generate Item code
		Declare	@RunNoTable TABLE(RunNo Varchar(20))
		INSERT INTO @RunNoTable	Exec sp_GenerateRunNo 'PYT1' 
		Select @Code = RunNo From @RunNoTable

		--- Create record now
		Insert Into Payments(
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
			ApplyCharge,
			TaxType
		)
		Values(
			@Code,
			@FileCode,
			'TX' + Cast(@Code as Varchar(15)),
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
			@ApplyCharge,
			@TaxType
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
			INSERT INTO @RunNoTable	Exec sp_GenerateRunNo 'CHG1' 
			Select @ChargeCode = RunNo From @RunNoTable

			Insert Into TaxCharges(
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
		select @branchCode = CBS_Code from Branches where BranchCode=(select BranchCode from Users where UserCode = @UserCode)
		select @Decl = DclntName from TaxFiles where FileCode = @FileCode

		---- Create response
		Select  @RespStat as RespStatus, 
				@RespMsg as RespMessage, 
				@Code as PaymentCode,
				@ModeCode as PaymentMode,
				@ApplyCharge as ApplyCharge,
				@PostUrl as PostUrl,
				@BalUrl as BalanceUrl,
				@Currency as CurrencyCode,
				@TaxAmount as MainAmount,
				@CBSTxnCode as MainTxnCode,
				@TxnType as MainTxnType,
				@Officer as CBSOfficer,
				@DDName as TransactorName,
				'TX' + Cast(@Code as Varchar(15)) as MainRefNo,
				'0' as MainFlag,
				@Dr_Account as MainDrAccount,
				@CrAccount as MainCrAccount,
				Coalesce(@Remarks,@Narration,@Decl) as MainNarration,
				@ChargeTxnCode as ChargeTxnCode,
				@ChargeTxnType as ChargeTxnType,
				'TX' + Cast(@Code as Varchar(15)) + '/' + Cast(@ChargeCode as Varchar(15)) as ChargeRefNo,
				'0' as ChargeFlag,
				@Dr_Account as ChargeDrAccount,
				@ChargeCrAccount as ChargeCrAccount,
				@ChargeNarration as ChargeNarration,
				@TxnCharge as ChargeAmount,
				@ChargeCode as ChargeCode,
				@Approval as ApprovalNeeded,
				@Extra1 as ChequeNo,
				@BranchCode as BrachCode,
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
-- Create date: 20/09/2022
-- Description:	A stored procedure to get payments approval queue
-- =============================================
CREATE PROCEDURE sp_GetPaymentApprovalQueue
	@TaxType int = 0
AS
BEGIN
	SET NOCOUNT ON;

    -- OBR Custom
	Select	a.PaymentCode,
			a.CreateDate,
			a.Amount,
			a.ModeCode,
			a.ApplyCharge,
			a.UserCode,
			a.Cr_Account as CrAccount,
			a.Dr_Account as DrAccount,
			a.TaxType,
			b.FileCode,
			b.CompanyName,
			SUBSTRING(b.CompanyName,0,35) +
			case when Len(b.CompanyName) > 35 then '....' else '' end as CompanyNameMini,
			b.DclntName,
			c.UserName,
			d.ModeName as PayModeName,
			'OBR CUSTOM' as TypeName,
			b.CompanyName as Details,
			Concat('Year:', b.RegYear) as TaxPeriod
	From Payments a
	Inner Join TaxFiles b on a.FileCode = b.FileCode
	Inner Join Users c on c.UserCode = a.UserCode
	Inner Join PaymentModes d on d.ModeCode = a.ModeCode
	Where a.StatusCode = 0
	UNION ALL
	-- Miarie Tax
	Select	a.PaymentCode,
			a.CreateDate,
			a.Amount,
			a.ModeCode,
			a.ApplyCharge,
			a.UserCode,
			a.Cr_Account as CrAccount,
			a.Dr_Account as DrAccount,
			a.TaxType,
			b.FileCode,
			'' as CompanyName,
			'' as CompanyNameMini,
			b.PayerName as DclntName,
			c.UserName,
			d.ModeName as PayModeName,
			'MIARIE TAX' as TypeName,
			b.Descr as Details,
			b.Period as TaxPeriod
	From Payments a
	Inner Join MiarieTaxFiles b on a.FileCode = b.FileCode
	Inner Join Users c on c.UserCode = a.UserCode
	Inner Join PaymentModes d on d.ModeCode = a.ModeCode
	Where a.StatusCode = 0
END
GO

-- =============================================
-- Author:		Alex Mugo
-- Create date: 20/09/2022
-- Description:	A stored procedure to get payment approval item
-- =============================================
CREATE PROCEDURE sp_GetPaymentApprovalItem
	@PaymentCode int
AS
BEGIN
	SET NOCOUNT ON;
	Declare @TaxType int

	Select @TaxType = TaxType From Payments Where PaymentCode = @PaymentCode

	If(@TaxType = 10)
	Begin
		-- OBR Custom
		Select	a.PaymentCode,
				a.CreateDate,
				a.Amount,
				a.ModeCode,
				a.ApplyCharge,
				a.UserCode,
				a.Cr_Account as CrAccount,
				a.Dr_Account as DrAccount,
				a.TaxType,
				b.FileCode,
				b.CompanyName,
				SUBSTRING(b.CompanyName,0,35) +
				case when Len(b.CompanyName) > 35 then '....' else '' end as CompanyNameMini,
				b.DclntName,
				c.UserName,
				d.ModeName as PayModeName,
				'OBR CUSTOM' as TypeName,
				b.CompanyName as Details,
				Concat('Year:', b.RegYear) as TaxPeriod
		From Payments a
		Inner Join TaxFiles b on a.FileCode = b.FileCode
		Inner Join Users c on c.UserCode = a.UserCode
		Inner Join PaymentModes d on d.ModeCode = a.ModeCode
		Where a.StatusCode = 0 and a.PaymentCode = @PaymentCode
	End
	Else
	Begin
		-- Miarie Tax
		Select	a.PaymentCode,
				a.CreateDate,
				a.Amount,
				a.ModeCode,
				a.ApplyCharge,
				a.UserCode,
				a.Cr_Account as CrAccount,
				a.Dr_Account as DrAccount,
				a.TaxType,
				b.FileCode,
				b.PayerName as DclntName,
				c.UserName,
				d.ModeName as PayModeName,
				'MIARIE TAX' as TypeName,
				b.Descr as Details,
				b.Period as TaxPeriod
		From Payments a
		Inner Join MiarieTaxFiles b on a.FileCode = b.FileCode
		Inner Join Users c on c.UserCode = a.UserCode
		Inner Join PaymentModes d on d.ModeCode = a.ModeCode
		Where a.StatusCode = 0 and a.PaymentCode = @PaymentCode
	End
END
GO

-- =============================================
-- Author:		Modified By Alex Mugo
-- Create date: 20/09/2022
-- Description:	A stored procedure to supervise tax payment
-- =============================================
ALTER PROCEDURE [dbo].[sp_SuperviseTaxPayment] 
	@PaymentCode int,
	@Action int,
	@Reason varchar(150),
	@UserCode int
AS
BEGIN
	SET NOCOUNT ON;
	Declare @PaymentMode int,
			@RespStat int = 0,
			@RespMsg varchar(150) = '',
			@UserRole int,
			@StatusCode int = 1,
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
			@Approval bit,
			@ModeId int,
			@GL_Account varchar(20),
			@ModeCode int,
			@ApplyCharge bit,
			@Dr_Account varchar(20),
			@Amount decimal(18,2),
			@TxnCharge decimal(18,2),
			@ChequeNo varchar(15),
			@FileCode int,
			@DeclName varchar(50)='',
			@DeclCode varchar(20)='',
			@BranchCode int,
			@TaxType int,
			@Maker int,
			@Data1 varchar(250) = '',
			@Data2 varchar(250) = '',
			@Data3 varchar(250) = '',
			@Data4 varchar(250) = '',
			@Data5 varchar(250) = '',
			@Data6 varchar(250) = '',
			@Data7 varchar(250) = '',
			@Data8 varchar(250) = '',
			@Data9 varchar(250) = '',
			@Data10 varchar(250) = ''

    BEGIN TRY
		---- Get payment details
		Select	@ApplyCharge = ApplyCharge, 
				@PaymentMode = ModeCode, 
				@Amount = Amount, 
				@Dr_Account = Dr_Account, 
				@CrAccount = Cr_Account,
				@Narration = Remarks,
				@ChequeNo = Extra1,
				@FileCode = FileCode,
				@TaxType = TaxType, 
				@Maker = UserCode
		From Payments Where PaymentCode = @PaymentCode
		If(@TaxType Is Null)
		Begin
			Select  1 as RespStatus, 'Invalid tax record!' as RespMessage
			Return
		End

		---- Validate
		If(@UserCode = @Maker)
		Begin
			Select  1 as RespStatus, 'You CANNOT approve your own transactions!' as RespMessage
			Return
		End

		Select @UserRole = UserRole From Users Where UserCode = @UserCode
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

		---- Status code
		If(@Action = 1)
		Begin
			--- Update record
			Update Payments Set StatusCode = 1, Checker = @UserCode Where PaymentCode = @PaymentCode

			Select @ChargeCode = ChargeCode, @TxnCharge = Amount From TaxCharges Where PaymentCode = @PaymentCode
			Set @ChargeCode = Coalesce(@ChargeCode, 0)
			Set @TxnCharge = Coalesce(@TxnCharge, 0)

			---- Get core banking transaction code
			Select @CBSTxnCode = CBS_Txn_Code From PaymentModes Where ModeCode = @PaymentMode
			set @ModeCode = @PaymentMode
			Set @TaxAmount = @Amount

			If(@ApplyCharge = 1)
				Set @TaxAmount = @Amount

			---- Get other details
			Select	@CashAccount = (CASE WHEN a.ItemName = 'CASH_DR_ACCOUNT' THEN a.ItemValue ELSE @CashAccount END ),
					@CrAccount = (CASE WHEN a.ItemName = 'TAX_CR_ACCOUNT' THEN a.ItemValue ELSE @CrAccount END),
					@PostUrl = (CASE WHEN a.ItemName = 'CBS_POST_URL' THEN a.ItemValue ELSE @PostUrl END),
					@Officer = (CASE WHEN a.ItemName = 'CBS_OFFICER' THEN a.ItemValue ELSE @Officer END),
					@DefaultTxnCode = (CASE WHEN a.ItemName = 'CBS_TXN_CODE' THEN a.ItemValue ELSE @DefaultTxnCode END),
					@TxnType = (CASE WHEN a.ItemName = 'CBS_TXN_TYPE' THEN a.ItemValue ELSE @TxnType END),
					@DDName = (CASE WHEN a.ItemName = 'CBS_TNCTR_NAME' THEN a.ItemValue ELSE @DDName END)
					--@Narration =(CASE WHEN a.ItemName = 'CBS_NARRATION' THEN a.ItemValue ELSE @Narration END)
			From SysSettings a 
			Where a.ItemName In('CASH_DR_ACCOUNT','TAX_CR_ACCOUNT','CBS_POST_URL','CBS_OFFICER','CBS_TXN_CODE',
							'CBS_TXN_TYPE','CBS_TNCTR_NAME')			

			If(@TaxType = 10)
				Select @Narration ='TX' + Cast(@FileCode as Varchar(15)) + ' OBR CSTM, REF: ' + DclntCode + ' PAYER:' + DclntName From TaxFiles Where FileCode = @FileCode 
			Else If(@TaxType = 11)
				Select @Narration ='TX' + Cast(@FileCode as Varchar(15)) + ', OBR DMSTC, REF: ' + DeclNo + ' PAYER:' + PayerName From DomesticTaxFiles Where FileCode = @FileCode 
			Else If(@TaxType = 12)
				Select @Narration ='TX' + Cast(@FileCode as Varchar(15)) + ', MIARIE TAX, REF: ' + NoteNo + ' PAYER:' + PayerName From MiarieTaxFiles Where FileCode = @FileCode 

			--- Get charge settings			
			Select	@BalUrl =(CASE WHEN a.ItemName = 'CBS_BALANCE_URL' THEN a.ItemValue ELSE @BalUrl END),					
					@ChargeCrAccount =(CASE WHEN a.ItemName = 'CHARGE_CR_ACCOUNT' THEN a.ItemValue ELSE @ChargeCrAccount END),
					@ChargeTxnCode =(CASE WHEN a.ItemName = 'CHARGE_CBS_TXN_CODE' THEN a.ItemValue ELSE @ChargeTxnCode END),
					@ChargeTxnType =(CASE WHEN a.ItemName = 'CHARGE_CBS_TXN_TYPE' THEN a.ItemValue ELSE @ChargeTxnType END)					
			From SysSettings a 
			Where a.ItemName In('CBS_BALANCE_URL','CHARGE_CR_ACCOUNT','CHARGE_CBS_TXN_CODE','CHARGE_CBS_TXN_TYPE')
			
			If(@TxnCharge > 0)
			Begin
				Set @Narration = 'TX' + Cast(@FileCode as Varchar(15)) + '/' + Cast(@ChargeCode as varchar(15)) + ' Declarant Code : ' + @DeclCode + ' Declarant :' + @DeclName

				If(@TaxType = 10)
					Select @Narration ='TX' + Cast(@FileCode as Varchar(15)) + Cast(@ChargeCode as varchar(15)) +  ' OBR CSTM, REF: ' + DclntCode + ' PAYER:' + DclntName From TaxFiles Where FileCode = @FileCode 
				Else If(@TaxType = 11)
					Select @Narration ='TX' + Cast(@FileCode as Varchar(15)) + Cast(@ChargeCode as varchar(15)) +  ', OBR DMSTC, REF: ' + DeclNo + ' PAYER:' + PayerName From DomesticTaxFiles Where FileCode = @FileCode 
				Else If(@TaxType = 12)
					Select @Narration ='TX' + Cast(@FileCode as Varchar(15)) + Cast(@ChargeCode as varchar(15)) +  ', MIARIE TAX, REF: ' + NoteNo + ' PAYER:' + PayerName From MiarieTaxFiles Where FileCode = @FileCode 

			End

			Set @CBSTxnCode = Coalesce(@CBSTxnCode, @DefaultTxnCode)
		
			Select @BranchCode=  CBS_Code From Branches where BranchCode = (Select BranchCode From Users where UserCode = @UserCode);
		
			If(@TaxType = 12)
			Begin
				Select	@Data1 = Case ItemName when 'FINBRIDGE_BASE_URL' then ItemValue else @Data1 end,
						@Data2 = Case ItemName when 'FINBRIDGE_APPID' then ItemValue else @Data2 end,
						@Data3 = Case ItemName when 'FINBRIDGE_APPKEY' then ItemValue else @Data3 end
				From SysSettings a 
				Where a.ItemName In('FINBRIDGE_BASE_URL','FINBRIDGE_APPID','FINBRIDGE_APPKEY')

				Select	@Data5 =  Cast(FileCode as varchar(20)),
						@Data6 = NoteNo,
						@Data7 = Cast(Amount as varchar(20)),
						@Data8 = Cast(NoteType as varchar(20))
				From MiarieTaxFiles Where FileCode = @FileCode
			End

			Set @Data4 = Cast(@TaxType as varchar(10))

			---- Create response
			Select  @RespStat as RespStatus, 
					@RespMsg as RespMessage, 
					@PaymentCode as PaymentCode,
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
					'TX' + Cast(@PaymentCode as Varchar(15)) as MainRefNo,
					'0' as MainFlag,
					@Dr_Account as MainDrAccount,
					@CrAccount as MainCrAccount,
					@Narration as MainNarration,
					@ChargeTxnCode as ChargeTxnCode,
					@ChargeTxnType as ChargeTxnType,
					'TX' + Cast(@PaymentCode as Varchar(15)) + '/' + Cast(@ChargeCode as Varchar(15)) as ChargeRefNo,
					'0' as ChargeFlag,
					@Dr_Account as ChargeDrAccount,
					@ChargeCrAccount as ChargeCrAccount,
					@ChargeNarration as ChargeNarration,
					@TxnCharge as ChargeAmount,
					@ChargeCode as ChargeCode,
					@ChequeNo as ChequeNo,
					@BranchCode as BrachCode,
					@Data1 as Data1,
					@Data2 as Data2,
					@Data3 as Data3,
					@Data4 as Data4,
					@Data5 as Data5,
					@Data6 as Data6,
					@Data7 as Data7,
					@Data8 as Data8,
					@Data9 as Data9,
					@Data10 as Data10
		End
		If(@Action = 0)
		Begin
			--- Update record
			Update Payments Set StatusCode = 2, Checker = @UserCode, Reason = @Reason Where PaymentCode = @PaymentCode

			Set @RespStat = 0;
			Set @RespMsg = 'Record rejected Successfully';
			Select  @RespStat as RespStatus, 
					@RespMsg as RespMessage;
		End		
	END TRY
	BEGIN CATCH
		SELECT @RespMsg = ERROR_MESSAGE(), @RespStat = 2;
		Select  @RespStat as RespStatus, @RespMsg as RespMessage
	END CATCH
END
Go

-- =============================================
-- Author:		Alex Mugo
-- Create date: 04/06/2018
-- Description:	A stored procedure to update tax payment status
-- =============================================
ALTER PROCEDURE [dbo].[sp_UpdateTaxPaymentStatus]
	@PaymentCode int,
	@Stat int,
	@Msg varchar(150)
AS
BEGIN
	SET NOCOUNT ON;
	Declare @Code int,
			@RespStat int = 0,
			@RespMsg varchar(150) = '',
			@FileCode int,
			@Attempts int,
			@PayStat int,
			@TaxType int

    BEGIN TRY
		---- Validate
		Select	@FileCode = FileCode, 
				@Attempts = PostAttempts, 
				@PayStat = StatusCode,
				@TaxType = TaxType 
		From Payments Where PaymentCode = @PaymentCode
		If(@FileCode Is Null)
		Begin
			Select  1 as RespStatus, 'Invalid tax payment record!' as RespMessage
			Return
		End

		If(@Stat = 0)
		Begin
			Update Payments Set StatusCode = 1, Extra3 = @Msg Where PaymentCode = @PaymentCode

			If(@TaxType = 10)
				Update TaxFiles Set StatusCode = 5 Where FileCode = @FileCode
			Else If(@TaxType = 11)
				Update DomesticTaxFiles Set StatusCode = 5 Where FileCode = @FileCode
			Else If(@TaxType = 12)
				Update MiarieTaxFiles Set StatusCode = 5 Where FileCode = @FileCode
		End
		Else
		Begin
			Set @Attempts = @Attempts + 1
			If(@Attempts > 9)
				Set @PayStat = 2
			Update Payments Set PostAttempts = @Attempts, StatusCode = @PayStat, StatusMsg = @Msg Where PaymentCode = @PaymentCode
		End

		---- Create response
		Select  @RespStat as RespStatus, @RespMsg as RespMessage
	END TRY
	BEGIN CATCH
		SELECT @RespMsg = ERROR_MESSAGE(), @RespStat = 2;
		Select  @RespStat as RespStatus, @RespMsg as RespMessage
	END CATCH
END
Go



