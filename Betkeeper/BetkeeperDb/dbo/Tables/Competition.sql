CREATE TABLE [dbo].[Competition] (
    [JoinCode]      NVARCHAR (12)  NOT NULL,
    [StartTime]     DATETIME2 (7)  NULL,
    [Description]   NVARCHAR (MAX) NULL,
    [CompetitionId] INT            IDENTITY (1, 1) NOT NULL,
    [Name]          NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_Competition] PRIMARY KEY CLUSTERED ([CompetitionId] ASC)
);




GO
CREATE TRIGGER competitionTargetsTrigger
    ON dbo.Competition
    FOR DELETE
AS
    DELETE FROM dbo.Target
    WHERE Competition IN(SELECT deleted.CompetitionId FROM deleted)
