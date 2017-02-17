CREATE TABLE [dbo].[Mission] (
    [MissionNumber] INT NOT NULL,
    [LastUpdate] DATETIME NOT NULL,
    [StartTime]   DATETIME NOT NULL,
    [Cycle] VARCHAR(50) NOT NULL, 
    [Phase] VARCHAR(50) NOT NULL, 
    [Initiator] VARCHAR(50) NOT NULL, 
    [Error] INT NOT NULL, 
    [BatteryPercent] INT NOT NULL, 
    PRIMARY KEY CLUSTERED ([MissionNumber] ASC)
);

