USE [master]
GO
/****** Object:  Database [PinovationTech]    Script Date: 6/22/2022 4:04:29 PM ******/
CREATE DATABASE [PinovationTech]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'PinovationTech', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL13.SQLEXPRESS\MSSQL\DATA\PinovationTech.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'PinovationTech_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL13.SQLEXPRESS\MSSQL\DATA\PinovationTech_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [PinovationTech] SET COMPATIBILITY_LEVEL = 130
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [PinovationTech].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [PinovationTech] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [PinovationTech] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [PinovationTech] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [PinovationTech] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [PinovationTech] SET ARITHABORT OFF 
GO
ALTER DATABASE [PinovationTech] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [PinovationTech] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [PinovationTech] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [PinovationTech] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [PinovationTech] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [PinovationTech] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [PinovationTech] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [PinovationTech] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [PinovationTech] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [PinovationTech] SET  DISABLE_BROKER 
GO
ALTER DATABASE [PinovationTech] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [PinovationTech] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [PinovationTech] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [PinovationTech] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [PinovationTech] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [PinovationTech] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [PinovationTech] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [PinovationTech] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [PinovationTech] SET  MULTI_USER 
GO
ALTER DATABASE [PinovationTech] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [PinovationTech] SET DB_CHAINING OFF 
GO
ALTER DATABASE [PinovationTech] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [PinovationTech] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [PinovationTech] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [PinovationTech] SET QUERY_STORE = OFF
GO
USE [PinovationTech]
GO
ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO
ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO
USE [PinovationTech]
GO
/****** Object:  UserDefinedFunction [dbo].[SF_FemaleId]    Script Date: 6/22/2022 4:04:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[SF_FemaleId]() 
RETURNS varchar(7)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @UserId varchar(7),@NewId int,@result bit = 1
	SELECT Top(1) @UserId = userId FROM TblUsers where userId % 2 <> 0 order by userId desc;
	Set @NewId = Case When @UserId is Null Then 1 Else Cast(@UserId as int)+2 End
		if(@NewId > 1) Begin
			While (@NewId < 9999999) Begin
			Set @result = dbo.SF_PrimeCheck(@NewId)
				if(@result = 0) Break
				Set @NewId += 2
			End
		End
	Set @UserId = REPLACE(STR(@NewId,7),' ', '0')
	-- Return the result of the function
	RETURN @UserId

END
GO
/****** Object:  UserDefinedFunction [dbo].[SF_MaleId]    Script Date: 6/22/2022 4:04:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[SF_MaleId]() 
RETURNS varchar(7)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @UserId varchar(7),@NewId int,@result bit = 1
	SELECT Top(1) @UserId = userId FROM TblUsers where userId % 2 = 0 order by userId desc;
	Set @NewId = Case When @UserId is Null Then 2 Else Cast(@UserId as int)+2 End
		
		While (@NewId < 9999999) Begin
		Set @result = dbo.SF_PrimeCheck(@NewId)
			if(@result = 0) Break
			Set @NewId += 2
		End
	Set @UserId = REPLACE(STR(@NewId,7),' ', '0')
	-- Return the result of the function
	RETURN @UserId
END
GO
/****** Object:  UserDefinedFunction [dbo].[SF_OtherId]    Script Date: 6/22/2022 4:04:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[SF_OtherId]() 
RETURNS varchar(7)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @UserId varchar(7),@NewId int,@result bit = 1

	select top(1) @UserId = userId from TblUsers where Cast(userId as int)>1 and userId not in 
	(   select distinct n2id 
		from 
		(   select n1.userId as n1id, n2.userId as n2id 
			from TblUsers n1 
			cross join TblUsers n2 
			where Cast(n1.userId as int)<Cast(n2.userId as int) and Cast(n1.userId as int)>1 
			and (Cast(n2.userId as int) % Cast(n1.userId as int) = 0) 
		) xDerived 
	) 
	order by userId desc; 
	-- Return the result of the function
	Set @NewId = Case When @UserId is Null Then 2 Else Cast(@UserId as int)+1 End
		
		While (@NewId < 9999999) Begin
		Set @result = dbo.SF_PrimeCheck(@NewId)
			if(@result = 1) Break
			Set @NewId += 1
		End
	Set @UserId = REPLACE(STR(@NewId,7),' ', '0')
	-- Return the result of the function
	RETURN @UserId

END
GO
/****** Object:  UserDefinedFunction [dbo].[SF_PrimeCheck]    Script Date: 6/22/2022 4:04:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create FUNCTION [dbo].[SF_PrimeCheck](@number int) 
RETURNS bit
AS
BEGIN
	-- Declare the return variable here
	Declare @result bit = 1,@i int = 2
	While (@i<@number)
	Begin
		if(@number % @i = 0)
		Begin
			Set @result = 0
			break
		End
		Set @i += 1
	End
	return @result

END
GO
/****** Object:  Table [dbo].[TblUsers]    Script Date: 6/22/2022 4:04:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TblUsers](
	[userId] [varchar](7) NOT NULL,
	[fName] [varchar](100) NOT NULL,
	[lName] [varchar](100) NULL,
	[phoneNo] [int] NOT NULL,
	[emailNo] [varchar](250) NOT NULL,
	[userCity] [bigint] NULL,
	[userImg] [varchar](max) NULL,
	[userCV] [varchar](max) NULL,
	[password] [varchar](max) NULL,
	[dob] [datetime] NULL,
 CONSTRAINT [Primarykeyname] PRIMARY KEY CLUSTERED 
(
	[userId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TblCity]    Script Date: 6/22/2022 4:04:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TblCity](
	[cityId] [bigint] IDENTITY(1,1) NOT NULL,
	[cityName] [varchar](100) NULL,
	[countryId] [int] NOT NULL,
 CONSTRAINT [PK_TblCity] PRIMARY KEY CLUSTERED 
(
	[cityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TblCountry]    Script Date: 6/22/2022 4:04:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TblCountry](
	[countryId] [int] IDENTITY(1,1) NOT NULL,
	[countryName] [varchar](100) NULL,
 CONSTRAINT [PK_TblCountry] PRIMARY KEY CLUSTERED 
(
	[countryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  UserDefinedFunction [dbo].[ITF_User]    Script Date: 6/22/2022 4:04:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[ITF_User] 
(	
	@UserId INT
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT 
	Case When userId > 1 And dbo.SF_PrimeCheck(userId) = 1 Then 'Other' 
	When userId % 2 = 0 And dbo.SF_PrimeCheck(userId) = 0 Then 'Male' Else 'Female' End As genderName,
	CEILING(DATEDIFF(year, dob, GETDATE())) - Case When (Month(dob) > MONTH(GETDATE()))
	Or (Month(dob) = MONTH(GETDATE()) And Day(dob) > Day(GETDATE())) Then 1 Else 0 End As age,
	userId,fName+' '+lName As name,phoneNo,emailNo,userImg,userCV,cityName,countryName
	FROM TblUsers U Left Join TblCity C On U.userCity = C.cityId Left Join TblCountry CY On C.countryId = CY.countryId where userId=@UserId
)
GO
SET IDENTITY_INSERT [dbo].[TblCity] ON 

INSERT [dbo].[TblCity] ([cityId], [cityName], [countryId]) VALUES (1, N'Chittagong', 1)
INSERT [dbo].[TblCity] ([cityId], [cityName], [countryId]) VALUES (2, N'Dhaka', 1)
INSERT [dbo].[TblCity] ([cityId], [cityName], [countryId]) VALUES (3, N'New Delhi', 2)
INSERT [dbo].[TblCity] ([cityId], [cityName], [countryId]) VALUES (4, N'ModdhaPradesh', 2)
SET IDENTITY_INSERT [dbo].[TblCity] OFF
GO
SET IDENTITY_INSERT [dbo].[TblCountry] ON 

INSERT [dbo].[TblCountry] ([countryId], [countryName]) VALUES (1, N'Bangladesh')
INSERT [dbo].[TblCountry] ([countryId], [countryName]) VALUES (2, N'India')
SET IDENTITY_INSERT [dbo].[TblCountry] OFF
GO
ALTER TABLE [dbo].[TblCity]  WITH CHECK ADD  CONSTRAINT [FK_TblCountry_TblCity] FOREIGN KEY([countryId])
REFERENCES [dbo].[TblCountry] ([countryId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TblCity] CHECK CONSTRAINT [FK_TblCountry_TblCity]
GO
ALTER TABLE [dbo].[TblUsers]  WITH CHECK ADD  CONSTRAINT [FK_TblUsers_TblCity] FOREIGN KEY([userCity])
REFERENCES [dbo].[TblCity] ([cityId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TblUsers] CHECK CONSTRAINT [FK_TblUsers_TblCity]
GO
/****** Object:  StoredProcedure [dbo].[MasterCRUD]    Script Date: 6/22/2022 4:04:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[MasterCRUD] (@userId	VARCHAR(7),
                            @fName    VARCHAR(100) = null,
                            @lName    VARCHAR(100) = null,
                            @phoneNo  INTEGER = 0,
                            @emailNo  VARCHAR(250) = null,
							@userCity BIGINT = 0,
							@userImg	VARCHAR(MAX) = null,
							@userCV	VARCHAR(MAX) = null,
							@dob		DATETIME = null,
                            @password  VARCHAR(250) = null,
							@gender	TINYINT = 0,
                            @ProcessType NVARCHAR(20) = '')
AS
  BEGIN
      IF @ProcessType = 'Insert'
        BEGIN
			Set @UserId = Case When @gender = 1 Then dbo.SF_MaleId() 
				When @gender = 2 Then dbo.SF_FemaleId() Else dbo.SF_OtherId() End
			IF @userImg IS NOT NULL Set @userImg = '/uploads/images/'+@UserId+'_'+@userImg
			IF @userCV IS NOT NULL Set @userCV = '/uploads/cvs/'+@UserId+'_'+@userCV
            INSERT INTO TblUsers (userId,fName,lName,phoneNo,emailNo,userCity,userImg,userCV,password,dob)
            OUTPUT inserted.userId
			VALUES ( @UserId,@fName,@lName,@phoneNo,@emailNo,@userCity,@userImg,@userCV,@password,@dob)
        END

      IF @ProcessType = 'Select'
        BEGIN
            SELECT Case When userId > 1 And dbo.SF_PrimeCheck(userId) = 1 Then 'Other' 
				When userId % 2 = 0 And dbo.SF_PrimeCheck(userId) = 0 Then 'Male' Else 'Female' End As genderName,
				U.*,C.countryId FROM TblUsers U Left Join TblCity C On U.userCity = C.cityId
				Where userId = Cast(@userId as Int)
        END

      IF @ProcessType = 'Update'
        BEGIN
            UPDATE TblUsers
            SET    fName = @fName,
                   lName = @lName,
                   phoneNo = @phoneNo,
                   userCity = @userCity,
				   userImg = @userImg,
				   userCV = @userCV,
				   password = @password,
				   dob = @dob
            WHERE  userId = Cast(@userId as Int)
        END
      ELSE IF @ProcessType = 'Delete'
        BEGIN
			DELETE TblUsers OUTPUT DELETED.* FROM TblUsers WHERE  userId = Cast(@userId as Int)
        END
  END
GO
USE [master]
GO
ALTER DATABASE [PinovationTech] SET  READ_WRITE 
GO
