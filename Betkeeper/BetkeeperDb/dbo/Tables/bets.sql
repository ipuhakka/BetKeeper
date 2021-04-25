CREATE TABLE [dbo].[bets] (
    [bet_won]   INT            NOT NULL,
    [name]      NVARCHAR (MAX) NULL,
    [odd]       FLOAT (53)     NOT NULL,
    [bet]       FLOAT (53)     NOT NULL,
    [date_time] DATETIME2 (7)  NULL,
    [owner]     INT            NULL,
    [bet_id]    INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__bets__551113D885D361C8] PRIMARY KEY CLUSTERED ([bet_id] ASC),
    CONSTRAINT [FK__bets__owner__656C112C] FOREIGN KEY ([owner]) REFERENCES [dbo].[users] ([user_id]) ON DELETE CASCADE
);

