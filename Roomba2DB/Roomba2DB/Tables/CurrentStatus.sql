CREATE TABLE [dbo].[CurrentStatus]
(
	[LastUpdate] DateTime NOT NULL ,
	[Status] varchar(100) NOT NULL, 
    [NextMission] DATETIME NULL, 
    [RoombaTime] DATETIME NULL
)
