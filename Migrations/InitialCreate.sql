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
GO

CREATE TABLE [Clients] (
    [ClientId] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [ContactDetails] nvarchar(max) NOT NULL,
    [Region] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Clients] PRIMARY KEY ([ClientId])
);
GO

CREATE TABLE [Contracts] (
    [ContractId] int NOT NULL IDENTITY,
    [ClientId] int NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NOT NULL,
    [Status] int NOT NULL,
    [ServiceLevel] nvarchar(max) NOT NULL,
    [SignedAgreementPath] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Contracts] PRIMARY KEY ([ContractId]),
    CONSTRAINT [FK_Contracts_Clients_ClientId] FOREIGN KEY ([ClientId]) REFERENCES [Clients] ([ClientId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [ServiceRequests] (
    [ServiceRequestId] int NOT NULL IDENTITY,
    [ContractId] int NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [CostUSD] decimal(18,2) NOT NULL,
    [CostZAR] decimal(18,2) NOT NULL,
    [Status] int NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_ServiceRequests] PRIMARY KEY ([ServiceRequestId]),
    CONSTRAINT [FK_ServiceRequests_Contracts_ContractId] FOREIGN KEY ([ContractId]) REFERENCES [Contracts] ([ContractId]) ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_Contracts_ClientId] ON [Contracts] ([ClientId]);
GO

CREATE INDEX [IX_ServiceRequests_ContractId] ON [ServiceRequests] ([ContractId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260413093813_InitialCreate', N'8.0.0');
GO

COMMIT;
GO

