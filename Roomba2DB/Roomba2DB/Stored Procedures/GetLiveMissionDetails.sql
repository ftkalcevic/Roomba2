--drop procedure [dbo].[GetLiveMissionDetails]


CREATE PROCEDURE [dbo].[GetLiveMissionDetails]
	@LastTick int
AS
	DECLARE @MissionNumber INT;
	DECLARE @Tick INT;
	declare @StartTime DATETIME;
	
	select	@MissionNumber=MissionNumber, @StartTime=StartTime
	from	Mission
	where	MissionNumber= (select	MAX(MissionNumber)
							from	Mission);

	if @MissionNumber is NULL
	BEGIN
			SELECT	@MissionNumber MissionNumber, @Tick LastTick, @StartTime StartTime;
	END
	ELSE
	BEGIN

		SELECT	@Tick = MAX(Tick)
		FROM	dbo.Position
		WHERE	MissionNumber = @MissionNumber;

		SELECT	@MissionNumber MissionNumber, @Tick LastTick, @StartTime StartTime;

		SELECT	x, y, theta, battery
		from	dbo.Position
		where	MissionNumber = @MissionNumber
			AND Tick > @LastTick 
			AND Tick <= @Tick
		order by Tick;

	END;

go
