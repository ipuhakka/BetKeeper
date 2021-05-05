CREATE TABLE [dbo].[Target] (
    [Competition]           INT            NOT NULL,
    [Bet]                   NVARCHAR (MAX) NOT NULL,
    [Type]                  INT            NOT NULL,
    [Result]                NVARCHAR (MAX) NULL,
    [TargetId]              INT            IDENTITY (1, 1) NOT NULL,
    [Selections]            NVARCHAR (MAX) NULL,
    [Scoring]               NVARCHAR (MAX) NOT NULL,
    [Grouping]              NVARCHAR (MAX) NULL,
    [AllowedSelectionCount] INT            NULL,
    CONSTRAINT [PK_Target] PRIMARY KEY CLUSTERED ([TargetId] ASC)
);



