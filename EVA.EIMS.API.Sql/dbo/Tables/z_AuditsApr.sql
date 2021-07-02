﻿CREATE TABLE [dbo].[z_AuditsApr] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [TableName] NVARCHAR (100) NULL,
    [DateTime]  DATETIME       NULL,
    [KeyValues] NVARCHAR (500) NULL,
    [OldValues] NVARCHAR (MAX) NULL,
    [NewValues] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Id_z_AuditsApr] PRIMARY KEY CLUSTERED ([Id] ASC)
);

