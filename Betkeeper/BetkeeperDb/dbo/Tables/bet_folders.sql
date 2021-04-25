CREATE TABLE [dbo].[bet_folders] (
    [folder_name] NVARCHAR (50) NOT NULL,
    [owner]       INT           NOT NULL,
    CONSTRAINT [PK_BET_FOLDER] PRIMARY KEY CLUSTERED ([folder_name] ASC, [owner] ASC)
);

