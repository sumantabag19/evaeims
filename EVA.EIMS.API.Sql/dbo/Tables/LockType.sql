CREATE TABLE [dbo].[LockType] (
    [LockTypeID]      INT            IDENTITY (1, 1) NOT NULL,
    [LockName]        NVARCHAR (50)  NOT NULL,
    [LockDescription] NVARCHAR (500) NULL,
    CONSTRAINT [PK_LockType] PRIMARY KEY CLUSTERED ([LockTypeID] ASC)
);

