CREATE TABLE [dbo].[Log](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Application] [nvarchar](50) NOT NULL,
	[Logged] [datetime] NOT NULL,
	[Level] [nvarchar](50) NOT NULL,
	[Message] [nvarchar](max) NOT NULL,
	[Exception] [nvarchar](max) NULL,
	[ClientId] [nvarchar](200) NULL,
	[UserId] [nvarchar](100) NULL,
	[StatusCode] [int] NULL,

 CONSTRAINT [PK_dbo.Log] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

