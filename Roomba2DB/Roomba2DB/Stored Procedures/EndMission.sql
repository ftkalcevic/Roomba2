
CREATE PROCEDURE [dbo].[EndMission]
	@MissionId INT

AS
	UPDATE	dbo.Mission
	SET		EndTime = getdate()
	WHERE	MissionId = @MissionId

RETURN 0
