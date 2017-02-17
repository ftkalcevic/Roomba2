CREATE TYPE [dbo].[PropertiesTableType] AS TABLE(
	[Name] [varchar](30) NOT NULL,
	[Value] [varchar](500) NOT NULL,
	PRIMARY KEY CLUSTERED 
(
	[Name] ASC
)WITH (IGNORE_DUP_KEY = OFF)
)
