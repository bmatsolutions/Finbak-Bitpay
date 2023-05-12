
Drop Procedure sp_MakeMiariePayment
Go
Drop Procedure sp_ValidateMiariePayment
Go
Drop Procedure sp_validateMiarieDecl
Go
Drop Procedure sp_UpdateMiariePaymentStatus
Go

Drop Table MiarieTaxCharges
Go
Drop Table MiariePayments
Go

Create view vw_MiarieTaxPayments
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
		a.StatusCode,
		b.Remarks,		
		b.PaymentCode,
		b.Cr_Account,
		b.Dr_Account,
		b.ReceiptNo,
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
Go


--- 22/09/2022

Alter Table Reports Add Show bit not null default 1
Go


Create Table ReportFilters(
	Id int identity not null,
	ReportCode int not null,
	FilterName varchar(100) not null,
	ValName varchar(50) not null,
	ValDefault varchar(50) not null,
	FilterType int not null,
	ListType int not null,
	Constraint pk_ReportFilters primary key(Id),
	Constraint uk_ReportFilters_ReportCode_ValName unique(ReportCode, ValName),
	Constraint fk_ReportFilters_ReportCode foreign key(ReportCode) references Reports(ReportCode)
)
Go

Create Table ReportDataSets(
	Id int identity not null,
	ReportCode int not null,
	SetNo int not null,
	SourceName varchar(100) not null,
	BindName varchar(100) not null,
	Constraint pk_ReportDataSets primary key(Id),
	Constraint uk_ReportDataSets_ReportCode_SetNo unique(ReportCode, SetNo),
	Constraint fk_ReportDataSets_ReportCode foreign key(ReportCode) references Reports(ReportCode)
)
Go

Update Reports Set Show = 0
Go

Insert Into Reports(ReportCode, ReportTitle, ReportName) Values(50,'ALL MIARIE TAXES','miarietax')
Insert Into Reports(ReportCode, ReportTitle, ReportName) Values(51,'MIARIE TAX PAYMENTS','miariepayment')
Insert Into Reports(ReportCode, ReportTitle, ReportName,Show) Values(52,'MIARIE TAX RECEIPT','miariereceipt',0)
Go

Insert Into ReportFilters(ReportCode, FilterName, ValName, ValDefault, FilterType, ListType) Values(50, 'Date Range','DateFrom','',1,0)
Insert Into ReportFilters(ReportCode, FilterName, ValName, ValDefault, FilterType, ListType) Values(51, 'Date Range','DateFrom','',1,0)
Go

Insert Into ReportDataSets(ReportCode, SetNo, SourceName, BindName) Values(50, 1,'vw_MiarieTaxPayments','Data1')
Insert Into ReportDataSets(ReportCode, SetNo, SourceName, BindName) Values(51, 1,'vw_MiarieTaxPayments','Data1')
Insert Into ReportDataSets(ReportCode, SetNo, SourceName, BindName) Values(52, 1,'vw_MiarieTaxPayments','Data1')
Go

Insert into SysSettings(ItemName, ItemValue, Descr) Values('REPORTS_DIR','','Reports directory')
Go


-- =============================================
-- Author:		Alex Mugo
-- Create date: 23/09/2022
-- Description:	A stored procedure to get report settings
-- =============================================
CREATE PROCEDURE sp_GetReportSetting
	@ReportCode int
AS
BEGIN
	SET NOCOUNT ON;
	Declare @RespStat int = 0,
			@RespMsg varchar(150) = '',
			@Title varchar(100),
			@SubTitle varchar(150),
			@FileName varchar(100),
			@ReportsDir varchar(150),
			@DataSets varchar(max) =''

	Select	@Title = ReportTitle, 
			@FileName = ReportName			  
	From Reports Where ReportCode = @ReportCode

	If(@Title Is Null)
	Begin
		Select  1 as RespStatus, 'Invalid report details!' as RespMessage
		Return
	End

	---- Get data sets
	Select @DataSets = COALESCE(@DataSets + '|', '') + Cast(SetNo as Varchar(5)) + ',' + SourceName + ',' + BindName 
	From ReportDataSets Where ReportCode = @ReportCode

	Select @ReportsDir = ItemValue From SysSettings Where ItemName = 'REPORTS_DIR'

    --- Create response
	Select	@RespStat as RespStatus, 
			@RespMsg as RespMessage,
			@Title as Title,
			@SubTitle as SubTitle,
			@FileName as FileName,
			@ReportsDir as ReportsDir,
			@DataSets as DataSets
END
Go

-- =============================================
-- Author:		Alex Mugo
-- Create date: 23/09/2022
-- Description:	A stored to process report data
-- =============================================
CREATE PROCEDURE [dbo].[sp_GetReportParams]
	@ReportCode int,
	@DateFrom varchar(50) = '01 Jan 1900',
    @DateTo  varchar(50) = '01 Jan 1900',
    @Filter1 varchar(150) = '',
    @Filter2 varchar(150) = '',
    @Filter3 varchar(150) = '',
    @Filter4 varchar(150) = '',
	@Filter5 varchar(150) = '',
    @Filter6 varchar(150) = ''
AS
BEGIN
	SET NOCOUNT ON;
	Declare @ParamsTable Table(
		ParamName varchar(30),
		ParamValue varchar(500)
	)

 --   If(@ReportCode In(11, 12, 13, 14, 15, 16, 17))
	--Begin
	--	--- Institution full statement
	--	Insert Into @ParamsTable(ParamName, ParamValue)
	--	Select 'rptTitle', ReportName From Reports Where ReportCode = @ReportCode
	--End

	Select * From @ParamsTable
END
GO

-- =============================================
-- Author:		Alex Mugo
-- Create date: 23/09/2022
-- Description:	A stored to process report data
-- =============================================
CREATE PROCEDURE sp_GetReportSetData
	@ReportCode int,
	@DateFrom varchar(50) = '01 Jan 1900',
    @DateTo varchar(50) = '01 Jan 1900',
	@DataSet int = 1,
    @Filter1 varchar(150) = '',
    @Filter2 varchar(150) = '',
    @Filter3 varchar(150) = '',
    @Filter4 varchar(150) = '',
	@Filter5 varchar(150) = '',
    @Filter6 varchar(150) = ''
AS
BEGIN
	SET NOCOUNT ON;

    If(@ReportCode = 50)
	Begin
		--- Institution full statement
		Select * From vw_MiarieTaxPayments Where Cast(CreateDate as Date) >= @DateFrom and Cast(CreateDate as Date) <= @DateTo  
		ORDER BY CreateDate Desc 
	End
	Else If(@ReportCode = 51)
	Begin
		Select * From vw_MiarieTaxPayments Where Cast(CreateDate as Date) >= @DateFrom and Cast(CreateDate as Date) <= @DateTo  
	End
	Else If(@ReportCode = 52)
	Begin
		Select * From vw_MiarieTaxPayments Where FileCode = @Filter1  
	End	
END
Go

