CREATE EXTERNAL TABLE [dbo].[SecQuesAnswer] (
    [ID] INT NOT NULL,
    [SecQuesID] INT NOT NULL,
    [Answer] NVARCHAR (MAX) NOT NULL,
    [UserID] UNIQUEIDENTIFIER NOT NULL
)
    WITH (
    DATA_SOURCE = [RefBaseAPIValUat]
    );

