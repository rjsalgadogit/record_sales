CREATE TABLE [dbo].[CashFlow] (
    [Id]                INT             IDENTITY (1, 1) NOT NULL,
    [Code]              NVARCHAR (128)  NULL,
    [TransactionTypeId] INT             NULL,
    [Amount]            DECIMAL (18, 2) NULL,
    [Description]       NVARCHAR (128)  NULL,
    [Notes]             NVARCHAR (MAX)  NULL,
    [CreationDate]      DATETIME        NULL,
    [CreationUser]      NVARCHAR (128)  NULL,
    [ModifiedDate]      DATETIME        NULL,
    [ModifiedUser]      NVARCHAR (128)  NULL
);

