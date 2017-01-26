CREATE TABLE [dbo].[CurrentStatus]
(
	[LastUpdate] DateTime NOT NULL ,
	[Status] varchar(100) NOT NULL, 
    [NextMission] INT NULL, 
    [RoombaTime] DATETIME NULL, 
    [NotReady] INT NOT NULL DEFAULT 0, 
    [Flags] INT NOT NULL DEFAULT 0, 
    [BatteryPercentage] INT NOT NULL DEFAULT 0, 
    [Error] INT NOT NULL DEFAULT 0
)
