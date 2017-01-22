CREATE PROCEDURE [dbo].[GetAllMissions]
AS
	SELECT	MissionId, StartTime, EndTime
	from	dbo.Mission;