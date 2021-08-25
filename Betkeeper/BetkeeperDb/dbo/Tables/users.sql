CREATE TABLE [dbo].[users] (
    [user_id]  INT            IDENTITY (1, 1) NOT NULL,
    [username] NVARCHAR (MAX) NOT NULL,
    [password] NVARCHAR (MAX) NOT NULL,
    [salt] NVARCHAR(MAX) NULL, 
    PRIMARY KEY CLUSTERED ([user_id] ASC)
);


GO
CREATE TRIGGER users_trigger
 ON users
    FOR DELETE
AS
    DELETE FROM bet_folders
    WHERE owner IN (SELECT deleted.user_id FROM deleted);