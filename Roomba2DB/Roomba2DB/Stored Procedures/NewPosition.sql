
CREATE PROCEDURE [dbo].[NewPosition]
	@MissionId INT,
	@Tick INT,
	@x INT,
	@y INT,
	@theta INT,
	@battery INT
AS
	INSERT INTO dbo.Position
		(MissionId,Tick,x,y,theta,battery)
	values
		(@MissionId,@Tick,@x,@y,@theta,@battery)
