
Create Table TxnTypes(
	Id int not null identity,
	TaxType int not null,
	PayMode int not null,
	CBSCode varchar(10) not null,
	GLAccount varchar(20),
	Constraint pk_TxnTypes primary key(Id),
	Constraint uk_TxnTypes_TaxType_PayMode unique(TaxType,PayMode)
)
Go

Insert Into TxnTypes(TaxType, PayMode, CBSCode, GLAccount) Values(10,0,'601','110111005')
Insert Into TxnTypes(TaxType, PayMode, CBSCode) Values(10,1,'602')
Insert Into TxnTypes(TaxType, PayMode, CBSCode) Values(10,2,'603')
Insert Into TxnTypes(TaxType, PayMode, CBSCode, GLAccount) Values(10,3,'604','10031923081')
Insert Into TxnTypes(TaxType, PayMode, CBSCode, GLAccount) Values(11,0,'601','110111005')
Insert Into TxnTypes(TaxType, PayMode, CBSCode) Values(11,1,'602')
Insert Into TxnTypes(TaxType, PayMode, CBSCode) Values(11,2,'603')
Insert Into TxnTypes(TaxType, PayMode, CBSCode, GLAccount) Values(11,3,'604','10031923081')
Insert Into TxnTypes(TaxType, PayMode, CBSCode, GLAccount) Values(12,0,'401','110111007')
Insert Into TxnTypes(TaxType, PayMode, CBSCode) Values(12,1,'402')
Insert Into TxnTypes(TaxType, PayMode, CBSCode) Values(12,2,'403')
Insert Into TxnTypes(TaxType, PayMode, CBSCode, GLAccount) Values(12,3,'404','10128362851')
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

		--- Get CBS settings
		Select @TxnType = CBSCode, @GL_Account = GLAccount From TxnTypes a Where a.TaxType = @TaxType and a.PayMode = @ModeCode

		Select	@ModeId = Id, 
				@Approval = Approval, 
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
			Set @GL_Account = Coalesce(@CashAccount,'')
		End

		---- Validate GL account 
		If(@ModeCode In(0,3,4))
		Begin
			Set @Dr_Account = Coalesce(@GL_Account,'')
			If(Len(@Dr_Account) = 0)
			Begin
				Select  1 as RespStatus, 'GL account(DR Account) is NOT set!' as RespMessage
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
									when 3 then CertChq_GL_Acc
									else @Dr_Account end
			From TaxTypes a Where a.TypeCode = @TaxType

			--- Finbridge settings
			Select	@Data1 = Case ItemName when 'FINBRIDGE_BASE_URL' then ItemValue else @Data1 end,
					@Data2 = Case ItemName when 'FINBRIDGE_APPID' then ItemValue else @Data2 end,
					@Data3 = Case ItemName when 'FINBRIDGE_APPKEY' then ItemValue else @Data3 end
			From SysSettings a 
			Where a.ItemName In('FINBRIDGE_BASE_URL','FINBRIDGE_APPID','FINBRIDGE_APPKEY')
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
					@DDName = (CASE WHEN a.ItemName = 'CBS_TNCTR_NAME' THEN a.ItemValue ELSE @DDName END)
					--@Narration =(CASE WHEN a.ItemName = 'CBS_NARRATION' THEN a.ItemValue ELSE @Narration END)
			From SysSettings a 
			Where a.ItemName In('CBS_POST_URL','CBS_OFFICER','CBS_TXN_CODE','CBS_TXN_TYPE','CBS_TNCTR_NAME')	
			
			--- Get CBS settings
			Select @TxnType = CBSCode From TxnTypes a Where a.TaxType = @TaxType and a.PayMode = @ModeCode

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


---==================== 06/10/2022 ===================================


-- =============================================
-- Author:		Alex Mugo
-- Create date: 13/06/2018
-- Description:	A stored procedure to get payment status
-- =============================================
ALTER PROCEDURE [dbo].[sp_GetPaymentStatus] 
	@Code int
AS
BEGIN
	SET NOCOUNT ON;
	
	Select 	Concat('Payment ', a.ReceiptNo,' ', b.StatusName) as Data1,
			@Code as Data2,
			Case a.StatusCode when 1 then 1 else 0 end as Data3,
			a.StatusCode as Data4,
			b.StatusName as Data5
	From Payments a
	Inner Join PaymentStatus b on a.StatusCode = b.StatusCode
	Where PaymentCode = @Code
END
Go

--=================== 12/10/2022 ===============================

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
			@Attempts int,
			@NoteType int = 1

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
				Select @TaxAmount = Amount, @Currency = 1, @NoteType = NoteType From MiarieTaxFiles Where FileCode = @FileCode

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

			If(@TaxType = 12 and @NoteType = 1)
			Begin
				--- For Mairie tax note amount can be less or more
				Set @ExpectedAmount = @ExpectedAmount
			End
			Else
			Begin
				If(@ExpectedAmount <> @Amount)
				Begin
					Select  1 as RespStatus, 'Invalid amount! Expected amount is: ' + Cast(@ExpectedAmount as Varchar(20)) as RespMessage
					Return
				End
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

Alter Table MiarieTaxFiles Add Title varchar(250)
Go


ALTER view [dbo].[vw_MiarieTaxPayments]
As
Select	a.FileCode,
		a.CreateDate,
		a.Amount,
		a.PayerName as TaxPayerName,
		a.Period,
		a.NoteNo as TaxNoteNo,
		a.RefNo,
		a.NoteType,
		a.Descr,
		a.Title,
		a.StatusCode,
		b.Remarks,		
		b.PaymentCode,
		b.Cr_Account,
		b.Dr_Account,
		b.ReceiptNo,
		b.Amount as PaidAmount,
		b.CreateDate as PayDate,
		b.Extra1 as ChequeNo,
		b.StatusCode as PayStatus,
		c.ModeName as PayModeName,
		d.UserName,
		Case a.NoteType
			when 1 then 'TAX NOTE'
			else 'TAX NOTICE'
		End as TypeName
From MiarieTaxFiles a 
Inner join Payments b on a.FileCode = b.FileCode
Inner join PaymentModes c on c.ModeCode = b.ModeCode
Inner join Users d on d.UserCode = a.UserCode 
Where TaxType = 12
GO

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
	@Title varchar(250)
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
			Period,
			Title
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
			@Title
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

---============================ 13/10/2022 ====================================

-- =============================================
-- Author:		Alex Mugo
-- Create date: 13/10/2022
-- Description:	A stored procedure to get mairie daily report data
-- =============================================
CREATE PROCEDURE sp_GetMairieReportData
AS
BEGIN
	SET NOCOUNT ON;
	Declare @Start Date

	Declare @ReportData Table(
		Id int identity not null,
		BankRef varchar(20),
        Amount decimal(18,2),
        RefNo varchar(20),
        ItemType int
	)

	--- Query data
	Insert Into @ReportData(
		BankRef,
        Amount,
        RefNo,
        ItemType
	)
	Select	b.ReceiptNo,
			b.Amount,
			a.NoteNo,
			a.NoteType
	From MiarieTaxFiles a
	Inner Join Payments b on a.FileCode = b.FileCode and b.TaxType = 12
	Where Cast(b.CreateDate as Date) = Cast(GETDATE() as Date) and b.StatusCode = 1

    -- Return data
	Select * From @ReportData
END
Go


--======================= 18/10/2022 ============================


Alter Table MiarieTaxFiles Add PayPartial bit not null default 0, TypeName varchar(250)
Go

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
			@PayPartial bit = 0

    BEGIN TRY
		---- Validate
		If Not Exists(Select Id From Users Where UserCode = @UserCode)
		Begin
			Select  1 as RespStatus, 'Invalid user details!' as RespMessage
			Return
		End

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

		---- Get charge

		---- Create response
		Select  @RespStat as RespStatus, 
				@RespMsg as RespMessage, 
				@Code as Data1, 
				@Charge as Data2,
				@PayPartial as Data3
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
			@Attempts int,
			@NoteType int = 1,
			@PayPartial bit = 0

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
				Select @TaxAmount = Amount, @Currency = 1, @NoteType = NoteType, @PayPartial = PayPartial From MiarieTaxFiles Where FileCode = @FileCode

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

			If(@PayPartial = 0)
			Begin
				If(@ExpectedAmount <> @Amount)
				Begin
					Select  1 as RespStatus, 'Invalid amount! Expected amount is: ' + Cast(@ExpectedAmount as Varchar(20)) as RespMessage
					Return
				End
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

		--- Get CBS settings
		Select @TxnType = CBSCode, @GL_Account = GLAccount From TxnTypes a Where a.TaxType = @TaxType and a.PayMode = @ModeCode

		Select	@ModeId = Id, 
				@Approval = Approval, 
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
			Set @GL_Account = Coalesce(@CashAccount,'')
		End

		---- Validate GL account 
		If(@ModeCode In(0,3,4))
		Begin
			Set @Dr_Account = Coalesce(@GL_Account,'')
			If(Len(@Dr_Account) = 0)
			Begin
				Select  1 as RespStatus, 'GL account(DR Account) is NOT set!' as RespMessage
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
									when 3 then CertChq_GL_Acc
									else @Dr_Account end
			From TaxTypes a Where a.TypeCode = @TaxType

			--- Finbridge settings
			Select	@Data1 = Case ItemName when 'FINBRIDGE_BASE_URL' then ItemValue else @Data1 end,
					@Data2 = Case ItemName when 'FINBRIDGE_APPID' then ItemValue else @Data2 end,
					@Data3 = Case ItemName when 'FINBRIDGE_APPKEY' then ItemValue else @Data3 end
			From SysSettings a 
			Where a.ItemName In('FINBRIDGE_BASE_URL','FINBRIDGE_APPID','FINBRIDGE_APPKEY')
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
				'BTPY_TX' + Cast(@Code as Varchar(15)) as MainRefNo,
				'0' as MainFlag,
				@Dr_Account as MainDrAccount,
				@CrAccount as MainCrAccount,
				Coalesce(@Remarks,@Narration,@Decl) as MainNarration,
				@ChargeTxnCode as ChargeTxnCode,
				@ChargeTxnType as ChargeTxnType,
				'BTPY_TX' + Cast(@Code as Varchar(15)) + '/' + Cast(@ChargeCode as Varchar(15)) as ChargeRefNo,
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

--========================= 25/10/2022 ======================================


-- =============================================
-- Author:		Alex Mugo
-- Create date: 20/09/2022
-- Description:	A stored procedure to get payments approval queue
-- =============================================
ALTER PROCEDURE [dbo].[sp_GetPaymentApprovalQueue]
	@TaxType int = 0
AS
BEGIN
	SET NOCOUNT ON;
	Declare @Data as Table(
		PaymentCode int,
		CreateDate datetime,
		Amount decimal(18,2),
		ModeCode int,
		ApplyCharge bit,
		UserCode int,
		CrAccount varchar(20),
		DrAccount varchar(20),
		TaxType int,
		FileCode int,
		CompanyName varchar(100),
		CompanyNameMini varchar(250),
		DclntName varchar(100),
		UserName varchar(50),
		PayModeName varchar(30),
		TypeName varchar(20),
		Details varchar(250),
		TaxPeriod varchar(50)
	)

    -- OBR Custom
	If(@TaxType = 0 or @TaxType = 10)
	Begin
		Insert Into @Data
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
	End

	-- OBR Domestic
	If(@TaxType = 0 or @TaxType = 11)
	Begin
		Insert Into @Data
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
				b.CustomerName as CompanyName,
				'' as CompanyNameMini,
				b.CustomerName as DclntName,
				c.UserName,
				d.ModeName as PayModeName,
				'OBR DOMESTIC' as TypeName,
				'' as Details,
				b. TaxPeriod
		From Payments a
		Inner Join DomesticTaxFiles b on a.FileCode = b.FileCode
		Inner Join Users c on c.UserCode = a.UserCode
		Inner Join PaymentModes d on d.ModeCode = a.ModeCode
		Where a.StatusCode = 0
	End

	-- Miarie Tax
	If(@TaxType = 0 or @TaxType = 12)
	Begin
		Insert Into @Data
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
	End

	Select * From @Data Order By CreateDate Desc
END
Go


-- =============================================
-- Author:		Alex Mugo
-- Create date: 20/09/2022
-- Description:	A stored procedure to get payment approval item
-- =============================================
ALTER PROCEDURE [dbo].[sp_GetPaymentApprovalItem]
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
	Else If(@TaxType = 11)
	Begin
		-- OBR Domestic
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
				b.CustomerName as CompanyName,
				'' as CompanyNameMini,
				b.CustomerName as DclntName,
				c.UserName,
				d.ModeName as PayModeName,
				'OBR DOMESTIC' as TypeName,
				'' as Details,
				b.TaxPeriod
		From Payments a
		Inner Join DomesticTaxFiles b on a.FileCode = b.FileCode
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
					@DDName = (CASE WHEN a.ItemName = 'CBS_TNCTR_NAME' THEN a.ItemValue ELSE @DDName END)
					--@Narration =(CASE WHEN a.ItemName = 'CBS_NARRATION' THEN a.ItemValue ELSE @Narration END)
			From SysSettings a 
			Where a.ItemName In('CBS_POST_URL','CBS_OFFICER','CBS_TXN_CODE','CBS_TXN_TYPE','CBS_TNCTR_NAME')	
			
			--- Get CBS settings
			Select @TxnType = CBSCode From TxnTypes a Where a.TaxType = @TaxType and a.PayMode = @ModeCode

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
					'BTPY_TX' + Cast(@PaymentCode as Varchar(15)) as MainRefNo,
					'0' as MainFlag,
					@Dr_Account as MainDrAccount,
					@CrAccount as MainCrAccount,
					@Narration as MainNarration,
					@ChargeTxnCode as ChargeTxnCode,
					@ChargeTxnType as ChargeTxnType,
					'BTPY_TX' + Cast(@PaymentCode as Varchar(15)) + '/' + Cast(@ChargeCode as Varchar(15)) as ChargeRefNo,
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

ALTER view [dbo].[vw_DomesticTaxFiles] as
SELECT
		a.FileCode,
		e.CreateDate,
		a.Cr_Account as AccountCredit,
		(select CatergoryName from DomesticTaxCatergory where CatergoryCode=e.TransactionType ) as TaxName,
		--(select CatergoryName from DomesticTaxCatergory where CatergoryCode=e.TransactionType ) as TransactionType,
		a.Remarks,
		e.Amount as TaxAmount,
		a.Extra4 as ReceivedFrom,
		Coalesce(Coalesce(e.DriverName,e.CustomerName),a.Extra4) as CustomerName,
		e.TaxPeriod as Period,
		e.DeclNo,
		e.Tin,
		e.CommuneName,
		a.Amount,
		a.PaymentCode,
		a.Cr_Account,
		a.Dr_Account,
		(select username from users where UserCode=a.UserCode ) as UserName,
		a.ReceiptNo,
		(select ModeName from PaymentModes where ModeCode=a.ModeCode)as Mode
From Payments a 
Inner join PaymentModes c on c.ModeCode = a.ModeCode
Inner join Users d on d.UserCode = a.UserCode 
Inner Join DomesticTaxFiles e on e.FileCode=a.FileCode
where a.StatusCode = 0 and a.TaxType = 11
GO

ALTER PROCEDURE [dbo].[sp_SuperviseDomesticTax] 
	@FileCode int,
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
			@Tin varchar(50)='',
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
			@PaymentCode int,
			@branchCode int,
            @IncomeTaxCatergory varchar(150) ='',
			@Declarant varchar(50) ='',
            @Period varchar(150) ='',
			@TaxName varchar(50) ='',
			@BankCode varchar(20),
            @PayerName varchar(150) =''

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
			Update Payments Set StatusCode = 1, Checker = @UserCode Where FileCode = @FileCode and TaxType = 11

			---- Get payment details
			Select	@ApplyCharge = ApplyCharge, 
					@PaymentMode = ModeCode, 
					@Amount = Amount, 
					@Dr_Account = Dr_Account, 
					@CrAccount = Cr_Account,
					@Narration = Remarks,
					@ChequeNo = Extra1,
					@Declarant= Coalesce(Extra4,''),
					@PaymentCode = PaymentCode
			From Payments Where FileCode = @FileCode
			Select @TaxAmount = Amount,@Tin = Tin from DomesticTaxFiles where FileCode=@FileCode
			Select @ChargeCode = ChargeCode, @TxnCharge = Amount From DomesticTaxCharges Where PaymentCode = @PaymentCode
			select @TaxName=dtc.CatergoryName from DomesticTaxFiles dtf inner Join DomesticTaxCatergory dtc on dtc.CatergoryCode=dtf.TransactionType where dtf.FileCode=@FileCode
			Set @ChargeCode = Coalesce(@ChargeCode, 0)
			Set @TxnCharge = Coalesce(@TxnCharge, 0)

			---- Get core banking transaction code
			Select @CBSTxnCode = CBS_Txn_Code From PaymentModes Where ModeCode = @PaymentMode
			set @ModeCode = @PaymentMode
			Set @TaxAmount = @Amount
			If(@ApplyCharge = 1)
				Set @TaxAmount = @Amount

			---- Get other details
			Select	@CashAccount =(CASE WHEN a.ItemName = 'CASH_DR_ACCOUNT' THEN a.ItemValue ELSE @CashAccount END ),
					
					@PostUrl =(CASE WHEN a.ItemName = 'CBS_POST_URL' THEN a.ItemValue ELSE @PostUrl END),
					@Officer =(CASE WHEN a.ItemName = 'CBS_OFFICER' THEN a.ItemValue ELSE @Officer END),
					@DefaultTxnCode =(CASE WHEN a.ItemName = 'CBS_TXN_CODE' THEN a.ItemValue ELSE @DefaultTxnCode END),
					@TxnType =(CASE WHEN a.ItemName = 'CBS_TXN_TYPE' THEN a.ItemValue ELSE @TxnType END),
					@BankCode = (Case When a.ItemName='DOM_BANK_CODE' Then a.ItemValue Else @BankCode END),
					@DDName =(CASE WHEN a.ItemName = 'CBS_TNCTR_NAME' THEN a.ItemValue ELSE @DDName END)
			From SysSettings a 
			Where a.ItemName In('CASH_DR_ACCOUNT','CBS_POST_URL','CBS_OFFICER','CBS_TXN_CODE',
							'CBS_TXN_TYPE','DOM_BANK_CODE','CBS_TNCTR_NAME')
			set @Narration=@BankCode+Cast(@FileCode as Varchar(15))+' Tax Paid: '+@TaxName+' Declarant: '+Coalesce(@Declarant,'')
			--- Get charge settings
			Select	@BalUrl =(CASE WHEN a.ItemName = 'CBS_BALANCE_URL' THEN a.ItemValue ELSE @BalUrl END),					
					@ChargeCrAccount =(CASE WHEN a.ItemName = 'CHARGE_CR_ACCOUNT' THEN a.ItemValue ELSE @ChargeCrAccount END),
					@ChargeTxnCode =(CASE WHEN a.ItemName = 'CHARGE_CBS_TXN_CODE' THEN a.ItemValue ELSE @ChargeTxnCode END),
					@ChargeTxnType =(CASE WHEN a.ItemName = 'CHARGE_CBS_TXN_TYPE' THEN a.ItemValue ELSE @ChargeTxnType END)
			From SysSettings a 
			Where a.ItemName In('CBS_BALANCE_URL','CHARGE_CR_ACCOUNT','CHARGE_CBS_TXN_CODE','CHARGE_CBS_TXN_TYPE')
          
			--Get Domestic tax information
			--SELECT @Period=TaxPeriod, @Amount=Amount,@PayerName = PayerName,@IncomeTaxCatergory = TransactionType FROM DomesticTaxFiles WHERE FileCode = @FileCode
		  Set @CBSTxnCode = Coalesce(@CBSTxnCode, @DefaultTxnCode)
		  select @branchCode= CBS_Code from Branches where BranchCode=(select BranchCode from Users where UserCode=@UserCode)	
		  set @ChargeNarration=@BankCode+Cast(@FileCode as Varchar(15))+'/'+Cast(@ChargeCode as Varchar(15))+' '+@TaxName+' '+Coalesce(@Declarant,'')

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
				@BankCode + Cast(@FileCode as Varchar(15)) as MainRefNo,
				'0' as MainFlag,
				@Dr_Account as MainDrAccount,
				@CrAccount as MainCrAccount,
				Coalesce(@Narration,'') as MainNarration,
				@ChargeTxnCode as ChargeTxnCode,
				@ChargeTxnType as ChargeTxnType,
				@BankCode + Cast(@FileCode as Varchar(15)) + '/' + Cast(@ChargeCode as Varchar(15)) as ChargeRefNo,
				'0' as ChargeFlag,
				@Dr_Account as ChargeDrAccount,
				@ChargeCrAccount as ChargeCrAccount,
				@ChargeNarration as ChargeNarration,
				@TxnCharge as ChargeAmount,
				@ChargeCode as ChargeCode,
				@ChequeNo as ChequeNo,
				@branchCode as brachCode,
				--@Period as Period,
				--@IncomeTaxCatergory as IncomeTaxCatergory ,
				--@Amount as Amount,
				--@PayerName as PayerName,
				@Filecode as fileCode,
				--@TaxAmount as TaxAmount,
				@Tin as Tin
				return;
		End
		If(@Action = 0)
		Begin
			--- Update record
			Update Payments Set StatusCode = 2, Checker = @UserCode, Reason = @Reason Where	FileCode = @FileCode and TaxType = 11
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
Go

-- =============================================
-- Author:		Brian Ogenge
-- Create date: 08/06/2020
-- Description:	A stored procedure to get domestic tax data
-- =============================================
ALTER PROCEDURE [dbo].[sp_getDomesticDetails] 
	@code int= 0
AS
BEGIN
	SET NOCOUNT ON;
	Declare @RespStat int = 0,
			@RespMsg varchar(150) = '',
			@CustomerName varchar(250) = '',
			@PayerName varchar(250)= '',
			@tin varchar(20)='',
			@period varchar(20)='',
			@filecode varchar(10)='',
			@taxcat varchar(6)='',
			@amount decimal(18,2) = 0.00,
			@declno varchar(50)='',
			@commune varchar(50)='',
			@adjust varchar(6)='',
			@delay varchar(20)='',
			@imma varchar(50)='',
			@chasis varchar(50)='',
			@carowner varchar(50)='',
			@edu varchar(50)='',
			@copies varchar(10)='',
			@contra varchar(50)='',
			@service varchar(50)='',
			@TransactionType varchar(50),
			@Product varchar(50)='',
			@BankCode varchar(10)=''

    	Begin
		if(@code =  0) 
		Begin
			Select 1 as RespStat,'Domestic Tax Details do not exist' as RespMessage
			Return
		End
		
		Select	@CustomerName = f.CustomerName,
				@PayerName= a.Extra4,
				@tin =f.Tin ,
				@period = f.TaxPeriod,
				@filecode =f.FileCode ,
				@taxcat=f.TransactionType,
				@amount =f.Amount ,
				@declno=f.DeclNo,
				@commune=f.CommuneName,
				@adjust=f.Adjustment,
				@delay=f.Delay,
				@TransactionType=TransactionType,
				@imma =f.Imma,
				@chasis=f.Chasis,
				@carowner=f.CarOnwer,
				@edu = f.Education,
				@copies=f.Copies,
				@contra=f.Contracavation,
				@service=f.Service,
				@Product =f.Product 
		 From DomesticTaxFiles f 
		 Inner Join Payments a on a.FileCode=f.FileCode 
		 Where f.FileCode = @code and TaxType = 11

		 Select @BankCode= ItemValue from SysSettings where  ItemName='DOM_BANK_CODE'

		 ---- Create response
		Select  @RespStat as RespStatus, 
				@RespMsg as RespMessage, 
				@CustomerName as CustomerName,
				@PayerName as ReceivedFrom,
				@tin as tin,
				@period as Period,
				@filecode as Filecode,
				@taxcat as IncomeTaxCatergory,
				@amount as TaxAmount,
				@declno as DeclNo,
				@commune as CommuneName,
				@adjust as adjustment,
				@delay as Delay,
				@TransactionType as DomesticTaxName,
				@imma as Imma,
				@chasis as Chasis,
				@carowner as CarOnwer,
				@edu as Education,
				@copies as Copies,
				@contra as Contravation,
				@service as Service,
				@Product as Product,
				@BankCode as BankCode
		return;
	End
END
Go

Alter Table Payments Add Extra5 varchar(200),  Extra6 varchar(200)
Go

--=============================================
-- Author: Alex Mugo
-- Create date: 04/06/2018
-- Description: A stored procedure to update tax payment status
-- =============================================
ALTER PROCEDURE [dbo].[sp_UpdateDomesticTaxPaymentStatus]
	@PayType int,---0 cbs 1 OBR
	@PaymentCode int,
	@Stat int,
	@Msg varchar(150),
	@ObrCode varchar(150)
AS
BEGIN
	SET NOCOUNT ON;
	Declare @Code int,
			@RespStat int = 0,
			@RespMsg varchar(150) = '',
			@FileCode int,
			@Attempts int,
			@PayStat int

	BEGIN TRY
		---- Validate
		Select @FileCode = FileCode, @Attempts = PostAttempts, @PayStat = StatusCode From Payments Where PaymentCode = @PaymentCode
		If(@FileCode Is Null)
		Begin
			Select 1 as RespStatus, 'Invalid tax payment!' as RespMessage
			Return
		End

		If(@PayType = 0)
		Begin 
			If(@Stat = 0)
			Begin
				Update Payments Set StatusCode = 1,Extra3 = @Msg Where PaymentCode = @PaymentCode
			End
			else if(@Stat = 4)--this means posting to core banking has failed i.e false repsonse was returned 
			Begin
				Update Payments Set StatusCode = 4, Extra3 = @Msg Where PaymentCode = @PaymentCode
			
				-- status 10 means posting to core banking has failed
				Update DomesticTaxFiles Set StatusCode = 10 Where FileCode = @FileCode
			End
		End
		Else If(@PayType = 1)
		Begin 
			If(@Stat = 0)
			Begin
				Update Payments Set Extra5 = @ObrCode Where PaymentCode = @PaymentCode

				Update DomesticTaxFiles Set StatusCode = 5 Where FileCode = @FileCode
			End
			else if(@Stat = 4)--this means posting to core banking has failed i.e false repsonse was returned 
			Begin
				--status 11 means posting to obr has failed
				Update DomesticTaxFiles Set StatusCode = 11 Where FileCode = @FileCode
			End
		End
		
		---- Create response
		Select	@RespStat as RespStatus, 
				@RespMsg as RespMessage
	END TRY
	BEGIN CATCH
		SELECT @RespMsg = ERROR_MESSAGE(), @RespStat = 2;
		Select @RespStat as RespStatus, @RespMsg as RespMessage
	END CATCH
END
Go


ALTER VIEW [dbo].[vw_DomesticTaxReceipts]
AS
SELECT      Coalesce(Coalesce(f.DriverName,f.CustomerName),a.Extra4) as CustomerName,
			a.Extra3 as ReferenceNo,
			Concat((Select ItemValue From SysSettings Where ItemName = 'DOM_BANK_CODE'),a.FileCode) as ReceiptNo,
			a.PostDate as ReceiptDate,
			a.PostDate,
			f.Tin as Nif,
			f.DeclNo as DeclarantNo,
			f.TaxPeriod as Period,
			a.Cr_Account as AccountCredit,
			a.Dr_Account as AccDebit,
			f.CustomerName as DeclarantDetails,
			a.FileCode,
			c.UserName,
			e.BranchName,
			h.CatergoryName as TaxDetails,
			h.CatergoryName as TaxName,
			a.Extra4 as ReceivedFrom,
			f.DeclNo as DeclarationNo,
			a.Amount,
			d.ModeName,
			a.StatusCode,
			a.Remarks,
			'Paiement Electronique par ' + d.ModeName AS PaymentMode,
			a.UserCode,
			a.PaymentCode,
			a.Extra5 as OBRRefNo,
			a.Extra5 as ObrNo 
FROM  dbo.Payments AS a 
INNER JOIN dbo.Users AS c ON c.UserCode = a.UserCode 
INNER JOIN dbo.PaymentModes AS d ON d.ModeCode = a.ModeCode 
INNER JOIN dbo.Branches AS e ON e.BranchCode = c.BranchCode 
INNER JOIN dbo.DomesticTaxFiles AS f ON F.FileCode = a.FileCode 
INNER JOIN dbo.DomesticTaxCatergory AS h ON f.TransactionType = h.CatergoryCode
where a.StatusCode=1 and a.TaxType = 11
GO


--========================= 26/10/2022 =========================================


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
			Update Payments Set StatusCode = 3, Extra3 = @Msg Where PaymentCode = @PaymentCode

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

Alter Table MiarieTaxFiles Add 
	PayAttempts int not null default 0,
	PostAttempts int not null default 0,
	Extra1 varchar(250),
	Extra2 varchar(250),
	Extra3 varchar(250),
	Extra4 varchar(250)
Go

-- =============================================
-- Author:		Alex Mugo
-- Create date: 15/09/2022
-- Description:	A stored procedure to update Miarie tax payment
-- =============================================
ALTER PROCEDURE [dbo].[sp_UpdateMiarieFileStatus]
	@FileCode int,
	@Stat int,
	@Msg varchar(250),
	@Extra1 varchar(250),
	@Extra2 varchar(250),
	@Extra3 varchar(250)
AS
BEGIN
	SET NOCOUNT ON;
	Declare @Code int,
			@RespStat int = 0,
			@RespMsg varchar(250) = '',
			@Attempts int,
			@FileStat int,
			@MaxAttempts int = 5

    BEGIN TRY
		---- Validate
		Select	@Attempts = PostAttempts, 
				@FileStat = StatusCode
		From MiarieTaxFiles Where FileCode = @FileCode
		If(@FileStat Is Null)
		Begin
			Select  1 as RespStatus, 'Invalid tax file!' as RespMessage
			Return
		End

		If(@FileStat = 5 or @FileStat = 9)
		Begin
			If(@Stat = 0)
			Begin
				--- Tax file processing complete
				Update MiarieTaxFiles Set 
					StatusCode = 6, 
					Extra1 = @Extra1,
					Extra2 = @Extra2,
					Extra3 = @Extra3 
				Where FileCode = @FileCode
			End
			Else
			Begin
				--- Tax file notification failed, make 5 attempts and marks it as failed
				Set @FileStat = 9
				Set @Attempts = @Attempts + 1
				If(@Attempts > @MaxAttempts)
					Set @FileStat = 7

				--- Update file notification failed
				Update MiarieTaxFiles Set StatusCode = @FileStat, PostAttempts = @Attempts, Extra1 = @Msg Where FileCode = @FileCode
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
			@PayPartial bit = 0

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

		---- Get charge

		---- Create response
		Select  @RespStat as RespStatus, 
				@RespMsg as RespMessage, 
				@Code as Data1, 
				@Charge as Data2,
				@PayPartial as Data3
	END TRY
	BEGIN CATCH
		SELECT @RespMsg = ERROR_MESSAGE(), @RespStat = 2;
		Select  @RespStat as RespStatus, @RespMsg as RespMessage
	END CATCH
END
Go

-- =============================================
-- Author:		Modified by Alex Mugo
-- Create date: 26/10/2022
-- Description:	A stored procedure to get domestic tax payments
-- =============================================
ALTER PROCEDURE [dbo].[sp_GetDomesticPayments] 
	@usercode int =0,
	@assesNo varchar(20)='',
	@DateFrom varchar(20)='',
	@DateTo varchar(20)=''
AS
BEGIN
	SET NOCOUNT ON;
	Begin
		If(@assesNo!='' or @assesNo!=null)
		Begin
				select * from vw_DomesticTaxReceipts where UserCode=@usercode and StatusCode=1 and ReceiptNo=''+ @assesNo+ ''  order by ReceiptDate desc;
		End
		Else if(@DateFrom!='' and @DateTo!='')
		Begin
		        select * from vw_DomesticTaxReceipts where UserCode=@usercode and StatusCode=1 and cast(ReceiptDate as date) >= cast(@DateFrom as date) and cast(ReceiptDate as date) <= cast(@DateTo as date)  order by ReceiptDate desc;
		End
	    Else if(@assesNo!='' and @DateFrom!='' and @DateTo!='')
		Begin
		    select * from vw_DomesticTaxReceipts where UserCode=@usercode and StatusCode=1 and ReceiptNo=''+ @assesNo+ '' and cast(ReceiptDate as date) >= cast(@DateFrom as date) and cast(ReceiptDate as date) <= cast(@DateTo as date)  order by ReceiptDate desc;
		End
	    Else
			select * from vw_DomesticTaxReceipts where StatusCode=1  order by ReceiptDate desc;
	End
End	
Go

ALTER VIEW [dbo].[vw_DomesticTaxReceipts]
AS
SELECT      Coalesce(Coalesce(f.DriverName,f.CustomerName),a.Extra4) as CustomerName,
			a.Extra3 as ReferenceNo,
			Concat((Select ItemValue From SysSettings Where ItemName = 'DOM_BANK_CODE'),a.FileCode) as ReceiptNo,
			a.CreateDate as ReceiptDate,
			a.PostDate,
			f.Tin as Nif,
			f.DeclNo as DeclarantNo,
			f.TaxPeriod as Period,
			a.Cr_Account as AccountCredit,
			a.Dr_Account as AccDebit,
			f.CustomerName as DeclarantDetails,
			a.FileCode,
			c.UserName,
			e.BranchName,
			h.CatergoryName as TaxDetails,
			h.CatergoryName as TaxName,
			a.Extra4 as ReceivedFrom,
			f.DeclNo as DeclarationNo,
			a.Amount,
			d.ModeName,
			a.StatusCode,
			a.Remarks,
			'Paiement Electronique par ' + d.ModeName AS PaymentMode,
			a.UserCode,
			a.PaymentCode,
			a.Extra5 as OBRRefNo,
			a.Extra5 as ObrNo 
FROM  dbo.Payments AS a 
INNER JOIN dbo.Users AS c ON c.UserCode = a.UserCode 
INNER JOIN dbo.PaymentModes AS d ON d.ModeCode = a.ModeCode 
INNER JOIN dbo.Branches AS e ON e.BranchCode = c.BranchCode 
INNER JOIN dbo.DomesticTaxFiles AS f ON F.FileCode = a.FileCode 
INNER JOIN dbo.DomesticTaxCatergory AS h ON f.TransactionType = h.CatergoryCode
where a.StatusCode=3 and a.TaxType = 11
GO

-- =============================================
-- Author:		Alex Mugo
-- Create date: 20/09/2022
-- Description:	A stored procedure to get payments approval queue
-- =============================================
ALTER PROCEDURE [dbo].[sp_GetPaymentApprovalQueue]
	@TaxType int = 0
AS
BEGIN
	SET NOCOUNT ON;
	Declare @Data as Table(
		PaymentCode int,
		CreateDate datetime,
		Amount decimal(18,2),
		ModeCode int,
		ApplyCharge bit,
		UserCode int,
		CrAccount varchar(20),
		DrAccount varchar(20),
		TaxType int,
		FileCode int,
		CompanyName varchar(300),
		CompanyNameMini varchar(500),
		DclntName varchar(200),
		UserName varchar(50),
		PayModeName varchar(30),
		TypeName varchar(20),
		Details varchar(500),
		TaxPeriod varchar(50)
	)

    -- OBR Custom
	If(@TaxType = 0 or @TaxType = 10)
	Begin
		Insert Into @Data
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
	End

	-- OBR Domestic
	If(@TaxType = 11)
	Begin
		Insert Into @Data
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
				b.CustomerName as CompanyName,
				'' as CompanyNameMini,
				b.CustomerName as DclntName,
				c.UserName,
				d.ModeName as PayModeName,
				'OBR DOMESTIC' as TypeName,
				'' as Details,
				b. TaxPeriod
		From Payments a
		Inner Join DomesticTaxFiles b on a.FileCode = b.FileCode
		Inner Join Users c on c.UserCode = a.UserCode
		Inner Join PaymentModes d on d.ModeCode = a.ModeCode
		Where a.StatusCode = 0
	End

	-- Miarie Tax
	If(@TaxType = 0 or @TaxType = 12)
	Begin
		Insert Into @Data
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
	End

	Select * From @Data Order By CreateDate Desc
END
Go

--=============================================
-- Author: Alex Mugo
-- Create date: 04/06/2018
-- Description: A stored procedure to update tax payment status
-- =============================================
ALTER PROCEDURE [dbo].[sp_UpdateDomesticTaxPaymentStatus]
	@PayType int,---0 cbs 1 OBR
	@PaymentCode int,
	@Stat int,
	@Msg varchar(150),
	@ObrCode varchar(150)
AS
BEGIN
	SET NOCOUNT ON;
	Declare @Code int,
			@RespStat int = 0,
			@RespMsg varchar(150) = '',
			@FileCode int,
			@Attempts int,
			@PayStat int

	BEGIN TRY
		---- Validate
		Select	@FileCode = FileCode, 
				@Attempts = PostAttempts, 
				@PayStat = StatusCode 
		From Payments Where PaymentCode = @PaymentCode
		If(@FileCode Is Null)
		Begin
			Select 1 as RespStatus, 'Invalid tax payment!' as RespMessage
			Return
		End

		If(@PayType = 0)
		Begin 
			If(@Stat = 0)
			Begin
				Update Payments Set StatusCode = 3, Extra3 = @Msg,Extra5 = @ObrCode Where PaymentCode = @PaymentCode
			End
			else if(@Stat = 4)--this means posting to core banking has failed i.e false repsonse was returned 
			Begin
				Update Payments Set StatusCode = 4, Extra3 = @Msg Where PaymentCode = @PaymentCode
			
				-- status 10 means posting to core banking has failed
				Update DomesticTaxFiles Set StatusCode = 10 Where FileCode = @FileCode
			End
		End
		Else If(@PayType = 1)
		Begin 
			If(@Stat = 0)
			Begin
				Update Payments Set Extra5 = @ObrCode Where PaymentCode = @PaymentCode

				Update DomesticTaxFiles Set StatusCode = 5 Where FileCode = @FileCode
			End
			else if(@Stat = 4)--this means posting to core banking has failed i.e false repsonse was returned 
			Begin
				--status 11 means posting to obr has failed
				Update DomesticTaxFiles Set StatusCode = 11 Where FileCode = @FileCode
			End
		End
		
		---- Create response
		Select	@RespStat as RespStatus, 
				@RespMsg as RespMessage
	END TRY
	BEGIN CATCH
		SELECT @RespMsg = ERROR_MESSAGE(), @RespStat = 2;
		Select @RespStat as RespStatus, @RespMsg as RespMessage
	END CATCH
END
Go


