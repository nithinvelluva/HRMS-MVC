USE [master]
GO

/****** Object:  Database [LoginInfo]    Script Date: 2018-01-26 12:15:30 PM ******/
DROP DATABASE [LoginInfo]
GO

/****** Object:  Database [LoginInfo]    Script Date: 2018-01-26 12:15:30 PM ******/
CREATE DATABASE [LoginInfo]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'LoginInfo', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.SQLEXPRESS\MSSQL\DATA\LoginInfo.mdf' , SIZE = 6144KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'LoginInfo_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.SQLEXPRESS\MSSQL\DATA\LoginInfo_log.ldf' , SIZE = 29504KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO

ALTER DATABASE [LoginInfo] SET COMPATIBILITY_LEVEL = 120
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [LoginInfo].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [LoginInfo] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [LoginInfo] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [LoginInfo] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [LoginInfo] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [LoginInfo] SET ARITHABORT OFF 
GO

ALTER DATABASE [LoginInfo] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [LoginInfo] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [LoginInfo] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [LoginInfo] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [LoginInfo] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [LoginInfo] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [LoginInfo] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [LoginInfo] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [LoginInfo] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [LoginInfo] SET  DISABLE_BROKER 
GO

ALTER DATABASE [LoginInfo] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [LoginInfo] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [LoginInfo] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [LoginInfo] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [LoginInfo] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [LoginInfo] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [LoginInfo] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [LoginInfo] SET RECOVERY SIMPLE 
GO

ALTER DATABASE [LoginInfo] SET  MULTI_USER 
GO

ALTER DATABASE [LoginInfo] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [LoginInfo] SET DB_CHAINING OFF 
GO

ALTER DATABASE [LoginInfo] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [LoginInfo] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO

ALTER DATABASE [LoginInfo] SET DELAYED_DURABILITY = DISABLED 
GO

ALTER DATABASE [LoginInfo] SET  READ_WRITE 
GO


