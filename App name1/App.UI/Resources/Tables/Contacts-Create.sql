-- Contacts Table ========================
CREATE TABLE [Contacts] (
    [Id] uniqueidentifier PRIMARY KEY,
    [Name] nvarchar(200)  NOT NULL,
    [Email] nvarchar(200)  NULL,
    [Tel] nvarchar(200)  NULL,
    [Notes] NTEXT  NULL,
    [Type] uniqueidentifier  NULL,
    [DateOfBirth] datetime2  NULL,
    [TimeOfBirth] time  NULL,
    [Photo_FileName] nvarchar(1500)  NULL,
    [Number] int  NOT NULL,
    [IsNice] bit  NULL
)
