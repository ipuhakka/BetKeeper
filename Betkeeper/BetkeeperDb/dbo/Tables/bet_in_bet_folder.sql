CREATE TABLE [dbo].[bet_in_bet_folder] (
    [folder] NVARCHAR (50) NOT NULL,
    [owner]  INT           NULL,
    [bet_id] INT           NOT NULL,
    CONSTRAINT [PK_BET_IN_FOLDER] PRIMARY KEY CLUSTERED ([folder] ASC, [bet_id] ASC),
    FOREIGN KEY ([folder], [owner]) REFERENCES [dbo].[bet_folders] ([folder_name], [owner]) ON DELETE CASCADE,
    CONSTRAINT [FK__bet_in_be__bet_i__1CBC4616] FOREIGN KEY ([bet_id]) REFERENCES [dbo].[bets] ([bet_id]) ON DELETE CASCADE
);

