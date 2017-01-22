CREATE PROCEDURE [dbo].[GetCurrentStatus]
AS
	SELECT	LastUpdate, Status, NextMission, RoombaTime
	from	dbo.CurrentStatus;
