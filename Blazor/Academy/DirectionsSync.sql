IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913122113_mssql_migration_546'
)
BEGIN
    CREATE TABLE [Directions] (
        [direction_id] TINYINT NOT NULL IDENTITY,
        [direction_name] nvarchar(50) NULL,
        CONSTRAINT [PK_Directions] PRIMARY KEY ([direction_id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913122113_mssql_migration_546'
)
BEGIN
    CREATE TABLE [Disciplines] (
        [discipline_id] SMALLINT NOT NULL,
        [discipline_name] nvarchar(150) NULL,
        [number_of_lessons] TINYINT NOT NULL,
        CONSTRAINT [PK_Disciplines] PRIMARY KEY ([discipline_id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913122113_mssql_migration_546'
)
BEGIN
    CREATE TABLE [Groups] (
        [group_id] int NOT NULL,
        [group_name] nchar(10) NULL,
        [direction] TINYINT NULL,
        [weekdays] TINYINT NULL,
        [start_time] time(0) NULL,
        [start_date] date NULL,
        CONSTRAINT [PK_Groups] PRIMARY KEY ([group_id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913122113_mssql_migration_546'
)
BEGIN
    CREATE TABLE [Teachers] (
        [teacher_id] SMALLINT NOT NULL IDENTITY,
        [last_name] nvarchar(50) NULL,
        [first_name] nvarchar(50) NULL,
        [middle_name] nvarchar(50) NULL,
        [birth_date] date NULL,
        [email] nvarchar(50) NULL,
        [phone] nchar(16) NULL,
        [photo] varbinary(max) NULL,
        [PhotoMimeType] nvarchar(50) NULL,
        [work_since] date NULL,
        [rate] smallmoney NULL,
        CONSTRAINT [PK_Teachers] PRIMARY KEY ([teacher_id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913122113_mssql_migration_546'
)
BEGIN
    CREATE TABLE [Students] (
        [stud_id] int NOT NULL IDENTITY,
        [last_name] nvarchar(50) NOT NULL,
        [first_name] nvarchar(50) NOT NULL,
        [middle_name] nvarchar(50) NULL,
        [birth_date] date NOT NULL,
        [email] nvarchar(50) NULL,
        [phone] nchar(16) NULL,
        [photo] varbinary(max) NULL,
        [PhotoMimeType] nvarchar(50) NULL,
        [group] int NULL,
        CONSTRAINT [PK_Students] PRIMARY KEY ([stud_id]),
        CONSTRAINT [FK_Students_Groups] FOREIGN KEY ([group]) REFERENCES [Groups] ([group_id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913122113_mssql_migration_546'
)
BEGIN
    CREATE INDEX [IX_Students_group] ON [Students] ([group]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260913122113_mssql_migration_546'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260913122113_mssql_migration_546', N'9.0.20');
END;

COMMIT;
GO

