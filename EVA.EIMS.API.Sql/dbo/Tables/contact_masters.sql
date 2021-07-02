CREATE TABLE [dbo].[contact_masters](
        [contact_id] [bigint] IDENTITY(1,1) NOT NULL,
        [org_id] [bigint] NOT NULL,
        [created_by] [bigint] NOT NULL,
        [created_date_time] [datetime] NOT NULL,
        [contact_code] [varchar](20) NOT NULL,
        [title] [nchar](10) NOT NULL,
        [first_name] [varchar](50) NOT NULL,
        [last_name] [varchar](50) NOT NULL,
        [primary_contact] [varchar](20) NOT NULL,
        [primary_email] [varchar](70) NOT NULL,
        [location_id] [bigint] NOT NULL,
        [location_others] [varchar](70) NOT NULL,
 CONSTRAINT [PK_contact_masters] PRIMARY KEY CLUSTERED 
(
        [contact_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
