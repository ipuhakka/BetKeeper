CREATE TABLE [dbo].[Session] (
    [UserId]         INT            NOT NULL,
    [Token]          NVARCHAR (MAX) NOT NULL,
    [ExpirationTime] DATETIME2 (7)  NOT NULL,
    CONSTRAINT [PK_Session] PRIMARY KEY CLUSTERED ([UserId] ASC)
);

