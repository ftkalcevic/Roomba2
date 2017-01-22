CREATE TABLE [dbo].[Position] (
    [MissionId] INT NOT NULL,
    [Tick]      INT NOT NULL,
    [x]         INT NOT NULL,
    [y]         INT NOT NULL,
    [theta]     INT NOT NULL,
    [battery]   INT NOT NULL,
    CONSTRAINT [PK_Table] PRIMARY KEY CLUSTERED ([MissionId] ASC, [Tick] ASC),
    CONSTRAINT [FK_MissionId] FOREIGN KEY ([MissionId]) REFERENCES [dbo].[Mission] ([MissionId])
);

