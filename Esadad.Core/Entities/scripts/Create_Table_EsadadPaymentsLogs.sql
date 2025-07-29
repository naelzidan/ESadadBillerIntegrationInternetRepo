CREATE TABLE [dbo].[EsadadPaymentsLogs](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Guid] [varchar](50) NOT NULL,
	[BillingNumber] [nvarchar](50) NOT NULL,
	[BillNumber] [nvarchar](50) NOT NULL,
	[PaidAmount] [decimal](12, 3) NOT NULL,
	[JOEBPPSTrx] [nvarchar](50) NOT NULL,
	[BankTrxID] [nvarchar](50) NOT NULL,
	[BankCode] [int] NOT NULL,
	[DueAmt] [decimal](12, 3) NOT NULL,
	[PaidAmt] [decimal](12, 3) NOT NULL,
	[FeesAmt] [decimal](12, 3) NULL,
	[FeesOnBiller] [bit] NOT NULL,
	[ProcessDate] [datetime] NOT NULL,
	[STMTDate] [datetime] NOT NULL,
	[AccessChannel] [nvarchar](50) NOT NULL,
	[PaymentMethod] [nvarchar](50) NOT NULL,
	[PaymentType] [nvarchar](50) NOT NULL,
	[Currency] [varchar](10) NOT NULL,
	[ServiceType] [nvarchar](50) NOT NULL,
	[PrepaidCat] [nvarchar](50) NULL,
	[Amount] [decimal](12, 3) NOT NULL,
	[SetBnkCode] [int] NOT NULL,
	[AcctNo] [nvarchar](50) NOT NULL,
	[IsPaymentPosted] [bit] NOT NULL,
 CONSTRAINT [PK_EsadadPaymetsLogs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[EsadadPaymentsLogs] ADD  CONSTRAINT [DF_EsadadPaymetsLogs_FeesOnBiller]  DEFAULT ((0)) FOR [FeesOnBiller]
GO

ALTER TABLE [dbo].[EsadadPaymentsLogs] ADD  CONSTRAINT [DF_EsadadPaymetsLogs_IsPaymentPosted]  DEFAULT ((0)) FOR [IsPaymentPosted]
GO
