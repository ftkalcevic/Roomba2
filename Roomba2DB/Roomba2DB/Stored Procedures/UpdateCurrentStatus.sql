
CREATE PROCEDURE [dbo].[UpdateCurrentStatus]
	@LastUpdate DateTime,
	@Status varchar(100), 
	@Flags INT,
	@BatteryPercentage INT,
	@Error INT,
	@NotReady int,
    @NextMission int = NULL,
	@RoombaTime DATETIME = NULL
AS
	UPDATE	dbo.CurrentStatus
	SET		LastUpdate = @LastUpdate,
			Status = @Status,
			NextMission = COALESCE( @NextMission, NextMission ),
			RoombaTime =  COALESCE( @RoombaTime, RoombaTime );
