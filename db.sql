/****** Object:  Database [examina]    Script Date: 2024-12-19 9:30:57 PM ******/
CREATE DATABASE [examina]  (EDITION = 'GeneralPurpose', SERVICE_OBJECTIVE = 'GP_S_Gen5_1', MAXSIZE = 32 GB) WITH CATALOG_COLLATION = SQL_Latin1_General_CP1_CI_AS, LEDGER = OFF;
GO
ALTER DATABASE [examina] SET COMPATIBILITY_LEVEL = 160
GO
ALTER DATABASE [examina] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [examina] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [examina] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [examina] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [examina] SET ARITHABORT OFF 
GO
ALTER DATABASE [examina] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [examina] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [examina] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [examina] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [examina] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [examina] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [examina] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [examina] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [examina] SET ALLOW_SNAPSHOT_ISOLATION ON 
GO
ALTER DATABASE [examina] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [examina] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [examina] SET  MULTI_USER 
GO
ALTER DATABASE [examina] SET ENCRYPTION ON
GO
ALTER DATABASE [examina] SET QUERY_STORE = ON
GO
ALTER DATABASE [examina] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 100, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
/*** The scripts of database scoped configurations in Azure should be executed inside the target database connection. ***/
GO
-- ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 8;
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 2024-12-19 9:30:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoleClaims]    Script Date: 2024-12-19 9:30:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoleClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [int] NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 2024-12-19 9:30:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](256) NULL,
	[NormalizedName] [nvarchar](256) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 2024-12-19 9:30:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 2024-12-19 9:30:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](450) NOT NULL,
	[ProviderKey] [nvarchar](450) NOT NULL,
	[ProviderDisplayName] [nvarchar](max) NULL,
	[UserId] [int] NOT NULL,
 CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 2024-12-19 9:30:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [int] NOT NULL,
	[RoleId] [int] NOT NULL,
 CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserTokens]    Script Date: 2024-12-19 9:30:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserTokens](
	[UserId] [int] NOT NULL,
	[LoginProvider] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](450) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[LoginProvider] ASC,
	[Name] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[completed_exams]    Script Date: 2024-12-19 9:30:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[completed_exams](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ExamId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[IsCompleted] [bit] NOT NULL,
	[TotalScore] [float] NOT NULL,
	[GradedAt] [datetime2](7) NULL,
	[CompletedAt] [datetime2](7) NOT NULL,
	[IsSubmitted] [bit] NOT NULL,
 CONSTRAINT [PK_completed_exams] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[exam_attempt]    Script Date: 2024-12-19 9:30:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[exam_attempt](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[QuestionId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[AnswerText] [text] NULL,
	[IsGraded] [bit] NOT NULL,
	[Grade] [float] NULL,
	[SubmittedAt] [datetime2](7) NOT NULL,
	[ExamId] [int] NOT NULL,
	[IsCorrect] [bit] NOT NULL,
 CONSTRAINT [PK_exam_attempt] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[exam_progress]    Script Date: 2024-12-19 9:30:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[exam_progress](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ExamId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[StartedAt] [datetime2](7) NOT NULL,
	[LastUpdated] [datetime2](7) NULL,
	[SavedAnswers] [text] NULL,
	[IsCompleted] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_exam_progress] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[exam_student_assignments]    Script Date: 2024-12-19 9:30:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[exam_student_assignments](
	[AssignedExamsId] [int] NOT NULL,
	[AssignedStudentsId] [int] NOT NULL,
 CONSTRAINT [PK_exam_student_assignments] PRIMARY KEY CLUSTERED 
(
	[AssignedExamsId] ASC,
	[AssignedStudentsId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[exams]    Script Date: 2024-12-19 9:30:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[exams](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](255) NOT NULL,
	[Description] [text] NULL,
	[Subject] [nvarchar](255) NOT NULL,
	[TeacherId] [int] NOT NULL,
	[State] [nvarchar](50) NOT NULL,
	[TotalScoreWeight] [float] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[AssignedStudentIds] [nvarchar](max) NULL,
	[Duration] [int] NOT NULL,
	[HasStarted] [bit] NOT NULL,
	[StartedAt] [datetime2](7) NULL,
	[ClosedAt] [datetime2](7) NULL,
	[IsClosed] [bit] NOT NULL,
	[IsSubmitted] [bit] NOT NULL,
 CONSTRAINT [PK_exams] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[questions]    Script Date: 2024-12-19 9:30:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[questions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ExamId] [int] NOT NULL,
	[QuestionText] [text] NOT NULL,
	[QuestionType] [nvarchar](50) NOT NULL,
	[ChoiceA] [nvarchar](max) NULL,
	[ChoiceB] [nvarchar](max) NULL,
	[ChoiceC] [nvarchar](max) NULL,
	[ChoiceD] [nvarchar](max) NULL,
	[CorrectAnswer] [nvarchar](255) NOT NULL,
	[ScoreWeight] [float] NOT NULL,
	[Order] [int] NULL,
 CONSTRAINT [PK_questions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[users]    Script Date: 2024-12-19 9:30:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[users](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Role] [nvarchar](50) NOT NULL,
	[CreatedAt] [datetime2](7) NULL,
	[UserName] [nvarchar](256) NULL,
	[NormalizedUserName] [nvarchar](256) NULL,
	[Email] [nvarchar](255) NOT NULL,
	[NormalizedEmail] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](255) NOT NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEnd] [datetimeoffset](7) NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[ProfilePictureUrl] [nvarchar](max) NOT NULL,
	[LastActivity] [datetime2](7) NULL,
 CONSTRAINT [PK_users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Index [IX_AspNetRoleClaims_RoleId]    Script Date: 2024-12-19 9:30:57 PM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetRoleClaims_RoleId] ON [dbo].[AspNetRoleClaims]
(
	[RoleId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [RoleNameIndex]    Script Date: 2024-12-19 9:30:57 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex] ON [dbo].[AspNetRoles]
(
	[NormalizedName] ASC
)
WHERE ([NormalizedName] IS NOT NULL)
WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AspNetUserClaims_UserId]    Script Date: 2024-12-19 9:30:57 PM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserClaims_UserId] ON [dbo].[AspNetUserClaims]
(
	[UserId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AspNetUserLogins_UserId]    Script Date: 2024-12-19 9:30:57 PM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserLogins_UserId] ON [dbo].[AspNetUserLogins]
(
	[UserId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AspNetUserRoles_RoleId]    Script Date: 2024-12-19 9:30:57 PM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserRoles_RoleId] ON [dbo].[AspNetUserRoles]
(
	[RoleId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_completed_exams_ExamId]    Script Date: 2024-12-19 9:30:57 PM ******/
CREATE NONCLUSTERED INDEX [IX_completed_exams_ExamId] ON [dbo].[completed_exams]
(
	[ExamId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_completed_exams_UserId]    Script Date: 2024-12-19 9:30:57 PM ******/
CREATE NONCLUSTERED INDEX [IX_completed_exams_UserId] ON [dbo].[completed_exams]
(
	[UserId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_exam_attempt_ExamId]    Script Date: 2024-12-19 9:30:57 PM ******/
CREATE NONCLUSTERED INDEX [IX_exam_attempt_ExamId] ON [dbo].[exam_attempt]
(
	[ExamId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_exam_attempt_QuestionId]    Script Date: 2024-12-19 9:30:57 PM ******/
CREATE NONCLUSTERED INDEX [IX_exam_attempt_QuestionId] ON [dbo].[exam_attempt]
(
	[QuestionId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_exam_attempt_UserId]    Script Date: 2024-12-19 9:30:57 PM ******/
CREATE NONCLUSTERED INDEX [IX_exam_attempt_UserId] ON [dbo].[exam_attempt]
(
	[UserId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_exam_progress_ExamId_UserId_IsCompleted_IsActive]    Script Date: 2024-12-19 9:30:57 PM ******/
CREATE NONCLUSTERED INDEX [IX_exam_progress_ExamId_UserId_IsCompleted_IsActive] ON [dbo].[exam_progress]
(
	[ExamId] ASC,
	[UserId] ASC,
	[IsCompleted] ASC,
	[IsActive] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_exam_progress_UserId]    Script Date: 2024-12-19 9:30:57 PM ******/
CREATE NONCLUSTERED INDEX [IX_exam_progress_UserId] ON [dbo].[exam_progress]
(
	[UserId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_exam_student_assignments_AssignedStudentsId]    Script Date: 2024-12-19 9:30:57 PM ******/
CREATE NONCLUSTERED INDEX [IX_exam_student_assignments_AssignedStudentsId] ON [dbo].[exam_student_assignments]
(
	[AssignedStudentsId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_exams_TeacherId]    Script Date: 2024-12-19 9:30:57 PM ******/
CREATE NONCLUSTERED INDEX [IX_exams_TeacherId] ON [dbo].[exams]
(
	[TeacherId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_questions_ExamId]    Script Date: 2024-12-19 9:30:57 PM ******/
CREATE NONCLUSTERED INDEX [IX_questions_ExamId] ON [dbo].[questions]
(
	[ExamId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [EmailIndex]    Script Date: 2024-12-19 9:30:57 PM ******/
CREATE NONCLUSTERED INDEX [EmailIndex] ON [dbo].[users]
(
	[NormalizedEmail] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_users_Email]    Script Date: 2024-12-19 9:30:57 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_users_Email] ON [dbo].[users]
(
	[Email] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UserNameIndex]    Script Date: 2024-12-19 9:30:57 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [dbo].[users]
(
	[NormalizedUserName] ASC
)
WHERE ([NormalizedUserName] IS NOT NULL)
WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[completed_exams] ADD  DEFAULT (CONVERT([bit],(0))) FOR [IsCompleted]
GO
ALTER TABLE [dbo].[completed_exams] ADD  DEFAULT ((0.0000000000000000e+000)) FOR [TotalScore]
GO
ALTER TABLE [dbo].[completed_exams] ADD  DEFAULT ('0001-01-01T00:00:00.0000000') FOR [CompletedAt]
GO
ALTER TABLE [dbo].[completed_exams] ADD  DEFAULT (CONVERT([bit],(0))) FOR [IsSubmitted]
GO
ALTER TABLE [dbo].[exam_attempt] ADD  DEFAULT (CONVERT([bit],(0))) FOR [IsGraded]
GO
ALTER TABLE [dbo].[exam_attempt] ADD  DEFAULT (getdate()) FOR [SubmittedAt]
GO
ALTER TABLE [dbo].[exam_attempt] ADD  DEFAULT ((0)) FOR [ExamId]
GO
ALTER TABLE [dbo].[exam_attempt] ADD  DEFAULT (CONVERT([bit],(0))) FOR [IsCorrect]
GO
ALTER TABLE [dbo].[exam_progress] ADD  DEFAULT (CONVERT([bit],(0))) FOR [IsCompleted]
GO
ALTER TABLE [dbo].[exam_progress] ADD  DEFAULT (CONVERT([bit],(1))) FOR [IsActive]
GO
ALTER TABLE [dbo].[exams] ADD  DEFAULT (N'') FOR [Subject]
GO
ALTER TABLE [dbo].[exams] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[exams] ADD  DEFAULT ((60)) FOR [Duration]
GO
ALTER TABLE [dbo].[exams] ADD  DEFAULT (CONVERT([bit],(0))) FOR [HasStarted]
GO
ALTER TABLE [dbo].[exams] ADD  DEFAULT (CONVERT([bit],(0))) FOR [IsClosed]
GO
ALTER TABLE [dbo].[exams] ADD  DEFAULT (CONVERT([bit],(0))) FOR [IsSubmitted]
GO
ALTER TABLE [dbo].[users] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[users] ADD  DEFAULT (N'') FOR [ProfilePictureUrl]
GO
ALTER TABLE [dbo].[AspNetRoleClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetRoleClaims] CHECK CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserClaims_users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_AspNetUserClaims_users_UserId]
GO
ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserLogins_users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_AspNetUserLogins_users_UserId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_users_UserId]
GO
ALTER TABLE [dbo].[AspNetUserTokens]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserTokens_users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserTokens] CHECK CONSTRAINT [FK_AspNetUserTokens_users_UserId]
GO
ALTER TABLE [dbo].[completed_exams]  WITH CHECK ADD  CONSTRAINT [FK_completed_exams_exams_ExamId] FOREIGN KEY([ExamId])
REFERENCES [dbo].[exams] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[completed_exams] CHECK CONSTRAINT [FK_completed_exams_exams_ExamId]
GO
ALTER TABLE [dbo].[completed_exams]  WITH CHECK ADD  CONSTRAINT [FK_completed_exams_users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[users] ([Id])
GO
ALTER TABLE [dbo].[completed_exams] CHECK CONSTRAINT [FK_completed_exams_users_UserId]
GO
ALTER TABLE [dbo].[exam_attempt]  WITH CHECK ADD  CONSTRAINT [FK_exam_attempt_exams_ExamId] FOREIGN KEY([ExamId])
REFERENCES [dbo].[exams] ([Id])
GO
ALTER TABLE [dbo].[exam_attempt] CHECK CONSTRAINT [FK_exam_attempt_exams_ExamId]
GO
ALTER TABLE [dbo].[exam_attempt]  WITH CHECK ADD  CONSTRAINT [FK_exam_attempt_questions_QuestionId] FOREIGN KEY([QuestionId])
REFERENCES [dbo].[questions] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[exam_attempt] CHECK CONSTRAINT [FK_exam_attempt_questions_QuestionId]
GO
ALTER TABLE [dbo].[exam_attempt]  WITH CHECK ADD  CONSTRAINT [FK_exam_attempt_users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[users] ([Id])
GO
ALTER TABLE [dbo].[exam_attempt] CHECK CONSTRAINT [FK_exam_attempt_users_UserId]
GO
ALTER TABLE [dbo].[exam_progress]  WITH CHECK ADD  CONSTRAINT [FK_exam_progress_exams_ExamId] FOREIGN KEY([ExamId])
REFERENCES [dbo].[exams] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[exam_progress] CHECK CONSTRAINT [FK_exam_progress_exams_ExamId]
GO
ALTER TABLE [dbo].[exam_progress]  WITH CHECK ADD  CONSTRAINT [FK_exam_progress_users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[users] ([Id])
GO
ALTER TABLE [dbo].[exam_progress] CHECK CONSTRAINT [FK_exam_progress_users_UserId]
GO
ALTER TABLE [dbo].[exam_student_assignments]  WITH CHECK ADD  CONSTRAINT [FK_exam_student_assignments_exams_AssignedExamsId] FOREIGN KEY([AssignedExamsId])
REFERENCES [dbo].[exams] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[exam_student_assignments] CHECK CONSTRAINT [FK_exam_student_assignments_exams_AssignedExamsId]
GO
ALTER TABLE [dbo].[exam_student_assignments]  WITH CHECK ADD  CONSTRAINT [FK_exam_student_assignments_users_AssignedStudentsId] FOREIGN KEY([AssignedStudentsId])
REFERENCES [dbo].[users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[exam_student_assignments] CHECK CONSTRAINT [FK_exam_student_assignments_users_AssignedStudentsId]
GO
ALTER TABLE [dbo].[exams]  WITH CHECK ADD  CONSTRAINT [FK_exams_users_TeacherId] FOREIGN KEY([TeacherId])
REFERENCES [dbo].[users] ([Id])
GO
ALTER TABLE [dbo].[exams] CHECK CONSTRAINT [FK_exams_users_TeacherId]
GO
ALTER TABLE [dbo].[questions]  WITH CHECK ADD  CONSTRAINT [FK_questions_exams_ExamId] FOREIGN KEY([ExamId])
REFERENCES [dbo].[exams] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[questions] CHECK CONSTRAINT [FK_questions_exams_ExamId]
GO
ALTER DATABASE [examina] SET  READ_WRITE 
GO
