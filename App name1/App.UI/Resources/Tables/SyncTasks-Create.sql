-- SyncTasks Table ========================
CREATE TABLE [SyncTasks] (
    [Id] uniqueidentifier PRIMARY KEY,
    [Index] numeric(27, 6)  NOT NULL,
    [ItemType] nvarchar(100)  NOT NULL,
    [ItemId] nvarchar(40)  NOT NULL,
    [Action] nvarchar(30)  NOT NULL,
    [Error] NTEXT  NULL,
    [LastAttemptUtc] datetime2  NULL,
    [Attempts] int  NOT NULL,
    [IsDone] bit  NOT NULL
)
