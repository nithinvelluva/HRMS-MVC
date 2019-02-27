/****** Object:  Table [dbo].[PasswordResetToken]    Script Date: 2018-03-07 20:53:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PasswordResetToken](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[empId] [int] NULL,
	[token] [nvarchar](max) NULL,
	[created] [nvarchar](50) NULL,
 CONSTRAINT [PK_PasswordResetToken] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserAccessToken]    Script Date: 2018-03-07 20:53:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserAccessToken](
	[Id] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[AccessToken] [nvarchar](max) NOT NULL,
	[CreatedDateTime] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_UserAccessToken] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO


ALTER TABLE [dbo].[PasswordResetToken]  WITH CHECK ADD FOREIGN KEY([empId])
REFERENCES [dbo].[EmployeeInfo] ([EmpId])

ALTER TABLE [dbo].[UserAccessToken]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[EmployeeInfo] ([EmpId])