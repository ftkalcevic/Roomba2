CREATE PROCEDURE [dbo].[GetMission]
	@MissionId INT
AS
	SELECT	MissionId, StartTime, EndTime
	from	dbo.Mission
	where	MissionId = @MissionId;