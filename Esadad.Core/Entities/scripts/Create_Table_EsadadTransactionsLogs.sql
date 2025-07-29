CREATE TABLE [dbo].[EsadadTransactionsLogs](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TransactionType] [varchar](20) NOT NULL,
	[ApiName] [varchar](50) NOT NULL,
	[Guid] [varchar](50) NOT NULL,
	[Timestamp] [datetime] NOT NULL,
	[BillingNumber] [nvarchar](50)  NULL,
	[BillNumber] [nvarchar](50)  NULL,
	[Currency] [varchar](10)  NULL,
	[ServiceType] [nvarchar](50)  NULL,
	[PrepaidCat] [nvarchar](50) NULL,
	[ValidationCode] [nvarchar](50) NULL,
	[TranXmlElement] [nvarchar](max) NOT NULL,
	[InsertDate] [datetime] NOT NULL,
 CONSTRAINT [PK_TransactionsLogs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[EsadadTransactionsLogs] ADD  CONSTRAINT [DF_TransactionsLogs_InsertDate]  DEFAULT (sysdatetime()) FOR [InsertDate]
GO