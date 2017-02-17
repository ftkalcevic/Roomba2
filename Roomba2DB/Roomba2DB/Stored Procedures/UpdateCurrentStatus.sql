
CREATE PROCEDURE [dbo].[UpdateCurrentStatus]
	@LastUpdate DateTime,
	@Status varchar(30), 
	@NextMission INT,
	@BatteryPercent INT
AS
	UPDATE	dbo.CurrentStatus
	SET		LastUpdate = @LastUpdate,
			Status = @Status,
			NextMission = @NextMission,
			BatteryPercent =  @BatteryPercent;
