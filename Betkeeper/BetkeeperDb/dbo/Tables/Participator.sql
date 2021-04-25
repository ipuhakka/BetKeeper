CREATE TABLE [dbo].[Participator] (
    [Competition]    INT NOT NULL,
    [Role]           INT NOT NULL,
    [UserId]         INT NOT NULL,
    [ParticipatorId] INT IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_Participator] PRIMARY KEY CLUSTERED ([ParticipatorId] ASC),
    CONSTRAINT [FK_Participator_Competition] FOREIGN KEY ([Competition]) REFERENCES [dbo].[Competition] ([CompetitionId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Participator_users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[users] ([user_id])
);

