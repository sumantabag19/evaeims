CREATE TYPE [dbo].[UDTOrganization] AS TABLE (
    [OrgId]       INT              NOT NULL,
    [OrgName]     NVARCHAR (150)   NOT NULL,
    [Description] NVARCHAR (200)   NULL,
    [IsActive]    BIT              NOT NULL,
    [CreatedBy]   UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]   DATETIME         NOT NULL,
    [ModifiedBy]  UNIQUEIDENTIFIER NULL,
    [ModifiedOn]  DATETIME         NULL);

