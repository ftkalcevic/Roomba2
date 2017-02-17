
CREATE PROCEDURE [dbo].[NewPosition]
	@MissionNumber INT,
	@Tick INT,
	@x INT,
	@y INT,
	@theta INT,
	@battery INT
AS
	INSERT INTO dbo.Position
		(MissionNumber,Tick,x,y,theta,battery)
	values
		(@MissionNumber,@Tick,@x,@y,@theta,@battery)
