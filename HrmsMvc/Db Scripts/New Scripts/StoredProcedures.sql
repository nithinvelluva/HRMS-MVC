DECLARE @Sql Nvarchar(max);

set @Sql = 'CREATE FUNCTION [dbo].[SplitString]
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
END'
EXEC SP_EXECUTESQL @sql;


-----------------------------------------------------------------------------------------

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.AddEmployeeLeave') AND type in (N'P', N'PC'))
DROP PROCEDURE dbo.[AddEmployeeLeave]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[AddEmployeeLeave]
(
@empId INT,
@fromdate nvarchar(50),
@todate nvarchar(50),
@leaveType INT,
@status INT,
@comments nvarchar(max),
@leavedurationtype varchar(10),
@calendarEntryId INT,
@leaveId INT OUT,
@leaveSessionType INT
)

AS
BEGIN
INSERT INTO EmployeeLeaveInfo([EmpId],[FromDate],[ToDate],[LeaveType],[status],[Comments],[DurationType],[CalendarEntryId],[LeaveSessionType])
VALUES(@empId,@fromdate,@todate,@leaveType,@status,@comments,@leavedurationtype,@calendarEntryId,@leaveSessionType)

Select @leaveId=@@IDENTITY
RETURN @leaveId

END







GO
/****** Object:  StoredProcedure [dbo].[AddLeaveInfo]    Script Date: 2018-03-03 18:18:42 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.AddLeaveInfo') AND type in (N'P', N'PC'))
DROP PROCEDURE dbo.[AddLeaveInfo]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[AddLeaveInfo]
(
  
  @year int
)

AS
BEGIN

IF OBJECT_ID('tempdb.dbo.#TempTable', 'U') IS NOT NULL
  DROP TABLE #TempTable;

  DECLARE @count int;
  DECLARE @loopVar int ;
  DECLARE @i int ;
  DECLARE @casual float ;
  DECLARE @festive float ;
  DECLARE @sick float;

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
	 DECLARE @EXIST_ROW int = 0;
	 SELECT @EXIST_ROW = ID FROM LeaveStatistics where EmpId = @empid AND Year = @year
	 SELECT @loopVar = @loopVar + 1
	 IF @EXIST_ROW <= 0
		 INSERT into LeaveStatistics(EmpId,CasualLeave,FestiveLeave,SickLeave,LossOfPay,[Year])
		 VALUES(@empid,@casual,@festive,@sick,'0',@year)
	 
END
IF OBJECT_ID('tempdb.dbo.#TempTable', 'U') IS NOT NULL
  DROP TABLE #TempTable;
END







GO
/****** Object:  StoredProcedure [dbo].[CreateCalendarTask]    Script Date: 2018-03-03 18:18:42 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.CreateCalendarTask') AND type in (N'P', N'PC'))
DROP PROCEDURE dbo.[CreateCalendarTask]
GO
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
@isLeaveTask bit,
@leaveId INT,
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

IF @isLeaveTask = 1
  UPDATE EmployeeLeaveInfo SET CalendarEntryId = @task_id WHERE Id = @leaveId;
END







GO
/****** Object:  StoredProcedure [dbo].[EditCalendarTask]    Script Date: 2018-03-03 18:18:42 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.EditCalendarTask') AND type in (N'P', N'PC'))
DROP PROCEDURE dbo.[EditCalendarTask]
GO
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

Go
/****** Object:  StoredProcedure [dbo].[InsertEmployeeDetailsMVC]    Script Date: 2018-03-03 18:18:42 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.InsertEmployeeDetailsMVC') AND type in (N'P', N'PC'))
DROP PROCEDURE dbo.[InsertEmployeeDetailsMVC]
GO
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
/****** Object:  StoredProcedure [dbo].[RemoveCalendarTask]    Script Date: 2018-03-03 18:18:42 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.RemoveCalendarTask') AND type in (N'P', N'PC'))
DROP PROCEDURE dbo.[RemoveCalendarTask]
GO
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
/****** Object:  StoredProcedure [dbo].[RemoveEmployee]    Script Date: 2018-03-03 18:18:42 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.RemoveEmployee') AND type in (N'P', N'PC'))
DROP PROCEDURE dbo.[RemoveEmployee]
GO
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
/****** Object:  StoredProcedure [dbo].[SplitEmployeeName]    Script Date: 2018-03-03 18:18:42 ******/
--IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.SplitEmployeeName') AND type in (N'P', N'PC'))
--DROP PROCEDURE dbo.[SplitEmployeeName]
--GO
--SET ANSI_NULLS ON
--GO
--SET QUOTED_IDENTIFIER ON
--GO

--CREATE procedure [dbo].[SplitEmployeeName]


--AS
--BEGIN

--UPDATE EmployeeInfo SET EmpFirstname = (
--    CASE WHEN CHARINDEX(' ',EmpName)>0 
--         THEN SUBSTRING(EmpName,1,CHARINDEX(' ',EmpName)-1) 
--         ELSE EmpName END
--		 ), EmpLastname =  
--		 (
--     CASE WHEN CHARINDEX(' ',EmpName)>0 
--         THEN SUBSTRING(EmpName,CHARINDEX(' ',EmpName)+1,len(EmpName))  
--         ELSE NULL END)
--FROM EmployeeInfo
--DECLARE @COUNT INT = 0;
--SELECT @COUNT = COUNT(*) FROM EmployeeInfo;
--RETURN @COUNT;
--END
