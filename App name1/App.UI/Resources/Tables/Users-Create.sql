-- Users Table ========================
CREATE TABLE [Users] (
    [Id] uniqueidentifier PRIMARY KEY,
    [FirstName] nvarchar(200)  NOT NULL,
    [LastName] nvarchar(200)  NOT NULL,
    [Email] nvarchar(100)  NOT NULL,
    [Password] nvarchar(100)  NULL,
    [IsDeactivated] bit  NOT NULL
)
