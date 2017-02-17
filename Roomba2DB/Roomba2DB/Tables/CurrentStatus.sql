CREATE TABLE [dbo].[CurrentStatus]
(
	[LastUpdate] DateTime NOT NULL ,
	[Status] varchar(30) NOT NULL, 
    [NextMission] INT NULL, 
    [BatteryPercent] INT NOT NULL DEFAULT 0 
)
