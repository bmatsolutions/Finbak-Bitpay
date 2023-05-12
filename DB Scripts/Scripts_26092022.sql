Alter Table TaxTypes Add 
	Cr_Acc varchar(20),
	Cash_GL_Acc varchar(20),
	InChq_GL_Acc varchar(20),
	CertChq_GL_Acc varchar(20)
Go

Update TaxTypes Set 
	Cr_Acc = '20002576012', 
	Cash_GL_Acc = '110111007',
	InChq_GL_Acc = '128120008',
	CertChq_GL_Acc = '10128362851'
Where TypeCode = 12
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
		Begin
			Set @TxnCharge = 0

			---- Manage Accounts
			Select	@CrAccount = Cr_Acc,
					@Dr_Account = Case @ModeCode
									when 0 then Cash_GL_Acc
									when 2 then InChq_GL_Acc
									when 3 then CertChq_GL_Acc
									else @Dr_Account end
			From TaxTypes a Where a.TypeCode = @TaxType
		End

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
			Select	@PostUrl = (CASE WHEN a.ItemName = 'CBS_POST_URL' THEN a.ItemValue ELSE @PostUrl END),
					@Officer = (CASE WHEN a.ItemName = 'CBS_OFFICER' THEN a.ItemValue ELSE @Officer END),
					@DefaultTxnCode = (CASE WHEN a.ItemName = 'CBS_TXN_CODE' THEN a.ItemValue ELSE @DefaultTxnCode END),
					@TxnType = (CASE WHEN a.ItemName = 'CBS_TXN_TYPE' THEN a.ItemValue ELSE @TxnType END),
					@DDName = (CASE WHEN a.ItemName = 'CBS_TNCTR_NAME' THEN a.ItemValue ELSE @DDName END)
					--@Narration =(CASE WHEN a.ItemName = 'CBS_NARRATION' THEN a.ItemValue ELSE @Narration END)
			From SysSettings a 
			Where a.ItemName In('CBS_POST_URL','CBS_OFFICER','CBS_TXN_CODE','CBS_TXN_TYPE','CBS_TNCTR_NAME')			

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