USE [Hrms]
GO
/****** Object:  StoredProcedure [dbo].[AddEmployeeReturnID]    Script Date: 2/27/2018 3:11:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AddEmployeeReturnID] 
	@FirstName varchar(50),
	@LastName varchar(50),
	@BirthDate datetime,
	@City varchar(50),
	@Country varchar(50)
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO  Employees (FirstName, LastName, BirthDate, City, Country)
	VALUES (@FirstName, @LastName, @BirthDate, @City, @Country) 
	SELECT SCOPE_IDENTITY()
END

GO
/****** Object:  StoredProcedure [dbo].[AddLeaveInfo]    Script Date: 2/27/2018 3:11:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[AddLeaveInfo]
(
  @count int OUT,
  @loopVar int OUT,
  @i int OUT,
  @casual float OUT,
  @festive float OUT,
  @sick float OUT,
  @year int
)

AS
BEGIN

IF OBJECT_ID('tempdb.dbo.#TempTable', 'U') IS NOT NULL
  DROP TABLE #TempTable;

  CREATE TABLE #TempTable
  (
    ID INT IDENTITY(1, 1) primary key ,
    EmpId INT
  );

INSERT INTO #TempTable SELECT ID FROM UserInfo WHERE UserType != 1 

SELECT @count = COUNT(ID) FROM #TempTable
SELECT @loopVar = 0
SELECT @i = 0

SELECT @casual = Count From LeaveType WHERE Type='Casual'
SELECT @festive = Count From LeaveType WHERE Type='Festive'
SELECT @sick = Count From LeaveType WHERE Type='Sick'

WHILE @loopVar < @count
BEGIN
     SELECT @i = @i + 1

     DECLARE @empid int;
	 SELECT @empid = EmpId from #TempTable where ID = @i	 

	 INSERT into LeaveStatistics(EmpId,CasualLeave,FestiveLeave,SickLeave,LossOfPay,[Year])
	 VALUES(@empid,@casual,@festive,@sick,'0',@year)

	 SELECT @loopVar = @loopVar + 1
END

END






GO
/****** Object:  StoredProcedure [dbo].[CreateCalendarTask]    Script Date: 2/27/2018 3:11:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[CreateCalendarTask]
(
@event_type INT,
@heading nvarchar(50),
@note nvarchar(max),
@status INT,
@start_date nvarchar(50),
@end_date nvarchar(50),
@employee_id nvarchar(max),
@task_id INT OUT
)

AS
BEGIN
INSERT INTO calendar_event_info([heading],[note],[status],[event_type])
VALUES(@heading,@note,@status,@event_type)

Select @task_id=@@IDENTITY 

INSERT INTO calendar_event_info_dates([task_id],[start_date],[end_date])
VALUES(@task_id,@start_date,@end_date)

-- Insert statement
INSERT INTO calendar_event_employees
SELECT @task_id, CAST(Item AS INT) 
FROM  dbo.SplitString(@employee_id, ',')   


END






GO
/****** Object:  StoredProcedure [dbo].[EditCalendarTask]    Script Date: 2/27/2018 3:11:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[EditCalendarTask]
(
@heading nvarchar(50),
@note nvarchar(max),
@status INT,
@start_date nvarchar(50),
@end_date nvarchar(50),
@employee_id nvarchar(max),
@task_id INT
)

AS
BEGIN

UPDATE calendar_event_info SET [heading] = @heading,[note] = @note,[status] = @status 
WHERE id = @task_id 

UPDATE calendar_event_info_dates SET [start_date] = @start_date,[end_date] = @end_date
WHERE task_id = @task_id

DELETE FROM calendar_event_employees WHERE task_id = @task_id
-- Insert statement
INSERT INTO calendar_event_employees
SELECT @task_id, CAST(Item AS INT) 
FROM  dbo.SplitString(@employee_id, ',')

END






GO
/****** Object:  StoredProcedure [dbo].[InsertEmployeeDetails]    Script Date: 2/27/2018 3:11:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[InsertEmployeeDetails]
(
@empname varchar(10),
@empusername varchar(10),
@emppassword varchar(10),
@empgender varchar(5),
@empphone varchar(10),
@empmail nvarchar(50),
@usertype INT,
@dob DATETIME,
@userphotopath nvarchar(max),
@empid int OUT,
@casual float OUT,
@festive float OUT,
@sick float OUT
)

AS
BEGIN
INSERT INTO RegiInfo(name,uname,pwd,gender,phone,EmailID,UserType,dob,PhotoPath) 
VALUES(@empname,@empusername,@emppassword,@empgender,@empphone,@empmail,@usertype,@dob,@userphotopath)

Select @empid=@@IDENTITY 
select @casual=Count From LeaveType WHERE Type='Casual'
select @festive=Count From LeaveType WHERE Type='Festive'
select @sick=Count From LeaveType WHERE Type='Sick'

INSERT into LeaveStatistics(EmpId,CasualLeave,FestiveLeave,SickLeave,LossOfPay) VALUES(@empid,@casual,@festive,@sick,'0')
END





GO
/****** Object:  StoredProcedure [dbo].[InsertEmployeeDetailsMVC]    Script Date: 2/27/2018 3:11:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[InsertEmployeeDetailsMVC]
(
@empfirstname nvarchar(50),
@emplastname nvarchar(50),
@empusername varchar(10),
@emppassword nvarchar(max),
@empgender varchar(5),
@empphone varchar(10),
@empmail nvarchar(50),
@usertype INT,
@designation INT,
@empdob DATETIME,
@empphotopath nvarchar(max),
@UserId int OUT,
@empid int OUT,
@casual float OUT,
@festive float OUT,
@sick float OUT,
@year INT OUT
)

AS
BEGIN
INSERT INTO UserInfo([UserName],[Password],[UserType])
VALUES(@empusername,@emppassword,@usertype)

Select @UserId=@@IDENTITY 

INSERT INTO EmployeeInfo([EmpId],[EmpFirstname],[EmpLastname],[EmpPhone],[EmpEmail],[EmpGender],[EmpDob],[EmpPhotoPath],[EmpDesignation])
VALUES(@UserId,@empfirstname,@emplastname,@empphone,@empmail,@empgender,@empdob,@empphotopath,@designation)

Select @empid=@@IDENTITY 

select @casual=Count From LeaveType WHERE Type='Casual'
select @festive=Count From LeaveType WHERE Type='Festive'
select @sick=Count From LeaveType WHERE Type='Sick'

INSERT into LeaveStatistics(EmpId,CasualLeave,FestiveLeave,SickLeave,LossOfPay,[Year]) VALUES(@UserId,@casual,@festive,@sick,'0',@year)
END






GO
/****** Object:  StoredProcedure [dbo].[RemoveCalendarTask]    Script Date: 2/27/2018 3:11:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[RemoveCalendarTask]
(
@task_id INT
)
AS
Begin
DELETE FROM calendar_event_employees WHERE task_id=@task_id
DELETE FROM calendar_event_treeitems WHERE task_id=@task_id
DELETE FROM calendar_event_info_dates WHERE task_id=@task_id
DELETE FROM calendar_event_log WHERE task_id=@task_id
DELETE FROM calendar_event_info WHERE ID=@task_id

END



GO
/****** Object:  StoredProcedure [dbo].[RemoveEmployee]    Script Date: 2/27/2018 3:11:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[RemoveEmployee]
(
@empid INT
)
AS
Begin
DELETE FROM Attendance WHERE EmpId=@empid
DELETE FROM EmployeeLeaveInfo WHERE EmpId=@empid
DELETE FROM LeaveStatistics WHERE EmpId=@empid
DELETE FROM HrmsQuery WHERE EmpId=@empid
DELETE FROM EmployeeInfo WHERE EmpId=@empid
DELETE FROM UserInfo WHERE ID=@empid

END



GO
/****** Object:  UserDefinedFunction [dbo].[SplitString]    Script Date: 2/27/2018 3:11:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[SplitString]
(	
	@Input NVARCHAR(MAX),
	@Character CHAR(1)
)
RETURNS @Output TABLE (
	Item NVARCHAR(1000)
)
AS
BEGIN
	DECLARE @StartIndex INT, @EndIndex INT

	SET @StartIndex = 1
	IF SUBSTRING(@Input, LEN(@Input) - 1, LEN(@Input)) <> @Character
	BEGIN
		SET @Input = @Input + @Character
	END

	WHILE CHARINDEX(@Character, @Input) > 0
	BEGIN
		SET @EndIndex = CHARINDEX(@Character, @Input)
		
		INSERT INTO @Output(Item)
		SELECT SUBSTRING(@Input, @StartIndex, @EndIndex - 1)
		
		SET @Input = SUBSTRING(@Input, @EndIndex + 1, LEN(@Input))
	END

	RETURN
END


GO
/****** Object:  Table [dbo].[Attendance]    Script Date: 2/27/2018 3:11:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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

GO
/****** Object:  Table [dbo].[calendar_event_archive]    Script Date: 2/27/2018 3:11:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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

GO
/****** Object:  Table [dbo].[calendar_event_employees]    Script Date: 2/27/2018 3:11:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[calendar_event_employees](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[task_id] [int] NULL,
	[employee_id] [int] NULL,
 CONSTRAINT [PK_calendar_event_employees] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[calendar_event_info]    Script Date: 2/27/2018 3:11:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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

GO
/****** Object:  Table [dbo].[calendar_event_info_dates]    Script Date: 2/27/2018 3:11:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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

GO
/****** Object:  Table [dbo].[calendar_event_log]    Script Date: 2/27/2018 3:11:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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

GO
/****** Object:  Table [dbo].[calendar_event_status]    Script Date: 2/27/2018 3:11:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[calendar_event_status](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[status] [nvarchar](20) NULL,
 CONSTRAINT [PK_calendar_event_status] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[calendar_event_treeitems]    Script Date: 2/27/2018 3:11:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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

GO
/****** Object:  Table [dbo].[calendar_event_type]    Script Date: 2/27/2018 3:11:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[calendar_event_type](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[type_name] [nvarchar](20) NULL,
 CONSTRAINT [PK_calendar_event_type] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[EmpDesignation]    Script Date: 2/27/2018 3:11:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmpDesignation](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Designation] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_EmpDesignation] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[EmployeeInfo]    Script Date: 2/27/2018 3:11:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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

GO
/****** Object:  Table [dbo].[EmployeeLeaveInfo]    Script Date: 2/27/2018 3:11:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EmployeeLeaveInfo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmpId] [int] NOT NULL,
	[FromDate] [datetime] NULL,
	[ToDate] [datetime] NULL,
	[LeaveType] [int] NULL,
	[Status] [int] NULL,
	[Comments] [varchar](50) NULL,
	[DurationType] [varchar](15) NULL,
 CONSTRAINT [PK_EmployeeLeaveInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[HrmsQuery]    Script Date: 2/27/2018 3:11:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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

GO
/****** Object:  Table [dbo].[LeaveStatistics]    Script Date: 2/27/2018 3:11:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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

GO
/****** Object:  Table [dbo].[LeaveType]    Script Date: 2/27/2018 3:11:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LeaveType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Type] [nchar](10) NOT NULL,
	[Count] [int] NOT NULL,
 CONSTRAINT [PK_LeaveType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserInfo]    Script Date: 2/27/2018 3:11:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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

GO
/****** Object:  Table [dbo].[UserPrivilages]    Script Date: 2/27/2018 3:11:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserPrivilages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Privilage] [nchar](10) NOT NULL,
 CONSTRAINT [PK_UserPrivilages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[Attendance]  WITH CHECK ADD FOREIGN KEY([EmpId])
REFERENCES [dbo].[EmployeeInfo] ([EmpId])
GO
ALTER TABLE [dbo].[EmployeeInfo]  WITH CHECK ADD FOREIGN KEY([EmpDesignation])
REFERENCES [dbo].[EmpDesignation] ([ID])
GO
ALTER TABLE [dbo].[EmployeeInfo]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeInfo_UserInfo] FOREIGN KEY([EmpId])
REFERENCES [dbo].[UserInfo] ([ID])
GO
ALTER TABLE [dbo].[EmployeeInfo] CHECK CONSTRAINT [FK_EmployeeInfo_UserInfo]
GO
ALTER TABLE [dbo].[EmployeeLeaveInfo]  WITH CHECK ADD FOREIGN KEY([EmpId])
REFERENCES [dbo].[EmployeeInfo] ([EmpId])
GO
ALTER TABLE [dbo].[EmployeeLeaveInfo]  WITH CHECK ADD FOREIGN KEY([LeaveType])
REFERENCES [dbo].[LeaveType] ([Id])
GO
ALTER TABLE [dbo].[LeaveStatistics]  WITH CHECK ADD FOREIGN KEY([EmpId])
REFERENCES [dbo].[EmployeeInfo] ([EmpId])
GO
ALTER TABLE [dbo].[UserInfo]  WITH CHECK ADD FOREIGN KEY([UserType])
REFERENCES [dbo].[UserPrivilages] ([Id])
GO
