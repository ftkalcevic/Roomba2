CREATE TABLE [dbo].[Mission] (
    [MissionId] INT      IDENTITY (1, 1) NOT NULL,
    [StartTime] DATETIME NOT NULL,
    [EndTime]   DATETIME NULL,
    PRIMARY KEY CLUSTERED ([MissionId] ASC)
);

