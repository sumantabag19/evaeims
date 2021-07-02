CREATE TABLE [dbo].[AuthProviderMaster] (
    [ProviderId]          INT              IDENTITY (1, 1) NOT NULL,
    [ProviderName]        NVARCHAR (200)   NULL,
    [ProviderDescription] NVARCHAR (250)   NULL,
    [Configuration]       NVARCHAR (500)   NULL,
    [UpdatedBy]           UNIQUEIDENTIFIER NULL,
    [UpdatedOn]           DATETIME         NULL,
    [IsActive]            BIT              NULL,
    CONSTRAINT [PK_ProviderMaster] PRIMARY KEY CLUSTERED ([ProviderId] ASC)
);

