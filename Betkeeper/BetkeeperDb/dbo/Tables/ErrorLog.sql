CREATE TABLE [dbo].[ErrorLog] (
    [Application] NVARCHAR (50)  NULL,
    [StackTrace]  NVARCHAR (MAX) NULL,
    [Message]     NVARCHAR (MAX) NULL,
    [Url]         NVARCHAR (MAX) NULL,
    [ErrorLogId]  INT            IDENTITY (1, 1) NOT NULL,
    [Time]        DATETIME2 (7)  NOT NULL,
    CONSTRAINT [PK_ErrorLog] PRIMARY KEY CLUSTERED ([ErrorLogId] ASC)
);

