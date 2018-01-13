USE [PlivoCodingAssignment]
GO

/****** Object:  Table [dbo].[account]    Script Date: 10-01-2018 04:08:39 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[account](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[auth_id] [nvarchar](40) NULL,
	[username] [nvarchar](30) NULL,
 CONSTRAINT [account_pkey] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


