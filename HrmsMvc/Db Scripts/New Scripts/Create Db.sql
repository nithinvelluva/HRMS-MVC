use master
IF (EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE ('[' + name + ']' = 'Hrms')))
BEGIN
	DROP DATABASE [Hrms]
END
ELSE
BEGIN
	CREATE DATABASE [Hrms] 
END