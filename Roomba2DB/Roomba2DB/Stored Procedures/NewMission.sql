
CREATE PROCEDURE [dbo].[NewMission]
AS
	INSERT INTO dbo.Mission
	( StartTime )
	VALUES
	( GETDATE() );

	SELECT CAST(@@IDENTITY as INTEGER) NewMissionId;