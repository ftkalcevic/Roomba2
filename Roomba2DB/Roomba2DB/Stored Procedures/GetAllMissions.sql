CREATE PROCEDURE [dbo].[GetAllMissions]
AS
	SELECT	MissionNumber, StartTime, LastUpdate, Cycle, Phase, Initiator, Error, BatteryPercent
	from	dbo.Mission
	order	by StartTime desc;