CREATE TABLE [dbo].[OrganizationTenantMapping] (
    [OrganizationTenantId] INT              NOT NULL Identity,
    [OrgId]                INT              NOT NULL,
    [TenantId]             UNIQUEIDENTIFIER NOT NULL,
	[IsActive]				bit DEFAULT(1),
	[CreatedBy]				UNIQUEIDENTIFIER NULL,
    [CreatedOn]				datetime DEFAULT(GETDATE()),
    [ModifiedOn]			datetime DEFAULT(GETDATE()),
    [ModifiedBy]			uniqueidentifier NULL,
    CONSTRAINT [PK_OrganizationTenantMapping] PRIMARY KEY CLUSTERED ([OrganizationTenantId] ASC)
);

