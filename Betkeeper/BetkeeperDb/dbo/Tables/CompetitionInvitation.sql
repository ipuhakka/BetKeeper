CREATE TABLE [dbo].[CompetitionInvitation] (
    [InvitationId] INT IDENTITY (1, 1) NOT NULL,
    [Competition]  INT NOT NULL,
    [UserId]       INT NOT NULL,
    CONSTRAINT [PK_CompetitionInvitation] PRIMARY KEY CLUSTERED ([InvitationId] ASC)
);

