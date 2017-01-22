
CREATE PROCEDURE [dbo].[UpdateCurrentStatus]
	@LastUpdate DateTime,
	@Status varchar(100), 
    @NextMission DATETIME = NULL
AS
	UPDATE	dbo.CurrentStatus
	SET		LastUpdate = @LastUpdate,
			Status = @Status,
			NextMission = COALESCE( @NextMission, NextMission );
