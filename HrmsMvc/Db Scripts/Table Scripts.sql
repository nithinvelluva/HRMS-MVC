IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_SCHEMA = 'dbo'  AND  TABLE_NAME = 'LeaveType'))
BEGIN
CREATE TABLE [dbo].[LeaveType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Type] [nchar](10) NOT NULL,
	[Count] [int] NOT NULL,
 CONSTRAINT [PK_LeaveType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
-----------------------------------------------
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_SCHEMA = 'dbo'  AND  TABLE_NAME = 'UserPrivilages'))
BEGIN
CREATE TABLE [dbo].[UserPrivilages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Privilage] [nchar](10) NOT NULL,
 CONSTRAINT [PK_UserPrivilages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
--------------------------------------------------------------

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_SCHEMA = 'dbo'  AND  TABLE_NAME = 'EmpDesignation'))
BEGIN
CREATE TABLE [dbo].[EmpDesignation](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Designation] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_EmpDesignation] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END

-------------------------------------------------------------------------------

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_SCHEMA = 'dbo'  AND  TABLE_NAME = 'UserInfo'))
BEGIN

CREATE TABLE [dbo].[UserInfo](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](max) NOT NULL,
	[UserType] [int] NOT NULL,
 CONSTRAINT [PK_UserInfo] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END
------------------------------------------------------------
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_SCHEMA = 'dbo'  AND  TABLE_NAME = 'EmployeeInfo'))
BEGIN
CREATE TABLE [dbo].[EmployeeInfo](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[EmpId] [int] NOT NULL,
	[EmpPhone] [nchar](10) NOT NULL,
	[EmpEmail] [nvarchar](50) NOT NULL,
	[EmpGender] [nchar](10) NOT NULL,
	[EmpDob] [datetime] NOT NULL,
	[EmpPhotoPath] [nvarchar](max) NULL,
	[EmpDesignation] [int] NULL,
	[EmpFirstname] [nvarchar](50) NULL,
	[EmpLastname] [nvarchar](50) NULL,
 CONSTRAINT [PK_Employee Info] PRIMARY KEY CLUSTERED 
(
	[EmpId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

ALTER TABLE [dbo].[EmployeeInfo]  WITH CHECK ADD FOREIGN KEY([EmpDesignation])
REFERENCES [dbo].[EmpDesignation] ([ID])

ALTER TABLE [dbo].[EmployeeInfo]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeInfo_UserInfo] FOREIGN KEY([EmpId])
REFERENCES [dbo].[UserInfo] ([ID])

ALTER TABLE [dbo].[EmployeeInfo] CHECK CONSTRAINT [FK_EmployeeInfo_UserInfo]

END
--------------------------------------------------------------------------------------
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_SCHEMA = 'dbo'  AND  TABLE_NAME = 'Attendance'))
BEGIN
CREATE TABLE [dbo].[Attendance](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[EmpId] [int] NOT NULL,
	[PunchinTime] [datetime] NULL,
	[PunchoutTime] [datetime] NULL,
	[Notes] [nvarchar](max) NULL,
 CONSTRAINT [PK_Attendance] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END
---------------------------------------------------------------------
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_SCHEMA = 'dbo'  AND  TABLE_NAME = 'calendar_event_archive'))
BEGIN
CREATE TABLE [dbo].[calendar_event_archive](
	[Id] [int] NOT NULL,
	[task_id] [int] NULL,
	[filename] [nvarchar](max) NULL,
	[filepath] [nvarchar](max) NULL,
	[date] [nvarchar](20) NULL,
 CONSTRAINT [PK_calendar_event_archive] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
---------------------------------------------------------------
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_SCHEMA = 'dbo'  AND  TABLE_NAME = 'calendar_event_employees'))
BEGIN
CREATE TABLE [dbo].[calendar_event_employees](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[task_id] [int] NULL,
	[employee_id] [int] NULL,
 CONSTRAINT [PK_calendar_event_employees] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
--------------------------------------------------------------------------------------
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_SCHEMA = 'dbo'  AND  TABLE_NAME = 'calendar_event_info'))
BEGIN
CREATE TABLE [dbo].[calendar_event_info](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[heading] [nvarchar](50) NULL,
	[note] [nvarchar](max) NULL,
	[status] [int] NULL,
	[event_type] [int] NULL,
 CONSTRAINT [PK_calendar_event_info] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
--------------------------------------------------------------------------------------
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_SCHEMA = 'dbo'  AND  TABLE_NAME = 'calendar_event_info_dates'))
BEGIN
CREATE TABLE [dbo].[calendar_event_info_dates](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[task_id] [int] NULL,
	[start_date] [nvarchar](30) NULL,
	[end_date] [nvarchar](30) NULL,
 CONSTRAINT [PK_calendar_event_info_dates] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
----------------------------------------------------------
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_SCHEMA = 'dbo'  AND  TABLE_NAME = 'calendar_event_log'))
BEGIN
CREATE TABLE [dbo].[calendar_event_log](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[task_id] [int] NULL,
	[created_by] [int] NULL,
	[created_at] [nvarchar](30) NULL,
	[event_log] [nvarchar](max) NULL,
 CONSTRAINT [PK_calendar_event_log] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
--------------------------------------------------
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_SCHEMA = 'dbo'  AND  TABLE_NAME = 'calendar_event_status'))
BEGIN
CREATE TABLE [dbo].[calendar_event_status](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[status] [nvarchar](20) NULL,
 CONSTRAINT [PK_calendar_event_status] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
-------------------------------------------------------------------------
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_SCHEMA = 'dbo'  AND  TABLE_NAME = 'calendar_event_type'))
BEGIN
CREATE TABLE [dbo].[calendar_event_type](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[type_name] [nvarchar](20) NULL,
 CONSTRAINT [PK_calendar_event_type] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
-------------------------------------------------------------------------------
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_SCHEMA = 'dbo'  AND  TABLE_NAME = 'calendar_event_treeitems'))
BEGIN
CREATE TABLE [dbo].[calendar_event_treeitems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[task_id] [int] NULL,
	[name] [nvarchar](max) NULL,
	[text] [nvarchar](max) NULL,
	[parent_id] [int] NULL,
	[node_type] [nvarchar](10) NULL,
	[deleted_at] [nvarchar](30) NULL,
	[created_at] [nvarchar](30) NULL,
	[updated_at] [nvarchar](30) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
-----------------------------------------------------------

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_SCHEMA = 'dbo'  AND  TABLE_NAME = 'EmployeeLeaveInfo'))
BEGIN
CREATE TABLE [dbo].[EmployeeLeaveInfo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmpId] [int] NOT NULL,
	[FromDate] [datetime] NULL,
	[ToDate] [datetime] NULL,
	[LeaveType] [int] NULL,
	[Status] [int] NULL,
	[Comments] [varchar](50) NULL,
	[DurationType] [varchar](15) NULL,
	[CalendarEntryId] [int] NULL,
	[LeaveSessionType] [int] NULL,
 CONSTRAINT [PK_EmployeeLeaveInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[EmployeeLeaveInfo]  WITH CHECK ADD FOREIGN KEY([EmpId])
REFERENCES [dbo].[EmployeeInfo] ([EmpId])

ALTER TABLE [dbo].[EmployeeLeaveInfo]  WITH CHECK ADD FOREIGN KEY([LeaveType])
REFERENCES [dbo].[LeaveType] ([Id])
END
-------------------------------------------------
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_SCHEMA = 'dbo'  AND  TABLE_NAME = 'HrmsQuery'))
BEGIN
CREATE TABLE [dbo].[HrmsQuery](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[QuerySubject] [nvarchar](max) NULL,
	[QueryBody] [nvarchar](max) NULL,
	[Date] [datetime] NULL,
	[IsRead] [bit] NULL,
	[EmpId] [int] NULL,
 CONSTRAINT [PK_HrmsQuery] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
-------------------------------------------------------------------
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_SCHEMA = 'dbo'  AND  TABLE_NAME = 'LeaveStatistics'))
BEGIN
CREATE TABLE [dbo].[LeaveStatistics](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmpId] [int] NOT NULL,
	[CasualLeave] [float] NULL,
	[FestiveLeave] [float] NULL,
	[SickLeave] [float] NULL,
	[LossOfPay] [float] NULL,
	[Year] [int] NULL,
 CONSTRAINT [PK_LeaveStatistics] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[LeaveStatistics]  WITH CHECK ADD FOREIGN KEY([EmpId])
REFERENCES [dbo].[EmployeeInfo] ([EmpId])
END
---------------------------------------------------------------

