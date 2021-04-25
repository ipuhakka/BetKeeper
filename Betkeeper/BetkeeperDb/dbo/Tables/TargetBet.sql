CREATE TABLE [dbo].[TargetBet] (
    [Target]       INT            NOT NULL,
    [Participator] INT            NOT NULL,
    [Bet]          NVARCHAR (MAX) NOT NULL,
    [TargetBetId]  INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_TargetBet] PRIMARY KEY CLUSTERED ([TargetBetId] ASC),
    CONSTRAINT [FK_TargetBet_Participator] FOREIGN KEY ([Participator]) REFERENCES [dbo].[Participator] ([ParticipatorId]) ON DELETE CASCADE,
    CONSTRAINT [FK_TargetBet_Target] FOREIGN KEY ([Target]) REFERENCES [dbo].[Target] ([TargetId]) ON DELETE CASCADE
);

