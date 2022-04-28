SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP TABLE IF EXISTS [dbo].[benchTable];
GO

CREATE TABLE [dbo].[benchTable](
	[time] [datetime] NOT NULL,
	[value] [float] NOT NULL,
	[deviceId] [varchar](50) NOT NULL,
) ON [PRIMARY]
GO;