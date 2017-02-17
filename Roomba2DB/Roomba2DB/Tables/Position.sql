CREATE TABLE [dbo].[Position] (
    [MissionNumber] INT NOT NULL,
    [Tick]      INT NOT NULL,
    [x]         INT NOT NULL,
    [y]         INT NOT NULL,
    [theta]     INT NOT NULL,
    [battery]   INT NOT NULL,
    CONSTRAINT [PK_Table] PRIMARY KEY CLUSTERED ([MissionNumber] ASC, [Tick] ASC),
    CONSTRAINT [FK_MissionId] FOREIGN KEY ([MissionNumber]) REFERENCES [dbo].[Mission] ([MissionNumber])
);

