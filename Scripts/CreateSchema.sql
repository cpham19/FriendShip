﻿IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

CREATE TABLE [Persons] (
    [PersonId] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    [Username] nvarchar(450) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [IsAdmin] bit NOT NULL DEFAULT 0,
	[ProfileDescription] nvarchar(max) NOT NULL DEFAULT 'Profile Description',
    CONSTRAINT [PK_Persons] PRIMARY KEY ([PersonId]),
);

GO

CREATE TABLE [WallPosts] (
    [WallPostId] int NOT NULL IDENTITY,
	[PosterName] nvarchar(max) NULL,
    [Description] nvarchar(max) NULL,
	[DatePosted] datetime2 NOT NULL,
	[PosterId] int NOT NULL,
	[UserIdOfProfile] int NOT NULL,
    CONSTRAINT [PK_WallPosts] PRIMARY KEY ([WallPostId]),
    CONSTRAINT [FK_WallPosts_Persons_PosterId] FOREIGN KEY ([PosterId]) REFERENCES [Persons] ([PersonId]) ON DELETE NO ACTION,
	CONSTRAINT [FK_WallPosts_Persons_UserIdOfProfile] FOREIGN KEY ([UserIdOfProfile]) REFERENCES [Persons] ([PersonId]) ON DELETE NO ACTION
);

GO

CREATE TABLE [ReplyPosts] (
    [ReplyPostId] int NOT NULL IDENTITY,
	[PosterName] nvarchar(max) NULL,
    [Description] nvarchar(max) NULL,
	[DatePosted] datetime2 NOT NULL,
	[WallPostId] int NOT NULL,
	[PosterId] int NOT NULL,
    CONSTRAINT [PK_ReplyPosts] PRIMARY KEY ([ReplyPostId]),
    CONSTRAINT [FK_ReplyPosts_WallPosts_WallPostId] FOREIGN KEY ([WallPostId]) REFERENCES [WallPosts] ([WallPostId]) ON DELETE NO ACTION,
	CONSTRAINT [FK_ReplyPosts_Persons_PosterId] FOREIGN KEY ([PosterId]) REFERENCES [Persons] ([PersonId]) ON DELETE NO ACTION
);

GO	

CREATE TABLE [Forums] (
    [ForumId] int NOT NULL IDENTITY,
    [ForumName] nvarchar(max) NULL,
	[PosterId] int NOT NULL,
    CONSTRAINT [PK_Forums] PRIMARY KEY ([ForumId]),
	CONSTRAINT [FK_Forums_Persons_PosterId] FOREIGN KEY ([PosterId]) REFERENCES [Persons] ([PersonId]) ON DELETE NO ACTION
);

GO

CREATE TABLE [Topics] (
    [TopicId] int NOT NULL IDENTITY,
    [TopicName] nvarchar(max) NULL,
    [TopicContent] nvarchar(max) NULL,
    [TopicDate] datetime2 NOT NULL,
    [ForumId] int NOT NULL,
	[PosterId] int NOT NULL,
	[PosterName] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Topics] PRIMARY KEY ([TopicId]),
	CONSTRAINT [FK_Topics_Forums_ForumId] FOREIGN KEY ([ForumId]) REFERENCES [Forums] ([ForumId]) ON DELETE NO ACTION,
	CONSTRAINT [FK_Topics_Persons_PosterId] FOREIGN KEY ([PosterId]) REFERENCES [Persons] ([PersonId]) ON DELETE NO ACTION
);

GO

CREATE TABLE [Replies] (
    [ReplyId] int NOT NULL IDENTITY,
    [ReplyContent] nvarchar(max) NULL,
	[ReplyDate] datetime2 NOT NULL,
    [TopicId] int NOT NULL,
	[PosterId] int NOT NULL,
	[PosterName] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Replies] PRIMARY KEY ([ReplyId]),
    CONSTRAINT [FK_Replies_Topics_TopicId] FOREIGN KEY ([TopicId]) REFERENCES [Topics] ([TopicId]) ON DELETE NO ACTION,
	CONSTRAINT [FK_Replies_Persons_PosterId] FOREIGN KEY ([PosterId]) REFERENCES [Persons] ([PersonId]) ON DELETE NO ACTION
);

GO

CREATE TABLE [Friends] (
	[PersonOneId] int NOT NULL,
	[PersonTwoId] int NOT NULL,
	[StatusCode] int NOT NULL DEFAULT 0,
	CONSTRAINT [PK_Friends] PRIMARY KEY ([PersonOneId], [PersonTwoId]),
	CONSTRAINT [FK_Friends_Persons_PersonOneId] FOREIGN KEY ([PersonOneId]) REFERENCES [Persons] ([PersonId]) ON DELETE NO ACTION,
	CONSTRAINT [FK_Friends_Persons_PersonTwoId] FOREIGN KEY ([PersonTwoId]) REFERENCES [Persons] ([PersonId]) ON DELETE NO ACTION
);

GO

CREATE TABLE [Groups] (
    [GroupId] int NOT NULL IDENTITY,
    [GroupName] nvarchar(max) NULL,
    [Description] nvarchar(450) NOT NULL,
    [DateCreated] datetime2 NOT NULL,
	[GroupPictureUrl] nvarchar(max) NOT NULL,
	[GroupCreatorId] int NOT NULL,
    CONSTRAINT [PK_Groups] PRIMARY KEY ([GroupId]),
	CONSTRAINT [FK_Groups_Persons_GroupCreatorId] FOREIGN KEY ([GroupCreatorId]) REFERENCES [Persons] ([PersonId]) ON DELETE NO ACTION
);

GO

CREATE TABLE [GroupMembers] (
    [GroupId] int NOT NULL,
    [GroupMemberId] int NOT NULL,
    CONSTRAINT [PK_GroupMembers] PRIMARY KEY ([GroupId], [GroupMemberId]),
	CONSTRAINT [FK_GroupsMembers_GroupId] FOREIGN KEY ([GroupId]) REFERENCES [Groups] ([GroupId]) ON DELETE NO ACTION,
	CONSTRAINT [FK_GroupsMembers_GroupMemberId] FOREIGN KEY ([GroupMemberId]) REFERENCES [Persons] ([PersonId]) ON DELETE NO ACTION
);

GO

CREATE INDEX [IX_FK_WallPosts_PosterId] ON [WallPosts] ([PosterId]);
CREATE INDEX [IX_FK_WallPosts_UserIdOfProfile] ON [WallPosts] ([UserIdOfProfile]);

GO

CREATE INDEX [IX_FK_ReplyPosts_WallPostId] ON [ReplyPosts] ([WallPostId]);
CREATE INDEX [IX_FK_ReplyPosts_PosterId] ON [ReplyPosts] ([PosterId]);

GO

CREATE INDEX [IX_FK_Topics_ForumId] ON [Topics] ([ForumId]);
CREATE INDEX [IX_FK_Topics_PosterId] ON [Topics] ([PosterId]);

GO 

CREATE INDEX [IX_FK_Replies_TopicId] ON [Replies] ([TopicId]);
CREATE INDEX [IX_FK_Replies_PosterId] ON [Replies] ([PosterId]);

GO

CREATE INDEX [IX_FK_Friends_PersonOneId] ON [Friends] ([PersonOneId]);
CREATE INDEX [IX_FK_Friends_PersonTwoId] ON [Friends] ([PersonTwoId]);

GO

CREATE INDEX [IX_FK_Groups_GroupCreatorId] ON [Groups] ([GroupCreatorId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20190726011148_InitialSchema', N'2.2.1-servicing-10028');

GO

