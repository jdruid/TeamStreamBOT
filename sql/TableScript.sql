USE [teamstream]
GO


DROP TABLE [dbo].[Caption]
GO
create table Caption (
    [Id] [int] IDENTITY(1,1) NOT NULL,
	[videoId] UNIQUEIDENTIFIER NOT NULL,
	[thumbnailIndex] INT NOT NULL,
    [text] [varchar](50) NULL,
    [confidence] [decimal](9,2) NOT NULL,
CONSTRAINT [PK_Caption] PRIMARY KEY CLUSTERED
   (
      [Id] asc
   )
)

DROP TABLE [dbo].[Description]
GO
CREATE TABLE Description (
    [Id] [int] IDENTITY(1,1) NOT NULL,
	[videoId] UNIQUEIDENTIFIER NOT NULL,
	[thumbnailIndex] INT NOT NULL,
    [tags] [varchar](50) NULL
CONSTRAINT [PK_Description] PRIMARY KEY CLUSTERED
   (
      [Id] asc
   )
)

DROP TABLE [dbo].[Metadata]
GO
CREATE TABLE Metadata (
    [Id] [int] IDENTITY(1,1) NOT NULL,
	[videoId] UNIQUEIDENTIFIER NOT NULL,
	[thumbnailIndex] INT NOT NULL,
    [width] [int] NOT NULL,
    [height] [int] NOT NULL,
    [format] [varchar](50) NULL,
CONSTRAINT [PK_Metadata] PRIMARY KEY CLUSTERED
   (
      [Id] asc
   )
)

DROP TABLE [dbo].[Video]
GO
CREATE TABLE [dbo].[Video](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](128) NULL,
	[RawUrl] [nvarchar](512) NULL,
	[Keywords] [nvarchar](512) NULL,
	[Date] [date] NULL,
 CONSTRAINT [PK_Video] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

DROP TABLE [dbo].[VisionAnalysis]
GO
CREATE TABLE VisionAnalysis (
    [Id] UNIQUEIDENTIFIER NOT NULL,
	[videoId] UNIQUEIDENTIFIER NOT NULL,
	[requestId] NVARCHAR(512) NOT NULL,
	[thumbnailUrl] NVARCHAR(512) NOT NULL,
	[thumbnailIndex] INT NOT NULL,
	[thumbnailCount] INT NOT NULL
CONSTRAINT [PK_VisionAnalysis] PRIMARY KEY CLUSTERED
   (
      [Id] asc
   )
)


