CREATE PROCEDURE [dbo].[GetMissionDetails]
	@MissionId INT
AS
	SELECT	x, y, theta, battery
	from	dbo.Position
	where	MissionId = @MissionId
	order by Tick;