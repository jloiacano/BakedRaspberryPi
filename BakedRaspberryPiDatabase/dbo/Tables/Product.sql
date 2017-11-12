CREATE TABLE [dbo].[Product] (
    [Id]    INT            IDENTITY (1, 1) NOT NULL,
    [Name]  NVARCHAR (100) NULL,
    [Price] MONEY          NULL,
	[Description] NTEXT,
    [size] INT NULL, 
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

