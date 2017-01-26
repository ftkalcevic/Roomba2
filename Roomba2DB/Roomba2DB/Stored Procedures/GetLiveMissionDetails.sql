--drop procedure [dbo].[GetLiveMissionDetails]


CREATE PROCEDURE [dbo].[GetLiveMissionDetails]
	@LastTick int
AS
	DECLARE @MissionId INT;
	DECLARE @Tick INT;
	declare @StartTime DATETIME;
	
	select	@MissionId=MissionId, @StartTime=StartTime
	from	Mission
	where	MissionId= (select	MAX(MissionId)
						from	Mission
						where	EndTime is NULL);

	if @MissionId is NULL
	BEGIN
			SELECT	@MissionId MissionId, @Tick LastTick, @StartTime StartTime;
	END
	ELSE
	BEGIN

		SELECT	@Tick = MAX(Tick)
		FROM	dbo.Position
		WHERE	MissionId = @MissionId;

		SELECT	@MissionId MissionId, @Tick LastTick, @StartTime StartTime;

		SELECT	x, y, theta, battery
		from	dbo.Position
		where	MissionId = @MissionId
			AND Tick > @LastTick 
			AND Tick <= @Tick
		order by Tick;

	END;

go
