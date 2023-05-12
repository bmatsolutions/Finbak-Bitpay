Use Bitpay

if exists(select name from sys.objects where name = N'sp_MakeTaxPayment')
	begin
		drop Procedure dbo.sp_MakeTaxPayment;
	end
Go

CREATE PROCEDURE [dbo].[sp_MakeTaxPayment] 
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
			@StatusCode int = 0,
			@TxnCharge decimal(18,2),
			@DeclName varchar(50)='',
			@DeclCode varchar(20)='',
			@Currency int,
			@CurrencyCode varchar(3)='',
			@branchCode int

    BEGIN TRY
		---- Validate
		If Not Exists(Select Id From Users Where UserCode = @UserCode)
		Begin
			Select  1 as RespStatus, 'Invalid user details!' as RespMessage
			Return
		End
		If Not Exists(Select Id From TaxFiles Where FileCode = @FileCode)
		Begin
			Select  1 as RespStatus, 'Invalid tax file details!' as RespMessage
			Return
		End
		--validate sortcode
		if((@Extra2!=null or @Extra2!=''))
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
				--@Narration =(CASE WHEN a.ItemName = 'CBS_NARRATION' THEN a.ItemValue ELSE @Narration END),
				@DefaultCharge =(CASE WHEN a.ItemName = 'TAX_CHARGE_AMOUNT' THEN a.ItemValue ELSE @DefaultCharge END)
		From SysSettings a 
		Where a.ItemName In('CASH_DR_ACCOUNT','TAX_CR_ACCOUNT','CBS_POST_URL','CBS_OFFICER','CBS_TXN_CODE',
							'CBS_TXN_TYPE','CBS_TNCTR_NAME','TAX_CHARGE_AMOUNT')

		

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
			ApplyCharge
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
			@ApplyCharge
		)

		Select @DeclCode=DclntCode,@DeclName=DclntName,@Currency=Currency from TaxFiles where FileCode=@FileCode 

		Select @CurrencyCode=ItemValue from SysSettings where ItemName=@Currency
		set @Narration='TX'+Cast(@Code as Varchar(15))+' Declarant Code : '+@DeclCode+' Declarant :'+@DeclName
		--- Create tax commission
		If(@ApplyCharge = 1)
		Begin
			--- Get charge settings
			Select	@BalUrl =(CASE WHEN a.ItemName = 'CBS_BALANCE_URL' THEN a.ItemValue ELSE @BalUrl END),					
					@ChargeCrAccount =(CASE WHEN a.ItemName = 'CHARGE_CR_ACCOUNT' THEN a.ItemValue ELSE @ChargeCrAccount END),
					@ChargeTxnCode =(CASE WHEN a.ItemName = 'CHARGE_CBS_TXN_CODE' THEN a.ItemValue ELSE @ChargeTxnCode END),
					@ChargeTxnType =(CASE WHEN a.ItemName = 'CHARGE_CBS_TXN_TYPE' THEN a.ItemValue ELSE @ChargeTxnType END)
					--@ChargeNarration =(CASE WHEN a.ItemName = 'CHARGE_CBS_NARRATION' THEN a.ItemValue ELSE @ChargeNarration END)
			From SysSettings a 
			Where a.ItemName In('CBS_BALANCE_URL','CHARGE_CR_ACCOUNT','CHARGE_CBS_TXN_CODE','CHARGE_CBS_TXN_TYPE')

			--- Generate code
			Delete From @RunNoTable
			INSERT INTO @RunNoTable
			Exec sp_GenerateRunNo 'CHG1' 
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
		set @ChargeNarration='TX'+Cast(@Code as Varchar(15))+'/'+Cast(@ChargeCode as varchar(15))+' Declarant Code : '+@DeclCode+' Declarant :'+@DeclName
		--select @branchCode= BranchCode from Users where UserCode=@UserCode;	
		select @branchCode= CBS_Code from Branches where BranchCode=(select BranchCode from Users where UserCode=@UserCode);
		---- Create response
		Select  @RespStat as RespStatus, 
				@RespMsg as RespMessage, 
				@Code as PaymentCode,
				@ModeCode as PaymentMode,
				@ApplyCharge as ApplyCharge,
				@PostUrl as PostUrl,
				@BalUrl as BalanceUrl,
				@CurrencyCode as CurrencyCode,
				@TaxAmount as MainAmount,
				@CBSTxnCode as MainTxnCode,
				@TxnType as MainTxnType,
				@Officer as CBSOfficer,
				@DDName as TransactorName,
				'TX' + Cast(@Code as Varchar(15)) as MainRefNo,
				'0' as MainFlag,
				@Dr_Account as MainDrAccount,
				@CrAccount as MainCrAccount,
				@Narration as MainNarration,
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

if exists(select name from sys.objects where name = N'sp_SuperviseTaxPayment')
	begin
		drop Procedure dbo.sp_SuperviseTaxPayment;
	end
Go
Create PROCEDURE [dbo].[sp_SuperviseTaxPayment] 
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
			@branchCode int

    BEGIN TRY
		---- Validate
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

			---- Get payment details
			Select	@ApplyCharge = ApplyCharge, 
					@PaymentMode = ModeCode, 
					@Amount = Amount, 
					@Dr_Account = Dr_Account, 
					@CrAccount = Cr_Account,
					@Narration = Remarks,
					@ChequeNo = Extra1,
					@FileCode = FileCode
			From Payments Where PaymentCode = @PaymentCode

			Select @ChargeCode = ChargeCode, @TxnCharge = Amount From TaxCharges Where PaymentCode = @PaymentCode
			Set @ChargeCode = Coalesce(@ChargeCode, 0)
			Set @TxnCharge = Coalesce(@TxnCharge, 0)

			---- Get core banking transaction code
			Select @CBSTxnCode = CBS_Txn_Code From PaymentModes Where ModeCode = @PaymentMode

			Set @TaxAmount = @Amount
			If(@ApplyCharge = 1)
				Set @TaxAmount = @Amount

			---- Get other details
			Select	@CashAccount =(CASE WHEN a.ItemName = 'CASH_DR_ACCOUNT' THEN a.ItemValue ELSE @CashAccount END ),
					@CrAccount =(CASE WHEN a.ItemName = 'TAX_CR_ACCOUNT' THEN a.ItemValue ELSE @CrAccount END),
					@PostUrl =(CASE WHEN a.ItemName = 'CBS_POST_URL' THEN a.ItemValue ELSE @PostUrl END),
					@Officer =(CASE WHEN a.ItemName = 'CBS_OFFICER' THEN a.ItemValue ELSE @Officer END),
					@DefaultTxnCode =(CASE WHEN a.ItemName = 'CBS_TXN_CODE' THEN a.ItemValue ELSE @DefaultTxnCode END),
					@TxnType =(CASE WHEN a.ItemName = 'CBS_TXN_TYPE' THEN a.ItemValue ELSE @TxnType END),
					@DDName =(CASE WHEN a.ItemName = 'CBS_TNCTR_NAME' THEN a.ItemValue ELSE @DDName END)
					--@Narration =(CASE WHEN a.ItemName = 'CBS_NARRATION' THEN a.ItemValue ELSE @Narration END)
			From SysSettings a 
			Where a.ItemName In('CASH_DR_ACCOUNT','TAX_CR_ACCOUNT','CBS_POST_URL','CBS_OFFICER','CBS_TXN_CODE',
							'CBS_TXN_TYPE','CBS_TNCTR_NAME')
			Select @DeclCode=DclntCode,@DeclName=DclntName from TaxFiles where FileCode=@FileCode 

		    set @Narration='TX'+Cast(@PaymentCode as Varchar(15))+' Declarant Code : '+@DeclCode+' Declarant :'+@DeclName
			--- Get charge settings
			Select	@BalUrl =(CASE WHEN a.ItemName = 'CBS_BALANCE_URL' THEN a.ItemValue ELSE @BalUrl END),					
					@ChargeCrAccount =(CASE WHEN a.ItemName = 'CHARGE_CR_ACCOUNT' THEN a.ItemValue ELSE @ChargeCrAccount END),
					@ChargeTxnCode =(CASE WHEN a.ItemName = 'CHARGE_CBS_TXN_CODE' THEN a.ItemValue ELSE @ChargeTxnCode END),
					@ChargeTxnType =(CASE WHEN a.ItemName = 'CHARGE_CBS_TXN_TYPE' THEN a.ItemValue ELSE @ChargeTxnType END)
					--@ChargeNarration =(CASE WHEN a.ItemName = 'CHARGE_CBS_NARRATION' THEN a.ItemValue ELSE @ChargeNarration END)
			From SysSettings a 
			Where a.ItemName In('CBS_BALANCE_URL','CHARGE_CR_ACCOUNT','CHARGE_CBS_TXN_CODE','CHARGE_CBS_TXN_TYPE')
          
		  set @ChargeNarration='TX'+Cast(@PaymentCode as Varchar(15))+'/'+Cast(@ChargeCode as varchar(15))+' Declarant Code : '+@DeclCode+' Declarant :'+@DeclName

		  Set @CBSTxnCode = Coalesce(@CBSTxnCode, @DefaultTxnCode)
		--  select @branchCode= BranchCode from Users where UserCode=@UserCode;	
		select @branchCode= CBS_Code from Branches where BranchCode=(select BranchCode from Users where UserCode=@UserCode);
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
				@branchCode as brachCode
				return;
		End
		If(@Action = 0)
		Begin
			--- Update record
			Update Payments Set StatusCode = 2, Checker = @UserCode, Reason = @Reason Where PaymentCode = @PaymentCode
			set @RespStat=1;
			set @RespMsg= 'Record rejected Successfully';
			SELECT  @RespStat as RespStatus, 
				@RespMsg as RespMessage;
			return;
		End
		
	END TRY
	BEGIN CATCH
		SELECT @RespMsg = ERROR_MESSAGE(), @RespStat = 2;
		Select  @RespStat as RespStatus, @RespMsg as RespMessage
	END CATCH
END
GO