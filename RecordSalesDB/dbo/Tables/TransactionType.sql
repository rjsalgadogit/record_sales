CREATE TABLE [dbo].[TransactionType] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [Description]  NVARCHAR (128) NULL,
    [CreationDate] DATETIME       NULL,
    [CreationUser] NVARCHAR (128) NULL,
    [ModifiedDate] DATETIME       NULL,
    [ModifiedUser] NVARCHAR (128) NULL
);

