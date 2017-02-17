CREATE PROCEDURE [dbo].[GetCurrentStatus]
AS
	SELECT	LastUpdate, Status, NextMission, BatteryPercent
	from	dbo.CurrentStatus;
