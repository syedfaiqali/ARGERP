-- ARGERP HR standalone schema. Generated from the checked-in EF models.
-- Run against ARGERP_HR only. Existing EDXS databases are never used.
IF OBJECT_ID(N'[WF_Actions]', N'U') IS NULL BEGIN CREATE TABLE [WF_Actions] (
    [ScreenKey] nvarchar(100) NOT NULL,
    [Service] nvarchar(max) NOT NULL,
    [RegistrationJson] nvarchar(max) NOT NULL,
    [RegisteredAtUtc] datetime2 NOT NULL,
    [utdoctype] int NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] nvarchar(25) NULL,
    [utaddsystem] nvarchar(25) NULL,
    [utaddip] nvarchar(25) NULL,
    [utaddappversion] nvarchar(10) NULL,
    [utadddbversion] nvarchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddsessionid] int NULL,
    [utedittime] datetime2 NULL,
    [utedituser] nvarchar(25) NULL,
    [uteditsystem] nvarchar(25) NULL,
    [uteditip] nvarchar(25) NULL,
    [uteditappversion] nvarchar(10) NULL,
    [uteditdbversion] nvarchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditsessionid] int NULL,
    [utrecordversion] int NOT NULL,
    CONSTRAINT [PK_WF_Actions] PRIMARY KEY ([ScreenKey])
); END;
GO
IF COL_LENGTH(N'[WF_Actions]', N'ScreenKey') IS NULL ALTER TABLE [WF_Actions] ADD [ScreenKey] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[WF_Actions]', N'Service') IS NULL ALTER TABLE [WF_Actions] ADD [Service] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[WF_Actions]', N'RegistrationJson') IS NULL ALTER TABLE [WF_Actions] ADD [RegistrationJson] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[WF_Actions]', N'RegisteredAtUtc') IS NULL ALTER TABLE [WF_Actions] ADD [RegisteredAtUtc] datetime2 NULL;
GO
IF COL_LENGTH(N'[WF_Actions]', N'utdoctype') IS NULL ALTER TABLE [WF_Actions] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[WF_Actions]', N'utsubdoctype') IS NULL ALTER TABLE [WF_Actions] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[WF_Actions]', N'utsysgen') IS NULL ALTER TABLE [WF_Actions] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[WF_Actions]', N'utblock') IS NULL ALTER TABLE [WF_Actions] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[WF_Actions]', N'utispartial') IS NULL ALTER TABLE [WF_Actions] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[WF_Actions]', N'utaddtime') IS NULL ALTER TABLE [WF_Actions] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[WF_Actions]', N'utadduser') IS NULL ALTER TABLE [WF_Actions] ADD [utadduser] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Actions]', N'utaddsystem') IS NULL ALTER TABLE [WF_Actions] ADD [utaddsystem] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Actions]', N'utaddip') IS NULL ALTER TABLE [WF_Actions] ADD [utaddip] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Actions]', N'utaddappversion') IS NULL ALTER TABLE [WF_Actions] ADD [utaddappversion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_Actions]', N'utadddbversion') IS NULL ALTER TABLE [WF_Actions] ADD [utadddbversion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_Actions]', N'utaddid') IS NULL ALTER TABLE [WF_Actions] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[WF_Actions]', N'utaddsessionid') IS NULL ALTER TABLE [WF_Actions] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[WF_Actions]', N'utedittime') IS NULL ALTER TABLE [WF_Actions] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[WF_Actions]', N'utedituser') IS NULL ALTER TABLE [WF_Actions] ADD [utedituser] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Actions]', N'uteditsystem') IS NULL ALTER TABLE [WF_Actions] ADD [uteditsystem] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Actions]', N'uteditip') IS NULL ALTER TABLE [WF_Actions] ADD [uteditip] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Actions]', N'uteditappversion') IS NULL ALTER TABLE [WF_Actions] ADD [uteditappversion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_Actions]', N'uteditdbversion') IS NULL ALTER TABLE [WF_Actions] ADD [uteditdbversion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_Actions]', N'uteditid') IS NULL ALTER TABLE [WF_Actions] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[WF_Actions]', N'uteditsessionid') IS NULL ALTER TABLE [WF_Actions] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[WF_Actions]', N'utrecordversion') IS NULL ALTER TABLE [WF_Actions] ADD [utrecordversion] int NULL;
GO
IF OBJECT_ID(N'[WF_Definitions]', N'U') IS NULL BEGIN CREATE TABLE [WF_Definitions] (
    [Id] uniqueidentifier NOT NULL,
    [ClientId] int NOT NULL,
    [ScreenKey] nvarchar(100) NOT NULL,
    [ActionKey] nvarchar(10) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [StepsJson] nvarchar(max) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [PreviousVersionId] uniqueidentifier NULL,
    [Version] int NOT NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    [CreatedBy] int NOT NULL,
    [utdoctype] int NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] nvarchar(25) NULL,
    [utaddsystem] nvarchar(25) NULL,
    [utaddip] nvarchar(25) NULL,
    [utaddappversion] nvarchar(10) NULL,
    [utadddbversion] nvarchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddsessionid] int NULL,
    [utedittime] datetime2 NULL,
    [utedituser] nvarchar(25) NULL,
    [uteditsystem] nvarchar(25) NULL,
    [uteditip] nvarchar(25) NULL,
    [uteditappversion] nvarchar(10) NULL,
    [uteditdbversion] nvarchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditsessionid] int NULL,
    [utrecordversion] int NOT NULL,
    CONSTRAINT [PK_WF_Definitions] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[WF_Definitions]', N'Id') IS NULL ALTER TABLE [WF_Definitions] ADD [Id] uniqueidentifier NULL;
GO
IF COL_LENGTH(N'[WF_Definitions]', N'ClientId') IS NULL ALTER TABLE [WF_Definitions] ADD [ClientId] int NULL;
GO
IF COL_LENGTH(N'[WF_Definitions]', N'ScreenKey') IS NULL ALTER TABLE [WF_Definitions] ADD [ScreenKey] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[WF_Definitions]', N'ActionKey') IS NULL ALTER TABLE [WF_Definitions] ADD [ActionKey] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_Definitions]', N'Name') IS NULL ALTER TABLE [WF_Definitions] ADD [Name] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[WF_Definitions]', N'StepsJson') IS NULL ALTER TABLE [WF_Definitions] ADD [StepsJson] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[WF_Definitions]', N'Status') IS NULL ALTER TABLE [WF_Definitions] ADD [Status] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[WF_Definitions]', N'PreviousVersionId') IS NULL ALTER TABLE [WF_Definitions] ADD [PreviousVersionId] uniqueidentifier NULL;
GO
IF COL_LENGTH(N'[WF_Definitions]', N'Version') IS NULL ALTER TABLE [WF_Definitions] ADD [Version] int NULL;
GO
IF COL_LENGTH(N'[WF_Definitions]', N'CreatedAtUtc') IS NULL ALTER TABLE [WF_Definitions] ADD [CreatedAtUtc] datetime2 NULL;
GO
IF COL_LENGTH(N'[WF_Definitions]', N'CreatedBy') IS NULL ALTER TABLE [WF_Definitions] ADD [CreatedBy] int NULL;
GO
IF COL_LENGTH(N'[WF_Definitions]', N'utdoctype') IS NULL ALTER TABLE [WF_Definitions] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[WF_Definitions]', N'utsubdoctype') IS NULL ALTER TABLE [WF_Definitions] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[WF_Definitions]', N'utsysgen') IS NULL ALTER TABLE [WF_Definitions] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[WF_Definitions]', N'utblock') IS NULL ALTER TABLE [WF_Definitions] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[WF_Definitions]', N'utispartial') IS NULL ALTER TABLE [WF_Definitions] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[WF_Definitions]', N'utaddtime') IS NULL ALTER TABLE [WF_Definitions] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[WF_Definitions]', N'utadduser') IS NULL ALTER TABLE [WF_Definitions] ADD [utadduser] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Definitions]', N'utaddsystem') IS NULL ALTER TABLE [WF_Definitions] ADD [utaddsystem] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Definitions]', N'utaddip') IS NULL ALTER TABLE [WF_Definitions] ADD [utaddip] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Definitions]', N'utaddappversion') IS NULL ALTER TABLE [WF_Definitions] ADD [utaddappversion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_Definitions]', N'utadddbversion') IS NULL ALTER TABLE [WF_Definitions] ADD [utadddbversion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_Definitions]', N'utaddid') IS NULL ALTER TABLE [WF_Definitions] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[WF_Definitions]', N'utaddsessionid') IS NULL ALTER TABLE [WF_Definitions] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[WF_Definitions]', N'utedittime') IS NULL ALTER TABLE [WF_Definitions] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[WF_Definitions]', N'utedituser') IS NULL ALTER TABLE [WF_Definitions] ADD [utedituser] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Definitions]', N'uteditsystem') IS NULL ALTER TABLE [WF_Definitions] ADD [uteditsystem] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Definitions]', N'uteditip') IS NULL ALTER TABLE [WF_Definitions] ADD [uteditip] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Definitions]', N'uteditappversion') IS NULL ALTER TABLE [WF_Definitions] ADD [uteditappversion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_Definitions]', N'uteditdbversion') IS NULL ALTER TABLE [WF_Definitions] ADD [uteditdbversion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_Definitions]', N'uteditid') IS NULL ALTER TABLE [WF_Definitions] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[WF_Definitions]', N'uteditsessionid') IS NULL ALTER TABLE [WF_Definitions] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[WF_Definitions]', N'utrecordversion') IS NULL ALTER TABLE [WF_Definitions] ADD [utrecordversion] int NULL;
GO
IF OBJECT_ID(N'[WF_Events]', N'U') IS NULL BEGIN CREATE TABLE [WF_Events] (
    [Id] bigint NOT NULL IDENTITY,
    [InstanceId] uniqueidentifier NULL,
    [ClientId] int NOT NULL,
    [UserId] int NOT NULL,
    [Kind] nvarchar(max) NOT NULL,
    [Comment] nvarchar(max) NOT NULL,
    [IdempotencyKey] uniqueidentifier NULL,
    [RequestHash] nvarchar(max) NULL,
    [AtUtc] datetime2 NOT NULL,
    [utdoctype] int NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] nvarchar(25) NULL,
    [utaddsystem] nvarchar(25) NULL,
    [utaddip] nvarchar(25) NULL,
    [utaddappversion] nvarchar(10) NULL,
    [utadddbversion] nvarchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddsessionid] int NULL,
    [utedittime] datetime2 NULL,
    [utedituser] nvarchar(25) NULL,
    [uteditsystem] nvarchar(25) NULL,
    [uteditip] nvarchar(25) NULL,
    [uteditappversion] nvarchar(10) NULL,
    [uteditdbversion] nvarchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditsessionid] int NULL,
    [utrecordversion] int NOT NULL,
    CONSTRAINT [PK_WF_Events] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[WF_Events]', N'Id') IS NULL ALTER TABLE [WF_Events] ADD [Id] bigint NULL ;
GO
IF COL_LENGTH(N'[WF_Events]', N'InstanceId') IS NULL ALTER TABLE [WF_Events] ADD [InstanceId] uniqueidentifier NULL;
GO
IF COL_LENGTH(N'[WF_Events]', N'ClientId') IS NULL ALTER TABLE [WF_Events] ADD [ClientId] int NULL;
GO
IF COL_LENGTH(N'[WF_Events]', N'UserId') IS NULL ALTER TABLE [WF_Events] ADD [UserId] int NULL;
GO
IF COL_LENGTH(N'[WF_Events]', N'Kind') IS NULL ALTER TABLE [WF_Events] ADD [Kind] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[WF_Events]', N'Comment') IS NULL ALTER TABLE [WF_Events] ADD [Comment] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[WF_Events]', N'IdempotencyKey') IS NULL ALTER TABLE [WF_Events] ADD [IdempotencyKey] uniqueidentifier NULL;
GO
IF COL_LENGTH(N'[WF_Events]', N'RequestHash') IS NULL ALTER TABLE [WF_Events] ADD [RequestHash] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[WF_Events]', N'AtUtc') IS NULL ALTER TABLE [WF_Events] ADD [AtUtc] datetime2 NULL;
GO
IF COL_LENGTH(N'[WF_Events]', N'utdoctype') IS NULL ALTER TABLE [WF_Events] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[WF_Events]', N'utsubdoctype') IS NULL ALTER TABLE [WF_Events] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[WF_Events]', N'utsysgen') IS NULL ALTER TABLE [WF_Events] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[WF_Events]', N'utblock') IS NULL ALTER TABLE [WF_Events] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[WF_Events]', N'utispartial') IS NULL ALTER TABLE [WF_Events] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[WF_Events]', N'utaddtime') IS NULL ALTER TABLE [WF_Events] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[WF_Events]', N'utadduser') IS NULL ALTER TABLE [WF_Events] ADD [utadduser] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Events]', N'utaddsystem') IS NULL ALTER TABLE [WF_Events] ADD [utaddsystem] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Events]', N'utaddip') IS NULL ALTER TABLE [WF_Events] ADD [utaddip] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Events]', N'utaddappversion') IS NULL ALTER TABLE [WF_Events] ADD [utaddappversion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_Events]', N'utadddbversion') IS NULL ALTER TABLE [WF_Events] ADD [utadddbversion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_Events]', N'utaddid') IS NULL ALTER TABLE [WF_Events] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[WF_Events]', N'utaddsessionid') IS NULL ALTER TABLE [WF_Events] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[WF_Events]', N'utedittime') IS NULL ALTER TABLE [WF_Events] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[WF_Events]', N'utedituser') IS NULL ALTER TABLE [WF_Events] ADD [utedituser] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Events]', N'uteditsystem') IS NULL ALTER TABLE [WF_Events] ADD [uteditsystem] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Events]', N'uteditip') IS NULL ALTER TABLE [WF_Events] ADD [uteditip] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Events]', N'uteditappversion') IS NULL ALTER TABLE [WF_Events] ADD [uteditappversion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_Events]', N'uteditdbversion') IS NULL ALTER TABLE [WF_Events] ADD [uteditdbversion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_Events]', N'uteditid') IS NULL ALTER TABLE [WF_Events] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[WF_Events]', N'uteditsessionid') IS NULL ALTER TABLE [WF_Events] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[WF_Events]', N'utrecordversion') IS NULL ALTER TABLE [WF_Events] ADD [utrecordversion] int NULL;
GO
IF OBJECT_ID(N'[WF_GroupBindings]', N'U') IS NULL BEGIN CREATE TABLE [WF_GroupBindings] (
    [ClientId] int NOT NULL,
    [GroupKey] nvarchar(100) NOT NULL,
    [SelectorsJson] nvarchar(max) NOT NULL,
    [Version] int NOT NULL,
    [utdoctype] int NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] nvarchar(25) NULL,
    [utaddsystem] nvarchar(25) NULL,
    [utaddip] nvarchar(25) NULL,
    [utaddappversion] nvarchar(10) NULL,
    [utadddbversion] nvarchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddsessionid] int NULL,
    [utedittime] datetime2 NULL,
    [utedituser] nvarchar(25) NULL,
    [uteditsystem] nvarchar(25) NULL,
    [uteditip] nvarchar(25) NULL,
    [uteditappversion] nvarchar(10) NULL,
    [uteditdbversion] nvarchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditsessionid] int NULL,
    [utrecordversion] int NOT NULL,
    CONSTRAINT [PK_WF_GroupBindings] PRIMARY KEY ([ClientId], [GroupKey])
); END;
GO
IF COL_LENGTH(N'[WF_GroupBindings]', N'ClientId') IS NULL ALTER TABLE [WF_GroupBindings] ADD [ClientId] int NULL;
GO
IF COL_LENGTH(N'[WF_GroupBindings]', N'GroupKey') IS NULL ALTER TABLE [WF_GroupBindings] ADD [GroupKey] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[WF_GroupBindings]', N'SelectorsJson') IS NULL ALTER TABLE [WF_GroupBindings] ADD [SelectorsJson] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[WF_GroupBindings]', N'Version') IS NULL ALTER TABLE [WF_GroupBindings] ADD [Version] int NULL;
GO
IF COL_LENGTH(N'[WF_GroupBindings]', N'utdoctype') IS NULL ALTER TABLE [WF_GroupBindings] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[WF_GroupBindings]', N'utsubdoctype') IS NULL ALTER TABLE [WF_GroupBindings] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[WF_GroupBindings]', N'utsysgen') IS NULL ALTER TABLE [WF_GroupBindings] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[WF_GroupBindings]', N'utblock') IS NULL ALTER TABLE [WF_GroupBindings] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[WF_GroupBindings]', N'utispartial') IS NULL ALTER TABLE [WF_GroupBindings] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[WF_GroupBindings]', N'utaddtime') IS NULL ALTER TABLE [WF_GroupBindings] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[WF_GroupBindings]', N'utadduser') IS NULL ALTER TABLE [WF_GroupBindings] ADD [utadduser] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_GroupBindings]', N'utaddsystem') IS NULL ALTER TABLE [WF_GroupBindings] ADD [utaddsystem] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_GroupBindings]', N'utaddip') IS NULL ALTER TABLE [WF_GroupBindings] ADD [utaddip] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_GroupBindings]', N'utaddappversion') IS NULL ALTER TABLE [WF_GroupBindings] ADD [utaddappversion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_GroupBindings]', N'utadddbversion') IS NULL ALTER TABLE [WF_GroupBindings] ADD [utadddbversion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_GroupBindings]', N'utaddid') IS NULL ALTER TABLE [WF_GroupBindings] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[WF_GroupBindings]', N'utaddsessionid') IS NULL ALTER TABLE [WF_GroupBindings] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[WF_GroupBindings]', N'utedittime') IS NULL ALTER TABLE [WF_GroupBindings] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[WF_GroupBindings]', N'utedituser') IS NULL ALTER TABLE [WF_GroupBindings] ADD [utedituser] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_GroupBindings]', N'uteditsystem') IS NULL ALTER TABLE [WF_GroupBindings] ADD [uteditsystem] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_GroupBindings]', N'uteditip') IS NULL ALTER TABLE [WF_GroupBindings] ADD [uteditip] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_GroupBindings]', N'uteditappversion') IS NULL ALTER TABLE [WF_GroupBindings] ADD [uteditappversion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_GroupBindings]', N'uteditdbversion') IS NULL ALTER TABLE [WF_GroupBindings] ADD [uteditdbversion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_GroupBindings]', N'uteditid') IS NULL ALTER TABLE [WF_GroupBindings] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[WF_GroupBindings]', N'uteditsessionid') IS NULL ALTER TABLE [WF_GroupBindings] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[WF_GroupBindings]', N'utrecordversion') IS NULL ALTER TABLE [WF_GroupBindings] ADD [utrecordversion] int NULL;
GO
IF OBJECT_ID(N'[WF_Groups]', N'U') IS NULL BEGIN CREATE TABLE [WF_Groups] (
    [Key] nvarchar(100) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [utdoctype] int NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] nvarchar(25) NULL,
    [utaddsystem] nvarchar(25) NULL,
    [utaddip] nvarchar(25) NULL,
    [utaddappversion] nvarchar(10) NULL,
    [utadddbversion] nvarchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddsessionid] int NULL,
    [utedittime] datetime2 NULL,
    [utedituser] nvarchar(25) NULL,
    [uteditsystem] nvarchar(25) NULL,
    [uteditip] nvarchar(25) NULL,
    [uteditappversion] nvarchar(10) NULL,
    [uteditdbversion] nvarchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditsessionid] int NULL,
    [utrecordversion] int NOT NULL,
    CONSTRAINT [PK_WF_Groups] PRIMARY KEY ([Key])
); END;
GO
IF COL_LENGTH(N'[WF_Groups]', N'Key') IS NULL ALTER TABLE [WF_Groups] ADD [Key] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[WF_Groups]', N'Name') IS NULL ALTER TABLE [WF_Groups] ADD [Name] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[WF_Groups]', N'utdoctype') IS NULL ALTER TABLE [WF_Groups] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[WF_Groups]', N'utsubdoctype') IS NULL ALTER TABLE [WF_Groups] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[WF_Groups]', N'utsysgen') IS NULL ALTER TABLE [WF_Groups] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[WF_Groups]', N'utblock') IS NULL ALTER TABLE [WF_Groups] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[WF_Groups]', N'utispartial') IS NULL ALTER TABLE [WF_Groups] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[WF_Groups]', N'utaddtime') IS NULL ALTER TABLE [WF_Groups] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[WF_Groups]', N'utadduser') IS NULL ALTER TABLE [WF_Groups] ADD [utadduser] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Groups]', N'utaddsystem') IS NULL ALTER TABLE [WF_Groups] ADD [utaddsystem] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Groups]', N'utaddip') IS NULL ALTER TABLE [WF_Groups] ADD [utaddip] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Groups]', N'utaddappversion') IS NULL ALTER TABLE [WF_Groups] ADD [utaddappversion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_Groups]', N'utadddbversion') IS NULL ALTER TABLE [WF_Groups] ADD [utadddbversion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_Groups]', N'utaddid') IS NULL ALTER TABLE [WF_Groups] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[WF_Groups]', N'utaddsessionid') IS NULL ALTER TABLE [WF_Groups] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[WF_Groups]', N'utedittime') IS NULL ALTER TABLE [WF_Groups] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[WF_Groups]', N'utedituser') IS NULL ALTER TABLE [WF_Groups] ADD [utedituser] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Groups]', N'uteditsystem') IS NULL ALTER TABLE [WF_Groups] ADD [uteditsystem] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Groups]', N'uteditip') IS NULL ALTER TABLE [WF_Groups] ADD [uteditip] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Groups]', N'uteditappversion') IS NULL ALTER TABLE [WF_Groups] ADD [uteditappversion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_Groups]', N'uteditdbversion') IS NULL ALTER TABLE [WF_Groups] ADD [uteditdbversion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_Groups]', N'uteditid') IS NULL ALTER TABLE [WF_Groups] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[WF_Groups]', N'uteditsessionid') IS NULL ALTER TABLE [WF_Groups] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[WF_Groups]', N'utrecordversion') IS NULL ALTER TABLE [WF_Groups] ADD [utrecordversion] int NULL;
GO
IF OBJECT_ID(N'[WF_Instances]', N'U') IS NULL BEGIN CREATE TABLE [WF_Instances] (
    [Id] uniqueidentifier NOT NULL,
    [ClientId] int NOT NULL,
    [EntityId] int NULL,
    [SubmitterId] int NOT NULL,
    [SubmitterName] nvarchar(max) NOT NULL,
    [ScreenKey] nvarchar(100) NOT NULL,
    [Operation] nvarchar(max) NOT NULL,
    [Subject] nvarchar(max) NOT NULL,
    [RecordKey] nvarchar(max) NULL,
    [OpenKey] nvarchar(64) NULL,
    [IdempotencyKey] uniqueidentifier NOT NULL,
    [RequestHash] nvarchar(max) NOT NULL,
    [DefinitionId] uniqueidentifier NOT NULL,
    [DefinitionName] nvarchar(max) NOT NULL,
    [StepsJson] nvarchar(max) NOT NULL,
    [PayloadEnvelope] nvarchar(max) NOT NULL,
    [PayloadHash] nvarchar(max) NOT NULL,
    [Status] nvarchar(40) NOT NULL,
    [StepIndex] int NOT NULL,
    [Generation] int NOT NULL,
    [Version] int NOT NULL,
    [PreviousInstanceId] uniqueidentifier NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    [UpdatedAtUtc] datetime2 NOT NULL,
    [NextAttemptAtUtc] datetime2 NULL,
    [LeaseUntilUtc] datetime2 NULL,
    [ExecutionToken] uniqueidentifier NULL,
    [Attempts] int NOT NULL,
    [FailureCode] nvarchar(max) NULL,
    [utdoctype] int NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] nvarchar(25) NULL,
    [utaddsystem] nvarchar(25) NULL,
    [utaddip] nvarchar(25) NULL,
    [utaddappversion] nvarchar(10) NULL,
    [utadddbversion] nvarchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddsessionid] int NULL,
    [utedittime] datetime2 NULL,
    [utedituser] nvarchar(25) NULL,
    [uteditsystem] nvarchar(25) NULL,
    [uteditip] nvarchar(25) NULL,
    [uteditappversion] nvarchar(10) NULL,
    [uteditdbversion] nvarchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditsessionid] int NULL,
    [utrecordversion] int NOT NULL,
    CONSTRAINT [PK_WF_Instances] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[WF_Instances]', N'Id') IS NULL ALTER TABLE [WF_Instances] ADD [Id] uniqueidentifier NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'ClientId') IS NULL ALTER TABLE [WF_Instances] ADD [ClientId] int NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'EntityId') IS NULL ALTER TABLE [WF_Instances] ADD [EntityId] int NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'SubmitterId') IS NULL ALTER TABLE [WF_Instances] ADD [SubmitterId] int NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'SubmitterName') IS NULL ALTER TABLE [WF_Instances] ADD [SubmitterName] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'ScreenKey') IS NULL ALTER TABLE [WF_Instances] ADD [ScreenKey] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'Operation') IS NULL ALTER TABLE [WF_Instances] ADD [Operation] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'Subject') IS NULL ALTER TABLE [WF_Instances] ADD [Subject] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'RecordKey') IS NULL ALTER TABLE [WF_Instances] ADD [RecordKey] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'OpenKey') IS NULL ALTER TABLE [WF_Instances] ADD [OpenKey] nvarchar(64) NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'IdempotencyKey') IS NULL ALTER TABLE [WF_Instances] ADD [IdempotencyKey] uniqueidentifier NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'RequestHash') IS NULL ALTER TABLE [WF_Instances] ADD [RequestHash] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'DefinitionId') IS NULL ALTER TABLE [WF_Instances] ADD [DefinitionId] uniqueidentifier NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'DefinitionName') IS NULL ALTER TABLE [WF_Instances] ADD [DefinitionName] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'StepsJson') IS NULL ALTER TABLE [WF_Instances] ADD [StepsJson] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'PayloadEnvelope') IS NULL ALTER TABLE [WF_Instances] ADD [PayloadEnvelope] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'PayloadHash') IS NULL ALTER TABLE [WF_Instances] ADD [PayloadHash] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'Status') IS NULL ALTER TABLE [WF_Instances] ADD [Status] nvarchar(40) NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'StepIndex') IS NULL ALTER TABLE [WF_Instances] ADD [StepIndex] int NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'Generation') IS NULL ALTER TABLE [WF_Instances] ADD [Generation] int NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'Version') IS NULL ALTER TABLE [WF_Instances] ADD [Version] int NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'PreviousInstanceId') IS NULL ALTER TABLE [WF_Instances] ADD [PreviousInstanceId] uniqueidentifier NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'CreatedAtUtc') IS NULL ALTER TABLE [WF_Instances] ADD [CreatedAtUtc] datetime2 NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'UpdatedAtUtc') IS NULL ALTER TABLE [WF_Instances] ADD [UpdatedAtUtc] datetime2 NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'NextAttemptAtUtc') IS NULL ALTER TABLE [WF_Instances] ADD [NextAttemptAtUtc] datetime2 NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'LeaseUntilUtc') IS NULL ALTER TABLE [WF_Instances] ADD [LeaseUntilUtc] datetime2 NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'ExecutionToken') IS NULL ALTER TABLE [WF_Instances] ADD [ExecutionToken] uniqueidentifier NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'Attempts') IS NULL ALTER TABLE [WF_Instances] ADD [Attempts] int NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'FailureCode') IS NULL ALTER TABLE [WF_Instances] ADD [FailureCode] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'utdoctype') IS NULL ALTER TABLE [WF_Instances] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'utsubdoctype') IS NULL ALTER TABLE [WF_Instances] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'utsysgen') IS NULL ALTER TABLE [WF_Instances] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'utblock') IS NULL ALTER TABLE [WF_Instances] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'utispartial') IS NULL ALTER TABLE [WF_Instances] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'utaddtime') IS NULL ALTER TABLE [WF_Instances] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'utadduser') IS NULL ALTER TABLE [WF_Instances] ADD [utadduser] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'utaddsystem') IS NULL ALTER TABLE [WF_Instances] ADD [utaddsystem] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'utaddip') IS NULL ALTER TABLE [WF_Instances] ADD [utaddip] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'utaddappversion') IS NULL ALTER TABLE [WF_Instances] ADD [utaddappversion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'utadddbversion') IS NULL ALTER TABLE [WF_Instances] ADD [utadddbversion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'utaddid') IS NULL ALTER TABLE [WF_Instances] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'utaddsessionid') IS NULL ALTER TABLE [WF_Instances] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'utedittime') IS NULL ALTER TABLE [WF_Instances] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'utedituser') IS NULL ALTER TABLE [WF_Instances] ADD [utedituser] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'uteditsystem') IS NULL ALTER TABLE [WF_Instances] ADD [uteditsystem] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'uteditip') IS NULL ALTER TABLE [WF_Instances] ADD [uteditip] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'uteditappversion') IS NULL ALTER TABLE [WF_Instances] ADD [uteditappversion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'uteditdbversion') IS NULL ALTER TABLE [WF_Instances] ADD [uteditdbversion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'uteditid') IS NULL ALTER TABLE [WF_Instances] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'uteditsessionid') IS NULL ALTER TABLE [WF_Instances] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[WF_Instances]', N'utrecordversion') IS NULL ALTER TABLE [WF_Instances] ADD [utrecordversion] int NULL;
GO
IF OBJECT_ID(N'[WF_Notifications]', N'U') IS NULL BEGIN CREATE TABLE [WF_Notifications] (
    [Id] bigint NOT NULL IDENTITY,
    [InstanceId] uniqueidentifier NOT NULL,
    [ClientId] int NOT NULL,
    [UserId] int NOT NULL,
    [Kind] nvarchar(max) NOT NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    [ReadAtUtc] datetime2 NULL,
    [utdoctype] int NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] nvarchar(25) NULL,
    [utaddsystem] nvarchar(25) NULL,
    [utaddip] nvarchar(25) NULL,
    [utaddappversion] nvarchar(10) NULL,
    [utadddbversion] nvarchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddsessionid] int NULL,
    [utedittime] datetime2 NULL,
    [utedituser] nvarchar(25) NULL,
    [uteditsystem] nvarchar(25) NULL,
    [uteditip] nvarchar(25) NULL,
    [uteditappversion] nvarchar(10) NULL,
    [uteditdbversion] nvarchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditsessionid] int NULL,
    [utrecordversion] int NOT NULL,
    CONSTRAINT [PK_WF_Notifications] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[WF_Notifications]', N'Id') IS NULL ALTER TABLE [WF_Notifications] ADD [Id] bigint NULL ;
GO
IF COL_LENGTH(N'[WF_Notifications]', N'InstanceId') IS NULL ALTER TABLE [WF_Notifications] ADD [InstanceId] uniqueidentifier NULL;
GO
IF COL_LENGTH(N'[WF_Notifications]', N'ClientId') IS NULL ALTER TABLE [WF_Notifications] ADD [ClientId] int NULL;
GO
IF COL_LENGTH(N'[WF_Notifications]', N'UserId') IS NULL ALTER TABLE [WF_Notifications] ADD [UserId] int NULL;
GO
IF COL_LENGTH(N'[WF_Notifications]', N'Kind') IS NULL ALTER TABLE [WF_Notifications] ADD [Kind] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[WF_Notifications]', N'CreatedAtUtc') IS NULL ALTER TABLE [WF_Notifications] ADD [CreatedAtUtc] datetime2 NULL;
GO
IF COL_LENGTH(N'[WF_Notifications]', N'ReadAtUtc') IS NULL ALTER TABLE [WF_Notifications] ADD [ReadAtUtc] datetime2 NULL;
GO
IF COL_LENGTH(N'[WF_Notifications]', N'utdoctype') IS NULL ALTER TABLE [WF_Notifications] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[WF_Notifications]', N'utsubdoctype') IS NULL ALTER TABLE [WF_Notifications] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[WF_Notifications]', N'utsysgen') IS NULL ALTER TABLE [WF_Notifications] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[WF_Notifications]', N'utblock') IS NULL ALTER TABLE [WF_Notifications] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[WF_Notifications]', N'utispartial') IS NULL ALTER TABLE [WF_Notifications] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[WF_Notifications]', N'utaddtime') IS NULL ALTER TABLE [WF_Notifications] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[WF_Notifications]', N'utadduser') IS NULL ALTER TABLE [WF_Notifications] ADD [utadduser] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Notifications]', N'utaddsystem') IS NULL ALTER TABLE [WF_Notifications] ADD [utaddsystem] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Notifications]', N'utaddip') IS NULL ALTER TABLE [WF_Notifications] ADD [utaddip] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Notifications]', N'utaddappversion') IS NULL ALTER TABLE [WF_Notifications] ADD [utaddappversion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_Notifications]', N'utadddbversion') IS NULL ALTER TABLE [WF_Notifications] ADD [utadddbversion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_Notifications]', N'utaddid') IS NULL ALTER TABLE [WF_Notifications] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[WF_Notifications]', N'utaddsessionid') IS NULL ALTER TABLE [WF_Notifications] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[WF_Notifications]', N'utedittime') IS NULL ALTER TABLE [WF_Notifications] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[WF_Notifications]', N'utedituser') IS NULL ALTER TABLE [WF_Notifications] ADD [utedituser] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Notifications]', N'uteditsystem') IS NULL ALTER TABLE [WF_Notifications] ADD [uteditsystem] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Notifications]', N'uteditip') IS NULL ALTER TABLE [WF_Notifications] ADD [uteditip] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Notifications]', N'uteditappversion') IS NULL ALTER TABLE [WF_Notifications] ADD [uteditappversion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_Notifications]', N'uteditdbversion') IS NULL ALTER TABLE [WF_Notifications] ADD [uteditdbversion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_Notifications]', N'uteditid') IS NULL ALTER TABLE [WF_Notifications] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[WF_Notifications]', N'uteditsessionid') IS NULL ALTER TABLE [WF_Notifications] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[WF_Notifications]', N'utrecordversion') IS NULL ALTER TABLE [WF_Notifications] ADD [utrecordversion] int NULL;
GO
IF OBJECT_ID(N'[WF_Outbox]', N'U') IS NULL BEGIN CREATE TABLE [WF_Outbox] (
    [Id] bigint NOT NULL IDENTITY,
    [InstanceId] uniqueidentifier NOT NULL,
    [ClientId] int NOT NULL,
    [UserId] int NOT NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    [DeliveredAtUtc] datetime2 NULL,
    [utdoctype] int NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] nvarchar(25) NULL,
    [utaddsystem] nvarchar(25) NULL,
    [utaddip] nvarchar(25) NULL,
    [utaddappversion] nvarchar(10) NULL,
    [utadddbversion] nvarchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddsessionid] int NULL,
    [utedittime] datetime2 NULL,
    [utedituser] nvarchar(25) NULL,
    [uteditsystem] nvarchar(25) NULL,
    [uteditip] nvarchar(25) NULL,
    [uteditappversion] nvarchar(10) NULL,
    [uteditdbversion] nvarchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditsessionid] int NULL,
    [utrecordversion] int NOT NULL,
    CONSTRAINT [PK_WF_Outbox] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[WF_Outbox]', N'Id') IS NULL ALTER TABLE [WF_Outbox] ADD [Id] bigint NULL ;
GO
IF COL_LENGTH(N'[WF_Outbox]', N'InstanceId') IS NULL ALTER TABLE [WF_Outbox] ADD [InstanceId] uniqueidentifier NULL;
GO
IF COL_LENGTH(N'[WF_Outbox]', N'ClientId') IS NULL ALTER TABLE [WF_Outbox] ADD [ClientId] int NULL;
GO
IF COL_LENGTH(N'[WF_Outbox]', N'UserId') IS NULL ALTER TABLE [WF_Outbox] ADD [UserId] int NULL;
GO
IF COL_LENGTH(N'[WF_Outbox]', N'CreatedAtUtc') IS NULL ALTER TABLE [WF_Outbox] ADD [CreatedAtUtc] datetime2 NULL;
GO
IF COL_LENGTH(N'[WF_Outbox]', N'DeliveredAtUtc') IS NULL ALTER TABLE [WF_Outbox] ADD [DeliveredAtUtc] datetime2 NULL;
GO
IF COL_LENGTH(N'[WF_Outbox]', N'utdoctype') IS NULL ALTER TABLE [WF_Outbox] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[WF_Outbox]', N'utsubdoctype') IS NULL ALTER TABLE [WF_Outbox] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[WF_Outbox]', N'utsysgen') IS NULL ALTER TABLE [WF_Outbox] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[WF_Outbox]', N'utblock') IS NULL ALTER TABLE [WF_Outbox] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[WF_Outbox]', N'utispartial') IS NULL ALTER TABLE [WF_Outbox] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[WF_Outbox]', N'utaddtime') IS NULL ALTER TABLE [WF_Outbox] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[WF_Outbox]', N'utadduser') IS NULL ALTER TABLE [WF_Outbox] ADD [utadduser] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Outbox]', N'utaddsystem') IS NULL ALTER TABLE [WF_Outbox] ADD [utaddsystem] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Outbox]', N'utaddip') IS NULL ALTER TABLE [WF_Outbox] ADD [utaddip] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Outbox]', N'utaddappversion') IS NULL ALTER TABLE [WF_Outbox] ADD [utaddappversion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_Outbox]', N'utadddbversion') IS NULL ALTER TABLE [WF_Outbox] ADD [utadddbversion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_Outbox]', N'utaddid') IS NULL ALTER TABLE [WF_Outbox] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[WF_Outbox]', N'utaddsessionid') IS NULL ALTER TABLE [WF_Outbox] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[WF_Outbox]', N'utedittime') IS NULL ALTER TABLE [WF_Outbox] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[WF_Outbox]', N'utedituser') IS NULL ALTER TABLE [WF_Outbox] ADD [utedituser] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Outbox]', N'uteditsystem') IS NULL ALTER TABLE [WF_Outbox] ADD [uteditsystem] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Outbox]', N'uteditip') IS NULL ALTER TABLE [WF_Outbox] ADD [uteditip] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Outbox]', N'uteditappversion') IS NULL ALTER TABLE [WF_Outbox] ADD [uteditappversion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_Outbox]', N'uteditdbversion') IS NULL ALTER TABLE [WF_Outbox] ADD [uteditdbversion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_Outbox]', N'uteditid') IS NULL ALTER TABLE [WF_Outbox] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[WF_Outbox]', N'uteditsessionid') IS NULL ALTER TABLE [WF_Outbox] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[WF_Outbox]', N'utrecordversion') IS NULL ALTER TABLE [WF_Outbox] ADD [utrecordversion] int NULL;
GO
IF OBJECT_ID(N'[WF_Policies]', N'U') IS NULL BEGIN CREATE TABLE [WF_Policies] (
    [ClientId] int NOT NULL,
    [ScreenKey] nvarchar(100) NOT NULL,
    [ActionKey] nvarchar(10) NOT NULL,
    [Mode] nvarchar(max) NOT NULL,
    [DefinitionId] uniqueidentifier NULL,
    [Version] int NOT NULL,
    [utdoctype] int NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] nvarchar(25) NULL,
    [utaddsystem] nvarchar(25) NULL,
    [utaddip] nvarchar(25) NULL,
    [utaddappversion] nvarchar(10) NULL,
    [utadddbversion] nvarchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddsessionid] int NULL,
    [utedittime] datetime2 NULL,
    [utedituser] nvarchar(25) NULL,
    [uteditsystem] nvarchar(25) NULL,
    [uteditip] nvarchar(25) NULL,
    [uteditappversion] nvarchar(10) NULL,
    [uteditdbversion] nvarchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditsessionid] int NULL,
    [utrecordversion] int NOT NULL,
    CONSTRAINT [PK_WF_Policies] PRIMARY KEY ([ClientId], [ScreenKey], [ActionKey])
); END;
GO
IF COL_LENGTH(N'[WF_Policies]', N'ClientId') IS NULL ALTER TABLE [WF_Policies] ADD [ClientId] int NULL;
GO
IF COL_LENGTH(N'[WF_Policies]', N'ScreenKey') IS NULL ALTER TABLE [WF_Policies] ADD [ScreenKey] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[WF_Policies]', N'ActionKey') IS NULL ALTER TABLE [WF_Policies] ADD [ActionKey] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_Policies]', N'Mode') IS NULL ALTER TABLE [WF_Policies] ADD [Mode] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[WF_Policies]', N'DefinitionId') IS NULL ALTER TABLE [WF_Policies] ADD [DefinitionId] uniqueidentifier NULL;
GO
IF COL_LENGTH(N'[WF_Policies]', N'Version') IS NULL ALTER TABLE [WF_Policies] ADD [Version] int NULL;
GO
IF COL_LENGTH(N'[WF_Policies]', N'utdoctype') IS NULL ALTER TABLE [WF_Policies] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[WF_Policies]', N'utsubdoctype') IS NULL ALTER TABLE [WF_Policies] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[WF_Policies]', N'utsysgen') IS NULL ALTER TABLE [WF_Policies] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[WF_Policies]', N'utblock') IS NULL ALTER TABLE [WF_Policies] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[WF_Policies]', N'utispartial') IS NULL ALTER TABLE [WF_Policies] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[WF_Policies]', N'utaddtime') IS NULL ALTER TABLE [WF_Policies] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[WF_Policies]', N'utadduser') IS NULL ALTER TABLE [WF_Policies] ADD [utadduser] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Policies]', N'utaddsystem') IS NULL ALTER TABLE [WF_Policies] ADD [utaddsystem] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Policies]', N'utaddip') IS NULL ALTER TABLE [WF_Policies] ADD [utaddip] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Policies]', N'utaddappversion') IS NULL ALTER TABLE [WF_Policies] ADD [utaddappversion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_Policies]', N'utadddbversion') IS NULL ALTER TABLE [WF_Policies] ADD [utadddbversion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_Policies]', N'utaddid') IS NULL ALTER TABLE [WF_Policies] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[WF_Policies]', N'utaddsessionid') IS NULL ALTER TABLE [WF_Policies] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[WF_Policies]', N'utedittime') IS NULL ALTER TABLE [WF_Policies] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[WF_Policies]', N'utedituser') IS NULL ALTER TABLE [WF_Policies] ADD [utedituser] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Policies]', N'uteditsystem') IS NULL ALTER TABLE [WF_Policies] ADD [uteditsystem] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Policies]', N'uteditip') IS NULL ALTER TABLE [WF_Policies] ADD [uteditip] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Policies]', N'uteditappversion') IS NULL ALTER TABLE [WF_Policies] ADD [uteditappversion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_Policies]', N'uteditdbversion') IS NULL ALTER TABLE [WF_Policies] ADD [uteditdbversion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_Policies]', N'uteditid') IS NULL ALTER TABLE [WF_Policies] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[WF_Policies]', N'uteditsessionid') IS NULL ALTER TABLE [WF_Policies] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[WF_Policies]', N'utrecordversion') IS NULL ALTER TABLE [WF_Policies] ADD [utrecordversion] int NULL;
GO
IF OBJECT_ID(N'[WF_Tasks]', N'U') IS NULL BEGIN CREATE TABLE [WF_Tasks] (
    [Id] uniqueidentifier NOT NULL,
    [InstanceId] uniqueidentifier NOT NULL,
    [ClientId] int NOT NULL,
    [UserId] int NOT NULL,
    [UserName] nvarchar(max) NOT NULL,
    [StepIndex] int NOT NULL,
    [Generation] int NOT NULL,
    [Status] nvarchar(450) NOT NULL,
    [Decision] nvarchar(max) NULL,
    [DecidedAtUtc] datetime2 NULL,
    [utdoctype] int NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] nvarchar(25) NULL,
    [utaddsystem] nvarchar(25) NULL,
    [utaddip] nvarchar(25) NULL,
    [utaddappversion] nvarchar(10) NULL,
    [utadddbversion] nvarchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddsessionid] int NULL,
    [utedittime] datetime2 NULL,
    [utedituser] nvarchar(25) NULL,
    [uteditsystem] nvarchar(25) NULL,
    [uteditip] nvarchar(25) NULL,
    [uteditappversion] nvarchar(10) NULL,
    [uteditdbversion] nvarchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditsessionid] int NULL,
    [utrecordversion] int NOT NULL,
    CONSTRAINT [PK_WF_Tasks] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[WF_Tasks]', N'Id') IS NULL ALTER TABLE [WF_Tasks] ADD [Id] uniqueidentifier NULL;
GO
IF COL_LENGTH(N'[WF_Tasks]', N'InstanceId') IS NULL ALTER TABLE [WF_Tasks] ADD [InstanceId] uniqueidentifier NULL;
GO
IF COL_LENGTH(N'[WF_Tasks]', N'ClientId') IS NULL ALTER TABLE [WF_Tasks] ADD [ClientId] int NULL;
GO
IF COL_LENGTH(N'[WF_Tasks]', N'UserId') IS NULL ALTER TABLE [WF_Tasks] ADD [UserId] int NULL;
GO
IF COL_LENGTH(N'[WF_Tasks]', N'UserName') IS NULL ALTER TABLE [WF_Tasks] ADD [UserName] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[WF_Tasks]', N'StepIndex') IS NULL ALTER TABLE [WF_Tasks] ADD [StepIndex] int NULL;
GO
IF COL_LENGTH(N'[WF_Tasks]', N'Generation') IS NULL ALTER TABLE [WF_Tasks] ADD [Generation] int NULL;
GO
IF COL_LENGTH(N'[WF_Tasks]', N'Status') IS NULL ALTER TABLE [WF_Tasks] ADD [Status] nvarchar(450) NULL;
GO
IF COL_LENGTH(N'[WF_Tasks]', N'Decision') IS NULL ALTER TABLE [WF_Tasks] ADD [Decision] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[WF_Tasks]', N'DecidedAtUtc') IS NULL ALTER TABLE [WF_Tasks] ADD [DecidedAtUtc] datetime2 NULL;
GO
IF COL_LENGTH(N'[WF_Tasks]', N'utdoctype') IS NULL ALTER TABLE [WF_Tasks] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[WF_Tasks]', N'utsubdoctype') IS NULL ALTER TABLE [WF_Tasks] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[WF_Tasks]', N'utsysgen') IS NULL ALTER TABLE [WF_Tasks] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[WF_Tasks]', N'utblock') IS NULL ALTER TABLE [WF_Tasks] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[WF_Tasks]', N'utispartial') IS NULL ALTER TABLE [WF_Tasks] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[WF_Tasks]', N'utaddtime') IS NULL ALTER TABLE [WF_Tasks] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[WF_Tasks]', N'utadduser') IS NULL ALTER TABLE [WF_Tasks] ADD [utadduser] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Tasks]', N'utaddsystem') IS NULL ALTER TABLE [WF_Tasks] ADD [utaddsystem] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Tasks]', N'utaddip') IS NULL ALTER TABLE [WF_Tasks] ADD [utaddip] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Tasks]', N'utaddappversion') IS NULL ALTER TABLE [WF_Tasks] ADD [utaddappversion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_Tasks]', N'utadddbversion') IS NULL ALTER TABLE [WF_Tasks] ADD [utadddbversion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_Tasks]', N'utaddid') IS NULL ALTER TABLE [WF_Tasks] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[WF_Tasks]', N'utaddsessionid') IS NULL ALTER TABLE [WF_Tasks] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[WF_Tasks]', N'utedittime') IS NULL ALTER TABLE [WF_Tasks] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[WF_Tasks]', N'utedituser') IS NULL ALTER TABLE [WF_Tasks] ADD [utedituser] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Tasks]', N'uteditsystem') IS NULL ALTER TABLE [WF_Tasks] ADD [uteditsystem] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Tasks]', N'uteditip') IS NULL ALTER TABLE [WF_Tasks] ADD [uteditip] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[WF_Tasks]', N'uteditappversion') IS NULL ALTER TABLE [WF_Tasks] ADD [uteditappversion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_Tasks]', N'uteditdbversion') IS NULL ALTER TABLE [WF_Tasks] ADD [uteditdbversion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[WF_Tasks]', N'uteditid') IS NULL ALTER TABLE [WF_Tasks] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[WF_Tasks]', N'uteditsessionid') IS NULL ALTER TABLE [WF_Tasks] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[WF_Tasks]', N'utrecordversion') IS NULL ALTER TABLE [WF_Tasks] ADD [utrecordversion] int NULL;
GO
IF OBJECT_ID(N'[ARG_ZkDeviceLinks]', N'U') IS NULL BEGIN CREATE TABLE [ARG_ZkDeviceLinks] (
    [MachineId] int NOT NULL,
    [SourceKey] nvarchar(64) NULL,
    [SourceClientId] int NULL,
    CONSTRAINT [PK_ARG_ZkDeviceLinks] PRIMARY KEY ([MachineId])
); END;
GO
IF COL_LENGTH(N'[ARG_ZkDeviceLinks]', N'MachineId') IS NULL ALTER TABLE [ARG_ZkDeviceLinks] ADD [MachineId] int NULL;
GO
IF COL_LENGTH(N'[ARG_ZkDeviceLinks]', N'SourceKey') IS NULL ALTER TABLE [ARG_ZkDeviceLinks] ADD [SourceKey] nvarchar(64) NULL;
GO
IF COL_LENGTH(N'[ARG_ZkDeviceLinks]', N'SourceClientId') IS NULL ALTER TABLE [ARG_ZkDeviceLinks] ADD [SourceClientId] int NULL;
GO
IF OBJECT_ID(N'[ARG_ZkEmployeeLinks]', N'U') IS NULL BEGIN CREATE TABLE [ARG_ZkEmployeeLinks] (
    [EmployeeId] int NOT NULL,
    [SourceKey] nvarchar(64) NULL,
    [SourceClientId] int NULL,
    [MachineCode] nvarchar(50) NULL,
    CONSTRAINT [PK_ARG_ZkEmployeeLinks] PRIMARY KEY ([EmployeeId])
); END;
GO
IF COL_LENGTH(N'[ARG_ZkEmployeeLinks]', N'EmployeeId') IS NULL ALTER TABLE [ARG_ZkEmployeeLinks] ADD [EmployeeId] int NULL;
GO
IF COL_LENGTH(N'[ARG_ZkEmployeeLinks]', N'SourceKey') IS NULL ALTER TABLE [ARG_ZkEmployeeLinks] ADD [SourceKey] nvarchar(64) NULL;
GO
IF COL_LENGTH(N'[ARG_ZkEmployeeLinks]', N'SourceClientId') IS NULL ALTER TABLE [ARG_ZkEmployeeLinks] ADD [SourceClientId] int NULL;
GO
IF COL_LENGTH(N'[ARG_ZkEmployeeLinks]', N'MachineCode') IS NULL ALTER TABLE [ARG_ZkEmployeeLinks] ADD [MachineCode] nvarchar(50) NULL;
GO
IF OBJECT_ID(N'[ARG_ZkImportReceipts]', N'U') IS NULL BEGIN CREATE TABLE [ARG_ZkImportReceipts] (
    [SourceKey] nvarchar(64) NOT NULL,
    [TransactionId] int NOT NULL,
    [AttendanceId] int NOT NULL,
    [ImportedAtUtc] datetime2 NOT NULL,
    CONSTRAINT [PK_ARG_ZkImportReceipts] PRIMARY KEY ([SourceKey], [TransactionId])
); END;
GO
IF COL_LENGTH(N'[ARG_ZkImportReceipts]', N'SourceKey') IS NULL ALTER TABLE [ARG_ZkImportReceipts] ADD [SourceKey] nvarchar(64) NULL;
GO
IF COL_LENGTH(N'[ARG_ZkImportReceipts]', N'TransactionId') IS NULL ALTER TABLE [ARG_ZkImportReceipts] ADD [TransactionId] int NULL;
GO
IF COL_LENGTH(N'[ARG_ZkImportReceipts]', N'AttendanceId') IS NULL ALTER TABLE [ARG_ZkImportReceipts] ADD [AttendanceId] int NULL;
GO
IF COL_LENGTH(N'[ARG_ZkImportReceipts]', N'ImportedAtUtc') IS NULL ALTER TABLE [ARG_ZkImportReceipts] ADD [ImportedAtUtc] datetime2 NULL;
GO
IF OBJECT_ID(N'[EPDeductiongrphdr]', N'U') IS NULL BEGIN CREATE TABLE [EPDeductiongrphdr] (
    [id] int NOT NULL,
    [code] nvarchar(10) NULL,
    [stxt] nvarchar(100) NULL,
    [clientid] int NULL,
    [utdoctype] int NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    CONSTRAINT [PK_EPDeductiongrphdr] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[EPDeductiongrphdr]', N'id') IS NULL ALTER TABLE [EPDeductiongrphdr] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrphdr]', N'code') IS NULL ALTER TABLE [EPDeductiongrphdr] ADD [code] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrphdr]', N'stxt') IS NULL ALTER TABLE [EPDeductiongrphdr] ADD [stxt] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrphdr]', N'clientid') IS NULL ALTER TABLE [EPDeductiongrphdr] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrphdr]', N'utdoctype') IS NULL ALTER TABLE [EPDeductiongrphdr] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrphdr]', N'utsubdoctype') IS NULL ALTER TABLE [EPDeductiongrphdr] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrphdr]', N'utsysgen') IS NULL ALTER TABLE [EPDeductiongrphdr] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrphdr]', N'utblock') IS NULL ALTER TABLE [EPDeductiongrphdr] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrphdr]', N'utispartial') IS NULL ALTER TABLE [EPDeductiongrphdr] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrphdr]', N'utrecordversion') IS NULL ALTER TABLE [EPDeductiongrphdr] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrphdr]', N'utaddappversion') IS NULL ALTER TABLE [EPDeductiongrphdr] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrphdr]', N'utadddbversion') IS NULL ALTER TABLE [EPDeductiongrphdr] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrphdr]', N'utaddid') IS NULL ALTER TABLE [EPDeductiongrphdr] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrphdr]', N'utaddip') IS NULL ALTER TABLE [EPDeductiongrphdr] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrphdr]', N'utaddsessionid') IS NULL ALTER TABLE [EPDeductiongrphdr] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrphdr]', N'utaddsystem') IS NULL ALTER TABLE [EPDeductiongrphdr] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrphdr]', N'utaddtime') IS NULL ALTER TABLE [EPDeductiongrphdr] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrphdr]', N'utadduser') IS NULL ALTER TABLE [EPDeductiongrphdr] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrphdr]', N'uteditappversion') IS NULL ALTER TABLE [EPDeductiongrphdr] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrphdr]', N'uteditdbversion') IS NULL ALTER TABLE [EPDeductiongrphdr] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrphdr]', N'uteditid') IS NULL ALTER TABLE [EPDeductiongrphdr] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrphdr]', N'uteditip') IS NULL ALTER TABLE [EPDeductiongrphdr] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrphdr]', N'uteditsessionid') IS NULL ALTER TABLE [EPDeductiongrphdr] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrphdr]', N'uteditsystem') IS NULL ALTER TABLE [EPDeductiongrphdr] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrphdr]', N'utedittime') IS NULL ALTER TABLE [EPDeductiongrphdr] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrphdr]', N'utedituser') IS NULL ALTER TABLE [EPDeductiongrphdr] ADD [utedituser] varchar(25) NULL;
GO
IF OBJECT_ID(N'[EPDeductiontype]', N'U') IS NULL BEGIN CREATE TABLE [EPDeductiontype] (
    [id] int NOT NULL,
    [code] nvarchar(10) NULL,
    [stxt] nvarchar(100) NULL,
    [ltxt] nvarchar(500) NULL,
    [payruntypeid] int NULL,
    [reimbursed] bit NOT NULL,
    [clientid] int NULL,
    [utdoctype] int NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    CONSTRAINT [PK_EPDeductiontype] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[EPDeductiontype]', N'id') IS NULL ALTER TABLE [EPDeductiontype] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[EPDeductiontype]', N'code') IS NULL ALTER TABLE [EPDeductiontype] ADD [code] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[EPDeductiontype]', N'stxt') IS NULL ALTER TABLE [EPDeductiontype] ADD [stxt] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[EPDeductiontype]', N'ltxt') IS NULL ALTER TABLE [EPDeductiontype] ADD [ltxt] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[EPDeductiontype]', N'payruntypeid') IS NULL ALTER TABLE [EPDeductiontype] ADD [payruntypeid] int NULL;
GO
IF COL_LENGTH(N'[EPDeductiontype]', N'reimbursed') IS NULL ALTER TABLE [EPDeductiontype] ADD [reimbursed] bit NULL;
GO
IF COL_LENGTH(N'[EPDeductiontype]', N'clientid') IS NULL ALTER TABLE [EPDeductiontype] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[EPDeductiontype]', N'utdoctype') IS NULL ALTER TABLE [EPDeductiontype] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[EPDeductiontype]', N'utsubdoctype') IS NULL ALTER TABLE [EPDeductiontype] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[EPDeductiontype]', N'utsysgen') IS NULL ALTER TABLE [EPDeductiontype] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[EPDeductiontype]', N'utblock') IS NULL ALTER TABLE [EPDeductiontype] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[EPDeductiontype]', N'utispartial') IS NULL ALTER TABLE [EPDeductiontype] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[EPDeductiontype]', N'utrecordversion') IS NULL ALTER TABLE [EPDeductiontype] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[EPDeductiontype]', N'utaddappversion') IS NULL ALTER TABLE [EPDeductiontype] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EPDeductiontype]', N'utadddbversion') IS NULL ALTER TABLE [EPDeductiontype] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EPDeductiontype]', N'utaddid') IS NULL ALTER TABLE [EPDeductiontype] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[EPDeductiontype]', N'utaddip') IS NULL ALTER TABLE [EPDeductiontype] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPDeductiontype]', N'utaddsessionid') IS NULL ALTER TABLE [EPDeductiontype] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[EPDeductiontype]', N'utaddsystem') IS NULL ALTER TABLE [EPDeductiontype] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPDeductiontype]', N'utaddtime') IS NULL ALTER TABLE [EPDeductiontype] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EPDeductiontype]', N'utadduser') IS NULL ALTER TABLE [EPDeductiontype] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPDeductiontype]', N'uteditappversion') IS NULL ALTER TABLE [EPDeductiontype] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EPDeductiontype]', N'uteditdbversion') IS NULL ALTER TABLE [EPDeductiontype] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EPDeductiontype]', N'uteditid') IS NULL ALTER TABLE [EPDeductiontype] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[EPDeductiontype]', N'uteditip') IS NULL ALTER TABLE [EPDeductiontype] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPDeductiontype]', N'uteditsessionid') IS NULL ALTER TABLE [EPDeductiontype] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[EPDeductiontype]', N'uteditsystem') IS NULL ALTER TABLE [EPDeductiontype] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPDeductiontype]', N'utedittime') IS NULL ALTER TABLE [EPDeductiontype] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EPDeductiontype]', N'utedituser') IS NULL ALTER TABLE [EPDeductiontype] ADD [utedituser] varchar(25) NULL;
GO
IF OBJECT_ID(N'[EPEarninggrphdr]', N'U') IS NULL BEGIN CREATE TABLE [EPEarninggrphdr] (
    [id] int NOT NULL,
    [code] nvarchar(10) NULL,
    [stxt] nvarchar(100) NULL,
    [clientid] int NULL,
    [utdoctype] int NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    CONSTRAINT [PK_EPEarninggrphdr] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[EPEarninggrphdr]', N'id') IS NULL ALTER TABLE [EPEarninggrphdr] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[EPEarninggrphdr]', N'code') IS NULL ALTER TABLE [EPEarninggrphdr] ADD [code] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[EPEarninggrphdr]', N'stxt') IS NULL ALTER TABLE [EPEarninggrphdr] ADD [stxt] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[EPEarninggrphdr]', N'clientid') IS NULL ALTER TABLE [EPEarninggrphdr] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[EPEarninggrphdr]', N'utdoctype') IS NULL ALTER TABLE [EPEarninggrphdr] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[EPEarninggrphdr]', N'utsubdoctype') IS NULL ALTER TABLE [EPEarninggrphdr] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[EPEarninggrphdr]', N'utsysgen') IS NULL ALTER TABLE [EPEarninggrphdr] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[EPEarninggrphdr]', N'utblock') IS NULL ALTER TABLE [EPEarninggrphdr] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[EPEarninggrphdr]', N'utispartial') IS NULL ALTER TABLE [EPEarninggrphdr] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[EPEarninggrphdr]', N'utrecordversion') IS NULL ALTER TABLE [EPEarninggrphdr] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[EPEarninggrphdr]', N'utaddappversion') IS NULL ALTER TABLE [EPEarninggrphdr] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EPEarninggrphdr]', N'utadddbversion') IS NULL ALTER TABLE [EPEarninggrphdr] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EPEarninggrphdr]', N'utaddid') IS NULL ALTER TABLE [EPEarninggrphdr] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[EPEarninggrphdr]', N'utaddip') IS NULL ALTER TABLE [EPEarninggrphdr] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPEarninggrphdr]', N'utaddsessionid') IS NULL ALTER TABLE [EPEarninggrphdr] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[EPEarninggrphdr]', N'utaddsystem') IS NULL ALTER TABLE [EPEarninggrphdr] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPEarninggrphdr]', N'utaddtime') IS NULL ALTER TABLE [EPEarninggrphdr] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EPEarninggrphdr]', N'utadduser') IS NULL ALTER TABLE [EPEarninggrphdr] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPEarninggrphdr]', N'uteditappversion') IS NULL ALTER TABLE [EPEarninggrphdr] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EPEarninggrphdr]', N'uteditdbversion') IS NULL ALTER TABLE [EPEarninggrphdr] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EPEarninggrphdr]', N'uteditid') IS NULL ALTER TABLE [EPEarninggrphdr] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[EPEarninggrphdr]', N'uteditip') IS NULL ALTER TABLE [EPEarninggrphdr] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPEarninggrphdr]', N'uteditsessionid') IS NULL ALTER TABLE [EPEarninggrphdr] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[EPEarninggrphdr]', N'uteditsystem') IS NULL ALTER TABLE [EPEarninggrphdr] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPEarninggrphdr]', N'utedittime') IS NULL ALTER TABLE [EPEarninggrphdr] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EPEarninggrphdr]', N'utedituser') IS NULL ALTER TABLE [EPEarninggrphdr] ADD [utedituser] varchar(25) NULL;
GO
IF OBJECT_ID(N'[EPEarningtype]', N'U') IS NULL BEGIN CREATE TABLE [EPEarningtype] (
    [id] int NOT NULL,
    [code] nvarchar(50) NULL,
    [stxt] nvarchar(250) NULL,
    [ltxt] nvarchar(500) NULL,
    [payruntypeid] int NULL,
    [taxmethodid] int NULL,
    [uomid] int NULL,
    [ratebasisid] int NULL,
    [bssalary] bit NULL,
    [earningtypeid] int NULL,
    [paysliindex] nvarchar(100) NULL,
    [payslipdesc] nvarchar(500) NULL,
    [value] decimal(18,2) NULL,
    [clientid] int NULL,
    [utsysgen] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utblock] bit NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_EPEarningtype] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[EPEarningtype]', N'id') IS NULL ALTER TABLE [EPEarningtype] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[EPEarningtype]', N'code') IS NULL ALTER TABLE [EPEarningtype] ADD [code] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[EPEarningtype]', N'stxt') IS NULL ALTER TABLE [EPEarningtype] ADD [stxt] nvarchar(250) NULL;
GO
IF COL_LENGTH(N'[EPEarningtype]', N'ltxt') IS NULL ALTER TABLE [EPEarningtype] ADD [ltxt] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[EPEarningtype]', N'payruntypeid') IS NULL ALTER TABLE [EPEarningtype] ADD [payruntypeid] int NULL;
GO
IF COL_LENGTH(N'[EPEarningtype]', N'taxmethodid') IS NULL ALTER TABLE [EPEarningtype] ADD [taxmethodid] int NULL;
GO
IF COL_LENGTH(N'[EPEarningtype]', N'uomid') IS NULL ALTER TABLE [EPEarningtype] ADD [uomid] int NULL;
GO
IF COL_LENGTH(N'[EPEarningtype]', N'ratebasisid') IS NULL ALTER TABLE [EPEarningtype] ADD [ratebasisid] int NULL;
GO
IF COL_LENGTH(N'[EPEarningtype]', N'bssalary') IS NULL ALTER TABLE [EPEarningtype] ADD [bssalary] bit NULL;
GO
IF COL_LENGTH(N'[EPEarningtype]', N'earningtypeid') IS NULL ALTER TABLE [EPEarningtype] ADD [earningtypeid] int NULL;
GO
IF COL_LENGTH(N'[EPEarningtype]', N'paysliindex') IS NULL ALTER TABLE [EPEarningtype] ADD [paysliindex] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[EPEarningtype]', N'payslipdesc') IS NULL ALTER TABLE [EPEarningtype] ADD [payslipdesc] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[EPEarningtype]', N'value') IS NULL ALTER TABLE [EPEarningtype] ADD [value] decimal(18,2) NULL;
GO
IF COL_LENGTH(N'[EPEarningtype]', N'clientid') IS NULL ALTER TABLE [EPEarningtype] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[EPEarningtype]', N'utsysgen') IS NULL ALTER TABLE [EPEarningtype] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[EPEarningtype]', N'utispartial') IS NULL ALTER TABLE [EPEarningtype] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[EPEarningtype]', N'utrecordversion') IS NULL ALTER TABLE [EPEarningtype] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[EPEarningtype]', N'utblock') IS NULL ALTER TABLE [EPEarningtype] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[EPEarningtype]', N'utaddappversion') IS NULL ALTER TABLE [EPEarningtype] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EPEarningtype]', N'utadddbversion') IS NULL ALTER TABLE [EPEarningtype] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EPEarningtype]', N'utaddid') IS NULL ALTER TABLE [EPEarningtype] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[EPEarningtype]', N'utaddip') IS NULL ALTER TABLE [EPEarningtype] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPEarningtype]', N'utaddsessionid') IS NULL ALTER TABLE [EPEarningtype] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[EPEarningtype]', N'utaddsystem') IS NULL ALTER TABLE [EPEarningtype] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPEarningtype]', N'utaddtime') IS NULL ALTER TABLE [EPEarningtype] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EPEarningtype]', N'utadduser') IS NULL ALTER TABLE [EPEarningtype] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPEarningtype]', N'utdoctype') IS NULL ALTER TABLE [EPEarningtype] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[EPEarningtype]', N'uteditappversion') IS NULL ALTER TABLE [EPEarningtype] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EPEarningtype]', N'uteditdbversion') IS NULL ALTER TABLE [EPEarningtype] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EPEarningtype]', N'uteditid') IS NULL ALTER TABLE [EPEarningtype] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[EPEarningtype]', N'uteditip') IS NULL ALTER TABLE [EPEarningtype] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPEarningtype]', N'uteditsessionid') IS NULL ALTER TABLE [EPEarningtype] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[EPEarningtype]', N'uteditsystem') IS NULL ALTER TABLE [EPEarningtype] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPEarningtype]', N'utedittime') IS NULL ALTER TABLE [EPEarningtype] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EPEarningtype]', N'utedituser') IS NULL ALTER TABLE [EPEarningtype] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPEarningtype]', N'utsubdoctype') IS NULL ALTER TABLE [EPEarningtype] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[EPLoantype]', N'U') IS NULL BEGIN CREATE TABLE [EPLoantype] (
    [id] int NOT NULL,
    [code] nvarchar(20) NULL,
    [stxt] nvarchar(150) NULL,
    [ltxt] nvarchar(500) NULL,
    [loannatureid] int NULL,
    [limitid] int NULL,
    [limit] float NULL,
    [availid] int NULL,
    [loaninterestid] int NULL,
    [jobduration] int NULL,
    [noofloan] int NULL,
    [noofinstallments] int NULL,
    [flexible] bit NOT NULL,
    [clientid] int NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_EPLoantype] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[EPLoantype]', N'id') IS NULL ALTER TABLE [EPLoantype] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[EPLoantype]', N'code') IS NULL ALTER TABLE [EPLoantype] ADD [code] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[EPLoantype]', N'stxt') IS NULL ALTER TABLE [EPLoantype] ADD [stxt] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[EPLoantype]', N'ltxt') IS NULL ALTER TABLE [EPLoantype] ADD [ltxt] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[EPLoantype]', N'loannatureid') IS NULL ALTER TABLE [EPLoantype] ADD [loannatureid] int NULL;
GO
IF COL_LENGTH(N'[EPLoantype]', N'limitid') IS NULL ALTER TABLE [EPLoantype] ADD [limitid] int NULL;
GO
IF COL_LENGTH(N'[EPLoantype]', N'limit') IS NULL ALTER TABLE [EPLoantype] ADD [limit] float NULL;
GO
IF COL_LENGTH(N'[EPLoantype]', N'availid') IS NULL ALTER TABLE [EPLoantype] ADD [availid] int NULL;
GO
IF COL_LENGTH(N'[EPLoantype]', N'loaninterestid') IS NULL ALTER TABLE [EPLoantype] ADD [loaninterestid] int NULL;
GO
IF COL_LENGTH(N'[EPLoantype]', N'jobduration') IS NULL ALTER TABLE [EPLoantype] ADD [jobduration] int NULL;
GO
IF COL_LENGTH(N'[EPLoantype]', N'noofloan') IS NULL ALTER TABLE [EPLoantype] ADD [noofloan] int NULL;
GO
IF COL_LENGTH(N'[EPLoantype]', N'noofinstallments') IS NULL ALTER TABLE [EPLoantype] ADD [noofinstallments] int NULL;
GO
IF COL_LENGTH(N'[EPLoantype]', N'flexible') IS NULL ALTER TABLE [EPLoantype] ADD [flexible] bit NULL;
GO
IF COL_LENGTH(N'[EPLoantype]', N'clientid') IS NULL ALTER TABLE [EPLoantype] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[EPLoantype]', N'utsysgen') IS NULL ALTER TABLE [EPLoantype] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[EPLoantype]', N'utblock') IS NULL ALTER TABLE [EPLoantype] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[EPLoantype]', N'utispartial') IS NULL ALTER TABLE [EPLoantype] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[EPLoantype]', N'utrecordversion') IS NULL ALTER TABLE [EPLoantype] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[EPLoantype]', N'utaddappversion') IS NULL ALTER TABLE [EPLoantype] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EPLoantype]', N'utadddbversion') IS NULL ALTER TABLE [EPLoantype] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EPLoantype]', N'utaddid') IS NULL ALTER TABLE [EPLoantype] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[EPLoantype]', N'utaddip') IS NULL ALTER TABLE [EPLoantype] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPLoantype]', N'utaddsessionid') IS NULL ALTER TABLE [EPLoantype] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[EPLoantype]', N'utaddsystem') IS NULL ALTER TABLE [EPLoantype] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPLoantype]', N'utaddtime') IS NULL ALTER TABLE [EPLoantype] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EPLoantype]', N'utadduser') IS NULL ALTER TABLE [EPLoantype] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPLoantype]', N'utdoctype') IS NULL ALTER TABLE [EPLoantype] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[EPLoantype]', N'uteditappversion') IS NULL ALTER TABLE [EPLoantype] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EPLoantype]', N'uteditdbversion') IS NULL ALTER TABLE [EPLoantype] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EPLoantype]', N'uteditid') IS NULL ALTER TABLE [EPLoantype] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[EPLoantype]', N'uteditip') IS NULL ALTER TABLE [EPLoantype] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPLoantype]', N'uteditsessionid') IS NULL ALTER TABLE [EPLoantype] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[EPLoantype]', N'uteditsystem') IS NULL ALTER TABLE [EPLoantype] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPLoantype]', N'utedittime') IS NULL ALTER TABLE [EPLoantype] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EPLoantype]', N'utedituser') IS NULL ALTER TABLE [EPLoantype] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPLoantype]', N'utsubdoctype') IS NULL ALTER TABLE [EPLoantype] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[ERDesgPayroll]', N'U') IS NULL BEGIN CREATE TABLE [ERDesgPayroll] (
    [Id] int NOT NULL,
    [designationid] int NULL,
    [clientid] int NULL,
    [loanallowed] bit NOT NULL,
    [advanceallowed] bit NOT NULL,
    [payfullsalary] bit NOT NULL,
    [overtimeallowed] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    CONSTRAINT [PK_ERDesgPayroll] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[ERDesgPayroll]', N'Id') IS NULL ALTER TABLE [ERDesgPayroll] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[ERDesgPayroll]', N'designationid') IS NULL ALTER TABLE [ERDesgPayroll] ADD [designationid] int NULL;
GO
IF COL_LENGTH(N'[ERDesgPayroll]', N'clientid') IS NULL ALTER TABLE [ERDesgPayroll] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[ERDesgPayroll]', N'loanallowed') IS NULL ALTER TABLE [ERDesgPayroll] ADD [loanallowed] bit NULL;
GO
IF COL_LENGTH(N'[ERDesgPayroll]', N'advanceallowed') IS NULL ALTER TABLE [ERDesgPayroll] ADD [advanceallowed] bit NULL;
GO
IF COL_LENGTH(N'[ERDesgPayroll]', N'payfullsalary') IS NULL ALTER TABLE [ERDesgPayroll] ADD [payfullsalary] bit NULL;
GO
IF COL_LENGTH(N'[ERDesgPayroll]', N'overtimeallowed') IS NULL ALTER TABLE [ERDesgPayroll] ADD [overtimeallowed] bit NULL;
GO
IF COL_LENGTH(N'[ERDesgPayroll]', N'utblock') IS NULL ALTER TABLE [ERDesgPayroll] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[ERDesgPayroll]', N'utrecordversion') IS NULL ALTER TABLE [ERDesgPayroll] ADD [utrecordversion] int NULL;
GO
IF OBJECT_ID(N'[EREmpaddress]', N'U') IS NULL BEGIN CREATE TABLE [EREmpaddress] (
    [id] int NOT NULL,
    [code] nvarchar(10) NULL,
    [employeeid] int NULL,
    [ltxt] nvarchar(50) NULL,
    [adtypeid] int NULL,
    [countryid] int NULL,
    [stateid] int NULL,
    [cityid] int NULL,
    [address] nvarchar(500) NULL,
    [primaryadd] bit NULL,
    [contactname] nvarchar(50) NULL,
    [landline] nvarchar(15) NULL,
    [ext] nvarchar(5) NULL,
    [mobile] nvarchar(15) NULL,
    [utblock] bit NULL,
    [utblock1] bit NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    CONSTRAINT [PK_EREmpaddress] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[EREmpaddress]', N'id') IS NULL ALTER TABLE [EREmpaddress] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[EREmpaddress]', N'code') IS NULL ALTER TABLE [EREmpaddress] ADD [code] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmpaddress]', N'employeeid') IS NULL ALTER TABLE [EREmpaddress] ADD [employeeid] int NULL;
GO
IF COL_LENGTH(N'[EREmpaddress]', N'ltxt') IS NULL ALTER TABLE [EREmpaddress] ADD [ltxt] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[EREmpaddress]', N'adtypeid') IS NULL ALTER TABLE [EREmpaddress] ADD [adtypeid] int NULL;
GO
IF COL_LENGTH(N'[EREmpaddress]', N'countryid') IS NULL ALTER TABLE [EREmpaddress] ADD [countryid] int NULL;
GO
IF COL_LENGTH(N'[EREmpaddress]', N'stateid') IS NULL ALTER TABLE [EREmpaddress] ADD [stateid] int NULL;
GO
IF COL_LENGTH(N'[EREmpaddress]', N'cityid') IS NULL ALTER TABLE [EREmpaddress] ADD [cityid] int NULL;
GO
IF COL_LENGTH(N'[EREmpaddress]', N'address') IS NULL ALTER TABLE [EREmpaddress] ADD [address] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[EREmpaddress]', N'primaryadd') IS NULL ALTER TABLE [EREmpaddress] ADD [primaryadd] bit NULL;
GO
IF COL_LENGTH(N'[EREmpaddress]', N'contactname') IS NULL ALTER TABLE [EREmpaddress] ADD [contactname] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[EREmpaddress]', N'landline') IS NULL ALTER TABLE [EREmpaddress] ADD [landline] nvarchar(15) NULL;
GO
IF COL_LENGTH(N'[EREmpaddress]', N'ext') IS NULL ALTER TABLE [EREmpaddress] ADD [ext] nvarchar(5) NULL;
GO
IF COL_LENGTH(N'[EREmpaddress]', N'mobile') IS NULL ALTER TABLE [EREmpaddress] ADD [mobile] nvarchar(15) NULL;
GO
IF COL_LENGTH(N'[EREmpaddress]', N'utblock') IS NULL ALTER TABLE [EREmpaddress] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[EREmpaddress]', N'utblock1') IS NULL ALTER TABLE [EREmpaddress] ADD [utblock1] bit NULL;
GO
IF COL_LENGTH(N'[EREmpaddress]', N'utaddappversion') IS NULL ALTER TABLE [EREmpaddress] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmpaddress]', N'utadddbversion') IS NULL ALTER TABLE [EREmpaddress] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmpaddress]', N'utaddid') IS NULL ALTER TABLE [EREmpaddress] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[EREmpaddress]', N'utaddip') IS NULL ALTER TABLE [EREmpaddress] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpaddress]', N'utaddsessionid') IS NULL ALTER TABLE [EREmpaddress] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[EREmpaddress]', N'utaddsystem') IS NULL ALTER TABLE [EREmpaddress] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpaddress]', N'utaddtime') IS NULL ALTER TABLE [EREmpaddress] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmpaddress]', N'utadduser') IS NULL ALTER TABLE [EREmpaddress] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpaddress]', N'utdoctype') IS NULL ALTER TABLE [EREmpaddress] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[EREmpaddress]', N'uteditappversion') IS NULL ALTER TABLE [EREmpaddress] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmpaddress]', N'uteditdbversion') IS NULL ALTER TABLE [EREmpaddress] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmpaddress]', N'uteditid') IS NULL ALTER TABLE [EREmpaddress] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[EREmpaddress]', N'uteditip') IS NULL ALTER TABLE [EREmpaddress] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpaddress]', N'uteditsessionid') IS NULL ALTER TABLE [EREmpaddress] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[EREmpaddress]', N'uteditsystem') IS NULL ALTER TABLE [EREmpaddress] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpaddress]', N'utedittime') IS NULL ALTER TABLE [EREmpaddress] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmpaddress]', N'utedituser') IS NULL ALTER TABLE [EREmpaddress] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpaddress]', N'utispartial') IS NULL ALTER TABLE [EREmpaddress] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[EREmpaddress]', N'utrecordversion') IS NULL ALTER TABLE [EREmpaddress] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[EREmpaddress]', N'utsubdoctype') IS NULL ALTER TABLE [EREmpaddress] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[EREmpaddress]', N'utsysgen') IS NULL ALTER TABLE [EREmpaddress] ADD [utsysgen] bit NULL;
GO
IF OBJECT_ID(N'[EREmpAuthority]', N'U') IS NULL BEGIN CREATE TABLE [EREmpAuthority] (
    [id] int NOT NULL,
    [code] nvarchar(10) NULL,
    [authtypeid] int NULL,
    [stxt] nvarchar(150) NULL,
    [ltxt] nvarchar(500) NULL,
    [email] nvarchar(255) NULL,
    [contactno] nvarchar(100) NULL,
    [phone] nvarchar(100) NULL,
    [web] nvarchar(255) NULL,
    [fax] nvarchar(100) NULL,
    [entityid] int NULL,
    [clientid] int NULL,
    [utblock] bit NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    CONSTRAINT [PK_EREmpAuthority] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[EREmpAuthority]', N'id') IS NULL ALTER TABLE [EREmpAuthority] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[EREmpAuthority]', N'code') IS NULL ALTER TABLE [EREmpAuthority] ADD [code] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmpAuthority]', N'authtypeid') IS NULL ALTER TABLE [EREmpAuthority] ADD [authtypeid] int NULL;
GO
IF COL_LENGTH(N'[EREmpAuthority]', N'stxt') IS NULL ALTER TABLE [EREmpAuthority] ADD [stxt] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[EREmpAuthority]', N'ltxt') IS NULL ALTER TABLE [EREmpAuthority] ADD [ltxt] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[EREmpAuthority]', N'email') IS NULL ALTER TABLE [EREmpAuthority] ADD [email] nvarchar(255) NULL;
GO
IF COL_LENGTH(N'[EREmpAuthority]', N'contactno') IS NULL ALTER TABLE [EREmpAuthority] ADD [contactno] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[EREmpAuthority]', N'phone') IS NULL ALTER TABLE [EREmpAuthority] ADD [phone] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[EREmpAuthority]', N'web') IS NULL ALTER TABLE [EREmpAuthority] ADD [web] nvarchar(255) NULL;
GO
IF COL_LENGTH(N'[EREmpAuthority]', N'fax') IS NULL ALTER TABLE [EREmpAuthority] ADD [fax] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[EREmpAuthority]', N'entityid') IS NULL ALTER TABLE [EREmpAuthority] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[EREmpAuthority]', N'clientid') IS NULL ALTER TABLE [EREmpAuthority] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[EREmpAuthority]', N'utblock') IS NULL ALTER TABLE [EREmpAuthority] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[EREmpAuthority]', N'utaddappversion') IS NULL ALTER TABLE [EREmpAuthority] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmpAuthority]', N'utadddbversion') IS NULL ALTER TABLE [EREmpAuthority] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmpAuthority]', N'utaddid') IS NULL ALTER TABLE [EREmpAuthority] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[EREmpAuthority]', N'utaddip') IS NULL ALTER TABLE [EREmpAuthority] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpAuthority]', N'utaddsessionid') IS NULL ALTER TABLE [EREmpAuthority] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[EREmpAuthority]', N'utaddsystem') IS NULL ALTER TABLE [EREmpAuthority] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpAuthority]', N'utaddtime') IS NULL ALTER TABLE [EREmpAuthority] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmpAuthority]', N'utadduser') IS NULL ALTER TABLE [EREmpAuthority] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpAuthority]', N'utdoctype') IS NULL ALTER TABLE [EREmpAuthority] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[EREmpAuthority]', N'uteditappversion') IS NULL ALTER TABLE [EREmpAuthority] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmpAuthority]', N'uteditdbversion') IS NULL ALTER TABLE [EREmpAuthority] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmpAuthority]', N'uteditid') IS NULL ALTER TABLE [EREmpAuthority] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[EREmpAuthority]', N'uteditip') IS NULL ALTER TABLE [EREmpAuthority] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpAuthority]', N'uteditsessionid') IS NULL ALTER TABLE [EREmpAuthority] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[EREmpAuthority]', N'uteditsystem') IS NULL ALTER TABLE [EREmpAuthority] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpAuthority]', N'utedittime') IS NULL ALTER TABLE [EREmpAuthority] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmpAuthority]', N'utedituser') IS NULL ALTER TABLE [EREmpAuthority] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpAuthority]', N'utispartial') IS NULL ALTER TABLE [EREmpAuthority] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[EREmpAuthority]', N'utrecordversion') IS NULL ALTER TABLE [EREmpAuthority] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[EREmpAuthority]', N'utsubdoctype') IS NULL ALTER TABLE [EREmpAuthority] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[EREmpAuthority]', N'utsysgen') IS NULL ALTER TABLE [EREmpAuthority] ADD [utsysgen] bit NULL;
GO
IF OBJECT_ID(N'[EREmployment]', N'U') IS NULL BEGIN CREATE TABLE [EREmployment] (
    [Id] int NOT NULL,
    [clientid] int NULL,
    [entityid] int NULL,
    [employeeid] int NULL,
    [employmenttypeid] int NULL,
    [startdate] datetime2 NULL,
    [enddate] datetime2 NULL,
    [utsysgen] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utrecordversion] int NOT NULL,
    [utblock] bit NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_EREmployment] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[EREmployment]', N'Id') IS NULL ALTER TABLE [EREmployment] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[EREmployment]', N'clientid') IS NULL ALTER TABLE [EREmployment] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[EREmployment]', N'entityid') IS NULL ALTER TABLE [EREmployment] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[EREmployment]', N'employeeid') IS NULL ALTER TABLE [EREmployment] ADD [employeeid] int NULL;
GO
IF COL_LENGTH(N'[EREmployment]', N'employmenttypeid') IS NULL ALTER TABLE [EREmployment] ADD [employmenttypeid] int NULL;
GO
IF COL_LENGTH(N'[EREmployment]', N'startdate') IS NULL ALTER TABLE [EREmployment] ADD [startdate] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmployment]', N'enddate') IS NULL ALTER TABLE [EREmployment] ADD [enddate] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmployment]', N'utsysgen') IS NULL ALTER TABLE [EREmployment] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[EREmployment]', N'utispartial') IS NULL ALTER TABLE [EREmployment] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[EREmployment]', N'utaddtime') IS NULL ALTER TABLE [EREmployment] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmployment]', N'utadduser') IS NULL ALTER TABLE [EREmployment] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmployment]', N'utedittime') IS NULL ALTER TABLE [EREmployment] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmployment]', N'utedituser') IS NULL ALTER TABLE [EREmployment] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmployment]', N'utrecordversion') IS NULL ALTER TABLE [EREmployment] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[EREmployment]', N'utblock') IS NULL ALTER TABLE [EREmployment] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[EREmployment]', N'utaddappversion') IS NULL ALTER TABLE [EREmployment] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmployment]', N'utadddbversion') IS NULL ALTER TABLE [EREmployment] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmployment]', N'utaddid') IS NULL ALTER TABLE [EREmployment] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[EREmployment]', N'utaddip') IS NULL ALTER TABLE [EREmployment] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmployment]', N'utaddsessionid') IS NULL ALTER TABLE [EREmployment] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[EREmployment]', N'utaddsystem') IS NULL ALTER TABLE [EREmployment] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmployment]', N'utdoctype') IS NULL ALTER TABLE [EREmployment] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[EREmployment]', N'uteditappversion') IS NULL ALTER TABLE [EREmployment] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmployment]', N'uteditdbversion') IS NULL ALTER TABLE [EREmployment] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmployment]', N'uteditid') IS NULL ALTER TABLE [EREmployment] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[EREmployment]', N'uteditip') IS NULL ALTER TABLE [EREmployment] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmployment]', N'uteditsessionid') IS NULL ALTER TABLE [EREmployment] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[EREmployment]', N'uteditsystem') IS NULL ALTER TABLE [EREmployment] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmployment]', N'utsubdoctype') IS NULL ALTER TABLE [EREmployment] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[EREmploymenttype]', N'U') IS NULL BEGIN CREATE TABLE [EREmploymenttype] (
    [id] int NOT NULL,
    [code] nvarchar(10) NULL,
    [stxt] nvarchar(150) NULL,
    [ltxt] nvarchar(500) NULL,
    CONSTRAINT [PK_EREmploymenttype] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[EREmploymenttype]', N'id') IS NULL ALTER TABLE [EREmploymenttype] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[EREmploymenttype]', N'code') IS NULL ALTER TABLE [EREmploymenttype] ADD [code] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmploymenttype]', N'stxt') IS NULL ALTER TABLE [EREmploymenttype] ADD [stxt] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[EREmploymenttype]', N'ltxt') IS NULL ALTER TABLE [EREmploymenttype] ADD [ltxt] nvarchar(500) NULL;
GO
IF OBJECT_ID(N'[ERPositiontype]', N'U') IS NULL BEGIN CREATE TABLE [ERPositiontype] (
    [id] int NOT NULL,
    [code] nvarchar(10) NULL,
    [emptypeid] int NULL,
    [stxt] nvarchar(150) NULL,
    [ltxt] nvarchar(500) NULL,
    [entityid] int NULL,
    [clientid] int NULL,
    [utblock] bit NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    CONSTRAINT [PK_ERPositiontype] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[ERPositiontype]', N'id') IS NULL ALTER TABLE [ERPositiontype] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[ERPositiontype]', N'code') IS NULL ALTER TABLE [ERPositiontype] ADD [code] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[ERPositiontype]', N'emptypeid') IS NULL ALTER TABLE [ERPositiontype] ADD [emptypeid] int NULL;
GO
IF COL_LENGTH(N'[ERPositiontype]', N'stxt') IS NULL ALTER TABLE [ERPositiontype] ADD [stxt] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[ERPositiontype]', N'ltxt') IS NULL ALTER TABLE [ERPositiontype] ADD [ltxt] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[ERPositiontype]', N'entityid') IS NULL ALTER TABLE [ERPositiontype] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[ERPositiontype]', N'clientid') IS NULL ALTER TABLE [ERPositiontype] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[ERPositiontype]', N'utblock') IS NULL ALTER TABLE [ERPositiontype] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[ERPositiontype]', N'utaddappversion') IS NULL ALTER TABLE [ERPositiontype] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ERPositiontype]', N'utadddbversion') IS NULL ALTER TABLE [ERPositiontype] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ERPositiontype]', N'utaddid') IS NULL ALTER TABLE [ERPositiontype] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[ERPositiontype]', N'utaddip') IS NULL ALTER TABLE [ERPositiontype] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERPositiontype]', N'utaddsessionid') IS NULL ALTER TABLE [ERPositiontype] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[ERPositiontype]', N'utaddsystem') IS NULL ALTER TABLE [ERPositiontype] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERPositiontype]', N'utaddtime') IS NULL ALTER TABLE [ERPositiontype] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ERPositiontype]', N'utadduser') IS NULL ALTER TABLE [ERPositiontype] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERPositiontype]', N'utdoctype') IS NULL ALTER TABLE [ERPositiontype] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[ERPositiontype]', N'uteditappversion') IS NULL ALTER TABLE [ERPositiontype] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ERPositiontype]', N'uteditdbversion') IS NULL ALTER TABLE [ERPositiontype] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ERPositiontype]', N'uteditid') IS NULL ALTER TABLE [ERPositiontype] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[ERPositiontype]', N'uteditip') IS NULL ALTER TABLE [ERPositiontype] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERPositiontype]', N'uteditsessionid') IS NULL ALTER TABLE [ERPositiontype] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[ERPositiontype]', N'uteditsystem') IS NULL ALTER TABLE [ERPositiontype] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERPositiontype]', N'utedittime') IS NULL ALTER TABLE [ERPositiontype] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ERPositiontype]', N'utedituser') IS NULL ALTER TABLE [ERPositiontype] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERPositiontype]', N'utispartial') IS NULL ALTER TABLE [ERPositiontype] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[ERPositiontype]', N'utrecordversion') IS NULL ALTER TABLE [ERPositiontype] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[ERPositiontype]', N'utsubdoctype') IS NULL ALTER TABLE [ERPositiontype] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[ERPositiontype]', N'utsysgen') IS NULL ALTER TABLE [ERPositiontype] ADD [utsysgen] bit NULL;
GO
IF OBJECT_ID(N'[ERQualification]', N'U') IS NULL BEGIN CREATE TABLE [ERQualification] (
    [id] int NOT NULL,
    [code] nvarchar(10) NULL,
    [quatypeid] int NULL,
    [stxt] nvarchar(150) NULL,
    [ltxt] nvarchar(500) NULL,
    [entityid] int NULL,
    [clientid] int NULL,
    [lovtypeid] int NULL,
    [utblock] bit NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    CONSTRAINT [PK_ERQualification] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[ERQualification]', N'id') IS NULL ALTER TABLE [ERQualification] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[ERQualification]', N'code') IS NULL ALTER TABLE [ERQualification] ADD [code] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[ERQualification]', N'quatypeid') IS NULL ALTER TABLE [ERQualification] ADD [quatypeid] int NULL;
GO
IF COL_LENGTH(N'[ERQualification]', N'stxt') IS NULL ALTER TABLE [ERQualification] ADD [stxt] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[ERQualification]', N'ltxt') IS NULL ALTER TABLE [ERQualification] ADD [ltxt] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[ERQualification]', N'entityid') IS NULL ALTER TABLE [ERQualification] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[ERQualification]', N'clientid') IS NULL ALTER TABLE [ERQualification] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[ERQualification]', N'lovtypeid') IS NULL ALTER TABLE [ERQualification] ADD [lovtypeid] int NULL;
GO
IF COL_LENGTH(N'[ERQualification]', N'utblock') IS NULL ALTER TABLE [ERQualification] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[ERQualification]', N'utaddappversion') IS NULL ALTER TABLE [ERQualification] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ERQualification]', N'utadddbversion') IS NULL ALTER TABLE [ERQualification] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ERQualification]', N'utaddid') IS NULL ALTER TABLE [ERQualification] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[ERQualification]', N'utaddip') IS NULL ALTER TABLE [ERQualification] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERQualification]', N'utaddsessionid') IS NULL ALTER TABLE [ERQualification] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[ERQualification]', N'utaddsystem') IS NULL ALTER TABLE [ERQualification] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERQualification]', N'utaddtime') IS NULL ALTER TABLE [ERQualification] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ERQualification]', N'utadduser') IS NULL ALTER TABLE [ERQualification] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERQualification]', N'utdoctype') IS NULL ALTER TABLE [ERQualification] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[ERQualification]', N'uteditappversion') IS NULL ALTER TABLE [ERQualification] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ERQualification]', N'uteditdbversion') IS NULL ALTER TABLE [ERQualification] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ERQualification]', N'uteditid') IS NULL ALTER TABLE [ERQualification] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[ERQualification]', N'uteditip') IS NULL ALTER TABLE [ERQualification] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERQualification]', N'uteditsessionid') IS NULL ALTER TABLE [ERQualification] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[ERQualification]', N'uteditsystem') IS NULL ALTER TABLE [ERQualification] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERQualification]', N'utedittime') IS NULL ALTER TABLE [ERQualification] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ERQualification]', N'utedituser') IS NULL ALTER TABLE [ERQualification] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERQualification]', N'utispartial') IS NULL ALTER TABLE [ERQualification] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[ERQualification]', N'utrecordversion') IS NULL ALTER TABLE [ERQualification] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[ERQualification]', N'utsubdoctype') IS NULL ALTER TABLE [ERQualification] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[ERQualification]', N'utsysgen') IS NULL ALTER TABLE [ERQualification] ADD [utsysgen] bit NULL;
GO
IF OBJECT_ID(N'[ERQualificationtype]', N'U') IS NULL BEGIN CREATE TABLE [ERQualificationtype] (
    [id] int NOT NULL,
    [code] nvarchar(10) NULL,
    [stxt] nvarchar(150) NULL,
    [ltxt] nvarchar(500) NULL,
    [entityid] int NULL,
    [clientid] int NULL,
    [lovtypeid] int NULL,
    [utblock] bit NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    CONSTRAINT [PK_ERQualificationtype] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[ERQualificationtype]', N'id') IS NULL ALTER TABLE [ERQualificationtype] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[ERQualificationtype]', N'code') IS NULL ALTER TABLE [ERQualificationtype] ADD [code] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[ERQualificationtype]', N'stxt') IS NULL ALTER TABLE [ERQualificationtype] ADD [stxt] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[ERQualificationtype]', N'ltxt') IS NULL ALTER TABLE [ERQualificationtype] ADD [ltxt] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[ERQualificationtype]', N'entityid') IS NULL ALTER TABLE [ERQualificationtype] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[ERQualificationtype]', N'clientid') IS NULL ALTER TABLE [ERQualificationtype] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[ERQualificationtype]', N'lovtypeid') IS NULL ALTER TABLE [ERQualificationtype] ADD [lovtypeid] int NULL;
GO
IF COL_LENGTH(N'[ERQualificationtype]', N'utblock') IS NULL ALTER TABLE [ERQualificationtype] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[ERQualificationtype]', N'utaddappversion') IS NULL ALTER TABLE [ERQualificationtype] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ERQualificationtype]', N'utadddbversion') IS NULL ALTER TABLE [ERQualificationtype] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ERQualificationtype]', N'utaddid') IS NULL ALTER TABLE [ERQualificationtype] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[ERQualificationtype]', N'utaddip') IS NULL ALTER TABLE [ERQualificationtype] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERQualificationtype]', N'utaddsessionid') IS NULL ALTER TABLE [ERQualificationtype] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[ERQualificationtype]', N'utaddsystem') IS NULL ALTER TABLE [ERQualificationtype] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERQualificationtype]', N'utaddtime') IS NULL ALTER TABLE [ERQualificationtype] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ERQualificationtype]', N'utadduser') IS NULL ALTER TABLE [ERQualificationtype] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERQualificationtype]', N'utdoctype') IS NULL ALTER TABLE [ERQualificationtype] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[ERQualificationtype]', N'uteditappversion') IS NULL ALTER TABLE [ERQualificationtype] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ERQualificationtype]', N'uteditdbversion') IS NULL ALTER TABLE [ERQualificationtype] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ERQualificationtype]', N'uteditid') IS NULL ALTER TABLE [ERQualificationtype] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[ERQualificationtype]', N'uteditip') IS NULL ALTER TABLE [ERQualificationtype] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERQualificationtype]', N'uteditsessionid') IS NULL ALTER TABLE [ERQualificationtype] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[ERQualificationtype]', N'uteditsystem') IS NULL ALTER TABLE [ERQualificationtype] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERQualificationtype]', N'utedittime') IS NULL ALTER TABLE [ERQualificationtype] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ERQualificationtype]', N'utedituser') IS NULL ALTER TABLE [ERQualificationtype] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERQualificationtype]', N'utispartial') IS NULL ALTER TABLE [ERQualificationtype] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[ERQualificationtype]', N'utrecordversion') IS NULL ALTER TABLE [ERQualificationtype] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[ERQualificationtype]', N'utsubdoctype') IS NULL ALTER TABLE [ERQualificationtype] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[ERQualificationtype]', N'utsysgen') IS NULL ALTER TABLE [ERQualificationtype] ADD [utsysgen] bit NULL;
GO
IF OBJECT_ID(N'[ERSkill]', N'U') IS NULL BEGIN CREATE TABLE [ERSkill] (
    [id] int NOT NULL,
    [code] nvarchar(10) NULL,
    [skilltypeid] int NULL,
    [stxt] nvarchar(150) NULL,
    [ltxt] nvarchar(500) NULL,
    [entityid] int NULL,
    [clientid] int NULL,
    [skillratingid] int NULL,
    [utblock] bit NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    CONSTRAINT [PK_ERSkill] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[ERSkill]', N'id') IS NULL ALTER TABLE [ERSkill] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[ERSkill]', N'code') IS NULL ALTER TABLE [ERSkill] ADD [code] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[ERSkill]', N'skilltypeid') IS NULL ALTER TABLE [ERSkill] ADD [skilltypeid] int NULL;
GO
IF COL_LENGTH(N'[ERSkill]', N'stxt') IS NULL ALTER TABLE [ERSkill] ADD [stxt] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[ERSkill]', N'ltxt') IS NULL ALTER TABLE [ERSkill] ADD [ltxt] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[ERSkill]', N'entityid') IS NULL ALTER TABLE [ERSkill] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[ERSkill]', N'clientid') IS NULL ALTER TABLE [ERSkill] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[ERSkill]', N'skillratingid') IS NULL ALTER TABLE [ERSkill] ADD [skillratingid] int NULL;
GO
IF COL_LENGTH(N'[ERSkill]', N'utblock') IS NULL ALTER TABLE [ERSkill] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[ERSkill]', N'utaddappversion') IS NULL ALTER TABLE [ERSkill] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ERSkill]', N'utadddbversion') IS NULL ALTER TABLE [ERSkill] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ERSkill]', N'utaddid') IS NULL ALTER TABLE [ERSkill] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[ERSkill]', N'utaddip') IS NULL ALTER TABLE [ERSkill] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERSkill]', N'utaddsessionid') IS NULL ALTER TABLE [ERSkill] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[ERSkill]', N'utaddsystem') IS NULL ALTER TABLE [ERSkill] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERSkill]', N'utaddtime') IS NULL ALTER TABLE [ERSkill] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ERSkill]', N'utadduser') IS NULL ALTER TABLE [ERSkill] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERSkill]', N'utdoctype') IS NULL ALTER TABLE [ERSkill] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[ERSkill]', N'uteditappversion') IS NULL ALTER TABLE [ERSkill] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ERSkill]', N'uteditdbversion') IS NULL ALTER TABLE [ERSkill] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ERSkill]', N'uteditid') IS NULL ALTER TABLE [ERSkill] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[ERSkill]', N'uteditip') IS NULL ALTER TABLE [ERSkill] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERSkill]', N'uteditsessionid') IS NULL ALTER TABLE [ERSkill] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[ERSkill]', N'uteditsystem') IS NULL ALTER TABLE [ERSkill] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERSkill]', N'utedittime') IS NULL ALTER TABLE [ERSkill] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ERSkill]', N'utedituser') IS NULL ALTER TABLE [ERSkill] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERSkill]', N'utispartial') IS NULL ALTER TABLE [ERSkill] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[ERSkill]', N'utrecordversion') IS NULL ALTER TABLE [ERSkill] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[ERSkill]', N'utsubdoctype') IS NULL ALTER TABLE [ERSkill] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[ERSkill]', N'utsysgen') IS NULL ALTER TABLE [ERSkill] ADD [utsysgen] bit NULL;
GO
IF OBJECT_ID(N'[ERSkillratinghdr]', N'U') IS NULL BEGIN CREATE TABLE [ERSkillratinghdr] (
    [id] int NOT NULL,
    [code] nvarchar(10) NULL,
    [stxt] nvarchar(150) NULL,
    [entityid] int NULL,
    [clientid] int NULL,
    [utblock] bit NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    CONSTRAINT [PK_ERSkillratinghdr] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[ERSkillratinghdr]', N'id') IS NULL ALTER TABLE [ERSkillratinghdr] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[ERSkillratinghdr]', N'code') IS NULL ALTER TABLE [ERSkillratinghdr] ADD [code] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[ERSkillratinghdr]', N'stxt') IS NULL ALTER TABLE [ERSkillratinghdr] ADD [stxt] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[ERSkillratinghdr]', N'entityid') IS NULL ALTER TABLE [ERSkillratinghdr] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[ERSkillratinghdr]', N'clientid') IS NULL ALTER TABLE [ERSkillratinghdr] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[ERSkillratinghdr]', N'utblock') IS NULL ALTER TABLE [ERSkillratinghdr] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[ERSkillratinghdr]', N'utaddappversion') IS NULL ALTER TABLE [ERSkillratinghdr] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ERSkillratinghdr]', N'utadddbversion') IS NULL ALTER TABLE [ERSkillratinghdr] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ERSkillratinghdr]', N'utaddid') IS NULL ALTER TABLE [ERSkillratinghdr] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[ERSkillratinghdr]', N'utaddip') IS NULL ALTER TABLE [ERSkillratinghdr] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERSkillratinghdr]', N'utaddsessionid') IS NULL ALTER TABLE [ERSkillratinghdr] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[ERSkillratinghdr]', N'utaddsystem') IS NULL ALTER TABLE [ERSkillratinghdr] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERSkillratinghdr]', N'utaddtime') IS NULL ALTER TABLE [ERSkillratinghdr] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ERSkillratinghdr]', N'utadduser') IS NULL ALTER TABLE [ERSkillratinghdr] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERSkillratinghdr]', N'utdoctype') IS NULL ALTER TABLE [ERSkillratinghdr] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[ERSkillratinghdr]', N'uteditappversion') IS NULL ALTER TABLE [ERSkillratinghdr] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ERSkillratinghdr]', N'uteditdbversion') IS NULL ALTER TABLE [ERSkillratinghdr] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ERSkillratinghdr]', N'uteditid') IS NULL ALTER TABLE [ERSkillratinghdr] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[ERSkillratinghdr]', N'uteditip') IS NULL ALTER TABLE [ERSkillratinghdr] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERSkillratinghdr]', N'uteditsessionid') IS NULL ALTER TABLE [ERSkillratinghdr] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[ERSkillratinghdr]', N'uteditsystem') IS NULL ALTER TABLE [ERSkillratinghdr] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERSkillratinghdr]', N'utedittime') IS NULL ALTER TABLE [ERSkillratinghdr] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ERSkillratinghdr]', N'utedituser') IS NULL ALTER TABLE [ERSkillratinghdr] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERSkillratinghdr]', N'utispartial') IS NULL ALTER TABLE [ERSkillratinghdr] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[ERSkillratinghdr]', N'utrecordversion') IS NULL ALTER TABLE [ERSkillratinghdr] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[ERSkillratinghdr]', N'utsubdoctype') IS NULL ALTER TABLE [ERSkillratinghdr] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[ERSkillratinghdr]', N'utsysgen') IS NULL ALTER TABLE [ERSkillratinghdr] ADD [utsysgen] bit NULL;
GO
IF OBJECT_ID(N'[ERSkilltype]', N'U') IS NULL BEGIN CREATE TABLE [ERSkilltype] (
    [id] int NOT NULL,
    [code] nvarchar(10) NULL,
    [stxt] nvarchar(150) NULL,
    [ltxt] nvarchar(500) NULL,
    [entityid] int NULL,
    [clientid] int NULL,
    [utblock] bit NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    CONSTRAINT [PK_ERSkilltype] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[ERSkilltype]', N'id') IS NULL ALTER TABLE [ERSkilltype] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[ERSkilltype]', N'code') IS NULL ALTER TABLE [ERSkilltype] ADD [code] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[ERSkilltype]', N'stxt') IS NULL ALTER TABLE [ERSkilltype] ADD [stxt] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[ERSkilltype]', N'ltxt') IS NULL ALTER TABLE [ERSkilltype] ADD [ltxt] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[ERSkilltype]', N'entityid') IS NULL ALTER TABLE [ERSkilltype] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[ERSkilltype]', N'clientid') IS NULL ALTER TABLE [ERSkilltype] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[ERSkilltype]', N'utblock') IS NULL ALTER TABLE [ERSkilltype] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[ERSkilltype]', N'utaddappversion') IS NULL ALTER TABLE [ERSkilltype] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ERSkilltype]', N'utadddbversion') IS NULL ALTER TABLE [ERSkilltype] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ERSkilltype]', N'utaddid') IS NULL ALTER TABLE [ERSkilltype] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[ERSkilltype]', N'utaddip') IS NULL ALTER TABLE [ERSkilltype] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERSkilltype]', N'utaddsessionid') IS NULL ALTER TABLE [ERSkilltype] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[ERSkilltype]', N'utaddsystem') IS NULL ALTER TABLE [ERSkilltype] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERSkilltype]', N'utaddtime') IS NULL ALTER TABLE [ERSkilltype] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ERSkilltype]', N'utadduser') IS NULL ALTER TABLE [ERSkilltype] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERSkilltype]', N'utdoctype') IS NULL ALTER TABLE [ERSkilltype] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[ERSkilltype]', N'uteditappversion') IS NULL ALTER TABLE [ERSkilltype] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ERSkilltype]', N'uteditdbversion') IS NULL ALTER TABLE [ERSkilltype] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ERSkilltype]', N'uteditid') IS NULL ALTER TABLE [ERSkilltype] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[ERSkilltype]', N'uteditip') IS NULL ALTER TABLE [ERSkilltype] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERSkilltype]', N'uteditsessionid') IS NULL ALTER TABLE [ERSkilltype] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[ERSkilltype]', N'uteditsystem') IS NULL ALTER TABLE [ERSkilltype] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERSkilltype]', N'utedittime') IS NULL ALTER TABLE [ERSkilltype] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ERSkilltype]', N'utedituser') IS NULL ALTER TABLE [ERSkilltype] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERSkilltype]', N'utispartial') IS NULL ALTER TABLE [ERSkilltype] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[ERSkilltype]', N'utrecordversion') IS NULL ALTER TABLE [ERSkilltype] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[ERSkilltype]', N'utsubdoctype') IS NULL ALTER TABLE [ERSkilltype] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[ERSkilltype]', N'utsysgen') IS NULL ALTER TABLE [ERSkilltype] ADD [utsysgen] bit NULL;
GO
IF OBJECT_ID(N'[ETCalholidayhdr]', N'U') IS NULL BEGIN CREATE TABLE [ETCalholidayhdr] (
    [id] int NOT NULL,
    [code] nvarchar(20) NULL,
    [stxt] nvarchar(150) NULL,
    [payperiodid] int NULL,
    [applyonid] int NULL,
    [lovsharedid] int NULL,
    [entityid] int NULL,
    [clientid] int NULL,
    [utblock] bit NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    CONSTRAINT [PK_ETCalholidayhdr] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[ETCalholidayhdr]', N'id') IS NULL ALTER TABLE [ETCalholidayhdr] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidayhdr]', N'code') IS NULL ALTER TABLE [ETCalholidayhdr] ADD [code] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[ETCalholidayhdr]', N'stxt') IS NULL ALTER TABLE [ETCalholidayhdr] ADD [stxt] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[ETCalholidayhdr]', N'payperiodid') IS NULL ALTER TABLE [ETCalholidayhdr] ADD [payperiodid] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidayhdr]', N'applyonid') IS NULL ALTER TABLE [ETCalholidayhdr] ADD [applyonid] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidayhdr]', N'lovsharedid') IS NULL ALTER TABLE [ETCalholidayhdr] ADD [lovsharedid] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidayhdr]', N'entityid') IS NULL ALTER TABLE [ETCalholidayhdr] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidayhdr]', N'clientid') IS NULL ALTER TABLE [ETCalholidayhdr] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidayhdr]', N'utblock') IS NULL ALTER TABLE [ETCalholidayhdr] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[ETCalholidayhdr]', N'utaddappversion') IS NULL ALTER TABLE [ETCalholidayhdr] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETCalholidayhdr]', N'utadddbversion') IS NULL ALTER TABLE [ETCalholidayhdr] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETCalholidayhdr]', N'utaddid') IS NULL ALTER TABLE [ETCalholidayhdr] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[ETCalholidayhdr]', N'utaddip') IS NULL ALTER TABLE [ETCalholidayhdr] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETCalholidayhdr]', N'utaddsessionid') IS NULL ALTER TABLE [ETCalholidayhdr] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidayhdr]', N'utaddsystem') IS NULL ALTER TABLE [ETCalholidayhdr] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETCalholidayhdr]', N'utaddtime') IS NULL ALTER TABLE [ETCalholidayhdr] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETCalholidayhdr]', N'utadduser') IS NULL ALTER TABLE [ETCalholidayhdr] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETCalholidayhdr]', N'utdoctype') IS NULL ALTER TABLE [ETCalholidayhdr] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidayhdr]', N'uteditappversion') IS NULL ALTER TABLE [ETCalholidayhdr] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETCalholidayhdr]', N'uteditdbversion') IS NULL ALTER TABLE [ETCalholidayhdr] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETCalholidayhdr]', N'uteditid') IS NULL ALTER TABLE [ETCalholidayhdr] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[ETCalholidayhdr]', N'uteditip') IS NULL ALTER TABLE [ETCalholidayhdr] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETCalholidayhdr]', N'uteditsessionid') IS NULL ALTER TABLE [ETCalholidayhdr] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidayhdr]', N'uteditsystem') IS NULL ALTER TABLE [ETCalholidayhdr] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETCalholidayhdr]', N'utedittime') IS NULL ALTER TABLE [ETCalholidayhdr] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETCalholidayhdr]', N'utedituser') IS NULL ALTER TABLE [ETCalholidayhdr] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETCalholidayhdr]', N'utispartial') IS NULL ALTER TABLE [ETCalholidayhdr] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[ETCalholidayhdr]', N'utrecordversion') IS NULL ALTER TABLE [ETCalholidayhdr] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidayhdr]', N'utsubdoctype') IS NULL ALTER TABLE [ETCalholidayhdr] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidayhdr]', N'utsysgen') IS NULL ALTER TABLE [ETCalholidayhdr] ADD [utsysgen] bit NULL;
GO
IF OBJECT_ID(N'[ETHolidaytype]', N'U') IS NULL BEGIN CREATE TABLE [ETHolidaytype] (
    [Id] int NOT NULL,
    [code] nvarchar(20) NULL,
    [stxt] nvarchar(150) NULL,
    [ltxt] nvarchar(500) NULL,
    [colorcode] nvarchar(50) NULL,
    [entityid] int NULL,
    [clientid] int NULL,
    [utblock] bit NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    CONSTRAINT [PK_ETHolidaytype] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[ETHolidaytype]', N'Id') IS NULL ALTER TABLE [ETHolidaytype] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[ETHolidaytype]', N'code') IS NULL ALTER TABLE [ETHolidaytype] ADD [code] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[ETHolidaytype]', N'stxt') IS NULL ALTER TABLE [ETHolidaytype] ADD [stxt] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[ETHolidaytype]', N'ltxt') IS NULL ALTER TABLE [ETHolidaytype] ADD [ltxt] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[ETHolidaytype]', N'colorcode') IS NULL ALTER TABLE [ETHolidaytype] ADD [colorcode] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[ETHolidaytype]', N'entityid') IS NULL ALTER TABLE [ETHolidaytype] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[ETHolidaytype]', N'clientid') IS NULL ALTER TABLE [ETHolidaytype] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[ETHolidaytype]', N'utblock') IS NULL ALTER TABLE [ETHolidaytype] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[ETHolidaytype]', N'utaddappversion') IS NULL ALTER TABLE [ETHolidaytype] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETHolidaytype]', N'utadddbversion') IS NULL ALTER TABLE [ETHolidaytype] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETHolidaytype]', N'utaddid') IS NULL ALTER TABLE [ETHolidaytype] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[ETHolidaytype]', N'utaddip') IS NULL ALTER TABLE [ETHolidaytype] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETHolidaytype]', N'utaddsessionid') IS NULL ALTER TABLE [ETHolidaytype] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETHolidaytype]', N'utaddsystem') IS NULL ALTER TABLE [ETHolidaytype] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETHolidaytype]', N'utaddtime') IS NULL ALTER TABLE [ETHolidaytype] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETHolidaytype]', N'utadduser') IS NULL ALTER TABLE [ETHolidaytype] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETHolidaytype]', N'utdoctype') IS NULL ALTER TABLE [ETHolidaytype] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[ETHolidaytype]', N'uteditappversion') IS NULL ALTER TABLE [ETHolidaytype] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETHolidaytype]', N'uteditdbversion') IS NULL ALTER TABLE [ETHolidaytype] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETHolidaytype]', N'uteditid') IS NULL ALTER TABLE [ETHolidaytype] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[ETHolidaytype]', N'uteditip') IS NULL ALTER TABLE [ETHolidaytype] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETHolidaytype]', N'uteditsessionid') IS NULL ALTER TABLE [ETHolidaytype] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETHolidaytype]', N'uteditsystem') IS NULL ALTER TABLE [ETHolidaytype] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETHolidaytype]', N'utedittime') IS NULL ALTER TABLE [ETHolidaytype] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETHolidaytype]', N'utedituser') IS NULL ALTER TABLE [ETHolidaytype] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETHolidaytype]', N'utispartial') IS NULL ALTER TABLE [ETHolidaytype] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[ETHolidaytype]', N'utrecordversion') IS NULL ALTER TABLE [ETHolidaytype] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[ETHolidaytype]', N'utsubdoctype') IS NULL ALTER TABLE [ETHolidaytype] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[ETHolidaytype]', N'utsysgen') IS NULL ALTER TABLE [ETHolidaytype] ADD [utsysgen] bit NULL;
GO
IF OBJECT_ID(N'[ETLeaveadjpolicy]', N'U') IS NULL BEGIN CREATE TABLE [ETLeaveadjpolicy] (
    [id] int NOT NULL,
    [code] nvarchar(10) NULL,
    [stxt] nvarchar(100) NULL,
    [entityid] int NULL,
    [clientid] int NULL,
    [utblock] bit NOT NULL,
    [utsysgen] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utispartial] bit NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_ETLeaveadjpolicy] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicy]', N'id') IS NULL ALTER TABLE [ETLeaveadjpolicy] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicy]', N'code') IS NULL ALTER TABLE [ETLeaveadjpolicy] ADD [code] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicy]', N'stxt') IS NULL ALTER TABLE [ETLeaveadjpolicy] ADD [stxt] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicy]', N'entityid') IS NULL ALTER TABLE [ETLeaveadjpolicy] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicy]', N'clientid') IS NULL ALTER TABLE [ETLeaveadjpolicy] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicy]', N'utblock') IS NULL ALTER TABLE [ETLeaveadjpolicy] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicy]', N'utsysgen') IS NULL ALTER TABLE [ETLeaveadjpolicy] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicy]', N'utrecordversion') IS NULL ALTER TABLE [ETLeaveadjpolicy] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicy]', N'utispartial') IS NULL ALTER TABLE [ETLeaveadjpolicy] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicy]', N'utaddappversion') IS NULL ALTER TABLE [ETLeaveadjpolicy] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicy]', N'utadddbversion') IS NULL ALTER TABLE [ETLeaveadjpolicy] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicy]', N'utaddid') IS NULL ALTER TABLE [ETLeaveadjpolicy] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicy]', N'utaddip') IS NULL ALTER TABLE [ETLeaveadjpolicy] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicy]', N'utaddsessionid') IS NULL ALTER TABLE [ETLeaveadjpolicy] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicy]', N'utaddsystem') IS NULL ALTER TABLE [ETLeaveadjpolicy] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicy]', N'utaddtime') IS NULL ALTER TABLE [ETLeaveadjpolicy] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicy]', N'utadduser') IS NULL ALTER TABLE [ETLeaveadjpolicy] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicy]', N'utdoctype') IS NULL ALTER TABLE [ETLeaveadjpolicy] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicy]', N'uteditappversion') IS NULL ALTER TABLE [ETLeaveadjpolicy] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicy]', N'uteditdbversion') IS NULL ALTER TABLE [ETLeaveadjpolicy] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicy]', N'uteditid') IS NULL ALTER TABLE [ETLeaveadjpolicy] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicy]', N'uteditip') IS NULL ALTER TABLE [ETLeaveadjpolicy] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicy]', N'uteditsessionid') IS NULL ALTER TABLE [ETLeaveadjpolicy] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicy]', N'uteditsystem') IS NULL ALTER TABLE [ETLeaveadjpolicy] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicy]', N'utedittime') IS NULL ALTER TABLE [ETLeaveadjpolicy] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicy]', N'utedituser') IS NULL ALTER TABLE [ETLeaveadjpolicy] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicy]', N'utsubdoctype') IS NULL ALTER TABLE [ETLeaveadjpolicy] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[ETLeavegrouphdr]', N'U') IS NULL BEGIN CREATE TABLE [ETLeavegrouphdr] (
    [Id] int NOT NULL,
    [code] nvarchar(20) NULL,
    [stxt] nvarchar(150) NULL,
    [adjpolicyid] int NULL,
    [entityid] int NULL,
    [clientid] int NULL,
    [utblock] bit NULL,
    [utsysgen] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_ETLeavegrouphdr] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[ETLeavegrouphdr]', N'Id') IS NULL ALTER TABLE [ETLeavegrouphdr] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[ETLeavegrouphdr]', N'code') IS NULL ALTER TABLE [ETLeavegrouphdr] ADD [code] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[ETLeavegrouphdr]', N'stxt') IS NULL ALTER TABLE [ETLeavegrouphdr] ADD [stxt] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[ETLeavegrouphdr]', N'adjpolicyid') IS NULL ALTER TABLE [ETLeavegrouphdr] ADD [adjpolicyid] int NULL;
GO
IF COL_LENGTH(N'[ETLeavegrouphdr]', N'entityid') IS NULL ALTER TABLE [ETLeavegrouphdr] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[ETLeavegrouphdr]', N'clientid') IS NULL ALTER TABLE [ETLeavegrouphdr] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[ETLeavegrouphdr]', N'utblock') IS NULL ALTER TABLE [ETLeavegrouphdr] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[ETLeavegrouphdr]', N'utsysgen') IS NULL ALTER TABLE [ETLeavegrouphdr] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[ETLeavegrouphdr]', N'utispartial') IS NULL ALTER TABLE [ETLeavegrouphdr] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[ETLeavegrouphdr]', N'utrecordversion') IS NULL ALTER TABLE [ETLeavegrouphdr] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[ETLeavegrouphdr]', N'utaddappversion') IS NULL ALTER TABLE [ETLeavegrouphdr] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETLeavegrouphdr]', N'utadddbversion') IS NULL ALTER TABLE [ETLeavegrouphdr] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETLeavegrouphdr]', N'utaddid') IS NULL ALTER TABLE [ETLeavegrouphdr] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[ETLeavegrouphdr]', N'utaddip') IS NULL ALTER TABLE [ETLeavegrouphdr] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETLeavegrouphdr]', N'utaddsessionid') IS NULL ALTER TABLE [ETLeavegrouphdr] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETLeavegrouphdr]', N'utaddsystem') IS NULL ALTER TABLE [ETLeavegrouphdr] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETLeavegrouphdr]', N'utaddtime') IS NULL ALTER TABLE [ETLeavegrouphdr] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETLeavegrouphdr]', N'utadduser') IS NULL ALTER TABLE [ETLeavegrouphdr] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETLeavegrouphdr]', N'utdoctype') IS NULL ALTER TABLE [ETLeavegrouphdr] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[ETLeavegrouphdr]', N'uteditappversion') IS NULL ALTER TABLE [ETLeavegrouphdr] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETLeavegrouphdr]', N'uteditdbversion') IS NULL ALTER TABLE [ETLeavegrouphdr] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETLeavegrouphdr]', N'uteditid') IS NULL ALTER TABLE [ETLeavegrouphdr] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[ETLeavegrouphdr]', N'uteditip') IS NULL ALTER TABLE [ETLeavegrouphdr] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETLeavegrouphdr]', N'uteditsessionid') IS NULL ALTER TABLE [ETLeavegrouphdr] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETLeavegrouphdr]', N'uteditsystem') IS NULL ALTER TABLE [ETLeavegrouphdr] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETLeavegrouphdr]', N'utedittime') IS NULL ALTER TABLE [ETLeavegrouphdr] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETLeavegrouphdr]', N'utedituser') IS NULL ALTER TABLE [ETLeavegrouphdr] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETLeavegrouphdr]', N'utsubdoctype') IS NULL ALTER TABLE [ETLeavegrouphdr] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[ETLeavetype]', N'U') IS NULL BEGIN CREATE TABLE [ETLeavetype] (
    [id] int NOT NULL,
    [code] nvarchar(10) NULL,
    [stxt] nvarchar(100) NULL,
    [entityid] int NULL,
    [clientid] int NULL,
    [leaves] float NULL,
    [renewonid] int NULL,
    [lvunitid] int NULL,
    [availunitid] int NULL,
    [maxavail] int NULL,
    [maritalid] int NULL,
    [carrylv] float NULL,
    [carrylvid] int NULL,
    [encash] float NULL,
    [encashid] int NULL,
    [genderid] int NULL,
    [entitleid] int NULL,
    [accrualunitid] int NULL,
    [accrualleaves] float NULL,
    [allowinprob] bit NULL,
    [quotavalidate] bit NULL,
    [lateadjust] bit NULL,
    [earlydepadjust] bit NULL,
    [halfadjust] bit NULL,
    [incholidays] bit NULL,
    [exclweeklyholidays] bit NULL,
    [ispaid] bit NULL,
    [requestbefdays] int NULL,
    [requestunitid] int NULL,
    [nom] int NULL,
    [utsysgen] bit NULL,
    [utblock] bit NULL,
    [utispartial] bit NULL,
    [utrecordversion] int NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_ETLeavetype] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'id') IS NULL ALTER TABLE [ETLeavetype] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'code') IS NULL ALTER TABLE [ETLeavetype] ADD [code] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'stxt') IS NULL ALTER TABLE [ETLeavetype] ADD [stxt] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'entityid') IS NULL ALTER TABLE [ETLeavetype] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'clientid') IS NULL ALTER TABLE [ETLeavetype] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'leaves') IS NULL ALTER TABLE [ETLeavetype] ADD [leaves] float NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'renewonid') IS NULL ALTER TABLE [ETLeavetype] ADD [renewonid] int NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'lvunitid') IS NULL ALTER TABLE [ETLeavetype] ADD [lvunitid] int NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'availunitid') IS NULL ALTER TABLE [ETLeavetype] ADD [availunitid] int NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'maxavail') IS NULL ALTER TABLE [ETLeavetype] ADD [maxavail] int NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'maritalid') IS NULL ALTER TABLE [ETLeavetype] ADD [maritalid] int NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'carrylv') IS NULL ALTER TABLE [ETLeavetype] ADD [carrylv] float NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'carrylvid') IS NULL ALTER TABLE [ETLeavetype] ADD [carrylvid] int NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'encash') IS NULL ALTER TABLE [ETLeavetype] ADD [encash] float NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'encashid') IS NULL ALTER TABLE [ETLeavetype] ADD [encashid] int NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'genderid') IS NULL ALTER TABLE [ETLeavetype] ADD [genderid] int NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'entitleid') IS NULL ALTER TABLE [ETLeavetype] ADD [entitleid] int NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'accrualunitid') IS NULL ALTER TABLE [ETLeavetype] ADD [accrualunitid] int NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'accrualleaves') IS NULL ALTER TABLE [ETLeavetype] ADD [accrualleaves] float NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'allowinprob') IS NULL ALTER TABLE [ETLeavetype] ADD [allowinprob] bit NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'quotavalidate') IS NULL ALTER TABLE [ETLeavetype] ADD [quotavalidate] bit NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'lateadjust') IS NULL ALTER TABLE [ETLeavetype] ADD [lateadjust] bit NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'earlydepadjust') IS NULL ALTER TABLE [ETLeavetype] ADD [earlydepadjust] bit NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'halfadjust') IS NULL ALTER TABLE [ETLeavetype] ADD [halfadjust] bit NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'incholidays') IS NULL ALTER TABLE [ETLeavetype] ADD [incholidays] bit NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'exclweeklyholidays') IS NULL ALTER TABLE [ETLeavetype] ADD [exclweeklyholidays] bit NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'ispaid') IS NULL ALTER TABLE [ETLeavetype] ADD [ispaid] bit NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'requestbefdays') IS NULL ALTER TABLE [ETLeavetype] ADD [requestbefdays] int NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'requestunitid') IS NULL ALTER TABLE [ETLeavetype] ADD [requestunitid] int NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'nom') IS NULL ALTER TABLE [ETLeavetype] ADD [nom] int NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'utsysgen') IS NULL ALTER TABLE [ETLeavetype] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'utblock') IS NULL ALTER TABLE [ETLeavetype] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'utispartial') IS NULL ALTER TABLE [ETLeavetype] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'utrecordversion') IS NULL ALTER TABLE [ETLeavetype] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'utaddappversion') IS NULL ALTER TABLE [ETLeavetype] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'utadddbversion') IS NULL ALTER TABLE [ETLeavetype] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'utaddid') IS NULL ALTER TABLE [ETLeavetype] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'utaddip') IS NULL ALTER TABLE [ETLeavetype] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'utaddsessionid') IS NULL ALTER TABLE [ETLeavetype] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'utaddsystem') IS NULL ALTER TABLE [ETLeavetype] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'utaddtime') IS NULL ALTER TABLE [ETLeavetype] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'utadduser') IS NULL ALTER TABLE [ETLeavetype] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'utdoctype') IS NULL ALTER TABLE [ETLeavetype] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'uteditappversion') IS NULL ALTER TABLE [ETLeavetype] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'uteditdbversion') IS NULL ALTER TABLE [ETLeavetype] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'uteditid') IS NULL ALTER TABLE [ETLeavetype] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'uteditip') IS NULL ALTER TABLE [ETLeavetype] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'uteditsessionid') IS NULL ALTER TABLE [ETLeavetype] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'uteditsystem') IS NULL ALTER TABLE [ETLeavetype] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'utedittime') IS NULL ALTER TABLE [ETLeavetype] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'utedituser') IS NULL ALTER TABLE [ETLeavetype] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETLeavetype]', N'utsubdoctype') IS NULL ALTER TABLE [ETLeavetype] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[ETPayperiodhdr]', N'U') IS NULL BEGIN CREATE TABLE [ETPayperiodhdr] (
    [id] int NOT NULL,
    [code] nvarchar(50) NULL,
    [date] datetime2 NULL,
    [atntypeid] int NULL,
    [fromyear] datetime2 NULL,
    [toyear] datetime2 NULL,
    [ltxt] nvarchar(250) NULL,
    [entityid] int NULL,
    [clientid] int NULL,
    [startfrom] int NULL,
    [statusid] int NULL,
    [lovsharedid] int NULL,
    [startdate] datetime2 NULL,
    [payrollregisterid] int NULL,
    [noofperiod] int NULL,
    [utsysgen] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utblock] bit NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_ETPayperiodhdr] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[ETPayperiodhdr]', N'id') IS NULL ALTER TABLE [ETPayperiodhdr] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[ETPayperiodhdr]', N'code') IS NULL ALTER TABLE [ETPayperiodhdr] ADD [code] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[ETPayperiodhdr]', N'date') IS NULL ALTER TABLE [ETPayperiodhdr] ADD [date] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETPayperiodhdr]', N'atntypeid') IS NULL ALTER TABLE [ETPayperiodhdr] ADD [atntypeid] int NULL;
GO
IF COL_LENGTH(N'[ETPayperiodhdr]', N'fromyear') IS NULL ALTER TABLE [ETPayperiodhdr] ADD [fromyear] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETPayperiodhdr]', N'toyear') IS NULL ALTER TABLE [ETPayperiodhdr] ADD [toyear] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETPayperiodhdr]', N'ltxt') IS NULL ALTER TABLE [ETPayperiodhdr] ADD [ltxt] nvarchar(250) NULL;
GO
IF COL_LENGTH(N'[ETPayperiodhdr]', N'entityid') IS NULL ALTER TABLE [ETPayperiodhdr] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[ETPayperiodhdr]', N'clientid') IS NULL ALTER TABLE [ETPayperiodhdr] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[ETPayperiodhdr]', N'startfrom') IS NULL ALTER TABLE [ETPayperiodhdr] ADD [startfrom] int NULL;
GO
IF COL_LENGTH(N'[ETPayperiodhdr]', N'statusid') IS NULL ALTER TABLE [ETPayperiodhdr] ADD [statusid] int NULL;
GO
IF COL_LENGTH(N'[ETPayperiodhdr]', N'lovsharedid') IS NULL ALTER TABLE [ETPayperiodhdr] ADD [lovsharedid] int NULL;
GO
IF COL_LENGTH(N'[ETPayperiodhdr]', N'startdate') IS NULL ALTER TABLE [ETPayperiodhdr] ADD [startdate] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETPayperiodhdr]', N'payrollregisterid') IS NULL ALTER TABLE [ETPayperiodhdr] ADD [payrollregisterid] int NULL;
GO
IF COL_LENGTH(N'[ETPayperiodhdr]', N'noofperiod') IS NULL ALTER TABLE [ETPayperiodhdr] ADD [noofperiod] int NULL;
GO
IF COL_LENGTH(N'[ETPayperiodhdr]', N'utsysgen') IS NULL ALTER TABLE [ETPayperiodhdr] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[ETPayperiodhdr]', N'utispartial') IS NULL ALTER TABLE [ETPayperiodhdr] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[ETPayperiodhdr]', N'utrecordversion') IS NULL ALTER TABLE [ETPayperiodhdr] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[ETPayperiodhdr]', N'utblock') IS NULL ALTER TABLE [ETPayperiodhdr] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[ETPayperiodhdr]', N'utaddappversion') IS NULL ALTER TABLE [ETPayperiodhdr] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETPayperiodhdr]', N'utadddbversion') IS NULL ALTER TABLE [ETPayperiodhdr] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETPayperiodhdr]', N'utaddid') IS NULL ALTER TABLE [ETPayperiodhdr] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[ETPayperiodhdr]', N'utaddip') IS NULL ALTER TABLE [ETPayperiodhdr] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETPayperiodhdr]', N'utaddsessionid') IS NULL ALTER TABLE [ETPayperiodhdr] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETPayperiodhdr]', N'utaddsystem') IS NULL ALTER TABLE [ETPayperiodhdr] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETPayperiodhdr]', N'utaddtime') IS NULL ALTER TABLE [ETPayperiodhdr] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETPayperiodhdr]', N'utadduser') IS NULL ALTER TABLE [ETPayperiodhdr] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETPayperiodhdr]', N'utdoctype') IS NULL ALTER TABLE [ETPayperiodhdr] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[ETPayperiodhdr]', N'uteditappversion') IS NULL ALTER TABLE [ETPayperiodhdr] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETPayperiodhdr]', N'uteditdbversion') IS NULL ALTER TABLE [ETPayperiodhdr] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETPayperiodhdr]', N'uteditid') IS NULL ALTER TABLE [ETPayperiodhdr] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[ETPayperiodhdr]', N'uteditip') IS NULL ALTER TABLE [ETPayperiodhdr] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETPayperiodhdr]', N'uteditsessionid') IS NULL ALTER TABLE [ETPayperiodhdr] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETPayperiodhdr]', N'uteditsystem') IS NULL ALTER TABLE [ETPayperiodhdr] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETPayperiodhdr]', N'utedittime') IS NULL ALTER TABLE [ETPayperiodhdr] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETPayperiodhdr]', N'utedituser') IS NULL ALTER TABLE [ETPayperiodhdr] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETPayperiodhdr]', N'utsubdoctype') IS NULL ALTER TABLE [ETPayperiodhdr] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[ETPayrollregister]', N'U') IS NULL BEGIN CREATE TABLE [ETPayrollregister] (
    [id] int NOT NULL,
    [code] nvarchar(50) NULL,
    [stxt] nvarchar(250) NULL,
    [ltxt] nvarchar(500) NULL,
    [clientid] int NULL,
    [utblock] bit NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    CONSTRAINT [PK_ETPayrollregister] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[ETPayrollregister]', N'id') IS NULL ALTER TABLE [ETPayrollregister] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[ETPayrollregister]', N'code') IS NULL ALTER TABLE [ETPayrollregister] ADD [code] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[ETPayrollregister]', N'stxt') IS NULL ALTER TABLE [ETPayrollregister] ADD [stxt] nvarchar(250) NULL;
GO
IF COL_LENGTH(N'[ETPayrollregister]', N'ltxt') IS NULL ALTER TABLE [ETPayrollregister] ADD [ltxt] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[ETPayrollregister]', N'clientid') IS NULL ALTER TABLE [ETPayrollregister] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[ETPayrollregister]', N'utblock') IS NULL ALTER TABLE [ETPayrollregister] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[ETPayrollregister]', N'utaddappversion') IS NULL ALTER TABLE [ETPayrollregister] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETPayrollregister]', N'utadddbversion') IS NULL ALTER TABLE [ETPayrollregister] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETPayrollregister]', N'utaddid') IS NULL ALTER TABLE [ETPayrollregister] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[ETPayrollregister]', N'utaddip') IS NULL ALTER TABLE [ETPayrollregister] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETPayrollregister]', N'utaddsessionid') IS NULL ALTER TABLE [ETPayrollregister] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETPayrollregister]', N'utaddsystem') IS NULL ALTER TABLE [ETPayrollregister] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETPayrollregister]', N'utaddtime') IS NULL ALTER TABLE [ETPayrollregister] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETPayrollregister]', N'utadduser') IS NULL ALTER TABLE [ETPayrollregister] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETPayrollregister]', N'utdoctype') IS NULL ALTER TABLE [ETPayrollregister] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[ETPayrollregister]', N'uteditappversion') IS NULL ALTER TABLE [ETPayrollregister] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETPayrollregister]', N'uteditdbversion') IS NULL ALTER TABLE [ETPayrollregister] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETPayrollregister]', N'uteditid') IS NULL ALTER TABLE [ETPayrollregister] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[ETPayrollregister]', N'uteditip') IS NULL ALTER TABLE [ETPayrollregister] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETPayrollregister]', N'uteditsessionid') IS NULL ALTER TABLE [ETPayrollregister] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETPayrollregister]', N'uteditsystem') IS NULL ALTER TABLE [ETPayrollregister] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETPayrollregister]', N'utedittime') IS NULL ALTER TABLE [ETPayrollregister] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETPayrollregister]', N'utedituser') IS NULL ALTER TABLE [ETPayrollregister] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETPayrollregister]', N'utispartial') IS NULL ALTER TABLE [ETPayrollregister] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[ETPayrollregister]', N'utrecordversion') IS NULL ALTER TABLE [ETPayrollregister] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[ETPayrollregister]', N'utsubdoctype') IS NULL ALTER TABLE [ETPayrollregister] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[ETPayrollregister]', N'utsysgen') IS NULL ALTER TABLE [ETPayrollregister] ADD [utsysgen] bit NULL;
GO
IF OBJECT_ID(N'[ETPolicyhdr]', N'U') IS NULL BEGIN CREATE TABLE [ETPolicyhdr] (
    [id] int NOT NULL,
    [code] nvarchar(50) NULL,
    [stxt] nvarchar(200) NULL,
    [policytypeid] int NULL,
    [applyoffdays] bit NULL,
    [noofexcuse] int NULL,
    [entityid] int NULL,
    [clientid] int NULL,
    [utsysgen] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utispartial] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_ETPolicyhdr] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[ETPolicyhdr]', N'id') IS NULL ALTER TABLE [ETPolicyhdr] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[ETPolicyhdr]', N'code') IS NULL ALTER TABLE [ETPolicyhdr] ADD [code] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[ETPolicyhdr]', N'stxt') IS NULL ALTER TABLE [ETPolicyhdr] ADD [stxt] nvarchar(200) NULL;
GO
IF COL_LENGTH(N'[ETPolicyhdr]', N'policytypeid') IS NULL ALTER TABLE [ETPolicyhdr] ADD [policytypeid] int NULL;
GO
IF COL_LENGTH(N'[ETPolicyhdr]', N'applyoffdays') IS NULL ALTER TABLE [ETPolicyhdr] ADD [applyoffdays] bit NULL;
GO
IF COL_LENGTH(N'[ETPolicyhdr]', N'noofexcuse') IS NULL ALTER TABLE [ETPolicyhdr] ADD [noofexcuse] int NULL;
GO
IF COL_LENGTH(N'[ETPolicyhdr]', N'entityid') IS NULL ALTER TABLE [ETPolicyhdr] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[ETPolicyhdr]', N'clientid') IS NULL ALTER TABLE [ETPolicyhdr] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[ETPolicyhdr]', N'utsysgen') IS NULL ALTER TABLE [ETPolicyhdr] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[ETPolicyhdr]', N'utrecordversion') IS NULL ALTER TABLE [ETPolicyhdr] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[ETPolicyhdr]', N'utispartial') IS NULL ALTER TABLE [ETPolicyhdr] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[ETPolicyhdr]', N'utblock') IS NULL ALTER TABLE [ETPolicyhdr] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[ETPolicyhdr]', N'utaddappversion') IS NULL ALTER TABLE [ETPolicyhdr] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETPolicyhdr]', N'utadddbversion') IS NULL ALTER TABLE [ETPolicyhdr] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETPolicyhdr]', N'utaddid') IS NULL ALTER TABLE [ETPolicyhdr] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[ETPolicyhdr]', N'utaddip') IS NULL ALTER TABLE [ETPolicyhdr] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETPolicyhdr]', N'utaddsessionid') IS NULL ALTER TABLE [ETPolicyhdr] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETPolicyhdr]', N'utaddsystem') IS NULL ALTER TABLE [ETPolicyhdr] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETPolicyhdr]', N'utaddtime') IS NULL ALTER TABLE [ETPolicyhdr] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETPolicyhdr]', N'utadduser') IS NULL ALTER TABLE [ETPolicyhdr] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETPolicyhdr]', N'utdoctype') IS NULL ALTER TABLE [ETPolicyhdr] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[ETPolicyhdr]', N'uteditappversion') IS NULL ALTER TABLE [ETPolicyhdr] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETPolicyhdr]', N'uteditdbversion') IS NULL ALTER TABLE [ETPolicyhdr] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETPolicyhdr]', N'uteditid') IS NULL ALTER TABLE [ETPolicyhdr] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[ETPolicyhdr]', N'uteditip') IS NULL ALTER TABLE [ETPolicyhdr] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETPolicyhdr]', N'uteditsessionid') IS NULL ALTER TABLE [ETPolicyhdr] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETPolicyhdr]', N'uteditsystem') IS NULL ALTER TABLE [ETPolicyhdr] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETPolicyhdr]', N'utedittime') IS NULL ALTER TABLE [ETPolicyhdr] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETPolicyhdr]', N'utedituser') IS NULL ALTER TABLE [ETPolicyhdr] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETPolicyhdr]', N'utsubdoctype') IS NULL ALTER TABLE [ETPolicyhdr] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[ETScheduledtl]', N'U') IS NULL BEGIN CREATE TABLE [ETScheduledtl] (
    [id] int NOT NULL,
    [scheduleid] int NULL,
    [lineindex] int NULL,
    [schdate] datetime2 NULL,
    [holiday] bit NULL,
    [shiftid] int NULL,
    [timein] datetime2 NULL,
    [timeout] datetime2 NULL,
    [duration] float NULL,
    [utsysgen] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utblock] bit NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_ETScheduledtl] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[ETScheduledtl]', N'id') IS NULL ALTER TABLE [ETScheduledtl] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[ETScheduledtl]', N'scheduleid') IS NULL ALTER TABLE [ETScheduledtl] ADD [scheduleid] int NULL;
GO
IF COL_LENGTH(N'[ETScheduledtl]', N'lineindex') IS NULL ALTER TABLE [ETScheduledtl] ADD [lineindex] int NULL;
GO
IF COL_LENGTH(N'[ETScheduledtl]', N'schdate') IS NULL ALTER TABLE [ETScheduledtl] ADD [schdate] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETScheduledtl]', N'holiday') IS NULL ALTER TABLE [ETScheduledtl] ADD [holiday] bit NULL;
GO
IF COL_LENGTH(N'[ETScheduledtl]', N'shiftid') IS NULL ALTER TABLE [ETScheduledtl] ADD [shiftid] int NULL;
GO
IF COL_LENGTH(N'[ETScheduledtl]', N'timein') IS NULL ALTER TABLE [ETScheduledtl] ADD [timein] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETScheduledtl]', N'timeout') IS NULL ALTER TABLE [ETScheduledtl] ADD [timeout] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETScheduledtl]', N'duration') IS NULL ALTER TABLE [ETScheduledtl] ADD [duration] float NULL;
GO
IF COL_LENGTH(N'[ETScheduledtl]', N'utsysgen') IS NULL ALTER TABLE [ETScheduledtl] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[ETScheduledtl]', N'utispartial') IS NULL ALTER TABLE [ETScheduledtl] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[ETScheduledtl]', N'utrecordversion') IS NULL ALTER TABLE [ETScheduledtl] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[ETScheduledtl]', N'utblock') IS NULL ALTER TABLE [ETScheduledtl] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[ETScheduledtl]', N'utaddappversion') IS NULL ALTER TABLE [ETScheduledtl] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETScheduledtl]', N'utadddbversion') IS NULL ALTER TABLE [ETScheduledtl] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETScheduledtl]', N'utaddid') IS NULL ALTER TABLE [ETScheduledtl] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[ETScheduledtl]', N'utaddip') IS NULL ALTER TABLE [ETScheduledtl] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETScheduledtl]', N'utaddsessionid') IS NULL ALTER TABLE [ETScheduledtl] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETScheduledtl]', N'utaddsystem') IS NULL ALTER TABLE [ETScheduledtl] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETScheduledtl]', N'utaddtime') IS NULL ALTER TABLE [ETScheduledtl] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETScheduledtl]', N'utadduser') IS NULL ALTER TABLE [ETScheduledtl] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETScheduledtl]', N'utdoctype') IS NULL ALTER TABLE [ETScheduledtl] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[ETScheduledtl]', N'uteditappversion') IS NULL ALTER TABLE [ETScheduledtl] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETScheduledtl]', N'uteditdbversion') IS NULL ALTER TABLE [ETScheduledtl] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETScheduledtl]', N'uteditid') IS NULL ALTER TABLE [ETScheduledtl] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[ETScheduledtl]', N'uteditip') IS NULL ALTER TABLE [ETScheduledtl] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETScheduledtl]', N'uteditsessionid') IS NULL ALTER TABLE [ETScheduledtl] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETScheduledtl]', N'uteditsystem') IS NULL ALTER TABLE [ETScheduledtl] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETScheduledtl]', N'utedittime') IS NULL ALTER TABLE [ETScheduledtl] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETScheduledtl]', N'utedituser') IS NULL ALTER TABLE [ETScheduledtl] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETScheduledtl]', N'utsubdoctype') IS NULL ALTER TABLE [ETScheduledtl] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[ETSchedulehdr]', N'U') IS NULL BEGIN CREATE TABLE [ETSchedulehdr] (
    [id] int NOT NULL,
    [code] nvarchar(20) NULL,
    [stxt] nvarchar(150) NULL,
    [fromdate] datetime2 NULL,
    [tilldate] datetime2 NULL,
    [schedulefor] int NULL,
    [shiftid] int NULL,
    [assignto] int NULL,
    [clientid] int NULL,
    [entityid] int NULL,
    [utsysgen] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utblock] bit NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_ETSchedulehdr] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[ETSchedulehdr]', N'id') IS NULL ALTER TABLE [ETSchedulehdr] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[ETSchedulehdr]', N'code') IS NULL ALTER TABLE [ETSchedulehdr] ADD [code] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[ETSchedulehdr]', N'stxt') IS NULL ALTER TABLE [ETSchedulehdr] ADD [stxt] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[ETSchedulehdr]', N'fromdate') IS NULL ALTER TABLE [ETSchedulehdr] ADD [fromdate] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETSchedulehdr]', N'tilldate') IS NULL ALTER TABLE [ETSchedulehdr] ADD [tilldate] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETSchedulehdr]', N'schedulefor') IS NULL ALTER TABLE [ETSchedulehdr] ADD [schedulefor] int NULL;
GO
IF COL_LENGTH(N'[ETSchedulehdr]', N'shiftid') IS NULL ALTER TABLE [ETSchedulehdr] ADD [shiftid] int NULL;
GO
IF COL_LENGTH(N'[ETSchedulehdr]', N'assignto') IS NULL ALTER TABLE [ETSchedulehdr] ADD [assignto] int NULL;
GO
IF COL_LENGTH(N'[ETSchedulehdr]', N'clientid') IS NULL ALTER TABLE [ETSchedulehdr] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[ETSchedulehdr]', N'entityid') IS NULL ALTER TABLE [ETSchedulehdr] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[ETSchedulehdr]', N'utsysgen') IS NULL ALTER TABLE [ETSchedulehdr] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[ETSchedulehdr]', N'utispartial') IS NULL ALTER TABLE [ETSchedulehdr] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[ETSchedulehdr]', N'utrecordversion') IS NULL ALTER TABLE [ETSchedulehdr] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[ETSchedulehdr]', N'utblock') IS NULL ALTER TABLE [ETSchedulehdr] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[ETSchedulehdr]', N'utaddappversion') IS NULL ALTER TABLE [ETSchedulehdr] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETSchedulehdr]', N'utadddbversion') IS NULL ALTER TABLE [ETSchedulehdr] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETSchedulehdr]', N'utaddid') IS NULL ALTER TABLE [ETSchedulehdr] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[ETSchedulehdr]', N'utaddip') IS NULL ALTER TABLE [ETSchedulehdr] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETSchedulehdr]', N'utaddsessionid') IS NULL ALTER TABLE [ETSchedulehdr] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETSchedulehdr]', N'utaddsystem') IS NULL ALTER TABLE [ETSchedulehdr] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETSchedulehdr]', N'utaddtime') IS NULL ALTER TABLE [ETSchedulehdr] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETSchedulehdr]', N'utadduser') IS NULL ALTER TABLE [ETSchedulehdr] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETSchedulehdr]', N'utdoctype') IS NULL ALTER TABLE [ETSchedulehdr] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[ETSchedulehdr]', N'uteditappversion') IS NULL ALTER TABLE [ETSchedulehdr] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETSchedulehdr]', N'uteditdbversion') IS NULL ALTER TABLE [ETSchedulehdr] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETSchedulehdr]', N'uteditid') IS NULL ALTER TABLE [ETSchedulehdr] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[ETSchedulehdr]', N'uteditip') IS NULL ALTER TABLE [ETSchedulehdr] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETSchedulehdr]', N'uteditsessionid') IS NULL ALTER TABLE [ETSchedulehdr] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETSchedulehdr]', N'uteditsystem') IS NULL ALTER TABLE [ETSchedulehdr] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETSchedulehdr]', N'utedittime') IS NULL ALTER TABLE [ETSchedulehdr] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETSchedulehdr]', N'utedituser') IS NULL ALTER TABLE [ETSchedulehdr] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETSchedulehdr]', N'utsubdoctype') IS NULL ALTER TABLE [ETSchedulehdr] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[ETShiftdtl]', N'U') IS NULL BEGIN CREATE TABLE [ETShiftdtl] (
    [Id] int NOT NULL,
    [shiftid] int NULL,
    [lineindex] int NULL,
    [stxt] nvarchar(200) NULL,
    [dayid] int NULL,
    [intime] datetime2 NULL,
    [outtime] datetime2 NULL,
    [gracetime] int NULL,
    [gracetimeend] int NULL,
    [latepolicyapplicable] bit NULL,
    [earlypolicyapplicable] bit NULL,
    [startdate] datetime2 NULL,
    [endate] datetime2 NULL,
    [duration] float NULL,
    [utblock] bit NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    CONSTRAINT [PK_ETShiftdtl] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[ETShiftdtl]', N'Id') IS NULL ALTER TABLE [ETShiftdtl] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[ETShiftdtl]', N'shiftid') IS NULL ALTER TABLE [ETShiftdtl] ADD [shiftid] int NULL;
GO
IF COL_LENGTH(N'[ETShiftdtl]', N'lineindex') IS NULL ALTER TABLE [ETShiftdtl] ADD [lineindex] int NULL;
GO
IF COL_LENGTH(N'[ETShiftdtl]', N'stxt') IS NULL ALTER TABLE [ETShiftdtl] ADD [stxt] nvarchar(200) NULL;
GO
IF COL_LENGTH(N'[ETShiftdtl]', N'dayid') IS NULL ALTER TABLE [ETShiftdtl] ADD [dayid] int NULL;
GO
IF COL_LENGTH(N'[ETShiftdtl]', N'intime') IS NULL ALTER TABLE [ETShiftdtl] ADD [intime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETShiftdtl]', N'outtime') IS NULL ALTER TABLE [ETShiftdtl] ADD [outtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETShiftdtl]', N'gracetime') IS NULL ALTER TABLE [ETShiftdtl] ADD [gracetime] int NULL;
GO
IF COL_LENGTH(N'[ETShiftdtl]', N'gracetimeend') IS NULL ALTER TABLE [ETShiftdtl] ADD [gracetimeend] int NULL;
GO
IF COL_LENGTH(N'[ETShiftdtl]', N'latepolicyapplicable') IS NULL ALTER TABLE [ETShiftdtl] ADD [latepolicyapplicable] bit NULL;
GO
IF COL_LENGTH(N'[ETShiftdtl]', N'earlypolicyapplicable') IS NULL ALTER TABLE [ETShiftdtl] ADD [earlypolicyapplicable] bit NULL;
GO
IF COL_LENGTH(N'[ETShiftdtl]', N'startdate') IS NULL ALTER TABLE [ETShiftdtl] ADD [startdate] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETShiftdtl]', N'endate') IS NULL ALTER TABLE [ETShiftdtl] ADD [endate] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETShiftdtl]', N'duration') IS NULL ALTER TABLE [ETShiftdtl] ADD [duration] float NULL;
GO
IF COL_LENGTH(N'[ETShiftdtl]', N'utblock') IS NULL ALTER TABLE [ETShiftdtl] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[ETShiftdtl]', N'utaddappversion') IS NULL ALTER TABLE [ETShiftdtl] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETShiftdtl]', N'utadddbversion') IS NULL ALTER TABLE [ETShiftdtl] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETShiftdtl]', N'utaddid') IS NULL ALTER TABLE [ETShiftdtl] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[ETShiftdtl]', N'utaddip') IS NULL ALTER TABLE [ETShiftdtl] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETShiftdtl]', N'utaddsessionid') IS NULL ALTER TABLE [ETShiftdtl] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETShiftdtl]', N'utaddsystem') IS NULL ALTER TABLE [ETShiftdtl] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETShiftdtl]', N'utaddtime') IS NULL ALTER TABLE [ETShiftdtl] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETShiftdtl]', N'utadduser') IS NULL ALTER TABLE [ETShiftdtl] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETShiftdtl]', N'utdoctype') IS NULL ALTER TABLE [ETShiftdtl] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[ETShiftdtl]', N'uteditappversion') IS NULL ALTER TABLE [ETShiftdtl] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETShiftdtl]', N'uteditdbversion') IS NULL ALTER TABLE [ETShiftdtl] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETShiftdtl]', N'uteditid') IS NULL ALTER TABLE [ETShiftdtl] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[ETShiftdtl]', N'uteditip') IS NULL ALTER TABLE [ETShiftdtl] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETShiftdtl]', N'uteditsessionid') IS NULL ALTER TABLE [ETShiftdtl] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETShiftdtl]', N'uteditsystem') IS NULL ALTER TABLE [ETShiftdtl] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETShiftdtl]', N'utedittime') IS NULL ALTER TABLE [ETShiftdtl] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETShiftdtl]', N'utedituser') IS NULL ALTER TABLE [ETShiftdtl] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETShiftdtl]', N'utispartial') IS NULL ALTER TABLE [ETShiftdtl] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[ETShiftdtl]', N'utrecordversion') IS NULL ALTER TABLE [ETShiftdtl] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[ETShiftdtl]', N'utsubdoctype') IS NULL ALTER TABLE [ETShiftdtl] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[ETShiftdtl]', N'utsysgen') IS NULL ALTER TABLE [ETShiftdtl] ADD [utsysgen] bit NULL;
GO
IF OBJECT_ID(N'[ETShifthdr]', N'U') IS NULL BEGIN CREATE TABLE [ETShifthdr] (
    [Id] int NOT NULL,
    [code] nvarchar(20) NULL,
    [stxt] nvarchar(50) NULL,
    [shifttypeid] int NULL,
    [womonday] bit NULL,
    [wotuesday] bit NULL,
    [wowednesday] bit NULL,
    [wothuresday] bit NULL,
    [wofriday] bit NULL,
    [wosaturday] bit NULL,
    [wosunday] bit NULL,
    [otpolicyid] int NULL,
    [latearivpolicyid] int NULL,
    [earlydeppolicyid] int NULL,
    [abspolicyid] int NULL,
    [lovshiftclassid] int NULL,
    [lovsharedid] int NULL,
    [otpolicymnt] int NULL,
    [latearivpolicymnt] int NULL,
    [earlydeppolicymnt] int NULL,
    [entityid] int NULL,
    [clientid] int NULL,
    [utblock] bit NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    CONSTRAINT [PK_ETShifthdr] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'Id') IS NULL ALTER TABLE [ETShifthdr] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'code') IS NULL ALTER TABLE [ETShifthdr] ADD [code] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'stxt') IS NULL ALTER TABLE [ETShifthdr] ADD [stxt] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'shifttypeid') IS NULL ALTER TABLE [ETShifthdr] ADD [shifttypeid] int NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'womonday') IS NULL ALTER TABLE [ETShifthdr] ADD [womonday] bit NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'wotuesday') IS NULL ALTER TABLE [ETShifthdr] ADD [wotuesday] bit NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'wowednesday') IS NULL ALTER TABLE [ETShifthdr] ADD [wowednesday] bit NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'wothuresday') IS NULL ALTER TABLE [ETShifthdr] ADD [wothuresday] bit NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'wofriday') IS NULL ALTER TABLE [ETShifthdr] ADD [wofriday] bit NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'wosaturday') IS NULL ALTER TABLE [ETShifthdr] ADD [wosaturday] bit NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'wosunday') IS NULL ALTER TABLE [ETShifthdr] ADD [wosunday] bit NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'otpolicyid') IS NULL ALTER TABLE [ETShifthdr] ADD [otpolicyid] int NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'latearivpolicyid') IS NULL ALTER TABLE [ETShifthdr] ADD [latearivpolicyid] int NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'earlydeppolicyid') IS NULL ALTER TABLE [ETShifthdr] ADD [earlydeppolicyid] int NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'abspolicyid') IS NULL ALTER TABLE [ETShifthdr] ADD [abspolicyid] int NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'lovshiftclassid') IS NULL ALTER TABLE [ETShifthdr] ADD [lovshiftclassid] int NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'lovsharedid') IS NULL ALTER TABLE [ETShifthdr] ADD [lovsharedid] int NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'otpolicymnt') IS NULL ALTER TABLE [ETShifthdr] ADD [otpolicymnt] int NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'latearivpolicymnt') IS NULL ALTER TABLE [ETShifthdr] ADD [latearivpolicymnt] int NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'earlydeppolicymnt') IS NULL ALTER TABLE [ETShifthdr] ADD [earlydeppolicymnt] int NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'entityid') IS NULL ALTER TABLE [ETShifthdr] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'clientid') IS NULL ALTER TABLE [ETShifthdr] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'utblock') IS NULL ALTER TABLE [ETShifthdr] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'utaddappversion') IS NULL ALTER TABLE [ETShifthdr] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'utadddbversion') IS NULL ALTER TABLE [ETShifthdr] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'utaddid') IS NULL ALTER TABLE [ETShifthdr] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'utaddip') IS NULL ALTER TABLE [ETShifthdr] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'utaddsessionid') IS NULL ALTER TABLE [ETShifthdr] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'utaddsystem') IS NULL ALTER TABLE [ETShifthdr] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'utaddtime') IS NULL ALTER TABLE [ETShifthdr] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'utadduser') IS NULL ALTER TABLE [ETShifthdr] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'utdoctype') IS NULL ALTER TABLE [ETShifthdr] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'uteditappversion') IS NULL ALTER TABLE [ETShifthdr] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'uteditdbversion') IS NULL ALTER TABLE [ETShifthdr] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'uteditid') IS NULL ALTER TABLE [ETShifthdr] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'uteditip') IS NULL ALTER TABLE [ETShifthdr] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'uteditsessionid') IS NULL ALTER TABLE [ETShifthdr] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'uteditsystem') IS NULL ALTER TABLE [ETShifthdr] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'utedittime') IS NULL ALTER TABLE [ETShifthdr] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'utedituser') IS NULL ALTER TABLE [ETShifthdr] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'utispartial') IS NULL ALTER TABLE [ETShifthdr] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'utrecordversion') IS NULL ALTER TABLE [ETShifthdr] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'utsubdoctype') IS NULL ALTER TABLE [ETShifthdr] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[ETShifthdr]', N'utsysgen') IS NULL ALTER TABLE [ETShifthdr] ADD [utsysgen] bit NULL;
GO
IF OBJECT_ID(N'[ETShiftshareddtl]', N'U') IS NULL BEGIN CREATE TABLE [ETShiftshareddtl] (
    [Id] int NOT NULL,
    [lineindex] int NULL,
    [shiftid] int NULL,
    [lovsharedid] int NULL,
    [pclientid] int NULL,
    [pentityid] int NULL,
    [pdepartid] int NULL,
    [pempid] int NULL,
    [utblock] bit NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    CONSTRAINT [PK_ETShiftshareddtl] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[ETShiftshareddtl]', N'Id') IS NULL ALTER TABLE [ETShiftshareddtl] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[ETShiftshareddtl]', N'lineindex') IS NULL ALTER TABLE [ETShiftshareddtl] ADD [lineindex] int NULL;
GO
IF COL_LENGTH(N'[ETShiftshareddtl]', N'shiftid') IS NULL ALTER TABLE [ETShiftshareddtl] ADD [shiftid] int NULL;
GO
IF COL_LENGTH(N'[ETShiftshareddtl]', N'lovsharedid') IS NULL ALTER TABLE [ETShiftshareddtl] ADD [lovsharedid] int NULL;
GO
IF COL_LENGTH(N'[ETShiftshareddtl]', N'pclientid') IS NULL ALTER TABLE [ETShiftshareddtl] ADD [pclientid] int NULL;
GO
IF COL_LENGTH(N'[ETShiftshareddtl]', N'pentityid') IS NULL ALTER TABLE [ETShiftshareddtl] ADD [pentityid] int NULL;
GO
IF COL_LENGTH(N'[ETShiftshareddtl]', N'pdepartid') IS NULL ALTER TABLE [ETShiftshareddtl] ADD [pdepartid] int NULL;
GO
IF COL_LENGTH(N'[ETShiftshareddtl]', N'pempid') IS NULL ALTER TABLE [ETShiftshareddtl] ADD [pempid] int NULL;
GO
IF COL_LENGTH(N'[ETShiftshareddtl]', N'utblock') IS NULL ALTER TABLE [ETShiftshareddtl] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[ETShiftshareddtl]', N'utaddappversion') IS NULL ALTER TABLE [ETShiftshareddtl] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETShiftshareddtl]', N'utadddbversion') IS NULL ALTER TABLE [ETShiftshareddtl] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETShiftshareddtl]', N'utaddid') IS NULL ALTER TABLE [ETShiftshareddtl] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[ETShiftshareddtl]', N'utaddip') IS NULL ALTER TABLE [ETShiftshareddtl] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETShiftshareddtl]', N'utaddsessionid') IS NULL ALTER TABLE [ETShiftshareddtl] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETShiftshareddtl]', N'utaddsystem') IS NULL ALTER TABLE [ETShiftshareddtl] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETShiftshareddtl]', N'utaddtime') IS NULL ALTER TABLE [ETShiftshareddtl] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETShiftshareddtl]', N'utadduser') IS NULL ALTER TABLE [ETShiftshareddtl] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETShiftshareddtl]', N'utdoctype') IS NULL ALTER TABLE [ETShiftshareddtl] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[ETShiftshareddtl]', N'uteditappversion') IS NULL ALTER TABLE [ETShiftshareddtl] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETShiftshareddtl]', N'uteditdbversion') IS NULL ALTER TABLE [ETShiftshareddtl] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETShiftshareddtl]', N'uteditid') IS NULL ALTER TABLE [ETShiftshareddtl] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[ETShiftshareddtl]', N'uteditip') IS NULL ALTER TABLE [ETShiftshareddtl] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETShiftshareddtl]', N'uteditsessionid') IS NULL ALTER TABLE [ETShiftshareddtl] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETShiftshareddtl]', N'uteditsystem') IS NULL ALTER TABLE [ETShiftshareddtl] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETShiftshareddtl]', N'utedittime') IS NULL ALTER TABLE [ETShiftshareddtl] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETShiftshareddtl]', N'utedituser') IS NULL ALTER TABLE [ETShiftshareddtl] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETShiftshareddtl]', N'utispartial') IS NULL ALTER TABLE [ETShiftshareddtl] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[ETShiftshareddtl]', N'utrecordversion') IS NULL ALTER TABLE [ETShiftshareddtl] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[ETShiftshareddtl]', N'utsubdoctype') IS NULL ALTER TABLE [ETShiftshareddtl] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[ETShiftshareddtl]', N'utsysgen') IS NULL ALTER TABLE [ETShiftshareddtl] ADD [utsysgen] bit NULL;
GO
IF OBJECT_ID(N'[ETShiftsubdtl]', N'U') IS NULL BEGIN CREATE TABLE [ETShiftsubdtl] (
    [Id] int NOT NULL,
    [shiftdtlid] int NULL,
    [lineindex] int NULL,
    [stxt] nvarchar(50) NULL,
    [starttime] datetime2 NULL,
    [endtime] datetime2 NULL,
    [duration] float NULL,
    [utblock] bit NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    CONSTRAINT [PK_ETShiftsubdtl] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[ETShiftsubdtl]', N'Id') IS NULL ALTER TABLE [ETShiftsubdtl] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[ETShiftsubdtl]', N'shiftdtlid') IS NULL ALTER TABLE [ETShiftsubdtl] ADD [shiftdtlid] int NULL;
GO
IF COL_LENGTH(N'[ETShiftsubdtl]', N'lineindex') IS NULL ALTER TABLE [ETShiftsubdtl] ADD [lineindex] int NULL;
GO
IF COL_LENGTH(N'[ETShiftsubdtl]', N'stxt') IS NULL ALTER TABLE [ETShiftsubdtl] ADD [stxt] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[ETShiftsubdtl]', N'starttime') IS NULL ALTER TABLE [ETShiftsubdtl] ADD [starttime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETShiftsubdtl]', N'endtime') IS NULL ALTER TABLE [ETShiftsubdtl] ADD [endtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETShiftsubdtl]', N'duration') IS NULL ALTER TABLE [ETShiftsubdtl] ADD [duration] float NULL;
GO
IF COL_LENGTH(N'[ETShiftsubdtl]', N'utblock') IS NULL ALTER TABLE [ETShiftsubdtl] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[ETShiftsubdtl]', N'utaddappversion') IS NULL ALTER TABLE [ETShiftsubdtl] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETShiftsubdtl]', N'utadddbversion') IS NULL ALTER TABLE [ETShiftsubdtl] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETShiftsubdtl]', N'utaddid') IS NULL ALTER TABLE [ETShiftsubdtl] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[ETShiftsubdtl]', N'utaddip') IS NULL ALTER TABLE [ETShiftsubdtl] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETShiftsubdtl]', N'utaddsessionid') IS NULL ALTER TABLE [ETShiftsubdtl] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETShiftsubdtl]', N'utaddsystem') IS NULL ALTER TABLE [ETShiftsubdtl] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETShiftsubdtl]', N'utaddtime') IS NULL ALTER TABLE [ETShiftsubdtl] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETShiftsubdtl]', N'utadduser') IS NULL ALTER TABLE [ETShiftsubdtl] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETShiftsubdtl]', N'utdoctype') IS NULL ALTER TABLE [ETShiftsubdtl] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[ETShiftsubdtl]', N'uteditappversion') IS NULL ALTER TABLE [ETShiftsubdtl] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETShiftsubdtl]', N'uteditdbversion') IS NULL ALTER TABLE [ETShiftsubdtl] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETShiftsubdtl]', N'uteditid') IS NULL ALTER TABLE [ETShiftsubdtl] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[ETShiftsubdtl]', N'uteditip') IS NULL ALTER TABLE [ETShiftsubdtl] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETShiftsubdtl]', N'uteditsessionid') IS NULL ALTER TABLE [ETShiftsubdtl] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETShiftsubdtl]', N'uteditsystem') IS NULL ALTER TABLE [ETShiftsubdtl] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETShiftsubdtl]', N'utedittime') IS NULL ALTER TABLE [ETShiftsubdtl] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETShiftsubdtl]', N'utedituser') IS NULL ALTER TABLE [ETShiftsubdtl] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETShiftsubdtl]', N'utispartial') IS NULL ALTER TABLE [ETShiftsubdtl] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[ETShiftsubdtl]', N'utrecordversion') IS NULL ALTER TABLE [ETShiftsubdtl] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[ETShiftsubdtl]', N'utsubdoctype') IS NULL ALTER TABLE [ETShiftsubdtl] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[ETShiftsubdtl]', N'utsysgen') IS NULL ALTER TABLE [ETShiftsubdtl] ADD [utsysgen] bit NULL;
GO
IF OBJECT_ID(N'[FMBankgrp]', N'U') IS NULL BEGIN CREATE TABLE [FMBankgrp] (
    [Id] int NOT NULL,
    [code] nvarchar(10) NULL,
    [stxt] nvarchar(150) NULL,
    [ltxt] nvarchar(500) NULL,
    [entityid] int NULL,
    [clientid] int NULL,
    [utblock] bit NOT NULL,
    CONSTRAINT [PK_FMBankgrp] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[FMBankgrp]', N'Id') IS NULL ALTER TABLE [FMBankgrp] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[FMBankgrp]', N'code') IS NULL ALTER TABLE [FMBankgrp] ADD [code] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[FMBankgrp]', N'stxt') IS NULL ALTER TABLE [FMBankgrp] ADD [stxt] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[FMBankgrp]', N'ltxt') IS NULL ALTER TABLE [FMBankgrp] ADD [ltxt] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[FMBankgrp]', N'entityid') IS NULL ALTER TABLE [FMBankgrp] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[FMBankgrp]', N'clientid') IS NULL ALTER TABLE [FMBankgrp] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[FMBankgrp]', N'utblock') IS NULL ALTER TABLE [FMBankgrp] ADD [utblock] bit NULL;
GO
IF OBJECT_ID(N'[HRAtnactivity]', N'U') IS NULL BEGIN CREATE TABLE [HRAtnactivity] (
    [id] int NOT NULL,
    [stxt] nvarchar(250) NULL,
    [value] int NULL,
    [orderid] int NULL,
    [clientid] int NULL,
    [utblock] bit NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    CONSTRAINT [PK_HRAtnactivity] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[HRAtnactivity]', N'id') IS NULL ALTER TABLE [HRAtnactivity] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[HRAtnactivity]', N'stxt') IS NULL ALTER TABLE [HRAtnactivity] ADD [stxt] nvarchar(250) NULL;
GO
IF COL_LENGTH(N'[HRAtnactivity]', N'value') IS NULL ALTER TABLE [HRAtnactivity] ADD [value] int NULL;
GO
IF COL_LENGTH(N'[HRAtnactivity]', N'orderid') IS NULL ALTER TABLE [HRAtnactivity] ADD [orderid] int NULL;
GO
IF COL_LENGTH(N'[HRAtnactivity]', N'clientid') IS NULL ALTER TABLE [HRAtnactivity] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[HRAtnactivity]', N'utblock') IS NULL ALTER TABLE [HRAtnactivity] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[HRAtnactivity]', N'utaddappversion') IS NULL ALTER TABLE [HRAtnactivity] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[HRAtnactivity]', N'utadddbversion') IS NULL ALTER TABLE [HRAtnactivity] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[HRAtnactivity]', N'utaddid') IS NULL ALTER TABLE [HRAtnactivity] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[HRAtnactivity]', N'utaddip') IS NULL ALTER TABLE [HRAtnactivity] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRAtnactivity]', N'utaddsessionid') IS NULL ALTER TABLE [HRAtnactivity] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[HRAtnactivity]', N'utaddsystem') IS NULL ALTER TABLE [HRAtnactivity] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRAtnactivity]', N'utaddtime') IS NULL ALTER TABLE [HRAtnactivity] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRAtnactivity]', N'utadduser') IS NULL ALTER TABLE [HRAtnactivity] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRAtnactivity]', N'utdoctype') IS NULL ALTER TABLE [HRAtnactivity] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[HRAtnactivity]', N'uteditappversion') IS NULL ALTER TABLE [HRAtnactivity] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[HRAtnactivity]', N'uteditdbversion') IS NULL ALTER TABLE [HRAtnactivity] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[HRAtnactivity]', N'uteditid') IS NULL ALTER TABLE [HRAtnactivity] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[HRAtnactivity]', N'uteditip') IS NULL ALTER TABLE [HRAtnactivity] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRAtnactivity]', N'uteditsessionid') IS NULL ALTER TABLE [HRAtnactivity] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[HRAtnactivity]', N'uteditsystem') IS NULL ALTER TABLE [HRAtnactivity] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRAtnactivity]', N'utedittime') IS NULL ALTER TABLE [HRAtnactivity] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRAtnactivity]', N'utedituser') IS NULL ALTER TABLE [HRAtnactivity] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRAtnactivity]', N'utispartial') IS NULL ALTER TABLE [HRAtnactivity] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[HRAtnactivity]', N'utrecordversion') IS NULL ALTER TABLE [HRAtnactivity] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[HRAtnactivity]', N'utsubdoctype') IS NULL ALTER TABLE [HRAtnactivity] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[HRAtnactivity]', N'utsysgen') IS NULL ALTER TABLE [HRAtnactivity] ADD [utsysgen] bit NULL;
GO
IF OBJECT_ID(N'[HRAtnintout]', N'U') IS NULL BEGIN CREATE TABLE [HRAtnintout] (
    [id] int NOT NULL,
    [empid] int NULL,
    [machineid] int NULL,
    [atntypeid] int NULL,
    [atnactivityid] int NULL,
    [dttime] datetime2 NULL,
    [orgdttime] datetime2 NULL,
    [shiftid] int NULL,
    [shiftdtlid] int NULL,
    [dayid] int NULL,
    [remarks] nvarchar(1500) NULL,
    [filepath] nvarchar(500) NULL,
    [posttransid] bigint NULL,
    [clientid] int NULL,
    [entityid] int NULL,
    [shiftin] datetime2 NULL,
    [shiftout] datetime2 NULL,
    [schtimein] datetime2 NULL,
    [schtimeout] datetime2 NULL,
    [latemin] int NULL,
    [latcount] int NULL,
    [edmin] int NULL,
    [edcount] int NULL,
    [otmin] int NULL,
    [otcount] int NULL,
    [utsysgen] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utblock] bit NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_HRAtnintout] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'id') IS NULL ALTER TABLE [HRAtnintout] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'empid') IS NULL ALTER TABLE [HRAtnintout] ADD [empid] int NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'machineid') IS NULL ALTER TABLE [HRAtnintout] ADD [machineid] int NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'atntypeid') IS NULL ALTER TABLE [HRAtnintout] ADD [atntypeid] int NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'atnactivityid') IS NULL ALTER TABLE [HRAtnintout] ADD [atnactivityid] int NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'dttime') IS NULL ALTER TABLE [HRAtnintout] ADD [dttime] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'orgdttime') IS NULL ALTER TABLE [HRAtnintout] ADD [orgdttime] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'shiftid') IS NULL ALTER TABLE [HRAtnintout] ADD [shiftid] int NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'shiftdtlid') IS NULL ALTER TABLE [HRAtnintout] ADD [shiftdtlid] int NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'dayid') IS NULL ALTER TABLE [HRAtnintout] ADD [dayid] int NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'remarks') IS NULL ALTER TABLE [HRAtnintout] ADD [remarks] nvarchar(1500) NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'filepath') IS NULL ALTER TABLE [HRAtnintout] ADD [filepath] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'posttransid') IS NULL ALTER TABLE [HRAtnintout] ADD [posttransid] bigint NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'clientid') IS NULL ALTER TABLE [HRAtnintout] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'entityid') IS NULL ALTER TABLE [HRAtnintout] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'shiftin') IS NULL ALTER TABLE [HRAtnintout] ADD [shiftin] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'shiftout') IS NULL ALTER TABLE [HRAtnintout] ADD [shiftout] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'schtimein') IS NULL ALTER TABLE [HRAtnintout] ADD [schtimein] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'schtimeout') IS NULL ALTER TABLE [HRAtnintout] ADD [schtimeout] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'latemin') IS NULL ALTER TABLE [HRAtnintout] ADD [latemin] int NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'latcount') IS NULL ALTER TABLE [HRAtnintout] ADD [latcount] int NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'edmin') IS NULL ALTER TABLE [HRAtnintout] ADD [edmin] int NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'edcount') IS NULL ALTER TABLE [HRAtnintout] ADD [edcount] int NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'otmin') IS NULL ALTER TABLE [HRAtnintout] ADD [otmin] int NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'otcount') IS NULL ALTER TABLE [HRAtnintout] ADD [otcount] int NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'utsysgen') IS NULL ALTER TABLE [HRAtnintout] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'utispartial') IS NULL ALTER TABLE [HRAtnintout] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'utrecordversion') IS NULL ALTER TABLE [HRAtnintout] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'utblock') IS NULL ALTER TABLE [HRAtnintout] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'utaddappversion') IS NULL ALTER TABLE [HRAtnintout] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'utadddbversion') IS NULL ALTER TABLE [HRAtnintout] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'utaddid') IS NULL ALTER TABLE [HRAtnintout] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'utaddip') IS NULL ALTER TABLE [HRAtnintout] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'utaddsessionid') IS NULL ALTER TABLE [HRAtnintout] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'utaddsystem') IS NULL ALTER TABLE [HRAtnintout] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'utaddtime') IS NULL ALTER TABLE [HRAtnintout] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'utadduser') IS NULL ALTER TABLE [HRAtnintout] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'utdoctype') IS NULL ALTER TABLE [HRAtnintout] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'uteditappversion') IS NULL ALTER TABLE [HRAtnintout] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'uteditdbversion') IS NULL ALTER TABLE [HRAtnintout] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'uteditid') IS NULL ALTER TABLE [HRAtnintout] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'uteditip') IS NULL ALTER TABLE [HRAtnintout] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'uteditsessionid') IS NULL ALTER TABLE [HRAtnintout] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'uteditsystem') IS NULL ALTER TABLE [HRAtnintout] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'utedittime') IS NULL ALTER TABLE [HRAtnintout] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'utedituser') IS NULL ALTER TABLE [HRAtnintout] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRAtnintout]', N'utsubdoctype') IS NULL ALTER TABLE [HRAtnintout] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[HRdailyattendance]', N'U') IS NULL BEGIN CREATE TABLE [HRdailyattendance] (
    [Id] bigint NOT NULL,
    [employeeid] int NULL,
    [atndate] datetime2 NULL,
    [schtimein] datetime2 NULL,
    [schtimeout] datetime2 NULL,
    [actshiftin] datetime2 NULL,
    [actshitout] datetime2 NULL,
    [shiftid] int NULL,
    [shiftdtlid] int NULL,
    [dayid] int NULL,
    [remarks] nvarchar(1500) NULL,
    [entityid] int NULL,
    [latemin] int NULL,
    [latecount] int NULL,
    [lathalfdays] int NULL,
    [lateabsent] int NULL,
    [lateexcuse] int NULL,
    [edmin] int NULL,
    [edcount] int NULL,
    [edhalfdays] int NULL,
    [edabsent] int NULL,
    [edexcuse] int NULL,
    [otmin] int NULL,
    [otcount] int NULL,
    [othalfdays] int NULL,
    [depid] int NULL,
    [designationid] int NULL,
    [isabsent] bit NOT NULL,
    [isholiday] bit NOT NULL,
    [utsysgen] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utblock] bit NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_HRdailyattendance] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'Id') IS NULL ALTER TABLE [HRdailyattendance] ADD [Id] bigint NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'employeeid') IS NULL ALTER TABLE [HRdailyattendance] ADD [employeeid] int NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'atndate') IS NULL ALTER TABLE [HRdailyattendance] ADD [atndate] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'schtimein') IS NULL ALTER TABLE [HRdailyattendance] ADD [schtimein] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'schtimeout') IS NULL ALTER TABLE [HRdailyattendance] ADD [schtimeout] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'actshiftin') IS NULL ALTER TABLE [HRdailyattendance] ADD [actshiftin] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'actshitout') IS NULL ALTER TABLE [HRdailyattendance] ADD [actshitout] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'shiftid') IS NULL ALTER TABLE [HRdailyattendance] ADD [shiftid] int NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'shiftdtlid') IS NULL ALTER TABLE [HRdailyattendance] ADD [shiftdtlid] int NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'dayid') IS NULL ALTER TABLE [HRdailyattendance] ADD [dayid] int NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'remarks') IS NULL ALTER TABLE [HRdailyattendance] ADD [remarks] nvarchar(1500) NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'entityid') IS NULL ALTER TABLE [HRdailyattendance] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'latemin') IS NULL ALTER TABLE [HRdailyattendance] ADD [latemin] int NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'latecount') IS NULL ALTER TABLE [HRdailyattendance] ADD [latecount] int NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'lathalfdays') IS NULL ALTER TABLE [HRdailyattendance] ADD [lathalfdays] int NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'lateabsent') IS NULL ALTER TABLE [HRdailyattendance] ADD [lateabsent] int NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'lateexcuse') IS NULL ALTER TABLE [HRdailyattendance] ADD [lateexcuse] int NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'edmin') IS NULL ALTER TABLE [HRdailyattendance] ADD [edmin] int NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'edcount') IS NULL ALTER TABLE [HRdailyattendance] ADD [edcount] int NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'edhalfdays') IS NULL ALTER TABLE [HRdailyattendance] ADD [edhalfdays] int NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'edabsent') IS NULL ALTER TABLE [HRdailyattendance] ADD [edabsent] int NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'edexcuse') IS NULL ALTER TABLE [HRdailyattendance] ADD [edexcuse] int NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'otmin') IS NULL ALTER TABLE [HRdailyattendance] ADD [otmin] int NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'otcount') IS NULL ALTER TABLE [HRdailyattendance] ADD [otcount] int NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'othalfdays') IS NULL ALTER TABLE [HRdailyattendance] ADD [othalfdays] int NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'depid') IS NULL ALTER TABLE [HRdailyattendance] ADD [depid] int NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'designationid') IS NULL ALTER TABLE [HRdailyattendance] ADD [designationid] int NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'isabsent') IS NULL ALTER TABLE [HRdailyattendance] ADD [isabsent] bit NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'isholiday') IS NULL ALTER TABLE [HRdailyattendance] ADD [isholiday] bit NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'utsysgen') IS NULL ALTER TABLE [HRdailyattendance] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'utispartial') IS NULL ALTER TABLE [HRdailyattendance] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'utrecordversion') IS NULL ALTER TABLE [HRdailyattendance] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'utblock') IS NULL ALTER TABLE [HRdailyattendance] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'utaddappversion') IS NULL ALTER TABLE [HRdailyattendance] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'utadddbversion') IS NULL ALTER TABLE [HRdailyattendance] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'utaddid') IS NULL ALTER TABLE [HRdailyattendance] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'utaddip') IS NULL ALTER TABLE [HRdailyattendance] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'utaddsessionid') IS NULL ALTER TABLE [HRdailyattendance] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'utaddsystem') IS NULL ALTER TABLE [HRdailyattendance] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'utaddtime') IS NULL ALTER TABLE [HRdailyattendance] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'utadduser') IS NULL ALTER TABLE [HRdailyattendance] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'utdoctype') IS NULL ALTER TABLE [HRdailyattendance] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'uteditappversion') IS NULL ALTER TABLE [HRdailyattendance] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'uteditdbversion') IS NULL ALTER TABLE [HRdailyattendance] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'uteditid') IS NULL ALTER TABLE [HRdailyattendance] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'uteditip') IS NULL ALTER TABLE [HRdailyattendance] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'uteditsessionid') IS NULL ALTER TABLE [HRdailyattendance] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'uteditsystem') IS NULL ALTER TABLE [HRdailyattendance] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'utedittime') IS NULL ALTER TABLE [HRdailyattendance] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'utedituser') IS NULL ALTER TABLE [HRdailyattendance] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRdailyattendance]', N'utsubdoctype') IS NULL ALTER TABLE [HRdailyattendance] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[HRDocType]', N'U') IS NULL BEGIN CREATE TABLE [HRDocType] (
    [id] int NOT NULL,
    [code] nvarchar(10) NULL,
    [stxt] nvarchar(150) NULL,
    [ltxt] nvarchar(500) NULL,
    [clientid] int NULL,
    [utdoctype] int NULL,
    [utsubdoctype] int NULL,
    [utblock] bit NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utsysgen] bit NOT NULL,
    CONSTRAINT [PK_HRDocType] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[HRDocType]', N'id') IS NULL ALTER TABLE [HRDocType] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[HRDocType]', N'code') IS NULL ALTER TABLE [HRDocType] ADD [code] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[HRDocType]', N'stxt') IS NULL ALTER TABLE [HRDocType] ADD [stxt] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[HRDocType]', N'ltxt') IS NULL ALTER TABLE [HRDocType] ADD [ltxt] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[HRDocType]', N'clientid') IS NULL ALTER TABLE [HRDocType] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[HRDocType]', N'utdoctype') IS NULL ALTER TABLE [HRDocType] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[HRDocType]', N'utsubdoctype') IS NULL ALTER TABLE [HRDocType] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[HRDocType]', N'utblock') IS NULL ALTER TABLE [HRDocType] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[HRDocType]', N'utaddappversion') IS NULL ALTER TABLE [HRDocType] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[HRDocType]', N'utadddbversion') IS NULL ALTER TABLE [HRDocType] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[HRDocType]', N'utaddid') IS NULL ALTER TABLE [HRDocType] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[HRDocType]', N'utaddip') IS NULL ALTER TABLE [HRDocType] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRDocType]', N'utaddsessionid') IS NULL ALTER TABLE [HRDocType] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[HRDocType]', N'utaddsystem') IS NULL ALTER TABLE [HRDocType] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRDocType]', N'utaddtime') IS NULL ALTER TABLE [HRDocType] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRDocType]', N'utadduser') IS NULL ALTER TABLE [HRDocType] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRDocType]', N'uteditappversion') IS NULL ALTER TABLE [HRDocType] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[HRDocType]', N'uteditdbversion') IS NULL ALTER TABLE [HRDocType] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[HRDocType]', N'uteditid') IS NULL ALTER TABLE [HRDocType] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[HRDocType]', N'uteditip') IS NULL ALTER TABLE [HRDocType] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRDocType]', N'uteditsessionid') IS NULL ALTER TABLE [HRDocType] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[HRDocType]', N'uteditsystem') IS NULL ALTER TABLE [HRDocType] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRDocType]', N'utedittime') IS NULL ALTER TABLE [HRDocType] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRDocType]', N'utedituser') IS NULL ALTER TABLE [HRDocType] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRDocType]', N'utispartial') IS NULL ALTER TABLE [HRDocType] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[HRDocType]', N'utrecordversion') IS NULL ALTER TABLE [HRDocType] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[HRDocType]', N'utsysgen') IS NULL ALTER TABLE [HRDocType] ADD [utsysgen] bit NULL;
GO
IF OBJECT_ID(N'[HRGrade]', N'U') IS NULL BEGIN CREATE TABLE [HRGrade] (
    [id] int NOT NULL,
    [clientid] int NOT NULL,
    [entityid] int NOT NULL,
    [code] nvarchar(30) NULL,
    [minsalary] decimal(18,2) NOT NULL,
    [maxsalary] decimal(18,2) NOT NULL,
    [utblock] bit NOT NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] nvarchar(100) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] nvarchar(100) NULL,
    [utrecordversion] int NOT NULL,
    CONSTRAINT [PK_HRGrade] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[HRGrade]', N'id') IS NULL ALTER TABLE [HRGrade] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[HRGrade]', N'clientid') IS NULL ALTER TABLE [HRGrade] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[HRGrade]', N'entityid') IS NULL ALTER TABLE [HRGrade] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[HRGrade]', N'code') IS NULL ALTER TABLE [HRGrade] ADD [code] nvarchar(30) NULL;
GO
IF COL_LENGTH(N'[HRGrade]', N'minsalary') IS NULL ALTER TABLE [HRGrade] ADD [minsalary] decimal(18,2) NULL;
GO
IF COL_LENGTH(N'[HRGrade]', N'maxsalary') IS NULL ALTER TABLE [HRGrade] ADD [maxsalary] decimal(18,2) NULL;
GO
IF COL_LENGTH(N'[HRGrade]', N'utblock') IS NULL ALTER TABLE [HRGrade] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[HRGrade]', N'utaddtime') IS NULL ALTER TABLE [HRGrade] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRGrade]', N'utadduser') IS NULL ALTER TABLE [HRGrade] ADD [utadduser] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[HRGrade]', N'utedittime') IS NULL ALTER TABLE [HRGrade] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRGrade]', N'utedituser') IS NULL ALTER TABLE [HRGrade] ADD [utedituser] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[HRGrade]', N'utrecordversion') IS NULL ALTER TABLE [HRGrade] ADD [utrecordversion] int NULL;
GO
IF OBJECT_ID(N'[HRVacancyNote]', N'U') IS NULL BEGIN CREATE TABLE [HRVacancyNote] (
    [Id] int NOT NULL IDENTITY,
    [VacancyId] int NOT NULL,
    [Note] nvarchar(2000) NULL,
    [CreatedBy] nvarchar(100) NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    CONSTRAINT [PK_HRVacancyNote] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[HRVacancyNote]', N'Id') IS NULL ALTER TABLE [HRVacancyNote] ADD [Id] int NULL ;
GO
IF COL_LENGTH(N'[HRVacancyNote]', N'VacancyId') IS NULL ALTER TABLE [HRVacancyNote] ADD [VacancyId] int NULL;
GO
IF COL_LENGTH(N'[HRVacancyNote]', N'Note') IS NULL ALTER TABLE [HRVacancyNote] ADD [Note] nvarchar(2000) NULL;
GO
IF COL_LENGTH(N'[HRVacancyNote]', N'CreatedBy') IS NULL ALTER TABLE [HRVacancyNote] ADD [CreatedBy] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[HRVacancyNote]', N'CreatedAtUtc') IS NULL ALTER TABLE [HRVacancyNote] ADD [CreatedAtUtc] datetime2 NULL;
GO
IF OBJECT_ID(N'[HRVacancyQualification]', N'U') IS NULL BEGIN CREATE TABLE [HRVacancyQualification] (
    [Id] int NOT NULL IDENTITY,
    [VacancyId] int NOT NULL,
    [QualificationId] int NOT NULL,
    [IsMandatory] bit NOT NULL,
    CONSTRAINT [PK_HRVacancyQualification] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[HRVacancyQualification]', N'Id') IS NULL ALTER TABLE [HRVacancyQualification] ADD [Id] int NULL ;
GO
IF COL_LENGTH(N'[HRVacancyQualification]', N'VacancyId') IS NULL ALTER TABLE [HRVacancyQualification] ADD [VacancyId] int NULL;
GO
IF COL_LENGTH(N'[HRVacancyQualification]', N'QualificationId') IS NULL ALTER TABLE [HRVacancyQualification] ADD [QualificationId] int NULL;
GO
IF COL_LENGTH(N'[HRVacancyQualification]', N'IsMandatory') IS NULL ALTER TABLE [HRVacancyQualification] ADD [IsMandatory] bit NULL;
GO
IF OBJECT_ID(N'[HRVacancyRevision]', N'U') IS NULL BEGIN CREATE TABLE [HRVacancyRevision] (
    [Id] int NOT NULL IDENTITY,
    [VacancyId] int NOT NULL,
    [RevisionNumber] int NOT NULL,
    [EventType] nvarchar(30) NULL,
    [SnapshotJson] nvarchar(max) NULL,
    [CreatedBy] nvarchar(100) NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    CONSTRAINT [PK_HRVacancyRevision] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[HRVacancyRevision]', N'Id') IS NULL ALTER TABLE [HRVacancyRevision] ADD [Id] int NULL ;
GO
IF COL_LENGTH(N'[HRVacancyRevision]', N'VacancyId') IS NULL ALTER TABLE [HRVacancyRevision] ADD [VacancyId] int NULL;
GO
IF COL_LENGTH(N'[HRVacancyRevision]', N'RevisionNumber') IS NULL ALTER TABLE [HRVacancyRevision] ADD [RevisionNumber] int NULL;
GO
IF COL_LENGTH(N'[HRVacancyRevision]', N'EventType') IS NULL ALTER TABLE [HRVacancyRevision] ADD [EventType] nvarchar(30) NULL;
GO
IF COL_LENGTH(N'[HRVacancyRevision]', N'SnapshotJson') IS NULL ALTER TABLE [HRVacancyRevision] ADD [SnapshotJson] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[HRVacancyRevision]', N'CreatedBy') IS NULL ALTER TABLE [HRVacancyRevision] ADD [CreatedBy] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[HRVacancyRevision]', N'CreatedAtUtc') IS NULL ALTER TABLE [HRVacancyRevision] ADD [CreatedAtUtc] datetime2 NULL;
GO
IF OBJECT_ID(N'[HRVacancySkill]', N'U') IS NULL BEGIN CREATE TABLE [HRVacancySkill] (
    [Id] int NOT NULL IDENTITY,
    [VacancyId] int NOT NULL,
    [SkillId] int NOT NULL,
    [Proficiency] nvarchar(50) NULL,
    [IsMandatory] bit NOT NULL,
    CONSTRAINT [PK_HRVacancySkill] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[HRVacancySkill]', N'Id') IS NULL ALTER TABLE [HRVacancySkill] ADD [Id] int NULL ;
GO
IF COL_LENGTH(N'[HRVacancySkill]', N'VacancyId') IS NULL ALTER TABLE [HRVacancySkill] ADD [VacancyId] int NULL;
GO
IF COL_LENGTH(N'[HRVacancySkill]', N'SkillId') IS NULL ALTER TABLE [HRVacancySkill] ADD [SkillId] int NULL;
GO
IF COL_LENGTH(N'[HRVacancySkill]', N'Proficiency') IS NULL ALTER TABLE [HRVacancySkill] ADD [Proficiency] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[HRVacancySkill]', N'IsMandatory') IS NULL ALTER TABLE [HRVacancySkill] ADD [IsMandatory] bit NULL;
GO
IF OBJECT_ID(N'[lov]', N'U') IS NULL BEGIN CREATE TABLE [lov] (
    [id] int NOT NULL,
    [code] nvarchar(50) NULL,
    [stxt] nvarchar(200) NULL,
    [ltxt] nvarchar(500) NULL,
    [vtype] nvarchar(20) NULL,
    [utblock] bit NOT NULL,
    CONSTRAINT [PK_lov] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[lov]', N'id') IS NULL ALTER TABLE [lov] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[lov]', N'code') IS NULL ALTER TABLE [lov] ADD [code] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[lov]', N'stxt') IS NULL ALTER TABLE [lov] ADD [stxt] nvarchar(200) NULL;
GO
IF COL_LENGTH(N'[lov]', N'ltxt') IS NULL ALTER TABLE [lov] ADD [ltxt] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[lov]', N'vtype') IS NULL ALTER TABLE [lov] ADD [vtype] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[lov]', N'utblock') IS NULL ALTER TABLE [lov] ADD [utblock] bit NULL;
GO
IF OBJECT_ID(N'[OrgDepartment]', N'U') IS NULL BEGIN CREATE TABLE [OrgDepartment] (
    [Id] int NOT NULL,
    [depcode] nvarchar(50) NULL,
    [depstxt] nvarchar(50) NULL,
    [clientid] int NULL,
    [opunitid] int NULL,
    [utblock] bit NULL,
    CONSTRAINT [PK_OrgDepartment] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[OrgDepartment]', N'Id') IS NULL ALTER TABLE [OrgDepartment] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[OrgDepartment]', N'depcode') IS NULL ALTER TABLE [OrgDepartment] ADD [depcode] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[OrgDepartment]', N'depstxt') IS NULL ALTER TABLE [OrgDepartment] ADD [depstxt] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[OrgDepartment]', N'clientid') IS NULL ALTER TABLE [OrgDepartment] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[OrgDepartment]', N'opunitid') IS NULL ALTER TABLE [OrgDepartment] ADD [opunitid] int NULL;
GO
IF COL_LENGTH(N'[OrgDepartment]', N'utblock') IS NULL ALTER TABLE [OrgDepartment] ADD [utblock] bit NULL;
GO
IF OBJECT_ID(N'[OrgDesignation]', N'U') IS NULL BEGIN CREATE TABLE [OrgDesignation] (
    [Id] int NOT NULL,
    [desgcode] nvarchar(50) NULL,
    [stxt] nvarchar(50) NULL,
    [clientid] int NULL,
    [utblock] bit NULL,
    CONSTRAINT [PK_OrgDesignation] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[OrgDesignation]', N'Id') IS NULL ALTER TABLE [OrgDesignation] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[OrgDesignation]', N'desgcode') IS NULL ALTER TABLE [OrgDesignation] ADD [desgcode] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[OrgDesignation]', N'stxt') IS NULL ALTER TABLE [OrgDesignation] ADD [stxt] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[OrgDesignation]', N'clientid') IS NULL ALTER TABLE [OrgDesignation] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[OrgDesignation]', N'utblock') IS NULL ALTER TABLE [OrgDesignation] ADD [utblock] bit NULL;
GO
IF OBJECT_ID(N'[orgincomtaxhdr]', N'U') IS NULL BEGIN CREATE TABLE [orgincomtaxhdr] (
    [Id] int NOT NULL,
    [code] nvarchar(10) NULL,
    [stxt] nvarchar(500) NULL,
    [clientid] int NULL,
    [utblock] bit NOT NULL,
    CONSTRAINT [PK_orgincomtaxhdr] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[orgincomtaxhdr]', N'Id') IS NULL ALTER TABLE [orgincomtaxhdr] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[orgincomtaxhdr]', N'code') IS NULL ALTER TABLE [orgincomtaxhdr] ADD [code] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[orgincomtaxhdr]', N'stxt') IS NULL ALTER TABLE [orgincomtaxhdr] ADD [stxt] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[orgincomtaxhdr]', N'clientid') IS NULL ALTER TABLE [orgincomtaxhdr] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[orgincomtaxhdr]', N'utblock') IS NULL ALTER TABLE [orgincomtaxhdr] ADD [utblock] bit NULL;
GO
IF OBJECT_ID(N'[OrgLegalentity]', N'U') IS NULL BEGIN CREATE TABLE [OrgLegalentity] (
    [id] int NOT NULL,
    [clientid] int NULL,
    [entityno] nvarchar(20) NULL,
    [shortname] nvarchar(50) NULL,
    [stxt] nvarchar(100) NULL,
    [campusname] nvarchar(150) NULL,
    [headentity] int NULL,
    [lovcampustypeid] int NULL,
    [countryid] int NULL,
    [cityid] int NULL,
    [utblock] bit NOT NULL,
    CONSTRAINT [PK_OrgLegalentity] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'id') IS NULL ALTER TABLE [OrgLegalentity] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'clientid') IS NULL ALTER TABLE [OrgLegalentity] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'entityno') IS NULL ALTER TABLE [OrgLegalentity] ADD [entityno] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'shortname') IS NULL ALTER TABLE [OrgLegalentity] ADD [shortname] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'stxt') IS NULL ALTER TABLE [OrgLegalentity] ADD [stxt] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'campusname') IS NULL ALTER TABLE [OrgLegalentity] ADD [campusname] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'headentity') IS NULL ALTER TABLE [OrgLegalentity] ADD [headentity] int NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'lovcampustypeid') IS NULL ALTER TABLE [OrgLegalentity] ADD [lovcampustypeid] int NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'countryid') IS NULL ALTER TABLE [OrgLegalentity] ADD [countryid] int NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'cityid') IS NULL ALTER TABLE [OrgLegalentity] ADD [cityid] int NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'utblock') IS NULL ALTER TABLE [OrgLegalentity] ADD [utblock] bit NULL;
GO
IF OBJECT_ID(N'[Orgsite]', N'U') IS NULL BEGIN CREATE TABLE [Orgsite] (
    [Id] int NOT NULL,
    [code] nvarchar(10) NULL,
    [stxt] nvarchar(500) NULL,
    [entityid] int NULL,
    [clientid] int NULL,
    [utblock] bit NOT NULL,
    CONSTRAINT [PK_Orgsite] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[Orgsite]', N'Id') IS NULL ALTER TABLE [Orgsite] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[Orgsite]', N'code') IS NULL ALTER TABLE [Orgsite] ADD [code] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[Orgsite]', N'stxt') IS NULL ALTER TABLE [Orgsite] ADD [stxt] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[Orgsite]', N'entityid') IS NULL ALTER TABLE [Orgsite] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[Orgsite]', N'clientid') IS NULL ALTER TABLE [Orgsite] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[Orgsite]', N'utblock') IS NULL ALTER TABLE [Orgsite] ADD [utblock] bit NULL;
GO
IF OBJECT_ID(N'[SMAcadpolicy]', N'U') IS NULL BEGIN CREATE TABLE [SMAcadpolicy] (
    [Id] int NOT NULL,
    [clientid] int NULL,
    [entityid] int NULL,
    [useVacancyProcess] bit NULL,
    [utblock] bit NULL,
    CONSTRAINT [PK_SMAcadpolicy] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[SMAcadpolicy]', N'Id') IS NULL ALTER TABLE [SMAcadpolicy] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[SMAcadpolicy]', N'clientid') IS NULL ALTER TABLE [SMAcadpolicy] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[SMAcadpolicy]', N'entityid') IS NULL ALTER TABLE [SMAcadpolicy] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[SMAcadpolicy]', N'useVacancyProcess') IS NULL ALTER TABLE [SMAcadpolicy] ADD [useVacancyProcess] bit NULL;
GO
IF COL_LENGTH(N'[SMAcadpolicy]', N'utblock') IS NULL ALTER TABLE [SMAcadpolicy] ADD [utblock] bit NULL;
GO
IF OBJECT_ID(N'[SMCourse]', N'U') IS NULL BEGIN CREATE TABLE [SMCourse] (
    [Id] int NOT NULL,
    [clientid] int NULL,
    [utblock] bit NOT NULL,
    CONSTRAINT [PK_SMCourse] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[SMCourse]', N'Id') IS NULL ALTER TABLE [SMCourse] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[SMCourse]', N'clientid') IS NULL ALTER TABLE [SMCourse] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[SMCourse]', N'utblock') IS NULL ALTER TABLE [SMCourse] ADD [utblock] bit NULL;
GO
IF OBJECT_ID(N'[SMCustomshiftdtl]', N'U') IS NULL BEGIN CREATE TABLE [SMCustomshiftdtl] (
    [id] int NOT NULL,
    [shiftid] int NULL,
    [lineindex] int NULL,
    [stxt] nvarchar(50) NULL,
    [starttime] datetime2 NULL,
    [endtime] datetime2 NULL,
    [duration] float NULL,
    [utdoctype] int NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddip] varchar(25) NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddsessionid] int NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [uteditsystem] varchar(25) NULL,
    [uteditip] varchar(25) NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditsessionid] int NULL,
    [utrecordversion] int NOT NULL,
    [utblock] bit NOT NULL,
    CONSTRAINT [PK_SMCustomshiftdtl] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[SMCustomshiftdtl]', N'id') IS NULL ALTER TABLE [SMCustomshiftdtl] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[SMCustomshiftdtl]', N'shiftid') IS NULL ALTER TABLE [SMCustomshiftdtl] ADD [shiftid] int NULL;
GO
IF COL_LENGTH(N'[SMCustomshiftdtl]', N'lineindex') IS NULL ALTER TABLE [SMCustomshiftdtl] ADD [lineindex] int NULL;
GO
IF COL_LENGTH(N'[SMCustomshiftdtl]', N'stxt') IS NULL ALTER TABLE [SMCustomshiftdtl] ADD [stxt] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[SMCustomshiftdtl]', N'starttime') IS NULL ALTER TABLE [SMCustomshiftdtl] ADD [starttime] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMCustomshiftdtl]', N'endtime') IS NULL ALTER TABLE [SMCustomshiftdtl] ADD [endtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMCustomshiftdtl]', N'duration') IS NULL ALTER TABLE [SMCustomshiftdtl] ADD [duration] float NULL;
GO
IF COL_LENGTH(N'[SMCustomshiftdtl]', N'utdoctype') IS NULL ALTER TABLE [SMCustomshiftdtl] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[SMCustomshiftdtl]', N'utsubdoctype') IS NULL ALTER TABLE [SMCustomshiftdtl] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[SMCustomshiftdtl]', N'utsysgen') IS NULL ALTER TABLE [SMCustomshiftdtl] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[SMCustomshiftdtl]', N'utispartial') IS NULL ALTER TABLE [SMCustomshiftdtl] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[SMCustomshiftdtl]', N'utaddtime') IS NULL ALTER TABLE [SMCustomshiftdtl] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMCustomshiftdtl]', N'utadduser') IS NULL ALTER TABLE [SMCustomshiftdtl] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMCustomshiftdtl]', N'utaddsystem') IS NULL ALTER TABLE [SMCustomshiftdtl] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMCustomshiftdtl]', N'utaddip') IS NULL ALTER TABLE [SMCustomshiftdtl] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMCustomshiftdtl]', N'utaddappversion') IS NULL ALTER TABLE [SMCustomshiftdtl] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMCustomshiftdtl]', N'utadddbversion') IS NULL ALTER TABLE [SMCustomshiftdtl] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMCustomshiftdtl]', N'utaddid') IS NULL ALTER TABLE [SMCustomshiftdtl] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[SMCustomshiftdtl]', N'utaddsessionid') IS NULL ALTER TABLE [SMCustomshiftdtl] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[SMCustomshiftdtl]', N'utedittime') IS NULL ALTER TABLE [SMCustomshiftdtl] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMCustomshiftdtl]', N'utedituser') IS NULL ALTER TABLE [SMCustomshiftdtl] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMCustomshiftdtl]', N'uteditsystem') IS NULL ALTER TABLE [SMCustomshiftdtl] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMCustomshiftdtl]', N'uteditip') IS NULL ALTER TABLE [SMCustomshiftdtl] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMCustomshiftdtl]', N'uteditappversion') IS NULL ALTER TABLE [SMCustomshiftdtl] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMCustomshiftdtl]', N'uteditdbversion') IS NULL ALTER TABLE [SMCustomshiftdtl] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMCustomshiftdtl]', N'uteditid') IS NULL ALTER TABLE [SMCustomshiftdtl] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[SMCustomshiftdtl]', N'uteditsessionid') IS NULL ALTER TABLE [SMCustomshiftdtl] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[SMCustomshiftdtl]', N'utrecordversion') IS NULL ALTER TABLE [SMCustomshiftdtl] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[SMCustomshiftdtl]', N'utblock') IS NULL ALTER TABLE [SMCustomshiftdtl] ADD [utblock] bit NULL;
GO
IF OBJECT_ID(N'[SMCustomshifthdr]', N'U') IS NULL BEGIN CREATE TABLE [SMCustomshifthdr] (
    [id] int NOT NULL,
    [code] nvarchar(20) NULL,
    [stxt] nvarchar(50) NULL,
    [starttime] datetime2 NULL,
    [endtime] datetime2 NULL,
    [gracetime] int NULL,
    [entityid] int NULL,
    [clientid] int NULL,
    [utdoctype] int NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddip] varchar(25) NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddsessionid] int NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [uteditsystem] varchar(25) NULL,
    [uteditip] varchar(25) NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditsessionid] int NULL,
    [utrecordversion] int NOT NULL,
    [utblock] bit NOT NULL,
    [shifttypeid] int NULL,
    [womonday] bit NOT NULL,
    [wotuesday] bit NOT NULL,
    [wowednesday] bit NOT NULL,
    [wothuresday] bit NOT NULL,
    [wofriday] bit NOT NULL,
    [wosaturday] bit NOT NULL,
    [wosunday] bit NOT NULL,
    [gracetimeed] int NULL,
    CONSTRAINT [PK_SMCustomshifthdr] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[SMCustomshifthdr]', N'id') IS NULL ALTER TABLE [SMCustomshifthdr] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[SMCustomshifthdr]', N'code') IS NULL ALTER TABLE [SMCustomshifthdr] ADD [code] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[SMCustomshifthdr]', N'stxt') IS NULL ALTER TABLE [SMCustomshifthdr] ADD [stxt] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[SMCustomshifthdr]', N'starttime') IS NULL ALTER TABLE [SMCustomshifthdr] ADD [starttime] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMCustomshifthdr]', N'endtime') IS NULL ALTER TABLE [SMCustomshifthdr] ADD [endtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMCustomshifthdr]', N'gracetime') IS NULL ALTER TABLE [SMCustomshifthdr] ADD [gracetime] int NULL;
GO
IF COL_LENGTH(N'[SMCustomshifthdr]', N'entityid') IS NULL ALTER TABLE [SMCustomshifthdr] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[SMCustomshifthdr]', N'clientid') IS NULL ALTER TABLE [SMCustomshifthdr] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[SMCustomshifthdr]', N'utdoctype') IS NULL ALTER TABLE [SMCustomshifthdr] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[SMCustomshifthdr]', N'utsubdoctype') IS NULL ALTER TABLE [SMCustomshifthdr] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[SMCustomshifthdr]', N'utsysgen') IS NULL ALTER TABLE [SMCustomshifthdr] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[SMCustomshifthdr]', N'utispartial') IS NULL ALTER TABLE [SMCustomshifthdr] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[SMCustomshifthdr]', N'utaddtime') IS NULL ALTER TABLE [SMCustomshifthdr] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMCustomshifthdr]', N'utadduser') IS NULL ALTER TABLE [SMCustomshifthdr] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMCustomshifthdr]', N'utaddsystem') IS NULL ALTER TABLE [SMCustomshifthdr] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMCustomshifthdr]', N'utaddip') IS NULL ALTER TABLE [SMCustomshifthdr] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMCustomshifthdr]', N'utaddappversion') IS NULL ALTER TABLE [SMCustomshifthdr] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMCustomshifthdr]', N'utadddbversion') IS NULL ALTER TABLE [SMCustomshifthdr] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMCustomshifthdr]', N'utaddid') IS NULL ALTER TABLE [SMCustomshifthdr] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[SMCustomshifthdr]', N'utaddsessionid') IS NULL ALTER TABLE [SMCustomshifthdr] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[SMCustomshifthdr]', N'utedittime') IS NULL ALTER TABLE [SMCustomshifthdr] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMCustomshifthdr]', N'utedituser') IS NULL ALTER TABLE [SMCustomshifthdr] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMCustomshifthdr]', N'uteditsystem') IS NULL ALTER TABLE [SMCustomshifthdr] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMCustomshifthdr]', N'uteditip') IS NULL ALTER TABLE [SMCustomshifthdr] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMCustomshifthdr]', N'uteditappversion') IS NULL ALTER TABLE [SMCustomshifthdr] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMCustomshifthdr]', N'uteditdbversion') IS NULL ALTER TABLE [SMCustomshifthdr] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMCustomshifthdr]', N'uteditid') IS NULL ALTER TABLE [SMCustomshifthdr] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[SMCustomshifthdr]', N'uteditsessionid') IS NULL ALTER TABLE [SMCustomshifthdr] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[SMCustomshifthdr]', N'utrecordversion') IS NULL ALTER TABLE [SMCustomshifthdr] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[SMCustomshifthdr]', N'utblock') IS NULL ALTER TABLE [SMCustomshifthdr] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[SMCustomshifthdr]', N'shifttypeid') IS NULL ALTER TABLE [SMCustomshifthdr] ADD [shifttypeid] int NULL;
GO
IF COL_LENGTH(N'[SMCustomshifthdr]', N'womonday') IS NULL ALTER TABLE [SMCustomshifthdr] ADD [womonday] bit NULL;
GO
IF COL_LENGTH(N'[SMCustomshifthdr]', N'wotuesday') IS NULL ALTER TABLE [SMCustomshifthdr] ADD [wotuesday] bit NULL;
GO
IF COL_LENGTH(N'[SMCustomshifthdr]', N'wowednesday') IS NULL ALTER TABLE [SMCustomshifthdr] ADD [wowednesday] bit NULL;
GO
IF COL_LENGTH(N'[SMCustomshifthdr]', N'wothuresday') IS NULL ALTER TABLE [SMCustomshifthdr] ADD [wothuresday] bit NULL;
GO
IF COL_LENGTH(N'[SMCustomshifthdr]', N'wofriday') IS NULL ALTER TABLE [SMCustomshifthdr] ADD [wofriday] bit NULL;
GO
IF COL_LENGTH(N'[SMCustomshifthdr]', N'wosaturday') IS NULL ALTER TABLE [SMCustomshifthdr] ADD [wosaturday] bit NULL;
GO
IF COL_LENGTH(N'[SMCustomshifthdr]', N'wosunday') IS NULL ALTER TABLE [SMCustomshifthdr] ADD [wosunday] bit NULL;
GO
IF COL_LENGTH(N'[SMCustomshifthdr]', N'gracetimeed') IS NULL ALTER TABLE [SMCustomshifthdr] ADD [gracetimeed] int NULL;
GO
IF OBJECT_ID(N'[SMDevicetype]', N'U') IS NULL BEGIN CREATE TABLE [SMDevicetype] (
    [Id] int NOT NULL,
    [code] nvarchar(10) NULL,
    [stxt] nvarchar(150) NULL,
    [ltxt] nvarchar(500) NULL,
    [clientid] int NULL,
    [utsysgen] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utblock] bit NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_SMDevicetype] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[SMDevicetype]', N'Id') IS NULL ALTER TABLE [SMDevicetype] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[SMDevicetype]', N'code') IS NULL ALTER TABLE [SMDevicetype] ADD [code] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[SMDevicetype]', N'stxt') IS NULL ALTER TABLE [SMDevicetype] ADD [stxt] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[SMDevicetype]', N'ltxt') IS NULL ALTER TABLE [SMDevicetype] ADD [ltxt] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[SMDevicetype]', N'clientid') IS NULL ALTER TABLE [SMDevicetype] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[SMDevicetype]', N'utsysgen') IS NULL ALTER TABLE [SMDevicetype] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[SMDevicetype]', N'utispartial') IS NULL ALTER TABLE [SMDevicetype] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[SMDevicetype]', N'utrecordversion') IS NULL ALTER TABLE [SMDevicetype] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[SMDevicetype]', N'utblock') IS NULL ALTER TABLE [SMDevicetype] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[SMDevicetype]', N'utaddappversion') IS NULL ALTER TABLE [SMDevicetype] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMDevicetype]', N'utadddbversion') IS NULL ALTER TABLE [SMDevicetype] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMDevicetype]', N'utaddid') IS NULL ALTER TABLE [SMDevicetype] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[SMDevicetype]', N'utaddip') IS NULL ALTER TABLE [SMDevicetype] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMDevicetype]', N'utaddsessionid') IS NULL ALTER TABLE [SMDevicetype] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[SMDevicetype]', N'utaddsystem') IS NULL ALTER TABLE [SMDevicetype] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMDevicetype]', N'utaddtime') IS NULL ALTER TABLE [SMDevicetype] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMDevicetype]', N'utadduser') IS NULL ALTER TABLE [SMDevicetype] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMDevicetype]', N'utdoctype') IS NULL ALTER TABLE [SMDevicetype] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[SMDevicetype]', N'uteditappversion') IS NULL ALTER TABLE [SMDevicetype] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMDevicetype]', N'uteditdbversion') IS NULL ALTER TABLE [SMDevicetype] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMDevicetype]', N'uteditid') IS NULL ALTER TABLE [SMDevicetype] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[SMDevicetype]', N'uteditip') IS NULL ALTER TABLE [SMDevicetype] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMDevicetype]', N'uteditsessionid') IS NULL ALTER TABLE [SMDevicetype] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[SMDevicetype]', N'uteditsystem') IS NULL ALTER TABLE [SMDevicetype] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMDevicetype]', N'utedittime') IS NULL ALTER TABLE [SMDevicetype] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMDevicetype]', N'utedituser') IS NULL ALTER TABLE [SMDevicetype] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMDevicetype]', N'utsubdoctype') IS NULL ALTER TABLE [SMDevicetype] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[SMDisease]', N'U') IS NULL BEGIN CREATE TABLE [SMDisease] (
    [id] int NOT NULL,
    [code] nvarchar(20) NULL,
    [lovdiseasecat] int NULL,
    [lovdiseasetype] int NULL,
    [stxt] nvarchar(150) NULL,
    [ltxt] nvarchar(500) NULL,
    [clientid] int NULL,
    [entityid] int NULL,
    [utblock] bit NOT NULL,
    CONSTRAINT [PK_SMDisease] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[SMDisease]', N'id') IS NULL ALTER TABLE [SMDisease] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[SMDisease]', N'code') IS NULL ALTER TABLE [SMDisease] ADD [code] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[SMDisease]', N'lovdiseasecat') IS NULL ALTER TABLE [SMDisease] ADD [lovdiseasecat] int NULL;
GO
IF COL_LENGTH(N'[SMDisease]', N'lovdiseasetype') IS NULL ALTER TABLE [SMDisease] ADD [lovdiseasetype] int NULL;
GO
IF COL_LENGTH(N'[SMDisease]', N'stxt') IS NULL ALTER TABLE [SMDisease] ADD [stxt] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[SMDisease]', N'ltxt') IS NULL ALTER TABLE [SMDisease] ADD [ltxt] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[SMDisease]', N'clientid') IS NULL ALTER TABLE [SMDisease] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[SMDisease]', N'entityid') IS NULL ALTER TABLE [SMDisease] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[SMDisease]', N'utblock') IS NULL ALTER TABLE [SMDisease] ADD [utblock] bit NULL;
GO
IF OBJECT_ID(N'[SMMachineRegistration]', N'U') IS NULL BEGIN CREATE TABLE [SMMachineRegistration] (
    [Id] int NOT NULL,
    [code] nvarchar(50) NULL,
    [stxt] nvarchar(100) NULL,
    [ltxt] nvarchar(500) NULL,
    [lovmachinetypeid] int NULL,
    [machineip] nvarchar(20) NULL,
    [port] int NULL,
    [inactive] bit NOT NULL,
    [entityid] int NULL,
    [clientid] int NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [siteid] int NULL,
    [devicetypeid] int NULL,
    [convtytypeid] int NULL,
    [deviceid] int NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_SMMachineRegistration] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[SMMachineRegistration]', N'Id') IS NULL ALTER TABLE [SMMachineRegistration] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[SMMachineRegistration]', N'code') IS NULL ALTER TABLE [SMMachineRegistration] ADD [code] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[SMMachineRegistration]', N'stxt') IS NULL ALTER TABLE [SMMachineRegistration] ADD [stxt] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[SMMachineRegistration]', N'ltxt') IS NULL ALTER TABLE [SMMachineRegistration] ADD [ltxt] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[SMMachineRegistration]', N'lovmachinetypeid') IS NULL ALTER TABLE [SMMachineRegistration] ADD [lovmachinetypeid] int NULL;
GO
IF COL_LENGTH(N'[SMMachineRegistration]', N'machineip') IS NULL ALTER TABLE [SMMachineRegistration] ADD [machineip] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[SMMachineRegistration]', N'port') IS NULL ALTER TABLE [SMMachineRegistration] ADD [port] int NULL;
GO
IF COL_LENGTH(N'[SMMachineRegistration]', N'inactive') IS NULL ALTER TABLE [SMMachineRegistration] ADD [inactive] bit NULL;
GO
IF COL_LENGTH(N'[SMMachineRegistration]', N'entityid') IS NULL ALTER TABLE [SMMachineRegistration] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[SMMachineRegistration]', N'clientid') IS NULL ALTER TABLE [SMMachineRegistration] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[SMMachineRegistration]', N'utsysgen') IS NULL ALTER TABLE [SMMachineRegistration] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[SMMachineRegistration]', N'utblock') IS NULL ALTER TABLE [SMMachineRegistration] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[SMMachineRegistration]', N'utispartial') IS NULL ALTER TABLE [SMMachineRegistration] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[SMMachineRegistration]', N'utrecordversion') IS NULL ALTER TABLE [SMMachineRegistration] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[SMMachineRegistration]', N'siteid') IS NULL ALTER TABLE [SMMachineRegistration] ADD [siteid] int NULL;
GO
IF COL_LENGTH(N'[SMMachineRegistration]', N'devicetypeid') IS NULL ALTER TABLE [SMMachineRegistration] ADD [devicetypeid] int NULL;
GO
IF COL_LENGTH(N'[SMMachineRegistration]', N'convtytypeid') IS NULL ALTER TABLE [SMMachineRegistration] ADD [convtytypeid] int NULL;
GO
IF COL_LENGTH(N'[SMMachineRegistration]', N'deviceid') IS NULL ALTER TABLE [SMMachineRegistration] ADD [deviceid] int NULL;
GO
IF COL_LENGTH(N'[SMMachineRegistration]', N'utaddappversion') IS NULL ALTER TABLE [SMMachineRegistration] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMMachineRegistration]', N'utadddbversion') IS NULL ALTER TABLE [SMMachineRegistration] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMMachineRegistration]', N'utaddid') IS NULL ALTER TABLE [SMMachineRegistration] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[SMMachineRegistration]', N'utaddip') IS NULL ALTER TABLE [SMMachineRegistration] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMMachineRegistration]', N'utaddsessionid') IS NULL ALTER TABLE [SMMachineRegistration] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[SMMachineRegistration]', N'utaddsystem') IS NULL ALTER TABLE [SMMachineRegistration] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMMachineRegistration]', N'utaddtime') IS NULL ALTER TABLE [SMMachineRegistration] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMMachineRegistration]', N'utadduser') IS NULL ALTER TABLE [SMMachineRegistration] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMMachineRegistration]', N'utdoctype') IS NULL ALTER TABLE [SMMachineRegistration] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[SMMachineRegistration]', N'uteditappversion') IS NULL ALTER TABLE [SMMachineRegistration] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMMachineRegistration]', N'uteditdbversion') IS NULL ALTER TABLE [SMMachineRegistration] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMMachineRegistration]', N'uteditid') IS NULL ALTER TABLE [SMMachineRegistration] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[SMMachineRegistration]', N'uteditip') IS NULL ALTER TABLE [SMMachineRegistration] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMMachineRegistration]', N'uteditsessionid') IS NULL ALTER TABLE [SMMachineRegistration] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[SMMachineRegistration]', N'uteditsystem') IS NULL ALTER TABLE [SMMachineRegistration] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMMachineRegistration]', N'utedittime') IS NULL ALTER TABLE [SMMachineRegistration] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMMachineRegistration]', N'utedituser') IS NULL ALTER TABLE [SMMachineRegistration] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMMachineRegistration]', N'utsubdoctype') IS NULL ALTER TABLE [SMMachineRegistration] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[SMmeetinghostinfo]', N'U') IS NULL BEGIN CREATE TABLE [SMmeetinghostinfo] (
    [Id] int NOT NULL,
    [employeeid] int NULL,
    [clientid] int NULL,
    [lovtypeid] int NULL,
    [username] nvarchar(500) NULL,
    [pwd] nvarchar(500) NULL,
    [meetingid] nvarchar(50) NULL,
    [meetingpwd] nvarchar(50) NULL,
    [invitepwd] nvarchar(max) NULL,
    [jwttoken] nvarchar(max) NULL,
    [apiKey] nvarchar(500) NULL,
    [apiSecret] nvarchar(500) NULL,
    [sdkkey] nvarchar(max) NULL,
    [sdksecret] nvarchar(max) NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utaddappversion] nvarchar(100) NULL,
    [utadddbversion] nvarchar(100) NULL,
    [utaddid] bigint NULL,
    [utaddip] nvarchar(100) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] nvarchar(100) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] nvarchar(100) NULL,
    [utdoctype] int NULL,
    [uteditappversion] nvarchar(50) NULL,
    [uteditdbversion] nvarchar(50) NULL,
    [uteditid] bigint NULL,
    [uteditip] nvarchar(50) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] nvarchar(50) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] nvarchar(50) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_SMmeetinghostinfo] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[SMmeetinghostinfo]', N'Id') IS NULL ALTER TABLE [SMmeetinghostinfo] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[SMmeetinghostinfo]', N'employeeid') IS NULL ALTER TABLE [SMmeetinghostinfo] ADD [employeeid] int NULL;
GO
IF COL_LENGTH(N'[SMmeetinghostinfo]', N'clientid') IS NULL ALTER TABLE [SMmeetinghostinfo] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[SMmeetinghostinfo]', N'lovtypeid') IS NULL ALTER TABLE [SMmeetinghostinfo] ADD [lovtypeid] int NULL;
GO
IF COL_LENGTH(N'[SMmeetinghostinfo]', N'username') IS NULL ALTER TABLE [SMmeetinghostinfo] ADD [username] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[SMmeetinghostinfo]', N'pwd') IS NULL ALTER TABLE [SMmeetinghostinfo] ADD [pwd] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[SMmeetinghostinfo]', N'meetingid') IS NULL ALTER TABLE [SMmeetinghostinfo] ADD [meetingid] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[SMmeetinghostinfo]', N'meetingpwd') IS NULL ALTER TABLE [SMmeetinghostinfo] ADD [meetingpwd] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[SMmeetinghostinfo]', N'invitepwd') IS NULL ALTER TABLE [SMmeetinghostinfo] ADD [invitepwd] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[SMmeetinghostinfo]', N'jwttoken') IS NULL ALTER TABLE [SMmeetinghostinfo] ADD [jwttoken] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[SMmeetinghostinfo]', N'apiKey') IS NULL ALTER TABLE [SMmeetinghostinfo] ADD [apiKey] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[SMmeetinghostinfo]', N'apiSecret') IS NULL ALTER TABLE [SMmeetinghostinfo] ADD [apiSecret] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[SMmeetinghostinfo]', N'sdkkey') IS NULL ALTER TABLE [SMmeetinghostinfo] ADD [sdkkey] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[SMmeetinghostinfo]', N'sdksecret') IS NULL ALTER TABLE [SMmeetinghostinfo] ADD [sdksecret] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[SMmeetinghostinfo]', N'utsysgen') IS NULL ALTER TABLE [SMmeetinghostinfo] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[SMmeetinghostinfo]', N'utblock') IS NULL ALTER TABLE [SMmeetinghostinfo] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[SMmeetinghostinfo]', N'utispartial') IS NULL ALTER TABLE [SMmeetinghostinfo] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[SMmeetinghostinfo]', N'utrecordversion') IS NULL ALTER TABLE [SMmeetinghostinfo] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[SMmeetinghostinfo]', N'utaddappversion') IS NULL ALTER TABLE [SMmeetinghostinfo] ADD [utaddappversion] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[SMmeetinghostinfo]', N'utadddbversion') IS NULL ALTER TABLE [SMmeetinghostinfo] ADD [utadddbversion] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[SMmeetinghostinfo]', N'utaddid') IS NULL ALTER TABLE [SMmeetinghostinfo] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[SMmeetinghostinfo]', N'utaddip') IS NULL ALTER TABLE [SMmeetinghostinfo] ADD [utaddip] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[SMmeetinghostinfo]', N'utaddsessionid') IS NULL ALTER TABLE [SMmeetinghostinfo] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[SMmeetinghostinfo]', N'utaddsystem') IS NULL ALTER TABLE [SMmeetinghostinfo] ADD [utaddsystem] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[SMmeetinghostinfo]', N'utaddtime') IS NULL ALTER TABLE [SMmeetinghostinfo] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMmeetinghostinfo]', N'utadduser') IS NULL ALTER TABLE [SMmeetinghostinfo] ADD [utadduser] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[SMmeetinghostinfo]', N'utdoctype') IS NULL ALTER TABLE [SMmeetinghostinfo] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[SMmeetinghostinfo]', N'uteditappversion') IS NULL ALTER TABLE [SMmeetinghostinfo] ADD [uteditappversion] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[SMmeetinghostinfo]', N'uteditdbversion') IS NULL ALTER TABLE [SMmeetinghostinfo] ADD [uteditdbversion] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[SMmeetinghostinfo]', N'uteditid') IS NULL ALTER TABLE [SMmeetinghostinfo] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[SMmeetinghostinfo]', N'uteditip') IS NULL ALTER TABLE [SMmeetinghostinfo] ADD [uteditip] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[SMmeetinghostinfo]', N'uteditsessionid') IS NULL ALTER TABLE [SMmeetinghostinfo] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[SMmeetinghostinfo]', N'uteditsystem') IS NULL ALTER TABLE [SMmeetinghostinfo] ADD [uteditsystem] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[SMmeetinghostinfo]', N'utedittime') IS NULL ALTER TABLE [SMmeetinghostinfo] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMmeetinghostinfo]', N'utedituser') IS NULL ALTER TABLE [SMmeetinghostinfo] ADD [utedituser] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[SMmeetinghostinfo]', N'utsubdoctype') IS NULL ALTER TABLE [SMmeetinghostinfo] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[SMPolicy]', N'U') IS NULL BEGIN CREATE TABLE [SMPolicy] (
    [Id] int NOT NULL,
    [clientid] int NULL,
    [entityid] int NULL,
    [defaultctryid] int NULL,
    [defaultcityid] int NULL,
    [utblock] bit NULL,
    CONSTRAINT [PK_SMPolicy] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[SMPolicy]', N'Id') IS NULL ALTER TABLE [SMPolicy] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[SMPolicy]', N'clientid') IS NULL ALTER TABLE [SMPolicy] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[SMPolicy]', N'entityid') IS NULL ALTER TABLE [SMPolicy] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[SMPolicy]', N'defaultctryid') IS NULL ALTER TABLE [SMPolicy] ADD [defaultctryid] int NULL;
GO
IF COL_LENGTH(N'[SMPolicy]', N'defaultcityid') IS NULL ALTER TABLE [SMPolicy] ADD [defaultcityid] int NULL;
GO
IF COL_LENGTH(N'[SMPolicy]', N'utblock') IS NULL ALTER TABLE [SMPolicy] ADD [utblock] bit NULL;
GO
IF OBJECT_ID(N'[SMRelationtype]', N'U') IS NULL BEGIN CREATE TABLE [SMRelationtype] (
    [Id] int NOT NULL,
    [relationtypeno] nvarchar(15) NULL,
    [stxt] nvarchar(150) NULL,
    [ltxt] nvarchar(500) NULL,
    [clientid] int NULL,
    [utblock] bit NULL,
    CONSTRAINT [PK_SMRelationtype] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[SMRelationtype]', N'Id') IS NULL ALTER TABLE [SMRelationtype] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[SMRelationtype]', N'relationtypeno') IS NULL ALTER TABLE [SMRelationtype] ADD [relationtypeno] nvarchar(15) NULL;
GO
IF COL_LENGTH(N'[SMRelationtype]', N'stxt') IS NULL ALTER TABLE [SMRelationtype] ADD [stxt] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[SMRelationtype]', N'ltxt') IS NULL ALTER TABLE [SMRelationtype] ADD [ltxt] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[SMRelationtype]', N'clientid') IS NULL ALTER TABLE [SMRelationtype] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[SMRelationtype]', N'utblock') IS NULL ALTER TABLE [SMRelationtype] ADD [utblock] bit NULL;
GO
IF OBJECT_ID(N'[SMShifthdr]', N'U') IS NULL BEGIN CREATE TABLE [SMShifthdr] (
    [id] int NOT NULL,
    [code] nvarchar(20) NULL,
    [stxt] nvarchar(50) NULL,
    [starttime] datetime2 NULL,
    [endtime] datetime2 NULL,
    [gracetime] int NULL,
    [entityid] int NULL,
    [clientid] int NULL,
    [utsysgen] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utblock] bit NOT NULL,
    [womonday] bit NOT NULL,
    [wotuesday] bit NOT NULL,
    [wowednesday] bit NOT NULL,
    [wothuresday] bit NOT NULL,
    [wofriday] bit NOT NULL,
    [wosaturday] bit NOT NULL,
    [wosunday] bit NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_SMShifthdr] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[SMShifthdr]', N'id') IS NULL ALTER TABLE [SMShifthdr] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[SMShifthdr]', N'code') IS NULL ALTER TABLE [SMShifthdr] ADD [code] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[SMShifthdr]', N'stxt') IS NULL ALTER TABLE [SMShifthdr] ADD [stxt] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[SMShifthdr]', N'starttime') IS NULL ALTER TABLE [SMShifthdr] ADD [starttime] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMShifthdr]', N'endtime') IS NULL ALTER TABLE [SMShifthdr] ADD [endtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMShifthdr]', N'gracetime') IS NULL ALTER TABLE [SMShifthdr] ADD [gracetime] int NULL;
GO
IF COL_LENGTH(N'[SMShifthdr]', N'entityid') IS NULL ALTER TABLE [SMShifthdr] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[SMShifthdr]', N'clientid') IS NULL ALTER TABLE [SMShifthdr] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[SMShifthdr]', N'utsysgen') IS NULL ALTER TABLE [SMShifthdr] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[SMShifthdr]', N'utispartial') IS NULL ALTER TABLE [SMShifthdr] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[SMShifthdr]', N'utrecordversion') IS NULL ALTER TABLE [SMShifthdr] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[SMShifthdr]', N'utblock') IS NULL ALTER TABLE [SMShifthdr] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[SMShifthdr]', N'womonday') IS NULL ALTER TABLE [SMShifthdr] ADD [womonday] bit NULL;
GO
IF COL_LENGTH(N'[SMShifthdr]', N'wotuesday') IS NULL ALTER TABLE [SMShifthdr] ADD [wotuesday] bit NULL;
GO
IF COL_LENGTH(N'[SMShifthdr]', N'wowednesday') IS NULL ALTER TABLE [SMShifthdr] ADD [wowednesday] bit NULL;
GO
IF COL_LENGTH(N'[SMShifthdr]', N'wothuresday') IS NULL ALTER TABLE [SMShifthdr] ADD [wothuresday] bit NULL;
GO
IF COL_LENGTH(N'[SMShifthdr]', N'wofriday') IS NULL ALTER TABLE [SMShifthdr] ADD [wofriday] bit NULL;
GO
IF COL_LENGTH(N'[SMShifthdr]', N'wosaturday') IS NULL ALTER TABLE [SMShifthdr] ADD [wosaturday] bit NULL;
GO
IF COL_LENGTH(N'[SMShifthdr]', N'wosunday') IS NULL ALTER TABLE [SMShifthdr] ADD [wosunday] bit NULL;
GO
IF COL_LENGTH(N'[SMShifthdr]', N'utaddappversion') IS NULL ALTER TABLE [SMShifthdr] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMShifthdr]', N'utadddbversion') IS NULL ALTER TABLE [SMShifthdr] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMShifthdr]', N'utaddid') IS NULL ALTER TABLE [SMShifthdr] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[SMShifthdr]', N'utaddip') IS NULL ALTER TABLE [SMShifthdr] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMShifthdr]', N'utaddsessionid') IS NULL ALTER TABLE [SMShifthdr] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[SMShifthdr]', N'utaddsystem') IS NULL ALTER TABLE [SMShifthdr] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMShifthdr]', N'utaddtime') IS NULL ALTER TABLE [SMShifthdr] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMShifthdr]', N'utadduser') IS NULL ALTER TABLE [SMShifthdr] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMShifthdr]', N'utdoctype') IS NULL ALTER TABLE [SMShifthdr] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[SMShifthdr]', N'uteditappversion') IS NULL ALTER TABLE [SMShifthdr] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMShifthdr]', N'uteditdbversion') IS NULL ALTER TABLE [SMShifthdr] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMShifthdr]', N'uteditid') IS NULL ALTER TABLE [SMShifthdr] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[SMShifthdr]', N'uteditip') IS NULL ALTER TABLE [SMShifthdr] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMShifthdr]', N'uteditsessionid') IS NULL ALTER TABLE [SMShifthdr] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[SMShifthdr]', N'uteditsystem') IS NULL ALTER TABLE [SMShifthdr] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMShifthdr]', N'utedittime') IS NULL ALTER TABLE [SMShifthdr] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMShifthdr]', N'utedituser') IS NULL ALTER TABLE [SMShifthdr] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMShifthdr]', N'utsubdoctype') IS NULL ALTER TABLE [SMShifthdr] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[SMSubjecttype]', N'U') IS NULL BEGIN CREATE TABLE [SMSubjecttype] (
    [Id] int NOT NULL,
    [code] nvarchar(10) NULL,
    [stxt] nvarchar(200) NULL,
    [ltxt] nvarchar(500) NULL,
    [clientid] int NULL,
    [utblock] bit NULL,
    CONSTRAINT [PK_SMSubjecttype] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[SMSubjecttype]', N'Id') IS NULL ALTER TABLE [SMSubjecttype] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[SMSubjecttype]', N'code') IS NULL ALTER TABLE [SMSubjecttype] ADD [code] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[SMSubjecttype]', N'stxt') IS NULL ALTER TABLE [SMSubjecttype] ADD [stxt] nvarchar(200) NULL;
GO
IF COL_LENGTH(N'[SMSubjecttype]', N'ltxt') IS NULL ALTER TABLE [SMSubjecttype] ADD [ltxt] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[SMSubjecttype]', N'clientid') IS NULL ALTER TABLE [SMSubjecttype] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[SMSubjecttype]', N'utblock') IS NULL ALTER TABLE [SMSubjecttype] ADD [utblock] bit NULL;
GO
IF OBJECT_ID(N'[utinstance]', N'U') IS NULL BEGIN CREATE TABLE [utinstance] (
    [instanceid] int NOT NULL,
    [dbserver] nvarchar(150) NULL,
    [dbuserid] nvarchar(150) NULL,
    [dbpassword] nvarchar(150) NULL,
    [dbname] nvarchar(150) NULL,
    [utblock] bit NOT NULL,
    CONSTRAINT [PK_utinstance] PRIMARY KEY ([instanceid])
); END;
GO
IF COL_LENGTH(N'[utinstance]', N'instanceid') IS NULL ALTER TABLE [utinstance] ADD [instanceid] int NULL;
GO
IF COL_LENGTH(N'[utinstance]', N'dbserver') IS NULL ALTER TABLE [utinstance] ADD [dbserver] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[utinstance]', N'dbuserid') IS NULL ALTER TABLE [utinstance] ADD [dbuserid] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[utinstance]', N'dbpassword') IS NULL ALTER TABLE [utinstance] ADD [dbpassword] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[utinstance]', N'dbname') IS NULL ALTER TABLE [utinstance] ADD [dbname] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[utinstance]', N'utblock') IS NULL ALTER TABLE [utinstance] ADD [utblock] bit NULL;
GO
IF OBJECT_ID(N'[EPDeductiongrpdtl]', N'U') IS NULL BEGIN CREATE TABLE [EPDeductiongrpdtl] (
    [id] int NOT NULL,
    [dedgrpid] int NULL,
    [lineindex] int NULL,
    [dedtypeid] int NULL,
    [ratebasisid] int NULL,
    [basisearntypeid] int NULL,
    [value] float NULL,
    [payslipindex] int NULL,
    [paysliptxt] nvarchar(100) NULL,
    [statusid] int NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_EPDeductiongrpdtl] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[EPDeductiongrpdtl]', N'id') IS NULL ALTER TABLE [EPDeductiongrpdtl] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrpdtl]', N'dedgrpid') IS NULL ALTER TABLE [EPDeductiongrpdtl] ADD [dedgrpid] int NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrpdtl]', N'lineindex') IS NULL ALTER TABLE [EPDeductiongrpdtl] ADD [lineindex] int NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrpdtl]', N'dedtypeid') IS NULL ALTER TABLE [EPDeductiongrpdtl] ADD [dedtypeid] int NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrpdtl]', N'ratebasisid') IS NULL ALTER TABLE [EPDeductiongrpdtl] ADD [ratebasisid] int NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrpdtl]', N'basisearntypeid') IS NULL ALTER TABLE [EPDeductiongrpdtl] ADD [basisearntypeid] int NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrpdtl]', N'value') IS NULL ALTER TABLE [EPDeductiongrpdtl] ADD [value] float NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrpdtl]', N'payslipindex') IS NULL ALTER TABLE [EPDeductiongrpdtl] ADD [payslipindex] int NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrpdtl]', N'paysliptxt') IS NULL ALTER TABLE [EPDeductiongrpdtl] ADD [paysliptxt] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrpdtl]', N'statusid') IS NULL ALTER TABLE [EPDeductiongrpdtl] ADD [statusid] int NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrpdtl]', N'utsysgen') IS NULL ALTER TABLE [EPDeductiongrpdtl] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrpdtl]', N'utblock') IS NULL ALTER TABLE [EPDeductiongrpdtl] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrpdtl]', N'utispartial') IS NULL ALTER TABLE [EPDeductiongrpdtl] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrpdtl]', N'utrecordversion') IS NULL ALTER TABLE [EPDeductiongrpdtl] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrpdtl]', N'utaddappversion') IS NULL ALTER TABLE [EPDeductiongrpdtl] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrpdtl]', N'utadddbversion') IS NULL ALTER TABLE [EPDeductiongrpdtl] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrpdtl]', N'utaddid') IS NULL ALTER TABLE [EPDeductiongrpdtl] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrpdtl]', N'utaddip') IS NULL ALTER TABLE [EPDeductiongrpdtl] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrpdtl]', N'utaddsessionid') IS NULL ALTER TABLE [EPDeductiongrpdtl] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrpdtl]', N'utaddsystem') IS NULL ALTER TABLE [EPDeductiongrpdtl] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrpdtl]', N'utaddtime') IS NULL ALTER TABLE [EPDeductiongrpdtl] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrpdtl]', N'utadduser') IS NULL ALTER TABLE [EPDeductiongrpdtl] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrpdtl]', N'utdoctype') IS NULL ALTER TABLE [EPDeductiongrpdtl] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrpdtl]', N'uteditappversion') IS NULL ALTER TABLE [EPDeductiongrpdtl] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrpdtl]', N'uteditdbversion') IS NULL ALTER TABLE [EPDeductiongrpdtl] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrpdtl]', N'uteditid') IS NULL ALTER TABLE [EPDeductiongrpdtl] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrpdtl]', N'uteditip') IS NULL ALTER TABLE [EPDeductiongrpdtl] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrpdtl]', N'uteditsessionid') IS NULL ALTER TABLE [EPDeductiongrpdtl] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrpdtl]', N'uteditsystem') IS NULL ALTER TABLE [EPDeductiongrpdtl] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrpdtl]', N'utedittime') IS NULL ALTER TABLE [EPDeductiongrpdtl] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrpdtl]', N'utedituser') IS NULL ALTER TABLE [EPDeductiongrpdtl] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPDeductiongrpdtl]', N'utsubdoctype') IS NULL ALTER TABLE [EPDeductiongrpdtl] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[EPEarninggrpdtl]', N'U') IS NULL BEGIN CREATE TABLE [EPEarninggrpdtl] (
    [id] int NOT NULL,
    [earngrpid] int NULL,
    [lineindex] int NULL,
    [earntypeid] int NULL,
    [ratebasisid] int NULL,
    [basisearntypeid] int NULL,
    [value] float NULL,
    [absnoteffective] bit NOT NULL,
    [levnoteffective] bit NULL,
    [isotpart] bigint NULL,
    [payslipindex] int NULL,
    [paysliptxt] nvarchar(100) NULL,
    [statusid] int NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_EPEarninggrpdtl] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[EPEarninggrpdtl]', N'id') IS NULL ALTER TABLE [EPEarninggrpdtl] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[EPEarninggrpdtl]', N'earngrpid') IS NULL ALTER TABLE [EPEarninggrpdtl] ADD [earngrpid] int NULL;
GO
IF COL_LENGTH(N'[EPEarninggrpdtl]', N'lineindex') IS NULL ALTER TABLE [EPEarninggrpdtl] ADD [lineindex] int NULL;
GO
IF COL_LENGTH(N'[EPEarninggrpdtl]', N'earntypeid') IS NULL ALTER TABLE [EPEarninggrpdtl] ADD [earntypeid] int NULL;
GO
IF COL_LENGTH(N'[EPEarninggrpdtl]', N'ratebasisid') IS NULL ALTER TABLE [EPEarninggrpdtl] ADD [ratebasisid] int NULL;
GO
IF COL_LENGTH(N'[EPEarninggrpdtl]', N'basisearntypeid') IS NULL ALTER TABLE [EPEarninggrpdtl] ADD [basisearntypeid] int NULL;
GO
IF COL_LENGTH(N'[EPEarninggrpdtl]', N'value') IS NULL ALTER TABLE [EPEarninggrpdtl] ADD [value] float NULL;
GO
IF COL_LENGTH(N'[EPEarninggrpdtl]', N'absnoteffective') IS NULL ALTER TABLE [EPEarninggrpdtl] ADD [absnoteffective] bit NULL;
GO
IF COL_LENGTH(N'[EPEarninggrpdtl]', N'levnoteffective') IS NULL ALTER TABLE [EPEarninggrpdtl] ADD [levnoteffective] bit NULL;
GO
IF COL_LENGTH(N'[EPEarninggrpdtl]', N'isotpart') IS NULL ALTER TABLE [EPEarninggrpdtl] ADD [isotpart] bigint NULL;
GO
IF COL_LENGTH(N'[EPEarninggrpdtl]', N'payslipindex') IS NULL ALTER TABLE [EPEarninggrpdtl] ADD [payslipindex] int NULL;
GO
IF COL_LENGTH(N'[EPEarninggrpdtl]', N'paysliptxt') IS NULL ALTER TABLE [EPEarninggrpdtl] ADD [paysliptxt] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[EPEarninggrpdtl]', N'statusid') IS NULL ALTER TABLE [EPEarninggrpdtl] ADD [statusid] int NULL;
GO
IF COL_LENGTH(N'[EPEarninggrpdtl]', N'utsysgen') IS NULL ALTER TABLE [EPEarninggrpdtl] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[EPEarninggrpdtl]', N'utblock') IS NULL ALTER TABLE [EPEarninggrpdtl] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[EPEarninggrpdtl]', N'utispartial') IS NULL ALTER TABLE [EPEarninggrpdtl] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[EPEarninggrpdtl]', N'utrecordversion') IS NULL ALTER TABLE [EPEarninggrpdtl] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[EPEarninggrpdtl]', N'utaddappversion') IS NULL ALTER TABLE [EPEarninggrpdtl] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EPEarninggrpdtl]', N'utadddbversion') IS NULL ALTER TABLE [EPEarninggrpdtl] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EPEarninggrpdtl]', N'utaddid') IS NULL ALTER TABLE [EPEarninggrpdtl] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[EPEarninggrpdtl]', N'utaddip') IS NULL ALTER TABLE [EPEarninggrpdtl] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPEarninggrpdtl]', N'utaddsessionid') IS NULL ALTER TABLE [EPEarninggrpdtl] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[EPEarninggrpdtl]', N'utaddsystem') IS NULL ALTER TABLE [EPEarninggrpdtl] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPEarninggrpdtl]', N'utaddtime') IS NULL ALTER TABLE [EPEarninggrpdtl] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EPEarninggrpdtl]', N'utadduser') IS NULL ALTER TABLE [EPEarninggrpdtl] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPEarninggrpdtl]', N'utdoctype') IS NULL ALTER TABLE [EPEarninggrpdtl] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[EPEarninggrpdtl]', N'uteditappversion') IS NULL ALTER TABLE [EPEarninggrpdtl] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EPEarninggrpdtl]', N'uteditdbversion') IS NULL ALTER TABLE [EPEarninggrpdtl] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EPEarninggrpdtl]', N'uteditid') IS NULL ALTER TABLE [EPEarninggrpdtl] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[EPEarninggrpdtl]', N'uteditip') IS NULL ALTER TABLE [EPEarninggrpdtl] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPEarninggrpdtl]', N'uteditsessionid') IS NULL ALTER TABLE [EPEarninggrpdtl] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[EPEarninggrpdtl]', N'uteditsystem') IS NULL ALTER TABLE [EPEarninggrpdtl] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPEarninggrpdtl]', N'utedittime') IS NULL ALTER TABLE [EPEarninggrpdtl] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EPEarninggrpdtl]', N'utedituser') IS NULL ALTER TABLE [EPEarninggrpdtl] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPEarninggrpdtl]', N'utsubdoctype') IS NULL ALTER TABLE [EPEarninggrpdtl] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[ERDesgPayAllowanceDtl]', N'U') IS NULL BEGIN CREATE TABLE [ERDesgPayAllowanceDtl] (
    [Id] int NOT NULL,
    [earntypeid] int NULL,
    [ispercentage] bit NOT NULL DEFAULT CAST(0 AS bit),
    [designationpayrollid] int NULL,
    [lineindex] int NULL,
    [value] float NULL,
    [startdate] datetime2 NULL,
    [enddate] datetime2 NULL,
    [utblock] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    CONSTRAINT [PK_ERDesgPayAllowanceDtl] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[ERDesgPayAllowanceDtl]', N'Id') IS NULL ALTER TABLE [ERDesgPayAllowanceDtl] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[ERDesgPayAllowanceDtl]', N'earntypeid') IS NULL ALTER TABLE [ERDesgPayAllowanceDtl] ADD [earntypeid] int NULL;
GO
IF COL_LENGTH(N'[ERDesgPayAllowanceDtl]', N'ispercentage') IS NULL ALTER TABLE [ERDesgPayAllowanceDtl] ADD [ispercentage] bit NULL DEFAULT CAST(0 AS bit);
GO
IF COL_LENGTH(N'[ERDesgPayAllowanceDtl]', N'designationpayrollid') IS NULL ALTER TABLE [ERDesgPayAllowanceDtl] ADD [designationpayrollid] int NULL;
GO
IF COL_LENGTH(N'[ERDesgPayAllowanceDtl]', N'lineindex') IS NULL ALTER TABLE [ERDesgPayAllowanceDtl] ADD [lineindex] int NULL;
GO
IF COL_LENGTH(N'[ERDesgPayAllowanceDtl]', N'value') IS NULL ALTER TABLE [ERDesgPayAllowanceDtl] ADD [value] float NULL;
GO
IF COL_LENGTH(N'[ERDesgPayAllowanceDtl]', N'startdate') IS NULL ALTER TABLE [ERDesgPayAllowanceDtl] ADD [startdate] datetime2 NULL;
GO
IF COL_LENGTH(N'[ERDesgPayAllowanceDtl]', N'enddate') IS NULL ALTER TABLE [ERDesgPayAllowanceDtl] ADD [enddate] datetime2 NULL;
GO
IF COL_LENGTH(N'[ERDesgPayAllowanceDtl]', N'utblock') IS NULL ALTER TABLE [ERDesgPayAllowanceDtl] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[ERDesgPayAllowanceDtl]', N'utrecordversion') IS NULL ALTER TABLE [ERDesgPayAllowanceDtl] ADD [utrecordversion] int NULL;
GO
IF OBJECT_ID(N'[ERDesgPayBaseSalaryDtl]', N'U') IS NULL BEGIN CREATE TABLE [ERDesgPayBaseSalaryDtl] (
    [Id] int NOT NULL,
    [earntypeid] int NULL,
    [tovalue] float NULL,
    [designationpayrollid] int NULL,
    [lineindex] int NULL,
    [value] float NULL,
    [startdate] datetime2 NULL,
    [enddate] datetime2 NULL,
    [utblock] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    CONSTRAINT [PK_ERDesgPayBaseSalaryDtl] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[ERDesgPayBaseSalaryDtl]', N'Id') IS NULL ALTER TABLE [ERDesgPayBaseSalaryDtl] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[ERDesgPayBaseSalaryDtl]', N'earntypeid') IS NULL ALTER TABLE [ERDesgPayBaseSalaryDtl] ADD [earntypeid] int NULL;
GO
IF COL_LENGTH(N'[ERDesgPayBaseSalaryDtl]', N'tovalue') IS NULL ALTER TABLE [ERDesgPayBaseSalaryDtl] ADD [tovalue] float NULL;
GO
IF COL_LENGTH(N'[ERDesgPayBaseSalaryDtl]', N'designationpayrollid') IS NULL ALTER TABLE [ERDesgPayBaseSalaryDtl] ADD [designationpayrollid] int NULL;
GO
IF COL_LENGTH(N'[ERDesgPayBaseSalaryDtl]', N'lineindex') IS NULL ALTER TABLE [ERDesgPayBaseSalaryDtl] ADD [lineindex] int NULL;
GO
IF COL_LENGTH(N'[ERDesgPayBaseSalaryDtl]', N'value') IS NULL ALTER TABLE [ERDesgPayBaseSalaryDtl] ADD [value] float NULL;
GO
IF COL_LENGTH(N'[ERDesgPayBaseSalaryDtl]', N'startdate') IS NULL ALTER TABLE [ERDesgPayBaseSalaryDtl] ADD [startdate] datetime2 NULL;
GO
IF COL_LENGTH(N'[ERDesgPayBaseSalaryDtl]', N'enddate') IS NULL ALTER TABLE [ERDesgPayBaseSalaryDtl] ADD [enddate] datetime2 NULL;
GO
IF COL_LENGTH(N'[ERDesgPayBaseSalaryDtl]', N'utblock') IS NULL ALTER TABLE [ERDesgPayBaseSalaryDtl] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[ERDesgPayBaseSalaryDtl]', N'utrecordversion') IS NULL ALTER TABLE [ERDesgPayBaseSalaryDtl] ADD [utrecordversion] int NULL;
GO
IF OBJECT_ID(N'[ERDesgPayDeductionDtl]', N'U') IS NULL BEGIN CREATE TABLE [ERDesgPayDeductionDtl] (
    [Id] int NOT NULL,
    [deductiontypeid] int NULL,
    [designationpayrollid] int NULL,
    [lineindex] int NULL,
    [value] float NULL,
    [startdate] datetime2 NULL,
    [enddate] datetime2 NULL,
    [utblock] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    CONSTRAINT [PK_ERDesgPayDeductionDtl] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[ERDesgPayDeductionDtl]', N'Id') IS NULL ALTER TABLE [ERDesgPayDeductionDtl] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[ERDesgPayDeductionDtl]', N'deductiontypeid') IS NULL ALTER TABLE [ERDesgPayDeductionDtl] ADD [deductiontypeid] int NULL;
GO
IF COL_LENGTH(N'[ERDesgPayDeductionDtl]', N'designationpayrollid') IS NULL ALTER TABLE [ERDesgPayDeductionDtl] ADD [designationpayrollid] int NULL;
GO
IF COL_LENGTH(N'[ERDesgPayDeductionDtl]', N'lineindex') IS NULL ALTER TABLE [ERDesgPayDeductionDtl] ADD [lineindex] int NULL;
GO
IF COL_LENGTH(N'[ERDesgPayDeductionDtl]', N'value') IS NULL ALTER TABLE [ERDesgPayDeductionDtl] ADD [value] float NULL;
GO
IF COL_LENGTH(N'[ERDesgPayDeductionDtl]', N'startdate') IS NULL ALTER TABLE [ERDesgPayDeductionDtl] ADD [startdate] datetime2 NULL;
GO
IF COL_LENGTH(N'[ERDesgPayDeductionDtl]', N'enddate') IS NULL ALTER TABLE [ERDesgPayDeductionDtl] ADD [enddate] datetime2 NULL;
GO
IF COL_LENGTH(N'[ERDesgPayDeductionDtl]', N'utblock') IS NULL ALTER TABLE [ERDesgPayDeductionDtl] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[ERDesgPayDeductionDtl]', N'utrecordversion') IS NULL ALTER TABLE [ERDesgPayDeductionDtl] ADD [utrecordversion] int NULL;
GO
IF OBJECT_ID(N'[ERDesgPayLoanType]', N'U') IS NULL BEGIN CREATE TABLE [ERDesgPayLoanType] (
    [Id] int NOT NULL,
    [designationpayrollid] int NULL,
    [loantypeid] int NULL,
    [utblock] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    CONSTRAINT [PK_ERDesgPayLoanType] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[ERDesgPayLoanType]', N'Id') IS NULL ALTER TABLE [ERDesgPayLoanType] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[ERDesgPayLoanType]', N'designationpayrollid') IS NULL ALTER TABLE [ERDesgPayLoanType] ADD [designationpayrollid] int NULL;
GO
IF COL_LENGTH(N'[ERDesgPayLoanType]', N'loantypeid') IS NULL ALTER TABLE [ERDesgPayLoanType] ADD [loantypeid] int NULL;
GO
IF COL_LENGTH(N'[ERDesgPayLoanType]', N'utblock') IS NULL ALTER TABLE [ERDesgPayLoanType] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[ERDesgPayLoanType]', N'utrecordversion') IS NULL ALTER TABLE [ERDesgPayLoanType] ADD [utrecordversion] int NULL;
GO
IF OBJECT_ID(N'[ERSkillratingdtl]', N'U') IS NULL BEGIN CREATE TABLE [ERSkillratingdtl] (
    [id] int NOT NULL,
    [ratingid] int NULL,
    [lineindex] int NULL,
    [code] nvarchar(200) NULL,
    [key] nvarchar(100) NULL,
    [stxt] nvarchar(255) NULL,
    [factor] float NULL,
    [utblock] bit NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    CONSTRAINT [PK_ERSkillratingdtl] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[ERSkillratingdtl]', N'id') IS NULL ALTER TABLE [ERSkillratingdtl] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[ERSkillratingdtl]', N'ratingid') IS NULL ALTER TABLE [ERSkillratingdtl] ADD [ratingid] int NULL;
GO
IF COL_LENGTH(N'[ERSkillratingdtl]', N'lineindex') IS NULL ALTER TABLE [ERSkillratingdtl] ADD [lineindex] int NULL;
GO
IF COL_LENGTH(N'[ERSkillratingdtl]', N'code') IS NULL ALTER TABLE [ERSkillratingdtl] ADD [code] nvarchar(200) NULL;
GO
IF COL_LENGTH(N'[ERSkillratingdtl]', N'key') IS NULL ALTER TABLE [ERSkillratingdtl] ADD [key] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[ERSkillratingdtl]', N'stxt') IS NULL ALTER TABLE [ERSkillratingdtl] ADD [stxt] nvarchar(255) NULL;
GO
IF COL_LENGTH(N'[ERSkillratingdtl]', N'factor') IS NULL ALTER TABLE [ERSkillratingdtl] ADD [factor] float NULL;
GO
IF COL_LENGTH(N'[ERSkillratingdtl]', N'utblock') IS NULL ALTER TABLE [ERSkillratingdtl] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[ERSkillratingdtl]', N'utaddappversion') IS NULL ALTER TABLE [ERSkillratingdtl] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ERSkillratingdtl]', N'utadddbversion') IS NULL ALTER TABLE [ERSkillratingdtl] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ERSkillratingdtl]', N'utaddid') IS NULL ALTER TABLE [ERSkillratingdtl] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[ERSkillratingdtl]', N'utaddip') IS NULL ALTER TABLE [ERSkillratingdtl] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERSkillratingdtl]', N'utaddsessionid') IS NULL ALTER TABLE [ERSkillratingdtl] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[ERSkillratingdtl]', N'utaddsystem') IS NULL ALTER TABLE [ERSkillratingdtl] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERSkillratingdtl]', N'utaddtime') IS NULL ALTER TABLE [ERSkillratingdtl] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ERSkillratingdtl]', N'utadduser') IS NULL ALTER TABLE [ERSkillratingdtl] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERSkillratingdtl]', N'utdoctype') IS NULL ALTER TABLE [ERSkillratingdtl] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[ERSkillratingdtl]', N'uteditappversion') IS NULL ALTER TABLE [ERSkillratingdtl] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ERSkillratingdtl]', N'uteditdbversion') IS NULL ALTER TABLE [ERSkillratingdtl] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ERSkillratingdtl]', N'uteditid') IS NULL ALTER TABLE [ERSkillratingdtl] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[ERSkillratingdtl]', N'uteditip') IS NULL ALTER TABLE [ERSkillratingdtl] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERSkillratingdtl]', N'uteditsessionid') IS NULL ALTER TABLE [ERSkillratingdtl] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[ERSkillratingdtl]', N'uteditsystem') IS NULL ALTER TABLE [ERSkillratingdtl] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERSkillratingdtl]', N'utedittime') IS NULL ALTER TABLE [ERSkillratingdtl] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ERSkillratingdtl]', N'utedituser') IS NULL ALTER TABLE [ERSkillratingdtl] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ERSkillratingdtl]', N'utispartial') IS NULL ALTER TABLE [ERSkillratingdtl] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[ERSkillratingdtl]', N'utrecordversion') IS NULL ALTER TABLE [ERSkillratingdtl] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[ERSkillratingdtl]', N'utsubdoctype') IS NULL ALTER TABLE [ERSkillratingdtl] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[ERSkillratingdtl]', N'utsysgen') IS NULL ALTER TABLE [ERSkillratingdtl] ADD [utsysgen] bit NULL;
GO
IF OBJECT_ID(N'[ETCalholidayapplyondtl]', N'U') IS NULL BEGIN CREATE TABLE [ETCalholidayapplyondtl] (
    [id] int NOT NULL,
    [calholidayid] int NULL,
    [lineindex] int NULL,
    [departid] int NULL,
    [emptypeid] int NULL,
    [utblock] bit NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    CONSTRAINT [PK_ETCalholidayapplyondtl] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[ETCalholidayapplyondtl]', N'id') IS NULL ALTER TABLE [ETCalholidayapplyondtl] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidayapplyondtl]', N'calholidayid') IS NULL ALTER TABLE [ETCalholidayapplyondtl] ADD [calholidayid] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidayapplyondtl]', N'lineindex') IS NULL ALTER TABLE [ETCalholidayapplyondtl] ADD [lineindex] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidayapplyondtl]', N'departid') IS NULL ALTER TABLE [ETCalholidayapplyondtl] ADD [departid] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidayapplyondtl]', N'emptypeid') IS NULL ALTER TABLE [ETCalholidayapplyondtl] ADD [emptypeid] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidayapplyondtl]', N'utblock') IS NULL ALTER TABLE [ETCalholidayapplyondtl] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[ETCalholidayapplyondtl]', N'utaddappversion') IS NULL ALTER TABLE [ETCalholidayapplyondtl] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETCalholidayapplyondtl]', N'utadddbversion') IS NULL ALTER TABLE [ETCalholidayapplyondtl] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETCalholidayapplyondtl]', N'utaddid') IS NULL ALTER TABLE [ETCalholidayapplyondtl] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[ETCalholidayapplyondtl]', N'utaddip') IS NULL ALTER TABLE [ETCalholidayapplyondtl] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETCalholidayapplyondtl]', N'utaddsessionid') IS NULL ALTER TABLE [ETCalholidayapplyondtl] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidayapplyondtl]', N'utaddsystem') IS NULL ALTER TABLE [ETCalholidayapplyondtl] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETCalholidayapplyondtl]', N'utaddtime') IS NULL ALTER TABLE [ETCalholidayapplyondtl] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETCalholidayapplyondtl]', N'utadduser') IS NULL ALTER TABLE [ETCalholidayapplyondtl] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETCalholidayapplyondtl]', N'utdoctype') IS NULL ALTER TABLE [ETCalholidayapplyondtl] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidayapplyondtl]', N'uteditappversion') IS NULL ALTER TABLE [ETCalholidayapplyondtl] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETCalholidayapplyondtl]', N'uteditdbversion') IS NULL ALTER TABLE [ETCalholidayapplyondtl] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETCalholidayapplyondtl]', N'uteditid') IS NULL ALTER TABLE [ETCalholidayapplyondtl] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[ETCalholidayapplyondtl]', N'uteditip') IS NULL ALTER TABLE [ETCalholidayapplyondtl] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETCalholidayapplyondtl]', N'uteditsessionid') IS NULL ALTER TABLE [ETCalholidayapplyondtl] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidayapplyondtl]', N'uteditsystem') IS NULL ALTER TABLE [ETCalholidayapplyondtl] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETCalholidayapplyondtl]', N'utedittime') IS NULL ALTER TABLE [ETCalholidayapplyondtl] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETCalholidayapplyondtl]', N'utedituser') IS NULL ALTER TABLE [ETCalholidayapplyondtl] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETCalholidayapplyondtl]', N'utispartial') IS NULL ALTER TABLE [ETCalholidayapplyondtl] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[ETCalholidayapplyondtl]', N'utrecordversion') IS NULL ALTER TABLE [ETCalholidayapplyondtl] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidayapplyondtl]', N'utsubdoctype') IS NULL ALTER TABLE [ETCalholidayapplyondtl] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidayapplyondtl]', N'utsysgen') IS NULL ALTER TABLE [ETCalholidayapplyondtl] ADD [utsysgen] bit NULL;
GO
IF OBJECT_ID(N'[ETCalholidaydtl]', N'U') IS NULL BEGIN CREATE TABLE [ETCalholidaydtl] (
    [id] int NOT NULL,
    [calholidayid] int NULL,
    [lineindex] int NULL,
    [holidaytypeid] int NULL,
    [ltxt] nvarchar(250) NULL,
    [startdate] datetime2 NULL,
    [enddate] datetime2 NULL,
    [days] int NULL,
    [payperioddtlid] int NULL,
    [utblock] bit NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    CONSTRAINT [PK_ETCalholidaydtl] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[ETCalholidaydtl]', N'id') IS NULL ALTER TABLE [ETCalholidaydtl] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidaydtl]', N'calholidayid') IS NULL ALTER TABLE [ETCalholidaydtl] ADD [calholidayid] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidaydtl]', N'lineindex') IS NULL ALTER TABLE [ETCalholidaydtl] ADD [lineindex] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidaydtl]', N'holidaytypeid') IS NULL ALTER TABLE [ETCalholidaydtl] ADD [holidaytypeid] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidaydtl]', N'ltxt') IS NULL ALTER TABLE [ETCalholidaydtl] ADD [ltxt] nvarchar(250) NULL;
GO
IF COL_LENGTH(N'[ETCalholidaydtl]', N'startdate') IS NULL ALTER TABLE [ETCalholidaydtl] ADD [startdate] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETCalholidaydtl]', N'enddate') IS NULL ALTER TABLE [ETCalholidaydtl] ADD [enddate] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETCalholidaydtl]', N'days') IS NULL ALTER TABLE [ETCalholidaydtl] ADD [days] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidaydtl]', N'payperioddtlid') IS NULL ALTER TABLE [ETCalholidaydtl] ADD [payperioddtlid] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidaydtl]', N'utblock') IS NULL ALTER TABLE [ETCalholidaydtl] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[ETCalholidaydtl]', N'utaddappversion') IS NULL ALTER TABLE [ETCalholidaydtl] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETCalholidaydtl]', N'utadddbversion') IS NULL ALTER TABLE [ETCalholidaydtl] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETCalholidaydtl]', N'utaddid') IS NULL ALTER TABLE [ETCalholidaydtl] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[ETCalholidaydtl]', N'utaddip') IS NULL ALTER TABLE [ETCalholidaydtl] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETCalholidaydtl]', N'utaddsessionid') IS NULL ALTER TABLE [ETCalholidaydtl] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidaydtl]', N'utaddsystem') IS NULL ALTER TABLE [ETCalholidaydtl] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETCalholidaydtl]', N'utaddtime') IS NULL ALTER TABLE [ETCalholidaydtl] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETCalholidaydtl]', N'utadduser') IS NULL ALTER TABLE [ETCalholidaydtl] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETCalholidaydtl]', N'utdoctype') IS NULL ALTER TABLE [ETCalholidaydtl] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidaydtl]', N'uteditappversion') IS NULL ALTER TABLE [ETCalholidaydtl] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETCalholidaydtl]', N'uteditdbversion') IS NULL ALTER TABLE [ETCalholidaydtl] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETCalholidaydtl]', N'uteditid') IS NULL ALTER TABLE [ETCalholidaydtl] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[ETCalholidaydtl]', N'uteditip') IS NULL ALTER TABLE [ETCalholidaydtl] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETCalholidaydtl]', N'uteditsessionid') IS NULL ALTER TABLE [ETCalholidaydtl] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidaydtl]', N'uteditsystem') IS NULL ALTER TABLE [ETCalholidaydtl] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETCalholidaydtl]', N'utedittime') IS NULL ALTER TABLE [ETCalholidaydtl] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETCalholidaydtl]', N'utedituser') IS NULL ALTER TABLE [ETCalholidaydtl] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETCalholidaydtl]', N'utispartial') IS NULL ALTER TABLE [ETCalholidaydtl] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[ETCalholidaydtl]', N'utrecordversion') IS NULL ALTER TABLE [ETCalholidaydtl] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidaydtl]', N'utsubdoctype') IS NULL ALTER TABLE [ETCalholidaydtl] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidaydtl]', N'utsysgen') IS NULL ALTER TABLE [ETCalholidaydtl] ADD [utsysgen] bit NULL;
GO
IF OBJECT_ID(N'[ETCalholidayshareddtl]', N'U') IS NULL BEGIN CREATE TABLE [ETCalholidayshareddtl] (
    [id] int NOT NULL,
    [calholidayid] int NULL,
    [lineindex] int NULL,
    [lovsharedid] int NULL,
    [pclientid] int NULL,
    [pentityid] int NULL,
    [regionid] int NULL,
    [utblock] bit NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    CONSTRAINT [PK_ETCalholidayshareddtl] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[ETCalholidayshareddtl]', N'id') IS NULL ALTER TABLE [ETCalholidayshareddtl] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidayshareddtl]', N'calholidayid') IS NULL ALTER TABLE [ETCalholidayshareddtl] ADD [calholidayid] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidayshareddtl]', N'lineindex') IS NULL ALTER TABLE [ETCalholidayshareddtl] ADD [lineindex] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidayshareddtl]', N'lovsharedid') IS NULL ALTER TABLE [ETCalholidayshareddtl] ADD [lovsharedid] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidayshareddtl]', N'pclientid') IS NULL ALTER TABLE [ETCalholidayshareddtl] ADD [pclientid] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidayshareddtl]', N'pentityid') IS NULL ALTER TABLE [ETCalholidayshareddtl] ADD [pentityid] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidayshareddtl]', N'regionid') IS NULL ALTER TABLE [ETCalholidayshareddtl] ADD [regionid] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidayshareddtl]', N'utblock') IS NULL ALTER TABLE [ETCalholidayshareddtl] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[ETCalholidayshareddtl]', N'utaddappversion') IS NULL ALTER TABLE [ETCalholidayshareddtl] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETCalholidayshareddtl]', N'utadddbversion') IS NULL ALTER TABLE [ETCalholidayshareddtl] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETCalholidayshareddtl]', N'utaddid') IS NULL ALTER TABLE [ETCalholidayshareddtl] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[ETCalholidayshareddtl]', N'utaddip') IS NULL ALTER TABLE [ETCalholidayshareddtl] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETCalholidayshareddtl]', N'utaddsessionid') IS NULL ALTER TABLE [ETCalholidayshareddtl] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidayshareddtl]', N'utaddsystem') IS NULL ALTER TABLE [ETCalholidayshareddtl] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETCalholidayshareddtl]', N'utaddtime') IS NULL ALTER TABLE [ETCalholidayshareddtl] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETCalholidayshareddtl]', N'utadduser') IS NULL ALTER TABLE [ETCalholidayshareddtl] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETCalholidayshareddtl]', N'utdoctype') IS NULL ALTER TABLE [ETCalholidayshareddtl] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidayshareddtl]', N'uteditappversion') IS NULL ALTER TABLE [ETCalholidayshareddtl] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETCalholidayshareddtl]', N'uteditdbversion') IS NULL ALTER TABLE [ETCalholidayshareddtl] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETCalholidayshareddtl]', N'uteditid') IS NULL ALTER TABLE [ETCalholidayshareddtl] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[ETCalholidayshareddtl]', N'uteditip') IS NULL ALTER TABLE [ETCalholidayshareddtl] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETCalholidayshareddtl]', N'uteditsessionid') IS NULL ALTER TABLE [ETCalholidayshareddtl] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidayshareddtl]', N'uteditsystem') IS NULL ALTER TABLE [ETCalholidayshareddtl] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETCalholidayshareddtl]', N'utedittime') IS NULL ALTER TABLE [ETCalholidayshareddtl] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETCalholidayshareddtl]', N'utedituser') IS NULL ALTER TABLE [ETCalholidayshareddtl] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETCalholidayshareddtl]', N'utispartial') IS NULL ALTER TABLE [ETCalholidayshareddtl] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[ETCalholidayshareddtl]', N'utrecordversion') IS NULL ALTER TABLE [ETCalholidayshareddtl] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidayshareddtl]', N'utsubdoctype') IS NULL ALTER TABLE [ETCalholidayshareddtl] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[ETCalholidayshareddtl]', N'utsysgen') IS NULL ALTER TABLE [ETCalholidayshareddtl] ADD [utsysgen] bit NULL;
GO
IF OBJECT_ID(N'[ETLeaveadjpolicydtl]', N'U') IS NULL BEGIN CREATE TABLE [ETLeaveadjpolicydtl] (
    [id] int NOT NULL,
    [leaveadjid] int NULL,
    [leavetypeid] int NULL,
    [adjtypeid] int NULL,
    [basedonid] int NULL,
    [priorityid] int NULL,
    [days] int NOT NULL,
    [utblock] bit NOT NULL,
    [utsysgen] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utispartial] bit NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_ETLeaveadjpolicydtl] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicydtl]', N'id') IS NULL ALTER TABLE [ETLeaveadjpolicydtl] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicydtl]', N'leaveadjid') IS NULL ALTER TABLE [ETLeaveadjpolicydtl] ADD [leaveadjid] int NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicydtl]', N'leavetypeid') IS NULL ALTER TABLE [ETLeaveadjpolicydtl] ADD [leavetypeid] int NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicydtl]', N'adjtypeid') IS NULL ALTER TABLE [ETLeaveadjpolicydtl] ADD [adjtypeid] int NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicydtl]', N'basedonid') IS NULL ALTER TABLE [ETLeaveadjpolicydtl] ADD [basedonid] int NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicydtl]', N'priorityid') IS NULL ALTER TABLE [ETLeaveadjpolicydtl] ADD [priorityid] int NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicydtl]', N'days') IS NULL ALTER TABLE [ETLeaveadjpolicydtl] ADD [days] int NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicydtl]', N'utblock') IS NULL ALTER TABLE [ETLeaveadjpolicydtl] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicydtl]', N'utsysgen') IS NULL ALTER TABLE [ETLeaveadjpolicydtl] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicydtl]', N'utrecordversion') IS NULL ALTER TABLE [ETLeaveadjpolicydtl] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicydtl]', N'utispartial') IS NULL ALTER TABLE [ETLeaveadjpolicydtl] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicydtl]', N'utaddappversion') IS NULL ALTER TABLE [ETLeaveadjpolicydtl] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicydtl]', N'utadddbversion') IS NULL ALTER TABLE [ETLeaveadjpolicydtl] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicydtl]', N'utaddid') IS NULL ALTER TABLE [ETLeaveadjpolicydtl] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicydtl]', N'utaddip') IS NULL ALTER TABLE [ETLeaveadjpolicydtl] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicydtl]', N'utaddsessionid') IS NULL ALTER TABLE [ETLeaveadjpolicydtl] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicydtl]', N'utaddsystem') IS NULL ALTER TABLE [ETLeaveadjpolicydtl] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicydtl]', N'utaddtime') IS NULL ALTER TABLE [ETLeaveadjpolicydtl] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicydtl]', N'utadduser') IS NULL ALTER TABLE [ETLeaveadjpolicydtl] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicydtl]', N'utdoctype') IS NULL ALTER TABLE [ETLeaveadjpolicydtl] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicydtl]', N'uteditappversion') IS NULL ALTER TABLE [ETLeaveadjpolicydtl] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicydtl]', N'uteditdbversion') IS NULL ALTER TABLE [ETLeaveadjpolicydtl] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicydtl]', N'uteditid') IS NULL ALTER TABLE [ETLeaveadjpolicydtl] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicydtl]', N'uteditip') IS NULL ALTER TABLE [ETLeaveadjpolicydtl] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicydtl]', N'uteditsessionid') IS NULL ALTER TABLE [ETLeaveadjpolicydtl] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicydtl]', N'uteditsystem') IS NULL ALTER TABLE [ETLeaveadjpolicydtl] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicydtl]', N'utedittime') IS NULL ALTER TABLE [ETLeaveadjpolicydtl] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicydtl]', N'utedituser') IS NULL ALTER TABLE [ETLeaveadjpolicydtl] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETLeaveadjpolicydtl]', N'utsubdoctype') IS NULL ALTER TABLE [ETLeaveadjpolicydtl] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[ETLeavegroupdtl]', N'U') IS NULL BEGIN CREATE TABLE [ETLeavegroupdtl] (
    [Id] int NOT NULL,
    [leavegrpid] int NULL,
    [leavetypeid] int NULL,
    [leavedays] int NULL,
    [utblock] bit NULL,
    [utsysgen] bit NOT NULL,
    [utdoctype] int NULL,
    [utsubdoctype] int NULL,
    [utrecordversion] int NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utispartial] bit NOT NULL,
    CONSTRAINT [PK_ETLeavegroupdtl] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[ETLeavegroupdtl]', N'Id') IS NULL ALTER TABLE [ETLeavegroupdtl] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[ETLeavegroupdtl]', N'leavegrpid') IS NULL ALTER TABLE [ETLeavegroupdtl] ADD [leavegrpid] int NULL;
GO
IF COL_LENGTH(N'[ETLeavegroupdtl]', N'leavetypeid') IS NULL ALTER TABLE [ETLeavegroupdtl] ADD [leavetypeid] int NULL;
GO
IF COL_LENGTH(N'[ETLeavegroupdtl]', N'leavedays') IS NULL ALTER TABLE [ETLeavegroupdtl] ADD [leavedays] int NULL;
GO
IF COL_LENGTH(N'[ETLeavegroupdtl]', N'utblock') IS NULL ALTER TABLE [ETLeavegroupdtl] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[ETLeavegroupdtl]', N'utsysgen') IS NULL ALTER TABLE [ETLeavegroupdtl] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[ETLeavegroupdtl]', N'utdoctype') IS NULL ALTER TABLE [ETLeavegroupdtl] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[ETLeavegroupdtl]', N'utsubdoctype') IS NULL ALTER TABLE [ETLeavegroupdtl] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[ETLeavegroupdtl]', N'utrecordversion') IS NULL ALTER TABLE [ETLeavegroupdtl] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[ETLeavegroupdtl]', N'utaddappversion') IS NULL ALTER TABLE [ETLeavegroupdtl] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETLeavegroupdtl]', N'utadddbversion') IS NULL ALTER TABLE [ETLeavegroupdtl] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETLeavegroupdtl]', N'utaddid') IS NULL ALTER TABLE [ETLeavegroupdtl] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[ETLeavegroupdtl]', N'utaddip') IS NULL ALTER TABLE [ETLeavegroupdtl] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETLeavegroupdtl]', N'utaddsessionid') IS NULL ALTER TABLE [ETLeavegroupdtl] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETLeavegroupdtl]', N'utaddsystem') IS NULL ALTER TABLE [ETLeavegroupdtl] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETLeavegroupdtl]', N'utaddtime') IS NULL ALTER TABLE [ETLeavegroupdtl] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETLeavegroupdtl]', N'utadduser') IS NULL ALTER TABLE [ETLeavegroupdtl] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETLeavegroupdtl]', N'uteditappversion') IS NULL ALTER TABLE [ETLeavegroupdtl] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETLeavegroupdtl]', N'uteditdbversion') IS NULL ALTER TABLE [ETLeavegroupdtl] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETLeavegroupdtl]', N'uteditid') IS NULL ALTER TABLE [ETLeavegroupdtl] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[ETLeavegroupdtl]', N'uteditip') IS NULL ALTER TABLE [ETLeavegroupdtl] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETLeavegroupdtl]', N'uteditsessionid') IS NULL ALTER TABLE [ETLeavegroupdtl] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETLeavegroupdtl]', N'uteditsystem') IS NULL ALTER TABLE [ETLeavegroupdtl] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETLeavegroupdtl]', N'utedittime') IS NULL ALTER TABLE [ETLeavegroupdtl] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETLeavegroupdtl]', N'utedituser') IS NULL ALTER TABLE [ETLeavegroupdtl] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETLeavegroupdtl]', N'utispartial') IS NULL ALTER TABLE [ETLeavegroupdtl] ADD [utispartial] bit NULL;
GO
IF OBJECT_ID(N'[ETPayperioddtl]', N'U') IS NULL BEGIN CREATE TABLE [ETPayperioddtl] (
    [id] int NOT NULL,
    [payperiodid] int NULL,
    [year] int NULL,
    [monthid] int NULL,
    [periodstxt] nvarchar(200) NULL,
    [startdate] datetime2 NULL,
    [enddate] datetime2 NULL,
    [noofdays] int NULL,
    [disbdate] datetime2 NULL,
    [statusid] int NULL,
    [lineindex] int NULL,
    [utsysgen] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utblock] bit NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_ETPayperioddtl] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[ETPayperioddtl]', N'id') IS NULL ALTER TABLE [ETPayperioddtl] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[ETPayperioddtl]', N'payperiodid') IS NULL ALTER TABLE [ETPayperioddtl] ADD [payperiodid] int NULL;
GO
IF COL_LENGTH(N'[ETPayperioddtl]', N'year') IS NULL ALTER TABLE [ETPayperioddtl] ADD [year] int NULL;
GO
IF COL_LENGTH(N'[ETPayperioddtl]', N'monthid') IS NULL ALTER TABLE [ETPayperioddtl] ADD [monthid] int NULL;
GO
IF COL_LENGTH(N'[ETPayperioddtl]', N'periodstxt') IS NULL ALTER TABLE [ETPayperioddtl] ADD [periodstxt] nvarchar(200) NULL;
GO
IF COL_LENGTH(N'[ETPayperioddtl]', N'startdate') IS NULL ALTER TABLE [ETPayperioddtl] ADD [startdate] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETPayperioddtl]', N'enddate') IS NULL ALTER TABLE [ETPayperioddtl] ADD [enddate] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETPayperioddtl]', N'noofdays') IS NULL ALTER TABLE [ETPayperioddtl] ADD [noofdays] int NULL;
GO
IF COL_LENGTH(N'[ETPayperioddtl]', N'disbdate') IS NULL ALTER TABLE [ETPayperioddtl] ADD [disbdate] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETPayperioddtl]', N'statusid') IS NULL ALTER TABLE [ETPayperioddtl] ADD [statusid] int NULL;
GO
IF COL_LENGTH(N'[ETPayperioddtl]', N'lineindex') IS NULL ALTER TABLE [ETPayperioddtl] ADD [lineindex] int NULL;
GO
IF COL_LENGTH(N'[ETPayperioddtl]', N'utsysgen') IS NULL ALTER TABLE [ETPayperioddtl] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[ETPayperioddtl]', N'utispartial') IS NULL ALTER TABLE [ETPayperioddtl] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[ETPayperioddtl]', N'utrecordversion') IS NULL ALTER TABLE [ETPayperioddtl] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[ETPayperioddtl]', N'utblock') IS NULL ALTER TABLE [ETPayperioddtl] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[ETPayperioddtl]', N'utaddappversion') IS NULL ALTER TABLE [ETPayperioddtl] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETPayperioddtl]', N'utadddbversion') IS NULL ALTER TABLE [ETPayperioddtl] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETPayperioddtl]', N'utaddid') IS NULL ALTER TABLE [ETPayperioddtl] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[ETPayperioddtl]', N'utaddip') IS NULL ALTER TABLE [ETPayperioddtl] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETPayperioddtl]', N'utaddsessionid') IS NULL ALTER TABLE [ETPayperioddtl] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETPayperioddtl]', N'utaddsystem') IS NULL ALTER TABLE [ETPayperioddtl] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETPayperioddtl]', N'utaddtime') IS NULL ALTER TABLE [ETPayperioddtl] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETPayperioddtl]', N'utadduser') IS NULL ALTER TABLE [ETPayperioddtl] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETPayperioddtl]', N'utdoctype') IS NULL ALTER TABLE [ETPayperioddtl] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[ETPayperioddtl]', N'uteditappversion') IS NULL ALTER TABLE [ETPayperioddtl] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETPayperioddtl]', N'uteditdbversion') IS NULL ALTER TABLE [ETPayperioddtl] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETPayperioddtl]', N'uteditid') IS NULL ALTER TABLE [ETPayperioddtl] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[ETPayperioddtl]', N'uteditip') IS NULL ALTER TABLE [ETPayperioddtl] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETPayperioddtl]', N'uteditsessionid') IS NULL ALTER TABLE [ETPayperioddtl] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETPayperioddtl]', N'uteditsystem') IS NULL ALTER TABLE [ETPayperioddtl] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETPayperioddtl]', N'utedittime') IS NULL ALTER TABLE [ETPayperioddtl] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETPayperioddtl]', N'utedituser') IS NULL ALTER TABLE [ETPayperioddtl] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETPayperioddtl]', N'utsubdoctype') IS NULL ALTER TABLE [ETPayperioddtl] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[ETAbsentpolicydtl]', N'U') IS NULL BEGIN CREATE TABLE [ETAbsentpolicydtl] (
    [id] int NOT NULL,
    [policyid] int NULL,
    [lineindex] int NULL,
    [periodtypeid] int NULL,
    [policycount] float NULL,
    [absentcount] float NULL,
    [utsysgen] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utispartial] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_ETAbsentpolicydtl] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[ETAbsentpolicydtl]', N'id') IS NULL ALTER TABLE [ETAbsentpolicydtl] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[ETAbsentpolicydtl]', N'policyid') IS NULL ALTER TABLE [ETAbsentpolicydtl] ADD [policyid] int NULL;
GO
IF COL_LENGTH(N'[ETAbsentpolicydtl]', N'lineindex') IS NULL ALTER TABLE [ETAbsentpolicydtl] ADD [lineindex] int NULL;
GO
IF COL_LENGTH(N'[ETAbsentpolicydtl]', N'periodtypeid') IS NULL ALTER TABLE [ETAbsentpolicydtl] ADD [periodtypeid] int NULL;
GO
IF COL_LENGTH(N'[ETAbsentpolicydtl]', N'policycount') IS NULL ALTER TABLE [ETAbsentpolicydtl] ADD [policycount] float NULL;
GO
IF COL_LENGTH(N'[ETAbsentpolicydtl]', N'absentcount') IS NULL ALTER TABLE [ETAbsentpolicydtl] ADD [absentcount] float NULL;
GO
IF COL_LENGTH(N'[ETAbsentpolicydtl]', N'utsysgen') IS NULL ALTER TABLE [ETAbsentpolicydtl] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[ETAbsentpolicydtl]', N'utrecordversion') IS NULL ALTER TABLE [ETAbsentpolicydtl] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[ETAbsentpolicydtl]', N'utispartial') IS NULL ALTER TABLE [ETAbsentpolicydtl] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[ETAbsentpolicydtl]', N'utblock') IS NULL ALTER TABLE [ETAbsentpolicydtl] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[ETAbsentpolicydtl]', N'utaddappversion') IS NULL ALTER TABLE [ETAbsentpolicydtl] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETAbsentpolicydtl]', N'utadddbversion') IS NULL ALTER TABLE [ETAbsentpolicydtl] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETAbsentpolicydtl]', N'utaddid') IS NULL ALTER TABLE [ETAbsentpolicydtl] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[ETAbsentpolicydtl]', N'utaddip') IS NULL ALTER TABLE [ETAbsentpolicydtl] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETAbsentpolicydtl]', N'utaddsessionid') IS NULL ALTER TABLE [ETAbsentpolicydtl] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETAbsentpolicydtl]', N'utaddsystem') IS NULL ALTER TABLE [ETAbsentpolicydtl] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETAbsentpolicydtl]', N'utaddtime') IS NULL ALTER TABLE [ETAbsentpolicydtl] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETAbsentpolicydtl]', N'utadduser') IS NULL ALTER TABLE [ETAbsentpolicydtl] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETAbsentpolicydtl]', N'utdoctype') IS NULL ALTER TABLE [ETAbsentpolicydtl] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[ETAbsentpolicydtl]', N'uteditappversion') IS NULL ALTER TABLE [ETAbsentpolicydtl] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETAbsentpolicydtl]', N'uteditdbversion') IS NULL ALTER TABLE [ETAbsentpolicydtl] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETAbsentpolicydtl]', N'uteditid') IS NULL ALTER TABLE [ETAbsentpolicydtl] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[ETAbsentpolicydtl]', N'uteditip') IS NULL ALTER TABLE [ETAbsentpolicydtl] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETAbsentpolicydtl]', N'uteditsessionid') IS NULL ALTER TABLE [ETAbsentpolicydtl] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETAbsentpolicydtl]', N'uteditsystem') IS NULL ALTER TABLE [ETAbsentpolicydtl] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETAbsentpolicydtl]', N'utedittime') IS NULL ALTER TABLE [ETAbsentpolicydtl] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETAbsentpolicydtl]', N'utedituser') IS NULL ALTER TABLE [ETAbsentpolicydtl] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETAbsentpolicydtl]', N'utsubdoctype') IS NULL ALTER TABLE [ETAbsentpolicydtl] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[ETPolicydtl]', N'U') IS NULL BEGIN CREATE TABLE [ETPolicydtl] (
    [id] int NOT NULL,
    [policyid] int NULL,
    [lineindex] int NULL,
    [starttime] int NULL,
    [endtime] int NULL,
    [periodtypeid] int NULL,
    [valtypeid] int NULL,
    [value] float NULL,
    [utsysgen] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utispartial] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_ETPolicydtl] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[ETPolicydtl]', N'id') IS NULL ALTER TABLE [ETPolicydtl] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[ETPolicydtl]', N'policyid') IS NULL ALTER TABLE [ETPolicydtl] ADD [policyid] int NULL;
GO
IF COL_LENGTH(N'[ETPolicydtl]', N'lineindex') IS NULL ALTER TABLE [ETPolicydtl] ADD [lineindex] int NULL;
GO
IF COL_LENGTH(N'[ETPolicydtl]', N'starttime') IS NULL ALTER TABLE [ETPolicydtl] ADD [starttime] int NULL;
GO
IF COL_LENGTH(N'[ETPolicydtl]', N'endtime') IS NULL ALTER TABLE [ETPolicydtl] ADD [endtime] int NULL;
GO
IF COL_LENGTH(N'[ETPolicydtl]', N'periodtypeid') IS NULL ALTER TABLE [ETPolicydtl] ADD [periodtypeid] int NULL;
GO
IF COL_LENGTH(N'[ETPolicydtl]', N'valtypeid') IS NULL ALTER TABLE [ETPolicydtl] ADD [valtypeid] int NULL;
GO
IF COL_LENGTH(N'[ETPolicydtl]', N'value') IS NULL ALTER TABLE [ETPolicydtl] ADD [value] float NULL;
GO
IF COL_LENGTH(N'[ETPolicydtl]', N'utsysgen') IS NULL ALTER TABLE [ETPolicydtl] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[ETPolicydtl]', N'utrecordversion') IS NULL ALTER TABLE [ETPolicydtl] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[ETPolicydtl]', N'utispartial') IS NULL ALTER TABLE [ETPolicydtl] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[ETPolicydtl]', N'utblock') IS NULL ALTER TABLE [ETPolicydtl] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[ETPolicydtl]', N'utaddappversion') IS NULL ALTER TABLE [ETPolicydtl] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETPolicydtl]', N'utadddbversion') IS NULL ALTER TABLE [ETPolicydtl] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETPolicydtl]', N'utaddid') IS NULL ALTER TABLE [ETPolicydtl] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[ETPolicydtl]', N'utaddip') IS NULL ALTER TABLE [ETPolicydtl] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETPolicydtl]', N'utaddsessionid') IS NULL ALTER TABLE [ETPolicydtl] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETPolicydtl]', N'utaddsystem') IS NULL ALTER TABLE [ETPolicydtl] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETPolicydtl]', N'utaddtime') IS NULL ALTER TABLE [ETPolicydtl] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETPolicydtl]', N'utadduser') IS NULL ALTER TABLE [ETPolicydtl] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETPolicydtl]', N'utdoctype') IS NULL ALTER TABLE [ETPolicydtl] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[ETPolicydtl]', N'uteditappversion') IS NULL ALTER TABLE [ETPolicydtl] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETPolicydtl]', N'uteditdbversion') IS NULL ALTER TABLE [ETPolicydtl] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETPolicydtl]', N'uteditid') IS NULL ALTER TABLE [ETPolicydtl] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[ETPolicydtl]', N'uteditip') IS NULL ALTER TABLE [ETPolicydtl] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETPolicydtl]', N'uteditsessionid') IS NULL ALTER TABLE [ETPolicydtl] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETPolicydtl]', N'uteditsystem') IS NULL ALTER TABLE [ETPolicydtl] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETPolicydtl]', N'utedittime') IS NULL ALTER TABLE [ETPolicydtl] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETPolicydtl]', N'utedituser') IS NULL ALTER TABLE [ETPolicydtl] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETPolicydtl]', N'utsubdoctype') IS NULL ALTER TABLE [ETPolicydtl] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[EREmployeeinfohdr]', N'U') IS NULL BEGIN CREATE TABLE [EREmployeeinfohdr] (
    [Id] int NOT NULL,
    [clientid] int NULL,
    [preventityid] int NULL,
    [entityid] int NULL,
    [employeecode] nvarchar(50) NULL,
    [lvemployeecateg] int NULL,
    [employeetypeid] int NULL,
    [firstname] nvarchar(150) NULL,
    [lastname] nvarchar(150) NULL,
    [parentname] nvarchar(150) NULL,
    [cellno] nvarchar(20) NULL,
    [depid] int NULL,
    [designationid] int NULL,
    [reportingtoemployeeid] int NULL,
    [substituteemployeeid] int NULL,
    [empstatusid] int NULL,
    [dateofbirth] datetime2 NULL,
    [genderid] int NULL,
    [bloodgroupid] int NULL,
    [maritalid] int NULL,
    [birthcountryid] int NULL,
    [birthcityid] int NULL,
    [nationalityid] int NULL,
    [religionid] int NULL,
    [joiningdate] datetime2 NULL,
    [resigndate] datetime2 NULL,
    [imageid] bigint NULL,
    [emailid] nvarchar(50) NULL,
    [lovpositiontypeid] int NULL,
    [shiftid] int NULL,
    [machinecode] nvarchar(50) NULL,
    [coursecategoryid] int NULL,
    [username] nvarchar(500) NULL,
    [pwd] nvarchar(500) NULL,
    [meetingid] nvarchar(50) NULL,
    [meetingpwd] nvarchar(50) NULL,
    [jwttoken] nvarchar(max) NULL,
    [apiKey] nvarchar(500) NULL,
    [apiSecret] nvarchar(500) NULL,
    [sdkkey] nvarchar(max) NULL,
    [sdksecret] nvarchar(max) NULL,
    [invitepwd] nvarchar(max) NULL,
    [cnicno] nvarchar(50) NULL,
    [cprno] nvarchar(50) NULL,
    [cprexpiry] date NULL,
    [passportno] nvarchar(50) NULL,
    [passportexpiry] date NULL,
    [visano] nvarchar(50) NULL,
    [visaexpiry] date NULL,
    [moeid] int NULL,
    [moeexpiry] date NULL,
    [address] nvarchar(max) NULL,
    [drivinglicenseno] nvarchar(50) NULL,
    [licenseexp] datetime2 NULL,
    [emergencyno] nvarchar(50) NULL,
    [vehicleid] int NULL,
    [homestopid] int NULL,
    [tvalidfrom] datetime2 NULL,
    [tvalidtill] datetime2 NULL,
    [tactive] bit NULL,
    [utblock] bit NULL,
    [utedittime] datetime2 NULL,
    [utrecordversion] int NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedituser] varchar(25) NULL,
    [utispartial] bit NOT NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    CONSTRAINT [PK_EREmployeeinfohdr] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'Id') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'clientid') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'preventityid') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [preventityid] int NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'entityid') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'employeecode') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [employeecode] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'lvemployeecateg') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [lvemployeecateg] int NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'employeetypeid') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [employeetypeid] int NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'firstname') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [firstname] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'lastname') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [lastname] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'parentname') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [parentname] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'cellno') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [cellno] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'depid') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [depid] int NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'designationid') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [designationid] int NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'reportingtoemployeeid') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [reportingtoemployeeid] int NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'substituteemployeeid') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [substituteemployeeid] int NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'empstatusid') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [empstatusid] int NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'dateofbirth') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [dateofbirth] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'genderid') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [genderid] int NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'bloodgroupid') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [bloodgroupid] int NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'maritalid') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [maritalid] int NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'birthcountryid') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [birthcountryid] int NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'birthcityid') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [birthcityid] int NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'nationalityid') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [nationalityid] int NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'religionid') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [religionid] int NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'joiningdate') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [joiningdate] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'resigndate') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [resigndate] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'imageid') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [imageid] bigint NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'emailid') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [emailid] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'lovpositiontypeid') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [lovpositiontypeid] int NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'shiftid') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [shiftid] int NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'machinecode') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [machinecode] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'coursecategoryid') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [coursecategoryid] int NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'username') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [username] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'pwd') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [pwd] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'meetingid') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [meetingid] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'meetingpwd') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [meetingpwd] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'jwttoken') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [jwttoken] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'apiKey') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [apiKey] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'apiSecret') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [apiSecret] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'sdkkey') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [sdkkey] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'sdksecret') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [sdksecret] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'invitepwd') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [invitepwd] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'cnicno') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [cnicno] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'cprno') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [cprno] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'cprexpiry') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [cprexpiry] date NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'passportno') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [passportno] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'passportexpiry') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [passportexpiry] date NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'visano') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [visano] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'visaexpiry') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [visaexpiry] date NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'moeid') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [moeid] int NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'moeexpiry') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [moeexpiry] date NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'address') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [address] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'drivinglicenseno') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [drivinglicenseno] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'licenseexp') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [licenseexp] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'emergencyno') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [emergencyno] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'vehicleid') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [vehicleid] int NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'homestopid') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [homestopid] int NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'tvalidfrom') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [tvalidfrom] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'tvalidtill') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [tvalidtill] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'tactive') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [tactive] bit NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'utblock') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'utedittime') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'utrecordversion') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'utaddappversion') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'utadddbversion') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'utaddid') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'utaddip') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'utaddsessionid') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'utaddsystem') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'utaddtime') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'utadduser') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'utdoctype') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'uteditappversion') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'uteditdbversion') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'uteditid') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'uteditip') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'uteditsessionid') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'uteditsystem') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'utedituser') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'utispartial') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'utsubdoctype') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'utsysgen') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [utsysgen] bit NULL;
GO
IF OBJECT_ID(N'[HRPosition]', N'U') IS NULL BEGIN CREATE TABLE [HRPosition] (
    [Id] int NOT NULL IDENTITY,
    [ClientId] int NOT NULL,
    [EntityId] int NULL,
    [ScopeType] nvarchar(10) NULL,
    [PositionKind] nvarchar(20) NULL,
    [DepartmentId] int NULL,
    [DesignationId] int NULL,
    [PositionNumber] nvarchar(30) NULL,
    [Title] nvarchar(200) NULL,
    [ReportsToType] nvarchar(20) NULL,
    [ReportsToPositionId] int NULL,
    [LovStatusId] int NOT NULL,
    [UtDocType] int NOT NULL,
    [UtSubDocType] int NOT NULL,
    [UtSysGen] bit NULL,
    [UtBlock] bit NULL,
    [UtIsPartial] bit NULL,
    [UtAddTime] datetime2 NOT NULL,
    [UtAddUser] nvarchar(25) NULL,
    [UtAddSystem] nvarchar(25) NULL,
    [UtAddIp] nvarchar(25) NULL,
    [UtAddAppVersion] nvarchar(10) NULL,
    [UtAddDbVersion] nvarchar(10) NULL,
    [UtAddId] bigint NOT NULL,
    [UtAddSessionId] int NOT NULL,
    [UtEditTime] datetime2 NOT NULL,
    [UtEditUser] nvarchar(25) NULL,
    [UtEditSystem] nvarchar(25) NULL,
    [UtEditIp] nvarchar(25) NULL,
    [UtEditAppVersion] nvarchar(10) NULL,
    [UtEditDbVersion] nvarchar(10) NULL,
    [UtEditId] bigint NOT NULL,
    [UtEditSessionId] int NOT NULL,
    [UtRecordVersion] int NULL,
    CONSTRAINT [PK_HRPosition] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[HRPosition]', N'Id') IS NULL ALTER TABLE [HRPosition] ADD [Id] int NULL ;
GO
IF COL_LENGTH(N'[HRPosition]', N'ClientId') IS NULL ALTER TABLE [HRPosition] ADD [ClientId] int NULL;
GO
IF COL_LENGTH(N'[HRPosition]', N'EntityId') IS NULL ALTER TABLE [HRPosition] ADD [EntityId] int NULL;
GO
IF COL_LENGTH(N'[HRPosition]', N'ScopeType') IS NULL ALTER TABLE [HRPosition] ADD [ScopeType] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[HRPosition]', N'PositionKind') IS NULL ALTER TABLE [HRPosition] ADD [PositionKind] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[HRPosition]', N'DepartmentId') IS NULL ALTER TABLE [HRPosition] ADD [DepartmentId] int NULL;
GO
IF COL_LENGTH(N'[HRPosition]', N'DesignationId') IS NULL ALTER TABLE [HRPosition] ADD [DesignationId] int NULL;
GO
IF COL_LENGTH(N'[HRPosition]', N'PositionNumber') IS NULL ALTER TABLE [HRPosition] ADD [PositionNumber] nvarchar(30) NULL;
GO
IF COL_LENGTH(N'[HRPosition]', N'Title') IS NULL ALTER TABLE [HRPosition] ADD [Title] nvarchar(200) NULL;
GO
IF COL_LENGTH(N'[HRPosition]', N'ReportsToType') IS NULL ALTER TABLE [HRPosition] ADD [ReportsToType] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[HRPosition]', N'ReportsToPositionId') IS NULL ALTER TABLE [HRPosition] ADD [ReportsToPositionId] int NULL;
GO
IF COL_LENGTH(N'[HRPosition]', N'LovStatusId') IS NULL ALTER TABLE [HRPosition] ADD [LovStatusId] int NULL;
GO
IF COL_LENGTH(N'[HRPosition]', N'UtDocType') IS NULL ALTER TABLE [HRPosition] ADD [UtDocType] int NULL;
GO
IF COL_LENGTH(N'[HRPosition]', N'UtSubDocType') IS NULL ALTER TABLE [HRPosition] ADD [UtSubDocType] int NULL;
GO
IF COL_LENGTH(N'[HRPosition]', N'UtSysGen') IS NULL ALTER TABLE [HRPosition] ADD [UtSysGen] bit NULL;
GO
IF COL_LENGTH(N'[HRPosition]', N'UtBlock') IS NULL ALTER TABLE [HRPosition] ADD [UtBlock] bit NULL;
GO
IF COL_LENGTH(N'[HRPosition]', N'UtIsPartial') IS NULL ALTER TABLE [HRPosition] ADD [UtIsPartial] bit NULL;
GO
IF COL_LENGTH(N'[HRPosition]', N'UtAddTime') IS NULL ALTER TABLE [HRPosition] ADD [UtAddTime] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRPosition]', N'UtAddUser') IS NULL ALTER TABLE [HRPosition] ADD [UtAddUser] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[HRPosition]', N'UtAddSystem') IS NULL ALTER TABLE [HRPosition] ADD [UtAddSystem] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[HRPosition]', N'UtAddIp') IS NULL ALTER TABLE [HRPosition] ADD [UtAddIp] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[HRPosition]', N'UtAddAppVersion') IS NULL ALTER TABLE [HRPosition] ADD [UtAddAppVersion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[HRPosition]', N'UtAddDbVersion') IS NULL ALTER TABLE [HRPosition] ADD [UtAddDbVersion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[HRPosition]', N'UtAddId') IS NULL ALTER TABLE [HRPosition] ADD [UtAddId] bigint NULL;
GO
IF COL_LENGTH(N'[HRPosition]', N'UtAddSessionId') IS NULL ALTER TABLE [HRPosition] ADD [UtAddSessionId] int NULL;
GO
IF COL_LENGTH(N'[HRPosition]', N'UtEditTime') IS NULL ALTER TABLE [HRPosition] ADD [UtEditTime] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRPosition]', N'UtEditUser') IS NULL ALTER TABLE [HRPosition] ADD [UtEditUser] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[HRPosition]', N'UtEditSystem') IS NULL ALTER TABLE [HRPosition] ADD [UtEditSystem] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[HRPosition]', N'UtEditIp') IS NULL ALTER TABLE [HRPosition] ADD [UtEditIp] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[HRPosition]', N'UtEditAppVersion') IS NULL ALTER TABLE [HRPosition] ADD [UtEditAppVersion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[HRPosition]', N'UtEditDbVersion') IS NULL ALTER TABLE [HRPosition] ADD [UtEditDbVersion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[HRPosition]', N'UtEditId') IS NULL ALTER TABLE [HRPosition] ADD [UtEditId] bigint NULL;
GO
IF COL_LENGTH(N'[HRPosition]', N'UtEditSessionId') IS NULL ALTER TABLE [HRPosition] ADD [UtEditSessionId] int NULL;
GO
IF COL_LENGTH(N'[HRPosition]', N'UtRecordVersion') IS NULL ALTER TABLE [HRPosition] ADD [UtRecordVersion] int NULL;
GO
IF OBJECT_ID(N'[EPLoanrequest]', N'U') IS NULL BEGIN CREATE TABLE [EPLoanrequest] (
    [id] int NOT NULL,
    [requestno] nvarchar(50) NULL,
    [requestdate] datetime2 NULL,
    [requiredate] datetime2 NULL,
    [employeeid] int NULL,
    [loannatureid] int NULL,
    [loantypeid] int NULL,
    [appliedamount] float NULL,
    [reasontypeid] int NULL,
    [reason] nvarchar(1500) NULL,
    [dedtypeid] int NULL,
    [instalamt] float NULL,
    [noofinstal] int NULL,
    [approvenoofinstallments] int NOT NULL,
    [approveamounts] float NOT NULL,
    [installmentstartdate] datetime2 NULL,
    [HRComments] nvarchar(1500) NULL,
    [LineManagerComments] nvarchar(1500) NULL,
    [statusid] int NULL,
    [entityid] int NULL,
    [clientid] int NULL,
    [utdoctype] int NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    CONSTRAINT [PK_EPLoanrequest] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'id') IS NULL ALTER TABLE [EPLoanrequest] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'requestno') IS NULL ALTER TABLE [EPLoanrequest] ADD [requestno] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'requestdate') IS NULL ALTER TABLE [EPLoanrequest] ADD [requestdate] datetime2 NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'requiredate') IS NULL ALTER TABLE [EPLoanrequest] ADD [requiredate] datetime2 NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'employeeid') IS NULL ALTER TABLE [EPLoanrequest] ADD [employeeid] int NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'loannatureid') IS NULL ALTER TABLE [EPLoanrequest] ADD [loannatureid] int NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'loantypeid') IS NULL ALTER TABLE [EPLoanrequest] ADD [loantypeid] int NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'appliedamount') IS NULL ALTER TABLE [EPLoanrequest] ADD [appliedamount] float NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'reasontypeid') IS NULL ALTER TABLE [EPLoanrequest] ADD [reasontypeid] int NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'reason') IS NULL ALTER TABLE [EPLoanrequest] ADD [reason] nvarchar(1500) NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'dedtypeid') IS NULL ALTER TABLE [EPLoanrequest] ADD [dedtypeid] int NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'instalamt') IS NULL ALTER TABLE [EPLoanrequest] ADD [instalamt] float NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'noofinstal') IS NULL ALTER TABLE [EPLoanrequest] ADD [noofinstal] int NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'approvenoofinstallments') IS NULL ALTER TABLE [EPLoanrequest] ADD [approvenoofinstallments] int NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'approveamounts') IS NULL ALTER TABLE [EPLoanrequest] ADD [approveamounts] float NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'installmentstartdate') IS NULL ALTER TABLE [EPLoanrequest] ADD [installmentstartdate] datetime2 NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'HRComments') IS NULL ALTER TABLE [EPLoanrequest] ADD [HRComments] nvarchar(1500) NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'LineManagerComments') IS NULL ALTER TABLE [EPLoanrequest] ADD [LineManagerComments] nvarchar(1500) NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'statusid') IS NULL ALTER TABLE [EPLoanrequest] ADD [statusid] int NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'entityid') IS NULL ALTER TABLE [EPLoanrequest] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'clientid') IS NULL ALTER TABLE [EPLoanrequest] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'utdoctype') IS NULL ALTER TABLE [EPLoanrequest] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'utsubdoctype') IS NULL ALTER TABLE [EPLoanrequest] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'utsysgen') IS NULL ALTER TABLE [EPLoanrequest] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'utblock') IS NULL ALTER TABLE [EPLoanrequest] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'utispartial') IS NULL ALTER TABLE [EPLoanrequest] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'utrecordversion') IS NULL ALTER TABLE [EPLoanrequest] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'utaddappversion') IS NULL ALTER TABLE [EPLoanrequest] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'utadddbversion') IS NULL ALTER TABLE [EPLoanrequest] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'utaddid') IS NULL ALTER TABLE [EPLoanrequest] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'utaddip') IS NULL ALTER TABLE [EPLoanrequest] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'utaddsessionid') IS NULL ALTER TABLE [EPLoanrequest] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'utaddsystem') IS NULL ALTER TABLE [EPLoanrequest] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'utaddtime') IS NULL ALTER TABLE [EPLoanrequest] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'utadduser') IS NULL ALTER TABLE [EPLoanrequest] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'uteditappversion') IS NULL ALTER TABLE [EPLoanrequest] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'uteditdbversion') IS NULL ALTER TABLE [EPLoanrequest] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'uteditid') IS NULL ALTER TABLE [EPLoanrequest] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'uteditip') IS NULL ALTER TABLE [EPLoanrequest] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'uteditsessionid') IS NULL ALTER TABLE [EPLoanrequest] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'uteditsystem') IS NULL ALTER TABLE [EPLoanrequest] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'utedittime') IS NULL ALTER TABLE [EPLoanrequest] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EPLoanrequest]', N'utedituser') IS NULL ALTER TABLE [EPLoanrequest] ADD [utedituser] varchar(25) NULL;
GO
IF OBJECT_ID(N'[EREmpBank]', N'U') IS NULL BEGIN CREATE TABLE [EREmpBank] (
    [id] int NOT NULL,
    [code] nvarchar(20) NULL,
    [employeeid] int NULL,
    [bankgrpid] int NULL,
    [branchcode] nvarchar(10) NULL,
    [branchname] nvarchar(100) NULL,
    [accountno] nvarchar(30) NULL,
    [ibanno] nvarchar(30) NULL,
    [title] nvarchar(100) NULL,
    [primaryacct] bit NULL,
    [utsysgen] bit NOT NULL,
    [utblock1] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utrecordversion] int NOT NULL,
    [utblock] bit NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_EREmpBank] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[EREmpBank]', N'id') IS NULL ALTER TABLE [EREmpBank] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[EREmpBank]', N'code') IS NULL ALTER TABLE [EREmpBank] ADD [code] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[EREmpBank]', N'employeeid') IS NULL ALTER TABLE [EREmpBank] ADD [employeeid] int NULL;
GO
IF COL_LENGTH(N'[EREmpBank]', N'bankgrpid') IS NULL ALTER TABLE [EREmpBank] ADD [bankgrpid] int NULL;
GO
IF COL_LENGTH(N'[EREmpBank]', N'branchcode') IS NULL ALTER TABLE [EREmpBank] ADD [branchcode] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmpBank]', N'branchname') IS NULL ALTER TABLE [EREmpBank] ADD [branchname] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[EREmpBank]', N'accountno') IS NULL ALTER TABLE [EREmpBank] ADD [accountno] nvarchar(30) NULL;
GO
IF COL_LENGTH(N'[EREmpBank]', N'ibanno') IS NULL ALTER TABLE [EREmpBank] ADD [ibanno] nvarchar(30) NULL;
GO
IF COL_LENGTH(N'[EREmpBank]', N'title') IS NULL ALTER TABLE [EREmpBank] ADD [title] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[EREmpBank]', N'primaryacct') IS NULL ALTER TABLE [EREmpBank] ADD [primaryacct] bit NULL;
GO
IF COL_LENGTH(N'[EREmpBank]', N'utsysgen') IS NULL ALTER TABLE [EREmpBank] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[EREmpBank]', N'utblock1') IS NULL ALTER TABLE [EREmpBank] ADD [utblock1] bit NULL;
GO
IF COL_LENGTH(N'[EREmpBank]', N'utispartial') IS NULL ALTER TABLE [EREmpBank] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[EREmpBank]', N'utaddtime') IS NULL ALTER TABLE [EREmpBank] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmpBank]', N'utadduser') IS NULL ALTER TABLE [EREmpBank] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpBank]', N'utedittime') IS NULL ALTER TABLE [EREmpBank] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmpBank]', N'utedituser') IS NULL ALTER TABLE [EREmpBank] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpBank]', N'utrecordversion') IS NULL ALTER TABLE [EREmpBank] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[EREmpBank]', N'utblock') IS NULL ALTER TABLE [EREmpBank] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[EREmpBank]', N'utaddappversion') IS NULL ALTER TABLE [EREmpBank] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmpBank]', N'utadddbversion') IS NULL ALTER TABLE [EREmpBank] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmpBank]', N'utaddid') IS NULL ALTER TABLE [EREmpBank] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[EREmpBank]', N'utaddip') IS NULL ALTER TABLE [EREmpBank] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpBank]', N'utaddsessionid') IS NULL ALTER TABLE [EREmpBank] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[EREmpBank]', N'utaddsystem') IS NULL ALTER TABLE [EREmpBank] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpBank]', N'utdoctype') IS NULL ALTER TABLE [EREmpBank] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[EREmpBank]', N'uteditappversion') IS NULL ALTER TABLE [EREmpBank] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmpBank]', N'uteditdbversion') IS NULL ALTER TABLE [EREmpBank] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmpBank]', N'uteditid') IS NULL ALTER TABLE [EREmpBank] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[EREmpBank]', N'uteditip') IS NULL ALTER TABLE [EREmpBank] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpBank]', N'uteditsessionid') IS NULL ALTER TABLE [EREmpBank] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[EREmpBank]', N'uteditsystem') IS NULL ALTER TABLE [EREmpBank] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpBank]', N'utsubdoctype') IS NULL ALTER TABLE [EREmpBank] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[EREmpCards]', N'U') IS NULL BEGIN CREATE TABLE [EREmpCards] (
    [id] int NOT NULL,
    [code] nvarchar(20) NULL,
    [employeeid] int NULL,
    [cardtypeid] int NULL,
    [authorityid] int NULL,
    [cardno] nvarchar(25) NULL,
    [validfrom] datetime2 NULL,
    [validtill] datetime2 NULL,
    [utdoctype] int NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    [utblock1] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddip] varchar(25) NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddsessionid] int NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [uteditsystem] varchar(25) NULL,
    [uteditip] varchar(25) NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditsessionid] int NULL,
    [utrecordversion] int NOT NULL,
    [utblock] bit NOT NULL,
    [crdtypeid] int NULL,
    [authtypeid] int NULL,
    [authid] int NULL,
    CONSTRAINT [PK_EREmpCards] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[EREmpCards]', N'id') IS NULL ALTER TABLE [EREmpCards] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[EREmpCards]', N'code') IS NULL ALTER TABLE [EREmpCards] ADD [code] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[EREmpCards]', N'employeeid') IS NULL ALTER TABLE [EREmpCards] ADD [employeeid] int NULL;
GO
IF COL_LENGTH(N'[EREmpCards]', N'cardtypeid') IS NULL ALTER TABLE [EREmpCards] ADD [cardtypeid] int NULL;
GO
IF COL_LENGTH(N'[EREmpCards]', N'authorityid') IS NULL ALTER TABLE [EREmpCards] ADD [authorityid] int NULL;
GO
IF COL_LENGTH(N'[EREmpCards]', N'cardno') IS NULL ALTER TABLE [EREmpCards] ADD [cardno] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpCards]', N'validfrom') IS NULL ALTER TABLE [EREmpCards] ADD [validfrom] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmpCards]', N'validtill') IS NULL ALTER TABLE [EREmpCards] ADD [validtill] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmpCards]', N'utdoctype') IS NULL ALTER TABLE [EREmpCards] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[EREmpCards]', N'utsubdoctype') IS NULL ALTER TABLE [EREmpCards] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[EREmpCards]', N'utsysgen') IS NULL ALTER TABLE [EREmpCards] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[EREmpCards]', N'utblock1') IS NULL ALTER TABLE [EREmpCards] ADD [utblock1] bit NULL;
GO
IF COL_LENGTH(N'[EREmpCards]', N'utispartial') IS NULL ALTER TABLE [EREmpCards] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[EREmpCards]', N'utaddtime') IS NULL ALTER TABLE [EREmpCards] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmpCards]', N'utadduser') IS NULL ALTER TABLE [EREmpCards] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpCards]', N'utaddsystem') IS NULL ALTER TABLE [EREmpCards] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpCards]', N'utaddip') IS NULL ALTER TABLE [EREmpCards] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpCards]', N'utaddappversion') IS NULL ALTER TABLE [EREmpCards] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmpCards]', N'utadddbversion') IS NULL ALTER TABLE [EREmpCards] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmpCards]', N'utaddid') IS NULL ALTER TABLE [EREmpCards] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[EREmpCards]', N'utaddsessionid') IS NULL ALTER TABLE [EREmpCards] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[EREmpCards]', N'utedittime') IS NULL ALTER TABLE [EREmpCards] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmpCards]', N'utedituser') IS NULL ALTER TABLE [EREmpCards] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpCards]', N'uteditsystem') IS NULL ALTER TABLE [EREmpCards] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpCards]', N'uteditip') IS NULL ALTER TABLE [EREmpCards] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpCards]', N'uteditappversion') IS NULL ALTER TABLE [EREmpCards] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmpCards]', N'uteditdbversion') IS NULL ALTER TABLE [EREmpCards] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmpCards]', N'uteditid') IS NULL ALTER TABLE [EREmpCards] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[EREmpCards]', N'uteditsessionid') IS NULL ALTER TABLE [EREmpCards] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[EREmpCards]', N'utrecordversion') IS NULL ALTER TABLE [EREmpCards] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[EREmpCards]', N'utblock') IS NULL ALTER TABLE [EREmpCards] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[EREmpCards]', N'crdtypeid') IS NULL ALTER TABLE [EREmpCards] ADD [crdtypeid] int NULL;
GO
IF COL_LENGTH(N'[EREmpCards]', N'authtypeid') IS NULL ALTER TABLE [EREmpCards] ADD [authtypeid] int NULL;
GO
IF COL_LENGTH(N'[EREmpCards]', N'authid') IS NULL ALTER TABLE [EREmpCards] ADD [authid] int NULL;
GO
IF OBJECT_ID(N'[EREmpExperience]', N'U') IS NULL BEGIN CREATE TABLE [EREmpExperience] (
    [id] int NOT NULL,
    [code] nvarchar(20) NULL,
    [employeeid] int NULL,
    [busnatureid] int NULL,
    [company] nvarchar(150) NULL,
    [department] nvarchar(50) NULL,
    [designation] nvarchar(50) NULL,
    [startdate] datetime2 NULL,
    [enddate] datetime2 NULL,
    [salary] int NULL,
    [address] nvarchar(500) NULL,
    [phone] nvarchar(15) NULL,
    [email] nvarchar(50) NULL,
    [web] nvarchar(50) NULL,
    [responsibility] nvarchar(1500) NULL,
    [utdoctype] int NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddip] varchar(25) NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddsessionid] int NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [uteditsystem] varchar(25) NULL,
    [uteditip] varchar(25) NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditsessionid] int NULL,
    [utrecordversion] int NOT NULL,
    [utblock] bit NOT NULL,
    [contactper] nvarchar(150) NULL,
    [persondesignation] nvarchar(150) NULL,
    [personaddress] nvarchar(150) NULL,
    [personphone] nvarchar(20) NULL,
    [personemail] nvarchar(100) NULL,
    [personweb] nvarchar(100) NULL,
    CONSTRAINT [PK_EREmpExperience] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'id') IS NULL ALTER TABLE [EREmpExperience] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'code') IS NULL ALTER TABLE [EREmpExperience] ADD [code] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'employeeid') IS NULL ALTER TABLE [EREmpExperience] ADD [employeeid] int NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'busnatureid') IS NULL ALTER TABLE [EREmpExperience] ADD [busnatureid] int NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'company') IS NULL ALTER TABLE [EREmpExperience] ADD [company] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'department') IS NULL ALTER TABLE [EREmpExperience] ADD [department] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'designation') IS NULL ALTER TABLE [EREmpExperience] ADD [designation] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'startdate') IS NULL ALTER TABLE [EREmpExperience] ADD [startdate] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'enddate') IS NULL ALTER TABLE [EREmpExperience] ADD [enddate] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'salary') IS NULL ALTER TABLE [EREmpExperience] ADD [salary] int NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'address') IS NULL ALTER TABLE [EREmpExperience] ADD [address] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'phone') IS NULL ALTER TABLE [EREmpExperience] ADD [phone] nvarchar(15) NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'email') IS NULL ALTER TABLE [EREmpExperience] ADD [email] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'web') IS NULL ALTER TABLE [EREmpExperience] ADD [web] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'responsibility') IS NULL ALTER TABLE [EREmpExperience] ADD [responsibility] nvarchar(1500) NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'utdoctype') IS NULL ALTER TABLE [EREmpExperience] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'utsubdoctype') IS NULL ALTER TABLE [EREmpExperience] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'utsysgen') IS NULL ALTER TABLE [EREmpExperience] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'utispartial') IS NULL ALTER TABLE [EREmpExperience] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'utaddtime') IS NULL ALTER TABLE [EREmpExperience] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'utadduser') IS NULL ALTER TABLE [EREmpExperience] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'utaddsystem') IS NULL ALTER TABLE [EREmpExperience] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'utaddip') IS NULL ALTER TABLE [EREmpExperience] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'utaddappversion') IS NULL ALTER TABLE [EREmpExperience] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'utadddbversion') IS NULL ALTER TABLE [EREmpExperience] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'utaddid') IS NULL ALTER TABLE [EREmpExperience] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'utaddsessionid') IS NULL ALTER TABLE [EREmpExperience] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'utedittime') IS NULL ALTER TABLE [EREmpExperience] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'utedituser') IS NULL ALTER TABLE [EREmpExperience] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'uteditsystem') IS NULL ALTER TABLE [EREmpExperience] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'uteditip') IS NULL ALTER TABLE [EREmpExperience] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'uteditappversion') IS NULL ALTER TABLE [EREmpExperience] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'uteditdbversion') IS NULL ALTER TABLE [EREmpExperience] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'uteditid') IS NULL ALTER TABLE [EREmpExperience] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'uteditsessionid') IS NULL ALTER TABLE [EREmpExperience] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'utrecordversion') IS NULL ALTER TABLE [EREmpExperience] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'utblock') IS NULL ALTER TABLE [EREmpExperience] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'contactper') IS NULL ALTER TABLE [EREmpExperience] ADD [contactper] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'persondesignation') IS NULL ALTER TABLE [EREmpExperience] ADD [persondesignation] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'personaddress') IS NULL ALTER TABLE [EREmpExperience] ADD [personaddress] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'personphone') IS NULL ALTER TABLE [EREmpExperience] ADD [personphone] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'personemail') IS NULL ALTER TABLE [EREmpExperience] ADD [personemail] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[EREmpExperience]', N'personweb') IS NULL ALTER TABLE [EREmpExperience] ADD [personweb] nvarchar(100) NULL;
GO
IF OBJECT_ID(N'[EREmployeedisease]', N'U') IS NULL BEGIN CREATE TABLE [EREmployeedisease] (
    [id] int NOT NULL,
    [lineindex] int NULL,
    [employeeid] int NULL,
    [diseasesid] int NULL,
    [discatid] int NULL,
    [distypeid] int NULL,
    [stxt] nvarchar(150) NULL,
    [ltxt] nvarchar(500) NULL,
    [utdoctype] int NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddip] varchar(25) NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddsessionid] int NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [uteditsystem] varchar(25) NULL,
    [uteditip] varchar(25) NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditsessionid] int NULL,
    [utrecordversion] int NOT NULL,
    [utblock] bit NOT NULL,
    CONSTRAINT [PK_EREmployeedisease] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[EREmployeedisease]', N'id') IS NULL ALTER TABLE [EREmployeedisease] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[EREmployeedisease]', N'lineindex') IS NULL ALTER TABLE [EREmployeedisease] ADD [lineindex] int NULL;
GO
IF COL_LENGTH(N'[EREmployeedisease]', N'employeeid') IS NULL ALTER TABLE [EREmployeedisease] ADD [employeeid] int NULL;
GO
IF COL_LENGTH(N'[EREmployeedisease]', N'diseasesid') IS NULL ALTER TABLE [EREmployeedisease] ADD [diseasesid] int NULL;
GO
IF COL_LENGTH(N'[EREmployeedisease]', N'discatid') IS NULL ALTER TABLE [EREmployeedisease] ADD [discatid] int NULL;
GO
IF COL_LENGTH(N'[EREmployeedisease]', N'distypeid') IS NULL ALTER TABLE [EREmployeedisease] ADD [distypeid] int NULL;
GO
IF COL_LENGTH(N'[EREmployeedisease]', N'stxt') IS NULL ALTER TABLE [EREmployeedisease] ADD [stxt] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[EREmployeedisease]', N'ltxt') IS NULL ALTER TABLE [EREmployeedisease] ADD [ltxt] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[EREmployeedisease]', N'utdoctype') IS NULL ALTER TABLE [EREmployeedisease] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[EREmployeedisease]', N'utsubdoctype') IS NULL ALTER TABLE [EREmployeedisease] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[EREmployeedisease]', N'utsysgen') IS NULL ALTER TABLE [EREmployeedisease] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[EREmployeedisease]', N'utispartial') IS NULL ALTER TABLE [EREmployeedisease] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[EREmployeedisease]', N'utaddtime') IS NULL ALTER TABLE [EREmployeedisease] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmployeedisease]', N'utadduser') IS NULL ALTER TABLE [EREmployeedisease] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmployeedisease]', N'utaddsystem') IS NULL ALTER TABLE [EREmployeedisease] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmployeedisease]', N'utaddip') IS NULL ALTER TABLE [EREmployeedisease] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmployeedisease]', N'utaddappversion') IS NULL ALTER TABLE [EREmployeedisease] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmployeedisease]', N'utadddbversion') IS NULL ALTER TABLE [EREmployeedisease] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmployeedisease]', N'utaddid') IS NULL ALTER TABLE [EREmployeedisease] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[EREmployeedisease]', N'utaddsessionid') IS NULL ALTER TABLE [EREmployeedisease] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[EREmployeedisease]', N'utedittime') IS NULL ALTER TABLE [EREmployeedisease] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmployeedisease]', N'utedituser') IS NULL ALTER TABLE [EREmployeedisease] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmployeedisease]', N'uteditsystem') IS NULL ALTER TABLE [EREmployeedisease] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmployeedisease]', N'uteditip') IS NULL ALTER TABLE [EREmployeedisease] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmployeedisease]', N'uteditappversion') IS NULL ALTER TABLE [EREmployeedisease] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmployeedisease]', N'uteditdbversion') IS NULL ALTER TABLE [EREmployeedisease] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmployeedisease]', N'uteditid') IS NULL ALTER TABLE [EREmployeedisease] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[EREmployeedisease]', N'uteditsessionid') IS NULL ALTER TABLE [EREmployeedisease] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[EREmployeedisease]', N'utrecordversion') IS NULL ALTER TABLE [EREmployeedisease] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[EREmployeedisease]', N'utblock') IS NULL ALTER TABLE [EREmployeedisease] ADD [utblock] bit NULL;
GO
IF OBJECT_ID(N'[EREmployeesubjectskill]', N'U') IS NULL BEGIN CREATE TABLE [EREmployeesubjectskill] (
    [Id] int NOT NULL,
    [employeeid] int NULL,
    [subjecttypeid] int NULL,
    [ratingdtlid] int NULL,
    [ratingvalue] int NULL,
    [utdoctype] int NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddip] varchar(25) NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddsessionid] int NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [uteditsystem] varchar(25) NULL,
    [uteditip] varchar(25) NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditsessionid] int NULL,
    [utrecordversion] int NOT NULL,
    CONSTRAINT [PK_EREmployeesubjectskill] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[EREmployeesubjectskill]', N'Id') IS NULL ALTER TABLE [EREmployeesubjectskill] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[EREmployeesubjectskill]', N'employeeid') IS NULL ALTER TABLE [EREmployeesubjectskill] ADD [employeeid] int NULL;
GO
IF COL_LENGTH(N'[EREmployeesubjectskill]', N'subjecttypeid') IS NULL ALTER TABLE [EREmployeesubjectskill] ADD [subjecttypeid] int NULL;
GO
IF COL_LENGTH(N'[EREmployeesubjectskill]', N'ratingdtlid') IS NULL ALTER TABLE [EREmployeesubjectskill] ADD [ratingdtlid] int NULL;
GO
IF COL_LENGTH(N'[EREmployeesubjectskill]', N'ratingvalue') IS NULL ALTER TABLE [EREmployeesubjectskill] ADD [ratingvalue] int NULL;
GO
IF COL_LENGTH(N'[EREmployeesubjectskill]', N'utdoctype') IS NULL ALTER TABLE [EREmployeesubjectskill] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[EREmployeesubjectskill]', N'utsubdoctype') IS NULL ALTER TABLE [EREmployeesubjectskill] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[EREmployeesubjectskill]', N'utsysgen') IS NULL ALTER TABLE [EREmployeesubjectskill] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[EREmployeesubjectskill]', N'utblock') IS NULL ALTER TABLE [EREmployeesubjectskill] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[EREmployeesubjectskill]', N'utispartial') IS NULL ALTER TABLE [EREmployeesubjectskill] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[EREmployeesubjectskill]', N'utaddtime') IS NULL ALTER TABLE [EREmployeesubjectskill] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmployeesubjectskill]', N'utadduser') IS NULL ALTER TABLE [EREmployeesubjectskill] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmployeesubjectskill]', N'utaddsystem') IS NULL ALTER TABLE [EREmployeesubjectskill] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmployeesubjectskill]', N'utaddip') IS NULL ALTER TABLE [EREmployeesubjectskill] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmployeesubjectskill]', N'utaddappversion') IS NULL ALTER TABLE [EREmployeesubjectskill] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmployeesubjectskill]', N'utadddbversion') IS NULL ALTER TABLE [EREmployeesubjectskill] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmployeesubjectskill]', N'utaddid') IS NULL ALTER TABLE [EREmployeesubjectskill] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[EREmployeesubjectskill]', N'utaddsessionid') IS NULL ALTER TABLE [EREmployeesubjectskill] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[EREmployeesubjectskill]', N'utedittime') IS NULL ALTER TABLE [EREmployeesubjectskill] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmployeesubjectskill]', N'utedituser') IS NULL ALTER TABLE [EREmployeesubjectskill] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmployeesubjectskill]', N'uteditsystem') IS NULL ALTER TABLE [EREmployeesubjectskill] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmployeesubjectskill]', N'uteditip') IS NULL ALTER TABLE [EREmployeesubjectskill] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmployeesubjectskill]', N'uteditappversion') IS NULL ALTER TABLE [EREmployeesubjectskill] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmployeesubjectskill]', N'uteditdbversion') IS NULL ALTER TABLE [EREmployeesubjectskill] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmployeesubjectskill]', N'uteditid') IS NULL ALTER TABLE [EREmployeesubjectskill] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[EREmployeesubjectskill]', N'uteditsessionid') IS NULL ALTER TABLE [EREmployeesubjectskill] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[EREmployeesubjectskill]', N'utrecordversion') IS NULL ALTER TABLE [EREmployeesubjectskill] ADD [utrecordversion] int NULL;
GO
IF OBJECT_ID(N'[EREmppayroll]', N'U') IS NULL BEGIN CREATE TABLE [EREmppayroll] (
    [Id] int NOT NULL,
    [employeeid] int NULL,
    [loanallowedid] int NULL,
    [advanceallowedid] int NULL,
    [ovtallowedid] int NULL,
    [ovtpolicyid] int NULL,
    [payfullsalary] bit NOT NULL,
    [utsysgen] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utblock] bit NOT NULL,
    [isloanallowed] bit NULL,
    [isadvallowed] bit NULL,
    [isovtallowed] bit NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_EREmppayroll] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[EREmppayroll]', N'Id') IS NULL ALTER TABLE [EREmppayroll] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[EREmppayroll]', N'employeeid') IS NULL ALTER TABLE [EREmppayroll] ADD [employeeid] int NULL;
GO
IF COL_LENGTH(N'[EREmppayroll]', N'loanallowedid') IS NULL ALTER TABLE [EREmppayroll] ADD [loanallowedid] int NULL;
GO
IF COL_LENGTH(N'[EREmppayroll]', N'advanceallowedid') IS NULL ALTER TABLE [EREmppayroll] ADD [advanceallowedid] int NULL;
GO
IF COL_LENGTH(N'[EREmppayroll]', N'ovtallowedid') IS NULL ALTER TABLE [EREmppayroll] ADD [ovtallowedid] int NULL;
GO
IF COL_LENGTH(N'[EREmppayroll]', N'ovtpolicyid') IS NULL ALTER TABLE [EREmppayroll] ADD [ovtpolicyid] int NULL;
GO
IF COL_LENGTH(N'[EREmppayroll]', N'payfullsalary') IS NULL ALTER TABLE [EREmppayroll] ADD [payfullsalary] bit NULL;
GO
IF COL_LENGTH(N'[EREmppayroll]', N'utsysgen') IS NULL ALTER TABLE [EREmppayroll] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[EREmppayroll]', N'utispartial') IS NULL ALTER TABLE [EREmppayroll] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[EREmppayroll]', N'utrecordversion') IS NULL ALTER TABLE [EREmppayroll] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[EREmppayroll]', N'utblock') IS NULL ALTER TABLE [EREmppayroll] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[EREmppayroll]', N'isloanallowed') IS NULL ALTER TABLE [EREmppayroll] ADD [isloanallowed] bit NULL;
GO
IF COL_LENGTH(N'[EREmppayroll]', N'isadvallowed') IS NULL ALTER TABLE [EREmppayroll] ADD [isadvallowed] bit NULL;
GO
IF COL_LENGTH(N'[EREmppayroll]', N'isovtallowed') IS NULL ALTER TABLE [EREmppayroll] ADD [isovtallowed] bit NULL;
GO
IF COL_LENGTH(N'[EREmppayroll]', N'utaddappversion') IS NULL ALTER TABLE [EREmppayroll] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmppayroll]', N'utadddbversion') IS NULL ALTER TABLE [EREmppayroll] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmppayroll]', N'utaddid') IS NULL ALTER TABLE [EREmppayroll] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[EREmppayroll]', N'utaddip') IS NULL ALTER TABLE [EREmppayroll] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmppayroll]', N'utaddsessionid') IS NULL ALTER TABLE [EREmppayroll] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[EREmppayroll]', N'utaddsystem') IS NULL ALTER TABLE [EREmppayroll] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmppayroll]', N'utaddtime') IS NULL ALTER TABLE [EREmppayroll] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmppayroll]', N'utadduser') IS NULL ALTER TABLE [EREmppayroll] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmppayroll]', N'utdoctype') IS NULL ALTER TABLE [EREmppayroll] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[EREmppayroll]', N'uteditappversion') IS NULL ALTER TABLE [EREmppayroll] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmppayroll]', N'uteditdbversion') IS NULL ALTER TABLE [EREmppayroll] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmppayroll]', N'uteditid') IS NULL ALTER TABLE [EREmppayroll] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[EREmppayroll]', N'uteditip') IS NULL ALTER TABLE [EREmppayroll] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmppayroll]', N'uteditsessionid') IS NULL ALTER TABLE [EREmppayroll] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[EREmppayroll]', N'uteditsystem') IS NULL ALTER TABLE [EREmppayroll] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmppayroll]', N'utedittime') IS NULL ALTER TABLE [EREmppayroll] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmppayroll]', N'utedituser') IS NULL ALTER TABLE [EREmppayroll] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmppayroll]', N'utsubdoctype') IS NULL ALTER TABLE [EREmppayroll] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[EREmpPosition]', N'U') IS NULL BEGIN CREATE TABLE [EREmpPosition] (
    [id] int NOT NULL,
    [employeeid] int NULL,
    [deptid] int NULL,
    [desgid] int NULL,
    [gradeid] int NULL,
    [actionid] int NULL,
    [remarks] nvarchar(1500) NULL,
    [fromdate] datetime NULL,
    [tilldate] datetime NULL,
    [responsibility] nvarchar(1500) NULL,
    [utblock] bit NULL,
    [utedittime] datetime2 NULL,
    [utrecordversion] int NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedituser] varchar(25) NULL,
    [utispartial] bit NOT NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    CONSTRAINT [PK_EREmpPosition] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[EREmpPosition]', N'id') IS NULL ALTER TABLE [EREmpPosition] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[EREmpPosition]', N'employeeid') IS NULL ALTER TABLE [EREmpPosition] ADD [employeeid] int NULL;
GO
IF COL_LENGTH(N'[EREmpPosition]', N'deptid') IS NULL ALTER TABLE [EREmpPosition] ADD [deptid] int NULL;
GO
IF COL_LENGTH(N'[EREmpPosition]', N'desgid') IS NULL ALTER TABLE [EREmpPosition] ADD [desgid] int NULL;
GO
IF COL_LENGTH(N'[EREmpPosition]', N'gradeid') IS NULL ALTER TABLE [EREmpPosition] ADD [gradeid] int NULL;
GO
IF COL_LENGTH(N'[EREmpPosition]', N'actionid') IS NULL ALTER TABLE [EREmpPosition] ADD [actionid] int NULL;
GO
IF COL_LENGTH(N'[EREmpPosition]', N'remarks') IS NULL ALTER TABLE [EREmpPosition] ADD [remarks] nvarchar(1500) NULL;
GO
IF COL_LENGTH(N'[EREmpPosition]', N'fromdate') IS NULL ALTER TABLE [EREmpPosition] ADD [fromdate] datetime NULL;
GO
IF COL_LENGTH(N'[EREmpPosition]', N'tilldate') IS NULL ALTER TABLE [EREmpPosition] ADD [tilldate] datetime NULL;
GO
IF COL_LENGTH(N'[EREmpPosition]', N'responsibility') IS NULL ALTER TABLE [EREmpPosition] ADD [responsibility] nvarchar(1500) NULL;
GO
IF COL_LENGTH(N'[EREmpPosition]', N'utblock') IS NULL ALTER TABLE [EREmpPosition] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[EREmpPosition]', N'utedittime') IS NULL ALTER TABLE [EREmpPosition] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmpPosition]', N'utrecordversion') IS NULL ALTER TABLE [EREmpPosition] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[EREmpPosition]', N'utaddappversion') IS NULL ALTER TABLE [EREmpPosition] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmpPosition]', N'utadddbversion') IS NULL ALTER TABLE [EREmpPosition] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmpPosition]', N'utaddid') IS NULL ALTER TABLE [EREmpPosition] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[EREmpPosition]', N'utaddip') IS NULL ALTER TABLE [EREmpPosition] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpPosition]', N'utaddsessionid') IS NULL ALTER TABLE [EREmpPosition] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[EREmpPosition]', N'utaddsystem') IS NULL ALTER TABLE [EREmpPosition] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpPosition]', N'utaddtime') IS NULL ALTER TABLE [EREmpPosition] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmpPosition]', N'utadduser') IS NULL ALTER TABLE [EREmpPosition] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpPosition]', N'utdoctype') IS NULL ALTER TABLE [EREmpPosition] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[EREmpPosition]', N'uteditappversion') IS NULL ALTER TABLE [EREmpPosition] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmpPosition]', N'uteditdbversion') IS NULL ALTER TABLE [EREmpPosition] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmpPosition]', N'uteditid') IS NULL ALTER TABLE [EREmpPosition] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[EREmpPosition]', N'uteditip') IS NULL ALTER TABLE [EREmpPosition] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpPosition]', N'uteditsessionid') IS NULL ALTER TABLE [EREmpPosition] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[EREmpPosition]', N'uteditsystem') IS NULL ALTER TABLE [EREmpPosition] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpPosition]', N'utedituser') IS NULL ALTER TABLE [EREmpPosition] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpPosition]', N'utispartial') IS NULL ALTER TABLE [EREmpPosition] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[EREmpPosition]', N'utsubdoctype') IS NULL ALTER TABLE [EREmpPosition] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[EREmpPosition]', N'utsysgen') IS NULL ALTER TABLE [EREmpPosition] ADD [utsysgen] bit NULL;
GO
IF OBJECT_ID(N'[EREmpRelation]', N'U') IS NULL BEGIN CREATE TABLE [EREmpRelation] (
    [id] int NOT NULL,
    [code] nvarchar(20) NULL,
    [employeeid] int NULL,
    [relationtypeid] int NULL,
    [genderid] int NULL,
    [firstname] nvarchar(50) NULL,
    [lastname] nvarchar(50) NULL,
    [address] nvarchar(500) NULL,
    [phoneno] nvarchar(15) NULL,
    [mobileno] nvarchar(15) NULL,
    [email] nvarchar(100) NULL,
    [emergency] bit NULL,
    [nextofkin] bit NULL,
    [utsysgen] bit NOT NULL,
    [utblock1] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utrecordversion] int NOT NULL,
    [utblock] bit NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_EREmpRelation] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[EREmpRelation]', N'id') IS NULL ALTER TABLE [EREmpRelation] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[EREmpRelation]', N'code') IS NULL ALTER TABLE [EREmpRelation] ADD [code] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[EREmpRelation]', N'employeeid') IS NULL ALTER TABLE [EREmpRelation] ADD [employeeid] int NULL;
GO
IF COL_LENGTH(N'[EREmpRelation]', N'relationtypeid') IS NULL ALTER TABLE [EREmpRelation] ADD [relationtypeid] int NULL;
GO
IF COL_LENGTH(N'[EREmpRelation]', N'genderid') IS NULL ALTER TABLE [EREmpRelation] ADD [genderid] int NULL;
GO
IF COL_LENGTH(N'[EREmpRelation]', N'firstname') IS NULL ALTER TABLE [EREmpRelation] ADD [firstname] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[EREmpRelation]', N'lastname') IS NULL ALTER TABLE [EREmpRelation] ADD [lastname] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[EREmpRelation]', N'address') IS NULL ALTER TABLE [EREmpRelation] ADD [address] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[EREmpRelation]', N'phoneno') IS NULL ALTER TABLE [EREmpRelation] ADD [phoneno] nvarchar(15) NULL;
GO
IF COL_LENGTH(N'[EREmpRelation]', N'mobileno') IS NULL ALTER TABLE [EREmpRelation] ADD [mobileno] nvarchar(15) NULL;
GO
IF COL_LENGTH(N'[EREmpRelation]', N'email') IS NULL ALTER TABLE [EREmpRelation] ADD [email] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[EREmpRelation]', N'emergency') IS NULL ALTER TABLE [EREmpRelation] ADD [emergency] bit NULL;
GO
IF COL_LENGTH(N'[EREmpRelation]', N'nextofkin') IS NULL ALTER TABLE [EREmpRelation] ADD [nextofkin] bit NULL;
GO
IF COL_LENGTH(N'[EREmpRelation]', N'utsysgen') IS NULL ALTER TABLE [EREmpRelation] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[EREmpRelation]', N'utblock1') IS NULL ALTER TABLE [EREmpRelation] ADD [utblock1] bit NULL;
GO
IF COL_LENGTH(N'[EREmpRelation]', N'utispartial') IS NULL ALTER TABLE [EREmpRelation] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[EREmpRelation]', N'utaddtime') IS NULL ALTER TABLE [EREmpRelation] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmpRelation]', N'utadduser') IS NULL ALTER TABLE [EREmpRelation] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpRelation]', N'utedittime') IS NULL ALTER TABLE [EREmpRelation] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmpRelation]', N'utedituser') IS NULL ALTER TABLE [EREmpRelation] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpRelation]', N'utrecordversion') IS NULL ALTER TABLE [EREmpRelation] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[EREmpRelation]', N'utblock') IS NULL ALTER TABLE [EREmpRelation] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[EREmpRelation]', N'utaddappversion') IS NULL ALTER TABLE [EREmpRelation] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmpRelation]', N'utadddbversion') IS NULL ALTER TABLE [EREmpRelation] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmpRelation]', N'utaddid') IS NULL ALTER TABLE [EREmpRelation] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[EREmpRelation]', N'utaddip') IS NULL ALTER TABLE [EREmpRelation] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpRelation]', N'utaddsessionid') IS NULL ALTER TABLE [EREmpRelation] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[EREmpRelation]', N'utaddsystem') IS NULL ALTER TABLE [EREmpRelation] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpRelation]', N'utdoctype') IS NULL ALTER TABLE [EREmpRelation] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[EREmpRelation]', N'uteditappversion') IS NULL ALTER TABLE [EREmpRelation] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmpRelation]', N'uteditdbversion') IS NULL ALTER TABLE [EREmpRelation] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmpRelation]', N'uteditid') IS NULL ALTER TABLE [EREmpRelation] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[EREmpRelation]', N'uteditip') IS NULL ALTER TABLE [EREmpRelation] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpRelation]', N'uteditsessionid') IS NULL ALTER TABLE [EREmpRelation] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[EREmpRelation]', N'uteditsystem') IS NULL ALTER TABLE [EREmpRelation] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpRelation]', N'utsubdoctype') IS NULL ALTER TABLE [EREmpRelation] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[EREmproaster]', N'U') IS NULL BEGIN CREATE TABLE [EREmproaster] (
    [Id] int NOT NULL,
    [shiftid] int NULL,
    [leavegrpid] int NULL,
    [lateallowed] bit NULL,
    [earlydep] bit NULL,
    [otallowed] bit NULL,
    [startdate] datetime NULL,
    [enddate] datetime NULL,
    [employeeid] int NULL,
    [taxid] int NULL,
    [taxexempt] bit NULL,
    [utblock] bit NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    CONSTRAINT [PK_EREmproaster] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[EREmproaster]', N'Id') IS NULL ALTER TABLE [EREmproaster] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[EREmproaster]', N'shiftid') IS NULL ALTER TABLE [EREmproaster] ADD [shiftid] int NULL;
GO
IF COL_LENGTH(N'[EREmproaster]', N'leavegrpid') IS NULL ALTER TABLE [EREmproaster] ADD [leavegrpid] int NULL;
GO
IF COL_LENGTH(N'[EREmproaster]', N'lateallowed') IS NULL ALTER TABLE [EREmproaster] ADD [lateallowed] bit NULL;
GO
IF COL_LENGTH(N'[EREmproaster]', N'earlydep') IS NULL ALTER TABLE [EREmproaster] ADD [earlydep] bit NULL;
GO
IF COL_LENGTH(N'[EREmproaster]', N'otallowed') IS NULL ALTER TABLE [EREmproaster] ADD [otallowed] bit NULL;
GO
IF COL_LENGTH(N'[EREmproaster]', N'startdate') IS NULL ALTER TABLE [EREmproaster] ADD [startdate] datetime NULL;
GO
IF COL_LENGTH(N'[EREmproaster]', N'enddate') IS NULL ALTER TABLE [EREmproaster] ADD [enddate] datetime NULL;
GO
IF COL_LENGTH(N'[EREmproaster]', N'employeeid') IS NULL ALTER TABLE [EREmproaster] ADD [employeeid] int NULL;
GO
IF COL_LENGTH(N'[EREmproaster]', N'taxid') IS NULL ALTER TABLE [EREmproaster] ADD [taxid] int NULL;
GO
IF COL_LENGTH(N'[EREmproaster]', N'taxexempt') IS NULL ALTER TABLE [EREmproaster] ADD [taxexempt] bit NULL;
GO
IF COL_LENGTH(N'[EREmproaster]', N'utblock') IS NULL ALTER TABLE [EREmproaster] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[EREmproaster]', N'utaddappversion') IS NULL ALTER TABLE [EREmproaster] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmproaster]', N'utadddbversion') IS NULL ALTER TABLE [EREmproaster] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmproaster]', N'utaddid') IS NULL ALTER TABLE [EREmproaster] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[EREmproaster]', N'utaddip') IS NULL ALTER TABLE [EREmproaster] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmproaster]', N'utaddsessionid') IS NULL ALTER TABLE [EREmproaster] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[EREmproaster]', N'utaddsystem') IS NULL ALTER TABLE [EREmproaster] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmproaster]', N'utaddtime') IS NULL ALTER TABLE [EREmproaster] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmproaster]', N'utadduser') IS NULL ALTER TABLE [EREmproaster] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmproaster]', N'utdoctype') IS NULL ALTER TABLE [EREmproaster] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[EREmproaster]', N'uteditappversion') IS NULL ALTER TABLE [EREmproaster] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmproaster]', N'uteditdbversion') IS NULL ALTER TABLE [EREmproaster] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmproaster]', N'uteditid') IS NULL ALTER TABLE [EREmproaster] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[EREmproaster]', N'uteditip') IS NULL ALTER TABLE [EREmproaster] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmproaster]', N'uteditsessionid') IS NULL ALTER TABLE [EREmproaster] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[EREmproaster]', N'uteditsystem') IS NULL ALTER TABLE [EREmproaster] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmproaster]', N'utedittime') IS NULL ALTER TABLE [EREmproaster] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmproaster]', N'utedituser') IS NULL ALTER TABLE [EREmproaster] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmproaster]', N'utispartial') IS NULL ALTER TABLE [EREmproaster] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[EREmproaster]', N'utrecordversion') IS NULL ALTER TABLE [EREmproaster] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[EREmproaster]', N'utsubdoctype') IS NULL ALTER TABLE [EREmproaster] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[EREmproaster]', N'utsysgen') IS NULL ALTER TABLE [EREmproaster] ADD [utsysgen] bit NULL;
GO
IF OBJECT_ID(N'[EREmpSkill]', N'U') IS NULL BEGIN CREATE TABLE [EREmpSkill] (
    [id] int NOT NULL,
    [employeeid] int NULL,
    [skilltypeid] int NULL,
    [skillid] int NULL,
    [institute] nvarchar(500) NULL,
    [levelid] int NULL,
    [rating] int NULL,
    [punitid] int NULL,
    [period] int NULL,
    [year] int NULL,
    [utsysgen] bit NOT NULL,
    [utblock1] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utrecordversion] int NOT NULL,
    [utblock] bit NOT NULL,
    [skillratingid] int NULL,
    [remarks] nvarchar(500) NULL,
    [skillratingdtlid] int NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_EREmpSkill] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[EREmpSkill]', N'id') IS NULL ALTER TABLE [EREmpSkill] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[EREmpSkill]', N'employeeid') IS NULL ALTER TABLE [EREmpSkill] ADD [employeeid] int NULL;
GO
IF COL_LENGTH(N'[EREmpSkill]', N'skilltypeid') IS NULL ALTER TABLE [EREmpSkill] ADD [skilltypeid] int NULL;
GO
IF COL_LENGTH(N'[EREmpSkill]', N'skillid') IS NULL ALTER TABLE [EREmpSkill] ADD [skillid] int NULL;
GO
IF COL_LENGTH(N'[EREmpSkill]', N'institute') IS NULL ALTER TABLE [EREmpSkill] ADD [institute] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[EREmpSkill]', N'levelid') IS NULL ALTER TABLE [EREmpSkill] ADD [levelid] int NULL;
GO
IF COL_LENGTH(N'[EREmpSkill]', N'rating') IS NULL ALTER TABLE [EREmpSkill] ADD [rating] int NULL;
GO
IF COL_LENGTH(N'[EREmpSkill]', N'punitid') IS NULL ALTER TABLE [EREmpSkill] ADD [punitid] int NULL;
GO
IF COL_LENGTH(N'[EREmpSkill]', N'period') IS NULL ALTER TABLE [EREmpSkill] ADD [period] int NULL;
GO
IF COL_LENGTH(N'[EREmpSkill]', N'year') IS NULL ALTER TABLE [EREmpSkill] ADD [year] int NULL;
GO
IF COL_LENGTH(N'[EREmpSkill]', N'utsysgen') IS NULL ALTER TABLE [EREmpSkill] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[EREmpSkill]', N'utblock1') IS NULL ALTER TABLE [EREmpSkill] ADD [utblock1] bit NULL;
GO
IF COL_LENGTH(N'[EREmpSkill]', N'utispartial') IS NULL ALTER TABLE [EREmpSkill] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[EREmpSkill]', N'utaddtime') IS NULL ALTER TABLE [EREmpSkill] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmpSkill]', N'utadduser') IS NULL ALTER TABLE [EREmpSkill] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpSkill]', N'utedittime') IS NULL ALTER TABLE [EREmpSkill] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmpSkill]', N'utedituser') IS NULL ALTER TABLE [EREmpSkill] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpSkill]', N'utrecordversion') IS NULL ALTER TABLE [EREmpSkill] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[EREmpSkill]', N'utblock') IS NULL ALTER TABLE [EREmpSkill] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[EREmpSkill]', N'skillratingid') IS NULL ALTER TABLE [EREmpSkill] ADD [skillratingid] int NULL;
GO
IF COL_LENGTH(N'[EREmpSkill]', N'remarks') IS NULL ALTER TABLE [EREmpSkill] ADD [remarks] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[EREmpSkill]', N'skillratingdtlid') IS NULL ALTER TABLE [EREmpSkill] ADD [skillratingdtlid] int NULL;
GO
IF COL_LENGTH(N'[EREmpSkill]', N'utaddappversion') IS NULL ALTER TABLE [EREmpSkill] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmpSkill]', N'utadddbversion') IS NULL ALTER TABLE [EREmpSkill] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmpSkill]', N'utaddid') IS NULL ALTER TABLE [EREmpSkill] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[EREmpSkill]', N'utaddip') IS NULL ALTER TABLE [EREmpSkill] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpSkill]', N'utaddsessionid') IS NULL ALTER TABLE [EREmpSkill] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[EREmpSkill]', N'utaddsystem') IS NULL ALTER TABLE [EREmpSkill] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpSkill]', N'utdoctype') IS NULL ALTER TABLE [EREmpSkill] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[EREmpSkill]', N'uteditappversion') IS NULL ALTER TABLE [EREmpSkill] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmpSkill]', N'uteditdbversion') IS NULL ALTER TABLE [EREmpSkill] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmpSkill]', N'uteditid') IS NULL ALTER TABLE [EREmpSkill] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[EREmpSkill]', N'uteditip') IS NULL ALTER TABLE [EREmpSkill] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpSkill]', N'uteditsessionid') IS NULL ALTER TABLE [EREmpSkill] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[EREmpSkill]', N'uteditsystem') IS NULL ALTER TABLE [EREmpSkill] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmpSkill]', N'utsubdoctype') IS NULL ALTER TABLE [EREmpSkill] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[ETLeaveRequest]', N'U') IS NULL BEGIN CREATE TABLE [ETLeaveRequest] (
    [Id] int NOT NULL,
    [requestno] nvarchar(20) NULL,
    [requestdate] datetime2 NULL,
    [employeeid] int NULL,
    [leavetypeid] int NULL,
    [leaveunitid] int NULL,
    [leavedays] float NULL,
    [DOB] datetime2 NULL,
    [inleuedate] datetime2 NULL,
    [fromdate] datetime2 NULL,
    [tilldate] datetime2 NULL,
    [fromtime] datetime2 NULL,
    [tilltime] datetime2 NULL,
    [reason] nvarchar(1500) NULL,
    [remarks] nvarchar(1500) NULL,
    [statusid] int NULL,
    [approvaldate] datetime2 NULL,
    [approvedbyid] int NULL,
    [entityid] int NULL,
    [clientid] int NULL,
    [utblock] bit NOT NULL,
    [utsysgen] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_ETLeaveRequest] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'Id') IS NULL ALTER TABLE [ETLeaveRequest] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'requestno') IS NULL ALTER TABLE [ETLeaveRequest] ADD [requestno] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'requestdate') IS NULL ALTER TABLE [ETLeaveRequest] ADD [requestdate] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'employeeid') IS NULL ALTER TABLE [ETLeaveRequest] ADD [employeeid] int NULL;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'leavetypeid') IS NULL ALTER TABLE [ETLeaveRequest] ADD [leavetypeid] int NULL;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'leaveunitid') IS NULL ALTER TABLE [ETLeaveRequest] ADD [leaveunitid] int NULL;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'leavedays') IS NULL ALTER TABLE [ETLeaveRequest] ADD [leavedays] float NULL;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'DOB') IS NULL ALTER TABLE [ETLeaveRequest] ADD [DOB] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'inleuedate') IS NULL ALTER TABLE [ETLeaveRequest] ADD [inleuedate] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'fromdate') IS NULL ALTER TABLE [ETLeaveRequest] ADD [fromdate] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'tilldate') IS NULL ALTER TABLE [ETLeaveRequest] ADD [tilldate] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'fromtime') IS NULL ALTER TABLE [ETLeaveRequest] ADD [fromtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'tilltime') IS NULL ALTER TABLE [ETLeaveRequest] ADD [tilltime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'reason') IS NULL ALTER TABLE [ETLeaveRequest] ADD [reason] nvarchar(1500) NULL;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'remarks') IS NULL ALTER TABLE [ETLeaveRequest] ADD [remarks] nvarchar(1500) NULL;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'statusid') IS NULL ALTER TABLE [ETLeaveRequest] ADD [statusid] int NULL;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'approvaldate') IS NULL ALTER TABLE [ETLeaveRequest] ADD [approvaldate] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'approvedbyid') IS NULL ALTER TABLE [ETLeaveRequest] ADD [approvedbyid] int NULL;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'entityid') IS NULL ALTER TABLE [ETLeaveRequest] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'clientid') IS NULL ALTER TABLE [ETLeaveRequest] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'utblock') IS NULL ALTER TABLE [ETLeaveRequest] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'utsysgen') IS NULL ALTER TABLE [ETLeaveRequest] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'utispartial') IS NULL ALTER TABLE [ETLeaveRequest] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'utrecordversion') IS NULL ALTER TABLE [ETLeaveRequest] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'utaddappversion') IS NULL ALTER TABLE [ETLeaveRequest] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'utadddbversion') IS NULL ALTER TABLE [ETLeaveRequest] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'utaddid') IS NULL ALTER TABLE [ETLeaveRequest] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'utaddip') IS NULL ALTER TABLE [ETLeaveRequest] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'utaddsessionid') IS NULL ALTER TABLE [ETLeaveRequest] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'utaddsystem') IS NULL ALTER TABLE [ETLeaveRequest] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'utaddtime') IS NULL ALTER TABLE [ETLeaveRequest] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'utadduser') IS NULL ALTER TABLE [ETLeaveRequest] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'utdoctype') IS NULL ALTER TABLE [ETLeaveRequest] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'uteditappversion') IS NULL ALTER TABLE [ETLeaveRequest] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'uteditdbversion') IS NULL ALTER TABLE [ETLeaveRequest] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'uteditid') IS NULL ALTER TABLE [ETLeaveRequest] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'uteditip') IS NULL ALTER TABLE [ETLeaveRequest] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'uteditsessionid') IS NULL ALTER TABLE [ETLeaveRequest] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'uteditsystem') IS NULL ALTER TABLE [ETLeaveRequest] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'utedittime') IS NULL ALTER TABLE [ETLeaveRequest] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'utedituser') IS NULL ALTER TABLE [ETLeaveRequest] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETLeaveRequest]', N'utsubdoctype') IS NULL ALTER TABLE [ETLeaveRequest] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[HREmployeeInformationRecord]', N'U') IS NULL BEGIN CREATE TABLE [HREmployeeInformationRecord] (
    [id] int NOT NULL IDENTITY,
    [employeeid] int NOT NULL,
    [clientid] int NOT NULL,
    [entityid] int NULL,
    [categoryid] int NOT NULL,
    [category] nvarchar(30) NULL,
    [title] nvarchar(250) NULL,
    [referencenumber] nvarchar(100) NULL,
    [recorddate] datetime2 NOT NULL,
    [enddate] datetime2 NULL,
    [status] nvarchar(50) NULL,
    [authority] nvarchar(200) NULL,
    [subcategory] nvarchar(100) NULL,
    [mode] nvarchar(50) NULL,
    [details] nvarchar(2000) NULL,
    [outcome] nvarchar(2000) NULL,
    [remarks] nvarchar(2000) NULL,
    [filename] nvarchar(260) NULL,
    [filecontenttype] nvarchar(150) NULL,
    [filedata] varbinary(max) NULL,
    [createdat] datetime2 NOT NULL,
    [createdby] nvarchar(100) NULL,
    [updatedat] datetime2 NULL,
    [updatedby] nvarchar(100) NULL,
    [isdeleted] bit NOT NULL,
    CONSTRAINT [PK_HREmployeeInformationRecord] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[HREmployeeInformationRecord]', N'id') IS NULL ALTER TABLE [HREmployeeInformationRecord] ADD [id] int NULL ;
GO
IF COL_LENGTH(N'[HREmployeeInformationRecord]', N'employeeid') IS NULL ALTER TABLE [HREmployeeInformationRecord] ADD [employeeid] int NULL;
GO
IF COL_LENGTH(N'[HREmployeeInformationRecord]', N'clientid') IS NULL ALTER TABLE [HREmployeeInformationRecord] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[HREmployeeInformationRecord]', N'entityid') IS NULL ALTER TABLE [HREmployeeInformationRecord] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[HREmployeeInformationRecord]', N'categoryid') IS NULL ALTER TABLE [HREmployeeInformationRecord] ADD [categoryid] int NULL;
GO
IF COL_LENGTH(N'[HREmployeeInformationRecord]', N'category') IS NULL ALTER TABLE [HREmployeeInformationRecord] ADD [category] nvarchar(30) NULL;
GO
IF COL_LENGTH(N'[HREmployeeInformationRecord]', N'title') IS NULL ALTER TABLE [HREmployeeInformationRecord] ADD [title] nvarchar(250) NULL;
GO
IF COL_LENGTH(N'[HREmployeeInformationRecord]', N'referencenumber') IS NULL ALTER TABLE [HREmployeeInformationRecord] ADD [referencenumber] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[HREmployeeInformationRecord]', N'recorddate') IS NULL ALTER TABLE [HREmployeeInformationRecord] ADD [recorddate] datetime2 NULL;
GO
IF COL_LENGTH(N'[HREmployeeInformationRecord]', N'enddate') IS NULL ALTER TABLE [HREmployeeInformationRecord] ADD [enddate] datetime2 NULL;
GO
IF COL_LENGTH(N'[HREmployeeInformationRecord]', N'status') IS NULL ALTER TABLE [HREmployeeInformationRecord] ADD [status] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[HREmployeeInformationRecord]', N'authority') IS NULL ALTER TABLE [HREmployeeInformationRecord] ADD [authority] nvarchar(200) NULL;
GO
IF COL_LENGTH(N'[HREmployeeInformationRecord]', N'subcategory') IS NULL ALTER TABLE [HREmployeeInformationRecord] ADD [subcategory] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[HREmployeeInformationRecord]', N'mode') IS NULL ALTER TABLE [HREmployeeInformationRecord] ADD [mode] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[HREmployeeInformationRecord]', N'details') IS NULL ALTER TABLE [HREmployeeInformationRecord] ADD [details] nvarchar(2000) NULL;
GO
IF COL_LENGTH(N'[HREmployeeInformationRecord]', N'outcome') IS NULL ALTER TABLE [HREmployeeInformationRecord] ADD [outcome] nvarchar(2000) NULL;
GO
IF COL_LENGTH(N'[HREmployeeInformationRecord]', N'remarks') IS NULL ALTER TABLE [HREmployeeInformationRecord] ADD [remarks] nvarchar(2000) NULL;
GO
IF COL_LENGTH(N'[HREmployeeInformationRecord]', N'filename') IS NULL ALTER TABLE [HREmployeeInformationRecord] ADD [filename] nvarchar(260) NULL;
GO
IF COL_LENGTH(N'[HREmployeeInformationRecord]', N'filecontenttype') IS NULL ALTER TABLE [HREmployeeInformationRecord] ADD [filecontenttype] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[HREmployeeInformationRecord]', N'filedata') IS NULL ALTER TABLE [HREmployeeInformationRecord] ADD [filedata] varbinary(max) NULL;
GO
IF COL_LENGTH(N'[HREmployeeInformationRecord]', N'createdat') IS NULL ALTER TABLE [HREmployeeInformationRecord] ADD [createdat] datetime2 NULL;
GO
IF COL_LENGTH(N'[HREmployeeInformationRecord]', N'createdby') IS NULL ALTER TABLE [HREmployeeInformationRecord] ADD [createdby] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[HREmployeeInformationRecord]', N'updatedat') IS NULL ALTER TABLE [HREmployeeInformationRecord] ADD [updatedat] datetime2 NULL;
GO
IF COL_LENGTH(N'[HREmployeeInformationRecord]', N'updatedby') IS NULL ALTER TABLE [HREmployeeInformationRecord] ADD [updatedby] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[HREmployeeInformationRecord]', N'isdeleted') IS NULL ALTER TABLE [HREmployeeInformationRecord] ADD [isdeleted] bit NULL;
GO
IF OBJECT_ID(N'[HRMonthlyAttendance]', N'U') IS NULL BEGIN CREATE TABLE [HRMonthlyAttendance] (
    [Id] int NOT NULL,
    [employeeid] int NULL,
    [registerid] int NULL,
    [payperiodid] int NULL,
    [payperiodtlid] int NULL,
    [startdate] datetime2 NULL,
    [enddate] datetime2 NULL,
    [disbdate] datetime2 NULL,
    [entityid] int NULL,
    [days] float NULL,
    [presentscount] float NULL,
    [absentscount] float NULL,
    [halfdayscount] float NULL,
    [latecount] float NULL,
    [earlycounts] float NULL,
    [actualdays] float NULL,
    [actualpresent] float NULL,
    [holidays] float NULL,
    [leaves] int NULL,
    [utblock] bit NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    CONSTRAINT [PK_HRMonthlyAttendance] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[HRMonthlyAttendance]', N'Id') IS NULL ALTER TABLE [HRMonthlyAttendance] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[HRMonthlyAttendance]', N'employeeid') IS NULL ALTER TABLE [HRMonthlyAttendance] ADD [employeeid] int NULL;
GO
IF COL_LENGTH(N'[HRMonthlyAttendance]', N'registerid') IS NULL ALTER TABLE [HRMonthlyAttendance] ADD [registerid] int NULL;
GO
IF COL_LENGTH(N'[HRMonthlyAttendance]', N'payperiodid') IS NULL ALTER TABLE [HRMonthlyAttendance] ADD [payperiodid] int NULL;
GO
IF COL_LENGTH(N'[HRMonthlyAttendance]', N'payperiodtlid') IS NULL ALTER TABLE [HRMonthlyAttendance] ADD [payperiodtlid] int NULL;
GO
IF COL_LENGTH(N'[HRMonthlyAttendance]', N'startdate') IS NULL ALTER TABLE [HRMonthlyAttendance] ADD [startdate] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRMonthlyAttendance]', N'enddate') IS NULL ALTER TABLE [HRMonthlyAttendance] ADD [enddate] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRMonthlyAttendance]', N'disbdate') IS NULL ALTER TABLE [HRMonthlyAttendance] ADD [disbdate] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRMonthlyAttendance]', N'entityid') IS NULL ALTER TABLE [HRMonthlyAttendance] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[HRMonthlyAttendance]', N'days') IS NULL ALTER TABLE [HRMonthlyAttendance] ADD [days] float NULL;
GO
IF COL_LENGTH(N'[HRMonthlyAttendance]', N'presentscount') IS NULL ALTER TABLE [HRMonthlyAttendance] ADD [presentscount] float NULL;
GO
IF COL_LENGTH(N'[HRMonthlyAttendance]', N'absentscount') IS NULL ALTER TABLE [HRMonthlyAttendance] ADD [absentscount] float NULL;
GO
IF COL_LENGTH(N'[HRMonthlyAttendance]', N'halfdayscount') IS NULL ALTER TABLE [HRMonthlyAttendance] ADD [halfdayscount] float NULL;
GO
IF COL_LENGTH(N'[HRMonthlyAttendance]', N'latecount') IS NULL ALTER TABLE [HRMonthlyAttendance] ADD [latecount] float NULL;
GO
IF COL_LENGTH(N'[HRMonthlyAttendance]', N'earlycounts') IS NULL ALTER TABLE [HRMonthlyAttendance] ADD [earlycounts] float NULL;
GO
IF COL_LENGTH(N'[HRMonthlyAttendance]', N'actualdays') IS NULL ALTER TABLE [HRMonthlyAttendance] ADD [actualdays] float NULL;
GO
IF COL_LENGTH(N'[HRMonthlyAttendance]', N'actualpresent') IS NULL ALTER TABLE [HRMonthlyAttendance] ADD [actualpresent] float NULL;
GO
IF COL_LENGTH(N'[HRMonthlyAttendance]', N'holidays') IS NULL ALTER TABLE [HRMonthlyAttendance] ADD [holidays] float NULL;
GO
IF COL_LENGTH(N'[HRMonthlyAttendance]', N'leaves') IS NULL ALTER TABLE [HRMonthlyAttendance] ADD [leaves] int NULL;
GO
IF COL_LENGTH(N'[HRMonthlyAttendance]', N'utblock') IS NULL ALTER TABLE [HRMonthlyAttendance] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[HRMonthlyAttendance]', N'utaddappversion') IS NULL ALTER TABLE [HRMonthlyAttendance] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[HRMonthlyAttendance]', N'utadddbversion') IS NULL ALTER TABLE [HRMonthlyAttendance] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[HRMonthlyAttendance]', N'utaddid') IS NULL ALTER TABLE [HRMonthlyAttendance] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[HRMonthlyAttendance]', N'utaddip') IS NULL ALTER TABLE [HRMonthlyAttendance] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRMonthlyAttendance]', N'utaddsessionid') IS NULL ALTER TABLE [HRMonthlyAttendance] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[HRMonthlyAttendance]', N'utaddsystem') IS NULL ALTER TABLE [HRMonthlyAttendance] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRMonthlyAttendance]', N'utaddtime') IS NULL ALTER TABLE [HRMonthlyAttendance] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRMonthlyAttendance]', N'utadduser') IS NULL ALTER TABLE [HRMonthlyAttendance] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRMonthlyAttendance]', N'utdoctype') IS NULL ALTER TABLE [HRMonthlyAttendance] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[HRMonthlyAttendance]', N'uteditappversion') IS NULL ALTER TABLE [HRMonthlyAttendance] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[HRMonthlyAttendance]', N'uteditdbversion') IS NULL ALTER TABLE [HRMonthlyAttendance] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[HRMonthlyAttendance]', N'uteditid') IS NULL ALTER TABLE [HRMonthlyAttendance] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[HRMonthlyAttendance]', N'uteditip') IS NULL ALTER TABLE [HRMonthlyAttendance] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRMonthlyAttendance]', N'uteditsessionid') IS NULL ALTER TABLE [HRMonthlyAttendance] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[HRMonthlyAttendance]', N'uteditsystem') IS NULL ALTER TABLE [HRMonthlyAttendance] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRMonthlyAttendance]', N'utedittime') IS NULL ALTER TABLE [HRMonthlyAttendance] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRMonthlyAttendance]', N'utedituser') IS NULL ALTER TABLE [HRMonthlyAttendance] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRMonthlyAttendance]', N'utispartial') IS NULL ALTER TABLE [HRMonthlyAttendance] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[HRMonthlyAttendance]', N'utrecordversion') IS NULL ALTER TABLE [HRMonthlyAttendance] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[HRMonthlyAttendance]', N'utsubdoctype') IS NULL ALTER TABLE [HRMonthlyAttendance] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[HRMonthlyAttendance]', N'utsysgen') IS NULL ALTER TABLE [HRMonthlyAttendance] ADD [utsysgen] bit NULL;
GO
IF OBJECT_ID(N'[HRpayrollprocess]', N'U') IS NULL BEGIN CREATE TABLE [HRpayrollprocess] (
    [Id] int NOT NULL,
    [transno] nvarchar(50) NULL,
    [transdate] datetime2 NULL,
    [registerid] int NULL,
    [periodid] int NULL,
    [salarytypeid] int NULL,
    [processtypeid] int NULL,
    [employeeid] int NULL,
    [deptid] int NULL,
    [desgid] int NULL,
    [statusid] int NULL,
    [days] int NULL,
    [present] int NULL,
    [absent] int NULL,
    [leaves] int NULL,
    [holidays] int NULL,
    [salary] float NULL,
    [allowance] float NULL,
    [deduction] float NULL,
    [tax] float NULL,
    [grosssalary] float NULL,
    [remarks] nvarchar(500) NULL,
    [entityid] int NULL,
    [clientid] int NULL,
    [perioddtlid] int NULL,
    [loanddeduction] float NULL,
    [latededuction] float NULL,
    [fixedded] float NULL,
    [absded] float NULL,
    [overtime] float NULL,
    [feededuction] float NULL,
    [tilldate] datetime2 NULL,
    [fromdate] datetime2 NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_HRpayrollprocess] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'Id') IS NULL ALTER TABLE [HRpayrollprocess] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'transno') IS NULL ALTER TABLE [HRpayrollprocess] ADD [transno] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'transdate') IS NULL ALTER TABLE [HRpayrollprocess] ADD [transdate] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'registerid') IS NULL ALTER TABLE [HRpayrollprocess] ADD [registerid] int NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'periodid') IS NULL ALTER TABLE [HRpayrollprocess] ADD [periodid] int NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'salarytypeid') IS NULL ALTER TABLE [HRpayrollprocess] ADD [salarytypeid] int NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'processtypeid') IS NULL ALTER TABLE [HRpayrollprocess] ADD [processtypeid] int NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'employeeid') IS NULL ALTER TABLE [HRpayrollprocess] ADD [employeeid] int NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'deptid') IS NULL ALTER TABLE [HRpayrollprocess] ADD [deptid] int NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'desgid') IS NULL ALTER TABLE [HRpayrollprocess] ADD [desgid] int NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'statusid') IS NULL ALTER TABLE [HRpayrollprocess] ADD [statusid] int NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'days') IS NULL ALTER TABLE [HRpayrollprocess] ADD [days] int NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'present') IS NULL ALTER TABLE [HRpayrollprocess] ADD [present] int NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'absent') IS NULL ALTER TABLE [HRpayrollprocess] ADD [absent] int NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'leaves') IS NULL ALTER TABLE [HRpayrollprocess] ADD [leaves] int NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'holidays') IS NULL ALTER TABLE [HRpayrollprocess] ADD [holidays] int NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'salary') IS NULL ALTER TABLE [HRpayrollprocess] ADD [salary] float NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'allowance') IS NULL ALTER TABLE [HRpayrollprocess] ADD [allowance] float NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'deduction') IS NULL ALTER TABLE [HRpayrollprocess] ADD [deduction] float NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'tax') IS NULL ALTER TABLE [HRpayrollprocess] ADD [tax] float NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'grosssalary') IS NULL ALTER TABLE [HRpayrollprocess] ADD [grosssalary] float NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'remarks') IS NULL ALTER TABLE [HRpayrollprocess] ADD [remarks] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'entityid') IS NULL ALTER TABLE [HRpayrollprocess] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'clientid') IS NULL ALTER TABLE [HRpayrollprocess] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'perioddtlid') IS NULL ALTER TABLE [HRpayrollprocess] ADD [perioddtlid] int NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'loanddeduction') IS NULL ALTER TABLE [HRpayrollprocess] ADD [loanddeduction] float NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'latededuction') IS NULL ALTER TABLE [HRpayrollprocess] ADD [latededuction] float NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'fixedded') IS NULL ALTER TABLE [HRpayrollprocess] ADD [fixedded] float NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'absded') IS NULL ALTER TABLE [HRpayrollprocess] ADD [absded] float NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'overtime') IS NULL ALTER TABLE [HRpayrollprocess] ADD [overtime] float NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'feededuction') IS NULL ALTER TABLE [HRpayrollprocess] ADD [feededuction] float NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'tilldate') IS NULL ALTER TABLE [HRpayrollprocess] ADD [tilldate] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'fromdate') IS NULL ALTER TABLE [HRpayrollprocess] ADD [fromdate] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'utsysgen') IS NULL ALTER TABLE [HRpayrollprocess] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'utblock') IS NULL ALTER TABLE [HRpayrollprocess] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'utispartial') IS NULL ALTER TABLE [HRpayrollprocess] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'utrecordversion') IS NULL ALTER TABLE [HRpayrollprocess] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'utaddappversion') IS NULL ALTER TABLE [HRpayrollprocess] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'utadddbversion') IS NULL ALTER TABLE [HRpayrollprocess] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'utaddid') IS NULL ALTER TABLE [HRpayrollprocess] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'utaddip') IS NULL ALTER TABLE [HRpayrollprocess] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'utaddsessionid') IS NULL ALTER TABLE [HRpayrollprocess] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'utaddsystem') IS NULL ALTER TABLE [HRpayrollprocess] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'utaddtime') IS NULL ALTER TABLE [HRpayrollprocess] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'utadduser') IS NULL ALTER TABLE [HRpayrollprocess] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'utdoctype') IS NULL ALTER TABLE [HRpayrollprocess] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'uteditappversion') IS NULL ALTER TABLE [HRpayrollprocess] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'uteditdbversion') IS NULL ALTER TABLE [HRpayrollprocess] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'uteditid') IS NULL ALTER TABLE [HRpayrollprocess] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'uteditip') IS NULL ALTER TABLE [HRpayrollprocess] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'uteditsessionid') IS NULL ALTER TABLE [HRpayrollprocess] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'uteditsystem') IS NULL ALTER TABLE [HRpayrollprocess] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'utedittime') IS NULL ALTER TABLE [HRpayrollprocess] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'utedituser') IS NULL ALTER TABLE [HRpayrollprocess] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocess]', N'utsubdoctype') IS NULL ALTER TABLE [HRpayrollprocess] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[HRVacancyAudit]', N'U') IS NULL BEGIN CREATE TABLE [HRVacancyAudit] (
    [Id] bigint NOT NULL IDENTITY,
    [ClientId] int NOT NULL,
    [EntityId] int NOT NULL,
    [VacancyId] int NOT NULL,
    [PositionId] int NULL,
    [Action] nvarchar(50) NULL,
    [Actor] nvarchar(100) NULL,
    [ActorEmployeeId] int NULL,
    [Remarks] nvarchar(1500) NULL,
    [BeforeJson] nvarchar(max) NULL,
    [AfterJson] nvarchar(max) NULL,
    [OccurredAtUtc] datetime2 NOT NULL,
    [utdoctype] int NOT NULL,
    [utsubdoctype] int NOT NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utaddtime] datetime2 NOT NULL,
    [utadduser] nvarchar(100) NULL,
    [utaddsystem] nvarchar(100) NULL,
    [utaddip] nvarchar(100) NULL,
    [utaddappversion] nvarchar(100) NULL,
    [utadddbversion] nvarchar(100) NULL,
    [utaddid] bigint NOT NULL,
    [utaddsessionid] int NULL,
    [utedittime] datetime2 NOT NULL,
    [utedituser] nvarchar(50) NULL,
    [uteditsystem] nvarchar(50) NULL,
    [uteditip] nvarchar(50) NULL,
    [uteditappversion] nvarchar(50) NULL,
    [uteditdbversion] nvarchar(50) NULL,
    [uteditid] bigint NOT NULL,
    [uteditsessionid] int NULL,
    [utrecordversion] int NOT NULL,
    CONSTRAINT [PK_HRVacancyAudit] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[HRVacancyAudit]', N'Id') IS NULL ALTER TABLE [HRVacancyAudit] ADD [Id] bigint NULL ;
GO
IF COL_LENGTH(N'[HRVacancyAudit]', N'ClientId') IS NULL ALTER TABLE [HRVacancyAudit] ADD [ClientId] int NULL;
GO
IF COL_LENGTH(N'[HRVacancyAudit]', N'EntityId') IS NULL ALTER TABLE [HRVacancyAudit] ADD [EntityId] int NULL;
GO
IF COL_LENGTH(N'[HRVacancyAudit]', N'VacancyId') IS NULL ALTER TABLE [HRVacancyAudit] ADD [VacancyId] int NULL;
GO
IF COL_LENGTH(N'[HRVacancyAudit]', N'PositionId') IS NULL ALTER TABLE [HRVacancyAudit] ADD [PositionId] int NULL;
GO
IF COL_LENGTH(N'[HRVacancyAudit]', N'Action') IS NULL ALTER TABLE [HRVacancyAudit] ADD [Action] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[HRVacancyAudit]', N'Actor') IS NULL ALTER TABLE [HRVacancyAudit] ADD [Actor] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[HRVacancyAudit]', N'ActorEmployeeId') IS NULL ALTER TABLE [HRVacancyAudit] ADD [ActorEmployeeId] int NULL;
GO
IF COL_LENGTH(N'[HRVacancyAudit]', N'Remarks') IS NULL ALTER TABLE [HRVacancyAudit] ADD [Remarks] nvarchar(1500) NULL;
GO
IF COL_LENGTH(N'[HRVacancyAudit]', N'BeforeJson') IS NULL ALTER TABLE [HRVacancyAudit] ADD [BeforeJson] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[HRVacancyAudit]', N'AfterJson') IS NULL ALTER TABLE [HRVacancyAudit] ADD [AfterJson] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[HRVacancyAudit]', N'OccurredAtUtc') IS NULL ALTER TABLE [HRVacancyAudit] ADD [OccurredAtUtc] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRVacancyAudit]', N'utdoctype') IS NULL ALTER TABLE [HRVacancyAudit] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[HRVacancyAudit]', N'utsubdoctype') IS NULL ALTER TABLE [HRVacancyAudit] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[HRVacancyAudit]', N'utsysgen') IS NULL ALTER TABLE [HRVacancyAudit] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[HRVacancyAudit]', N'utblock') IS NULL ALTER TABLE [HRVacancyAudit] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[HRVacancyAudit]', N'utispartial') IS NULL ALTER TABLE [HRVacancyAudit] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[HRVacancyAudit]', N'utaddtime') IS NULL ALTER TABLE [HRVacancyAudit] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRVacancyAudit]', N'utadduser') IS NULL ALTER TABLE [HRVacancyAudit] ADD [utadduser] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[HRVacancyAudit]', N'utaddsystem') IS NULL ALTER TABLE [HRVacancyAudit] ADD [utaddsystem] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[HRVacancyAudit]', N'utaddip') IS NULL ALTER TABLE [HRVacancyAudit] ADD [utaddip] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[HRVacancyAudit]', N'utaddappversion') IS NULL ALTER TABLE [HRVacancyAudit] ADD [utaddappversion] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[HRVacancyAudit]', N'utadddbversion') IS NULL ALTER TABLE [HRVacancyAudit] ADD [utadddbversion] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[HRVacancyAudit]', N'utaddid') IS NULL ALTER TABLE [HRVacancyAudit] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[HRVacancyAudit]', N'utaddsessionid') IS NULL ALTER TABLE [HRVacancyAudit] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[HRVacancyAudit]', N'utedittime') IS NULL ALTER TABLE [HRVacancyAudit] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRVacancyAudit]', N'utedituser') IS NULL ALTER TABLE [HRVacancyAudit] ADD [utedituser] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[HRVacancyAudit]', N'uteditsystem') IS NULL ALTER TABLE [HRVacancyAudit] ADD [uteditsystem] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[HRVacancyAudit]', N'uteditip') IS NULL ALTER TABLE [HRVacancyAudit] ADD [uteditip] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[HRVacancyAudit]', N'uteditappversion') IS NULL ALTER TABLE [HRVacancyAudit] ADD [uteditappversion] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[HRVacancyAudit]', N'uteditdbversion') IS NULL ALTER TABLE [HRVacancyAudit] ADD [uteditdbversion] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[HRVacancyAudit]', N'uteditid') IS NULL ALTER TABLE [HRVacancyAudit] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[HRVacancyAudit]', N'uteditsessionid') IS NULL ALTER TABLE [HRVacancyAudit] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[HRVacancyAudit]', N'utrecordversion') IS NULL ALTER TABLE [HRVacancyAudit] ADD [utrecordversion] int NULL;
GO
IF OBJECT_ID(N'[HRVacancyPosition]', N'U') IS NULL BEGIN CREATE TABLE [HRVacancyPosition] (
    [Id] int NOT NULL IDENTITY,
    [VacancyId] int NOT NULL,
    [RevisionId] int NOT NULL,
    [PositionId] int NOT NULL,
    [LineNumber] int NOT NULL,
    [PositionNumber] nvarchar(30) NULL,
    [Status] nvarchar(30) NULL,
    [CurrentEmployeeId] int NULL,
    [NeedsReview] bit NOT NULL,
    [ReviewReason] nvarchar(500) NULL,
    [CreatedBy] nvarchar(100) NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(100) NULL,
    [UpdatedAtUtc] datetime2 NULL,
    [RowVersion] rowversion NOT NULL,
    [utdoctype] int NOT NULL,
    [utsubdoctype] int NOT NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utaddtime] datetime2 NOT NULL,
    [utadduser] varchar(25) NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddip] varchar(25) NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NOT NULL,
    [utaddsessionid] int NULL,
    [utedittime] datetime2 NOT NULL,
    [utedituser] varchar(25) NULL,
    [uteditsystem] varchar(25) NULL,
    [uteditip] varchar(25) NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NOT NULL,
    [uteditsessionid] int NULL,
    [utrecordversion] int NOT NULL,
    CONSTRAINT [PK_HRVacancyPosition] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[HRVacancyPosition]', N'Id') IS NULL ALTER TABLE [HRVacancyPosition] ADD [Id] int NULL ;
GO
IF COL_LENGTH(N'[HRVacancyPosition]', N'VacancyId') IS NULL ALTER TABLE [HRVacancyPosition] ADD [VacancyId] int NULL;
GO
IF COL_LENGTH(N'[HRVacancyPosition]', N'RevisionId') IS NULL ALTER TABLE [HRVacancyPosition] ADD [RevisionId] int NULL;
GO
IF COL_LENGTH(N'[HRVacancyPosition]', N'PositionId') IS NULL ALTER TABLE [HRVacancyPosition] ADD [PositionId] int NULL;
GO
IF COL_LENGTH(N'[HRVacancyPosition]', N'LineNumber') IS NULL ALTER TABLE [HRVacancyPosition] ADD [LineNumber] int NULL;
GO
IF COL_LENGTH(N'[HRVacancyPosition]', N'PositionNumber') IS NULL ALTER TABLE [HRVacancyPosition] ADD [PositionNumber] nvarchar(30) NULL;
GO
IF COL_LENGTH(N'[HRVacancyPosition]', N'Status') IS NULL ALTER TABLE [HRVacancyPosition] ADD [Status] nvarchar(30) NULL;
GO
IF COL_LENGTH(N'[HRVacancyPosition]', N'CurrentEmployeeId') IS NULL ALTER TABLE [HRVacancyPosition] ADD [CurrentEmployeeId] int NULL;
GO
IF COL_LENGTH(N'[HRVacancyPosition]', N'NeedsReview') IS NULL ALTER TABLE [HRVacancyPosition] ADD [NeedsReview] bit NULL;
GO
IF COL_LENGTH(N'[HRVacancyPosition]', N'ReviewReason') IS NULL ALTER TABLE [HRVacancyPosition] ADD [ReviewReason] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[HRVacancyPosition]', N'CreatedBy') IS NULL ALTER TABLE [HRVacancyPosition] ADD [CreatedBy] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[HRVacancyPosition]', N'CreatedAtUtc') IS NULL ALTER TABLE [HRVacancyPosition] ADD [CreatedAtUtc] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRVacancyPosition]', N'UpdatedBy') IS NULL ALTER TABLE [HRVacancyPosition] ADD [UpdatedBy] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[HRVacancyPosition]', N'UpdatedAtUtc') IS NULL ALTER TABLE [HRVacancyPosition] ADD [UpdatedAtUtc] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRVacancyPosition]', N'RowVersion') IS NULL ALTER TABLE [HRVacancyPosition] ADD [RowVersion] rowversion NULL;
GO
IF COL_LENGTH(N'[HRVacancyPosition]', N'utdoctype') IS NULL ALTER TABLE [HRVacancyPosition] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[HRVacancyPosition]', N'utsubdoctype') IS NULL ALTER TABLE [HRVacancyPosition] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[HRVacancyPosition]', N'utsysgen') IS NULL ALTER TABLE [HRVacancyPosition] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[HRVacancyPosition]', N'utblock') IS NULL ALTER TABLE [HRVacancyPosition] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[HRVacancyPosition]', N'utispartial') IS NULL ALTER TABLE [HRVacancyPosition] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[HRVacancyPosition]', N'utaddtime') IS NULL ALTER TABLE [HRVacancyPosition] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRVacancyPosition]', N'utadduser') IS NULL ALTER TABLE [HRVacancyPosition] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRVacancyPosition]', N'utaddsystem') IS NULL ALTER TABLE [HRVacancyPosition] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRVacancyPosition]', N'utaddip') IS NULL ALTER TABLE [HRVacancyPosition] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRVacancyPosition]', N'utaddappversion') IS NULL ALTER TABLE [HRVacancyPosition] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[HRVacancyPosition]', N'utadddbversion') IS NULL ALTER TABLE [HRVacancyPosition] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[HRVacancyPosition]', N'utaddid') IS NULL ALTER TABLE [HRVacancyPosition] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[HRVacancyPosition]', N'utaddsessionid') IS NULL ALTER TABLE [HRVacancyPosition] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[HRVacancyPosition]', N'utedittime') IS NULL ALTER TABLE [HRVacancyPosition] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRVacancyPosition]', N'utedituser') IS NULL ALTER TABLE [HRVacancyPosition] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRVacancyPosition]', N'uteditsystem') IS NULL ALTER TABLE [HRVacancyPosition] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRVacancyPosition]', N'uteditip') IS NULL ALTER TABLE [HRVacancyPosition] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRVacancyPosition]', N'uteditappversion') IS NULL ALTER TABLE [HRVacancyPosition] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[HRVacancyPosition]', N'uteditdbversion') IS NULL ALTER TABLE [HRVacancyPosition] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[HRVacancyPosition]', N'uteditid') IS NULL ALTER TABLE [HRVacancyPosition] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[HRVacancyPosition]', N'uteditsessionid') IS NULL ALTER TABLE [HRVacancyPosition] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[HRVacancyPosition]', N'utrecordversion') IS NULL ALTER TABLE [HRVacancyPosition] ADD [utrecordversion] int NULL;
GO
IF OBJECT_ID(N'[HRVacancyPositionAssignment]', N'U') IS NULL BEGIN CREATE TABLE [HRVacancyPositionAssignment] (
    [Id] int NOT NULL IDENTITY,
    [PositionId] int NOT NULL,
    [EmployeeId] int NOT NULL,
    [AssignmentType] nvarchar(30) NULL,
    [Status] nvarchar(20) NULL,
    [EffectiveDate] datetime2 NOT NULL,
    [SourcePositionId] int NULL,
    [Remarks] nvarchar(1500) NULL,
    [FailureReason] nvarchar(1500) NULL,
    [CreatedBy] nvarchar(100) NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    [ActivatedAtUtc] datetime2 NULL,
    [utdoctype] int NOT NULL,
    [utsubdoctype] int NOT NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utaddtime] datetime2 NOT NULL,
    [utadduser] varchar(25) NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddip] varchar(25) NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NOT NULL,
    [utaddsessionid] int NOT NULL,
    [utedittime] datetime2 NOT NULL,
    [utedituser] varchar(25) NULL,
    [uteditsystem] varchar(25) NULL,
    [uteditip] varchar(25) NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NOT NULL,
    [uteditsessionid] int NOT NULL,
    [utrecordversion] int NOT NULL,
    CONSTRAINT [PK_HRVacancyPositionAssignment] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[HRVacancyPositionAssignment]', N'Id') IS NULL ALTER TABLE [HRVacancyPositionAssignment] ADD [Id] int NULL ;
GO
IF COL_LENGTH(N'[HRVacancyPositionAssignment]', N'PositionId') IS NULL ALTER TABLE [HRVacancyPositionAssignment] ADD [PositionId] int NULL;
GO
IF COL_LENGTH(N'[HRVacancyPositionAssignment]', N'EmployeeId') IS NULL ALTER TABLE [HRVacancyPositionAssignment] ADD [EmployeeId] int NULL;
GO
IF COL_LENGTH(N'[HRVacancyPositionAssignment]', N'AssignmentType') IS NULL ALTER TABLE [HRVacancyPositionAssignment] ADD [AssignmentType] nvarchar(30) NULL;
GO
IF COL_LENGTH(N'[HRVacancyPositionAssignment]', N'Status') IS NULL ALTER TABLE [HRVacancyPositionAssignment] ADD [Status] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[HRVacancyPositionAssignment]', N'EffectiveDate') IS NULL ALTER TABLE [HRVacancyPositionAssignment] ADD [EffectiveDate] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRVacancyPositionAssignment]', N'SourcePositionId') IS NULL ALTER TABLE [HRVacancyPositionAssignment] ADD [SourcePositionId] int NULL;
GO
IF COL_LENGTH(N'[HRVacancyPositionAssignment]', N'Remarks') IS NULL ALTER TABLE [HRVacancyPositionAssignment] ADD [Remarks] nvarchar(1500) NULL;
GO
IF COL_LENGTH(N'[HRVacancyPositionAssignment]', N'FailureReason') IS NULL ALTER TABLE [HRVacancyPositionAssignment] ADD [FailureReason] nvarchar(1500) NULL;
GO
IF COL_LENGTH(N'[HRVacancyPositionAssignment]', N'CreatedBy') IS NULL ALTER TABLE [HRVacancyPositionAssignment] ADD [CreatedBy] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[HRVacancyPositionAssignment]', N'CreatedAtUtc') IS NULL ALTER TABLE [HRVacancyPositionAssignment] ADD [CreatedAtUtc] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRVacancyPositionAssignment]', N'ActivatedAtUtc') IS NULL ALTER TABLE [HRVacancyPositionAssignment] ADD [ActivatedAtUtc] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRVacancyPositionAssignment]', N'utdoctype') IS NULL ALTER TABLE [HRVacancyPositionAssignment] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[HRVacancyPositionAssignment]', N'utsubdoctype') IS NULL ALTER TABLE [HRVacancyPositionAssignment] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[HRVacancyPositionAssignment]', N'utsysgen') IS NULL ALTER TABLE [HRVacancyPositionAssignment] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[HRVacancyPositionAssignment]', N'utblock') IS NULL ALTER TABLE [HRVacancyPositionAssignment] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[HRVacancyPositionAssignment]', N'utispartial') IS NULL ALTER TABLE [HRVacancyPositionAssignment] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[HRVacancyPositionAssignment]', N'utaddtime') IS NULL ALTER TABLE [HRVacancyPositionAssignment] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRVacancyPositionAssignment]', N'utadduser') IS NULL ALTER TABLE [HRVacancyPositionAssignment] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRVacancyPositionAssignment]', N'utaddsystem') IS NULL ALTER TABLE [HRVacancyPositionAssignment] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRVacancyPositionAssignment]', N'utaddip') IS NULL ALTER TABLE [HRVacancyPositionAssignment] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRVacancyPositionAssignment]', N'utaddappversion') IS NULL ALTER TABLE [HRVacancyPositionAssignment] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[HRVacancyPositionAssignment]', N'utadddbversion') IS NULL ALTER TABLE [HRVacancyPositionAssignment] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[HRVacancyPositionAssignment]', N'utaddid') IS NULL ALTER TABLE [HRVacancyPositionAssignment] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[HRVacancyPositionAssignment]', N'utaddsessionid') IS NULL ALTER TABLE [HRVacancyPositionAssignment] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[HRVacancyPositionAssignment]', N'utedittime') IS NULL ALTER TABLE [HRVacancyPositionAssignment] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRVacancyPositionAssignment]', N'utedituser') IS NULL ALTER TABLE [HRVacancyPositionAssignment] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRVacancyPositionAssignment]', N'uteditsystem') IS NULL ALTER TABLE [HRVacancyPositionAssignment] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRVacancyPositionAssignment]', N'uteditip') IS NULL ALTER TABLE [HRVacancyPositionAssignment] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRVacancyPositionAssignment]', N'uteditappversion') IS NULL ALTER TABLE [HRVacancyPositionAssignment] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[HRVacancyPositionAssignment]', N'uteditdbversion') IS NULL ALTER TABLE [HRVacancyPositionAssignment] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[HRVacancyPositionAssignment]', N'uteditid') IS NULL ALTER TABLE [HRVacancyPositionAssignment] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[HRVacancyPositionAssignment]', N'uteditsessionid') IS NULL ALTER TABLE [HRVacancyPositionAssignment] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[HRVacancyPositionAssignment]', N'utrecordversion') IS NULL ALTER TABLE [HRVacancyPositionAssignment] ADD [utrecordversion] int NULL;
GO
IF OBJECT_ID(N'[HRVacancyRecruiter]', N'U') IS NULL BEGIN CREATE TABLE [HRVacancyRecruiter] (
    [Id] int NOT NULL IDENTITY,
    [VacancyId] int NOT NULL,
    [EmployeeId] int NOT NULL,
    [AssignedBy] nvarchar(100) NULL,
    [AssignedAtUtc] datetime2 NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_HRVacancyRecruiter] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[HRVacancyRecruiter]', N'Id') IS NULL ALTER TABLE [HRVacancyRecruiter] ADD [Id] int NULL ;
GO
IF COL_LENGTH(N'[HRVacancyRecruiter]', N'VacancyId') IS NULL ALTER TABLE [HRVacancyRecruiter] ADD [VacancyId] int NULL;
GO
IF COL_LENGTH(N'[HRVacancyRecruiter]', N'EmployeeId') IS NULL ALTER TABLE [HRVacancyRecruiter] ADD [EmployeeId] int NULL;
GO
IF COL_LENGTH(N'[HRVacancyRecruiter]', N'AssignedBy') IS NULL ALTER TABLE [HRVacancyRecruiter] ADD [AssignedBy] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[HRVacancyRecruiter]', N'AssignedAtUtc') IS NULL ALTER TABLE [HRVacancyRecruiter] ADD [AssignedAtUtc] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRVacancyRecruiter]', N'IsActive') IS NULL ALTER TABLE [HRVacancyRecruiter] ADD [IsActive] bit NULL;
GO
IF OBJECT_ID(N'[orgsubcordinate]', N'U') IS NULL BEGIN CREATE TABLE [orgsubcordinate] (
    [id] int NOT NULL,
    [empid] int NULL,
    [subcampus] int NULL,
    [fromdate] datetime2 NULL,
    [tilldate] datetime2 NULL,
    [remarks] nvarchar(500) NULL,
    [utsysgen] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] nvarchar(100) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] nvarchar(50) NULL,
    [utrecordversion] int NOT NULL,
    [utblock] bit NULL,
    [utaddappversion] nvarchar(100) NULL,
    [utadddbversion] nvarchar(100) NULL,
    [utaddid] bigint NULL,
    [utaddip] nvarchar(100) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] nvarchar(100) NULL,
    [utdoctype] int NULL,
    [uteditappversion] nvarchar(50) NULL,
    [uteditdbversion] nvarchar(50) NULL,
    [uteditid] bigint NULL,
    [uteditip] nvarchar(50) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] nvarchar(50) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_orgsubcordinate] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[orgsubcordinate]', N'id') IS NULL ALTER TABLE [orgsubcordinate] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[orgsubcordinate]', N'empid') IS NULL ALTER TABLE [orgsubcordinate] ADD [empid] int NULL;
GO
IF COL_LENGTH(N'[orgsubcordinate]', N'subcampus') IS NULL ALTER TABLE [orgsubcordinate] ADD [subcampus] int NULL;
GO
IF COL_LENGTH(N'[orgsubcordinate]', N'fromdate') IS NULL ALTER TABLE [orgsubcordinate] ADD [fromdate] datetime2 NULL;
GO
IF COL_LENGTH(N'[orgsubcordinate]', N'tilldate') IS NULL ALTER TABLE [orgsubcordinate] ADD [tilldate] datetime2 NULL;
GO
IF COL_LENGTH(N'[orgsubcordinate]', N'remarks') IS NULL ALTER TABLE [orgsubcordinate] ADD [remarks] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[orgsubcordinate]', N'utsysgen') IS NULL ALTER TABLE [orgsubcordinate] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[orgsubcordinate]', N'utispartial') IS NULL ALTER TABLE [orgsubcordinate] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[orgsubcordinate]', N'utaddtime') IS NULL ALTER TABLE [orgsubcordinate] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[orgsubcordinate]', N'utadduser') IS NULL ALTER TABLE [orgsubcordinate] ADD [utadduser] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[orgsubcordinate]', N'utedittime') IS NULL ALTER TABLE [orgsubcordinate] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[orgsubcordinate]', N'utedituser') IS NULL ALTER TABLE [orgsubcordinate] ADD [utedituser] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[orgsubcordinate]', N'utrecordversion') IS NULL ALTER TABLE [orgsubcordinate] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[orgsubcordinate]', N'utblock') IS NULL ALTER TABLE [orgsubcordinate] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[orgsubcordinate]', N'utaddappversion') IS NULL ALTER TABLE [orgsubcordinate] ADD [utaddappversion] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[orgsubcordinate]', N'utadddbversion') IS NULL ALTER TABLE [orgsubcordinate] ADD [utadddbversion] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[orgsubcordinate]', N'utaddid') IS NULL ALTER TABLE [orgsubcordinate] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[orgsubcordinate]', N'utaddip') IS NULL ALTER TABLE [orgsubcordinate] ADD [utaddip] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[orgsubcordinate]', N'utaddsessionid') IS NULL ALTER TABLE [orgsubcordinate] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[orgsubcordinate]', N'utaddsystem') IS NULL ALTER TABLE [orgsubcordinate] ADD [utaddsystem] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[orgsubcordinate]', N'utdoctype') IS NULL ALTER TABLE [orgsubcordinate] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[orgsubcordinate]', N'uteditappversion') IS NULL ALTER TABLE [orgsubcordinate] ADD [uteditappversion] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[orgsubcordinate]', N'uteditdbversion') IS NULL ALTER TABLE [orgsubcordinate] ADD [uteditdbversion] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[orgsubcordinate]', N'uteditid') IS NULL ALTER TABLE [orgsubcordinate] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[orgsubcordinate]', N'uteditip') IS NULL ALTER TABLE [orgsubcordinate] ADD [uteditip] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[orgsubcordinate]', N'uteditsessionid') IS NULL ALTER TABLE [orgsubcordinate] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[orgsubcordinate]', N'uteditsystem') IS NULL ALTER TABLE [orgsubcordinate] ADD [uteditsystem] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[orgsubcordinate]', N'utsubdoctype') IS NULL ALTER TABLE [orgsubcordinate] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[SMAcademic]', N'U') IS NULL BEGIN CREATE TABLE [SMAcademic] (
    [Id] int NOT NULL,
    [studentid] int NULL,
    [employeeid] int NULL,
    [qualificationtypeid] int NULL,
    [qualificationid] int NULL,
    [insitute] nvarchar(50) NULL,
    [year] int NULL,
    [grade] nvarchar(3) NULL,
    [percentage] float NULL,
    [totalmarks] float NULL,
    [obtainmarks] int NULL,
    [division] nvarchar(3) NULL,
    [utsysgen] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utrecordversion] int NOT NULL,
    [utblock] bit NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_SMAcademic] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[SMAcademic]', N'Id') IS NULL ALTER TABLE [SMAcademic] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[SMAcademic]', N'studentid') IS NULL ALTER TABLE [SMAcademic] ADD [studentid] int NULL;
GO
IF COL_LENGTH(N'[SMAcademic]', N'employeeid') IS NULL ALTER TABLE [SMAcademic] ADD [employeeid] int NULL;
GO
IF COL_LENGTH(N'[SMAcademic]', N'qualificationtypeid') IS NULL ALTER TABLE [SMAcademic] ADD [qualificationtypeid] int NULL;
GO
IF COL_LENGTH(N'[SMAcademic]', N'qualificationid') IS NULL ALTER TABLE [SMAcademic] ADD [qualificationid] int NULL;
GO
IF COL_LENGTH(N'[SMAcademic]', N'insitute') IS NULL ALTER TABLE [SMAcademic] ADD [insitute] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[SMAcademic]', N'year') IS NULL ALTER TABLE [SMAcademic] ADD [year] int NULL;
GO
IF COL_LENGTH(N'[SMAcademic]', N'grade') IS NULL ALTER TABLE [SMAcademic] ADD [grade] nvarchar(3) NULL;
GO
IF COL_LENGTH(N'[SMAcademic]', N'percentage') IS NULL ALTER TABLE [SMAcademic] ADD [percentage] float NULL;
GO
IF COL_LENGTH(N'[SMAcademic]', N'totalmarks') IS NULL ALTER TABLE [SMAcademic] ADD [totalmarks] float NULL;
GO
IF COL_LENGTH(N'[SMAcademic]', N'obtainmarks') IS NULL ALTER TABLE [SMAcademic] ADD [obtainmarks] int NULL;
GO
IF COL_LENGTH(N'[SMAcademic]', N'division') IS NULL ALTER TABLE [SMAcademic] ADD [division] nvarchar(3) NULL;
GO
IF COL_LENGTH(N'[SMAcademic]', N'utsysgen') IS NULL ALTER TABLE [SMAcademic] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[SMAcademic]', N'utispartial') IS NULL ALTER TABLE [SMAcademic] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[SMAcademic]', N'utaddtime') IS NULL ALTER TABLE [SMAcademic] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMAcademic]', N'utadduser') IS NULL ALTER TABLE [SMAcademic] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMAcademic]', N'utedittime') IS NULL ALTER TABLE [SMAcademic] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMAcademic]', N'utedituser') IS NULL ALTER TABLE [SMAcademic] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMAcademic]', N'utrecordversion') IS NULL ALTER TABLE [SMAcademic] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[SMAcademic]', N'utblock') IS NULL ALTER TABLE [SMAcademic] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[SMAcademic]', N'utaddappversion') IS NULL ALTER TABLE [SMAcademic] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMAcademic]', N'utadddbversion') IS NULL ALTER TABLE [SMAcademic] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMAcademic]', N'utaddid') IS NULL ALTER TABLE [SMAcademic] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[SMAcademic]', N'utaddip') IS NULL ALTER TABLE [SMAcademic] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMAcademic]', N'utaddsessionid') IS NULL ALTER TABLE [SMAcademic] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[SMAcademic]', N'utaddsystem') IS NULL ALTER TABLE [SMAcademic] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMAcademic]', N'utdoctype') IS NULL ALTER TABLE [SMAcademic] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[SMAcademic]', N'uteditappversion') IS NULL ALTER TABLE [SMAcademic] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMAcademic]', N'uteditdbversion') IS NULL ALTER TABLE [SMAcademic] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMAcademic]', N'uteditid') IS NULL ALTER TABLE [SMAcademic] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[SMAcademic]', N'uteditip') IS NULL ALTER TABLE [SMAcademic] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMAcademic]', N'uteditsessionid') IS NULL ALTER TABLE [SMAcademic] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[SMAcademic]', N'uteditsystem') IS NULL ALTER TABLE [SMAcademic] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMAcademic]', N'utsubdoctype') IS NULL ALTER TABLE [SMAcademic] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[HRPositionAssignment]', N'U') IS NULL BEGIN CREATE TABLE [HRPositionAssignment] (
    [Id] int NOT NULL IDENTITY,
    [PositionId] int NOT NULL,
    [EmployeeId] int NOT NULL,
    [AssignmentType] nvarchar(30) NULL,
    [LovStatusId] int NOT NULL,
    [EffectiveFrom] datetime2 NOT NULL,
    [EffectiveTo] datetime2 NULL,
    [Remarks] nvarchar(1500) NULL,
    [UtDocType] int NOT NULL,
    [UtSubDocType] int NOT NULL,
    [UtSysGen] bit NULL,
    [UtBlock] bit NULL,
    [UtIsPartial] bit NULL,
    [UtAddTime] datetime2 NOT NULL,
    [UtAddUser] nvarchar(25) NULL,
    [UtAddSystem] nvarchar(25) NULL,
    [UtAddIp] nvarchar(25) NULL,
    [UtAddAppVersion] nvarchar(10) NULL,
    [UtAddDbVersion] nvarchar(10) NULL,
    [UtAddId] bigint NOT NULL,
    [UtAddSessionId] int NOT NULL,
    [UtEditTime] datetime2 NOT NULL,
    [UtEditUser] nvarchar(25) NULL,
    [UtEditSystem] nvarchar(25) NULL,
    [UtEditIp] nvarchar(25) NULL,
    [UtEditAppVersion] nvarchar(10) NULL,
    [UtEditDbVersion] nvarchar(10) NULL,
    [UtEditId] bigint NOT NULL,
    [UtEditSessionId] int NOT NULL,
    [UtRecordVersion] int NULL,
    CONSTRAINT [PK_HRPositionAssignment] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[HRPositionAssignment]', N'Id') IS NULL ALTER TABLE [HRPositionAssignment] ADD [Id] int NULL ;
GO
IF COL_LENGTH(N'[HRPositionAssignment]', N'PositionId') IS NULL ALTER TABLE [HRPositionAssignment] ADD [PositionId] int NULL;
GO
IF COL_LENGTH(N'[HRPositionAssignment]', N'EmployeeId') IS NULL ALTER TABLE [HRPositionAssignment] ADD [EmployeeId] int NULL;
GO
IF COL_LENGTH(N'[HRPositionAssignment]', N'AssignmentType') IS NULL ALTER TABLE [HRPositionAssignment] ADD [AssignmentType] nvarchar(30) NULL;
GO
IF COL_LENGTH(N'[HRPositionAssignment]', N'LovStatusId') IS NULL ALTER TABLE [HRPositionAssignment] ADD [LovStatusId] int NULL;
GO
IF COL_LENGTH(N'[HRPositionAssignment]', N'EffectiveFrom') IS NULL ALTER TABLE [HRPositionAssignment] ADD [EffectiveFrom] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRPositionAssignment]', N'EffectiveTo') IS NULL ALTER TABLE [HRPositionAssignment] ADD [EffectiveTo] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRPositionAssignment]', N'Remarks') IS NULL ALTER TABLE [HRPositionAssignment] ADD [Remarks] nvarchar(1500) NULL;
GO
IF COL_LENGTH(N'[HRPositionAssignment]', N'UtDocType') IS NULL ALTER TABLE [HRPositionAssignment] ADD [UtDocType] int NULL;
GO
IF COL_LENGTH(N'[HRPositionAssignment]', N'UtSubDocType') IS NULL ALTER TABLE [HRPositionAssignment] ADD [UtSubDocType] int NULL;
GO
IF COL_LENGTH(N'[HRPositionAssignment]', N'UtSysGen') IS NULL ALTER TABLE [HRPositionAssignment] ADD [UtSysGen] bit NULL;
GO
IF COL_LENGTH(N'[HRPositionAssignment]', N'UtBlock') IS NULL ALTER TABLE [HRPositionAssignment] ADD [UtBlock] bit NULL;
GO
IF COL_LENGTH(N'[HRPositionAssignment]', N'UtIsPartial') IS NULL ALTER TABLE [HRPositionAssignment] ADD [UtIsPartial] bit NULL;
GO
IF COL_LENGTH(N'[HRPositionAssignment]', N'UtAddTime') IS NULL ALTER TABLE [HRPositionAssignment] ADD [UtAddTime] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRPositionAssignment]', N'UtAddUser') IS NULL ALTER TABLE [HRPositionAssignment] ADD [UtAddUser] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[HRPositionAssignment]', N'UtAddSystem') IS NULL ALTER TABLE [HRPositionAssignment] ADD [UtAddSystem] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[HRPositionAssignment]', N'UtAddIp') IS NULL ALTER TABLE [HRPositionAssignment] ADD [UtAddIp] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[HRPositionAssignment]', N'UtAddAppVersion') IS NULL ALTER TABLE [HRPositionAssignment] ADD [UtAddAppVersion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[HRPositionAssignment]', N'UtAddDbVersion') IS NULL ALTER TABLE [HRPositionAssignment] ADD [UtAddDbVersion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[HRPositionAssignment]', N'UtAddId') IS NULL ALTER TABLE [HRPositionAssignment] ADD [UtAddId] bigint NULL;
GO
IF COL_LENGTH(N'[HRPositionAssignment]', N'UtAddSessionId') IS NULL ALTER TABLE [HRPositionAssignment] ADD [UtAddSessionId] int NULL;
GO
IF COL_LENGTH(N'[HRPositionAssignment]', N'UtEditTime') IS NULL ALTER TABLE [HRPositionAssignment] ADD [UtEditTime] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRPositionAssignment]', N'UtEditUser') IS NULL ALTER TABLE [HRPositionAssignment] ADD [UtEditUser] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[HRPositionAssignment]', N'UtEditSystem') IS NULL ALTER TABLE [HRPositionAssignment] ADD [UtEditSystem] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[HRPositionAssignment]', N'UtEditIp') IS NULL ALTER TABLE [HRPositionAssignment] ADD [UtEditIp] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[HRPositionAssignment]', N'UtEditAppVersion') IS NULL ALTER TABLE [HRPositionAssignment] ADD [UtEditAppVersion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[HRPositionAssignment]', N'UtEditDbVersion') IS NULL ALTER TABLE [HRPositionAssignment] ADD [UtEditDbVersion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[HRPositionAssignment]', N'UtEditId') IS NULL ALTER TABLE [HRPositionAssignment] ADD [UtEditId] bigint NULL;
GO
IF COL_LENGTH(N'[HRPositionAssignment]', N'UtEditSessionId') IS NULL ALTER TABLE [HRPositionAssignment] ADD [UtEditSessionId] int NULL;
GO
IF COL_LENGTH(N'[HRPositionAssignment]', N'UtRecordVersion') IS NULL ALTER TABLE [HRPositionAssignment] ADD [UtRecordVersion] int NULL;
GO
IF OBJECT_ID(N'[HRPositionAudit]', N'U') IS NULL BEGIN CREATE TABLE [HRPositionAudit] (
    [Id] bigint NOT NULL IDENTITY,
    [ClientId] int NOT NULL,
    [EntityId] int NULL,
    [PositionId] int NOT NULL,
    [Action] nvarchar(50) NULL,
    [Actor] nvarchar(100) NULL,
    [ActorEmployeeId] int NULL,
    [Remarks] nvarchar(1500) NULL,
    [BeforeJson] nvarchar(max) NULL,
    [AfterJson] nvarchar(max) NULL,
    [OccurredAtUtc] datetime2 NOT NULL,
    [UtDocType] int NOT NULL,
    [UtSubDocType] int NOT NULL,
    [UtSysGen] bit NULL,
    [UtBlock] bit NULL,
    [UtIsPartial] bit NULL,
    [UtAddTime] datetime2 NOT NULL,
    [UtAddUser] nvarchar(25) NULL,
    [UtAddSystem] nvarchar(25) NULL,
    [UtAddIp] nvarchar(25) NULL,
    [UtAddAppVersion] nvarchar(10) NULL,
    [UtAddDbVersion] nvarchar(10) NULL,
    [UtAddId] bigint NOT NULL,
    [UtAddSessionId] int NOT NULL,
    [UtEditTime] datetime2 NOT NULL,
    [UtEditUser] nvarchar(25) NULL,
    [UtEditSystem] nvarchar(25) NULL,
    [UtEditIp] nvarchar(25) NULL,
    [UtEditAppVersion] nvarchar(10) NULL,
    [UtEditDbVersion] nvarchar(10) NULL,
    [UtEditId] bigint NOT NULL,
    [UtEditSessionId] int NOT NULL,
    [UtRecordVersion] int NULL,
    CONSTRAINT [PK_HRPositionAudit] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[HRPositionAudit]', N'Id') IS NULL ALTER TABLE [HRPositionAudit] ADD [Id] bigint NULL ;
GO
IF COL_LENGTH(N'[HRPositionAudit]', N'ClientId') IS NULL ALTER TABLE [HRPositionAudit] ADD [ClientId] int NULL;
GO
IF COL_LENGTH(N'[HRPositionAudit]', N'EntityId') IS NULL ALTER TABLE [HRPositionAudit] ADD [EntityId] int NULL;
GO
IF COL_LENGTH(N'[HRPositionAudit]', N'PositionId') IS NULL ALTER TABLE [HRPositionAudit] ADD [PositionId] int NULL;
GO
IF COL_LENGTH(N'[HRPositionAudit]', N'Action') IS NULL ALTER TABLE [HRPositionAudit] ADD [Action] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[HRPositionAudit]', N'Actor') IS NULL ALTER TABLE [HRPositionAudit] ADD [Actor] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[HRPositionAudit]', N'ActorEmployeeId') IS NULL ALTER TABLE [HRPositionAudit] ADD [ActorEmployeeId] int NULL;
GO
IF COL_LENGTH(N'[HRPositionAudit]', N'Remarks') IS NULL ALTER TABLE [HRPositionAudit] ADD [Remarks] nvarchar(1500) NULL;
GO
IF COL_LENGTH(N'[HRPositionAudit]', N'BeforeJson') IS NULL ALTER TABLE [HRPositionAudit] ADD [BeforeJson] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[HRPositionAudit]', N'AfterJson') IS NULL ALTER TABLE [HRPositionAudit] ADD [AfterJson] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[HRPositionAudit]', N'OccurredAtUtc') IS NULL ALTER TABLE [HRPositionAudit] ADD [OccurredAtUtc] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRPositionAudit]', N'UtDocType') IS NULL ALTER TABLE [HRPositionAudit] ADD [UtDocType] int NULL;
GO
IF COL_LENGTH(N'[HRPositionAudit]', N'UtSubDocType') IS NULL ALTER TABLE [HRPositionAudit] ADD [UtSubDocType] int NULL;
GO
IF COL_LENGTH(N'[HRPositionAudit]', N'UtSysGen') IS NULL ALTER TABLE [HRPositionAudit] ADD [UtSysGen] bit NULL;
GO
IF COL_LENGTH(N'[HRPositionAudit]', N'UtBlock') IS NULL ALTER TABLE [HRPositionAudit] ADD [UtBlock] bit NULL;
GO
IF COL_LENGTH(N'[HRPositionAudit]', N'UtIsPartial') IS NULL ALTER TABLE [HRPositionAudit] ADD [UtIsPartial] bit NULL;
GO
IF COL_LENGTH(N'[HRPositionAudit]', N'UtAddTime') IS NULL ALTER TABLE [HRPositionAudit] ADD [UtAddTime] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRPositionAudit]', N'UtAddUser') IS NULL ALTER TABLE [HRPositionAudit] ADD [UtAddUser] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[HRPositionAudit]', N'UtAddSystem') IS NULL ALTER TABLE [HRPositionAudit] ADD [UtAddSystem] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[HRPositionAudit]', N'UtAddIp') IS NULL ALTER TABLE [HRPositionAudit] ADD [UtAddIp] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[HRPositionAudit]', N'UtAddAppVersion') IS NULL ALTER TABLE [HRPositionAudit] ADD [UtAddAppVersion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[HRPositionAudit]', N'UtAddDbVersion') IS NULL ALTER TABLE [HRPositionAudit] ADD [UtAddDbVersion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[HRPositionAudit]', N'UtAddId') IS NULL ALTER TABLE [HRPositionAudit] ADD [UtAddId] bigint NULL;
GO
IF COL_LENGTH(N'[HRPositionAudit]', N'UtAddSessionId') IS NULL ALTER TABLE [HRPositionAudit] ADD [UtAddSessionId] int NULL;
GO
IF COL_LENGTH(N'[HRPositionAudit]', N'UtEditTime') IS NULL ALTER TABLE [HRPositionAudit] ADD [UtEditTime] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRPositionAudit]', N'UtEditUser') IS NULL ALTER TABLE [HRPositionAudit] ADD [UtEditUser] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[HRPositionAudit]', N'UtEditSystem') IS NULL ALTER TABLE [HRPositionAudit] ADD [UtEditSystem] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[HRPositionAudit]', N'UtEditIp') IS NULL ALTER TABLE [HRPositionAudit] ADD [UtEditIp] nvarchar(25) NULL;
GO
IF COL_LENGTH(N'[HRPositionAudit]', N'UtEditAppVersion') IS NULL ALTER TABLE [HRPositionAudit] ADD [UtEditAppVersion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[HRPositionAudit]', N'UtEditDbVersion') IS NULL ALTER TABLE [HRPositionAudit] ADD [UtEditDbVersion] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[HRPositionAudit]', N'UtEditId') IS NULL ALTER TABLE [HRPositionAudit] ADD [UtEditId] bigint NULL;
GO
IF COL_LENGTH(N'[HRPositionAudit]', N'UtEditSessionId') IS NULL ALTER TABLE [HRPositionAudit] ADD [UtEditSessionId] int NULL;
GO
IF COL_LENGTH(N'[HRPositionAudit]', N'UtRecordVersion') IS NULL ALTER TABLE [HRPositionAudit] ADD [UtRecordVersion] int NULL;
GO
IF OBJECT_ID(N'[HRVacancy]', N'U') IS NULL BEGIN CREATE TABLE [HRVacancy] (
    [Id] int NOT NULL IDENTITY,
    [ClientId] int NOT NULL,
    [EntityId] int NOT NULL,
    [VacancyType] nvarchar(20) NULL,
    [ReportsToType] nvarchar(20) NULL,
    [ReportsToPositionId] int NULL,
    [ReplacementPositionId] int NULL,
    [VacancyNumber] nvarchar(30) NULL,
    [JobTitle] nvarchar(200) NULL,
    [DepartmentId] int NOT NULL,
    [DesignationId] int NOT NULL,
    [GradeId] int NULL,
    [SiteId] int NULL,
    [EmploymentTypeId] int NULL,
    [HiringManagerEmployeeId] int NULL,
    [ApproverEmployeeId] int NULL,
    [Priority] nvarchar(20) NULL,
    [RequestedOpenings] int NOT NULL,
    [TargetFillDate] datetime2 NULL,
    [MinimumExperienceYears] decimal(5,2) NULL,
    [MaximumExperienceYears] decimal(5,2) NULL,
    [JobDescription] nvarchar(4000) NULL,
    [Responsibilities] nvarchar(4000) NULL,
    [Requirements] nvarchar(4000) NULL,
    [RequiredQualifications] nvarchar(4000) NULL,
    [SalaryMinimum] decimal(18,2) NULL,
    [SalaryMaximum] decimal(18,2) NULL,
    [CurrencyCode] nvarchar(10) NULL,
    [HiringBudget] decimal(18,2) NULL,
    [ApprovalStatus] nvarchar(20) NULL,
    [LifecycleStatus] nvarchar(20) NULL,
    [RevisionNumber] int NOT NULL,
    [JobPortalJobId] nvarchar(100) NULL,
    [JobPortalPostedAtUtc] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    [CreatedBy] nvarchar(100) NULL,
    [CreatedAtUtc] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(100) NULL,
    [UpdatedAtUtc] datetime2 NULL,
    [RowVersion] rowversion NOT NULL,
    [utdoctype] int NOT NULL,
    [utsubdoctype] int NOT NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utaddtime] datetime2 NOT NULL,
    [utadduser] nvarchar(100) NULL,
    [utaddsystem] nvarchar(100) NULL,
    [utaddip] nvarchar(100) NULL,
    [utaddappversion] nvarchar(100) NULL,
    [utadddbversion] nvarchar(100) NULL,
    [utaddid] bigint NOT NULL,
    [utaddsessionid] int NULL,
    [utedittime] datetime2 NOT NULL,
    [utedituser] nvarchar(50) NULL,
    [uteditsystem] nvarchar(50) NULL,
    [uteditip] nvarchar(50) NULL,
    [uteditappversion] nvarchar(50) NULL,
    [uteditdbversion] nvarchar(50) NULL,
    [uteditid] bigint NOT NULL,
    [uteditsessionid] int NULL,
    [utrecordversion] int NOT NULL,
    CONSTRAINT [PK_HRVacancy] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[HRVacancy]', N'Id') IS NULL ALTER TABLE [HRVacancy] ADD [Id] int NULL ;
GO
IF COL_LENGTH(N'[HRVacancy]', N'ClientId') IS NULL ALTER TABLE [HRVacancy] ADD [ClientId] int NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'EntityId') IS NULL ALTER TABLE [HRVacancy] ADD [EntityId] int NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'VacancyType') IS NULL ALTER TABLE [HRVacancy] ADD [VacancyType] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'ReportsToType') IS NULL ALTER TABLE [HRVacancy] ADD [ReportsToType] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'ReportsToPositionId') IS NULL ALTER TABLE [HRVacancy] ADD [ReportsToPositionId] int NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'ReplacementPositionId') IS NULL ALTER TABLE [HRVacancy] ADD [ReplacementPositionId] int NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'VacancyNumber') IS NULL ALTER TABLE [HRVacancy] ADD [VacancyNumber] nvarchar(30) NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'JobTitle') IS NULL ALTER TABLE [HRVacancy] ADD [JobTitle] nvarchar(200) NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'DepartmentId') IS NULL ALTER TABLE [HRVacancy] ADD [DepartmentId] int NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'DesignationId') IS NULL ALTER TABLE [HRVacancy] ADD [DesignationId] int NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'GradeId') IS NULL ALTER TABLE [HRVacancy] ADD [GradeId] int NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'SiteId') IS NULL ALTER TABLE [HRVacancy] ADD [SiteId] int NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'EmploymentTypeId') IS NULL ALTER TABLE [HRVacancy] ADD [EmploymentTypeId] int NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'HiringManagerEmployeeId') IS NULL ALTER TABLE [HRVacancy] ADD [HiringManagerEmployeeId] int NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'ApproverEmployeeId') IS NULL ALTER TABLE [HRVacancy] ADD [ApproverEmployeeId] int NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'Priority') IS NULL ALTER TABLE [HRVacancy] ADD [Priority] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'RequestedOpenings') IS NULL ALTER TABLE [HRVacancy] ADD [RequestedOpenings] int NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'TargetFillDate') IS NULL ALTER TABLE [HRVacancy] ADD [TargetFillDate] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'MinimumExperienceYears') IS NULL ALTER TABLE [HRVacancy] ADD [MinimumExperienceYears] decimal(5,2) NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'MaximumExperienceYears') IS NULL ALTER TABLE [HRVacancy] ADD [MaximumExperienceYears] decimal(5,2) NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'JobDescription') IS NULL ALTER TABLE [HRVacancy] ADD [JobDescription] nvarchar(4000) NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'Responsibilities') IS NULL ALTER TABLE [HRVacancy] ADD [Responsibilities] nvarchar(4000) NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'Requirements') IS NULL ALTER TABLE [HRVacancy] ADD [Requirements] nvarchar(4000) NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'RequiredQualifications') IS NULL ALTER TABLE [HRVacancy] ADD [RequiredQualifications] nvarchar(4000) NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'SalaryMinimum') IS NULL ALTER TABLE [HRVacancy] ADD [SalaryMinimum] decimal(18,2) NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'SalaryMaximum') IS NULL ALTER TABLE [HRVacancy] ADD [SalaryMaximum] decimal(18,2) NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'CurrencyCode') IS NULL ALTER TABLE [HRVacancy] ADD [CurrencyCode] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'HiringBudget') IS NULL ALTER TABLE [HRVacancy] ADD [HiringBudget] decimal(18,2) NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'ApprovalStatus') IS NULL ALTER TABLE [HRVacancy] ADD [ApprovalStatus] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'LifecycleStatus') IS NULL ALTER TABLE [HRVacancy] ADD [LifecycleStatus] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'RevisionNumber') IS NULL ALTER TABLE [HRVacancy] ADD [RevisionNumber] int NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'JobPortalJobId') IS NULL ALTER TABLE [HRVacancy] ADD [JobPortalJobId] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'JobPortalPostedAtUtc') IS NULL ALTER TABLE [HRVacancy] ADD [JobPortalPostedAtUtc] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'IsDeleted') IS NULL ALTER TABLE [HRVacancy] ADD [IsDeleted] bit NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'CreatedBy') IS NULL ALTER TABLE [HRVacancy] ADD [CreatedBy] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'CreatedAtUtc') IS NULL ALTER TABLE [HRVacancy] ADD [CreatedAtUtc] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'UpdatedBy') IS NULL ALTER TABLE [HRVacancy] ADD [UpdatedBy] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'UpdatedAtUtc') IS NULL ALTER TABLE [HRVacancy] ADD [UpdatedAtUtc] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'RowVersion') IS NULL ALTER TABLE [HRVacancy] ADD [RowVersion] rowversion NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'utdoctype') IS NULL ALTER TABLE [HRVacancy] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'utsubdoctype') IS NULL ALTER TABLE [HRVacancy] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'utsysgen') IS NULL ALTER TABLE [HRVacancy] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'utblock') IS NULL ALTER TABLE [HRVacancy] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'utispartial') IS NULL ALTER TABLE [HRVacancy] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'utaddtime') IS NULL ALTER TABLE [HRVacancy] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'utadduser') IS NULL ALTER TABLE [HRVacancy] ADD [utadduser] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'utaddsystem') IS NULL ALTER TABLE [HRVacancy] ADD [utaddsystem] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'utaddip') IS NULL ALTER TABLE [HRVacancy] ADD [utaddip] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'utaddappversion') IS NULL ALTER TABLE [HRVacancy] ADD [utaddappversion] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'utadddbversion') IS NULL ALTER TABLE [HRVacancy] ADD [utadddbversion] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'utaddid') IS NULL ALTER TABLE [HRVacancy] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'utaddsessionid') IS NULL ALTER TABLE [HRVacancy] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'utedittime') IS NULL ALTER TABLE [HRVacancy] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'utedituser') IS NULL ALTER TABLE [HRVacancy] ADD [utedituser] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'uteditsystem') IS NULL ALTER TABLE [HRVacancy] ADD [uteditsystem] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'uteditip') IS NULL ALTER TABLE [HRVacancy] ADD [uteditip] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'uteditappversion') IS NULL ALTER TABLE [HRVacancy] ADD [uteditappversion] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'uteditdbversion') IS NULL ALTER TABLE [HRVacancy] ADD [uteditdbversion] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'uteditid') IS NULL ALTER TABLE [HRVacancy] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'uteditsessionid') IS NULL ALTER TABLE [HRVacancy] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[HRVacancy]', N'utrecordversion') IS NULL ALTER TABLE [HRVacancy] ADD [utrecordversion] int NULL;
GO
IF OBJECT_ID(N'[EPLoantranshdr]', N'U') IS NULL BEGIN CREATE TABLE [EPLoantranshdr] (
    [id] int NOT NULL,
    [loanno] nvarchar(20) NULL,
    [loandate] datetime2 NULL,
    [postingdate] datetime2 NULL,
    [typeid] int NULL,
    [requestid] int NULL,
    [employeeid] int NULL,
    [loantypeid] int NULL,
    [appliedamount] float NULL,
    [loanamount] float NULL,
    [instalamt] float NULL,
    [noofinstal] int NULL,
    [recoveryyear] int NULL,
    [recoverymonth] int NULL,
    [dedtypeid] int NULL,
    [payperiodid] int NULL,
    [perioddtlid] int NULL,
    [reason] nvarchar(1500) NULL,
    [statusid] int NULL,
    [TrusteeStatusId] int NOT NULL,
    [utdoctype] int NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    CONSTRAINT [PK_EPLoantranshdr] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'id') IS NULL ALTER TABLE [EPLoantranshdr] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'loanno') IS NULL ALTER TABLE [EPLoantranshdr] ADD [loanno] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'loandate') IS NULL ALTER TABLE [EPLoantranshdr] ADD [loandate] datetime2 NULL;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'postingdate') IS NULL ALTER TABLE [EPLoantranshdr] ADD [postingdate] datetime2 NULL;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'typeid') IS NULL ALTER TABLE [EPLoantranshdr] ADD [typeid] int NULL;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'requestid') IS NULL ALTER TABLE [EPLoantranshdr] ADD [requestid] int NULL;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'employeeid') IS NULL ALTER TABLE [EPLoantranshdr] ADD [employeeid] int NULL;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'loantypeid') IS NULL ALTER TABLE [EPLoantranshdr] ADD [loantypeid] int NULL;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'appliedamount') IS NULL ALTER TABLE [EPLoantranshdr] ADD [appliedamount] float NULL;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'loanamount') IS NULL ALTER TABLE [EPLoantranshdr] ADD [loanamount] float NULL;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'instalamt') IS NULL ALTER TABLE [EPLoantranshdr] ADD [instalamt] float NULL;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'noofinstal') IS NULL ALTER TABLE [EPLoantranshdr] ADD [noofinstal] int NULL;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'recoveryyear') IS NULL ALTER TABLE [EPLoantranshdr] ADD [recoveryyear] int NULL;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'recoverymonth') IS NULL ALTER TABLE [EPLoantranshdr] ADD [recoverymonth] int NULL;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'dedtypeid') IS NULL ALTER TABLE [EPLoantranshdr] ADD [dedtypeid] int NULL;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'payperiodid') IS NULL ALTER TABLE [EPLoantranshdr] ADD [payperiodid] int NULL;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'perioddtlid') IS NULL ALTER TABLE [EPLoantranshdr] ADD [perioddtlid] int NULL;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'reason') IS NULL ALTER TABLE [EPLoantranshdr] ADD [reason] nvarchar(1500) NULL;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'statusid') IS NULL ALTER TABLE [EPLoantranshdr] ADD [statusid] int NULL;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'TrusteeStatusId') IS NULL ALTER TABLE [EPLoantranshdr] ADD [TrusteeStatusId] int NULL;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'utdoctype') IS NULL ALTER TABLE [EPLoantranshdr] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'utsubdoctype') IS NULL ALTER TABLE [EPLoantranshdr] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'utsysgen') IS NULL ALTER TABLE [EPLoantranshdr] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'utblock') IS NULL ALTER TABLE [EPLoantranshdr] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'utispartial') IS NULL ALTER TABLE [EPLoantranshdr] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'utrecordversion') IS NULL ALTER TABLE [EPLoantranshdr] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'utaddtime') IS NULL ALTER TABLE [EPLoantranshdr] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'utadduser') IS NULL ALTER TABLE [EPLoantranshdr] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'utaddappversion') IS NULL ALTER TABLE [EPLoantranshdr] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'utadddbversion') IS NULL ALTER TABLE [EPLoantranshdr] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'utaddid') IS NULL ALTER TABLE [EPLoantranshdr] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'utaddip') IS NULL ALTER TABLE [EPLoantranshdr] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'utaddsessionid') IS NULL ALTER TABLE [EPLoantranshdr] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'utaddsystem') IS NULL ALTER TABLE [EPLoantranshdr] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'uteditappversion') IS NULL ALTER TABLE [EPLoantranshdr] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'uteditdbversion') IS NULL ALTER TABLE [EPLoantranshdr] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'uteditid') IS NULL ALTER TABLE [EPLoantranshdr] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'uteditip') IS NULL ALTER TABLE [EPLoantranshdr] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'uteditsessionid') IS NULL ALTER TABLE [EPLoantranshdr] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'uteditsystem') IS NULL ALTER TABLE [EPLoantranshdr] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'utedittime') IS NULL ALTER TABLE [EPLoantranshdr] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EPLoantranshdr]', N'utedituser') IS NULL ALTER TABLE [EPLoantranshdr] ADD [utedituser] varchar(25) NULL;
GO
IF OBJECT_ID(N'[EREmppayallowancedtl]', N'U') IS NULL BEGIN CREATE TABLE [EREmppayallowancedtl] (
    [Id] int NOT NULL,
    [earntypeid] int NULL,
    [ispercentage] bit NOT NULL DEFAULT CAST(0 AS bit),
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    [emppayrollid] int NULL,
    [lineindex] int NULL,
    [value] float NULL,
    [startdate] datetime2 NULL,
    [enddate] datetime2 NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    CONSTRAINT [PK_EREmppayallowancedtl] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[EREmppayallowancedtl]', N'Id') IS NULL ALTER TABLE [EREmppayallowancedtl] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[EREmppayallowancedtl]', N'earntypeid') IS NULL ALTER TABLE [EREmppayallowancedtl] ADD [earntypeid] int NULL;
GO
IF COL_LENGTH(N'[EREmppayallowancedtl]', N'ispercentage') IS NULL ALTER TABLE [EREmppayallowancedtl] ADD [ispercentage] bit NULL DEFAULT CAST(0 AS bit);
GO
IF COL_LENGTH(N'[EREmppayallowancedtl]', N'utaddappversion') IS NULL ALTER TABLE [EREmppayallowancedtl] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmppayallowancedtl]', N'utadddbversion') IS NULL ALTER TABLE [EREmppayallowancedtl] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmppayallowancedtl]', N'utaddid') IS NULL ALTER TABLE [EREmppayallowancedtl] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[EREmppayallowancedtl]', N'utaddip') IS NULL ALTER TABLE [EREmppayallowancedtl] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmppayallowancedtl]', N'utaddsessionid') IS NULL ALTER TABLE [EREmppayallowancedtl] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[EREmppayallowancedtl]', N'utaddsystem') IS NULL ALTER TABLE [EREmppayallowancedtl] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmppayallowancedtl]', N'utaddtime') IS NULL ALTER TABLE [EREmppayallowancedtl] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmppayallowancedtl]', N'utadduser') IS NULL ALTER TABLE [EREmppayallowancedtl] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmppayallowancedtl]', N'utdoctype') IS NULL ALTER TABLE [EREmppayallowancedtl] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[EREmppayallowancedtl]', N'uteditappversion') IS NULL ALTER TABLE [EREmppayallowancedtl] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmppayallowancedtl]', N'uteditdbversion') IS NULL ALTER TABLE [EREmppayallowancedtl] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmppayallowancedtl]', N'uteditid') IS NULL ALTER TABLE [EREmppayallowancedtl] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[EREmppayallowancedtl]', N'uteditip') IS NULL ALTER TABLE [EREmppayallowancedtl] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmppayallowancedtl]', N'uteditsessionid') IS NULL ALTER TABLE [EREmppayallowancedtl] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[EREmppayallowancedtl]', N'uteditsystem') IS NULL ALTER TABLE [EREmppayallowancedtl] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmppayallowancedtl]', N'utedittime') IS NULL ALTER TABLE [EREmppayallowancedtl] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmppayallowancedtl]', N'utedituser') IS NULL ALTER TABLE [EREmppayallowancedtl] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmppayallowancedtl]', N'utsubdoctype') IS NULL ALTER TABLE [EREmppayallowancedtl] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[EREmppayallowancedtl]', N'emppayrollid') IS NULL ALTER TABLE [EREmppayallowancedtl] ADD [emppayrollid] int NULL;
GO
IF COL_LENGTH(N'[EREmppayallowancedtl]', N'lineindex') IS NULL ALTER TABLE [EREmppayallowancedtl] ADD [lineindex] int NULL;
GO
IF COL_LENGTH(N'[EREmppayallowancedtl]', N'value') IS NULL ALTER TABLE [EREmppayallowancedtl] ADD [value] float NULL;
GO
IF COL_LENGTH(N'[EREmppayallowancedtl]', N'startdate') IS NULL ALTER TABLE [EREmppayallowancedtl] ADD [startdate] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmppayallowancedtl]', N'enddate') IS NULL ALTER TABLE [EREmppayallowancedtl] ADD [enddate] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmppayallowancedtl]', N'utsysgen') IS NULL ALTER TABLE [EREmppayallowancedtl] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[EREmppayallowancedtl]', N'utblock') IS NULL ALTER TABLE [EREmppayallowancedtl] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[EREmppayallowancedtl]', N'utispartial') IS NULL ALTER TABLE [EREmppayallowancedtl] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[EREmppayallowancedtl]', N'utrecordversion') IS NULL ALTER TABLE [EREmppayallowancedtl] ADD [utrecordversion] int NULL;
GO
IF OBJECT_ID(N'[EREmppaybasesalarydtl]', N'U') IS NULL BEGIN CREATE TABLE [EREmppaybasesalarydtl] (
    [Id] bigint NOT NULL,
    [emppayrollid] int NULL,
    [lineindex] int NULL,
    [earntypeid] int NULL,
    [value] float NULL,
    [tovalue] float NULL,
    [startdate] datetime2 NULL,
    [enddate] datetime2 NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_EREmppaybasesalarydtl] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[EREmppaybasesalarydtl]', N'Id') IS NULL ALTER TABLE [EREmppaybasesalarydtl] ADD [Id] bigint NULL;
GO
IF COL_LENGTH(N'[EREmppaybasesalarydtl]', N'emppayrollid') IS NULL ALTER TABLE [EREmppaybasesalarydtl] ADD [emppayrollid] int NULL;
GO
IF COL_LENGTH(N'[EREmppaybasesalarydtl]', N'lineindex') IS NULL ALTER TABLE [EREmppaybasesalarydtl] ADD [lineindex] int NULL;
GO
IF COL_LENGTH(N'[EREmppaybasesalarydtl]', N'earntypeid') IS NULL ALTER TABLE [EREmppaybasesalarydtl] ADD [earntypeid] int NULL;
GO
IF COL_LENGTH(N'[EREmppaybasesalarydtl]', N'value') IS NULL ALTER TABLE [EREmppaybasesalarydtl] ADD [value] float NULL;
GO
IF COL_LENGTH(N'[EREmppaybasesalarydtl]', N'tovalue') IS NULL ALTER TABLE [EREmppaybasesalarydtl] ADD [tovalue] float NULL;
GO
IF COL_LENGTH(N'[EREmppaybasesalarydtl]', N'startdate') IS NULL ALTER TABLE [EREmppaybasesalarydtl] ADD [startdate] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmppaybasesalarydtl]', N'enddate') IS NULL ALTER TABLE [EREmppaybasesalarydtl] ADD [enddate] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmppaybasesalarydtl]', N'utsysgen') IS NULL ALTER TABLE [EREmppaybasesalarydtl] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[EREmppaybasesalarydtl]', N'utblock') IS NULL ALTER TABLE [EREmppaybasesalarydtl] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[EREmppaybasesalarydtl]', N'utispartial') IS NULL ALTER TABLE [EREmppaybasesalarydtl] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[EREmppaybasesalarydtl]', N'utrecordversion') IS NULL ALTER TABLE [EREmppaybasesalarydtl] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[EREmppaybasesalarydtl]', N'utaddappversion') IS NULL ALTER TABLE [EREmppaybasesalarydtl] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmppaybasesalarydtl]', N'utadddbversion') IS NULL ALTER TABLE [EREmppaybasesalarydtl] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmppaybasesalarydtl]', N'utaddid') IS NULL ALTER TABLE [EREmppaybasesalarydtl] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[EREmppaybasesalarydtl]', N'utaddip') IS NULL ALTER TABLE [EREmppaybasesalarydtl] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmppaybasesalarydtl]', N'utaddsessionid') IS NULL ALTER TABLE [EREmppaybasesalarydtl] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[EREmppaybasesalarydtl]', N'utaddsystem') IS NULL ALTER TABLE [EREmppaybasesalarydtl] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmppaybasesalarydtl]', N'utaddtime') IS NULL ALTER TABLE [EREmppaybasesalarydtl] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmppaybasesalarydtl]', N'utadduser') IS NULL ALTER TABLE [EREmppaybasesalarydtl] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmppaybasesalarydtl]', N'utdoctype') IS NULL ALTER TABLE [EREmppaybasesalarydtl] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[EREmppaybasesalarydtl]', N'uteditappversion') IS NULL ALTER TABLE [EREmppaybasesalarydtl] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmppaybasesalarydtl]', N'uteditdbversion') IS NULL ALTER TABLE [EREmppaybasesalarydtl] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmppaybasesalarydtl]', N'uteditid') IS NULL ALTER TABLE [EREmppaybasesalarydtl] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[EREmppaybasesalarydtl]', N'uteditip') IS NULL ALTER TABLE [EREmppaybasesalarydtl] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmppaybasesalarydtl]', N'uteditsessionid') IS NULL ALTER TABLE [EREmppaybasesalarydtl] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[EREmppaybasesalarydtl]', N'uteditsystem') IS NULL ALTER TABLE [EREmppaybasesalarydtl] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmppaybasesalarydtl]', N'utedittime') IS NULL ALTER TABLE [EREmppaybasesalarydtl] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmppaybasesalarydtl]', N'utedituser') IS NULL ALTER TABLE [EREmppaybasesalarydtl] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmppaybasesalarydtl]', N'utsubdoctype') IS NULL ALTER TABLE [EREmppaybasesalarydtl] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[EREmppaydeductiondtl]', N'U') IS NULL BEGIN CREATE TABLE [EREmppaydeductiondtl] (
    [Id] int NOT NULL,
    [deductiontypeid] int NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    [emppayrollid] int NULL,
    [lineindex] int NULL,
    [value] float NULL,
    [startdate] datetime2 NULL,
    [enddate] datetime2 NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    CONSTRAINT [PK_EREmppaydeductiondtl] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[EREmppaydeductiondtl]', N'Id') IS NULL ALTER TABLE [EREmppaydeductiondtl] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[EREmppaydeductiondtl]', N'deductiontypeid') IS NULL ALTER TABLE [EREmppaydeductiondtl] ADD [deductiontypeid] int NULL;
GO
IF COL_LENGTH(N'[EREmppaydeductiondtl]', N'utaddappversion') IS NULL ALTER TABLE [EREmppaydeductiondtl] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmppaydeductiondtl]', N'utadddbversion') IS NULL ALTER TABLE [EREmppaydeductiondtl] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmppaydeductiondtl]', N'utaddid') IS NULL ALTER TABLE [EREmppaydeductiondtl] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[EREmppaydeductiondtl]', N'utaddip') IS NULL ALTER TABLE [EREmppaydeductiondtl] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmppaydeductiondtl]', N'utaddsessionid') IS NULL ALTER TABLE [EREmppaydeductiondtl] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[EREmppaydeductiondtl]', N'utaddsystem') IS NULL ALTER TABLE [EREmppaydeductiondtl] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmppaydeductiondtl]', N'utaddtime') IS NULL ALTER TABLE [EREmppaydeductiondtl] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmppaydeductiondtl]', N'utadduser') IS NULL ALTER TABLE [EREmppaydeductiondtl] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmppaydeductiondtl]', N'utdoctype') IS NULL ALTER TABLE [EREmppaydeductiondtl] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[EREmppaydeductiondtl]', N'uteditappversion') IS NULL ALTER TABLE [EREmppaydeductiondtl] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmppaydeductiondtl]', N'uteditdbversion') IS NULL ALTER TABLE [EREmppaydeductiondtl] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EREmppaydeductiondtl]', N'uteditid') IS NULL ALTER TABLE [EREmppaydeductiondtl] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[EREmppaydeductiondtl]', N'uteditip') IS NULL ALTER TABLE [EREmppaydeductiondtl] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmppaydeductiondtl]', N'uteditsessionid') IS NULL ALTER TABLE [EREmppaydeductiondtl] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[EREmppaydeductiondtl]', N'uteditsystem') IS NULL ALTER TABLE [EREmppaydeductiondtl] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmppaydeductiondtl]', N'utedittime') IS NULL ALTER TABLE [EREmppaydeductiondtl] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmppaydeductiondtl]', N'utedituser') IS NULL ALTER TABLE [EREmppaydeductiondtl] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EREmppaydeductiondtl]', N'utsubdoctype') IS NULL ALTER TABLE [EREmppaydeductiondtl] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[EREmppaydeductiondtl]', N'emppayrollid') IS NULL ALTER TABLE [EREmppaydeductiondtl] ADD [emppayrollid] int NULL;
GO
IF COL_LENGTH(N'[EREmppaydeductiondtl]', N'lineindex') IS NULL ALTER TABLE [EREmppaydeductiondtl] ADD [lineindex] int NULL;
GO
IF COL_LENGTH(N'[EREmppaydeductiondtl]', N'value') IS NULL ALTER TABLE [EREmppaydeductiondtl] ADD [value] float NULL;
GO
IF COL_LENGTH(N'[EREmppaydeductiondtl]', N'startdate') IS NULL ALTER TABLE [EREmppaydeductiondtl] ADD [startdate] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmppaydeductiondtl]', N'enddate') IS NULL ALTER TABLE [EREmppaydeductiondtl] ADD [enddate] datetime2 NULL;
GO
IF COL_LENGTH(N'[EREmppaydeductiondtl]', N'utsysgen') IS NULL ALTER TABLE [EREmppaydeductiondtl] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[EREmppaydeductiondtl]', N'utblock') IS NULL ALTER TABLE [EREmppaydeductiondtl] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[EREmppaydeductiondtl]', N'utispartial') IS NULL ALTER TABLE [EREmppaydeductiondtl] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[EREmppaydeductiondtl]', N'utrecordversion') IS NULL ALTER TABLE [EREmppaydeductiondtl] ADD [utrecordversion] int NULL;
GO
IF OBJECT_ID(N'[EREmppayrollLoanType]', N'U') IS NULL BEGIN CREATE TABLE [EREmppayrollLoanType] (
    [Id] int NOT NULL,
    [emppayrollid] int NULL,
    [loantypeid] int NULL,
    [utblock] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    CONSTRAINT [PK_EREmppayrollLoanType] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[EREmppayrollLoanType]', N'Id') IS NULL ALTER TABLE [EREmppayrollLoanType] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[EREmppayrollLoanType]', N'emppayrollid') IS NULL ALTER TABLE [EREmppayrollLoanType] ADD [emppayrollid] int NULL;
GO
IF COL_LENGTH(N'[EREmppayrollLoanType]', N'loantypeid') IS NULL ALTER TABLE [EREmppayrollLoanType] ADD [loantypeid] int NULL;
GO
IF COL_LENGTH(N'[EREmppayrollLoanType]', N'utblock') IS NULL ALTER TABLE [EREmppayrollLoanType] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[EREmppayrollLoanType]', N'utrecordversion') IS NULL ALTER TABLE [EREmppayrollLoanType] ADD [utrecordversion] int NULL;
GO
IF OBJECT_ID(N'[ETLeavetrans]', N'U') IS NULL BEGIN CREATE TABLE [ETLeavetrans] (
    [id] int NOT NULL,
    [transno] nvarchar(50) NULL,
    [transdate] datetime2 NULL,
    [requestid] int NULL,
    [employeeid] int NULL,
    [leaveunitid] int NULL,
    [leavetypeid] int NULL,
    [fromdate] datetime2 NULL,
    [tilldate] datetime2 NULL,
    [leavedays] float NULL,
    [entityid] int NULL,
    [clientid] int NULL,
    [ispaid] bit NULL,
    [statusid] int NULL,
    [remarks] nvarchar(1500) NULL,
    [admApproval] int NULL,
    [utblock] bit NOT NULL,
    [utsysgen] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utdoctype] int NULL,
    [utsubdoctype] int NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    CONSTRAINT [PK_ETLeavetrans] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[ETLeavetrans]', N'id') IS NULL ALTER TABLE [ETLeavetrans] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[ETLeavetrans]', N'transno') IS NULL ALTER TABLE [ETLeavetrans] ADD [transno] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[ETLeavetrans]', N'transdate') IS NULL ALTER TABLE [ETLeavetrans] ADD [transdate] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETLeavetrans]', N'requestid') IS NULL ALTER TABLE [ETLeavetrans] ADD [requestid] int NULL;
GO
IF COL_LENGTH(N'[ETLeavetrans]', N'employeeid') IS NULL ALTER TABLE [ETLeavetrans] ADD [employeeid] int NULL;
GO
IF COL_LENGTH(N'[ETLeavetrans]', N'leaveunitid') IS NULL ALTER TABLE [ETLeavetrans] ADD [leaveunitid] int NULL;
GO
IF COL_LENGTH(N'[ETLeavetrans]', N'leavetypeid') IS NULL ALTER TABLE [ETLeavetrans] ADD [leavetypeid] int NULL;
GO
IF COL_LENGTH(N'[ETLeavetrans]', N'fromdate') IS NULL ALTER TABLE [ETLeavetrans] ADD [fromdate] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETLeavetrans]', N'tilldate') IS NULL ALTER TABLE [ETLeavetrans] ADD [tilldate] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETLeavetrans]', N'leavedays') IS NULL ALTER TABLE [ETLeavetrans] ADD [leavedays] float NULL;
GO
IF COL_LENGTH(N'[ETLeavetrans]', N'entityid') IS NULL ALTER TABLE [ETLeavetrans] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[ETLeavetrans]', N'clientid') IS NULL ALTER TABLE [ETLeavetrans] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[ETLeavetrans]', N'ispaid') IS NULL ALTER TABLE [ETLeavetrans] ADD [ispaid] bit NULL;
GO
IF COL_LENGTH(N'[ETLeavetrans]', N'statusid') IS NULL ALTER TABLE [ETLeavetrans] ADD [statusid] int NULL;
GO
IF COL_LENGTH(N'[ETLeavetrans]', N'remarks') IS NULL ALTER TABLE [ETLeavetrans] ADD [remarks] nvarchar(1500) NULL;
GO
IF COL_LENGTH(N'[ETLeavetrans]', N'admApproval') IS NULL ALTER TABLE [ETLeavetrans] ADD [admApproval] int NULL;
GO
IF COL_LENGTH(N'[ETLeavetrans]', N'utblock') IS NULL ALTER TABLE [ETLeavetrans] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[ETLeavetrans]', N'utsysgen') IS NULL ALTER TABLE [ETLeavetrans] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[ETLeavetrans]', N'utispartial') IS NULL ALTER TABLE [ETLeavetrans] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[ETLeavetrans]', N'utrecordversion') IS NULL ALTER TABLE [ETLeavetrans] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[ETLeavetrans]', N'utdoctype') IS NULL ALTER TABLE [ETLeavetrans] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[ETLeavetrans]', N'utsubdoctype') IS NULL ALTER TABLE [ETLeavetrans] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[ETLeavetrans]', N'utaddappversion') IS NULL ALTER TABLE [ETLeavetrans] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETLeavetrans]', N'utadddbversion') IS NULL ALTER TABLE [ETLeavetrans] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETLeavetrans]', N'utaddid') IS NULL ALTER TABLE [ETLeavetrans] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[ETLeavetrans]', N'utaddip') IS NULL ALTER TABLE [ETLeavetrans] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETLeavetrans]', N'utaddsessionid') IS NULL ALTER TABLE [ETLeavetrans] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETLeavetrans]', N'utaddsystem') IS NULL ALTER TABLE [ETLeavetrans] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETLeavetrans]', N'utaddtime') IS NULL ALTER TABLE [ETLeavetrans] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETLeavetrans]', N'utadduser') IS NULL ALTER TABLE [ETLeavetrans] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETLeavetrans]', N'uteditappversion') IS NULL ALTER TABLE [ETLeavetrans] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETLeavetrans]', N'uteditdbversion') IS NULL ALTER TABLE [ETLeavetrans] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ETLeavetrans]', N'uteditid') IS NULL ALTER TABLE [ETLeavetrans] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[ETLeavetrans]', N'uteditip') IS NULL ALTER TABLE [ETLeavetrans] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETLeavetrans]', N'uteditsessionid') IS NULL ALTER TABLE [ETLeavetrans] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[ETLeavetrans]', N'uteditsystem') IS NULL ALTER TABLE [ETLeavetrans] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ETLeavetrans]', N'utedittime') IS NULL ALTER TABLE [ETLeavetrans] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ETLeavetrans]', N'utedituser') IS NULL ALTER TABLE [ETLeavetrans] ADD [utedituser] varchar(25) NULL;
GO
IF OBJECT_ID(N'[HRpayrollprocessdtl]', N'U') IS NULL BEGIN CREATE TABLE [HRpayrollprocessdtl] (
    [Id] int NOT NULL,
    [processid] int NULL,
    [lovtypeid] int NULL,
    [typeid] int NULL,
    [fromdate] datetime2 NULL,
    [tilldate] datetime2 NULL,
    [value] float NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_HRpayrollprocessdtl] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[HRpayrollprocessdtl]', N'Id') IS NULL ALTER TABLE [HRpayrollprocessdtl] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocessdtl]', N'processid') IS NULL ALTER TABLE [HRpayrollprocessdtl] ADD [processid] int NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocessdtl]', N'lovtypeid') IS NULL ALTER TABLE [HRpayrollprocessdtl] ADD [lovtypeid] int NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocessdtl]', N'typeid') IS NULL ALTER TABLE [HRpayrollprocessdtl] ADD [typeid] int NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocessdtl]', N'fromdate') IS NULL ALTER TABLE [HRpayrollprocessdtl] ADD [fromdate] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocessdtl]', N'tilldate') IS NULL ALTER TABLE [HRpayrollprocessdtl] ADD [tilldate] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocessdtl]', N'value') IS NULL ALTER TABLE [HRpayrollprocessdtl] ADD [value] float NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocessdtl]', N'utsysgen') IS NULL ALTER TABLE [HRpayrollprocessdtl] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocessdtl]', N'utblock') IS NULL ALTER TABLE [HRpayrollprocessdtl] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocessdtl]', N'utispartial') IS NULL ALTER TABLE [HRpayrollprocessdtl] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocessdtl]', N'utrecordversion') IS NULL ALTER TABLE [HRpayrollprocessdtl] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocessdtl]', N'utaddappversion') IS NULL ALTER TABLE [HRpayrollprocessdtl] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocessdtl]', N'utadddbversion') IS NULL ALTER TABLE [HRpayrollprocessdtl] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocessdtl]', N'utaddid') IS NULL ALTER TABLE [HRpayrollprocessdtl] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocessdtl]', N'utaddip') IS NULL ALTER TABLE [HRpayrollprocessdtl] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocessdtl]', N'utaddsessionid') IS NULL ALTER TABLE [HRpayrollprocessdtl] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocessdtl]', N'utaddsystem') IS NULL ALTER TABLE [HRpayrollprocessdtl] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocessdtl]', N'utaddtime') IS NULL ALTER TABLE [HRpayrollprocessdtl] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocessdtl]', N'utadduser') IS NULL ALTER TABLE [HRpayrollprocessdtl] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocessdtl]', N'utdoctype') IS NULL ALTER TABLE [HRpayrollprocessdtl] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocessdtl]', N'uteditappversion') IS NULL ALTER TABLE [HRpayrollprocessdtl] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocessdtl]', N'uteditdbversion') IS NULL ALTER TABLE [HRpayrollprocessdtl] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocessdtl]', N'uteditid') IS NULL ALTER TABLE [HRpayrollprocessdtl] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocessdtl]', N'uteditip') IS NULL ALTER TABLE [HRpayrollprocessdtl] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocessdtl]', N'uteditsessionid') IS NULL ALTER TABLE [HRpayrollprocessdtl] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocessdtl]', N'uteditsystem') IS NULL ALTER TABLE [HRpayrollprocessdtl] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocessdtl]', N'utedittime') IS NULL ALTER TABLE [HRpayrollprocessdtl] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocessdtl]', N'utedituser') IS NULL ALTER TABLE [HRpayrollprocessdtl] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[HRpayrollprocessdtl]', N'utsubdoctype') IS NULL ALTER TABLE [HRpayrollprocessdtl] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[EPLoantransdtl]', N'U') IS NULL BEGIN CREATE TABLE [EPLoantransdtl] (
    [id] int NOT NULL,
    [loanid] int NULL,
    [year] int NULL,
    [monthid] int NULL,
    [installment] float NULL,
    [balance] float NULL,
    [recovered] bit NOT NULL,
    [merged] bit NOT NULL,
    [notdeduct] bit NOT NULL,
    [mergeloanid] int NULL,
    [reason] nvarchar(500) NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_EPLoantransdtl] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[EPLoantransdtl]', N'id') IS NULL ALTER TABLE [EPLoantransdtl] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[EPLoantransdtl]', N'loanid') IS NULL ALTER TABLE [EPLoantransdtl] ADD [loanid] int NULL;
GO
IF COL_LENGTH(N'[EPLoantransdtl]', N'year') IS NULL ALTER TABLE [EPLoantransdtl] ADD [year] int NULL;
GO
IF COL_LENGTH(N'[EPLoantransdtl]', N'monthid') IS NULL ALTER TABLE [EPLoantransdtl] ADD [monthid] int NULL;
GO
IF COL_LENGTH(N'[EPLoantransdtl]', N'installment') IS NULL ALTER TABLE [EPLoantransdtl] ADD [installment] float NULL;
GO
IF COL_LENGTH(N'[EPLoantransdtl]', N'balance') IS NULL ALTER TABLE [EPLoantransdtl] ADD [balance] float NULL;
GO
IF COL_LENGTH(N'[EPLoantransdtl]', N'recovered') IS NULL ALTER TABLE [EPLoantransdtl] ADD [recovered] bit NULL;
GO
IF COL_LENGTH(N'[EPLoantransdtl]', N'merged') IS NULL ALTER TABLE [EPLoantransdtl] ADD [merged] bit NULL;
GO
IF COL_LENGTH(N'[EPLoantransdtl]', N'notdeduct') IS NULL ALTER TABLE [EPLoantransdtl] ADD [notdeduct] bit NULL;
GO
IF COL_LENGTH(N'[EPLoantransdtl]', N'mergeloanid') IS NULL ALTER TABLE [EPLoantransdtl] ADD [mergeloanid] int NULL;
GO
IF COL_LENGTH(N'[EPLoantransdtl]', N'reason') IS NULL ALTER TABLE [EPLoantransdtl] ADD [reason] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[EPLoantransdtl]', N'utsysgen') IS NULL ALTER TABLE [EPLoantransdtl] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[EPLoantransdtl]', N'utblock') IS NULL ALTER TABLE [EPLoantransdtl] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[EPLoantransdtl]', N'utispartial') IS NULL ALTER TABLE [EPLoantransdtl] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[EPLoantransdtl]', N'utrecordversion') IS NULL ALTER TABLE [EPLoantransdtl] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[EPLoantransdtl]', N'utaddappversion') IS NULL ALTER TABLE [EPLoantransdtl] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EPLoantransdtl]', N'utadddbversion') IS NULL ALTER TABLE [EPLoantransdtl] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EPLoantransdtl]', N'utaddid') IS NULL ALTER TABLE [EPLoantransdtl] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[EPLoantransdtl]', N'utaddip') IS NULL ALTER TABLE [EPLoantransdtl] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPLoantransdtl]', N'utaddsessionid') IS NULL ALTER TABLE [EPLoantransdtl] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[EPLoantransdtl]', N'utaddsystem') IS NULL ALTER TABLE [EPLoantransdtl] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPLoantransdtl]', N'utaddtime') IS NULL ALTER TABLE [EPLoantransdtl] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EPLoantransdtl]', N'utadduser') IS NULL ALTER TABLE [EPLoantransdtl] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPLoantransdtl]', N'utdoctype') IS NULL ALTER TABLE [EPLoantransdtl] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[EPLoantransdtl]', N'uteditappversion') IS NULL ALTER TABLE [EPLoantransdtl] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EPLoantransdtl]', N'uteditdbversion') IS NULL ALTER TABLE [EPLoantransdtl] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[EPLoantransdtl]', N'uteditid') IS NULL ALTER TABLE [EPLoantransdtl] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[EPLoantransdtl]', N'uteditip') IS NULL ALTER TABLE [EPLoantransdtl] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPLoantransdtl]', N'uteditsessionid') IS NULL ALTER TABLE [EPLoantransdtl] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[EPLoantransdtl]', N'uteditsystem') IS NULL ALTER TABLE [EPLoantransdtl] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPLoantransdtl]', N'utedittime') IS NULL ALTER TABLE [EPLoantransdtl] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[EPLoantransdtl]', N'utedituser') IS NULL ALTER TABLE [EPLoantransdtl] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[EPLoantransdtl]', N'utsubdoctype') IS NULL ALTER TABLE [EPLoantransdtl] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[Country]', N'U') IS NULL BEGIN CREATE TABLE [Country] (
    [Id] int NOT NULL,
    [countryname] nvarchar(100) NULL,
    [countrycode] nvarchar(2) NULL,
    [isocode] nvarchar(5) NULL,
    [dialcode] nvarchar(10) NULL,
    [utblock] bit NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    CONSTRAINT [PK_Country] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[Country]', N'Id') IS NULL ALTER TABLE [Country] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[Country]', N'countryname') IS NULL ALTER TABLE [Country] ADD [countryname] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[Country]', N'countrycode') IS NULL ALTER TABLE [Country] ADD [countrycode] nvarchar(2) NULL;
GO
IF COL_LENGTH(N'[Country]', N'isocode') IS NULL ALTER TABLE [Country] ADD [isocode] nvarchar(5) NULL;
GO
IF COL_LENGTH(N'[Country]', N'dialcode') IS NULL ALTER TABLE [Country] ADD [dialcode] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[Country]', N'utblock') IS NULL ALTER TABLE [Country] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[Country]', N'utaddappversion') IS NULL ALTER TABLE [Country] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[Country]', N'utadddbversion') IS NULL ALTER TABLE [Country] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[Country]', N'utaddid') IS NULL ALTER TABLE [Country] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[Country]', N'utaddip') IS NULL ALTER TABLE [Country] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[Country]', N'utaddsessionid') IS NULL ALTER TABLE [Country] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[Country]', N'utaddsystem') IS NULL ALTER TABLE [Country] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[Country]', N'utaddtime') IS NULL ALTER TABLE [Country] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[Country]', N'utadduser') IS NULL ALTER TABLE [Country] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[Country]', N'utdoctype') IS NULL ALTER TABLE [Country] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[Country]', N'uteditappversion') IS NULL ALTER TABLE [Country] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[Country]', N'uteditdbversion') IS NULL ALTER TABLE [Country] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[Country]', N'uteditid') IS NULL ALTER TABLE [Country] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[Country]', N'uteditip') IS NULL ALTER TABLE [Country] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[Country]', N'uteditsessionid') IS NULL ALTER TABLE [Country] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[Country]', N'uteditsystem') IS NULL ALTER TABLE [Country] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[Country]', N'utedittime') IS NULL ALTER TABLE [Country] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[Country]', N'utedituser') IS NULL ALTER TABLE [Country] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[Country]', N'utispartial') IS NULL ALTER TABLE [Country] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[Country]', N'utrecordversion') IS NULL ALTER TABLE [Country] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[Country]', N'utsubdoctype') IS NULL ALTER TABLE [Country] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[Country]', N'utsysgen') IS NULL ALTER TABLE [Country] ADD [utsysgen] bit NULL;
GO
IF OBJECT_ID(N'[EREmployeeinfohdr]', N'U') IS NULL BEGIN CREATE TABLE [EREmployeeinfohdr] (
    [Id] int NOT NULL,
    [clientid] int NULL,
    [entityid] int NULL,
    [employeecode] nvarchar(max) NULL,
    [firstname] nvarchar(100) NULL,
    [lastname] nvarchar(100) NULL,
    [empstatusid] int NULL,
    [utblock] bit NOT NULL,
    CONSTRAINT [PK_EREmployeeinfohdr] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'Id') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'clientid') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'entityid') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'employeecode') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [employeecode] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'firstname') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [firstname] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'lastname') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [lastname] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'empstatusid') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [empstatusid] int NULL;
GO
IF COL_LENGTH(N'[EREmployeeinfohdr]', N'utblock') IS NULL ALTER TABLE [EREmployeeinfohdr] ADD [utblock] bit NULL;
GO
IF OBJECT_ID(N'[FIBankaccountsetup]', N'U') IS NULL BEGIN CREATE TABLE [FIBankaccountsetup] (
    [Id] int NOT NULL,
    [code] nvarchar(100) NULL,
    [title] nvarchar(100) NULL,
    [accountno] nvarchar(100) NULL,
    [clientid] int NULL,
    [entityid] int NULL,
    [status] bit NULL,
    [utblock] bit NOT NULL,
    CONSTRAINT [PK_FIBankaccountsetup] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[FIBankaccountsetup]', N'Id') IS NULL ALTER TABLE [FIBankaccountsetup] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[FIBankaccountsetup]', N'code') IS NULL ALTER TABLE [FIBankaccountsetup] ADD [code] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[FIBankaccountsetup]', N'title') IS NULL ALTER TABLE [FIBankaccountsetup] ADD [title] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[FIBankaccountsetup]', N'accountno') IS NULL ALTER TABLE [FIBankaccountsetup] ADD [accountno] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[FIBankaccountsetup]', N'clientid') IS NULL ALTER TABLE [FIBankaccountsetup] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[FIBankaccountsetup]', N'entityid') IS NULL ALTER TABLE [FIBankaccountsetup] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[FIBankaccountsetup]', N'status') IS NULL ALTER TABLE [FIBankaccountsetup] ADD [status] bit NULL;
GO
IF COL_LENGTH(N'[FIBankaccountsetup]', N'utblock') IS NULL ALTER TABLE [FIBankaccountsetup] ADD [utblock] bit NULL;
GO
IF OBJECT_ID(N'[FICOA_NEW]', N'U') IS NULL BEGIN CREATE TABLE [FICOA_NEW] (
    [Id] int NOT NULL,
    [Title] nvarchar(max) NULL,
    [accountcode] nvarchar(max) NULL,
    [clientid] int NULL,
    [Subsidiary] bit NULL,
    [Active] bit NULL,
    [accountnatureid] int NULL,
    CONSTRAINT [PK_FICOA_NEW] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[FICOA_NEW]', N'Id') IS NULL ALTER TABLE [FICOA_NEW] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[FICOA_NEW]', N'Title') IS NULL ALTER TABLE [FICOA_NEW] ADD [Title] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[FICOA_NEW]', N'accountcode') IS NULL ALTER TABLE [FICOA_NEW] ADD [accountcode] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[FICOA_NEW]', N'clientid') IS NULL ALTER TABLE [FICOA_NEW] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[FICOA_NEW]', N'Subsidiary') IS NULL ALTER TABLE [FICOA_NEW] ADD [Subsidiary] bit NULL;
GO
IF COL_LENGTH(N'[FICOA_NEW]', N'Active') IS NULL ALTER TABLE [FICOA_NEW] ADD [Active] bit NULL;
GO
IF COL_LENGTH(N'[FICOA_NEW]', N'accountnatureid') IS NULL ALTER TABLE [FICOA_NEW] ADD [accountnatureid] int NULL;
GO
IF OBJECT_ID(N'[FITransactionDetail]', N'U') IS NULL BEGIN CREATE TABLE [FITransactionDetail] (
    [Id] int NOT NULL,
    [transactionid] int NULL,
    [ficoaid] int NULL,
    [debit] float NULL,
    [credit] float NULL,
    [utblock] bit NOT NULL,
    CONSTRAINT [PK_FITransactionDetail] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[FITransactionDetail]', N'Id') IS NULL ALTER TABLE [FITransactionDetail] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[FITransactionDetail]', N'transactionid') IS NULL ALTER TABLE [FITransactionDetail] ADD [transactionid] int NULL;
GO
IF COL_LENGTH(N'[FITransactionDetail]', N'ficoaid') IS NULL ALTER TABLE [FITransactionDetail] ADD [ficoaid] int NULL;
GO
IF COL_LENGTH(N'[FITransactionDetail]', N'debit') IS NULL ALTER TABLE [FITransactionDetail] ADD [debit] float NULL;
GO
IF COL_LENGTH(N'[FITransactionDetail]', N'credit') IS NULL ALTER TABLE [FITransactionDetail] ADD [credit] float NULL;
GO
IF COL_LENGTH(N'[FITransactionDetail]', N'utblock') IS NULL ALTER TABLE [FITransactionDetail] ADD [utblock] bit NULL;
GO
IF OBJECT_ID(N'[FITransactionMaster]', N'U') IS NULL BEGIN CREATE TABLE [FITransactionMaster] (
    [Id] int NOT NULL,
    [clientid] int NULL,
    [entityid] int NULL,
    [transdate] datetime2 NOT NULL,
    [utblock] bit NOT NULL,
    CONSTRAINT [PK_FITransactionMaster] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[FITransactionMaster]', N'Id') IS NULL ALTER TABLE [FITransactionMaster] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[FITransactionMaster]', N'clientid') IS NULL ALTER TABLE [FITransactionMaster] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[FITransactionMaster]', N'entityid') IS NULL ALTER TABLE [FITransactionMaster] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[FITransactionMaster]', N'transdate') IS NULL ALTER TABLE [FITransactionMaster] ADD [transdate] datetime2 NULL;
GO
IF COL_LENGTH(N'[FITransactionMaster]', N'utblock') IS NULL ALTER TABLE [FITransactionMaster] ADD [utblock] bit NULL;
GO
IF OBJECT_ID(N'[FOAdmEnquiry]', N'U') IS NULL BEGIN CREATE TABLE [FOAdmEnquiry] (
    [id] int NOT NULL,
    [clientid] int NULL,
    [entityid] int NULL,
    CONSTRAINT [PK_FOAdmEnquiry] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[FOAdmEnquiry]', N'id') IS NULL ALTER TABLE [FOAdmEnquiry] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[FOAdmEnquiry]', N'clientid') IS NULL ALTER TABLE [FOAdmEnquiry] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[FOAdmEnquiry]', N'entityid') IS NULL ALTER TABLE [FOAdmEnquiry] ADD [entityid] int NULL;
GO
IF OBJECT_ID(N'[FOAdmEnquirydtl]', N'U') IS NULL BEGIN CREATE TABLE [FOAdmEnquirydtl] (
    [Id] int NOT NULL,
    [enquiryid] int NULL,
    CONSTRAINT [PK_FOAdmEnquirydtl] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[FOAdmEnquirydtl]', N'Id') IS NULL ALTER TABLE [FOAdmEnquirydtl] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[FOAdmEnquirydtl]', N'enquiryid') IS NULL ALTER TABLE [FOAdmEnquirydtl] ADD [enquiryid] int NULL;
GO
IF OBJECT_ID(N'[FOPurpose]', N'U') IS NULL BEGIN CREATE TABLE [FOPurpose] (
    [id] int NOT NULL,
    [code] nvarchar(max) NULL,
    [stxt] nvarchar(max) NULL,
    [ltxt] nvarchar(max) NULL,
    [clientid] int NULL,
    [utsubdoctype] int NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utsysgen] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    CONSTRAINT [PK_FOPurpose] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[FOPurpose]', N'id') IS NULL ALTER TABLE [FOPurpose] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[FOPurpose]', N'code') IS NULL ALTER TABLE [FOPurpose] ADD [code] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[FOPurpose]', N'stxt') IS NULL ALTER TABLE [FOPurpose] ADD [stxt] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[FOPurpose]', N'ltxt') IS NULL ALTER TABLE [FOPurpose] ADD [ltxt] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[FOPurpose]', N'clientid') IS NULL ALTER TABLE [FOPurpose] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[FOPurpose]', N'utsubdoctype') IS NULL ALTER TABLE [FOPurpose] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[FOPurpose]', N'utblock') IS NULL ALTER TABLE [FOPurpose] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[FOPurpose]', N'utispartial') IS NULL ALTER TABLE [FOPurpose] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[FOPurpose]', N'utsysgen') IS NULL ALTER TABLE [FOPurpose] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[FOPurpose]', N'utrecordversion') IS NULL ALTER TABLE [FOPurpose] ADD [utrecordversion] int NULL;
GO
IF OBJECT_ID(N'[GNImage]', N'U') IS NULL BEGIN CREATE TABLE [GNImage] (
    [Id] bigint NOT NULL,
    [filename] nvarchar(500) NULL,
    [img] image NULL,
    [clientid] int NULL,
    [logdate] datetime2 NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_GNImage] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[GNImage]', N'Id') IS NULL ALTER TABLE [GNImage] ADD [Id] bigint NULL;
GO
IF COL_LENGTH(N'[GNImage]', N'filename') IS NULL ALTER TABLE [GNImage] ADD [filename] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[GNImage]', N'img') IS NULL ALTER TABLE [GNImage] ADD [img] image NULL;
GO
IF COL_LENGTH(N'[GNImage]', N'clientid') IS NULL ALTER TABLE [GNImage] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[GNImage]', N'logdate') IS NULL ALTER TABLE [GNImage] ADD [logdate] datetime2 NULL;
GO
IF COL_LENGTH(N'[GNImage]', N'utsysgen') IS NULL ALTER TABLE [GNImage] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[GNImage]', N'utblock') IS NULL ALTER TABLE [GNImage] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[GNImage]', N'utispartial') IS NULL ALTER TABLE [GNImage] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[GNImage]', N'utrecordversion') IS NULL ALTER TABLE [GNImage] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[GNImage]', N'utaddappversion') IS NULL ALTER TABLE [GNImage] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[GNImage]', N'utadddbversion') IS NULL ALTER TABLE [GNImage] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[GNImage]', N'utaddid') IS NULL ALTER TABLE [GNImage] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[GNImage]', N'utaddip') IS NULL ALTER TABLE [GNImage] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[GNImage]', N'utaddsessionid') IS NULL ALTER TABLE [GNImage] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[GNImage]', N'utaddsystem') IS NULL ALTER TABLE [GNImage] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[GNImage]', N'utaddtime') IS NULL ALTER TABLE [GNImage] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[GNImage]', N'utadduser') IS NULL ALTER TABLE [GNImage] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[GNImage]', N'utdoctype') IS NULL ALTER TABLE [GNImage] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[GNImage]', N'uteditappversion') IS NULL ALTER TABLE [GNImage] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[GNImage]', N'uteditdbversion') IS NULL ALTER TABLE [GNImage] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[GNImage]', N'uteditid') IS NULL ALTER TABLE [GNImage] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[GNImage]', N'uteditip') IS NULL ALTER TABLE [GNImage] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[GNImage]', N'uteditsessionid') IS NULL ALTER TABLE [GNImage] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[GNImage]', N'uteditsystem') IS NULL ALTER TABLE [GNImage] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[GNImage]', N'utedittime') IS NULL ALTER TABLE [GNImage] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[GNImage]', N'utedituser') IS NULL ALTER TABLE [GNImage] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[GNImage]', N'utsubdoctype') IS NULL ALTER TABLE [GNImage] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[HRDailyAttendance]', N'U') IS NULL BEGIN CREATE TABLE [HRDailyAttendance] (
    [Id] bigint NOT NULL IDENTITY,
    [employeeid] int NULL,
    [atndate] datetime2 NULL,
    [entityid] int NULL,
    [isabsent] bit NOT NULL,
    [utblock] bit NOT NULL,
    CONSTRAINT [PK_HRDailyAttendance] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[HRDailyAttendance]', N'Id') IS NULL ALTER TABLE [HRDailyAttendance] ADD [Id] bigint NULL ;
GO
IF COL_LENGTH(N'[HRDailyAttendance]', N'employeeid') IS NULL ALTER TABLE [HRDailyAttendance] ADD [employeeid] int NULL;
GO
IF COL_LENGTH(N'[HRDailyAttendance]', N'atndate') IS NULL ALTER TABLE [HRDailyAttendance] ADD [atndate] datetime2 NULL;
GO
IF COL_LENGTH(N'[HRDailyAttendance]', N'entityid') IS NULL ALTER TABLE [HRDailyAttendance] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[HRDailyAttendance]', N'isabsent') IS NULL ALTER TABLE [HRDailyAttendance] ADD [isabsent] bit NULL;
GO
IF COL_LENGTH(N'[HRDailyAttendance]', N'utblock') IS NULL ALTER TABLE [HRDailyAttendance] ADD [utblock] bit NULL;
GO
IF OBJECT_ID(N'[lov]', N'U') IS NULL BEGIN CREATE TABLE [lov] (
    [Id] int NOT NULL,
    [code] nvarchar(20) NULL,
    [stxt] nvarchar(200) NULL,
    [ltxt] nvarchar(500) NULL,
    [plovid] int NULL,
    [vtype] nvarchar(20) NULL,
    [utblock] bit NOT NULL,
    [indx] int NULL,
    CONSTRAINT [PK_lov] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[lov]', N'Id') IS NULL ALTER TABLE [lov] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[lov]', N'code') IS NULL ALTER TABLE [lov] ADD [code] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[lov]', N'stxt') IS NULL ALTER TABLE [lov] ADD [stxt] nvarchar(200) NULL;
GO
IF COL_LENGTH(N'[lov]', N'ltxt') IS NULL ALTER TABLE [lov] ADD [ltxt] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[lov]', N'plovid') IS NULL ALTER TABLE [lov] ADD [plovid] int NULL;
GO
IF COL_LENGTH(N'[lov]', N'vtype') IS NULL ALTER TABLE [lov] ADD [vtype] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[lov]', N'utblock') IS NULL ALTER TABLE [lov] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[lov]', N'indx') IS NULL ALTER TABLE [lov] ADD [indx] int NULL;
GO
IF OBJECT_ID(N'[Occupation]', N'U') IS NULL BEGIN CREATE TABLE [Occupation] (
    [Id] int NOT NULL,
    [clientid] int NULL,
    [code] nvarchar(10) NULL,
    [stxt] nvarchar(150) NULL,
    [ltxt] nvarchar(500) NULL,
    [utblock] bit NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    CONSTRAINT [PK_Occupation] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[Occupation]', N'Id') IS NULL ALTER TABLE [Occupation] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[Occupation]', N'clientid') IS NULL ALTER TABLE [Occupation] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[Occupation]', N'code') IS NULL ALTER TABLE [Occupation] ADD [code] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[Occupation]', N'stxt') IS NULL ALTER TABLE [Occupation] ADD [stxt] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[Occupation]', N'ltxt') IS NULL ALTER TABLE [Occupation] ADD [ltxt] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[Occupation]', N'utblock') IS NULL ALTER TABLE [Occupation] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[Occupation]', N'utaddappversion') IS NULL ALTER TABLE [Occupation] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[Occupation]', N'utadddbversion') IS NULL ALTER TABLE [Occupation] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[Occupation]', N'utaddid') IS NULL ALTER TABLE [Occupation] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[Occupation]', N'utaddip') IS NULL ALTER TABLE [Occupation] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[Occupation]', N'utaddsessionid') IS NULL ALTER TABLE [Occupation] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[Occupation]', N'utaddsystem') IS NULL ALTER TABLE [Occupation] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[Occupation]', N'utaddtime') IS NULL ALTER TABLE [Occupation] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[Occupation]', N'utadduser') IS NULL ALTER TABLE [Occupation] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[Occupation]', N'utdoctype') IS NULL ALTER TABLE [Occupation] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[Occupation]', N'uteditappversion') IS NULL ALTER TABLE [Occupation] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[Occupation]', N'uteditdbversion') IS NULL ALTER TABLE [Occupation] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[Occupation]', N'uteditid') IS NULL ALTER TABLE [Occupation] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[Occupation]', N'uteditip') IS NULL ALTER TABLE [Occupation] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[Occupation]', N'uteditsessionid') IS NULL ALTER TABLE [Occupation] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[Occupation]', N'uteditsystem') IS NULL ALTER TABLE [Occupation] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[Occupation]', N'utedittime') IS NULL ALTER TABLE [Occupation] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[Occupation]', N'utedituser') IS NULL ALTER TABLE [Occupation] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[Occupation]', N'utispartial') IS NULL ALTER TABLE [Occupation] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[Occupation]', N'utrecordversion') IS NULL ALTER TABLE [Occupation] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[Occupation]', N'utsubdoctype') IS NULL ALTER TABLE [Occupation] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[Occupation]', N'utsysgen') IS NULL ALTER TABLE [Occupation] ADD [utsysgen] bit NULL;
GO
IF OBJECT_ID(N'[OrgBuilding]', N'U') IS NULL BEGIN CREATE TABLE [OrgBuilding] (
    [Id] int NOT NULL,
    [code] nvarchar(10) NULL,
    [stxt] nvarchar(100) NULL,
    [entityid] int NULL,
    [clientid] int NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_OrgBuilding] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[OrgBuilding]', N'Id') IS NULL ALTER TABLE [OrgBuilding] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[OrgBuilding]', N'code') IS NULL ALTER TABLE [OrgBuilding] ADD [code] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[OrgBuilding]', N'stxt') IS NULL ALTER TABLE [OrgBuilding] ADD [stxt] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[OrgBuilding]', N'entityid') IS NULL ALTER TABLE [OrgBuilding] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[OrgBuilding]', N'clientid') IS NULL ALTER TABLE [OrgBuilding] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[OrgBuilding]', N'utsysgen') IS NULL ALTER TABLE [OrgBuilding] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[OrgBuilding]', N'utblock') IS NULL ALTER TABLE [OrgBuilding] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[OrgBuilding]', N'utispartial') IS NULL ALTER TABLE [OrgBuilding] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[OrgBuilding]', N'utrecordversion') IS NULL ALTER TABLE [OrgBuilding] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[OrgBuilding]', N'utaddappversion') IS NULL ALTER TABLE [OrgBuilding] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[OrgBuilding]', N'utadddbversion') IS NULL ALTER TABLE [OrgBuilding] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[OrgBuilding]', N'utaddid') IS NULL ALTER TABLE [OrgBuilding] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[OrgBuilding]', N'utaddip') IS NULL ALTER TABLE [OrgBuilding] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgBuilding]', N'utaddsessionid') IS NULL ALTER TABLE [OrgBuilding] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[OrgBuilding]', N'utaddsystem') IS NULL ALTER TABLE [OrgBuilding] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgBuilding]', N'utaddtime') IS NULL ALTER TABLE [OrgBuilding] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgBuilding]', N'utadduser') IS NULL ALTER TABLE [OrgBuilding] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgBuilding]', N'utdoctype') IS NULL ALTER TABLE [OrgBuilding] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[OrgBuilding]', N'uteditappversion') IS NULL ALTER TABLE [OrgBuilding] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[OrgBuilding]', N'uteditdbversion') IS NULL ALTER TABLE [OrgBuilding] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[OrgBuilding]', N'uteditid') IS NULL ALTER TABLE [OrgBuilding] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[OrgBuilding]', N'uteditip') IS NULL ALTER TABLE [OrgBuilding] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgBuilding]', N'uteditsessionid') IS NULL ALTER TABLE [OrgBuilding] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[OrgBuilding]', N'uteditsystem') IS NULL ALTER TABLE [OrgBuilding] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgBuilding]', N'utedittime') IS NULL ALTER TABLE [OrgBuilding] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgBuilding]', N'utedituser') IS NULL ALTER TABLE [OrgBuilding] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgBuilding]', N'utsubdoctype') IS NULL ALTER TABLE [OrgBuilding] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[OrgBusinessUnits]', N'U') IS NULL BEGIN CREATE TABLE [OrgBusinessUnits] (
    [BusinessUnitId] int NOT NULL,
    [ClientId] int NOT NULL,
    [BusinessUnitCode] nvarchar(20) NULL,
    [BusinessUnitName] nvarchar(150) NULL,
    [BusinessUnitType] nvarchar(50) NULL,
    [Description] nvarchar(500) NULL,
    [HeadName] nvarchar(100) NULL,
    [HeadEmail] nvarchar(100) NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedBy] int NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [UpdatedBy] int NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_OrgBusinessUnits] PRIMARY KEY ([BusinessUnitId])
); END;
GO
IF COL_LENGTH(N'[OrgBusinessUnits]', N'BusinessUnitId') IS NULL ALTER TABLE [OrgBusinessUnits] ADD [BusinessUnitId] int NULL;
GO
IF COL_LENGTH(N'[OrgBusinessUnits]', N'ClientId') IS NULL ALTER TABLE [OrgBusinessUnits] ADD [ClientId] int NULL;
GO
IF COL_LENGTH(N'[OrgBusinessUnits]', N'BusinessUnitCode') IS NULL ALTER TABLE [OrgBusinessUnits] ADD [BusinessUnitCode] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[OrgBusinessUnits]', N'BusinessUnitName') IS NULL ALTER TABLE [OrgBusinessUnits] ADD [BusinessUnitName] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[OrgBusinessUnits]', N'BusinessUnitType') IS NULL ALTER TABLE [OrgBusinessUnits] ADD [BusinessUnitType] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[OrgBusinessUnits]', N'Description') IS NULL ALTER TABLE [OrgBusinessUnits] ADD [Description] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[OrgBusinessUnits]', N'HeadName') IS NULL ALTER TABLE [OrgBusinessUnits] ADD [HeadName] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[OrgBusinessUnits]', N'HeadEmail') IS NULL ALTER TABLE [OrgBusinessUnits] ADD [HeadEmail] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[OrgBusinessUnits]', N'IsActive') IS NULL ALTER TABLE [OrgBusinessUnits] ADD [IsActive] bit NULL;
GO
IF COL_LENGTH(N'[OrgBusinessUnits]', N'CreatedAt') IS NULL ALTER TABLE [OrgBusinessUnits] ADD [CreatedAt] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgBusinessUnits]', N'CreatedBy') IS NULL ALTER TABLE [OrgBusinessUnits] ADD [CreatedBy] int NULL;
GO
IF COL_LENGTH(N'[OrgBusinessUnits]', N'UpdatedAt') IS NULL ALTER TABLE [OrgBusinessUnits] ADD [UpdatedAt] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgBusinessUnits]', N'UpdatedBy') IS NULL ALTER TABLE [OrgBusinessUnits] ADD [UpdatedBy] int NULL;
GO
IF COL_LENGTH(N'[OrgBusinessUnits]', N'IsDeleted') IS NULL ALTER TABLE [OrgBusinessUnits] ADD [IsDeleted] bit NULL;
GO
IF OBJECT_ID(N'[OrgCastes]', N'U') IS NULL BEGIN CREATE TABLE [OrgCastes] (
    [CasteId] int NOT NULL,
    [CasteName] nvarchar(100) NULL,
    [CasteCode] nvarchar(20) NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedBy] int NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [UpdatedBy] int NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_OrgCastes] PRIMARY KEY ([CasteId])
); END;
GO
IF COL_LENGTH(N'[OrgCastes]', N'CasteId') IS NULL ALTER TABLE [OrgCastes] ADD [CasteId] int NULL;
GO
IF COL_LENGTH(N'[OrgCastes]', N'CasteName') IS NULL ALTER TABLE [OrgCastes] ADD [CasteName] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[OrgCastes]', N'CasteCode') IS NULL ALTER TABLE [OrgCastes] ADD [CasteCode] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[OrgCastes]', N'IsActive') IS NULL ALTER TABLE [OrgCastes] ADD [IsActive] bit NULL;
GO
IF COL_LENGTH(N'[OrgCastes]', N'CreatedAt') IS NULL ALTER TABLE [OrgCastes] ADD [CreatedAt] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgCastes]', N'CreatedBy') IS NULL ALTER TABLE [OrgCastes] ADD [CreatedBy] int NULL;
GO
IF COL_LENGTH(N'[OrgCastes]', N'UpdatedAt') IS NULL ALTER TABLE [OrgCastes] ADD [UpdatedAt] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgCastes]', N'UpdatedBy') IS NULL ALTER TABLE [OrgCastes] ADD [UpdatedBy] int NULL;
GO
IF COL_LENGTH(N'[OrgCastes]', N'IsDeleted') IS NULL ALTER TABLE [OrgCastes] ADD [IsDeleted] bit NULL;
GO
IF OBJECT_ID(N'[OrgCostCenters]', N'U') IS NULL BEGIN CREATE TABLE [OrgCostCenters] (
    [CostCenterId] int NOT NULL,
    [ClientId] int NOT NULL,
    [CostCenterCode] nvarchar(20) NULL,
    [CostCenterName] nvarchar(150) NULL,
    [Description] nvarchar(500) NULL,
    [BudgetAmount] decimal(18,2) NULL,
    [ManagerName] nvarchar(100) NULL,
    [ManagerEmail] nvarchar(100) NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedBy] int NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [UpdatedBy] int NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_OrgCostCenters] PRIMARY KEY ([CostCenterId])
); END;
GO
IF COL_LENGTH(N'[OrgCostCenters]', N'CostCenterId') IS NULL ALTER TABLE [OrgCostCenters] ADD [CostCenterId] int NULL;
GO
IF COL_LENGTH(N'[OrgCostCenters]', N'ClientId') IS NULL ALTER TABLE [OrgCostCenters] ADD [ClientId] int NULL;
GO
IF COL_LENGTH(N'[OrgCostCenters]', N'CostCenterCode') IS NULL ALTER TABLE [OrgCostCenters] ADD [CostCenterCode] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[OrgCostCenters]', N'CostCenterName') IS NULL ALTER TABLE [OrgCostCenters] ADD [CostCenterName] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[OrgCostCenters]', N'Description') IS NULL ALTER TABLE [OrgCostCenters] ADD [Description] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[OrgCostCenters]', N'BudgetAmount') IS NULL ALTER TABLE [OrgCostCenters] ADD [BudgetAmount] decimal(18,2) NULL;
GO
IF COL_LENGTH(N'[OrgCostCenters]', N'ManagerName') IS NULL ALTER TABLE [OrgCostCenters] ADD [ManagerName] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[OrgCostCenters]', N'ManagerEmail') IS NULL ALTER TABLE [OrgCostCenters] ADD [ManagerEmail] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[OrgCostCenters]', N'IsActive') IS NULL ALTER TABLE [OrgCostCenters] ADD [IsActive] bit NULL;
GO
IF COL_LENGTH(N'[OrgCostCenters]', N'CreatedAt') IS NULL ALTER TABLE [OrgCostCenters] ADD [CreatedAt] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgCostCenters]', N'CreatedBy') IS NULL ALTER TABLE [OrgCostCenters] ADD [CreatedBy] int NULL;
GO
IF COL_LENGTH(N'[OrgCostCenters]', N'UpdatedAt') IS NULL ALTER TABLE [OrgCostCenters] ADD [UpdatedAt] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgCostCenters]', N'UpdatedBy') IS NULL ALTER TABLE [OrgCostCenters] ADD [UpdatedBy] int NULL;
GO
IF COL_LENGTH(N'[OrgCostCenters]', N'IsDeleted') IS NULL ALTER TABLE [OrgCostCenters] ADD [IsDeleted] bit NULL;
GO
IF OBJECT_ID(N'[OrgDesignation]', N'U') IS NULL BEGIN CREATE TABLE [OrgDesignation] (
    [Id] int NOT NULL,
    [desgcode] nvarchar(5) NULL,
    [stxt] nvarchar(50) NULL,
    [ltxt] nvarchar(500) NULL,
    [blank] bit NULL,
    [clientid] int NULL,
    [utblock] bit NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    CONSTRAINT [PK_OrgDesignation] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[OrgDesignation]', N'Id') IS NULL ALTER TABLE [OrgDesignation] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[OrgDesignation]', N'desgcode') IS NULL ALTER TABLE [OrgDesignation] ADD [desgcode] nvarchar(5) NULL;
GO
IF COL_LENGTH(N'[OrgDesignation]', N'stxt') IS NULL ALTER TABLE [OrgDesignation] ADD [stxt] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[OrgDesignation]', N'ltxt') IS NULL ALTER TABLE [OrgDesignation] ADD [ltxt] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[OrgDesignation]', N'blank') IS NULL ALTER TABLE [OrgDesignation] ADD [blank] bit NULL;
GO
IF COL_LENGTH(N'[OrgDesignation]', N'clientid') IS NULL ALTER TABLE [OrgDesignation] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[OrgDesignation]', N'utblock') IS NULL ALTER TABLE [OrgDesignation] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[OrgDesignation]', N'utaddappversion') IS NULL ALTER TABLE [OrgDesignation] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[OrgDesignation]', N'utadddbversion') IS NULL ALTER TABLE [OrgDesignation] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[OrgDesignation]', N'utaddid') IS NULL ALTER TABLE [OrgDesignation] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[OrgDesignation]', N'utaddip') IS NULL ALTER TABLE [OrgDesignation] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgDesignation]', N'utaddsessionid') IS NULL ALTER TABLE [OrgDesignation] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[OrgDesignation]', N'utaddsystem') IS NULL ALTER TABLE [OrgDesignation] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgDesignation]', N'utaddtime') IS NULL ALTER TABLE [OrgDesignation] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgDesignation]', N'utadduser') IS NULL ALTER TABLE [OrgDesignation] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgDesignation]', N'utdoctype') IS NULL ALTER TABLE [OrgDesignation] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[OrgDesignation]', N'uteditappversion') IS NULL ALTER TABLE [OrgDesignation] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[OrgDesignation]', N'uteditdbversion') IS NULL ALTER TABLE [OrgDesignation] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[OrgDesignation]', N'uteditid') IS NULL ALTER TABLE [OrgDesignation] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[OrgDesignation]', N'uteditip') IS NULL ALTER TABLE [OrgDesignation] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgDesignation]', N'uteditsessionid') IS NULL ALTER TABLE [OrgDesignation] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[OrgDesignation]', N'uteditsystem') IS NULL ALTER TABLE [OrgDesignation] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgDesignation]', N'utedittime') IS NULL ALTER TABLE [OrgDesignation] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgDesignation]', N'utedituser') IS NULL ALTER TABLE [OrgDesignation] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgDesignation]', N'utispartial') IS NULL ALTER TABLE [OrgDesignation] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[OrgDesignation]', N'utrecordversion') IS NULL ALTER TABLE [OrgDesignation] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[OrgDesignation]', N'utsubdoctype') IS NULL ALTER TABLE [OrgDesignation] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[OrgDesignation]', N'utsysgen') IS NULL ALTER TABLE [OrgDesignation] ADD [utsysgen] bit NULL;
GO
IF OBJECT_ID(N'[OrgDonor]', N'U') IS NULL BEGIN CREATE TABLE [OrgDonor] (
    [Id] int NOT NULL,
    [code] nvarchar(10) NULL,
    [stxt] nvarchar(50) NULL,
    [ltxt] nvarchar(50) NULL,
    [contactno] nvarchar(50) NULL,
    [clientid] int NULL,
    [entityid] int NULL,
    [utblock] bit NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    CONSTRAINT [PK_OrgDonor] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[OrgDonor]', N'Id') IS NULL ALTER TABLE [OrgDonor] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[OrgDonor]', N'code') IS NULL ALTER TABLE [OrgDonor] ADD [code] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[OrgDonor]', N'stxt') IS NULL ALTER TABLE [OrgDonor] ADD [stxt] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[OrgDonor]', N'ltxt') IS NULL ALTER TABLE [OrgDonor] ADD [ltxt] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[OrgDonor]', N'contactno') IS NULL ALTER TABLE [OrgDonor] ADD [contactno] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[OrgDonor]', N'clientid') IS NULL ALTER TABLE [OrgDonor] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[OrgDonor]', N'entityid') IS NULL ALTER TABLE [OrgDonor] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[OrgDonor]', N'utblock') IS NULL ALTER TABLE [OrgDonor] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[OrgDonor]', N'utaddappversion') IS NULL ALTER TABLE [OrgDonor] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[OrgDonor]', N'utadddbversion') IS NULL ALTER TABLE [OrgDonor] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[OrgDonor]', N'utaddid') IS NULL ALTER TABLE [OrgDonor] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[OrgDonor]', N'utaddip') IS NULL ALTER TABLE [OrgDonor] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgDonor]', N'utaddsessionid') IS NULL ALTER TABLE [OrgDonor] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[OrgDonor]', N'utaddsystem') IS NULL ALTER TABLE [OrgDonor] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgDonor]', N'utaddtime') IS NULL ALTER TABLE [OrgDonor] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgDonor]', N'utadduser') IS NULL ALTER TABLE [OrgDonor] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgDonor]', N'utdoctype') IS NULL ALTER TABLE [OrgDonor] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[OrgDonor]', N'uteditappversion') IS NULL ALTER TABLE [OrgDonor] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[OrgDonor]', N'uteditdbversion') IS NULL ALTER TABLE [OrgDonor] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[OrgDonor]', N'uteditid') IS NULL ALTER TABLE [OrgDonor] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[OrgDonor]', N'uteditip') IS NULL ALTER TABLE [OrgDonor] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgDonor]', N'uteditsessionid') IS NULL ALTER TABLE [OrgDonor] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[OrgDonor]', N'uteditsystem') IS NULL ALTER TABLE [OrgDonor] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgDonor]', N'utedittime') IS NULL ALTER TABLE [OrgDonor] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgDonor]', N'utedituser') IS NULL ALTER TABLE [OrgDonor] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgDonor]', N'utispartial') IS NULL ALTER TABLE [OrgDonor] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[OrgDonor]', N'utrecordversion') IS NULL ALTER TABLE [OrgDonor] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[OrgDonor]', N'utsubdoctype') IS NULL ALTER TABLE [OrgDonor] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[OrgDonor]', N'utsysgen') IS NULL ALTER TABLE [OrgDonor] ADD [utsysgen] bit NULL;
GO
IF OBJECT_ID(N'[ORGentitycontact]', N'U') IS NULL BEGIN CREATE TABLE [ORGentitycontact] (
    [id] int NOT NULL,
    [entityid] int NULL,
    [stxt] nvarchar(200) NULL,
    [cntypid] int NULL,
    [value] nvarchar(200) NULL,
    [lineindex] int NULL,
    [utblock] bit NOT NULL,
    [utsysgen] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_ORGentitycontact] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[ORGentitycontact]', N'id') IS NULL ALTER TABLE [ORGentitycontact] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[ORGentitycontact]', N'entityid') IS NULL ALTER TABLE [ORGentitycontact] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[ORGentitycontact]', N'stxt') IS NULL ALTER TABLE [ORGentitycontact] ADD [stxt] nvarchar(200) NULL;
GO
IF COL_LENGTH(N'[ORGentitycontact]', N'cntypid') IS NULL ALTER TABLE [ORGentitycontact] ADD [cntypid] int NULL;
GO
IF COL_LENGTH(N'[ORGentitycontact]', N'value') IS NULL ALTER TABLE [ORGentitycontact] ADD [value] nvarchar(200) NULL;
GO
IF COL_LENGTH(N'[ORGentitycontact]', N'lineindex') IS NULL ALTER TABLE [ORGentitycontact] ADD [lineindex] int NULL;
GO
IF COL_LENGTH(N'[ORGentitycontact]', N'utblock') IS NULL ALTER TABLE [ORGentitycontact] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[ORGentitycontact]', N'utsysgen') IS NULL ALTER TABLE [ORGentitycontact] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[ORGentitycontact]', N'utispartial') IS NULL ALTER TABLE [ORGentitycontact] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[ORGentitycontact]', N'utrecordversion') IS NULL ALTER TABLE [ORGentitycontact] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[ORGentitycontact]', N'utaddappversion') IS NULL ALTER TABLE [ORGentitycontact] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ORGentitycontact]', N'utadddbversion') IS NULL ALTER TABLE [ORGentitycontact] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ORGentitycontact]', N'utaddid') IS NULL ALTER TABLE [ORGentitycontact] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[ORGentitycontact]', N'utaddip') IS NULL ALTER TABLE [ORGentitycontact] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ORGentitycontact]', N'utaddsessionid') IS NULL ALTER TABLE [ORGentitycontact] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[ORGentitycontact]', N'utaddsystem') IS NULL ALTER TABLE [ORGentitycontact] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ORGentitycontact]', N'utaddtime') IS NULL ALTER TABLE [ORGentitycontact] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ORGentitycontact]', N'utadduser') IS NULL ALTER TABLE [ORGentitycontact] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ORGentitycontact]', N'utdoctype') IS NULL ALTER TABLE [ORGentitycontact] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[ORGentitycontact]', N'uteditappversion') IS NULL ALTER TABLE [ORGentitycontact] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ORGentitycontact]', N'uteditdbversion') IS NULL ALTER TABLE [ORGentitycontact] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[ORGentitycontact]', N'uteditid') IS NULL ALTER TABLE [ORGentitycontact] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[ORGentitycontact]', N'uteditip') IS NULL ALTER TABLE [ORGentitycontact] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ORGentitycontact]', N'uteditsessionid') IS NULL ALTER TABLE [ORGentitycontact] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[ORGentitycontact]', N'uteditsystem') IS NULL ALTER TABLE [ORGentitycontact] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ORGentitycontact]', N'utedittime') IS NULL ALTER TABLE [ORGentitycontact] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[ORGentitycontact]', N'utedituser') IS NULL ALTER TABLE [ORGentitycontact] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[ORGentitycontact]', N'utsubdoctype') IS NULL ALTER TABLE [ORGentitycontact] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[OrgFloor]', N'U') IS NULL BEGIN CREATE TABLE [OrgFloor] (
    [Id] int NOT NULL,
    [code] nvarchar(10) NULL,
    [stxt] nvarchar(100) NULL,
    [entityid] int NULL,
    [clientid] int NULL,
    [buildingid] int NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_OrgFloor] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[OrgFloor]', N'Id') IS NULL ALTER TABLE [OrgFloor] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[OrgFloor]', N'code') IS NULL ALTER TABLE [OrgFloor] ADD [code] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[OrgFloor]', N'stxt') IS NULL ALTER TABLE [OrgFloor] ADD [stxt] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[OrgFloor]', N'entityid') IS NULL ALTER TABLE [OrgFloor] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[OrgFloor]', N'clientid') IS NULL ALTER TABLE [OrgFloor] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[OrgFloor]', N'buildingid') IS NULL ALTER TABLE [OrgFloor] ADD [buildingid] int NULL;
GO
IF COL_LENGTH(N'[OrgFloor]', N'utsysgen') IS NULL ALTER TABLE [OrgFloor] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[OrgFloor]', N'utblock') IS NULL ALTER TABLE [OrgFloor] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[OrgFloor]', N'utispartial') IS NULL ALTER TABLE [OrgFloor] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[OrgFloor]', N'utrecordversion') IS NULL ALTER TABLE [OrgFloor] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[OrgFloor]', N'utaddappversion') IS NULL ALTER TABLE [OrgFloor] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[OrgFloor]', N'utadddbversion') IS NULL ALTER TABLE [OrgFloor] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[OrgFloor]', N'utaddid') IS NULL ALTER TABLE [OrgFloor] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[OrgFloor]', N'utaddip') IS NULL ALTER TABLE [OrgFloor] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgFloor]', N'utaddsessionid') IS NULL ALTER TABLE [OrgFloor] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[OrgFloor]', N'utaddsystem') IS NULL ALTER TABLE [OrgFloor] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgFloor]', N'utaddtime') IS NULL ALTER TABLE [OrgFloor] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgFloor]', N'utadduser') IS NULL ALTER TABLE [OrgFloor] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgFloor]', N'utdoctype') IS NULL ALTER TABLE [OrgFloor] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[OrgFloor]', N'uteditappversion') IS NULL ALTER TABLE [OrgFloor] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[OrgFloor]', N'uteditdbversion') IS NULL ALTER TABLE [OrgFloor] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[OrgFloor]', N'uteditid') IS NULL ALTER TABLE [OrgFloor] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[OrgFloor]', N'uteditip') IS NULL ALTER TABLE [OrgFloor] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgFloor]', N'uteditsessionid') IS NULL ALTER TABLE [OrgFloor] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[OrgFloor]', N'uteditsystem') IS NULL ALTER TABLE [OrgFloor] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgFloor]', N'utedittime') IS NULL ALTER TABLE [OrgFloor] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgFloor]', N'utedituser') IS NULL ALTER TABLE [OrgFloor] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgFloor]', N'utsubdoctype') IS NULL ALTER TABLE [OrgFloor] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[OrgLanguages]', N'U') IS NULL BEGIN CREATE TABLE [OrgLanguages] (
    [LanguageId] int NOT NULL,
    [LanguageName] nvarchar(100) NULL,
    [LanguageCode] nvarchar(5) NULL,
    [IsDefault] bit NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedBy] int NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [UpdatedBy] int NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_OrgLanguages] PRIMARY KEY ([LanguageId])
); END;
GO
IF COL_LENGTH(N'[OrgLanguages]', N'LanguageId') IS NULL ALTER TABLE [OrgLanguages] ADD [LanguageId] int NULL;
GO
IF COL_LENGTH(N'[OrgLanguages]', N'LanguageName') IS NULL ALTER TABLE [OrgLanguages] ADD [LanguageName] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[OrgLanguages]', N'LanguageCode') IS NULL ALTER TABLE [OrgLanguages] ADD [LanguageCode] nvarchar(5) NULL;
GO
IF COL_LENGTH(N'[OrgLanguages]', N'IsDefault') IS NULL ALTER TABLE [OrgLanguages] ADD [IsDefault] bit NULL;
GO
IF COL_LENGTH(N'[OrgLanguages]', N'IsActive') IS NULL ALTER TABLE [OrgLanguages] ADD [IsActive] bit NULL;
GO
IF COL_LENGTH(N'[OrgLanguages]', N'CreatedAt') IS NULL ALTER TABLE [OrgLanguages] ADD [CreatedAt] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgLanguages]', N'CreatedBy') IS NULL ALTER TABLE [OrgLanguages] ADD [CreatedBy] int NULL;
GO
IF COL_LENGTH(N'[OrgLanguages]', N'UpdatedAt') IS NULL ALTER TABLE [OrgLanguages] ADD [UpdatedAt] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgLanguages]', N'UpdatedBy') IS NULL ALTER TABLE [OrgLanguages] ADD [UpdatedBy] int NULL;
GO
IF COL_LENGTH(N'[OrgLanguages]', N'IsDeleted') IS NULL ALTER TABLE [OrgLanguages] ADD [IsDeleted] bit NULL;
GO
IF OBJECT_ID(N'[OrgLegalentity]', N'U') IS NULL BEGIN CREATE TABLE [OrgLegalentity] (
    [Id] int NOT NULL,
    [entityno] nvarchar(20) NULL,
    [clientid] int NULL,
    [stxt] nvarchar(500) NULL,
    [shortname] nvarchar(50) NULL,
    [estdate] datetime2 NULL,
    [contactperson] nvarchar(200) NULL,
    [contactno] nvarchar(50) NULL,
    [address] nvarchar(500) NULL,
    [countryid] int NULL,
    [stateid] int NULL,
    [cityid] int NULL,
    [regionid] int NULL,
    [latitude] float NULL,
    [longitude] float NULL,
    [lovorgtypeid] int NULL,
    [lovcampustypeid] int NULL,
    [orgstatusid] int NULL,
    [registerid] int NULL,
    [slogan] nvarchar(500) NULL,
    [institutiontypeid] int NULL,
    [entitytypeid] int NULL,
    [imageid] bigint NULL,
    [watermarklogo] bigint NULL,
    [utblock] bit NOT NULL,
    [utsysgen] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_OrgLegalentity] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'Id') IS NULL ALTER TABLE [OrgLegalentity] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'entityno') IS NULL ALTER TABLE [OrgLegalentity] ADD [entityno] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'clientid') IS NULL ALTER TABLE [OrgLegalentity] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'stxt') IS NULL ALTER TABLE [OrgLegalentity] ADD [stxt] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'shortname') IS NULL ALTER TABLE [OrgLegalentity] ADD [shortname] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'estdate') IS NULL ALTER TABLE [OrgLegalentity] ADD [estdate] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'contactperson') IS NULL ALTER TABLE [OrgLegalentity] ADD [contactperson] nvarchar(200) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'contactno') IS NULL ALTER TABLE [OrgLegalentity] ADD [contactno] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'address') IS NULL ALTER TABLE [OrgLegalentity] ADD [address] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'countryid') IS NULL ALTER TABLE [OrgLegalentity] ADD [countryid] int NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'stateid') IS NULL ALTER TABLE [OrgLegalentity] ADD [stateid] int NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'cityid') IS NULL ALTER TABLE [OrgLegalentity] ADD [cityid] int NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'regionid') IS NULL ALTER TABLE [OrgLegalentity] ADD [regionid] int NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'latitude') IS NULL ALTER TABLE [OrgLegalentity] ADD [latitude] float NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'longitude') IS NULL ALTER TABLE [OrgLegalentity] ADD [longitude] float NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'lovorgtypeid') IS NULL ALTER TABLE [OrgLegalentity] ADD [lovorgtypeid] int NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'lovcampustypeid') IS NULL ALTER TABLE [OrgLegalentity] ADD [lovcampustypeid] int NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'orgstatusid') IS NULL ALTER TABLE [OrgLegalentity] ADD [orgstatusid] int NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'registerid') IS NULL ALTER TABLE [OrgLegalentity] ADD [registerid] int NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'slogan') IS NULL ALTER TABLE [OrgLegalentity] ADD [slogan] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'institutiontypeid') IS NULL ALTER TABLE [OrgLegalentity] ADD [institutiontypeid] int NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'entitytypeid') IS NULL ALTER TABLE [OrgLegalentity] ADD [entitytypeid] int NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'imageid') IS NULL ALTER TABLE [OrgLegalentity] ADD [imageid] bigint NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'watermarklogo') IS NULL ALTER TABLE [OrgLegalentity] ADD [watermarklogo] bigint NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'utblock') IS NULL ALTER TABLE [OrgLegalentity] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'utsysgen') IS NULL ALTER TABLE [OrgLegalentity] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'utispartial') IS NULL ALTER TABLE [OrgLegalentity] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'utrecordversion') IS NULL ALTER TABLE [OrgLegalentity] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'utaddappversion') IS NULL ALTER TABLE [OrgLegalentity] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'utadddbversion') IS NULL ALTER TABLE [OrgLegalentity] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'utaddid') IS NULL ALTER TABLE [OrgLegalentity] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'utaddip') IS NULL ALTER TABLE [OrgLegalentity] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'utaddsessionid') IS NULL ALTER TABLE [OrgLegalentity] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'utaddsystem') IS NULL ALTER TABLE [OrgLegalentity] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'utaddtime') IS NULL ALTER TABLE [OrgLegalentity] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'utadduser') IS NULL ALTER TABLE [OrgLegalentity] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'utdoctype') IS NULL ALTER TABLE [OrgLegalentity] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'uteditappversion') IS NULL ALTER TABLE [OrgLegalentity] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'uteditdbversion') IS NULL ALTER TABLE [OrgLegalentity] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'uteditid') IS NULL ALTER TABLE [OrgLegalentity] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'uteditip') IS NULL ALTER TABLE [OrgLegalentity] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'uteditsessionid') IS NULL ALTER TABLE [OrgLegalentity] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'uteditsystem') IS NULL ALTER TABLE [OrgLegalentity] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'utedittime') IS NULL ALTER TABLE [OrgLegalentity] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'utedituser') IS NULL ALTER TABLE [OrgLegalentity] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentity]', N'utsubdoctype') IS NULL ALTER TABLE [OrgLegalentity] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[OrgLegalentitydtl]', N'U') IS NULL BEGIN CREATE TABLE [OrgLegalentitydtl] (
    [Id] int NOT NULL,
    [entityid] int NULL,
    [lineindex] int NULL,
    [lovtypeid] int NULL,
    [employeeid] int NULL,
    [utblock] bit NOT NULL,
    [utsysgen] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utaddappversion] nvarchar(100) NULL,
    [utadddbversion] nvarchar(100) NULL,
    [utaddid] bigint NULL,
    [utaddip] nvarchar(100) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] nvarchar(100) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] nvarchar(100) NULL,
    [utdoctype] int NULL,
    [uteditappversion] nvarchar(50) NULL,
    [uteditdbversion] nvarchar(50) NULL,
    [uteditid] bigint NULL,
    [uteditip] nvarchar(50) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] nvarchar(50) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] nvarchar(50) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_OrgLegalentitydtl] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[OrgLegalentitydtl]', N'Id') IS NULL ALTER TABLE [OrgLegalentitydtl] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitydtl]', N'entityid') IS NULL ALTER TABLE [OrgLegalentitydtl] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitydtl]', N'lineindex') IS NULL ALTER TABLE [OrgLegalentitydtl] ADD [lineindex] int NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitydtl]', N'lovtypeid') IS NULL ALTER TABLE [OrgLegalentitydtl] ADD [lovtypeid] int NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitydtl]', N'employeeid') IS NULL ALTER TABLE [OrgLegalentitydtl] ADD [employeeid] int NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitydtl]', N'utblock') IS NULL ALTER TABLE [OrgLegalentitydtl] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitydtl]', N'utsysgen') IS NULL ALTER TABLE [OrgLegalentitydtl] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitydtl]', N'utispartial') IS NULL ALTER TABLE [OrgLegalentitydtl] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitydtl]', N'utrecordversion') IS NULL ALTER TABLE [OrgLegalentitydtl] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitydtl]', N'utaddappversion') IS NULL ALTER TABLE [OrgLegalentitydtl] ADD [utaddappversion] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitydtl]', N'utadddbversion') IS NULL ALTER TABLE [OrgLegalentitydtl] ADD [utadddbversion] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitydtl]', N'utaddid') IS NULL ALTER TABLE [OrgLegalentitydtl] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitydtl]', N'utaddip') IS NULL ALTER TABLE [OrgLegalentitydtl] ADD [utaddip] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitydtl]', N'utaddsessionid') IS NULL ALTER TABLE [OrgLegalentitydtl] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitydtl]', N'utaddsystem') IS NULL ALTER TABLE [OrgLegalentitydtl] ADD [utaddsystem] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitydtl]', N'utaddtime') IS NULL ALTER TABLE [OrgLegalentitydtl] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitydtl]', N'utadduser') IS NULL ALTER TABLE [OrgLegalentitydtl] ADD [utadduser] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitydtl]', N'utdoctype') IS NULL ALTER TABLE [OrgLegalentitydtl] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitydtl]', N'uteditappversion') IS NULL ALTER TABLE [OrgLegalentitydtl] ADD [uteditappversion] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitydtl]', N'uteditdbversion') IS NULL ALTER TABLE [OrgLegalentitydtl] ADD [uteditdbversion] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitydtl]', N'uteditid') IS NULL ALTER TABLE [OrgLegalentitydtl] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitydtl]', N'uteditip') IS NULL ALTER TABLE [OrgLegalentitydtl] ADD [uteditip] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitydtl]', N'uteditsessionid') IS NULL ALTER TABLE [OrgLegalentitydtl] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitydtl]', N'uteditsystem') IS NULL ALTER TABLE [OrgLegalentitydtl] ADD [uteditsystem] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitydtl]', N'utedittime') IS NULL ALTER TABLE [OrgLegalentitydtl] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitydtl]', N'utedituser') IS NULL ALTER TABLE [OrgLegalentitydtl] ADD [utedituser] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitydtl]', N'utsubdoctype') IS NULL ALTER TABLE [OrgLegalentitydtl] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[OrgLegalentitypvdtl]', N'U') IS NULL BEGIN CREATE TABLE [OrgLegalentitypvdtl] (
    [Id] int NOT NULL,
    [entityid] int NULL,
    [lineindex] int NULL,
    [pvendorid] int NULL,
    [prefix] nvarchar(50) NULL,
    [paymethodid] int NULL,
    [accid] int NULL,
    [isdefault] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utsysgen] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_OrgLegalentitypvdtl] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[OrgLegalentitypvdtl]', N'Id') IS NULL ALTER TABLE [OrgLegalentitypvdtl] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitypvdtl]', N'entityid') IS NULL ALTER TABLE [OrgLegalentitypvdtl] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitypvdtl]', N'lineindex') IS NULL ALTER TABLE [OrgLegalentitypvdtl] ADD [lineindex] int NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitypvdtl]', N'pvendorid') IS NULL ALTER TABLE [OrgLegalentitypvdtl] ADD [pvendorid] int NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitypvdtl]', N'prefix') IS NULL ALTER TABLE [OrgLegalentitypvdtl] ADD [prefix] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitypvdtl]', N'paymethodid') IS NULL ALTER TABLE [OrgLegalentitypvdtl] ADD [paymethodid] int NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitypvdtl]', N'accid') IS NULL ALTER TABLE [OrgLegalentitypvdtl] ADD [accid] int NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitypvdtl]', N'isdefault') IS NULL ALTER TABLE [OrgLegalentitypvdtl] ADD [isdefault] bit NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitypvdtl]', N'utblock') IS NULL ALTER TABLE [OrgLegalentitypvdtl] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitypvdtl]', N'utsysgen') IS NULL ALTER TABLE [OrgLegalentitypvdtl] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitypvdtl]', N'utispartial') IS NULL ALTER TABLE [OrgLegalentitypvdtl] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitypvdtl]', N'utrecordversion') IS NULL ALTER TABLE [OrgLegalentitypvdtl] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitypvdtl]', N'utaddappversion') IS NULL ALTER TABLE [OrgLegalentitypvdtl] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitypvdtl]', N'utadddbversion') IS NULL ALTER TABLE [OrgLegalentitypvdtl] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitypvdtl]', N'utaddid') IS NULL ALTER TABLE [OrgLegalentitypvdtl] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitypvdtl]', N'utaddip') IS NULL ALTER TABLE [OrgLegalentitypvdtl] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitypvdtl]', N'utaddsessionid') IS NULL ALTER TABLE [OrgLegalentitypvdtl] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitypvdtl]', N'utaddsystem') IS NULL ALTER TABLE [OrgLegalentitypvdtl] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitypvdtl]', N'utaddtime') IS NULL ALTER TABLE [OrgLegalentitypvdtl] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitypvdtl]', N'utadduser') IS NULL ALTER TABLE [OrgLegalentitypvdtl] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitypvdtl]', N'utdoctype') IS NULL ALTER TABLE [OrgLegalentitypvdtl] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitypvdtl]', N'uteditappversion') IS NULL ALTER TABLE [OrgLegalentitypvdtl] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitypvdtl]', N'uteditdbversion') IS NULL ALTER TABLE [OrgLegalentitypvdtl] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitypvdtl]', N'uteditid') IS NULL ALTER TABLE [OrgLegalentitypvdtl] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitypvdtl]', N'uteditip') IS NULL ALTER TABLE [OrgLegalentitypvdtl] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitypvdtl]', N'uteditsessionid') IS NULL ALTER TABLE [OrgLegalentitypvdtl] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitypvdtl]', N'uteditsystem') IS NULL ALTER TABLE [OrgLegalentitypvdtl] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitypvdtl]', N'utedittime') IS NULL ALTER TABLE [OrgLegalentitypvdtl] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitypvdtl]', N'utedituser') IS NULL ALTER TABLE [OrgLegalentitypvdtl] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgLegalentitypvdtl]', N'utsubdoctype') IS NULL ALTER TABLE [OrgLegalentitypvdtl] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[Orgroom]', N'U') IS NULL BEGIN CREATE TABLE [Orgroom] (
    [Id] int NOT NULL,
    [code] nvarchar(10) NULL,
    [stxt] nvarchar(100) NULL,
    [entityid] int NULL,
    [clientid] int NULL,
    [lovfloorid] int NULL,
    [roomtypeid] int NULL,
    [buildingid] int NULL,
    [strength] int NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_Orgroom] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[Orgroom]', N'Id') IS NULL ALTER TABLE [Orgroom] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[Orgroom]', N'code') IS NULL ALTER TABLE [Orgroom] ADD [code] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[Orgroom]', N'stxt') IS NULL ALTER TABLE [Orgroom] ADD [stxt] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[Orgroom]', N'entityid') IS NULL ALTER TABLE [Orgroom] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[Orgroom]', N'clientid') IS NULL ALTER TABLE [Orgroom] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[Orgroom]', N'lovfloorid') IS NULL ALTER TABLE [Orgroom] ADD [lovfloorid] int NULL;
GO
IF COL_LENGTH(N'[Orgroom]', N'roomtypeid') IS NULL ALTER TABLE [Orgroom] ADD [roomtypeid] int NULL;
GO
IF COL_LENGTH(N'[Orgroom]', N'buildingid') IS NULL ALTER TABLE [Orgroom] ADD [buildingid] int NULL;
GO
IF COL_LENGTH(N'[Orgroom]', N'strength') IS NULL ALTER TABLE [Orgroom] ADD [strength] int NULL;
GO
IF COL_LENGTH(N'[Orgroom]', N'utsysgen') IS NULL ALTER TABLE [Orgroom] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[Orgroom]', N'utblock') IS NULL ALTER TABLE [Orgroom] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[Orgroom]', N'utispartial') IS NULL ALTER TABLE [Orgroom] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[Orgroom]', N'utrecordversion') IS NULL ALTER TABLE [Orgroom] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[Orgroom]', N'utaddappversion') IS NULL ALTER TABLE [Orgroom] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[Orgroom]', N'utadddbversion') IS NULL ALTER TABLE [Orgroom] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[Orgroom]', N'utaddid') IS NULL ALTER TABLE [Orgroom] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[Orgroom]', N'utaddip') IS NULL ALTER TABLE [Orgroom] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[Orgroom]', N'utaddsessionid') IS NULL ALTER TABLE [Orgroom] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[Orgroom]', N'utaddsystem') IS NULL ALTER TABLE [Orgroom] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[Orgroom]', N'utaddtime') IS NULL ALTER TABLE [Orgroom] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[Orgroom]', N'utadduser') IS NULL ALTER TABLE [Orgroom] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[Orgroom]', N'utdoctype') IS NULL ALTER TABLE [Orgroom] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[Orgroom]', N'uteditappversion') IS NULL ALTER TABLE [Orgroom] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[Orgroom]', N'uteditdbversion') IS NULL ALTER TABLE [Orgroom] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[Orgroom]', N'uteditid') IS NULL ALTER TABLE [Orgroom] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[Orgroom]', N'uteditip') IS NULL ALTER TABLE [Orgroom] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[Orgroom]', N'uteditsessionid') IS NULL ALTER TABLE [Orgroom] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[Orgroom]', N'uteditsystem') IS NULL ALTER TABLE [Orgroom] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[Orgroom]', N'utedittime') IS NULL ALTER TABLE [Orgroom] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[Orgroom]', N'utedituser') IS NULL ALTER TABLE [Orgroom] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[Orgroom]', N'utsubdoctype') IS NULL ALTER TABLE [Orgroom] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[PaymentVendor]', N'U') IS NULL BEGIN CREATE TABLE [PaymentVendor] (
    [Id] int NOT NULL,
    [code] nvarchar(20) NULL,
    [stxt] nvarchar(200) NULL,
    [ltxt] nvarchar(500) NULL,
    [utblock] bit NOT NULL,
    CONSTRAINT [PK_PaymentVendor] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[PaymentVendor]', N'Id') IS NULL ALTER TABLE [PaymentVendor] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[PaymentVendor]', N'code') IS NULL ALTER TABLE [PaymentVendor] ADD [code] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[PaymentVendor]', N'stxt') IS NULL ALTER TABLE [PaymentVendor] ADD [stxt] nvarchar(200) NULL;
GO
IF COL_LENGTH(N'[PaymentVendor]', N'ltxt') IS NULL ALTER TABLE [PaymentVendor] ADD [ltxt] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[PaymentVendor]', N'utblock') IS NULL ALTER TABLE [PaymentVendor] ADD [utblock] bit NULL;
GO
IF OBJECT_ID(N'[SignupClientHashKeys]', N'U') IS NULL BEGIN CREATE TABLE [SignupClientHashKeys] (
    [Id] int NOT NULL,
    [ClientId] int NOT NULL,
    [ClientName] nvarchar(150) NULL,
    [HashKey] nvarchar(128) NULL,
    [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreatedAt] datetime2 NOT NULL DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT [PK_SignupClientHashKeys] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[SignupClientHashKeys]', N'Id') IS NULL ALTER TABLE [SignupClientHashKeys] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[SignupClientHashKeys]', N'ClientId') IS NULL ALTER TABLE [SignupClientHashKeys] ADD [ClientId] int NULL;
GO
IF COL_LENGTH(N'[SignupClientHashKeys]', N'ClientName') IS NULL ALTER TABLE [SignupClientHashKeys] ADD [ClientName] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[SignupClientHashKeys]', N'HashKey') IS NULL ALTER TABLE [SignupClientHashKeys] ADD [HashKey] nvarchar(128) NULL;
GO
IF COL_LENGTH(N'[SignupClientHashKeys]', N'IsActive') IS NULL ALTER TABLE [SignupClientHashKeys] ADD [IsActive] bit NULL DEFAULT CAST(1 AS bit);
GO
IF COL_LENGTH(N'[SignupClientHashKeys]', N'CreatedAt') IS NULL ALTER TABLE [SignupClientHashKeys] ADD [CreatedAt] datetime2 NULL DEFAULT (SYSUTCDATETIME());
GO
IF OBJECT_ID(N'[SMAcademicregister]', N'U') IS NULL BEGIN CREATE TABLE [SMAcademicregister] (
    [Id] int NOT NULL,
    [code] nvarchar(max) NULL,
    [stxt] nvarchar(max) NULL,
    [ltxt] nvarchar(max) NULL,
    [clientid] int NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_SMAcademicregister] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[SMAcademicregister]', N'Id') IS NULL ALTER TABLE [SMAcademicregister] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[SMAcademicregister]', N'code') IS NULL ALTER TABLE [SMAcademicregister] ADD [code] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[SMAcademicregister]', N'stxt') IS NULL ALTER TABLE [SMAcademicregister] ADD [stxt] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[SMAcademicregister]', N'ltxt') IS NULL ALTER TABLE [SMAcademicregister] ADD [ltxt] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[SMAcademicregister]', N'clientid') IS NULL ALTER TABLE [SMAcademicregister] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[SMAcademicregister]', N'utsysgen') IS NULL ALTER TABLE [SMAcademicregister] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[SMAcademicregister]', N'utblock') IS NULL ALTER TABLE [SMAcademicregister] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[SMAcademicregister]', N'utispartial') IS NULL ALTER TABLE [SMAcademicregister] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[SMAcademicregister]', N'utrecordversion') IS NULL ALTER TABLE [SMAcademicregister] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[SMAcademicregister]', N'utaddappversion') IS NULL ALTER TABLE [SMAcademicregister] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMAcademicregister]', N'utadddbversion') IS NULL ALTER TABLE [SMAcademicregister] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMAcademicregister]', N'utaddid') IS NULL ALTER TABLE [SMAcademicregister] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[SMAcademicregister]', N'utaddip') IS NULL ALTER TABLE [SMAcademicregister] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMAcademicregister]', N'utaddsessionid') IS NULL ALTER TABLE [SMAcademicregister] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[SMAcademicregister]', N'utaddsystem') IS NULL ALTER TABLE [SMAcademicregister] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMAcademicregister]', N'utaddtime') IS NULL ALTER TABLE [SMAcademicregister] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMAcademicregister]', N'utadduser') IS NULL ALTER TABLE [SMAcademicregister] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMAcademicregister]', N'utdoctype') IS NULL ALTER TABLE [SMAcademicregister] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[SMAcademicregister]', N'uteditappversion') IS NULL ALTER TABLE [SMAcademicregister] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMAcademicregister]', N'uteditdbversion') IS NULL ALTER TABLE [SMAcademicregister] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMAcademicregister]', N'uteditid') IS NULL ALTER TABLE [SMAcademicregister] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[SMAcademicregister]', N'uteditip') IS NULL ALTER TABLE [SMAcademicregister] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMAcademicregister]', N'uteditsessionid') IS NULL ALTER TABLE [SMAcademicregister] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[SMAcademicregister]', N'uteditsystem') IS NULL ALTER TABLE [SMAcademicregister] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMAcademicregister]', N'utedittime') IS NULL ALTER TABLE [SMAcademicregister] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMAcademicregister]', N'utedituser') IS NULL ALTER TABLE [SMAcademicregister] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMAcademicregister]', N'utsubdoctype') IS NULL ALTER TABLE [SMAcademicregister] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[SMAdditionalinfoconfig]', N'U') IS NULL BEGIN CREATE TABLE [SMAdditionalinfoconfig] (
    [Id] int NOT NULL,
    [code] nvarchar(20) NULL,
    [stxt] nvarchar(200) NULL,
    [indx] int NOT NULL,
    [webicon] nvarchar(100) NULL,
    [route] nvarchar(200) NULL,
    [visible] bit NOT NULL,
    [class] nvarchar(100) NULL,
    [grponid] int NULL,
    [infoforid] int NULL,
    [utblock] bit NOT NULL,
    CONSTRAINT [PK_SMAdditionalinfoconfig] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[SMAdditionalinfoconfig]', N'Id') IS NULL ALTER TABLE [SMAdditionalinfoconfig] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[SMAdditionalinfoconfig]', N'code') IS NULL ALTER TABLE [SMAdditionalinfoconfig] ADD [code] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[SMAdditionalinfoconfig]', N'stxt') IS NULL ALTER TABLE [SMAdditionalinfoconfig] ADD [stxt] nvarchar(200) NULL;
GO
IF COL_LENGTH(N'[SMAdditionalinfoconfig]', N'indx') IS NULL ALTER TABLE [SMAdditionalinfoconfig] ADD [indx] int NULL;
GO
IF COL_LENGTH(N'[SMAdditionalinfoconfig]', N'webicon') IS NULL ALTER TABLE [SMAdditionalinfoconfig] ADD [webicon] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[SMAdditionalinfoconfig]', N'route') IS NULL ALTER TABLE [SMAdditionalinfoconfig] ADD [route] nvarchar(200) NULL;
GO
IF COL_LENGTH(N'[SMAdditionalinfoconfig]', N'visible') IS NULL ALTER TABLE [SMAdditionalinfoconfig] ADD [visible] bit NULL;
GO
IF COL_LENGTH(N'[SMAdditionalinfoconfig]', N'class') IS NULL ALTER TABLE [SMAdditionalinfoconfig] ADD [class] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[SMAdditionalinfoconfig]', N'grponid') IS NULL ALTER TABLE [SMAdditionalinfoconfig] ADD [grponid] int NULL;
GO
IF COL_LENGTH(N'[SMAdditionalinfoconfig]', N'infoforid') IS NULL ALTER TABLE [SMAdditionalinfoconfig] ADD [infoforid] int NULL;
GO
IF COL_LENGTH(N'[SMAdditionalinfoconfig]', N'utblock') IS NULL ALTER TABLE [SMAdditionalinfoconfig] ADD [utblock] bit NULL;
GO
IF OBJECT_ID(N'[SMAdditionalregistration]', N'U') IS NULL BEGIN CREATE TABLE [SMAdditionalregistration] (
    [Id] int NOT NULL,
    [caption] nvarchar(200) NULL,
    [configid] int NULL,
    [entityid] int NULL,
    [clientid] int NULL,
    [hide] bit NOT NULL,
    [infoforid] int NULL,
    [sequance] int NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_SMAdditionalregistration] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[SMAdditionalregistration]', N'Id') IS NULL ALTER TABLE [SMAdditionalregistration] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[SMAdditionalregistration]', N'caption') IS NULL ALTER TABLE [SMAdditionalregistration] ADD [caption] nvarchar(200) NULL;
GO
IF COL_LENGTH(N'[SMAdditionalregistration]', N'configid') IS NULL ALTER TABLE [SMAdditionalregistration] ADD [configid] int NULL;
GO
IF COL_LENGTH(N'[SMAdditionalregistration]', N'entityid') IS NULL ALTER TABLE [SMAdditionalregistration] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[SMAdditionalregistration]', N'clientid') IS NULL ALTER TABLE [SMAdditionalregistration] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[SMAdditionalregistration]', N'hide') IS NULL ALTER TABLE [SMAdditionalregistration] ADD [hide] bit NULL;
GO
IF COL_LENGTH(N'[SMAdditionalregistration]', N'infoforid') IS NULL ALTER TABLE [SMAdditionalregistration] ADD [infoforid] int NULL;
GO
IF COL_LENGTH(N'[SMAdditionalregistration]', N'sequance') IS NULL ALTER TABLE [SMAdditionalregistration] ADD [sequance] int NULL;
GO
IF COL_LENGTH(N'[SMAdditionalregistration]', N'utsysgen') IS NULL ALTER TABLE [SMAdditionalregistration] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[SMAdditionalregistration]', N'utblock') IS NULL ALTER TABLE [SMAdditionalregistration] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[SMAdditionalregistration]', N'utispartial') IS NULL ALTER TABLE [SMAdditionalregistration] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[SMAdditionalregistration]', N'utrecordversion') IS NULL ALTER TABLE [SMAdditionalregistration] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[SMAdditionalregistration]', N'utaddappversion') IS NULL ALTER TABLE [SMAdditionalregistration] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMAdditionalregistration]', N'utadddbversion') IS NULL ALTER TABLE [SMAdditionalregistration] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMAdditionalregistration]', N'utaddid') IS NULL ALTER TABLE [SMAdditionalregistration] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[SMAdditionalregistration]', N'utaddip') IS NULL ALTER TABLE [SMAdditionalregistration] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMAdditionalregistration]', N'utaddsessionid') IS NULL ALTER TABLE [SMAdditionalregistration] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[SMAdditionalregistration]', N'utaddsystem') IS NULL ALTER TABLE [SMAdditionalregistration] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMAdditionalregistration]', N'utaddtime') IS NULL ALTER TABLE [SMAdditionalregistration] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMAdditionalregistration]', N'utadduser') IS NULL ALTER TABLE [SMAdditionalregistration] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMAdditionalregistration]', N'utdoctype') IS NULL ALTER TABLE [SMAdditionalregistration] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[SMAdditionalregistration]', N'uteditappversion') IS NULL ALTER TABLE [SMAdditionalregistration] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMAdditionalregistration]', N'uteditdbversion') IS NULL ALTER TABLE [SMAdditionalregistration] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMAdditionalregistration]', N'uteditid') IS NULL ALTER TABLE [SMAdditionalregistration] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[SMAdditionalregistration]', N'uteditip') IS NULL ALTER TABLE [SMAdditionalregistration] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMAdditionalregistration]', N'uteditsessionid') IS NULL ALTER TABLE [SMAdditionalregistration] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[SMAdditionalregistration]', N'uteditsystem') IS NULL ALTER TABLE [SMAdditionalregistration] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMAdditionalregistration]', N'utedittime') IS NULL ALTER TABLE [SMAdditionalregistration] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMAdditionalregistration]', N'utedituser') IS NULL ALTER TABLE [SMAdditionalregistration] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMAdditionalregistration]', N'utsubdoctype') IS NULL ALTER TABLE [SMAdditionalregistration] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[SMAdmApplication]', N'U') IS NULL BEGIN CREATE TABLE [SMAdmApplication] (
    [id] int NOT NULL,
    [enquirydtlid] int NULL,
    [appdate] datetime2 NULL,
    CONSTRAINT [PK_SMAdmApplication] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[SMAdmApplication]', N'id') IS NULL ALTER TABLE [SMAdmApplication] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[SMAdmApplication]', N'enquirydtlid') IS NULL ALTER TABLE [SMAdmApplication] ADD [enquirydtlid] int NULL;
GO
IF COL_LENGTH(N'[SMAdmApplication]', N'appdate') IS NULL ALTER TABLE [SMAdmApplication] ADD [appdate] datetime2 NULL;
GO
IF OBJECT_ID(N'[SMCourse]', N'U') IS NULL BEGIN CREATE TABLE [SMCourse] (
    [Id] int NOT NULL,
    [stxt] nvarchar(max) NULL,
    [indx] int NULL,
    CONSTRAINT [PK_SMCourse] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[SMCourse]', N'Id') IS NULL ALTER TABLE [SMCourse] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[SMCourse]', N'stxt') IS NULL ALTER TABLE [SMCourse] ADD [stxt] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[SMCourse]', N'indx') IS NULL ALTER TABLE [SMCourse] ADD [indx] int NULL;
GO
IF OBJECT_ID(N'[SMCourseextend]', N'U') IS NULL BEGIN CREATE TABLE [SMCourseextend] (
    [id] int NOT NULL,
    [entityid] int NULL,
    [courseid] int NULL,
    CONSTRAINT [PK_SMCourseextend] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[SMCourseextend]', N'id') IS NULL ALTER TABLE [SMCourseextend] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[SMCourseextend]', N'entityid') IS NULL ALTER TABLE [SMCourseextend] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[SMCourseextend]', N'courseid') IS NULL ALTER TABLE [SMCourseextend] ADD [courseid] int NULL;
GO
IF OBJECT_ID(N'[SMFeechallanhdr]', N'U') IS NULL BEGIN CREATE TABLE [SMFeechallanhdr] (
    [Id] bigint NOT NULL IDENTITY,
    [clientid] int NULL,
    [entityid] int NULL,
    [challandate] datetime2 NULL,
    [balanceamount] float NULL,
    [chlnfineamt] float NULL,
    [revchln] bit NULL,
    [utblock] bit NULL,
    CONSTRAINT [PK_SMFeechallanhdr] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[SMFeechallanhdr]', N'Id') IS NULL ALTER TABLE [SMFeechallanhdr] ADD [Id] bigint NULL ;
GO
IF COL_LENGTH(N'[SMFeechallanhdr]', N'clientid') IS NULL ALTER TABLE [SMFeechallanhdr] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[SMFeechallanhdr]', N'entityid') IS NULL ALTER TABLE [SMFeechallanhdr] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[SMFeechallanhdr]', N'challandate') IS NULL ALTER TABLE [SMFeechallanhdr] ADD [challandate] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMFeechallanhdr]', N'balanceamount') IS NULL ALTER TABLE [SMFeechallanhdr] ADD [balanceamount] float NULL;
GO
IF COL_LENGTH(N'[SMFeechallanhdr]', N'chlnfineamt') IS NULL ALTER TABLE [SMFeechallanhdr] ADD [chlnfineamt] float NULL;
GO
IF COL_LENGTH(N'[SMFeechallanhdr]', N'revchln') IS NULL ALTER TABLE [SMFeechallanhdr] ADD [revchln] bit NULL;
GO
IF COL_LENGTH(N'[SMFeechallanhdr]', N'utblock') IS NULL ALTER TABLE [SMFeechallanhdr] ADD [utblock] bit NULL;
GO
IF OBJECT_ID(N'[SMFeecollectiondtl]', N'U') IS NULL BEGIN CREATE TABLE [SMFeecollectiondtl] (
    [id] bigint NOT NULL IDENTITY,
    [transid] bigint NULL,
    [fineamount] float NULL,
    CONSTRAINT [PK_SMFeecollectiondtl] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[SMFeecollectiondtl]', N'id') IS NULL ALTER TABLE [SMFeecollectiondtl] ADD [id] bigint NULL ;
GO
IF COL_LENGTH(N'[SMFeecollectiondtl]', N'transid') IS NULL ALTER TABLE [SMFeecollectiondtl] ADD [transid] bigint NULL;
GO
IF COL_LENGTH(N'[SMFeecollectiondtl]', N'fineamount') IS NULL ALTER TABLE [SMFeecollectiondtl] ADD [fineamount] float NULL;
GO
IF OBJECT_ID(N'[SMFeecollectionhdr]', N'U') IS NULL BEGIN CREATE TABLE [SMFeecollectionhdr] (
    [Id] bigint NOT NULL IDENTITY,
    [clientid] int NULL,
    [entityid] int NULL,
    [transdate] datetime2 NULL,
    [paidamount] float NULL,
    [revcoll] bit NULL,
    [utblock] bit NULL,
    CONSTRAINT [PK_SMFeecollectionhdr] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[SMFeecollectionhdr]', N'Id') IS NULL ALTER TABLE [SMFeecollectionhdr] ADD [Id] bigint NULL ;
GO
IF COL_LENGTH(N'[SMFeecollectionhdr]', N'clientid') IS NULL ALTER TABLE [SMFeecollectionhdr] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[SMFeecollectionhdr]', N'entityid') IS NULL ALTER TABLE [SMFeecollectionhdr] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[SMFeecollectionhdr]', N'transdate') IS NULL ALTER TABLE [SMFeecollectionhdr] ADD [transdate] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMFeecollectionhdr]', N'paidamount') IS NULL ALTER TABLE [SMFeecollectionhdr] ADD [paidamount] float NULL;
GO
IF COL_LENGTH(N'[SMFeecollectionhdr]', N'revcoll') IS NULL ALTER TABLE [SMFeecollectionhdr] ADD [revcoll] bit NULL;
GO
IF COL_LENGTH(N'[SMFeecollectionhdr]', N'utblock') IS NULL ALTER TABLE [SMFeecollectionhdr] ADD [utblock] bit NULL;
GO
IF OBJECT_ID(N'[SMFeetype]', N'U') IS NULL BEGIN CREATE TABLE [SMFeetype] (
    [Id] int NOT NULL,
    [clientid] int NULL,
    [entityid] int NULL,
    [ficoaid] int NULL,
    [utblock] bit NULL,
    CONSTRAINT [PK_SMFeetype] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[SMFeetype]', N'Id') IS NULL ALTER TABLE [SMFeetype] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[SMFeetype]', N'clientid') IS NULL ALTER TABLE [SMFeetype] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[SMFeetype]', N'entityid') IS NULL ALTER TABLE [SMFeetype] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[SMFeetype]', N'ficoaid') IS NULL ALTER TABLE [SMFeetype] ADD [ficoaid] int NULL;
GO
IF COL_LENGTH(N'[SMFeetype]', N'utblock') IS NULL ALTER TABLE [SMFeetype] ADD [utblock] bit NULL;
GO
IF OBJECT_ID(N'[SMGuardian]', N'U') IS NULL BEGIN CREATE TABLE [SMGuardian] (
    [id] int NOT NULL,
    [clientid] int NULL,
    [entityid] int NULL,
    CONSTRAINT [PK_SMGuardian] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[SMGuardian]', N'id') IS NULL ALTER TABLE [SMGuardian] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[SMGuardian]', N'clientid') IS NULL ALTER TABLE [SMGuardian] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[SMGuardian]', N'entityid') IS NULL ALTER TABLE [SMGuardian] ADD [entityid] int NULL;
GO
IF OBJECT_ID(N'[SMPublishdocumenthdr]', N'U') IS NULL BEGIN CREATE TABLE [SMPublishdocumenthdr] (
    [Id] int NOT NULL,
    [code] nvarchar(10) NULL,
    [lovsharedid] int NULL,
    [doctypeid] int NULL,
    [stxt] nvarchar(100) NULL,
    [clientid] int NULL,
    [publishdate] datetime2 NULL,
    [validtill] datetime2 NULL,
    [validfrom] datetime2 NULL,
    [subjectid] int NULL,
    [courseid] int NULL,
    [acdyearid] int NULL,
    [lovdoccategoryid] int NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_SMPublishdocumenthdr] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[SMPublishdocumenthdr]', N'Id') IS NULL ALTER TABLE [SMPublishdocumenthdr] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumenthdr]', N'code') IS NULL ALTER TABLE [SMPublishdocumenthdr] ADD [code] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumenthdr]', N'lovsharedid') IS NULL ALTER TABLE [SMPublishdocumenthdr] ADD [lovsharedid] int NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumenthdr]', N'doctypeid') IS NULL ALTER TABLE [SMPublishdocumenthdr] ADD [doctypeid] int NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumenthdr]', N'stxt') IS NULL ALTER TABLE [SMPublishdocumenthdr] ADD [stxt] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumenthdr]', N'clientid') IS NULL ALTER TABLE [SMPublishdocumenthdr] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumenthdr]', N'publishdate') IS NULL ALTER TABLE [SMPublishdocumenthdr] ADD [publishdate] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumenthdr]', N'validtill') IS NULL ALTER TABLE [SMPublishdocumenthdr] ADD [validtill] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumenthdr]', N'validfrom') IS NULL ALTER TABLE [SMPublishdocumenthdr] ADD [validfrom] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumenthdr]', N'subjectid') IS NULL ALTER TABLE [SMPublishdocumenthdr] ADD [subjectid] int NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumenthdr]', N'courseid') IS NULL ALTER TABLE [SMPublishdocumenthdr] ADD [courseid] int NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumenthdr]', N'acdyearid') IS NULL ALTER TABLE [SMPublishdocumenthdr] ADD [acdyearid] int NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumenthdr]', N'lovdoccategoryid') IS NULL ALTER TABLE [SMPublishdocumenthdr] ADD [lovdoccategoryid] int NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumenthdr]', N'utsysgen') IS NULL ALTER TABLE [SMPublishdocumenthdr] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumenthdr]', N'utblock') IS NULL ALTER TABLE [SMPublishdocumenthdr] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumenthdr]', N'utispartial') IS NULL ALTER TABLE [SMPublishdocumenthdr] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumenthdr]', N'utrecordversion') IS NULL ALTER TABLE [SMPublishdocumenthdr] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumenthdr]', N'utaddappversion') IS NULL ALTER TABLE [SMPublishdocumenthdr] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumenthdr]', N'utadddbversion') IS NULL ALTER TABLE [SMPublishdocumenthdr] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumenthdr]', N'utaddid') IS NULL ALTER TABLE [SMPublishdocumenthdr] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumenthdr]', N'utaddip') IS NULL ALTER TABLE [SMPublishdocumenthdr] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumenthdr]', N'utaddsessionid') IS NULL ALTER TABLE [SMPublishdocumenthdr] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumenthdr]', N'utaddsystem') IS NULL ALTER TABLE [SMPublishdocumenthdr] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumenthdr]', N'utaddtime') IS NULL ALTER TABLE [SMPublishdocumenthdr] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumenthdr]', N'utadduser') IS NULL ALTER TABLE [SMPublishdocumenthdr] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumenthdr]', N'utdoctype') IS NULL ALTER TABLE [SMPublishdocumenthdr] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumenthdr]', N'uteditappversion') IS NULL ALTER TABLE [SMPublishdocumenthdr] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumenthdr]', N'uteditdbversion') IS NULL ALTER TABLE [SMPublishdocumenthdr] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumenthdr]', N'uteditid') IS NULL ALTER TABLE [SMPublishdocumenthdr] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumenthdr]', N'uteditip') IS NULL ALTER TABLE [SMPublishdocumenthdr] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumenthdr]', N'uteditsessionid') IS NULL ALTER TABLE [SMPublishdocumenthdr] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumenthdr]', N'uteditsystem') IS NULL ALTER TABLE [SMPublishdocumenthdr] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumenthdr]', N'utedittime') IS NULL ALTER TABLE [SMPublishdocumenthdr] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumenthdr]', N'utedituser') IS NULL ALTER TABLE [SMPublishdocumenthdr] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumenthdr]', N'utsubdoctype') IS NULL ALTER TABLE [SMPublishdocumenthdr] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[SMStuAttendance]', N'U') IS NULL BEGIN CREATE TABLE [SMStuAttendance] (
    [Id] bigint NOT NULL IDENTITY,
    [studentid] int NULL,
    [atndate] datetime2 NULL,
    [atnactivityid] int NULL,
    [entityid] int NULL,
    [clientid] int NULL,
    [utblock] bit NOT NULL,
    CONSTRAINT [PK_SMStuAttendance] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[SMStuAttendance]', N'Id') IS NULL ALTER TABLE [SMStuAttendance] ADD [Id] bigint NULL ;
GO
IF COL_LENGTH(N'[SMStuAttendance]', N'studentid') IS NULL ALTER TABLE [SMStuAttendance] ADD [studentid] int NULL;
GO
IF COL_LENGTH(N'[SMStuAttendance]', N'atndate') IS NULL ALTER TABLE [SMStuAttendance] ADD [atndate] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMStuAttendance]', N'atnactivityid') IS NULL ALTER TABLE [SMStuAttendance] ADD [atnactivityid] int NULL;
GO
IF COL_LENGTH(N'[SMStuAttendance]', N'entityid') IS NULL ALTER TABLE [SMStuAttendance] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[SMStuAttendance]', N'clientid') IS NULL ALTER TABLE [SMStuAttendance] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[SMStuAttendance]', N'utblock') IS NULL ALTER TABLE [SMStuAttendance] ADD [utblock] bit NULL;
GO
IF OBJECT_ID(N'[SMStudentguardian]', N'U') IS NULL BEGIN CREATE TABLE [SMStudentguardian] (
    [id] int NOT NULL,
    [studentid] int NULL,
    [guardianid] int NULL,
    CONSTRAINT [PK_SMStudentguardian] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[SMStudentguardian]', N'id') IS NULL ALTER TABLE [SMStudentguardian] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[SMStudentguardian]', N'studentid') IS NULL ALTER TABLE [SMStudentguardian] ADD [studentid] int NULL;
GO
IF COL_LENGTH(N'[SMStudentguardian]', N'guardianid') IS NULL ALTER TABLE [SMStudentguardian] ADD [guardianid] int NULL;
GO
IF OBJECT_ID(N'[SMStudentinfo]', N'U') IS NULL BEGIN CREATE TABLE [SMStudentinfo] (
    [id] int NOT NULL,
    [clientid] int NULL,
    [entityid] int NULL,
    [regdate] datetime2 NULL,
    [lovstatusid] int NULL,
    [genderid] int NULL,
    [curcourseid] int NULL,
    [appid] int NULL,
    CONSTRAINT [PK_SMStudentinfo] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[SMStudentinfo]', N'id') IS NULL ALTER TABLE [SMStudentinfo] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[SMStudentinfo]', N'clientid') IS NULL ALTER TABLE [SMStudentinfo] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[SMStudentinfo]', N'entityid') IS NULL ALTER TABLE [SMStudentinfo] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[SMStudentinfo]', N'regdate') IS NULL ALTER TABLE [SMStudentinfo] ADD [regdate] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMStudentinfo]', N'lovstatusid') IS NULL ALTER TABLE [SMStudentinfo] ADD [lovstatusid] int NULL;
GO
IF COL_LENGTH(N'[SMStudentinfo]', N'genderid') IS NULL ALTER TABLE [SMStudentinfo] ADD [genderid] int NULL;
GO
IF COL_LENGTH(N'[SMStudentinfo]', N'curcourseid') IS NULL ALTER TABLE [SMStudentinfo] ADD [curcourseid] int NULL;
GO
IF COL_LENGTH(N'[SMStudentinfo]', N'appid') IS NULL ALTER TABLE [SMStudentinfo] ADD [appid] int NULL;
GO
IF OBJECT_ID(N'[SMStudentwithdrawal]', N'U') IS NULL BEGIN CREATE TABLE [SMStudentwithdrawal] (
    [id] int NOT NULL,
    [studentid] int NULL,
    [entityid] int NULL,
    [leavingdate] datetime2 NULL,
    [lovstatusid] int NULL,
    CONSTRAINT [PK_SMStudentwithdrawal] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[SMStudentwithdrawal]', N'id') IS NULL ALTER TABLE [SMStudentwithdrawal] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[SMStudentwithdrawal]', N'studentid') IS NULL ALTER TABLE [SMStudentwithdrawal] ADD [studentid] int NULL;
GO
IF COL_LENGTH(N'[SMStudentwithdrawal]', N'entityid') IS NULL ALTER TABLE [SMStudentwithdrawal] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[SMStudentwithdrawal]', N'leavingdate') IS NULL ALTER TABLE [SMStudentwithdrawal] ADD [leavingdate] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMStudentwithdrawal]', N'lovstatusid') IS NULL ALTER TABLE [SMStudentwithdrawal] ADD [lovstatusid] int NULL;
GO
IF OBJECT_ID(N'[TerminologySet]', N'U') IS NULL BEGIN CREATE TABLE [TerminologySet] (
    [Id] int NOT NULL,
    [SetName] nvarchar(max) NULL,
    [ConfigJson] nvarchar(max) NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_TerminologySet] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[TerminologySet]', N'Id') IS NULL ALTER TABLE [TerminologySet] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[TerminologySet]', N'SetName') IS NULL ALTER TABLE [TerminologySet] ADD [SetName] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[TerminologySet]', N'ConfigJson') IS NULL ALTER TABLE [TerminologySet] ADD [ConfigJson] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[TerminologySet]', N'IsActive') IS NULL ALTER TABLE [TerminologySet] ADD [IsActive] bit NULL;
GO
IF COL_LENGTH(N'[TerminologySet]', N'CreatedAt') IS NULL ALTER TABLE [TerminologySet] ADD [CreatedAt] datetime2 NULL;
GO
IF OBJECT_ID(N'[OrgRegion]', N'U') IS NULL BEGIN CREATE TABLE [OrgRegion] (
    [Id] int NOT NULL,
    [ClientId] int NOT NULL,
    [CountryId] int NULL,
    [Code] nvarchar(20) NULL,
    [stxt] nvarchar(150) NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_OrgRegion] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[OrgRegion]', N'Id') IS NULL ALTER TABLE [OrgRegion] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[OrgRegion]', N'ClientId') IS NULL ALTER TABLE [OrgRegion] ADD [ClientId] int NULL;
GO
IF COL_LENGTH(N'[OrgRegion]', N'CountryId') IS NULL ALTER TABLE [OrgRegion] ADD [CountryId] int NULL;
GO
IF COL_LENGTH(N'[OrgRegion]', N'Code') IS NULL ALTER TABLE [OrgRegion] ADD [Code] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[OrgRegion]', N'stxt') IS NULL ALTER TABLE [OrgRegion] ADD [stxt] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[OrgRegion]', N'utsysgen') IS NULL ALTER TABLE [OrgRegion] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[OrgRegion]', N'utblock') IS NULL ALTER TABLE [OrgRegion] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[OrgRegion]', N'utispartial') IS NULL ALTER TABLE [OrgRegion] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[OrgRegion]', N'utrecordversion') IS NULL ALTER TABLE [OrgRegion] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[OrgRegion]', N'utaddappversion') IS NULL ALTER TABLE [OrgRegion] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[OrgRegion]', N'utadddbversion') IS NULL ALTER TABLE [OrgRegion] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[OrgRegion]', N'utaddid') IS NULL ALTER TABLE [OrgRegion] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[OrgRegion]', N'utaddip') IS NULL ALTER TABLE [OrgRegion] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgRegion]', N'utaddsessionid') IS NULL ALTER TABLE [OrgRegion] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[OrgRegion]', N'utaddsystem') IS NULL ALTER TABLE [OrgRegion] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgRegion]', N'utaddtime') IS NULL ALTER TABLE [OrgRegion] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgRegion]', N'utadduser') IS NULL ALTER TABLE [OrgRegion] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgRegion]', N'utdoctype') IS NULL ALTER TABLE [OrgRegion] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[OrgRegion]', N'uteditappversion') IS NULL ALTER TABLE [OrgRegion] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[OrgRegion]', N'uteditdbversion') IS NULL ALTER TABLE [OrgRegion] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[OrgRegion]', N'uteditid') IS NULL ALTER TABLE [OrgRegion] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[OrgRegion]', N'uteditip') IS NULL ALTER TABLE [OrgRegion] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgRegion]', N'uteditsessionid') IS NULL ALTER TABLE [OrgRegion] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[OrgRegion]', N'uteditsystem') IS NULL ALTER TABLE [OrgRegion] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgRegion]', N'utedittime') IS NULL ALTER TABLE [OrgRegion] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgRegion]', N'utedituser') IS NULL ALTER TABLE [OrgRegion] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgRegion]', N'utsubdoctype') IS NULL ALTER TABLE [OrgRegion] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[State]', N'U') IS NULL BEGIN CREATE TABLE [State] (
    [Id] int NOT NULL,
    [countryid] int NOT NULL,
    [stxt] nvarchar(150) NULL,
    [statecode] nvarchar(5) NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utblock] bit NOT NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    CONSTRAINT [PK_State] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[State]', N'Id') IS NULL ALTER TABLE [State] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[State]', N'countryid') IS NULL ALTER TABLE [State] ADD [countryid] int NULL;
GO
IF COL_LENGTH(N'[State]', N'stxt') IS NULL ALTER TABLE [State] ADD [stxt] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[State]', N'statecode') IS NULL ALTER TABLE [State] ADD [statecode] nvarchar(5) NULL;
GO
IF COL_LENGTH(N'[State]', N'utaddappversion') IS NULL ALTER TABLE [State] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[State]', N'utadddbversion') IS NULL ALTER TABLE [State] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[State]', N'utaddid') IS NULL ALTER TABLE [State] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[State]', N'utaddip') IS NULL ALTER TABLE [State] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[State]', N'utaddsessionid') IS NULL ALTER TABLE [State] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[State]', N'utaddsystem') IS NULL ALTER TABLE [State] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[State]', N'utaddtime') IS NULL ALTER TABLE [State] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[State]', N'utadduser') IS NULL ALTER TABLE [State] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[State]', N'utblock') IS NULL ALTER TABLE [State] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[State]', N'utdoctype') IS NULL ALTER TABLE [State] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[State]', N'uteditappversion') IS NULL ALTER TABLE [State] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[State]', N'uteditdbversion') IS NULL ALTER TABLE [State] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[State]', N'uteditid') IS NULL ALTER TABLE [State] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[State]', N'uteditip') IS NULL ALTER TABLE [State] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[State]', N'uteditsessionid') IS NULL ALTER TABLE [State] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[State]', N'uteditsystem') IS NULL ALTER TABLE [State] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[State]', N'utedittime') IS NULL ALTER TABLE [State] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[State]', N'utedituser') IS NULL ALTER TABLE [State] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[State]', N'utispartial') IS NULL ALTER TABLE [State] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[State]', N'utrecordversion') IS NULL ALTER TABLE [State] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[State]', N'utsubdoctype') IS NULL ALTER TABLE [State] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[State]', N'utsysgen') IS NULL ALTER TABLE [State] ADD [utsysgen] bit NULL;
GO
IF OBJECT_ID(N'[OrgDepartment]', N'U') IS NULL BEGIN CREATE TABLE [OrgDepartment] (
    [Id] int NOT NULL,
    [depcode] nvarchar(10) NULL,
    [depstxt] nvarchar(50) NULL,
    [depltxt] nvarchar(500) NULL,
    [clientid] int NULL,
    [ficoaid] int NULL,
    [ficoaid_rec] int NULL,
    [utblock] bit NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    CONSTRAINT [PK_OrgDepartment] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[OrgDepartment]', N'Id') IS NULL ALTER TABLE [OrgDepartment] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[OrgDepartment]', N'depcode') IS NULL ALTER TABLE [OrgDepartment] ADD [depcode] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[OrgDepartment]', N'depstxt') IS NULL ALTER TABLE [OrgDepartment] ADD [depstxt] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[OrgDepartment]', N'depltxt') IS NULL ALTER TABLE [OrgDepartment] ADD [depltxt] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[OrgDepartment]', N'clientid') IS NULL ALTER TABLE [OrgDepartment] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[OrgDepartment]', N'ficoaid') IS NULL ALTER TABLE [OrgDepartment] ADD [ficoaid] int NULL;
GO
IF COL_LENGTH(N'[OrgDepartment]', N'ficoaid_rec') IS NULL ALTER TABLE [OrgDepartment] ADD [ficoaid_rec] int NULL;
GO
IF COL_LENGTH(N'[OrgDepartment]', N'utblock') IS NULL ALTER TABLE [OrgDepartment] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[OrgDepartment]', N'utaddappversion') IS NULL ALTER TABLE [OrgDepartment] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[OrgDepartment]', N'utadddbversion') IS NULL ALTER TABLE [OrgDepartment] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[OrgDepartment]', N'utaddid') IS NULL ALTER TABLE [OrgDepartment] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[OrgDepartment]', N'utaddip') IS NULL ALTER TABLE [OrgDepartment] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgDepartment]', N'utaddsessionid') IS NULL ALTER TABLE [OrgDepartment] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[OrgDepartment]', N'utaddsystem') IS NULL ALTER TABLE [OrgDepartment] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgDepartment]', N'utaddtime') IS NULL ALTER TABLE [OrgDepartment] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgDepartment]', N'utadduser') IS NULL ALTER TABLE [OrgDepartment] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgDepartment]', N'utdoctype') IS NULL ALTER TABLE [OrgDepartment] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[OrgDepartment]', N'uteditappversion') IS NULL ALTER TABLE [OrgDepartment] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[OrgDepartment]', N'uteditdbversion') IS NULL ALTER TABLE [OrgDepartment] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[OrgDepartment]', N'uteditid') IS NULL ALTER TABLE [OrgDepartment] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[OrgDepartment]', N'uteditip') IS NULL ALTER TABLE [OrgDepartment] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgDepartment]', N'uteditsessionid') IS NULL ALTER TABLE [OrgDepartment] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[OrgDepartment]', N'uteditsystem') IS NULL ALTER TABLE [OrgDepartment] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgDepartment]', N'utedittime') IS NULL ALTER TABLE [OrgDepartment] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgDepartment]', N'utedituser') IS NULL ALTER TABLE [OrgDepartment] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[OrgDepartment]', N'utispartial') IS NULL ALTER TABLE [OrgDepartment] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[OrgDepartment]', N'utrecordversion') IS NULL ALTER TABLE [OrgDepartment] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[OrgDepartment]', N'utsubdoctype') IS NULL ALTER TABLE [OrgDepartment] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[OrgDepartment]', N'utsysgen') IS NULL ALTER TABLE [OrgDepartment] ADD [utsysgen] bit NULL;
GO
IF OBJECT_ID(N'[FOCalllog]', N'U') IS NULL BEGIN CREATE TABLE [FOCalllog] (
    [id] int NOT NULL,
    [logid] nvarchar(15) NULL,
    [calldate] datetime2 NULL,
    [calltime] datetime2 NULL,
    [purposetypeid] int NULL,
    [name] nvarchar(50) NULL,
    [phoneno] nvarchar(15) NULL,
    [calltypeid] int NULL,
    [callmediumid] int NULL,
    [duration] nvarchar(50) NULL,
    [description] nvarchar(500) NULL,
    [entityid] int NOT NULL,
    [clientid] int NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utsysgen] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_FOCalllog] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[FOCalllog]', N'id') IS NULL ALTER TABLE [FOCalllog] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[FOCalllog]', N'logid') IS NULL ALTER TABLE [FOCalllog] ADD [logid] nvarchar(15) NULL;
GO
IF COL_LENGTH(N'[FOCalllog]', N'calldate') IS NULL ALTER TABLE [FOCalllog] ADD [calldate] datetime2 NULL;
GO
IF COL_LENGTH(N'[FOCalllog]', N'calltime') IS NULL ALTER TABLE [FOCalllog] ADD [calltime] datetime2 NULL;
GO
IF COL_LENGTH(N'[FOCalllog]', N'purposetypeid') IS NULL ALTER TABLE [FOCalllog] ADD [purposetypeid] int NULL;
GO
IF COL_LENGTH(N'[FOCalllog]', N'name') IS NULL ALTER TABLE [FOCalllog] ADD [name] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[FOCalllog]', N'phoneno') IS NULL ALTER TABLE [FOCalllog] ADD [phoneno] nvarchar(15) NULL;
GO
IF COL_LENGTH(N'[FOCalllog]', N'calltypeid') IS NULL ALTER TABLE [FOCalllog] ADD [calltypeid] int NULL;
GO
IF COL_LENGTH(N'[FOCalllog]', N'callmediumid') IS NULL ALTER TABLE [FOCalllog] ADD [callmediumid] int NULL;
GO
IF COL_LENGTH(N'[FOCalllog]', N'duration') IS NULL ALTER TABLE [FOCalllog] ADD [duration] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[FOCalllog]', N'description') IS NULL ALTER TABLE [FOCalllog] ADD [description] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[FOCalllog]', N'entityid') IS NULL ALTER TABLE [FOCalllog] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[FOCalllog]', N'clientid') IS NULL ALTER TABLE [FOCalllog] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[FOCalllog]', N'utblock') IS NULL ALTER TABLE [FOCalllog] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[FOCalllog]', N'utispartial') IS NULL ALTER TABLE [FOCalllog] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[FOCalllog]', N'utsysgen') IS NULL ALTER TABLE [FOCalllog] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[FOCalllog]', N'utrecordversion') IS NULL ALTER TABLE [FOCalllog] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[FOCalllog]', N'utaddappversion') IS NULL ALTER TABLE [FOCalllog] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[FOCalllog]', N'utadddbversion') IS NULL ALTER TABLE [FOCalllog] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[FOCalllog]', N'utaddid') IS NULL ALTER TABLE [FOCalllog] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[FOCalllog]', N'utaddip') IS NULL ALTER TABLE [FOCalllog] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[FOCalllog]', N'utaddsessionid') IS NULL ALTER TABLE [FOCalllog] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[FOCalllog]', N'utaddsystem') IS NULL ALTER TABLE [FOCalllog] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[FOCalllog]', N'utaddtime') IS NULL ALTER TABLE [FOCalllog] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[FOCalllog]', N'utadduser') IS NULL ALTER TABLE [FOCalllog] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[FOCalllog]', N'utdoctype') IS NULL ALTER TABLE [FOCalllog] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[FOCalllog]', N'uteditappversion') IS NULL ALTER TABLE [FOCalllog] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[FOCalllog]', N'uteditdbversion') IS NULL ALTER TABLE [FOCalllog] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[FOCalllog]', N'uteditid') IS NULL ALTER TABLE [FOCalllog] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[FOCalllog]', N'uteditip') IS NULL ALTER TABLE [FOCalllog] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[FOCalllog]', N'uteditsessionid') IS NULL ALTER TABLE [FOCalllog] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[FOCalllog]', N'uteditsystem') IS NULL ALTER TABLE [FOCalllog] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[FOCalllog]', N'utedittime') IS NULL ALTER TABLE [FOCalllog] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[FOCalllog]', N'utedituser') IS NULL ALTER TABLE [FOCalllog] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[FOCalllog]', N'utsubdoctype') IS NULL ALTER TABLE [FOCalllog] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[FOPostaldispatch]', N'U') IS NULL BEGIN CREATE TABLE [FOPostaldispatch] (
    [id] int NOT NULL,
    [postalno] nvarchar(50) NULL,
    [postaldate] datetime2 NULL,
    [postaltypeid] int NULL,
    [courierid] int NULL,
    [title] nvarchar(250) NULL,
    [fromperson] nvarchar(250) NULL,
    [toperson] nvarchar(250) NULL,
    [address] nvarchar(500) NULL,
    [refno] nvarchar(100) NULL,
    [attention] nvarchar(250) NULL,
    [Cellno] nvarchar(20) NULL,
    [note] nvarchar(1500) NULL,
    [entityid] int NULL,
    [clientid] int NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utsysgen] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    CONSTRAINT [PK_FOPostaldispatch] PRIMARY KEY ([id])
); END;
GO
IF COL_LENGTH(N'[FOPostaldispatch]', N'id') IS NULL ALTER TABLE [FOPostaldispatch] ADD [id] int NULL;
GO
IF COL_LENGTH(N'[FOPostaldispatch]', N'postalno') IS NULL ALTER TABLE [FOPostaldispatch] ADD [postalno] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[FOPostaldispatch]', N'postaldate') IS NULL ALTER TABLE [FOPostaldispatch] ADD [postaldate] datetime2 NULL;
GO
IF COL_LENGTH(N'[FOPostaldispatch]', N'postaltypeid') IS NULL ALTER TABLE [FOPostaldispatch] ADD [postaltypeid] int NULL;
GO
IF COL_LENGTH(N'[FOPostaldispatch]', N'courierid') IS NULL ALTER TABLE [FOPostaldispatch] ADD [courierid] int NULL;
GO
IF COL_LENGTH(N'[FOPostaldispatch]', N'title') IS NULL ALTER TABLE [FOPostaldispatch] ADD [title] nvarchar(250) NULL;
GO
IF COL_LENGTH(N'[FOPostaldispatch]', N'fromperson') IS NULL ALTER TABLE [FOPostaldispatch] ADD [fromperson] nvarchar(250) NULL;
GO
IF COL_LENGTH(N'[FOPostaldispatch]', N'toperson') IS NULL ALTER TABLE [FOPostaldispatch] ADD [toperson] nvarchar(250) NULL;
GO
IF COL_LENGTH(N'[FOPostaldispatch]', N'address') IS NULL ALTER TABLE [FOPostaldispatch] ADD [address] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[FOPostaldispatch]', N'refno') IS NULL ALTER TABLE [FOPostaldispatch] ADD [refno] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[FOPostaldispatch]', N'attention') IS NULL ALTER TABLE [FOPostaldispatch] ADD [attention] nvarchar(250) NULL;
GO
IF COL_LENGTH(N'[FOPostaldispatch]', N'Cellno') IS NULL ALTER TABLE [FOPostaldispatch] ADD [Cellno] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[FOPostaldispatch]', N'note') IS NULL ALTER TABLE [FOPostaldispatch] ADD [note] nvarchar(1500) NULL;
GO
IF COL_LENGTH(N'[FOPostaldispatch]', N'entityid') IS NULL ALTER TABLE [FOPostaldispatch] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[FOPostaldispatch]', N'clientid') IS NULL ALTER TABLE [FOPostaldispatch] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[FOPostaldispatch]', N'utblock') IS NULL ALTER TABLE [FOPostaldispatch] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[FOPostaldispatch]', N'utispartial') IS NULL ALTER TABLE [FOPostaldispatch] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[FOPostaldispatch]', N'utsysgen') IS NULL ALTER TABLE [FOPostaldispatch] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[FOPostaldispatch]', N'utrecordversion') IS NULL ALTER TABLE [FOPostaldispatch] ADD [utrecordversion] int NULL;
GO
IF OBJECT_ID(N'[SMAcademicYear]', N'U') IS NULL BEGIN CREATE TABLE [SMAcademicYear] (
    [Id] int NOT NULL,
    [code] nvarchar(max) NULL,
    [stxt] nvarchar(max) NULL,
    [ltxt] nvarchar(max) NULL,
    [startdate] datetime2 NULL,
    [enddate] datetime2 NULL,
    [clientid] int NULL,
    [active] bit NOT NULL,
    [nooftest] int NULL,
    [noofterms] int NULL,
    [sessionnotvisiable] bit NOT NULL,
    [registerid] int NULL,
    [semlovid] int NULL,
    [coursecatid] int NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_SMAcademicYear] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[SMAcademicYear]', N'Id') IS NULL ALTER TABLE [SMAcademicYear] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[SMAcademicYear]', N'code') IS NULL ALTER TABLE [SMAcademicYear] ADD [code] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[SMAcademicYear]', N'stxt') IS NULL ALTER TABLE [SMAcademicYear] ADD [stxt] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[SMAcademicYear]', N'ltxt') IS NULL ALTER TABLE [SMAcademicYear] ADD [ltxt] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[SMAcademicYear]', N'startdate') IS NULL ALTER TABLE [SMAcademicYear] ADD [startdate] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMAcademicYear]', N'enddate') IS NULL ALTER TABLE [SMAcademicYear] ADD [enddate] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMAcademicYear]', N'clientid') IS NULL ALTER TABLE [SMAcademicYear] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[SMAcademicYear]', N'active') IS NULL ALTER TABLE [SMAcademicYear] ADD [active] bit NULL;
GO
IF COL_LENGTH(N'[SMAcademicYear]', N'nooftest') IS NULL ALTER TABLE [SMAcademicYear] ADD [nooftest] int NULL;
GO
IF COL_LENGTH(N'[SMAcademicYear]', N'noofterms') IS NULL ALTER TABLE [SMAcademicYear] ADD [noofterms] int NULL;
GO
IF COL_LENGTH(N'[SMAcademicYear]', N'sessionnotvisiable') IS NULL ALTER TABLE [SMAcademicYear] ADD [sessionnotvisiable] bit NULL;
GO
IF COL_LENGTH(N'[SMAcademicYear]', N'registerid') IS NULL ALTER TABLE [SMAcademicYear] ADD [registerid] int NULL;
GO
IF COL_LENGTH(N'[SMAcademicYear]', N'semlovid') IS NULL ALTER TABLE [SMAcademicYear] ADD [semlovid] int NULL;
GO
IF COL_LENGTH(N'[SMAcademicYear]', N'coursecatid') IS NULL ALTER TABLE [SMAcademicYear] ADD [coursecatid] int NULL;
GO
IF COL_LENGTH(N'[SMAcademicYear]', N'utsysgen') IS NULL ALTER TABLE [SMAcademicYear] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[SMAcademicYear]', N'utblock') IS NULL ALTER TABLE [SMAcademicYear] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[SMAcademicYear]', N'utispartial') IS NULL ALTER TABLE [SMAcademicYear] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[SMAcademicYear]', N'utrecordversion') IS NULL ALTER TABLE [SMAcademicYear] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[SMAcademicYear]', N'utaddappversion') IS NULL ALTER TABLE [SMAcademicYear] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMAcademicYear]', N'utadddbversion') IS NULL ALTER TABLE [SMAcademicYear] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMAcademicYear]', N'utaddid') IS NULL ALTER TABLE [SMAcademicYear] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[SMAcademicYear]', N'utaddip') IS NULL ALTER TABLE [SMAcademicYear] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMAcademicYear]', N'utaddsessionid') IS NULL ALTER TABLE [SMAcademicYear] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[SMAcademicYear]', N'utaddsystem') IS NULL ALTER TABLE [SMAcademicYear] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMAcademicYear]', N'utaddtime') IS NULL ALTER TABLE [SMAcademicYear] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMAcademicYear]', N'utadduser') IS NULL ALTER TABLE [SMAcademicYear] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMAcademicYear]', N'utdoctype') IS NULL ALTER TABLE [SMAcademicYear] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[SMAcademicYear]', N'uteditappversion') IS NULL ALTER TABLE [SMAcademicYear] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMAcademicYear]', N'uteditdbversion') IS NULL ALTER TABLE [SMAcademicYear] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMAcademicYear]', N'uteditid') IS NULL ALTER TABLE [SMAcademicYear] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[SMAcademicYear]', N'uteditip') IS NULL ALTER TABLE [SMAcademicYear] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMAcademicYear]', N'uteditsessionid') IS NULL ALTER TABLE [SMAcademicYear] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[SMAcademicYear]', N'uteditsystem') IS NULL ALTER TABLE [SMAcademicYear] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMAcademicYear]', N'utedittime') IS NULL ALTER TABLE [SMAcademicYear] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMAcademicYear]', N'utedituser') IS NULL ALTER TABLE [SMAcademicYear] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMAcademicYear]', N'utsubdoctype') IS NULL ALTER TABLE [SMAcademicYear] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[SMPublishdocumentdtl]', N'U') IS NULL BEGIN CREATE TABLE [SMPublishdocumentdtl] (
    [Id] int NOT NULL,
    [publishid] int NULL,
    [lovshareid] int NULL,
    [pclientid] int NULL,
    [pentityid] int NULL,
    [regionid] int NULL,
    [camplvelid] int NULL,
    [courseid] int NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_SMPublishdocumentdtl] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[SMPublishdocumentdtl]', N'Id') IS NULL ALTER TABLE [SMPublishdocumentdtl] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumentdtl]', N'publishid') IS NULL ALTER TABLE [SMPublishdocumentdtl] ADD [publishid] int NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumentdtl]', N'lovshareid') IS NULL ALTER TABLE [SMPublishdocumentdtl] ADD [lovshareid] int NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumentdtl]', N'pclientid') IS NULL ALTER TABLE [SMPublishdocumentdtl] ADD [pclientid] int NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumentdtl]', N'pentityid') IS NULL ALTER TABLE [SMPublishdocumentdtl] ADD [pentityid] int NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumentdtl]', N'regionid') IS NULL ALTER TABLE [SMPublishdocumentdtl] ADD [regionid] int NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumentdtl]', N'camplvelid') IS NULL ALTER TABLE [SMPublishdocumentdtl] ADD [camplvelid] int NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumentdtl]', N'courseid') IS NULL ALTER TABLE [SMPublishdocumentdtl] ADD [courseid] int NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumentdtl]', N'utsysgen') IS NULL ALTER TABLE [SMPublishdocumentdtl] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumentdtl]', N'utblock') IS NULL ALTER TABLE [SMPublishdocumentdtl] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumentdtl]', N'utispartial') IS NULL ALTER TABLE [SMPublishdocumentdtl] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumentdtl]', N'utrecordversion') IS NULL ALTER TABLE [SMPublishdocumentdtl] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumentdtl]', N'utaddappversion') IS NULL ALTER TABLE [SMPublishdocumentdtl] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumentdtl]', N'utadddbversion') IS NULL ALTER TABLE [SMPublishdocumentdtl] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumentdtl]', N'utaddid') IS NULL ALTER TABLE [SMPublishdocumentdtl] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumentdtl]', N'utaddip') IS NULL ALTER TABLE [SMPublishdocumentdtl] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumentdtl]', N'utaddsessionid') IS NULL ALTER TABLE [SMPublishdocumentdtl] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumentdtl]', N'utaddsystem') IS NULL ALTER TABLE [SMPublishdocumentdtl] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumentdtl]', N'utaddtime') IS NULL ALTER TABLE [SMPublishdocumentdtl] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumentdtl]', N'utadduser') IS NULL ALTER TABLE [SMPublishdocumentdtl] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumentdtl]', N'utdoctype') IS NULL ALTER TABLE [SMPublishdocumentdtl] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumentdtl]', N'uteditappversion') IS NULL ALTER TABLE [SMPublishdocumentdtl] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumentdtl]', N'uteditdbversion') IS NULL ALTER TABLE [SMPublishdocumentdtl] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumentdtl]', N'uteditid') IS NULL ALTER TABLE [SMPublishdocumentdtl] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumentdtl]', N'uteditip') IS NULL ALTER TABLE [SMPublishdocumentdtl] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumentdtl]', N'uteditsessionid') IS NULL ALTER TABLE [SMPublishdocumentdtl] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumentdtl]', N'uteditsystem') IS NULL ALTER TABLE [SMPublishdocumentdtl] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumentdtl]', N'utedittime') IS NULL ALTER TABLE [SMPublishdocumentdtl] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumentdtl]', N'utedituser') IS NULL ALTER TABLE [SMPublishdocumentdtl] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMPublishdocumentdtl]', N'utsubdoctype') IS NULL ALTER TABLE [SMPublishdocumentdtl] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[Orgsite]', N'U') IS NULL BEGIN CREATE TABLE [Orgsite] (
    [Id] int NOT NULL,
    [code] nvarchar(10) NULL,
    [stxt] nvarchar(500) NULL,
    [sitetypeid] int NULL,
    [regionid] int NULL,
    [siteforid] int NULL,
    [forid] int NULL,
    [entityid] int NULL,
    [clientid] int NULL,
    [siteid] int NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_Orgsite] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[Orgsite]', N'Id') IS NULL ALTER TABLE [Orgsite] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[Orgsite]', N'code') IS NULL ALTER TABLE [Orgsite] ADD [code] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[Orgsite]', N'stxt') IS NULL ALTER TABLE [Orgsite] ADD [stxt] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[Orgsite]', N'sitetypeid') IS NULL ALTER TABLE [Orgsite] ADD [sitetypeid] int NULL;
GO
IF COL_LENGTH(N'[Orgsite]', N'regionid') IS NULL ALTER TABLE [Orgsite] ADD [regionid] int NULL;
GO
IF COL_LENGTH(N'[Orgsite]', N'siteforid') IS NULL ALTER TABLE [Orgsite] ADD [siteforid] int NULL;
GO
IF COL_LENGTH(N'[Orgsite]', N'forid') IS NULL ALTER TABLE [Orgsite] ADD [forid] int NULL;
GO
IF COL_LENGTH(N'[Orgsite]', N'entityid') IS NULL ALTER TABLE [Orgsite] ADD [entityid] int NULL;
GO
IF COL_LENGTH(N'[Orgsite]', N'clientid') IS NULL ALTER TABLE [Orgsite] ADD [clientid] int NULL;
GO
IF COL_LENGTH(N'[Orgsite]', N'siteid') IS NULL ALTER TABLE [Orgsite] ADD [siteid] int NULL;
GO
IF COL_LENGTH(N'[Orgsite]', N'utsysgen') IS NULL ALTER TABLE [Orgsite] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[Orgsite]', N'utblock') IS NULL ALTER TABLE [Orgsite] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[Orgsite]', N'utispartial') IS NULL ALTER TABLE [Orgsite] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[Orgsite]', N'utrecordversion') IS NULL ALTER TABLE [Orgsite] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[Orgsite]', N'utaddappversion') IS NULL ALTER TABLE [Orgsite] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[Orgsite]', N'utadddbversion') IS NULL ALTER TABLE [Orgsite] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[Orgsite]', N'utaddid') IS NULL ALTER TABLE [Orgsite] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[Orgsite]', N'utaddip') IS NULL ALTER TABLE [Orgsite] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[Orgsite]', N'utaddsessionid') IS NULL ALTER TABLE [Orgsite] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[Orgsite]', N'utaddsystem') IS NULL ALTER TABLE [Orgsite] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[Orgsite]', N'utaddtime') IS NULL ALTER TABLE [Orgsite] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[Orgsite]', N'utadduser') IS NULL ALTER TABLE [Orgsite] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[Orgsite]', N'utdoctype') IS NULL ALTER TABLE [Orgsite] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[Orgsite]', N'uteditappversion') IS NULL ALTER TABLE [Orgsite] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[Orgsite]', N'uteditdbversion') IS NULL ALTER TABLE [Orgsite] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[Orgsite]', N'uteditid') IS NULL ALTER TABLE [Orgsite] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[Orgsite]', N'uteditip') IS NULL ALTER TABLE [Orgsite] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[Orgsite]', N'uteditsessionid') IS NULL ALTER TABLE [Orgsite] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[Orgsite]', N'uteditsystem') IS NULL ALTER TABLE [Orgsite] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[Orgsite]', N'utedittime') IS NULL ALTER TABLE [Orgsite] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[Orgsite]', N'utedituser') IS NULL ALTER TABLE [Orgsite] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[Orgsite]', N'utsubdoctype') IS NULL ALTER TABLE [Orgsite] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[City]', N'U') IS NULL BEGIN CREATE TABLE [City] (
    [Id] int NOT NULL,
    [stateid] int NOT NULL,
    [ctryid] int NOT NULL,
    [citystxt] nvarchar(100) NULL,
    [citycode] nvarchar(10) NULL,
    [dialcode] nvarchar(10) NULL,
    [utblock] bit NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utsubdoctype] int NULL,
    [utsysgen] bit NOT NULL,
    CONSTRAINT [PK_City] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[City]', N'Id') IS NULL ALTER TABLE [City] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[City]', N'stateid') IS NULL ALTER TABLE [City] ADD [stateid] int NULL;
GO
IF COL_LENGTH(N'[City]', N'ctryid') IS NULL ALTER TABLE [City] ADD [ctryid] int NULL;
GO
IF COL_LENGTH(N'[City]', N'citystxt') IS NULL ALTER TABLE [City] ADD [citystxt] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[City]', N'citycode') IS NULL ALTER TABLE [City] ADD [citycode] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[City]', N'dialcode') IS NULL ALTER TABLE [City] ADD [dialcode] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[City]', N'utblock') IS NULL ALTER TABLE [City] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[City]', N'utaddappversion') IS NULL ALTER TABLE [City] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[City]', N'utadddbversion') IS NULL ALTER TABLE [City] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[City]', N'utaddid') IS NULL ALTER TABLE [City] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[City]', N'utaddip') IS NULL ALTER TABLE [City] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[City]', N'utaddsessionid') IS NULL ALTER TABLE [City] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[City]', N'utaddsystem') IS NULL ALTER TABLE [City] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[City]', N'utaddtime') IS NULL ALTER TABLE [City] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[City]', N'utadduser') IS NULL ALTER TABLE [City] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[City]', N'utdoctype') IS NULL ALTER TABLE [City] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[City]', N'uteditappversion') IS NULL ALTER TABLE [City] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[City]', N'uteditdbversion') IS NULL ALTER TABLE [City] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[City]', N'uteditid') IS NULL ALTER TABLE [City] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[City]', N'uteditip') IS NULL ALTER TABLE [City] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[City]', N'uteditsessionid') IS NULL ALTER TABLE [City] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[City]', N'uteditsystem') IS NULL ALTER TABLE [City] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[City]', N'utedittime') IS NULL ALTER TABLE [City] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[City]', N'utedituser') IS NULL ALTER TABLE [City] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[City]', N'utispartial') IS NULL ALTER TABLE [City] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[City]', N'utrecordversion') IS NULL ALTER TABLE [City] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[City]', N'utsubdoctype') IS NULL ALTER TABLE [City] ADD [utsubdoctype] int NULL;
GO
IF COL_LENGTH(N'[City]', N'utsysgen') IS NULL ALTER TABLE [City] ADD [utsysgen] bit NULL;
GO
IF OBJECT_ID(N'[SMAcademicYeardtl]', N'U') IS NULL BEGIN CREATE TABLE [SMAcademicYeardtl] (
    [Id] int NOT NULL,
    [acadyrid] int NOT NULL,
    [lineindex] int NULL,
    [year] int NULL,
    [monthid] int NULL,
    [periodstxt] nvarchar(max) NULL,
    [startdate] datetime2 NULL,
    [enddate] datetime2 NULL,
    [utsysgen] bit NOT NULL,
    [utblock] bit NOT NULL,
    [utispartial] bit NOT NULL,
    [utrecordversion] int NOT NULL,
    [utaddappversion] varchar(10) NULL,
    [utadddbversion] varchar(10) NULL,
    [utaddid] bigint NULL,
    [utaddip] varchar(25) NULL,
    [utaddsessionid] int NULL,
    [utaddsystem] varchar(25) NULL,
    [utaddtime] datetime2 NULL,
    [utadduser] varchar(25) NULL,
    [utdoctype] int NULL,
    [uteditappversion] varchar(10) NULL,
    [uteditdbversion] varchar(10) NULL,
    [uteditid] bigint NULL,
    [uteditip] varchar(25) NULL,
    [uteditsessionid] int NULL,
    [uteditsystem] varchar(25) NULL,
    [utedittime] datetime2 NULL,
    [utedituser] varchar(25) NULL,
    [utsubdoctype] int NULL,
    CONSTRAINT [PK_SMAcademicYeardtl] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[SMAcademicYeardtl]', N'Id') IS NULL ALTER TABLE [SMAcademicYeardtl] ADD [Id] int NULL;
GO
IF COL_LENGTH(N'[SMAcademicYeardtl]', N'acadyrid') IS NULL ALTER TABLE [SMAcademicYeardtl] ADD [acadyrid] int NULL;
GO
IF COL_LENGTH(N'[SMAcademicYeardtl]', N'lineindex') IS NULL ALTER TABLE [SMAcademicYeardtl] ADD [lineindex] int NULL;
GO
IF COL_LENGTH(N'[SMAcademicYeardtl]', N'year') IS NULL ALTER TABLE [SMAcademicYeardtl] ADD [year] int NULL;
GO
IF COL_LENGTH(N'[SMAcademicYeardtl]', N'monthid') IS NULL ALTER TABLE [SMAcademicYeardtl] ADD [monthid] int NULL;
GO
IF COL_LENGTH(N'[SMAcademicYeardtl]', N'periodstxt') IS NULL ALTER TABLE [SMAcademicYeardtl] ADD [periodstxt] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[SMAcademicYeardtl]', N'startdate') IS NULL ALTER TABLE [SMAcademicYeardtl] ADD [startdate] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMAcademicYeardtl]', N'enddate') IS NULL ALTER TABLE [SMAcademicYeardtl] ADD [enddate] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMAcademicYeardtl]', N'utsysgen') IS NULL ALTER TABLE [SMAcademicYeardtl] ADD [utsysgen] bit NULL;
GO
IF COL_LENGTH(N'[SMAcademicYeardtl]', N'utblock') IS NULL ALTER TABLE [SMAcademicYeardtl] ADD [utblock] bit NULL;
GO
IF COL_LENGTH(N'[SMAcademicYeardtl]', N'utispartial') IS NULL ALTER TABLE [SMAcademicYeardtl] ADD [utispartial] bit NULL;
GO
IF COL_LENGTH(N'[SMAcademicYeardtl]', N'utrecordversion') IS NULL ALTER TABLE [SMAcademicYeardtl] ADD [utrecordversion] int NULL;
GO
IF COL_LENGTH(N'[SMAcademicYeardtl]', N'utaddappversion') IS NULL ALTER TABLE [SMAcademicYeardtl] ADD [utaddappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMAcademicYeardtl]', N'utadddbversion') IS NULL ALTER TABLE [SMAcademicYeardtl] ADD [utadddbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMAcademicYeardtl]', N'utaddid') IS NULL ALTER TABLE [SMAcademicYeardtl] ADD [utaddid] bigint NULL;
GO
IF COL_LENGTH(N'[SMAcademicYeardtl]', N'utaddip') IS NULL ALTER TABLE [SMAcademicYeardtl] ADD [utaddip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMAcademicYeardtl]', N'utaddsessionid') IS NULL ALTER TABLE [SMAcademicYeardtl] ADD [utaddsessionid] int NULL;
GO
IF COL_LENGTH(N'[SMAcademicYeardtl]', N'utaddsystem') IS NULL ALTER TABLE [SMAcademicYeardtl] ADD [utaddsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMAcademicYeardtl]', N'utaddtime') IS NULL ALTER TABLE [SMAcademicYeardtl] ADD [utaddtime] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMAcademicYeardtl]', N'utadduser') IS NULL ALTER TABLE [SMAcademicYeardtl] ADD [utadduser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMAcademicYeardtl]', N'utdoctype') IS NULL ALTER TABLE [SMAcademicYeardtl] ADD [utdoctype] int NULL;
GO
IF COL_LENGTH(N'[SMAcademicYeardtl]', N'uteditappversion') IS NULL ALTER TABLE [SMAcademicYeardtl] ADD [uteditappversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMAcademicYeardtl]', N'uteditdbversion') IS NULL ALTER TABLE [SMAcademicYeardtl] ADD [uteditdbversion] varchar(10) NULL;
GO
IF COL_LENGTH(N'[SMAcademicYeardtl]', N'uteditid') IS NULL ALTER TABLE [SMAcademicYeardtl] ADD [uteditid] bigint NULL;
GO
IF COL_LENGTH(N'[SMAcademicYeardtl]', N'uteditip') IS NULL ALTER TABLE [SMAcademicYeardtl] ADD [uteditip] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMAcademicYeardtl]', N'uteditsessionid') IS NULL ALTER TABLE [SMAcademicYeardtl] ADD [uteditsessionid] int NULL;
GO
IF COL_LENGTH(N'[SMAcademicYeardtl]', N'uteditsystem') IS NULL ALTER TABLE [SMAcademicYeardtl] ADD [uteditsystem] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMAcademicYeardtl]', N'utedittime') IS NULL ALTER TABLE [SMAcademicYeardtl] ADD [utedittime] datetime2 NULL;
GO
IF COL_LENGTH(N'[SMAcademicYeardtl]', N'utedituser') IS NULL ALTER TABLE [SMAcademicYeardtl] ADD [utedituser] varchar(25) NULL;
GO
IF COL_LENGTH(N'[SMAcademicYeardtl]', N'utsubdoctype') IS NULL ALTER TABLE [SMAcademicYeardtl] ADD [utsubdoctype] int NULL;
GO
IF OBJECT_ID(N'[Districts]', N'U') IS NULL BEGIN CREATE TABLE [Districts] (
    [DistrictId] int NOT NULL,
    [CityId] int NOT NULL,
    [DistrictName] nvarchar(100) NULL,
    [DistrictCode] nvarchar(10) NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedBy] int NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [UpdatedBy] int NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Districts] PRIMARY KEY ([DistrictId])
); END;
GO
IF COL_LENGTH(N'[Districts]', N'DistrictId') IS NULL ALTER TABLE [Districts] ADD [DistrictId] int NULL;
GO
IF COL_LENGTH(N'[Districts]', N'CityId') IS NULL ALTER TABLE [Districts] ADD [CityId] int NULL;
GO
IF COL_LENGTH(N'[Districts]', N'DistrictName') IS NULL ALTER TABLE [Districts] ADD [DistrictName] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[Districts]', N'DistrictCode') IS NULL ALTER TABLE [Districts] ADD [DistrictCode] nvarchar(10) NULL;
GO
IF COL_LENGTH(N'[Districts]', N'IsActive') IS NULL ALTER TABLE [Districts] ADD [IsActive] bit NULL;
GO
IF COL_LENGTH(N'[Districts]', N'CreatedAt') IS NULL ALTER TABLE [Districts] ADD [CreatedAt] datetime2 NULL;
GO
IF COL_LENGTH(N'[Districts]', N'CreatedBy') IS NULL ALTER TABLE [Districts] ADD [CreatedBy] int NULL;
GO
IF COL_LENGTH(N'[Districts]', N'UpdatedAt') IS NULL ALTER TABLE [Districts] ADD [UpdatedAt] datetime2 NULL;
GO
IF COL_LENGTH(N'[Districts]', N'UpdatedBy') IS NULL ALTER TABLE [Districts] ADD [UpdatedBy] int NULL;
GO
IF COL_LENGTH(N'[Districts]', N'IsDeleted') IS NULL ALTER TABLE [Districts] ADD [IsDeleted] bit NULL;
GO
IF OBJECT_ID(N'[OrgEntities]', N'U') IS NULL BEGIN CREATE TABLE [OrgEntities] (
    [EntityId] int NOT NULL,
    [ClientId] int NOT NULL,
    [EntityCode] nvarchar(20) NULL,
    [EntityName] nvarchar(150) NULL,
    [EntityType] nvarchar(50) NULL,
    [TerminologySetId] int NULL,
    [CountryId] int NOT NULL,
    [StateId] int NULL,
    [CityId] int NULL,
    [Address] nvarchar(300) NULL,
    [PostalCode] nvarchar(20) NULL,
    [PhoneNumber] nvarchar(20) NULL,
    [EmailAddress] nvarchar(100) NULL,
    [WebsiteUrl] nvarchar(200) NULL,
    [LogoUrl] nvarchar(200) NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedBy] int NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [UpdatedBy] int NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_OrgEntities] PRIMARY KEY ([EntityId])
); END;
GO
IF COL_LENGTH(N'[OrgEntities]', N'EntityId') IS NULL ALTER TABLE [OrgEntities] ADD [EntityId] int NULL;
GO
IF COL_LENGTH(N'[OrgEntities]', N'ClientId') IS NULL ALTER TABLE [OrgEntities] ADD [ClientId] int NULL;
GO
IF COL_LENGTH(N'[OrgEntities]', N'EntityCode') IS NULL ALTER TABLE [OrgEntities] ADD [EntityCode] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[OrgEntities]', N'EntityName') IS NULL ALTER TABLE [OrgEntities] ADD [EntityName] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[OrgEntities]', N'EntityType') IS NULL ALTER TABLE [OrgEntities] ADD [EntityType] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[OrgEntities]', N'TerminologySetId') IS NULL ALTER TABLE [OrgEntities] ADD [TerminologySetId] int NULL;
GO
IF COL_LENGTH(N'[OrgEntities]', N'CountryId') IS NULL ALTER TABLE [OrgEntities] ADD [CountryId] int NULL;
GO
IF COL_LENGTH(N'[OrgEntities]', N'StateId') IS NULL ALTER TABLE [OrgEntities] ADD [StateId] int NULL;
GO
IF COL_LENGTH(N'[OrgEntities]', N'CityId') IS NULL ALTER TABLE [OrgEntities] ADD [CityId] int NULL;
GO
IF COL_LENGTH(N'[OrgEntities]', N'Address') IS NULL ALTER TABLE [OrgEntities] ADD [Address] nvarchar(300) NULL;
GO
IF COL_LENGTH(N'[OrgEntities]', N'PostalCode') IS NULL ALTER TABLE [OrgEntities] ADD [PostalCode] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[OrgEntities]', N'PhoneNumber') IS NULL ALTER TABLE [OrgEntities] ADD [PhoneNumber] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[OrgEntities]', N'EmailAddress') IS NULL ALTER TABLE [OrgEntities] ADD [EmailAddress] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[OrgEntities]', N'WebsiteUrl') IS NULL ALTER TABLE [OrgEntities] ADD [WebsiteUrl] nvarchar(200) NULL;
GO
IF COL_LENGTH(N'[OrgEntities]', N'LogoUrl') IS NULL ALTER TABLE [OrgEntities] ADD [LogoUrl] nvarchar(200) NULL;
GO
IF COL_LENGTH(N'[OrgEntities]', N'IsActive') IS NULL ALTER TABLE [OrgEntities] ADD [IsActive] bit NULL;
GO
IF COL_LENGTH(N'[OrgEntities]', N'CreatedAt') IS NULL ALTER TABLE [OrgEntities] ADD [CreatedAt] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgEntities]', N'CreatedBy') IS NULL ALTER TABLE [OrgEntities] ADD [CreatedBy] int NULL;
GO
IF COL_LENGTH(N'[OrgEntities]', N'UpdatedAt') IS NULL ALTER TABLE [OrgEntities] ADD [UpdatedAt] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgEntities]', N'UpdatedBy') IS NULL ALTER TABLE [OrgEntities] ADD [UpdatedBy] int NULL;
GO
IF COL_LENGTH(N'[OrgEntities]', N'IsDeleted') IS NULL ALTER TABLE [OrgEntities] ADD [IsDeleted] bit NULL;
GO
IF OBJECT_ID(N'[OrgCampuses]', N'U') IS NULL BEGIN CREATE TABLE [OrgCampuses] (
    [CampusId] int NOT NULL,
    [EntityId] int NOT NULL,
    [ClientId] int NOT NULL,
    [CampusCode] nvarchar(20) NULL,
    [CampusName] nvarchar(150) NULL,
    [CampusType] nvarchar(50) NULL,
    [CountryId] int NOT NULL,
    [StateId] int NULL,
    [CityId] int NULL,
    [Address] nvarchar(300) NULL,
    [PostalCode] nvarchar(20) NULL,
    [PhoneNumber] nvarchar(20) NULL,
    [EmailAddress] nvarchar(100) NULL,
    [PrincipalName] nvarchar(100) NULL,
    [PrincipalEmail] nvarchar(100) NULL,
    [PrincipalPhone] nvarchar(20) NULL,
    [EstablishedDate] datetime2 NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedBy] int NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [UpdatedBy] int NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_OrgCampuses] PRIMARY KEY ([CampusId])
); END;
GO
IF COL_LENGTH(N'[OrgCampuses]', N'CampusId') IS NULL ALTER TABLE [OrgCampuses] ADD [CampusId] int NULL;
GO
IF COL_LENGTH(N'[OrgCampuses]', N'EntityId') IS NULL ALTER TABLE [OrgCampuses] ADD [EntityId] int NULL;
GO
IF COL_LENGTH(N'[OrgCampuses]', N'ClientId') IS NULL ALTER TABLE [OrgCampuses] ADD [ClientId] int NULL;
GO
IF COL_LENGTH(N'[OrgCampuses]', N'CampusCode') IS NULL ALTER TABLE [OrgCampuses] ADD [CampusCode] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[OrgCampuses]', N'CampusName') IS NULL ALTER TABLE [OrgCampuses] ADD [CampusName] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[OrgCampuses]', N'CampusType') IS NULL ALTER TABLE [OrgCampuses] ADD [CampusType] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[OrgCampuses]', N'CountryId') IS NULL ALTER TABLE [OrgCampuses] ADD [CountryId] int NULL;
GO
IF COL_LENGTH(N'[OrgCampuses]', N'StateId') IS NULL ALTER TABLE [OrgCampuses] ADD [StateId] int NULL;
GO
IF COL_LENGTH(N'[OrgCampuses]', N'CityId') IS NULL ALTER TABLE [OrgCampuses] ADD [CityId] int NULL;
GO
IF COL_LENGTH(N'[OrgCampuses]', N'Address') IS NULL ALTER TABLE [OrgCampuses] ADD [Address] nvarchar(300) NULL;
GO
IF COL_LENGTH(N'[OrgCampuses]', N'PostalCode') IS NULL ALTER TABLE [OrgCampuses] ADD [PostalCode] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[OrgCampuses]', N'PhoneNumber') IS NULL ALTER TABLE [OrgCampuses] ADD [PhoneNumber] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[OrgCampuses]', N'EmailAddress') IS NULL ALTER TABLE [OrgCampuses] ADD [EmailAddress] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[OrgCampuses]', N'PrincipalName') IS NULL ALTER TABLE [OrgCampuses] ADD [PrincipalName] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[OrgCampuses]', N'PrincipalEmail') IS NULL ALTER TABLE [OrgCampuses] ADD [PrincipalEmail] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[OrgCampuses]', N'PrincipalPhone') IS NULL ALTER TABLE [OrgCampuses] ADD [PrincipalPhone] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[OrgCampuses]', N'EstablishedDate') IS NULL ALTER TABLE [OrgCampuses] ADD [EstablishedDate] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgCampuses]', N'IsActive') IS NULL ALTER TABLE [OrgCampuses] ADD [IsActive] bit NULL;
GO
IF COL_LENGTH(N'[OrgCampuses]', N'CreatedAt') IS NULL ALTER TABLE [OrgCampuses] ADD [CreatedAt] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgCampuses]', N'CreatedBy') IS NULL ALTER TABLE [OrgCampuses] ADD [CreatedBy] int NULL;
GO
IF COL_LENGTH(N'[OrgCampuses]', N'UpdatedAt') IS NULL ALTER TABLE [OrgCampuses] ADD [UpdatedAt] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgCampuses]', N'UpdatedBy') IS NULL ALTER TABLE [OrgCampuses] ADD [UpdatedBy] int NULL;
GO
IF COL_LENGTH(N'[OrgCampuses]', N'IsDeleted') IS NULL ALTER TABLE [OrgCampuses] ADD [IsDeleted] bit NULL;
GO
IF OBJECT_ID(N'[OrgBuildings]', N'U') IS NULL BEGIN CREATE TABLE [OrgBuildings] (
    [BuildingId] int NOT NULL,
    [CampusId] int NOT NULL,
    [BuildingCode] nvarchar(20) NULL,
    [BuildingName] nvarchar(150) NULL,
    [FloorCount] int NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedBy] int NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [UpdatedBy] int NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_OrgBuildings] PRIMARY KEY ([BuildingId])
); END;
GO
IF COL_LENGTH(N'[OrgBuildings]', N'BuildingId') IS NULL ALTER TABLE [OrgBuildings] ADD [BuildingId] int NULL;
GO
IF COL_LENGTH(N'[OrgBuildings]', N'CampusId') IS NULL ALTER TABLE [OrgBuildings] ADD [CampusId] int NULL;
GO
IF COL_LENGTH(N'[OrgBuildings]', N'BuildingCode') IS NULL ALTER TABLE [OrgBuildings] ADD [BuildingCode] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[OrgBuildings]', N'BuildingName') IS NULL ALTER TABLE [OrgBuildings] ADD [BuildingName] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[OrgBuildings]', N'FloorCount') IS NULL ALTER TABLE [OrgBuildings] ADD [FloorCount] int NULL;
GO
IF COL_LENGTH(N'[OrgBuildings]', N'IsActive') IS NULL ALTER TABLE [OrgBuildings] ADD [IsActive] bit NULL;
GO
IF COL_LENGTH(N'[OrgBuildings]', N'CreatedAt') IS NULL ALTER TABLE [OrgBuildings] ADD [CreatedAt] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgBuildings]', N'CreatedBy') IS NULL ALTER TABLE [OrgBuildings] ADD [CreatedBy] int NULL;
GO
IF COL_LENGTH(N'[OrgBuildings]', N'UpdatedAt') IS NULL ALTER TABLE [OrgBuildings] ADD [UpdatedAt] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgBuildings]', N'UpdatedBy') IS NULL ALTER TABLE [OrgBuildings] ADD [UpdatedBy] int NULL;
GO
IF COL_LENGTH(N'[OrgBuildings]', N'IsDeleted') IS NULL ALTER TABLE [OrgBuildings] ADD [IsDeleted] bit NULL;
GO
IF OBJECT_ID(N'[OrgDepartments]', N'U') IS NULL BEGIN CREATE TABLE [OrgDepartments] (
    [DepartmentId] int NOT NULL,
    [CampusId] int NOT NULL,
    [ClientId] int NOT NULL,
    [DepartmentCode] nvarchar(20) NULL,
    [DepartmentName] nvarchar(150) NULL,
    [DepartmentType] nvarchar(50) NULL,
    [HeadName] nvarchar(100) NULL,
    [HeadEmail] nvarchar(100) NULL,
    [HeadPhone] nvarchar(20) NULL,
    [Description] nvarchar(500) NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedBy] int NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [UpdatedBy] int NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_OrgDepartments] PRIMARY KEY ([DepartmentId])
); END;
GO
IF COL_LENGTH(N'[OrgDepartments]', N'DepartmentId') IS NULL ALTER TABLE [OrgDepartments] ADD [DepartmentId] int NULL;
GO
IF COL_LENGTH(N'[OrgDepartments]', N'CampusId') IS NULL ALTER TABLE [OrgDepartments] ADD [CampusId] int NULL;
GO
IF COL_LENGTH(N'[OrgDepartments]', N'ClientId') IS NULL ALTER TABLE [OrgDepartments] ADD [ClientId] int NULL;
GO
IF COL_LENGTH(N'[OrgDepartments]', N'DepartmentCode') IS NULL ALTER TABLE [OrgDepartments] ADD [DepartmentCode] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[OrgDepartments]', N'DepartmentName') IS NULL ALTER TABLE [OrgDepartments] ADD [DepartmentName] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[OrgDepartments]', N'DepartmentType') IS NULL ALTER TABLE [OrgDepartments] ADD [DepartmentType] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[OrgDepartments]', N'HeadName') IS NULL ALTER TABLE [OrgDepartments] ADD [HeadName] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[OrgDepartments]', N'HeadEmail') IS NULL ALTER TABLE [OrgDepartments] ADD [HeadEmail] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[OrgDepartments]', N'HeadPhone') IS NULL ALTER TABLE [OrgDepartments] ADD [HeadPhone] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[OrgDepartments]', N'Description') IS NULL ALTER TABLE [OrgDepartments] ADD [Description] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[OrgDepartments]', N'IsActive') IS NULL ALTER TABLE [OrgDepartments] ADD [IsActive] bit NULL;
GO
IF COL_LENGTH(N'[OrgDepartments]', N'CreatedAt') IS NULL ALTER TABLE [OrgDepartments] ADD [CreatedAt] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgDepartments]', N'CreatedBy') IS NULL ALTER TABLE [OrgDepartments] ADD [CreatedBy] int NULL;
GO
IF COL_LENGTH(N'[OrgDepartments]', N'UpdatedAt') IS NULL ALTER TABLE [OrgDepartments] ADD [UpdatedAt] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgDepartments]', N'UpdatedBy') IS NULL ALTER TABLE [OrgDepartments] ADD [UpdatedBy] int NULL;
GO
IF COL_LENGTH(N'[OrgDepartments]', N'IsDeleted') IS NULL ALTER TABLE [OrgDepartments] ADD [IsDeleted] bit NULL;
GO
IF OBJECT_ID(N'[OrgFloors]', N'U') IS NULL BEGIN CREATE TABLE [OrgFloors] (
    [FloorId] int NOT NULL,
    [BuildingId] int NOT NULL,
    [FloorNumber] int NOT NULL,
    [FloorName] nvarchar(100) NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedBy] int NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [UpdatedBy] int NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_OrgFloors] PRIMARY KEY ([FloorId])
); END;
GO
IF COL_LENGTH(N'[OrgFloors]', N'FloorId') IS NULL ALTER TABLE [OrgFloors] ADD [FloorId] int NULL;
GO
IF COL_LENGTH(N'[OrgFloors]', N'BuildingId') IS NULL ALTER TABLE [OrgFloors] ADD [BuildingId] int NULL;
GO
IF COL_LENGTH(N'[OrgFloors]', N'FloorNumber') IS NULL ALTER TABLE [OrgFloors] ADD [FloorNumber] int NULL;
GO
IF COL_LENGTH(N'[OrgFloors]', N'FloorName') IS NULL ALTER TABLE [OrgFloors] ADD [FloorName] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[OrgFloors]', N'IsActive') IS NULL ALTER TABLE [OrgFloors] ADD [IsActive] bit NULL;
GO
IF COL_LENGTH(N'[OrgFloors]', N'CreatedAt') IS NULL ALTER TABLE [OrgFloors] ADD [CreatedAt] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgFloors]', N'CreatedBy') IS NULL ALTER TABLE [OrgFloors] ADD [CreatedBy] int NULL;
GO
IF COL_LENGTH(N'[OrgFloors]', N'UpdatedAt') IS NULL ALTER TABLE [OrgFloors] ADD [UpdatedAt] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgFloors]', N'UpdatedBy') IS NULL ALTER TABLE [OrgFloors] ADD [UpdatedBy] int NULL;
GO
IF COL_LENGTH(N'[OrgFloors]', N'IsDeleted') IS NULL ALTER TABLE [OrgFloors] ADD [IsDeleted] bit NULL;
GO
IF OBJECT_ID(N'[OrgDesignations]', N'U') IS NULL BEGIN CREATE TABLE [OrgDesignations] (
    [DesignationId] int NOT NULL,
    [DepartmentId] int NOT NULL,
    [ClientId] int NOT NULL,
    [DesignationCode] nvarchar(20) NULL,
    [DesignationName] nvarchar(150) NULL,
    [DesignationType] nvarchar(50) NULL,
    [ReportingToDesignationId] int NULL,
    [Description] nvarchar(500) NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedBy] int NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [UpdatedBy] int NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_OrgDesignations] PRIMARY KEY ([DesignationId])
); END;
GO
IF COL_LENGTH(N'[OrgDesignations]', N'DesignationId') IS NULL ALTER TABLE [OrgDesignations] ADD [DesignationId] int NULL;
GO
IF COL_LENGTH(N'[OrgDesignations]', N'DepartmentId') IS NULL ALTER TABLE [OrgDesignations] ADD [DepartmentId] int NULL;
GO
IF COL_LENGTH(N'[OrgDesignations]', N'ClientId') IS NULL ALTER TABLE [OrgDesignations] ADD [ClientId] int NULL;
GO
IF COL_LENGTH(N'[OrgDesignations]', N'DesignationCode') IS NULL ALTER TABLE [OrgDesignations] ADD [DesignationCode] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[OrgDesignations]', N'DesignationName') IS NULL ALTER TABLE [OrgDesignations] ADD [DesignationName] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[OrgDesignations]', N'DesignationType') IS NULL ALTER TABLE [OrgDesignations] ADD [DesignationType] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[OrgDesignations]', N'ReportingToDesignationId') IS NULL ALTER TABLE [OrgDesignations] ADD [ReportingToDesignationId] int NULL;
GO
IF COL_LENGTH(N'[OrgDesignations]', N'Description') IS NULL ALTER TABLE [OrgDesignations] ADD [Description] nvarchar(500) NULL;
GO
IF COL_LENGTH(N'[OrgDesignations]', N'IsActive') IS NULL ALTER TABLE [OrgDesignations] ADD [IsActive] bit NULL;
GO
IF COL_LENGTH(N'[OrgDesignations]', N'CreatedAt') IS NULL ALTER TABLE [OrgDesignations] ADD [CreatedAt] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgDesignations]', N'CreatedBy') IS NULL ALTER TABLE [OrgDesignations] ADD [CreatedBy] int NULL;
GO
IF COL_LENGTH(N'[OrgDesignations]', N'UpdatedAt') IS NULL ALTER TABLE [OrgDesignations] ADD [UpdatedAt] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgDesignations]', N'UpdatedBy') IS NULL ALTER TABLE [OrgDesignations] ADD [UpdatedBy] int NULL;
GO
IF COL_LENGTH(N'[OrgDesignations]', N'IsDeleted') IS NULL ALTER TABLE [OrgDesignations] ADD [IsDeleted] bit NULL;
GO
IF OBJECT_ID(N'[OrgRooms]', N'U') IS NULL BEGIN CREATE TABLE [OrgRooms] (
    [RoomId] int NOT NULL,
    [FloorId] int NOT NULL,
    [RoomNumber] nvarchar(20) NULL,
    [RoomName] nvarchar(150) NULL,
    [RoomType] nvarchar(50) NULL,
    [Capacity] int NULL,
    [HasProjector] bit NOT NULL,
    [HasSmartBoard] bit NOT NULL,
    [HasACUnit] bit NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedBy] int NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [UpdatedBy] int NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_OrgRooms] PRIMARY KEY ([RoomId])
); END;
GO
IF COL_LENGTH(N'[OrgRooms]', N'RoomId') IS NULL ALTER TABLE [OrgRooms] ADD [RoomId] int NULL;
GO
IF COL_LENGTH(N'[OrgRooms]', N'FloorId') IS NULL ALTER TABLE [OrgRooms] ADD [FloorId] int NULL;
GO
IF COL_LENGTH(N'[OrgRooms]', N'RoomNumber') IS NULL ALTER TABLE [OrgRooms] ADD [RoomNumber] nvarchar(20) NULL;
GO
IF COL_LENGTH(N'[OrgRooms]', N'RoomName') IS NULL ALTER TABLE [OrgRooms] ADD [RoomName] nvarchar(150) NULL;
GO
IF COL_LENGTH(N'[OrgRooms]', N'RoomType') IS NULL ALTER TABLE [OrgRooms] ADD [RoomType] nvarchar(50) NULL;
GO
IF COL_LENGTH(N'[OrgRooms]', N'Capacity') IS NULL ALTER TABLE [OrgRooms] ADD [Capacity] int NULL;
GO
IF COL_LENGTH(N'[OrgRooms]', N'HasProjector') IS NULL ALTER TABLE [OrgRooms] ADD [HasProjector] bit NULL;
GO
IF COL_LENGTH(N'[OrgRooms]', N'HasSmartBoard') IS NULL ALTER TABLE [OrgRooms] ADD [HasSmartBoard] bit NULL;
GO
IF COL_LENGTH(N'[OrgRooms]', N'HasACUnit') IS NULL ALTER TABLE [OrgRooms] ADD [HasACUnit] bit NULL;
GO
IF COL_LENGTH(N'[OrgRooms]', N'IsActive') IS NULL ALTER TABLE [OrgRooms] ADD [IsActive] bit NULL;
GO
IF COL_LENGTH(N'[OrgRooms]', N'CreatedAt') IS NULL ALTER TABLE [OrgRooms] ADD [CreatedAt] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgRooms]', N'CreatedBy') IS NULL ALTER TABLE [OrgRooms] ADD [CreatedBy] int NULL;
GO
IF COL_LENGTH(N'[OrgRooms]', N'UpdatedAt') IS NULL ALTER TABLE [OrgRooms] ADD [UpdatedAt] datetime2 NULL;
GO
IF COL_LENGTH(N'[OrgRooms]', N'UpdatedBy') IS NULL ALTER TABLE [OrgRooms] ADD [UpdatedBy] int NULL;
GO
IF COL_LENGTH(N'[OrgRooms]', N'IsDeleted') IS NULL ALTER TABLE [OrgRooms] ADD [IsDeleted] bit NULL;
GO
IF OBJECT_ID(N'[ArgUsers]', N'U') IS NULL BEGIN CREATE TABLE [ArgUsers] (
    [Id] int NOT NULL IDENTITY,
    [Username] nvarchar(100) NOT NULL,
    [FullName] nvarchar(max) NOT NULL,
    [PasswordHash] nvarchar(max) NOT NULL,
    [ClientId] int NOT NULL,
    [EntityId] int NOT NULL,
    [EmployeeId] int NULL,
    [Blocked] bit NOT NULL,
    [SessionVersion] int NULL,
    CONSTRAINT [PK_ArgUsers] PRIMARY KEY ([Id])
); END;
GO
IF COL_LENGTH(N'[ArgUsers]', N'Id') IS NULL ALTER TABLE [ArgUsers] ADD [Id] int NULL ;
GO
IF COL_LENGTH(N'[ArgUsers]', N'Username') IS NULL ALTER TABLE [ArgUsers] ADD [Username] nvarchar(100) NULL;
GO
IF COL_LENGTH(N'[ArgUsers]', N'FullName') IS NULL ALTER TABLE [ArgUsers] ADD [FullName] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[ArgUsers]', N'PasswordHash') IS NULL ALTER TABLE [ArgUsers] ADD [PasswordHash] nvarchar(max) NULL;
GO
IF COL_LENGTH(N'[ArgUsers]', N'ClientId') IS NULL ALTER TABLE [ArgUsers] ADD [ClientId] int NULL;
GO
IF COL_LENGTH(N'[ArgUsers]', N'EntityId') IS NULL ALTER TABLE [ArgUsers] ADD [EntityId] int NULL;
GO
IF COL_LENGTH(N'[ArgUsers]', N'EmployeeId') IS NULL ALTER TABLE [ArgUsers] ADD [EmployeeId] int NULL;
GO
IF COL_LENGTH(N'[ArgUsers]', N'Blocked') IS NULL ALTER TABLE [ArgUsers] ADD [Blocked] bit NULL;
GO
IF COL_LENGTH(N'[ArgUsers]', N'SessionVersion') IS NULL ALTER TABLE [ArgUsers] ADD [SessionVersion] int NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_WF_Tasks_WF_Instances_InstanceId') ALTER TABLE [WF_Tasks] ADD CONSTRAINT [FK_WF_Tasks_WF_Instances_InstanceId] FOREIGN KEY ([InstanceId]) REFERENCES [WF_Instances] ([Id]) ON DELETE NO ACTION;
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EPDeductiongrpdtl_EPDeductiongrphdr_dedgrpid') ALTER TABLE [EPDeductiongrpdtl] ADD CONSTRAINT [FK_EPDeductiongrpdtl_EPDeductiongrphdr_dedgrpid] FOREIGN KEY ([dedgrpid]) REFERENCES [EPDeductiongrphdr] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EPEarninggrpdtl_EPEarninggrphdr_earngrpid') ALTER TABLE [EPEarninggrpdtl] ADD CONSTRAINT [FK_EPEarninggrpdtl_EPEarninggrphdr_earngrpid] FOREIGN KEY ([earngrpid]) REFERENCES [EPEarninggrphdr] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_ERDesgPayAllowanceDtl_EPEarningtype_earntypeid') ALTER TABLE [ERDesgPayAllowanceDtl] ADD CONSTRAINT [FK_ERDesgPayAllowanceDtl_EPEarningtype_earntypeid] FOREIGN KEY ([earntypeid]) REFERENCES [EPEarningtype] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_ERDesgPayAllowanceDtl_ERDesgPayroll_designationpayrollid') ALTER TABLE [ERDesgPayAllowanceDtl] ADD CONSTRAINT [FK_ERDesgPayAllowanceDtl_ERDesgPayroll_designationpayrollid] FOREIGN KEY ([designationpayrollid]) REFERENCES [ERDesgPayroll] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_ERDesgPayBaseSalaryDtl_EPEarningtype_earntypeid') ALTER TABLE [ERDesgPayBaseSalaryDtl] ADD CONSTRAINT [FK_ERDesgPayBaseSalaryDtl_EPEarningtype_earntypeid] FOREIGN KEY ([earntypeid]) REFERENCES [EPEarningtype] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_ERDesgPayBaseSalaryDtl_ERDesgPayroll_designationpayrollid') ALTER TABLE [ERDesgPayBaseSalaryDtl] ADD CONSTRAINT [FK_ERDesgPayBaseSalaryDtl_ERDesgPayroll_designationpayrollid] FOREIGN KEY ([designationpayrollid]) REFERENCES [ERDesgPayroll] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_ERDesgPayDeductionDtl_EPDeductiontype_deductiontypeid') ALTER TABLE [ERDesgPayDeductionDtl] ADD CONSTRAINT [FK_ERDesgPayDeductionDtl_EPDeductiontype_deductiontypeid] FOREIGN KEY ([deductiontypeid]) REFERENCES [EPDeductiontype] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_ERDesgPayDeductionDtl_ERDesgPayroll_designationpayrollid') ALTER TABLE [ERDesgPayDeductionDtl] ADD CONSTRAINT [FK_ERDesgPayDeductionDtl_ERDesgPayroll_designationpayrollid] FOREIGN KEY ([designationpayrollid]) REFERENCES [ERDesgPayroll] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_ERDesgPayLoanType_EPLoantype_loantypeid') ALTER TABLE [ERDesgPayLoanType] ADD CONSTRAINT [FK_ERDesgPayLoanType_EPLoantype_loantypeid] FOREIGN KEY ([loantypeid]) REFERENCES [EPLoantype] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_ERDesgPayLoanType_ERDesgPayroll_designationpayrollid') ALTER TABLE [ERDesgPayLoanType] ADD CONSTRAINT [FK_ERDesgPayLoanType_ERDesgPayroll_designationpayrollid] FOREIGN KEY ([designationpayrollid]) REFERENCES [ERDesgPayroll] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_ERSkillratingdtl_ERSkillratinghdr_ratingid') ALTER TABLE [ERSkillratingdtl] ADD CONSTRAINT [FK_ERSkillratingdtl_ERSkillratinghdr_ratingid] FOREIGN KEY ([ratingid]) REFERENCES [ERSkillratinghdr] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_ETCalholidayapplyondtl_ETCalholidayhdr_calholidayid') ALTER TABLE [ETCalholidayapplyondtl] ADD CONSTRAINT [FK_ETCalholidayapplyondtl_ETCalholidayhdr_calholidayid] FOREIGN KEY ([calholidayid]) REFERENCES [ETCalholidayhdr] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_ETCalholidaydtl_ETCalholidayhdr_calholidayid') ALTER TABLE [ETCalholidaydtl] ADD CONSTRAINT [FK_ETCalholidaydtl_ETCalholidayhdr_calholidayid] FOREIGN KEY ([calholidayid]) REFERENCES [ETCalholidayhdr] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_ETCalholidayshareddtl_ETCalholidayhdr_calholidayid') ALTER TABLE [ETCalholidayshareddtl] ADD CONSTRAINT [FK_ETCalholidayshareddtl_ETCalholidayhdr_calholidayid] FOREIGN KEY ([calholidayid]) REFERENCES [ETCalholidayhdr] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_ETLeaveadjpolicydtl_ETLeaveadjpolicy_leaveadjid') ALTER TABLE [ETLeaveadjpolicydtl] ADD CONSTRAINT [FK_ETLeaveadjpolicydtl_ETLeaveadjpolicy_leaveadjid] FOREIGN KEY ([leaveadjid]) REFERENCES [ETLeaveadjpolicy] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_ETLeavegroupdtl_ETLeavegrouphdr_leavegrpid') ALTER TABLE [ETLeavegroupdtl] ADD CONSTRAINT [FK_ETLeavegroupdtl_ETLeavegrouphdr_leavegrpid] FOREIGN KEY ([leavegrpid]) REFERENCES [ETLeavegrouphdr] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_ETPayperioddtl_ETPayperiodhdr_payperiodid') ALTER TABLE [ETPayperioddtl] ADD CONSTRAINT [FK_ETPayperioddtl_ETPayperiodhdr_payperiodid] FOREIGN KEY ([payperiodid]) REFERENCES [ETPayperiodhdr] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_ETAbsentpolicydtl_ETPolicyhdr_policyid') ALTER TABLE [ETAbsentpolicydtl] ADD CONSTRAINT [FK_ETAbsentpolicydtl_ETPolicyhdr_policyid] FOREIGN KEY ([policyid]) REFERENCES [ETPolicyhdr] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_ETPolicydtl_ETPolicyhdr_policyid') ALTER TABLE [ETPolicydtl] ADD CONSTRAINT [FK_ETPolicydtl_ETPolicyhdr_policyid] FOREIGN KEY ([policyid]) REFERENCES [ETPolicyhdr] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EREmployeeinfohdr_OrgDepartment_depid') ALTER TABLE [EREmployeeinfohdr] ADD CONSTRAINT [FK_EREmployeeinfohdr_OrgDepartment_depid] FOREIGN KEY ([depid]) REFERENCES [OrgDepartment] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EREmployeeinfohdr_OrgDesignation_designationid') ALTER TABLE [EREmployeeinfohdr] ADD CONSTRAINT [FK_EREmployeeinfohdr_OrgDesignation_designationid] FOREIGN KEY ([designationid]) REFERENCES [OrgDesignation] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_HRPosition_HRPosition_ReportsToPositionId') ALTER TABLE [HRPosition] ADD CONSTRAINT [FK_HRPosition_HRPosition_ReportsToPositionId] FOREIGN KEY ([ReportsToPositionId]) REFERENCES [HRPosition] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_HRPosition_OrgDepartment_DepartmentId') ALTER TABLE [HRPosition] ADD CONSTRAINT [FK_HRPosition_OrgDepartment_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [OrgDepartment] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_HRPosition_OrgDesignation_DesignationId') ALTER TABLE [HRPosition] ADD CONSTRAINT [FK_HRPosition_OrgDesignation_DesignationId] FOREIGN KEY ([DesignationId]) REFERENCES [OrgDesignation] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_HRPosition_OrgLegalentity_EntityId') ALTER TABLE [HRPosition] ADD CONSTRAINT [FK_HRPosition_OrgLegalentity_EntityId] FOREIGN KEY ([EntityId]) REFERENCES [OrgLegalentity] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_HRPosition_lov_LovStatusId') ALTER TABLE [HRPosition] ADD CONSTRAINT [FK_HRPosition_lov_LovStatusId] FOREIGN KEY ([LovStatusId]) REFERENCES [lov] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EPLoanrequest_EPLoantype_loantypeid') ALTER TABLE [EPLoanrequest] ADD CONSTRAINT [FK_EPLoanrequest_EPLoantype_loantypeid] FOREIGN KEY ([loantypeid]) REFERENCES [EPLoantype] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EPLoanrequest_EREmployeeinfohdr_employeeid') ALTER TABLE [EPLoanrequest] ADD CONSTRAINT [FK_EPLoanrequest_EREmployeeinfohdr_employeeid] FOREIGN KEY ([employeeid]) REFERENCES [EREmployeeinfohdr] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EREmpBank_EREmployeeinfohdr_employeeid') ALTER TABLE [EREmpBank] ADD CONSTRAINT [FK_EREmpBank_EREmployeeinfohdr_employeeid] FOREIGN KEY ([employeeid]) REFERENCES [EREmployeeinfohdr] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EREmpBank_FMBankgrp_bankgrpid') ALTER TABLE [EREmpBank] ADD CONSTRAINT [FK_EREmpBank_FMBankgrp_bankgrpid] FOREIGN KEY ([bankgrpid]) REFERENCES [FMBankgrp] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EREmpCards_EREmployeeinfohdr_employeeid') ALTER TABLE [EREmpCards] ADD CONSTRAINT [FK_EREmpCards_EREmployeeinfohdr_employeeid] FOREIGN KEY ([employeeid]) REFERENCES [EREmployeeinfohdr] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EREmpCards_lov_authorityid') ALTER TABLE [EREmpCards] ADD CONSTRAINT [FK_EREmpCards_lov_authorityid] FOREIGN KEY ([authorityid]) REFERENCES [lov] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EREmpCards_lov_cardtypeid') ALTER TABLE [EREmpCards] ADD CONSTRAINT [FK_EREmpCards_lov_cardtypeid] FOREIGN KEY ([cardtypeid]) REFERENCES [lov] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EREmpExperience_EREmployeeinfohdr_employeeid') ALTER TABLE [EREmpExperience] ADD CONSTRAINT [FK_EREmpExperience_EREmployeeinfohdr_employeeid] FOREIGN KEY ([employeeid]) REFERENCES [EREmployeeinfohdr] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EREmpExperience_lov_busnatureid') ALTER TABLE [EREmpExperience] ADD CONSTRAINT [FK_EREmpExperience_lov_busnatureid] FOREIGN KEY ([busnatureid]) REFERENCES [lov] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EREmployeedisease_EREmployeeinfohdr_employeeid') ALTER TABLE [EREmployeedisease] ADD CONSTRAINT [FK_EREmployeedisease_EREmployeeinfohdr_employeeid] FOREIGN KEY ([employeeid]) REFERENCES [EREmployeeinfohdr] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EREmployeedisease_SMDisease_diseasesid') ALTER TABLE [EREmployeedisease] ADD CONSTRAINT [FK_EREmployeedisease_SMDisease_diseasesid] FOREIGN KEY ([diseasesid]) REFERENCES [SMDisease] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EREmployeedisease_lov_discatid') ALTER TABLE [EREmployeedisease] ADD CONSTRAINT [FK_EREmployeedisease_lov_discatid] FOREIGN KEY ([discatid]) REFERENCES [lov] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EREmployeedisease_lov_distypeid') ALTER TABLE [EREmployeedisease] ADD CONSTRAINT [FK_EREmployeedisease_lov_distypeid] FOREIGN KEY ([distypeid]) REFERENCES [lov] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EREmployeesubjectskill_EREmployeeinfohdr_employeeid') ALTER TABLE [EREmployeesubjectskill] ADD CONSTRAINT [FK_EREmployeesubjectskill_EREmployeeinfohdr_employeeid] FOREIGN KEY ([employeeid]) REFERENCES [EREmployeeinfohdr] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EREmployeesubjectskill_ERSkillratingdtl_ratingdtlid') ALTER TABLE [EREmployeesubjectskill] ADD CONSTRAINT [FK_EREmployeesubjectskill_ERSkillratingdtl_ratingdtlid] FOREIGN KEY ([ratingdtlid]) REFERENCES [ERSkillratingdtl] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EREmployeesubjectskill_SMSubjecttype_subjecttypeid') ALTER TABLE [EREmployeesubjectskill] ADD CONSTRAINT [FK_EREmployeesubjectskill_SMSubjecttype_subjecttypeid] FOREIGN KEY ([subjecttypeid]) REFERENCES [SMSubjecttype] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EREmppayroll_EREmployeeinfohdr_employeeid') ALTER TABLE [EREmppayroll] ADD CONSTRAINT [FK_EREmppayroll_EREmployeeinfohdr_employeeid] FOREIGN KEY ([employeeid]) REFERENCES [EREmployeeinfohdr] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EREmpPosition_EREmployeeinfohdr_employeeid') ALTER TABLE [EREmpPosition] ADD CONSTRAINT [FK_EREmpPosition_EREmployeeinfohdr_employeeid] FOREIGN KEY ([employeeid]) REFERENCES [EREmployeeinfohdr] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EREmpPosition_OrgDepartment_deptid') ALTER TABLE [EREmpPosition] ADD CONSTRAINT [FK_EREmpPosition_OrgDepartment_deptid] FOREIGN KEY ([deptid]) REFERENCES [OrgDepartment] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EREmpPosition_OrgDesignation_desgid') ALTER TABLE [EREmpPosition] ADD CONSTRAINT [FK_EREmpPosition_OrgDesignation_desgid] FOREIGN KEY ([desgid]) REFERENCES [OrgDesignation] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EREmpRelation_EREmployeeinfohdr_employeeid') ALTER TABLE [EREmpRelation] ADD CONSTRAINT [FK_EREmpRelation_EREmployeeinfohdr_employeeid] FOREIGN KEY ([employeeid]) REFERENCES [EREmployeeinfohdr] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EREmpRelation_SMRelationtype_relationtypeid') ALTER TABLE [EREmpRelation] ADD CONSTRAINT [FK_EREmpRelation_SMRelationtype_relationtypeid] FOREIGN KEY ([relationtypeid]) REFERENCES [SMRelationtype] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EREmpRelation_lov_genderid') ALTER TABLE [EREmpRelation] ADD CONSTRAINT [FK_EREmpRelation_lov_genderid] FOREIGN KEY ([genderid]) REFERENCES [lov] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EREmproaster_EREmployeeinfohdr_employeeid') ALTER TABLE [EREmproaster] ADD CONSTRAINT [FK_EREmproaster_EREmployeeinfohdr_employeeid] FOREIGN KEY ([employeeid]) REFERENCES [EREmployeeinfohdr] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EREmproaster_ETLeavegrouphdr_leavegrpid') ALTER TABLE [EREmproaster] ADD CONSTRAINT [FK_EREmproaster_ETLeavegrouphdr_leavegrpid] FOREIGN KEY ([leavegrpid]) REFERENCES [ETLeavegrouphdr] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EREmproaster_ETShifthdr_shiftid') ALTER TABLE [EREmproaster] ADD CONSTRAINT [FK_EREmproaster_ETShifthdr_shiftid] FOREIGN KEY ([shiftid]) REFERENCES [ETShifthdr] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EREmproaster_orgincomtaxhdr_taxid') ALTER TABLE [EREmproaster] ADD CONSTRAINT [FK_EREmproaster_orgincomtaxhdr_taxid] FOREIGN KEY ([taxid]) REFERENCES [orgincomtaxhdr] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EREmpSkill_EREmployeeinfohdr_employeeid') ALTER TABLE [EREmpSkill] ADD CONSTRAINT [FK_EREmpSkill_EREmployeeinfohdr_employeeid] FOREIGN KEY ([employeeid]) REFERENCES [EREmployeeinfohdr] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EREmpSkill_ERSkill_skillid') ALTER TABLE [EREmpSkill] ADD CONSTRAINT [FK_EREmpSkill_ERSkill_skillid] FOREIGN KEY ([skillid]) REFERENCES [ERSkill] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EREmpSkill_ERSkillratingdtl_skillratingdtlid') ALTER TABLE [EREmpSkill] ADD CONSTRAINT [FK_EREmpSkill_ERSkillratingdtl_skillratingdtlid] FOREIGN KEY ([skillratingdtlid]) REFERENCES [ERSkillratingdtl] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EREmpSkill_ERSkillratinghdr_skillratingid') ALTER TABLE [EREmpSkill] ADD CONSTRAINT [FK_EREmpSkill_ERSkillratinghdr_skillratingid] FOREIGN KEY ([skillratingid]) REFERENCES [ERSkillratinghdr] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EREmpSkill_ERSkilltype_skilltypeid') ALTER TABLE [EREmpSkill] ADD CONSTRAINT [FK_EREmpSkill_ERSkilltype_skilltypeid] FOREIGN KEY ([skilltypeid]) REFERENCES [ERSkilltype] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EREmpSkill_lov_punitid') ALTER TABLE [EREmpSkill] ADD CONSTRAINT [FK_EREmpSkill_lov_punitid] FOREIGN KEY ([punitid]) REFERENCES [lov] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_ETLeaveRequest_EREmployeeinfohdr_employeeid') ALTER TABLE [ETLeaveRequest] ADD CONSTRAINT [FK_ETLeaveRequest_EREmployeeinfohdr_employeeid] FOREIGN KEY ([employeeid]) REFERENCES [EREmployeeinfohdr] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_ETLeaveRequest_ETLeavetype_leavetypeid') ALTER TABLE [ETLeaveRequest] ADD CONSTRAINT [FK_ETLeaveRequest_ETLeavetype_leavetypeid] FOREIGN KEY ([leavetypeid]) REFERENCES [ETLeavetype] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_HREmployeeInformationRecord_EREmployeeinfohdr_employeeid') ALTER TABLE [HREmployeeInformationRecord] ADD CONSTRAINT [FK_HREmployeeInformationRecord_EREmployeeinfohdr_employeeid] FOREIGN KEY ([employeeid]) REFERENCES [EREmployeeinfohdr] ([Id]) ON DELETE NO ACTION;
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_HRMonthlyAttendance_EREmployeeinfohdr_employeeid') ALTER TABLE [HRMonthlyAttendance] ADD CONSTRAINT [FK_HRMonthlyAttendance_EREmployeeinfohdr_employeeid] FOREIGN KEY ([employeeid]) REFERENCES [EREmployeeinfohdr] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_HRpayrollprocess_EREmployeeinfohdr_employeeid') ALTER TABLE [HRpayrollprocess] ADD CONSTRAINT [FK_HRpayrollprocess_EREmployeeinfohdr_employeeid] FOREIGN KEY ([employeeid]) REFERENCES [EREmployeeinfohdr] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_HRVacancyAudit_EREmployeeinfohdr_ActorEmployeeId') ALTER TABLE [HRVacancyAudit] ADD CONSTRAINT [FK_HRVacancyAudit_EREmployeeinfohdr_ActorEmployeeId] FOREIGN KEY ([ActorEmployeeId]) REFERENCES [EREmployeeinfohdr] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_HRVacancyPosition_EREmployeeinfohdr_CurrentEmployeeId') ALTER TABLE [HRVacancyPosition] ADD CONSTRAINT [FK_HRVacancyPosition_EREmployeeinfohdr_CurrentEmployeeId] FOREIGN KEY ([CurrentEmployeeId]) REFERENCES [EREmployeeinfohdr] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_HRVacancyPositionAssignment_EREmployeeinfohdr_EmployeeId') ALTER TABLE [HRVacancyPositionAssignment] ADD CONSTRAINT [FK_HRVacancyPositionAssignment_EREmployeeinfohdr_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [EREmployeeinfohdr] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_HRVacancyRecruiter_EREmployeeinfohdr_EmployeeId') ALTER TABLE [HRVacancyRecruiter] ADD CONSTRAINT [FK_HRVacancyRecruiter_EREmployeeinfohdr_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [EREmployeeinfohdr] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_orgsubcordinate_EREmployeeinfohdr_empid') ALTER TABLE [orgsubcordinate] ADD CONSTRAINT [FK_orgsubcordinate_EREmployeeinfohdr_empid] FOREIGN KEY ([empid]) REFERENCES [EREmployeeinfohdr] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_orgsubcordinate_OrgLegalentity_subcampus') ALTER TABLE [orgsubcordinate] ADD CONSTRAINT [FK_orgsubcordinate_OrgLegalentity_subcampus] FOREIGN KEY ([subcampus]) REFERENCES [OrgLegalentity] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_SMAcademic_EREmployeeinfohdr_employeeid') ALTER TABLE [SMAcademic] ADD CONSTRAINT [FK_SMAcademic_EREmployeeinfohdr_employeeid] FOREIGN KEY ([employeeid]) REFERENCES [EREmployeeinfohdr] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_SMAcademic_ERQualification_qualificationid') ALTER TABLE [SMAcademic] ADD CONSTRAINT [FK_SMAcademic_ERQualification_qualificationid] FOREIGN KEY ([qualificationid]) REFERENCES [ERQualification] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_SMAcademic_ERQualificationtype_qualificationtypeid') ALTER TABLE [SMAcademic] ADD CONSTRAINT [FK_SMAcademic_ERQualificationtype_qualificationtypeid] FOREIGN KEY ([qualificationtypeid]) REFERENCES [ERQualificationtype] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_HRPositionAssignment_EREmployeeinfohdr_EmployeeId') ALTER TABLE [HRPositionAssignment] ADD CONSTRAINT [FK_HRPositionAssignment_EREmployeeinfohdr_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [EREmployeeinfohdr] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_HRPositionAssignment_HRPosition_PositionId') ALTER TABLE [HRPositionAssignment] ADD CONSTRAINT [FK_HRPositionAssignment_HRPosition_PositionId] FOREIGN KEY ([PositionId]) REFERENCES [HRPosition] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_HRPositionAssignment_lov_LovStatusId') ALTER TABLE [HRPositionAssignment] ADD CONSTRAINT [FK_HRPositionAssignment_lov_LovStatusId] FOREIGN KEY ([LovStatusId]) REFERENCES [lov] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_HRPositionAudit_EREmployeeinfohdr_ActorEmployeeId') ALTER TABLE [HRPositionAudit] ADD CONSTRAINT [FK_HRPositionAudit_EREmployeeinfohdr_ActorEmployeeId] FOREIGN KEY ([ActorEmployeeId]) REFERENCES [EREmployeeinfohdr] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_HRPositionAudit_HRPosition_PositionId') ALTER TABLE [HRPositionAudit] ADD CONSTRAINT [FK_HRPositionAudit_HRPosition_PositionId] FOREIGN KEY ([PositionId]) REFERENCES [HRPosition] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_HRVacancy_EREmployeeinfohdr_ApproverEmployeeId') ALTER TABLE [HRVacancy] ADD CONSTRAINT [FK_HRVacancy_EREmployeeinfohdr_ApproverEmployeeId] FOREIGN KEY ([ApproverEmployeeId]) REFERENCES [EREmployeeinfohdr] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_HRVacancy_EREmployeeinfohdr_HiringManagerEmployeeId') ALTER TABLE [HRVacancy] ADD CONSTRAINT [FK_HRVacancy_EREmployeeinfohdr_HiringManagerEmployeeId] FOREIGN KEY ([HiringManagerEmployeeId]) REFERENCES [EREmployeeinfohdr] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_HRVacancy_HRPosition_ReplacementPositionId') ALTER TABLE [HRVacancy] ADD CONSTRAINT [FK_HRVacancy_HRPosition_ReplacementPositionId] FOREIGN KEY ([ReplacementPositionId]) REFERENCES [HRPosition] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_HRVacancy_HRPosition_ReportsToPositionId') ALTER TABLE [HRVacancy] ADD CONSTRAINT [FK_HRVacancy_HRPosition_ReportsToPositionId] FOREIGN KEY ([ReportsToPositionId]) REFERENCES [HRPosition] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_HRVacancy_OrgLegalentity_EntityId') ALTER TABLE [HRVacancy] ADD CONSTRAINT [FK_HRVacancy_OrgLegalentity_EntityId] FOREIGN KEY ([EntityId]) REFERENCES [OrgLegalentity] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EPLoantranshdr_EPLoanrequest_requestid') ALTER TABLE [EPLoantranshdr] ADD CONSTRAINT [FK_EPLoantranshdr_EPLoanrequest_requestid] FOREIGN KEY ([requestid]) REFERENCES [EPLoanrequest] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EPLoantranshdr_EREmployeeinfohdr_employeeid') ALTER TABLE [EPLoantranshdr] ADD CONSTRAINT [FK_EPLoantranshdr_EREmployeeinfohdr_employeeid] FOREIGN KEY ([employeeid]) REFERENCES [EREmployeeinfohdr] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EPLoantranshdr_lov_TrusteeStatusId') ALTER TABLE [EPLoantranshdr] ADD CONSTRAINT [FK_EPLoantranshdr_lov_TrusteeStatusId] FOREIGN KEY ([TrusteeStatusId]) REFERENCES [lov] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EREmppayallowancedtl_EPEarningtype_earntypeid') ALTER TABLE [EREmppayallowancedtl] ADD CONSTRAINT [FK_EREmppayallowancedtl_EPEarningtype_earntypeid] FOREIGN KEY ([earntypeid]) REFERENCES [EPEarningtype] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EREmppayallowancedtl_EREmppayroll_emppayrollid') ALTER TABLE [EREmppayallowancedtl] ADD CONSTRAINT [FK_EREmppayallowancedtl_EREmppayroll_emppayrollid] FOREIGN KEY ([emppayrollid]) REFERENCES [EREmppayroll] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EREmppaybasesalarydtl_EPEarningtype_earntypeid') ALTER TABLE [EREmppaybasesalarydtl] ADD CONSTRAINT [FK_EREmppaybasesalarydtl_EPEarningtype_earntypeid] FOREIGN KEY ([earntypeid]) REFERENCES [EPEarningtype] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EREmppaybasesalarydtl_EREmppayroll_emppayrollid') ALTER TABLE [EREmppaybasesalarydtl] ADD CONSTRAINT [FK_EREmppaybasesalarydtl_EREmppayroll_emppayrollid] FOREIGN KEY ([emppayrollid]) REFERENCES [EREmppayroll] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EREmppaydeductiondtl_EPDeductiontype_deductiontypeid') ALTER TABLE [EREmppaydeductiondtl] ADD CONSTRAINT [FK_EREmppaydeductiondtl_EPDeductiontype_deductiontypeid] FOREIGN KEY ([deductiontypeid]) REFERENCES [EPDeductiontype] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EREmppaydeductiondtl_EREmppayroll_emppayrollid') ALTER TABLE [EREmppaydeductiondtl] ADD CONSTRAINT [FK_EREmppaydeductiondtl_EREmppayroll_emppayrollid] FOREIGN KEY ([emppayrollid]) REFERENCES [EREmppayroll] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EREmppayrollLoanType_EPLoantype_loantypeid') ALTER TABLE [EREmppayrollLoanType] ADD CONSTRAINT [FK_EREmppayrollLoanType_EPLoantype_loantypeid] FOREIGN KEY ([loantypeid]) REFERENCES [EPLoantype] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EREmppayrollLoanType_EREmppayroll_emppayrollid') ALTER TABLE [EREmppayrollLoanType] ADD CONSTRAINT [FK_EREmppayrollLoanType_EREmppayroll_emppayrollid] FOREIGN KEY ([emppayrollid]) REFERENCES [EREmppayroll] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_ETLeavetrans_EREmployeeinfohdr_employeeid') ALTER TABLE [ETLeavetrans] ADD CONSTRAINT [FK_ETLeavetrans_EREmployeeinfohdr_employeeid] FOREIGN KEY ([employeeid]) REFERENCES [EREmployeeinfohdr] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_ETLeavetrans_ETLeaveRequest_requestid') ALTER TABLE [ETLeavetrans] ADD CONSTRAINT [FK_ETLeavetrans_ETLeaveRequest_requestid] FOREIGN KEY ([requestid]) REFERENCES [ETLeaveRequest] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_ETLeavetrans_ETLeavetype_leavetypeid') ALTER TABLE [ETLeavetrans] ADD CONSTRAINT [FK_ETLeavetrans_ETLeavetype_leavetypeid] FOREIGN KEY ([leavetypeid]) REFERENCES [ETLeavetype] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_HRpayrollprocessdtl_HRpayrollprocess_processid') ALTER TABLE [HRpayrollprocessdtl] ADD CONSTRAINT [FK_HRpayrollprocessdtl_HRpayrollprocess_processid] FOREIGN KEY ([processid]) REFERENCES [HRpayrollprocess] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_EPLoantransdtl_EPLoantranshdr_loanid') ALTER TABLE [EPLoantransdtl] ADD CONSTRAINT [FK_EPLoantransdtl_EPLoantranshdr_loanid] FOREIGN KEY ([loanid]) REFERENCES [EPLoantranshdr] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_OrgRegion_Country_CountryId') ALTER TABLE [OrgRegion] ADD CONSTRAINT [FK_OrgRegion_Country_CountryId] FOREIGN KEY ([CountryId]) REFERENCES [Country] ([Id]) ON DELETE NO ACTION;
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_State_Country_countryid') ALTER TABLE [State] ADD CONSTRAINT [FK_State_Country_countryid] FOREIGN KEY ([countryid]) REFERENCES [Country] ([Id]) ON DELETE NO ACTION;
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_OrgDepartment_FICOA_NEW_ficoaid') ALTER TABLE [OrgDepartment] ADD CONSTRAINT [FK_OrgDepartment_FICOA_NEW_ficoaid] FOREIGN KEY ([ficoaid]) REFERENCES [FICOA_NEW] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_OrgDepartment_FICOA_NEW_ficoaid_rec') ALTER TABLE [OrgDepartment] ADD CONSTRAINT [FK_OrgDepartment_FICOA_NEW_ficoaid_rec] FOREIGN KEY ([ficoaid_rec]) REFERENCES [FICOA_NEW] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_FOCalllog_FOPurpose_purposetypeid') ALTER TABLE [FOCalllog] ADD CONSTRAINT [FK_FOCalllog_FOPurpose_purposetypeid] FOREIGN KEY ([purposetypeid]) REFERENCES [FOPurpose] ([id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_FOPostaldispatch_lov_courierid') ALTER TABLE [FOPostaldispatch] ADD CONSTRAINT [FK_FOPostaldispatch_lov_courierid] FOREIGN KEY ([courierid]) REFERENCES [lov] ([Id]) ON DELETE NO ACTION;
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_SMAcademicYear_SMAcademicregister_registerid') ALTER TABLE [SMAcademicYear] ADD CONSTRAINT [FK_SMAcademicYear_SMAcademicregister_registerid] FOREIGN KEY ([registerid]) REFERENCES [SMAcademicregister] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_SMPublishdocumentdtl_SMPublishdocumenthdr_publishid') ALTER TABLE [SMPublishdocumentdtl] ADD CONSTRAINT [FK_SMPublishdocumentdtl_SMPublishdocumenthdr_publishid] FOREIGN KEY ([publishid]) REFERENCES [SMPublishdocumenthdr] ([Id]) ON DELETE CASCADE;
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_Orgsite_OrgRegion_regionid') ALTER TABLE [Orgsite] ADD CONSTRAINT [FK_Orgsite_OrgRegion_regionid] FOREIGN KEY ([regionid]) REFERENCES [OrgRegion] ([Id]) ON DELETE NO ACTION;
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_Orgsite_Orgsite_siteid') ALTER TABLE [Orgsite] ADD CONSTRAINT [FK_Orgsite_Orgsite_siteid] FOREIGN KEY ([siteid]) REFERENCES [Orgsite] ([Id]) ON DELETE NO ACTION;
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_City_Country_ctryid') ALTER TABLE [City] ADD CONSTRAINT [FK_City_Country_ctryid] FOREIGN KEY ([ctryid]) REFERENCES [Country] ([Id]) ON DELETE NO ACTION;
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_City_State_stateid') ALTER TABLE [City] ADD CONSTRAINT [FK_City_State_stateid] FOREIGN KEY ([stateid]) REFERENCES [State] ([Id]) ON DELETE NO ACTION;
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_SMAcademicYeardtl_SMAcademicYear_acadyrid') ALTER TABLE [SMAcademicYeardtl] ADD CONSTRAINT [FK_SMAcademicYeardtl_SMAcademicYear_acadyrid] FOREIGN KEY ([acadyrid]) REFERENCES [SMAcademicYear] ([Id]) ON DELETE CASCADE;
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_Districts_City_CityId') ALTER TABLE [Districts] ADD CONSTRAINT [FK_Districts_City_CityId] FOREIGN KEY ([CityId]) REFERENCES [City] ([Id]) ON DELETE NO ACTION;
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_OrgEntities_City_CityId') ALTER TABLE [OrgEntities] ADD CONSTRAINT [FK_OrgEntities_City_CityId] FOREIGN KEY ([CityId]) REFERENCES [City] ([Id]) ON DELETE NO ACTION;
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_OrgEntities_Country_CountryId') ALTER TABLE [OrgEntities] ADD CONSTRAINT [FK_OrgEntities_Country_CountryId] FOREIGN KEY ([CountryId]) REFERENCES [Country] ([Id]) ON DELETE NO ACTION;
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_OrgEntities_State_StateId') ALTER TABLE [OrgEntities] ADD CONSTRAINT [FK_OrgEntities_State_StateId] FOREIGN KEY ([StateId]) REFERENCES [State] ([Id]) ON DELETE NO ACTION;
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_OrgEntities_TerminologySet_TerminologySetId') ALTER TABLE [OrgEntities] ADD CONSTRAINT [FK_OrgEntities_TerminologySet_TerminologySetId] FOREIGN KEY ([TerminologySetId]) REFERENCES [TerminologySet] ([Id]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_OrgCampuses_City_CityId') ALTER TABLE [OrgCampuses] ADD CONSTRAINT [FK_OrgCampuses_City_CityId] FOREIGN KEY ([CityId]) REFERENCES [City] ([Id]) ON DELETE NO ACTION;
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_OrgCampuses_Country_CountryId') ALTER TABLE [OrgCampuses] ADD CONSTRAINT [FK_OrgCampuses_Country_CountryId] FOREIGN KEY ([CountryId]) REFERENCES [Country] ([Id]) ON DELETE NO ACTION;
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_OrgCampuses_OrgEntities_EntityId') ALTER TABLE [OrgCampuses] ADD CONSTRAINT [FK_OrgCampuses_OrgEntities_EntityId] FOREIGN KEY ([EntityId]) REFERENCES [OrgEntities] ([EntityId]) ON DELETE NO ACTION;
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_OrgCampuses_State_StateId') ALTER TABLE [OrgCampuses] ADD CONSTRAINT [FK_OrgCampuses_State_StateId] FOREIGN KEY ([StateId]) REFERENCES [State] ([Id]) ON DELETE NO ACTION;
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_OrgBuildings_OrgCampuses_CampusId') ALTER TABLE [OrgBuildings] ADD CONSTRAINT [FK_OrgBuildings_OrgCampuses_CampusId] FOREIGN KEY ([CampusId]) REFERENCES [OrgCampuses] ([CampusId]) ON DELETE CASCADE;
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_OrgDepartments_OrgCampuses_CampusId') ALTER TABLE [OrgDepartments] ADD CONSTRAINT [FK_OrgDepartments_OrgCampuses_CampusId] FOREIGN KEY ([CampusId]) REFERENCES [OrgCampuses] ([CampusId]) ON DELETE NO ACTION;
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_OrgFloors_OrgBuildings_BuildingId') ALTER TABLE [OrgFloors] ADD CONSTRAINT [FK_OrgFloors_OrgBuildings_BuildingId] FOREIGN KEY ([BuildingId]) REFERENCES [OrgBuildings] ([BuildingId]) ON DELETE CASCADE;
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_OrgDesignations_OrgDepartments_DepartmentId') ALTER TABLE [OrgDesignations] ADD CONSTRAINT [FK_OrgDesignations_OrgDepartments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [OrgDepartments] ([DepartmentId]) ON DELETE NO ACTION;
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_OrgDesignations_OrgDesignations_ReportingToDesignationId') ALTER TABLE [OrgDesignations] ADD CONSTRAINT [FK_OrgDesignations_OrgDesignations_ReportingToDesignationId] FOREIGN KEY ([ReportingToDesignationId]) REFERENCES [OrgDesignations] ([DesignationId]) ON DELETE NO ACTION;
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name=N'FK_OrgRooms_OrgFloors_FloorId') ALTER TABLE [OrgRooms] ADD CONSTRAINT [FK_OrgRooms_OrgFloors_FloorId] FOREIGN KEY ([FloorId]) REFERENCES [OrgFloors] ([FloorId]) ON DELETE CASCADE;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_WF_Definitions_ClientId_ScreenKey_ActionKey' AND object_id=OBJECT_ID(N'[WF_Definitions]')) CREATE INDEX [IX_WF_Definitions_ClientId_ScreenKey_ActionKey] ON [WF_Definitions] ([ClientId], [ScreenKey], [ActionKey]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_WF_Events_ClientId_InstanceId_AtUtc' AND object_id=OBJECT_ID(N'[WF_Events]')) CREATE INDEX [IX_WF_Events_ClientId_InstanceId_AtUtc] ON [WF_Events] ([ClientId], [InstanceId], [AtUtc]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_WF_Events_InstanceId_UserId_IdempotencyKey' AND object_id=OBJECT_ID(N'[WF_Events]')) CREATE UNIQUE INDEX [IX_WF_Events_InstanceId_UserId_IdempotencyKey] ON [WF_Events] ([InstanceId], [UserId], [IdempotencyKey]) WHERE [IdempotencyKey] IS NOT NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_WF_Instances_ClientId_Status_CreatedAtUtc' AND object_id=OBJECT_ID(N'[WF_Instances]')) CREATE INDEX [IX_WF_Instances_ClientId_Status_CreatedAtUtc] ON [WF_Instances] ([ClientId], [Status], [CreatedAtUtc]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_WF_Instances_ClientId_SubmitterId_IdempotencyKey' AND object_id=OBJECT_ID(N'[WF_Instances]')) CREATE UNIQUE INDEX [IX_WF_Instances_ClientId_SubmitterId_IdempotencyKey] ON [WF_Instances] ([ClientId], [SubmitterId], [IdempotencyKey]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_WF_Instances_OpenKey' AND object_id=OBJECT_ID(N'[WF_Instances]')) CREATE UNIQUE INDEX [IX_WF_Instances_OpenKey] ON [WF_Instances] ([OpenKey]) WHERE [OpenKey] IS NOT NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_WF_Instances_Status_NextAttemptAtUtc' AND object_id=OBJECT_ID(N'[WF_Instances]')) CREATE INDEX [IX_WF_Instances_Status_NextAttemptAtUtc] ON [WF_Instances] ([Status], [NextAttemptAtUtc]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_WF_Notifications_ClientId_UserId_ReadAtUtc' AND object_id=OBJECT_ID(N'[WF_Notifications]')) CREATE INDEX [IX_WF_Notifications_ClientId_UserId_ReadAtUtc] ON [WF_Notifications] ([ClientId], [UserId], [ReadAtUtc]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_WF_Outbox_DeliveredAtUtc' AND object_id=OBJECT_ID(N'[WF_Outbox]')) CREATE INDEX [IX_WF_Outbox_DeliveredAtUtc] ON [WF_Outbox] ([DeliveredAtUtc]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_WF_Tasks_ClientId_UserId_Status' AND object_id=OBJECT_ID(N'[WF_Tasks]')) CREATE INDEX [IX_WF_Tasks_ClientId_UserId_Status] ON [WF_Tasks] ([ClientId], [UserId], [Status]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_WF_Tasks_InstanceId_StepIndex_Generation_UserId' AND object_id=OBJECT_ID(N'[WF_Tasks]')) CREATE UNIQUE INDEX [IX_WF_Tasks_InstanceId_StepIndex_Generation_UserId] ON [WF_Tasks] ([InstanceId], [StepIndex], [Generation], [UserId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_ARG_ZkEmployeeLinks_SourceKey_SourceClientId_MachineCode' AND object_id=OBJECT_ID(N'[ARG_ZkEmployeeLinks]')) CREATE INDEX [IX_ARG_ZkEmployeeLinks_SourceKey_SourceClientId_MachineCode] ON [ARG_ZkEmployeeLinks] ([SourceKey], [SourceClientId], [MachineCode]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EPDeductiongrpdtl_dedgrpid' AND object_id=OBJECT_ID(N'[EPDeductiongrpdtl]')) CREATE INDEX [IX_EPDeductiongrpdtl_dedgrpid] ON [EPDeductiongrpdtl] ([dedgrpid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EPEarninggrpdtl_earngrpid' AND object_id=OBJECT_ID(N'[EPEarninggrpdtl]')) CREATE INDEX [IX_EPEarninggrpdtl_earngrpid] ON [EPEarninggrpdtl] ([earngrpid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EPLoanrequest_employeeid' AND object_id=OBJECT_ID(N'[EPLoanrequest]')) CREATE INDEX [IX_EPLoanrequest_employeeid] ON [EPLoanrequest] ([employeeid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EPLoanrequest_loantypeid' AND object_id=OBJECT_ID(N'[EPLoanrequest]')) CREATE INDEX [IX_EPLoanrequest_loantypeid] ON [EPLoanrequest] ([loantypeid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EPLoantransdtl_loanid' AND object_id=OBJECT_ID(N'[EPLoantransdtl]')) CREATE INDEX [IX_EPLoantransdtl_loanid] ON [EPLoantransdtl] ([loanid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EPLoantranshdr_employeeid' AND object_id=OBJECT_ID(N'[EPLoantranshdr]')) CREATE INDEX [IX_EPLoantranshdr_employeeid] ON [EPLoantranshdr] ([employeeid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EPLoantranshdr_requestid' AND object_id=OBJECT_ID(N'[EPLoantranshdr]')) CREATE INDEX [IX_EPLoantranshdr_requestid] ON [EPLoantranshdr] ([requestid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EPLoantranshdr_TrusteeStatusId' AND object_id=OBJECT_ID(N'[EPLoantranshdr]')) CREATE INDEX [IX_EPLoantranshdr_TrusteeStatusId] ON [EPLoantranshdr] ([TrusteeStatusId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_ERDesgPayAllowanceDtl_designationpayrollid' AND object_id=OBJECT_ID(N'[ERDesgPayAllowanceDtl]')) CREATE INDEX [IX_ERDesgPayAllowanceDtl_designationpayrollid] ON [ERDesgPayAllowanceDtl] ([designationpayrollid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_ERDesgPayAllowanceDtl_earntypeid' AND object_id=OBJECT_ID(N'[ERDesgPayAllowanceDtl]')) CREATE INDEX [IX_ERDesgPayAllowanceDtl_earntypeid] ON [ERDesgPayAllowanceDtl] ([earntypeid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_ERDesgPayBaseSalaryDtl_designationpayrollid' AND object_id=OBJECT_ID(N'[ERDesgPayBaseSalaryDtl]')) CREATE INDEX [IX_ERDesgPayBaseSalaryDtl_designationpayrollid] ON [ERDesgPayBaseSalaryDtl] ([designationpayrollid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_ERDesgPayBaseSalaryDtl_earntypeid' AND object_id=OBJECT_ID(N'[ERDesgPayBaseSalaryDtl]')) CREATE INDEX [IX_ERDesgPayBaseSalaryDtl_earntypeid] ON [ERDesgPayBaseSalaryDtl] ([earntypeid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_ERDesgPayDeductionDtl_deductiontypeid' AND object_id=OBJECT_ID(N'[ERDesgPayDeductionDtl]')) CREATE INDEX [IX_ERDesgPayDeductionDtl_deductiontypeid] ON [ERDesgPayDeductionDtl] ([deductiontypeid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_ERDesgPayDeductionDtl_designationpayrollid' AND object_id=OBJECT_ID(N'[ERDesgPayDeductionDtl]')) CREATE INDEX [IX_ERDesgPayDeductionDtl_designationpayrollid] ON [ERDesgPayDeductionDtl] ([designationpayrollid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_ERDesgPayLoanType_designationpayrollid' AND object_id=OBJECT_ID(N'[ERDesgPayLoanType]')) CREATE INDEX [IX_ERDesgPayLoanType_designationpayrollid] ON [ERDesgPayLoanType] ([designationpayrollid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_ERDesgPayLoanType_loantypeid' AND object_id=OBJECT_ID(N'[ERDesgPayLoanType]')) CREATE INDEX [IX_ERDesgPayLoanType_loantypeid] ON [ERDesgPayLoanType] ([loantypeid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EREmpBank_bankgrpid' AND object_id=OBJECT_ID(N'[EREmpBank]')) CREATE INDEX [IX_EREmpBank_bankgrpid] ON [EREmpBank] ([bankgrpid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EREmpBank_employeeid' AND object_id=OBJECT_ID(N'[EREmpBank]')) CREATE INDEX [IX_EREmpBank_employeeid] ON [EREmpBank] ([employeeid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EREmpCards_authorityid' AND object_id=OBJECT_ID(N'[EREmpCards]')) CREATE INDEX [IX_EREmpCards_authorityid] ON [EREmpCards] ([authorityid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EREmpCards_cardtypeid' AND object_id=OBJECT_ID(N'[EREmpCards]')) CREATE INDEX [IX_EREmpCards_cardtypeid] ON [EREmpCards] ([cardtypeid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EREmpCards_employeeid' AND object_id=OBJECT_ID(N'[EREmpCards]')) CREATE INDEX [IX_EREmpCards_employeeid] ON [EREmpCards] ([employeeid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EREmpExperience_busnatureid' AND object_id=OBJECT_ID(N'[EREmpExperience]')) CREATE INDEX [IX_EREmpExperience_busnatureid] ON [EREmpExperience] ([busnatureid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EREmpExperience_employeeid' AND object_id=OBJECT_ID(N'[EREmpExperience]')) CREATE INDEX [IX_EREmpExperience_employeeid] ON [EREmpExperience] ([employeeid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EREmployeedisease_discatid' AND object_id=OBJECT_ID(N'[EREmployeedisease]')) CREATE INDEX [IX_EREmployeedisease_discatid] ON [EREmployeedisease] ([discatid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EREmployeedisease_diseasesid' AND object_id=OBJECT_ID(N'[EREmployeedisease]')) CREATE INDEX [IX_EREmployeedisease_diseasesid] ON [EREmployeedisease] ([diseasesid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EREmployeedisease_distypeid' AND object_id=OBJECT_ID(N'[EREmployeedisease]')) CREATE INDEX [IX_EREmployeedisease_distypeid] ON [EREmployeedisease] ([distypeid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EREmployeedisease_employeeid' AND object_id=OBJECT_ID(N'[EREmployeedisease]')) CREATE INDEX [IX_EREmployeedisease_employeeid] ON [EREmployeedisease] ([employeeid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EREmployeeinfohdr_depid' AND object_id=OBJECT_ID(N'[EREmployeeinfohdr]')) CREATE INDEX [IX_EREmployeeinfohdr_depid] ON [EREmployeeinfohdr] ([depid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EREmployeeinfohdr_designationid' AND object_id=OBJECT_ID(N'[EREmployeeinfohdr]')) CREATE INDEX [IX_EREmployeeinfohdr_designationid] ON [EREmployeeinfohdr] ([designationid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EREmployeesubjectskill_employeeid' AND object_id=OBJECT_ID(N'[EREmployeesubjectskill]')) CREATE INDEX [IX_EREmployeesubjectskill_employeeid] ON [EREmployeesubjectskill] ([employeeid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EREmployeesubjectskill_ratingdtlid' AND object_id=OBJECT_ID(N'[EREmployeesubjectskill]')) CREATE INDEX [IX_EREmployeesubjectskill_ratingdtlid] ON [EREmployeesubjectskill] ([ratingdtlid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EREmployeesubjectskill_subjecttypeid' AND object_id=OBJECT_ID(N'[EREmployeesubjectskill]')) CREATE INDEX [IX_EREmployeesubjectskill_subjecttypeid] ON [EREmployeesubjectskill] ([subjecttypeid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EREmppayallowancedtl_earntypeid' AND object_id=OBJECT_ID(N'[EREmppayallowancedtl]')) CREATE INDEX [IX_EREmppayallowancedtl_earntypeid] ON [EREmppayallowancedtl] ([earntypeid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EREmppayallowancedtl_emppayrollid' AND object_id=OBJECT_ID(N'[EREmppayallowancedtl]')) CREATE INDEX [IX_EREmppayallowancedtl_emppayrollid] ON [EREmppayallowancedtl] ([emppayrollid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EREmppaybasesalarydtl_earntypeid' AND object_id=OBJECT_ID(N'[EREmppaybasesalarydtl]')) CREATE INDEX [IX_EREmppaybasesalarydtl_earntypeid] ON [EREmppaybasesalarydtl] ([earntypeid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EREmppaybasesalarydtl_emppayrollid' AND object_id=OBJECT_ID(N'[EREmppaybasesalarydtl]')) CREATE INDEX [IX_EREmppaybasesalarydtl_emppayrollid] ON [EREmppaybasesalarydtl] ([emppayrollid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EREmppaydeductiondtl_deductiontypeid' AND object_id=OBJECT_ID(N'[EREmppaydeductiondtl]')) CREATE INDEX [IX_EREmppaydeductiondtl_deductiontypeid] ON [EREmppaydeductiondtl] ([deductiontypeid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EREmppaydeductiondtl_emppayrollid' AND object_id=OBJECT_ID(N'[EREmppaydeductiondtl]')) CREATE INDEX [IX_EREmppaydeductiondtl_emppayrollid] ON [EREmppaydeductiondtl] ([emppayrollid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EREmppayroll_employeeid' AND object_id=OBJECT_ID(N'[EREmppayroll]')) CREATE INDEX [IX_EREmppayroll_employeeid] ON [EREmppayroll] ([employeeid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EREmppayrollLoanType_emppayrollid' AND object_id=OBJECT_ID(N'[EREmppayrollLoanType]')) CREATE INDEX [IX_EREmppayrollLoanType_emppayrollid] ON [EREmppayrollLoanType] ([emppayrollid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EREmppayrollLoanType_loantypeid' AND object_id=OBJECT_ID(N'[EREmppayrollLoanType]')) CREATE INDEX [IX_EREmppayrollLoanType_loantypeid] ON [EREmppayrollLoanType] ([loantypeid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EREmpPosition_deptid' AND object_id=OBJECT_ID(N'[EREmpPosition]')) CREATE INDEX [IX_EREmpPosition_deptid] ON [EREmpPosition] ([deptid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EREmpPosition_desgid' AND object_id=OBJECT_ID(N'[EREmpPosition]')) CREATE INDEX [IX_EREmpPosition_desgid] ON [EREmpPosition] ([desgid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EREmpPosition_employeeid' AND object_id=OBJECT_ID(N'[EREmpPosition]')) CREATE INDEX [IX_EREmpPosition_employeeid] ON [EREmpPosition] ([employeeid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EREmpRelation_employeeid' AND object_id=OBJECT_ID(N'[EREmpRelation]')) CREATE INDEX [IX_EREmpRelation_employeeid] ON [EREmpRelation] ([employeeid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EREmpRelation_genderid' AND object_id=OBJECT_ID(N'[EREmpRelation]')) CREATE INDEX [IX_EREmpRelation_genderid] ON [EREmpRelation] ([genderid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EREmpRelation_relationtypeid' AND object_id=OBJECT_ID(N'[EREmpRelation]')) CREATE INDEX [IX_EREmpRelation_relationtypeid] ON [EREmpRelation] ([relationtypeid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EREmproaster_employeeid' AND object_id=OBJECT_ID(N'[EREmproaster]')) CREATE INDEX [IX_EREmproaster_employeeid] ON [EREmproaster] ([employeeid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EREmproaster_leavegrpid' AND object_id=OBJECT_ID(N'[EREmproaster]')) CREATE INDEX [IX_EREmproaster_leavegrpid] ON [EREmproaster] ([leavegrpid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EREmproaster_shiftid' AND object_id=OBJECT_ID(N'[EREmproaster]')) CREATE INDEX [IX_EREmproaster_shiftid] ON [EREmproaster] ([shiftid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EREmproaster_taxid' AND object_id=OBJECT_ID(N'[EREmproaster]')) CREATE INDEX [IX_EREmproaster_taxid] ON [EREmproaster] ([taxid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EREmpSkill_employeeid' AND object_id=OBJECT_ID(N'[EREmpSkill]')) CREATE INDEX [IX_EREmpSkill_employeeid] ON [EREmpSkill] ([employeeid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EREmpSkill_punitid' AND object_id=OBJECT_ID(N'[EREmpSkill]')) CREATE INDEX [IX_EREmpSkill_punitid] ON [EREmpSkill] ([punitid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EREmpSkill_skillid' AND object_id=OBJECT_ID(N'[EREmpSkill]')) CREATE INDEX [IX_EREmpSkill_skillid] ON [EREmpSkill] ([skillid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EREmpSkill_skillratingdtlid' AND object_id=OBJECT_ID(N'[EREmpSkill]')) CREATE INDEX [IX_EREmpSkill_skillratingdtlid] ON [EREmpSkill] ([skillratingdtlid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EREmpSkill_skillratingid' AND object_id=OBJECT_ID(N'[EREmpSkill]')) CREATE INDEX [IX_EREmpSkill_skillratingid] ON [EREmpSkill] ([skillratingid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_EREmpSkill_skilltypeid' AND object_id=OBJECT_ID(N'[EREmpSkill]')) CREATE INDEX [IX_EREmpSkill_skilltypeid] ON [EREmpSkill] ([skilltypeid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_ERSkillratingdtl_ratingid' AND object_id=OBJECT_ID(N'[ERSkillratingdtl]')) CREATE INDEX [IX_ERSkillratingdtl_ratingid] ON [ERSkillratingdtl] ([ratingid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_ETAbsentpolicydtl_policyid' AND object_id=OBJECT_ID(N'[ETAbsentpolicydtl]')) CREATE INDEX [IX_ETAbsentpolicydtl_policyid] ON [ETAbsentpolicydtl] ([policyid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_ETCalholidayapplyondtl_calholidayid' AND object_id=OBJECT_ID(N'[ETCalholidayapplyondtl]')) CREATE INDEX [IX_ETCalholidayapplyondtl_calholidayid] ON [ETCalholidayapplyondtl] ([calholidayid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_ETCalholidaydtl_calholidayid' AND object_id=OBJECT_ID(N'[ETCalholidaydtl]')) CREATE INDEX [IX_ETCalholidaydtl_calholidayid] ON [ETCalholidaydtl] ([calholidayid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_ETCalholidayshareddtl_calholidayid' AND object_id=OBJECT_ID(N'[ETCalholidayshareddtl]')) CREATE INDEX [IX_ETCalholidayshareddtl_calholidayid] ON [ETCalholidayshareddtl] ([calholidayid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_ETLeaveadjpolicydtl_leaveadjid' AND object_id=OBJECT_ID(N'[ETLeaveadjpolicydtl]')) CREATE INDEX [IX_ETLeaveadjpolicydtl_leaveadjid] ON [ETLeaveadjpolicydtl] ([leaveadjid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_ETLeavegroupdtl_leavegrpid' AND object_id=OBJECT_ID(N'[ETLeavegroupdtl]')) CREATE INDEX [IX_ETLeavegroupdtl_leavegrpid] ON [ETLeavegroupdtl] ([leavegrpid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_ETLeaveRequest_employeeid' AND object_id=OBJECT_ID(N'[ETLeaveRequest]')) CREATE INDEX [IX_ETLeaveRequest_employeeid] ON [ETLeaveRequest] ([employeeid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_ETLeaveRequest_leavetypeid' AND object_id=OBJECT_ID(N'[ETLeaveRequest]')) CREATE INDEX [IX_ETLeaveRequest_leavetypeid] ON [ETLeaveRequest] ([leavetypeid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_ETLeavetrans_employeeid' AND object_id=OBJECT_ID(N'[ETLeavetrans]')) CREATE INDEX [IX_ETLeavetrans_employeeid] ON [ETLeavetrans] ([employeeid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_ETLeavetrans_leavetypeid' AND object_id=OBJECT_ID(N'[ETLeavetrans]')) CREATE INDEX [IX_ETLeavetrans_leavetypeid] ON [ETLeavetrans] ([leavetypeid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_ETLeavetrans_requestid' AND object_id=OBJECT_ID(N'[ETLeavetrans]')) CREATE INDEX [IX_ETLeavetrans_requestid] ON [ETLeavetrans] ([requestid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_ETPayperioddtl_payperiodid' AND object_id=OBJECT_ID(N'[ETPayperioddtl]')) CREATE INDEX [IX_ETPayperioddtl_payperiodid] ON [ETPayperioddtl] ([payperiodid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_ETPolicydtl_policyid' AND object_id=OBJECT_ID(N'[ETPolicydtl]')) CREATE INDEX [IX_ETPolicydtl_policyid] ON [ETPolicydtl] ([policyid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HREmployeeInformationRecord_employeeid' AND object_id=OBJECT_ID(N'[HREmployeeInformationRecord]')) CREATE INDEX [IX_HREmployeeInformationRecord_employeeid] ON [HREmployeeInformationRecord] ([employeeid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HRMonthlyAttendance_employeeid' AND object_id=OBJECT_ID(N'[HRMonthlyAttendance]')) CREATE INDEX [IX_HRMonthlyAttendance_employeeid] ON [HRMonthlyAttendance] ([employeeid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HRpayrollprocess_employeeid' AND object_id=OBJECT_ID(N'[HRpayrollprocess]')) CREATE INDEX [IX_HRpayrollprocess_employeeid] ON [HRpayrollprocess] ([employeeid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HRpayrollprocessdtl_processid' AND object_id=OBJECT_ID(N'[HRpayrollprocessdtl]')) CREATE INDEX [IX_HRpayrollprocessdtl_processid] ON [HRpayrollprocessdtl] ([processid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HRPosition_ClientId_EntityId_LovStatusId' AND object_id=OBJECT_ID(N'[HRPosition]')) CREATE INDEX [IX_HRPosition_ClientId_EntityId_LovStatusId] ON [HRPosition] ([ClientId], [EntityId], [LovStatusId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HRPosition_ClientId_PositionNumber' AND object_id=OBJECT_ID(N'[HRPosition]')) CREATE UNIQUE INDEX [IX_HRPosition_ClientId_PositionNumber] ON [HRPosition] ([ClientId], [PositionNumber]) WHERE [PositionNumber] IS NOT NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HRPosition_DepartmentId' AND object_id=OBJECT_ID(N'[HRPosition]')) CREATE INDEX [IX_HRPosition_DepartmentId] ON [HRPosition] ([DepartmentId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HRPosition_DesignationId' AND object_id=OBJECT_ID(N'[HRPosition]')) CREATE INDEX [IX_HRPosition_DesignationId] ON [HRPosition] ([DesignationId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HRPosition_EntityId' AND object_id=OBJECT_ID(N'[HRPosition]')) CREATE INDEX [IX_HRPosition_EntityId] ON [HRPosition] ([EntityId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HRPosition_LovStatusId' AND object_id=OBJECT_ID(N'[HRPosition]')) CREATE INDEX [IX_HRPosition_LovStatusId] ON [HRPosition] ([LovStatusId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HRPosition_ReportsToPositionId' AND object_id=OBJECT_ID(N'[HRPosition]')) CREATE INDEX [IX_HRPosition_ReportsToPositionId] ON [HRPosition] ([ReportsToPositionId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HRPositionAssignment_EmployeeId_LovStatusId_EffectiveTo' AND object_id=OBJECT_ID(N'[HRPositionAssignment]')) CREATE INDEX [IX_HRPositionAssignment_EmployeeId_LovStatusId_EffectiveTo] ON [HRPositionAssignment] ([EmployeeId], [LovStatusId], [EffectiveTo]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HRPositionAssignment_LovStatusId' AND object_id=OBJECT_ID(N'[HRPositionAssignment]')) CREATE INDEX [IX_HRPositionAssignment_LovStatusId] ON [HRPositionAssignment] ([LovStatusId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HRPositionAssignment_PositionId_LovStatusId_EffectiveTo' AND object_id=OBJECT_ID(N'[HRPositionAssignment]')) CREATE INDEX [IX_HRPositionAssignment_PositionId_LovStatusId_EffectiveTo] ON [HRPositionAssignment] ([PositionId], [LovStatusId], [EffectiveTo]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HRPositionAudit_ActorEmployeeId' AND object_id=OBJECT_ID(N'[HRPositionAudit]')) CREATE INDEX [IX_HRPositionAudit_ActorEmployeeId] ON [HRPositionAudit] ([ActorEmployeeId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HRPositionAudit_PositionId_OccurredAtUtc' AND object_id=OBJECT_ID(N'[HRPositionAudit]')) CREATE INDEX [IX_HRPositionAudit_PositionId_OccurredAtUtc] ON [HRPositionAudit] ([PositionId], [OccurredAtUtc]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HRVacancy_ApproverEmployeeId' AND object_id=OBJECT_ID(N'[HRVacancy]')) CREATE INDEX [IX_HRVacancy_ApproverEmployeeId] ON [HRVacancy] ([ApproverEmployeeId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HRVacancy_ClientId_EntityId_ApprovalStatus_LifecycleStatus' AND object_id=OBJECT_ID(N'[HRVacancy]')) CREATE INDEX [IX_HRVacancy_ClientId_EntityId_ApprovalStatus_LifecycleStatus] ON [HRVacancy] ([ClientId], [EntityId], [ApprovalStatus], [LifecycleStatus]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HRVacancy_ClientId_VacancyNumber' AND object_id=OBJECT_ID(N'[HRVacancy]')) CREATE UNIQUE INDEX [IX_HRVacancy_ClientId_VacancyNumber] ON [HRVacancy] ([ClientId], [VacancyNumber]) WHERE [VacancyNumber] IS NOT NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HRVacancy_EntityId' AND object_id=OBJECT_ID(N'[HRVacancy]')) CREATE INDEX [IX_HRVacancy_EntityId] ON [HRVacancy] ([EntityId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HRVacancy_HiringManagerEmployeeId' AND object_id=OBJECT_ID(N'[HRVacancy]')) CREATE INDEX [IX_HRVacancy_HiringManagerEmployeeId] ON [HRVacancy] ([HiringManagerEmployeeId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HRVacancy_ReplacementPositionId' AND object_id=OBJECT_ID(N'[HRVacancy]')) CREATE INDEX [IX_HRVacancy_ReplacementPositionId] ON [HRVacancy] ([ReplacementPositionId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HRVacancy_ReportsToPositionId' AND object_id=OBJECT_ID(N'[HRVacancy]')) CREATE INDEX [IX_HRVacancy_ReportsToPositionId] ON [HRVacancy] ([ReportsToPositionId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HRVacancyAudit_ActorEmployeeId' AND object_id=OBJECT_ID(N'[HRVacancyAudit]')) CREATE INDEX [IX_HRVacancyAudit_ActorEmployeeId] ON [HRVacancyAudit] ([ActorEmployeeId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HRVacancyAudit_VacancyId_OccurredAtUtc' AND object_id=OBJECT_ID(N'[HRVacancyAudit]')) CREATE INDEX [IX_HRVacancyAudit_VacancyId_OccurredAtUtc] ON [HRVacancyAudit] ([VacancyId], [OccurredAtUtc]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HRVacancyPosition_CurrentEmployeeId' AND object_id=OBJECT_ID(N'[HRVacancyPosition]')) CREATE INDEX [IX_HRVacancyPosition_CurrentEmployeeId] ON [HRVacancyPosition] ([CurrentEmployeeId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HRVacancyPosition_PositionNumber' AND object_id=OBJECT_ID(N'[HRVacancyPosition]')) CREATE INDEX [IX_HRVacancyPosition_PositionNumber] ON [HRVacancyPosition] ([PositionNumber]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HRVacancyPosition_VacancyId_Status' AND object_id=OBJECT_ID(N'[HRVacancyPosition]')) CREATE INDEX [IX_HRVacancyPosition_VacancyId_Status] ON [HRVacancyPosition] ([VacancyId], [Status]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HRVacancyPositionAssignment_EmployeeId' AND object_id=OBJECT_ID(N'[HRVacancyPositionAssignment]')) CREATE INDEX [IX_HRVacancyPositionAssignment_EmployeeId] ON [HRVacancyPositionAssignment] ([EmployeeId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HRVacancyPositionAssignment_PositionId_Status_EffectiveDate' AND object_id=OBJECT_ID(N'[HRVacancyPositionAssignment]')) CREATE INDEX [IX_HRVacancyPositionAssignment_PositionId_Status_EffectiveDate] ON [HRVacancyPositionAssignment] ([PositionId], [Status], [EffectiveDate]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HRVacancyQualification_VacancyId_QualificationId' AND object_id=OBJECT_ID(N'[HRVacancyQualification]')) CREATE UNIQUE INDEX [IX_HRVacancyQualification_VacancyId_QualificationId] ON [HRVacancyQualification] ([VacancyId], [QualificationId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HRVacancyRecruiter_EmployeeId' AND object_id=OBJECT_ID(N'[HRVacancyRecruiter]')) CREATE INDEX [IX_HRVacancyRecruiter_EmployeeId] ON [HRVacancyRecruiter] ([EmployeeId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HRVacancyRecruiter_VacancyId_EmployeeId_IsActive' AND object_id=OBJECT_ID(N'[HRVacancyRecruiter]')) CREATE INDEX [IX_HRVacancyRecruiter_VacancyId_EmployeeId_IsActive] ON [HRVacancyRecruiter] ([VacancyId], [EmployeeId], [IsActive]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HRVacancyRevision_VacancyId_RevisionNumber' AND object_id=OBJECT_ID(N'[HRVacancyRevision]')) CREATE UNIQUE INDEX [IX_HRVacancyRevision_VacancyId_RevisionNumber] ON [HRVacancyRevision] ([VacancyId], [RevisionNumber]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_HRVacancySkill_VacancyId_SkillId' AND object_id=OBJECT_ID(N'[HRVacancySkill]')) CREATE UNIQUE INDEX [IX_HRVacancySkill_VacancyId_SkillId] ON [HRVacancySkill] ([VacancyId], [SkillId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_orgsubcordinate_empid' AND object_id=OBJECT_ID(N'[orgsubcordinate]')) CREATE INDEX [IX_orgsubcordinate_empid] ON [orgsubcordinate] ([empid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_orgsubcordinate_subcampus' AND object_id=OBJECT_ID(N'[orgsubcordinate]')) CREATE INDEX [IX_orgsubcordinate_subcampus] ON [orgsubcordinate] ([subcampus]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_SMAcademic_employeeid' AND object_id=OBJECT_ID(N'[SMAcademic]')) CREATE INDEX [IX_SMAcademic_employeeid] ON [SMAcademic] ([employeeid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_SMAcademic_qualificationid' AND object_id=OBJECT_ID(N'[SMAcademic]')) CREATE INDEX [IX_SMAcademic_qualificationid] ON [SMAcademic] ([qualificationid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_SMAcademic_qualificationtypeid' AND object_id=OBJECT_ID(N'[SMAcademic]')) CREATE INDEX [IX_SMAcademic_qualificationtypeid] ON [SMAcademic] ([qualificationtypeid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_City_ctryid' AND object_id=OBJECT_ID(N'[City]')) CREATE INDEX [IX_City_ctryid] ON [City] ([ctryid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_City_stateid' AND object_id=OBJECT_ID(N'[City]')) CREATE INDEX [IX_City_stateid] ON [City] ([stateid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_Country_countrycode' AND object_id=OBJECT_ID(N'[Country]')) CREATE UNIQUE INDEX [IX_Country_countrycode] ON [Country] ([countrycode]) WHERE [countrycode] IS NOT NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_Districts_CityId' AND object_id=OBJECT_ID(N'[Districts]')) CREATE INDEX [IX_Districts_CityId] ON [Districts] ([CityId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_Districts_IsDeleted' AND object_id=OBJECT_ID(N'[Districts]')) CREATE INDEX [IX_Districts_IsDeleted] ON [Districts] ([IsDeleted]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_FOCalllog_purposetypeid' AND object_id=OBJECT_ID(N'[FOCalllog]')) CREATE INDEX [IX_FOCalllog_purposetypeid] ON [FOCalllog] ([purposetypeid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_FOPostaldispatch_clientid' AND object_id=OBJECT_ID(N'[FOPostaldispatch]')) CREATE INDEX [IX_FOPostaldispatch_clientid] ON [FOPostaldispatch] ([clientid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_FOPostaldispatch_courierid' AND object_id=OBJECT_ID(N'[FOPostaldispatch]')) CREATE INDEX [IX_FOPostaldispatch_courierid] ON [FOPostaldispatch] ([courierid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_FOPostaldispatch_postaldate' AND object_id=OBJECT_ID(N'[FOPostaldispatch]')) CREATE INDEX [IX_FOPostaldispatch_postaldate] ON [FOPostaldispatch] ([postaldate]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_FOPostaldispatch_postaltypeid' AND object_id=OBJECT_ID(N'[FOPostaldispatch]')) CREATE INDEX [IX_FOPostaldispatch_postaltypeid] ON [FOPostaldispatch] ([postaltypeid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_lov_vtype' AND object_id=OBJECT_ID(N'[lov]')) CREATE INDEX [IX_lov_vtype] ON [lov] ([vtype]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_OrgBuildings_CampusId_BuildingCode' AND object_id=OBJECT_ID(N'[OrgBuildings]')) CREATE UNIQUE INDEX [IX_OrgBuildings_CampusId_BuildingCode] ON [OrgBuildings] ([CampusId], [BuildingCode]) WHERE [BuildingCode] IS NOT NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_OrgBuildings_IsDeleted' AND object_id=OBJECT_ID(N'[OrgBuildings]')) CREATE INDEX [IX_OrgBuildings_IsDeleted] ON [OrgBuildings] ([IsDeleted]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_OrgBusinessUnits_ClientId_BusinessUnitCode' AND object_id=OBJECT_ID(N'[OrgBusinessUnits]')) CREATE UNIQUE INDEX [IX_OrgBusinessUnits_ClientId_BusinessUnitCode] ON [OrgBusinessUnits] ([ClientId], [BusinessUnitCode]) WHERE [BusinessUnitCode] IS NOT NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_OrgBusinessUnits_IsDeleted' AND object_id=OBJECT_ID(N'[OrgBusinessUnits]')) CREATE INDEX [IX_OrgBusinessUnits_IsDeleted] ON [OrgBusinessUnits] ([IsDeleted]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_OrgCampuses_CityId' AND object_id=OBJECT_ID(N'[OrgCampuses]')) CREATE INDEX [IX_OrgCampuses_CityId] ON [OrgCampuses] ([CityId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_OrgCampuses_ClientId_CampusCode' AND object_id=OBJECT_ID(N'[OrgCampuses]')) CREATE UNIQUE INDEX [IX_OrgCampuses_ClientId_CampusCode] ON [OrgCampuses] ([ClientId], [CampusCode]) WHERE [CampusCode] IS NOT NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_OrgCampuses_CountryId' AND object_id=OBJECT_ID(N'[OrgCampuses]')) CREATE INDEX [IX_OrgCampuses_CountryId] ON [OrgCampuses] ([CountryId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_OrgCampuses_EntityId' AND object_id=OBJECT_ID(N'[OrgCampuses]')) CREATE INDEX [IX_OrgCampuses_EntityId] ON [OrgCampuses] ([EntityId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_OrgCampuses_IsDeleted' AND object_id=OBJECT_ID(N'[OrgCampuses]')) CREATE INDEX [IX_OrgCampuses_IsDeleted] ON [OrgCampuses] ([IsDeleted]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_OrgCampuses_StateId' AND object_id=OBJECT_ID(N'[OrgCampuses]')) CREATE INDEX [IX_OrgCampuses_StateId] ON [OrgCampuses] ([StateId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_OrgCastes_CasteCode' AND object_id=OBJECT_ID(N'[OrgCastes]')) CREATE UNIQUE INDEX [IX_OrgCastes_CasteCode] ON [OrgCastes] ([CasteCode]) WHERE [CasteCode] IS NOT NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_OrgCastes_IsDeleted' AND object_id=OBJECT_ID(N'[OrgCastes]')) CREATE INDEX [IX_OrgCastes_IsDeleted] ON [OrgCastes] ([IsDeleted]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_OrgCostCenters_ClientId_CostCenterCode' AND object_id=OBJECT_ID(N'[OrgCostCenters]')) CREATE UNIQUE INDEX [IX_OrgCostCenters_ClientId_CostCenterCode] ON [OrgCostCenters] ([ClientId], [CostCenterCode]) WHERE [CostCenterCode] IS NOT NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_OrgCostCenters_IsDeleted' AND object_id=OBJECT_ID(N'[OrgCostCenters]')) CREATE INDEX [IX_OrgCostCenters_IsDeleted] ON [OrgCostCenters] ([IsDeleted]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_OrgDepartment_ficoaid' AND object_id=OBJECT_ID(N'[OrgDepartment]')) CREATE INDEX [IX_OrgDepartment_ficoaid] ON [OrgDepartment] ([ficoaid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_OrgDepartment_ficoaid_rec' AND object_id=OBJECT_ID(N'[OrgDepartment]')) CREATE INDEX [IX_OrgDepartment_ficoaid_rec] ON [OrgDepartment] ([ficoaid_rec]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_OrgDepartments_CampusId' AND object_id=OBJECT_ID(N'[OrgDepartments]')) CREATE INDEX [IX_OrgDepartments_CampusId] ON [OrgDepartments] ([CampusId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_OrgDepartments_ClientId_DepartmentCode' AND object_id=OBJECT_ID(N'[OrgDepartments]')) CREATE UNIQUE INDEX [IX_OrgDepartments_ClientId_DepartmentCode] ON [OrgDepartments] ([ClientId], [DepartmentCode]) WHERE [DepartmentCode] IS NOT NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_OrgDepartments_IsDeleted' AND object_id=OBJECT_ID(N'[OrgDepartments]')) CREATE INDEX [IX_OrgDepartments_IsDeleted] ON [OrgDepartments] ([IsDeleted]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_OrgDesignations_ClientId_DesignationCode' AND object_id=OBJECT_ID(N'[OrgDesignations]')) CREATE UNIQUE INDEX [IX_OrgDesignations_ClientId_DesignationCode] ON [OrgDesignations] ([ClientId], [DesignationCode]) WHERE [DesignationCode] IS NOT NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_OrgDesignations_DepartmentId' AND object_id=OBJECT_ID(N'[OrgDesignations]')) CREATE INDEX [IX_OrgDesignations_DepartmentId] ON [OrgDesignations] ([DepartmentId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_OrgDesignations_IsDeleted' AND object_id=OBJECT_ID(N'[OrgDesignations]')) CREATE INDEX [IX_OrgDesignations_IsDeleted] ON [OrgDesignations] ([IsDeleted]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_OrgDesignations_ReportingToDesignationId' AND object_id=OBJECT_ID(N'[OrgDesignations]')) CREATE INDEX [IX_OrgDesignations_ReportingToDesignationId] ON [OrgDesignations] ([ReportingToDesignationId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_OrgEntities_CityId' AND object_id=OBJECT_ID(N'[OrgEntities]')) CREATE INDEX [IX_OrgEntities_CityId] ON [OrgEntities] ([CityId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_OrgEntities_ClientId_EntityCode' AND object_id=OBJECT_ID(N'[OrgEntities]')) CREATE UNIQUE INDEX [IX_OrgEntities_ClientId_EntityCode] ON [OrgEntities] ([ClientId], [EntityCode]) WHERE [EntityCode] IS NOT NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_OrgEntities_CountryId' AND object_id=OBJECT_ID(N'[OrgEntities]')) CREATE INDEX [IX_OrgEntities_CountryId] ON [OrgEntities] ([CountryId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_OrgEntities_IsDeleted' AND object_id=OBJECT_ID(N'[OrgEntities]')) CREATE INDEX [IX_OrgEntities_IsDeleted] ON [OrgEntities] ([IsDeleted]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_OrgEntities_StateId' AND object_id=OBJECT_ID(N'[OrgEntities]')) CREATE INDEX [IX_OrgEntities_StateId] ON [OrgEntities] ([StateId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_OrgEntities_TerminologySetId' AND object_id=OBJECT_ID(N'[OrgEntities]')) CREATE INDEX [IX_OrgEntities_TerminologySetId] ON [OrgEntities] ([TerminologySetId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_ORGentitycontact_entityid' AND object_id=OBJECT_ID(N'[ORGentitycontact]')) CREATE INDEX [IX_ORGentitycontact_entityid] ON [ORGentitycontact] ([entityid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_OrgFloors_BuildingId_FloorNumber' AND object_id=OBJECT_ID(N'[OrgFloors]')) CREATE UNIQUE INDEX [IX_OrgFloors_BuildingId_FloorNumber] ON [OrgFloors] ([BuildingId], [FloorNumber]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_OrgFloors_IsDeleted' AND object_id=OBJECT_ID(N'[OrgFloors]')) CREATE INDEX [IX_OrgFloors_IsDeleted] ON [OrgFloors] ([IsDeleted]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_OrgLanguages_IsDeleted' AND object_id=OBJECT_ID(N'[OrgLanguages]')) CREATE INDEX [IX_OrgLanguages_IsDeleted] ON [OrgLanguages] ([IsDeleted]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_OrgLanguages_LanguageCode' AND object_id=OBJECT_ID(N'[OrgLanguages]')) CREATE UNIQUE INDEX [IX_OrgLanguages_LanguageCode] ON [OrgLanguages] ([LanguageCode]) WHERE [LanguageCode] IS NOT NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_OrgLegalentity_clientid_entityno' AND object_id=OBJECT_ID(N'[OrgLegalentity]')) CREATE INDEX [IX_OrgLegalentity_clientid_entityno] ON [OrgLegalentity] ([clientid], [entityno]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_OrgLegalentity_clientid_stxt' AND object_id=OBJECT_ID(N'[OrgLegalentity]')) CREATE INDEX [IX_OrgLegalentity_clientid_stxt] ON [OrgLegalentity] ([clientid], [stxt]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_OrgLegalentitydtl_entityid' AND object_id=OBJECT_ID(N'[OrgLegalentitydtl]')) CREATE INDEX [IX_OrgLegalentitydtl_entityid] ON [OrgLegalentitydtl] ([entityid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_OrgLegalentitypvdtl_entityid' AND object_id=OBJECT_ID(N'[OrgLegalentitypvdtl]')) CREATE INDEX [IX_OrgLegalentitypvdtl_entityid] ON [OrgLegalentitypvdtl] ([entityid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_OrgRegion_ClientId_Code' AND object_id=OBJECT_ID(N'[OrgRegion]')) CREATE UNIQUE INDEX [IX_OrgRegion_ClientId_Code] ON [OrgRegion] ([ClientId], [Code]) WHERE [Code] IS NOT NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_OrgRegion_CountryId' AND object_id=OBJECT_ID(N'[OrgRegion]')) CREATE INDEX [IX_OrgRegion_CountryId] ON [OrgRegion] ([CountryId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_Orgroom_clientid_entityid_buildingid_lovfloorid_stxt' AND object_id=OBJECT_ID(N'[Orgroom]')) CREATE INDEX [IX_Orgroom_clientid_entityid_buildingid_lovfloorid_stxt] ON [Orgroom] ([clientid], [entityid], [buildingid], [lovfloorid], [stxt]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_Orgroom_clientid_entityid_code' AND object_id=OBJECT_ID(N'[Orgroom]')) CREATE INDEX [IX_Orgroom_clientid_entityid_code] ON [Orgroom] ([clientid], [entityid], [code]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_OrgRooms_FloorId_RoomNumber' AND object_id=OBJECT_ID(N'[OrgRooms]')) CREATE UNIQUE INDEX [IX_OrgRooms_FloorId_RoomNumber] ON [OrgRooms] ([FloorId], [RoomNumber]) WHERE [RoomNumber] IS NOT NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_OrgRooms_IsDeleted' AND object_id=OBJECT_ID(N'[OrgRooms]')) CREATE INDEX [IX_OrgRooms_IsDeleted] ON [OrgRooms] ([IsDeleted]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_Orgsite_clientid_entityid_code' AND object_id=OBJECT_ID(N'[Orgsite]')) CREATE INDEX [IX_Orgsite_clientid_entityid_code] ON [Orgsite] ([clientid], [entityid], [code]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_Orgsite_clientid_entityid_stxt' AND object_id=OBJECT_ID(N'[Orgsite]')) CREATE INDEX [IX_Orgsite_clientid_entityid_stxt] ON [Orgsite] ([clientid], [entityid], [stxt]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_Orgsite_regionid' AND object_id=OBJECT_ID(N'[Orgsite]')) CREATE INDEX [IX_Orgsite_regionid] ON [Orgsite] ([regionid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_Orgsite_siteid' AND object_id=OBJECT_ID(N'[Orgsite]')) CREATE INDEX [IX_Orgsite_siteid] ON [Orgsite] ([siteid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_SignupClientHashKeys_ClientId' AND object_id=OBJECT_ID(N'[SignupClientHashKeys]')) CREATE UNIQUE INDEX [IX_SignupClientHashKeys_ClientId] ON [SignupClientHashKeys] ([ClientId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_SignupClientHashKeys_HashKey' AND object_id=OBJECT_ID(N'[SignupClientHashKeys]')) CREATE UNIQUE INDEX [IX_SignupClientHashKeys_HashKey] ON [SignupClientHashKeys] ([HashKey]) WHERE [HashKey] IS NOT NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_SMAcademicYear_registerid' AND object_id=OBJECT_ID(N'[SMAcademicYear]')) CREATE INDEX [IX_SMAcademicYear_registerid] ON [SMAcademicYear] ([registerid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_SMAcademicYeardtl_acadyrid' AND object_id=OBJECT_ID(N'[SMAcademicYeardtl]')) CREATE INDEX [IX_SMAcademicYeardtl_acadyrid] ON [SMAcademicYeardtl] ([acadyrid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_SMPublishdocumentdtl_publishid' AND object_id=OBJECT_ID(N'[SMPublishdocumentdtl]')) CREATE INDEX [IX_SMPublishdocumentdtl_publishid] ON [SMPublishdocumentdtl] ([publishid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_State_countryid' AND object_id=OBJECT_ID(N'[State]')) CREATE INDEX [IX_State_countryid] ON [State] ([countryid]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_ArgUsers_Username' AND object_id=OBJECT_ID(N'[ArgUsers]')) CREATE UNIQUE INDEX [IX_ArgUsers_Username] ON [ArgUsers] ([Username]);
GO
-- Supporting master data for the standalone installation. No source business records are copied.
SET XACT_ABORT ON;
DECLARE @defaults nvarchar(max)=N'';
SELECT @defaults+=N'ALTER TABLE '+QUOTENAME(OBJECT_SCHEMA_NAME(c.object_id))+N'.'+QUOTENAME(OBJECT_NAME(c.object_id))+N' ADD DEFAULT (0) FOR '+QUOTENAME(c.name)+N';'
FROM sys.columns c JOIN sys.tables t ON t.object_id=c.object_id
WHERE c.name IN ('utsysgen','utblock','utispartial','utrecordversion') AND c.default_object_id=0;
EXEC sys.sp_executesql @defaults;

GO

IF OBJECT_ID('dbo.religion','U') IS NULL CREATE TABLE dbo.religion (Id int NOT NULL PRIMARY KEY, religioncode nvarchar(20), stxt nvarchar(100), ltxt nvarchar(500), utblock bit NOT NULL DEFAULT(0));
IF OBJECT_ID('dbo.utnumberhdr','U') IS NULL CREATE TABLE dbo.utnumberhdr (id int NOT NULL PRIMARY KEY, clientid int, fieldid nvarchar(100), modelname nvarchar(100), prefix nvarchar(50), postfix nvarchar(50), counterlength int, totallength int, startvalue int, endvalue int, allowpadding bit, paddingvalue nvarchar(10), isdefault bit, utblock bit NOT NULL DEFAULT(0));

GO

CREATE OR ALTER VIEW dbo.utusers AS SELECT Id userid, Username username, FullName firstname, CAST('' AS nvarchar(100)) lastname, EmployeeId empid, ClientId userclientid, EntityId entityid, Blocked utblock FROM dbo.ArgUsers;

GO

CREATE OR ALTER VIEW dbo.utuserent AS SELECT u.Id*100000+e.id id,u.Id userid,e.id entityid,CAST(0 AS bit) utblock FROM dbo.ArgUsers u JOIN dbo.OrgLegalentity e ON e.clientid=u.ClientId WHERE u.Blocked=0 AND e.utblock=0;

GO

IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=337) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(337,N'M',N'Male',N'Male',N'GEND',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=338) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(338,N'F',N'Female',N'Female',N'GEND',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=339) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(339,N'O',N'Other',N'Other',N'GEND',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=125) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(125,N'ACT',N'Active',N'Active',N'JBSTS',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=126) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(126,N'RES',N'Resigned',N'Resigned',N'JBSTS',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=127) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(127,N'TER',N'Terminated',N'Terminated',N'JBSTS',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=267) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(267,N'ADMIN',N'Administration',N'Administration',N'ECAT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=268) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(268,N'MGT',N'Management',N'Management',N'ECAT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=269) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(269,N'TEACH',N'Teaching Staff',N'Teaching Staff',N'ECAT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=571) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(571,N'SUPPORT',N'Support Staff',N'Support Staff',N'ECAT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=561) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(561,N'ACT',N'Active',N'Active',N'ORGST',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=562) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(562,N'DEV',N'Under Development',N'Under Development',N'ORGST',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=563) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(563,N'SUS',N'Suspended',N'Suspended',N'ORGST',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=368) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(368,N'QUAL',N'Qualification',N'Qualification',N'QTYP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2001) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2001,N'ACT',N'Active',N'Active',N'AACT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2002) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2002,N'INA',N'Inactive',N'Inactive',N'AACT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2010) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2010,N'PERM',N'Permanent',N'Permanent',N'EMPTP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2011) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2011,N'CONTRACT',N'Contract',N'Contract',N'EMPTP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2012) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2012,N'PROB',N'Probation',N'Probation',N'EMPTP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2013) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2013,N'TEMP',N'Temporary',N'Temporary',N'EMPTP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2020) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2020,N'FULL',N'Full Time',N'Full Time',N'PSTYP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2021) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2021,N'PART',N'Part Time',N'Part Time',N'PSTYP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2030) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2030,N'SINGLE',N'Single',N'Single',N'MRSTS',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2031) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2031,N'MARRIED',N'Married',N'Married',N'MRSTS',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2040) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2040,N'HOME',N'Home',N'Home',N'ADDTP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2041) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2041,N'PERM',N'Permanent',N'Permanent',N'ADDTP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2042) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2042,N'OFFICE',N'Office',N'Office',N'ADDTP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2050) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2050,N'SALARY',N'Salary',N'Salary',N'ACUNT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2051) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2051,N'CURRENT',N'Current',N'Current',N'ACUNT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2052) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2052,N'SAVINGS',N'Savings',N'Savings',N'ACUNT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2060) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2060,N'DAYS',N'Days',N'Days',N'LVUNT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2061) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2061,N'HOURS',N'Hours',N'Hours',N'LVUNT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2062) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2062,N'MONTHS',N'Months',N'Months',N'LVUNT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2070) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2070,N'PEND',N'Pending',N'Pending',N'LVSTS',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2071) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2071,N'APP',N'Approved',N'Approved',N'LVSTS',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2072) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2072,N'REJ',N'Rejected',N'Rejected',N'LVSTS',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2073) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2073,N'CAN',N'Cancelled',N'Cancelled',N'LVSTS',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2080) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2080,N'PEND',N'Pending',N'Pending',N'LRST',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2081) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2081,N'APP',N'Approved',N'Approved',N'LRST',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2082) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2082,N'REJ',N'Rejected',N'Rejected',N'LRST',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2090) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2090,N'FIXED',N'Fixed',N'Fixed',N'LDT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2091) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2091,N'PERCENT',N'Percentage',N'Percentage',N'LDT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2100) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2100,N'ENTITY',N'Branch',N'Branch',N'APLON',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2101) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2101,N'DEPARTMENT',N'Department',N'Department',N'APLON',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2102) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2102,N'ALL',N'All Employees',N'All Employees',N'APLON',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2110) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2110,N'JOIN',N'Joining',N'Joining',N'EMACT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2111) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2111,N'PROM',N'Promotion',N'Promotion',N'EMACT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2112) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2112,N'TRANSFER',N'Transfer',N'Transfer',N'EMACT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2113) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2113,N'RESIGN',N'Resignation',N'Resignation',N'EMACT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2120) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2120,N'PEND',N'Pending',N'Pending',N'TRTP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2121) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2121,N'APP',N'Approved',N'Approved',N'TRTP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2122) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2122,N'POST',N'Posted',N'Posted',N'TRTP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2130) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2130,N'OPEN',N'Open',N'Open',N'STATS',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2131) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2131,N'CLOSED',N'Closed',N'Closed',N'STATS',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2140) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2140,N'DAILY',N'Daily',N'Daily',N'ATNTY',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2141) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2141,N'MONTHLY',N'Monthly',N'Monthly',N'ATNTY',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2150) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2150,N'SHIFT',N'Shift Based',N'Shift Based',N'ATPOL',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2151) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2151,N'FLEX',N'Flexible',N'Flexible',N'ATPOL',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2160) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2160,N'PAID',N'Paid',N'Paid',N'LTYP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2161) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2161,N'UNPAID',N'Unpaid',N'Unpaid',N'LTYP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2170) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2170,N'ADD',N'Addition',N'Addition',N'ADJP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2171) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2171,N'DEDUCT',N'Deduction',N'Deduction',N'ADJP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2180) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2180,N'HIGH',N'High',N'High',N'PRIT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2181) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2181,N'NORMAL',N'Normal',N'Normal',N'PRIT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2182) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2182,N'LOW',N'Low',N'Low',N'PRIT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2190) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2190,N'BANK',N'Bank',N'Bank',N'PAYTP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2191) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2191,N'CASH',N'Cash',N'Cash',N'PAYTP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2200) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2200,N'NATIONAL',N'National ID',N'National ID',N'HRDTP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2201) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2201,N'PASSPORT',N'Passport',N'Passport',N'HRDTP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2202) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2202,N'QUAL',N'Qualification Certificate',N'Qualification Certificate',N'HRDTP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2210) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2210,N'PUBLIC',N'Public',N'Public',N'HOTYP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2211) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2211,N'ORG',N'Organization',N'Organization',N'HOTYP',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2220) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2220,N'RECOGNITION',N'Recognition',N'Recognition',N'HRRCA',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2221) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2221,N'TRAINING',N'Training',N'Training',N'HRRCA',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2222) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2222,N'ACHIEVEMENT',N'Achievement',N'Achievement',N'HRRCA',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2223) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2223,N'DISCIPLINARY',N'Disciplinary',N'Disciplinary',N'HRRCA',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2224) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2224,N'LEGAL',N'Legal',N'Legal',N'HRRCA',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2230) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2230,N'HEAD',N'Head Office',N'Head Office',N'ENTITYTYPE',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2231) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2231,N'BRANCH',N'Branch',N'Branch',N'ENTITYTYPE',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2300) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2300,N'A+',N'A+',N'A+',N'BLOOD',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2301) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2301,N'A-',N'A-',N'A-',N'BLOOD',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2302) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2302,N'B+',N'B+',N'B+',N'BLOOD',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2303) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2303,N'B-',N'B-',N'B-',N'BLOOD',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2304) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2304,N'AB+',N'AB+',N'AB+',N'BLOOD',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2305) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2305,N'AB-',N'AB-',N'AB-',N'BLOOD',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2306) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2306,N'O+',N'O+',N'O+',N'BLOOD',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2307) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2307,N'O-',N'O-',N'O-',N'BLOOD',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2401) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2401,N'1',N'January',N'January',N'M',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2402) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2402,N'2',N'February',N'February',N'M',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2403) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2403,N'3',N'March',N'March',N'M',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2404) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2404,N'4',N'April',N'April',N'M',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2405) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2405,N'5',N'May',N'May',N'M',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2406) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2406,N'6',N'June',N'June',N'M',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2407) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2407,N'7',N'July',N'July',N'M',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2408) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2408,N'8',N'August',N'August',N'M',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2409) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2409,N'9',N'September',N'September',N'M',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2410) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2410,N'10',N'October',N'October',N'M',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2411) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2411,N'11',N'November',N'November',N'M',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=2412) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(2412,N'12',N'December',N'December',N'M',0);

IF NOT EXISTS(SELECT 1 FROM dbo.Country WHERE Id=1) INSERT dbo.Country(Id,countrycode,countryname,isocode,dialcode) VALUES(1,'PK','Pakistan','PAK','+92');
IF NOT EXISTS(SELECT 1 FROM dbo.State WHERE Id=1) INSERT dbo.State(Id,countryid,stxt,statecode) VALUES(1,1,'Sindh','SD');
IF NOT EXISTS(SELECT 1 FROM dbo.City WHERE Id=1) INSERT dbo.City(Id,ctryid,stateid,citystxt,citycode) VALUES(1,1,1,'Karachi','KHI');
IF NOT EXISTS(SELECT 1 FROM dbo.OrgLegalentity WHERE Id=1) INSERT dbo.OrgLegalentity(id,clientid,entityno,stxt,shortname,campusname,countryid,cityid) VALUES(1,1,'ARG-001','ARG ERP Head Office','ARG ERP','Head Office',1,1);
IF NOT EXISTS(SELECT 1 FROM dbo.OrgDepartment WHERE Id=1) INSERT dbo.OrgDepartment(Id,depcode,depstxt,clientid,opunitid) VALUES(1,'00001','Human Resources',1,1);
IF NOT EXISTS(SELECT 1 FROM dbo.OrgDesignation WHERE Id=1) INSERT dbo.OrgDesignation(Id,stxt,clientid) VALUES(1,'HR Administrator',1);
IF NOT EXISTS(SELECT 1 FROM dbo.EREmployeeinfohdr WHERE Id=1) INSERT dbo.EREmployeeinfohdr(Id,clientid,entityid,employeecode,firstname,lastname,depid,designationid,empstatusid,lvemployeecateg,joiningdate) VALUES(1,1,1,'SYS-ADMIN','System','Administrator',1,1,125,267,CONVERT(date,GETDATE()));
IF NOT EXISTS(SELECT 1 FROM dbo.SMPolicy WHERE Id=1) INSERT dbo.SMPolicy(Id,clientid,entityid,defaultctryid,defaultcityid,utblock) VALUES(1,1,1,1,1,0);
IF NOT EXISTS(SELECT 1 FROM dbo.religion WHERE Id=1) INSERT dbo.religion(Id,religioncode,stxt,ltxt) VALUES(1,'ISLAM','Islam','Islam');

IF COL_LENGTH('dbo.utnumberhdr','controllerid') IS NULL ALTER TABLE dbo.utnumberhdr ADD controllerid nvarchar(100) NULL;

GO

IF NOT EXISTS(SELECT 1 FROM dbo.utnumberhdr WHERE id=1) INSERT dbo.utnumberhdr(id,clientid,fieldid,modelname,controllerid,counterlength,startvalue,allowpadding,paddingvalue,isdefault) VALUES(1,1,'employeecode','employee','EmployeeProfile',6,1,1,'0',1);

IF OBJECT_ID('dbo.WF_ExecutionReceipts','U') IS NULL CREATE TABLE dbo.WF_ExecutionReceipts (ServiceName nvarchar(100) NOT NULL, ExecutionId uniqueidentifier NOT NULL, RequestHash nvarchar(64) NOT NULL, ResultEnvelope nvarchar(max) NOT NULL, CompletedAtUtc datetime2 NOT NULL, CONSTRAINT PK_WF_ExecutionReceipts PRIMARY KEY (ServiceName, ExecutionId));


GO

-- Supporting tables used by raw SQL attachment operations rather than EF entities.
IF OBJECT_ID('dbo.SMAttachments','U') IS NULL CREATE TABLE dbo.SMAttachments(id int NOT NULL PRIMARY KEY,attchmentno nvarchar(50) NULL,refdocid int NULL,doctypeid int NULL,docname nvarchar(500) NULL,browsepath nvarchar(2000) NULL,uploadpath nvarchar(2000) NULL,lovreftypeid int NULL,externallink nvarchar(2000) NULL,typeid int NULL,entityid int NULL,clientid int NULL,utdoctype int NULL,utsubdoctype int NULL,utsysgen bit NOT NULL DEFAULT(0),utispartial bit NOT NULL DEFAULT(0),utblock bit NOT NULL DEFAULT(0),utrecordversion int NOT NULL DEFAULT(1),utaddtime datetime NULL,utadduser varchar(25) NULL,utaddsystem varchar(25) NULL,utaddip varchar(25) NULL,utaddappversion varchar(10) NULL,utadddbversion varchar(10) NULL,utaddid bigint NULL,utaddsessionid int NULL,utedittime datetime NULL,utedituser varchar(25) NULL,uteditsystem varchar(25) NULL,uteditip varchar(25) NULL,uteditappversion varchar(10) NULL,uteditdbversion varchar(10) NULL,uteditid bigint NULL,uteditsessionid int NULL);
IF OBJECT_ID('dbo.EREmpDocument','U') IS NULL CREATE TABLE dbo.EREmpDocument(Id int NOT NULL PRIMARY KEY,documenttypeid int NULL,employeeid int NULL,utdoctype int NULL,utsubdoctype int NULL,utsysgen bit NOT NULL DEFAULT(0),utispartial bit NOT NULL DEFAULT(0),utblock bit NOT NULL DEFAULT(0),utrecordversion int NOT NULL DEFAULT(1),utaddtime datetime NULL,utadduser varchar(25) NULL,utaddsystem varchar(25) NULL,utaddip varchar(25) NULL,utaddappversion varchar(10) NULL,utadddbversion varchar(10) NULL,utaddid bigint NULL,utaddsessionid int NULL,utedittime datetime NULL,utedituser varchar(25) NULL,uteditsystem varchar(25) NULL,uteditip varchar(25) NULL,uteditappversion varchar(10) NULL,uteditdbversion varchar(10) NULL,uteditid bigint NULL,uteditsessionid int NULL);
IF OBJECT_ID('dbo.HREmployeeDocumentFile','U') IS NULL CREATE TABLE dbo.HREmployeeDocumentFile(attachmentid int NOT NULL,clientid int NOT NULL,filename nvarchar(260) NOT NULL,contenttype nvarchar(150) NOT NULL,filedata varbinary(max) NOT NULL,createdat datetime2 NOT NULL,updatedat datetime2 NOT NULL,PRIMARY KEY(attachmentid,clientid));
IF COL_LENGTH('dbo.EREmploymenttype','utblock') IS NULL ALTER TABLE dbo.EREmploymenttype ADD utblock bit NOT NULL DEFAULT(0);
IF COL_LENGTH('dbo.EREmployeeinfohdr','teachingclassids') IS NULL ALTER TABLE dbo.EREmployeeinfohdr ADD teachingclassids nvarchar(max) NULL;
IF COL_LENGTH('dbo.SMCourse','code') IS NULL ALTER TABLE dbo.SMCourse ADD code nvarchar(20) NULL;
IF COL_LENGTH('dbo.SMCourse','entityid') IS NULL ALTER TABLE dbo.SMCourse ADD entityid int NULL;
IF OBJECT_ID('dbo.SMSubject','U') IS NULL CREATE TABLE dbo.SMSubject(Id int NOT NULL PRIMARY KEY,code nvarchar(20) NULL,stxt nvarchar(150) NULL,clientid int NULL,entityid int NULL,utblock bit NOT NULL DEFAULT(0));

IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=255) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(255,'DAY','Days','Days','LVUNT',0);
IF NOT EXISTS(SELECT 1 FROM dbo.lov WHERE id=256) INSERT dbo.lov(id,code,stxt,ltxt,vtype,utblock) VALUES(256,'HOUR','Hours','Hours','LVUNT',0);

GO
-- Explicitly labelled synthetic data. Machine codes are NULL, so demo employees never receive real ZKT punches.
IF NOT EXISTS(SELECT 1 FROM dbo.OrgDepartment WHERE depcode='DEMO-OPS') INSERT dbo.OrgDepartment(Id,depcode,depstxt,clientid,opunitid) SELECT ISNULL(MAX(Id),0)+1,'DEMO-OPS','Operations (Demo)',1,1 FROM dbo.OrgDepartment;
IF NOT EXISTS(SELECT 1 FROM dbo.OrgDesignation WHERE desgcode='DEMOO') INSERT dbo.OrgDesignation(Id,desgcode,stxt,clientid) SELECT ISNULL(MAX(Id),0)+1,'DEMOO','Officer (Demo)',1 FROM dbo.OrgDesignation;
IF NOT EXISTS(SELECT 1 FROM dbo.ERPositiontype WHERE code='DEMO-FT') INSERT dbo.ERPositiontype(id,code,stxt,emptypeid,entityid,clientid) SELECT ISNULL(MAX(id),0)+1,'DEMO-FT','Full Time (Demo)',2010,1,1 FROM dbo.ERPositiontype;
IF NOT EXISTS(SELECT 1 FROM dbo.HRGrade WHERE code='DEMO-G1') INSERT dbo.HRGrade(id,clientid,entityid,code,minsalary,maxsalary,utblock,utrecordversion) SELECT ISNULL(MAX(id),0)+1,1,1,'DEMO-G1',40000,80000,0,1 FROM dbo.HRGrade;
IF NOT EXISTS(SELECT 1 FROM dbo.ETShifthdr WHERE code='DEMO-DAY') INSERT dbo.ETShifthdr(Id,code,stxt,entityid,clientid,wosaturday,wosunday,utblock) SELECT ISNULL(MAX(Id),0)+1,'DEMO-DAY','Day Shift (Demo)',1,1,1,1,0 FROM dbo.ETShifthdr;
DECLARE @shift int=(SELECT TOP 1 Id FROM dbo.ETShifthdr WHERE code='DEMO-DAY');
DECLARE @day int=1;
WHILE @day<=5
BEGIN
IF NOT EXISTS(SELECT 1 FROM dbo.ETShiftdtl WHERE shiftid=@shift AND dayid=@day)
INSERT dbo.ETShiftdtl(Id,shiftid,lineindex,stxt,dayid,intime,outtime,duration,startdate,utblock)
SELECT ISNULL(MAX(Id),0)+1,@shift,@day,'Demo working day',@day,'2000-01-01T09:00:00','2000-01-01T17:00:00',8,'2020-01-01',0 FROM dbo.ETShiftdtl;
SET @day=@day+1;
END;
DECLARE @i int=1,@emp int,@atn int;
WHILE @i<=3
BEGIN
IF NOT EXISTS(SELECT 1 FROM dbo.EREmployeeinfohdr WHERE employeecode=CONCAT('DEMO-',@i))
INSERT dbo.EREmployeeinfohdr(Id,clientid,entityid,employeecode,firstname,lastname,depid,designationid,empstatusid,lvemployeecateg,employeetypeid,lovpositiontypeid,shiftid,joiningdate,dateofbirth,genderid,birthcountryid,birthcityid,nationalityid,machinecode)
SELECT ISNULL(MAX(Id),0)+1,1,1,CONCAT('DEMO-',@i),CASE @i WHEN 1 THEN 'Ayesha' WHEN 2 THEN 'Ali' ELSE 'Sara' END,'Demo',
(SELECT TOP 1 Id FROM dbo.OrgDepartment WHERE depcode='DEMO-OPS'),(SELECT TOP 1 Id FROM dbo.OrgDesignation WHERE desgcode='DEMOO'),125,267,2010,(SELECT TOP 1 Id FROM dbo.ERPositiontype WHERE code='DEMO-FT'),@shift,DATEADD(month,-6,CONVERT(date,GETDATE())),'1995-01-01',CASE WHEN @i=2 THEN 337 ELSE 338 END,1,1,1,NULL FROM dbo.EREmployeeinfohdr;
SELECT @emp=Id FROM dbo.EREmployeeinfohdr WHERE employeecode=CONCAT('DEMO-',@i);
IF NOT EXISTS(SELECT 1 FROM dbo.HRAtnintout WHERE empid=@emp AND remarks='Sample attendance (Demo)')
BEGIN
SELECT @atn=ISNULL(MAX(id),0)+1 FROM dbo.HRAtnintout;
INSERT dbo.HRAtnintout(id,empid,atnactivityid,atntypeid,dttime,orgdttime,clientid,entityid,shiftid,remarks,utblock,utsysgen,utispartial,utrecordversion)
VALUES(@atn,@emp,1,1,DATEADD(hour,9,CAST(CONVERT(date,GETDATE()) AS datetime2)),DATEADD(hour,9,CAST(CONVERT(date,GETDATE()) AS datetime2)),1,1,@shift,'Sample attendance (Demo)',0,0,0,1),
(@atn+1,@emp,2,1,DATEADD(hour,17,CAST(CONVERT(date,GETDATE()) AS datetime2)),DATEADD(hour,17,CAST(CONVERT(date,GETDATE()) AS datetime2)),1,1,@shift,'Sample attendance (Demo)',0,0,0,1);
END;
SET @i=@i+1;
END;
IF NOT EXISTS(SELECT 1 FROM dbo.HRAtnactivity WHERE Id=1) INSERT dbo.HRAtnactivity(Id,stxt,value,orderid,clientid,utblock) VALUES(1,'Time In',1,1,1,0);
IF NOT EXISTS(SELECT 1 FROM dbo.HRAtnactivity WHERE Id=2) INSERT dbo.HRAtnactivity(Id,stxt,value,orderid,clientid,utblock) VALUES(2,'Time Out',2,2,1,0);
IF NOT EXISTS(SELECT 1 FROM dbo.ETLeavetype WHERE code='DEMO-CL') INSERT dbo.ETLeavetype(id,code,stxt,entityid,clientid,leaves,lvunitid,availunitid,allowinprob,quotavalidate,utblock) SELECT ISNULL(MAX(id),0)+1,'DEMO-CL','Casual Leave (Demo)',1,1,10,255,255,1,1,0 FROM dbo.ETLeavetype;
IF NOT EXISTS(SELECT 1 FROM dbo.ETLeaveRequest WHERE requestno='DEMO-LV-001')
INSERT dbo.ETLeaveRequest(Id,requestno,requestdate,employeeid,leavetypeid,leaveunitid,leavedays,fromdate,tilldate,reason,statusid,entityid,clientid)
SELECT ISNULL(MAX(Id),0)+1,'DEMO-LV-001',GETDATE(),(SELECT TOP 1 Id FROM dbo.EREmployeeinfohdr WHERE employeecode='DEMO-1'),(SELECT TOP 1 id FROM dbo.ETLeavetype WHERE code='DEMO-CL'),255,1,DATEADD(day,7,CONVERT(date,GETDATE())),DATEADD(day,7,CONVERT(date,GETDATE())),'Sample leave request (Demo)',2,1,1 FROM dbo.ETLeaveRequest;

UPDATE dbo.ETLeaveRequest SET statusid=2,leaveunitid=255 WHERE requestno='DEMO-LV-001' AND statusid=2070;
UPDATE dbo.ETLeavetype SET lvunitid=255,availunitid=255 WHERE code='DEMO-CL' AND lvunitid=2060;

GO
