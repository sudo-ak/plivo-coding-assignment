USE [PlivoCodingAssignment]
GO

/****** Object:  Table [dbo].[phone_number]    Script Date: 10-01-2018 04:09:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[phone_number](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[number] [nvarchar](40) NULL,
	[account_id] [bigint] NULL,
 CONSTRAINT [phone_number_pkey] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[phone_number]  WITH CHECK ADD  CONSTRAINT [FK_phone_number_account] FOREIGN KEY([account_id])
REFERENCES [dbo].[account] ([id])
GO

ALTER TABLE [dbo].[phone_number] CHECK CONSTRAINT [FK_phone_number_account]
GO


