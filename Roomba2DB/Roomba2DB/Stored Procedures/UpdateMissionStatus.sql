CREATE PROCEDURE [dbo].[UpdateMissionStatus]
@LastUpdate datetime,
@MissionNumber int,
@Cycle varchar(50),
@Phase varchar(50),
@Initiator  varchar(50),
@Error int,
@BatteryPercent int
AS
	if exists ( select 1 from dbo.Mission where MissionNumber = @MissionNumber )
		Update	dbo.Mission
		Set		LastUpdate = @LastUpdate,
				Cycle = @Cycle,
				Phase = @Phase,
				Error = @Error ,
				BatteryPercent = @BatteryPercent
		where	MissionNumber = @MissionNumber;
	ELSE
		insert into dbo.Mission
				( MissionNumber,  StartTime,   LastUpdate,  Cycle,  Phase,  Initiator,  Error,  BatteryPercent )
		values	( @MissionNumber, @LastUpdate, @LastUpdate, @Cycle, @Phase, @Initiator, @Error, @BatteryPercent );

	 
RETURN 0
