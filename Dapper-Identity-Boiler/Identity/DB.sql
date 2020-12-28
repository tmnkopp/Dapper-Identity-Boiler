/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 15-Mar-17 10:27:06 PM ******/
 
IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetRoles') 
BEGIN
    ALTER TABLE "AspNetUserRoles" 
    DROP CONSTRAINT "FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId";

    ALTER TABLE "AspNetUserRoles" 
    DROP CONSTRAINT "FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId";

    DROP TABLE AspNetRoles 
END
CREATE TABLE [dbo].[AspNetRoles]( 
    [Id] [nvarchar](128) NOT NULL, 
    [Name] [nvarchar](256) NOT NULL, 
    CONSTRAINT [PK_dbo.AspNetRoles] PRIMARY KEY CLUSTERED     (   [Id] ASC   )  
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 15-Mar-17 10:27:06 PM ******/ 
IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetUserClaims') 
BEGIN
    ALTER TABLE "AspNetUserClaims" 
    DROP CONSTRAINT "FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId";
    DROP TABLE AspNetUserClaims
END
CREATE TABLE [dbo].[AspNetUserClaims](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [UserId] [nvarchar](128) NOT NULL,
    [ClaimType] [nvarchar](max) NULL,
    [ClaimValue] [nvarchar](max) NULL, 
    CONSTRAINT [PK_dbo.AspNetUserClaims] PRIMARY KEY CLUSTERED ([Id] ASC) 
) ON [PRIMARY] 
GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 15-Mar-17 10:27:06 PM ******/ 
IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetUserLogins') 
BEGIN
    DROP TABLE AspNetUserLogins
END
CREATE TABLE [dbo].[AspNetUserLogins]( 
    [LoginProvider] [nvarchar](128) NOT NULL, 
    [ProviderKey] [nvarchar](128) NOT NULL, 
    [UserId] [nvarchar](128) NOT NULL, 
    CONSTRAINT [PK_dbo.AspNetUserLogins] PRIMARY KEY CLUSTERED 
    ( 
        [LoginProvider] ASC,
        [ProviderKey] ASC,
        [UserId] ASC
    ) 
) ON [PRIMARY] 
GO 
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 15-Mar-17 10:27:06 PM ******/ 
IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetUserRoles') 
BEGIN
    DROP TABLE AspNetUserRoles
END
CREATE TABLE [dbo].[AspNetUserRoles]( 
    [UserId] [nvarchar](128) NOT NULL, 
    [RoleId] [nvarchar](128) NOT NULL, 
    CONSTRAINT [PK_dbo.AspNetUserRoles] PRIMARY KEY CLUSTERED 
    ( 
        [UserId] ASC, 
        [RoleId] ASC 
    )  
) ON [PRIMARY]
 
GO 
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 15-Mar-17 10:27:06 PM ******/ 
IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetUsers') 
BEGIN
    DROP TABLE AspNetUsers
END
CREATE TABLE [dbo].[AspNetUsers](

	[Id] [nvarchar](128) NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	[Email] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[LockoutEnd] [datetimeoffset](7) NULL,
	[NormalizedEmail] [nvarchar](256) NULL,
	[NormalizedUserName] [nvarchar](256) NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[UserName] [nvarchar](256) NULL, 
    CONSTRAINT [PK_dbo.AspNetUsers] PRIMARY KEY CLUSTERED   (   [Id] ASC    ) 

) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]



GO

ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId]
GO

ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId]) 
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId]
GO

ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId]
GO

ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId] 
GO