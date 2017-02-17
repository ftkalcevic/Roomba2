CREATE PROCEDURE [dbo].[GetMissionDetails]
	@MissionNumber INT
AS
	SELECT	x, y, theta, battery
	from	dbo.Position
	where	MissionNumber = @MissionNumber
	order by Tick;