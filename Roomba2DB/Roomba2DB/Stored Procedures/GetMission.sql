CREATE PROCEDURE [dbo].[GetMission]
	@MissionNumber INT
AS
	SELECT	MissionNumber, StartTime, LastUpdate, Cycle, Phase, Initiator, Error, BatteryPercent
	from	dbo.Mission
	where	MissionNumber = @MissionNumber;