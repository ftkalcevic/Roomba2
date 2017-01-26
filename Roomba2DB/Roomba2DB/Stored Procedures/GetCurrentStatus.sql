CREATE PROCEDURE [dbo].[GetCurrentStatus]
AS
	SELECT	LastUpdate, Status, NextMission, RoombaTime, Flags, BatteryPercentage, Error, NotReady
	from	dbo.CurrentStatus;
