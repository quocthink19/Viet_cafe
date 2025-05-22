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

CREATE TABLE [Users] (
    [Id] uniqueidentifier NOT NULL,
    [Username] nvarchar(max) NOT NULL,
    [PasswordHash] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [role] int NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250416082040_InitialCreate', N'8.0.8');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'Username');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [Users] ALTER COLUMN [Username] nvarchar(max) NULL;
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'PasswordHash');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [Users] ALTER COLUMN [PasswordHash] nvarchar(max) NULL;
GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'Email');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [Users] ALTER COLUMN [Email] nvarchar(max) NULL;
GO

CREATE TABLE [Customers] (
    [Id] uniqueidentifier NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    [FullName] nvarchar(max) NULL,
    [BirthDate] datetime2 NOT NULL,
    [gender] int NOT NULL,
    [Wallet] decimal(18,2) NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_Customers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Customers_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX [IX_Customers_UserId] ON [Customers] ([UserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250419072158_Customer', N'8.0.8');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250419072707_updateCustomer', N'8.0.8');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Users] ADD [isActive] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250519083403_update_data', N'8.0.8');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250519141559_category', N'8.0.8');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Categories] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250519142707_update', N'8.0.8');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Toppings] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [price] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Toppings] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250520094941_update_t', N'8.0.8');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

EXEC sp_rename N'[Toppings].[price]', N'Price', N'COLUMN';
GO

CREATE TABLE [Sizes] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(max) NULL,
    [Volume] float NULL,
    CONSTRAINT [PK_Sizes] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250520103038_update_siz', N'8.0.8');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Users] ADD [RefreshToken] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [Users] ADD [RefreshTokenExpiryTime] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250520134234_update_user', N'8.0.8');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

EXEC sp_rename N'[Sizes].[Volume]', N'Value', N'COLUMN';
GO

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Toppings]') AND [c].[name] = N'Price');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Toppings] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [Toppings] ALTER COLUMN [Price] float NULL;
GO

CREATE TABLE [Products] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(max) NULL,
    [Description] nvarchar(max) NULL,
    [Price] float NULL,
    [Rating] float NULL,
    [PurchaseCount] float NULL,
    [IsAvaillable] bit NOT NULL,
    [CategoryId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Products_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Customizes] (
    [Id] uniqueidentifier NOT NULL,
    [Milk] int NOT NULL,
    [Ice] nvarchar(max) NOT NULL,
    [Sugar] nvarchar(max) NOT NULL,
    [Temperature] nvarchar(max) NOT NULL,
    [Extra] float NULL,
    [SizeId] uniqueidentifier NOT NULL,
    [ProductId] uniqueidentifier NOT NULL,
    [Price] float NULL,
    CONSTRAINT [PK_Customizes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Customizes_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Customizes_Sizes_SizeId] FOREIGN KEY ([SizeId]) REFERENCES [Sizes] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [CustomizeToppings] (
    [CustomizeId] uniqueidentifier NOT NULL,
    [ToppingId] uniqueidentifier NOT NULL,
    [Quantity] int NOT NULL,
    CONSTRAINT [PK_CustomizeToppings] PRIMARY KEY ([CustomizeId], [ToppingId]),
    CONSTRAINT [FK_CustomizeToppings_Customizes_CustomizeId] FOREIGN KEY ([CustomizeId]) REFERENCES [Customizes] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_CustomizeToppings_Toppings_ToppingId] FOREIGN KEY ([ToppingId]) REFERENCES [Toppings] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_Customizes_ProductId] ON [Customizes] ([ProductId]);
GO

CREATE INDEX [IX_Customizes_SizeId] ON [Customizes] ([SizeId]);
GO

CREATE INDEX [IX_CustomizeToppings_ToppingId] ON [CustomizeToppings] ([ToppingId]);
GO

CREATE INDEX [IX_Products_CategoryId] ON [Products] ([CategoryId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250521103146_customize', N'8.0.8');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Sizes] ADD [ExtraPrice] float NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250521105009_update_size', N'8.0.8');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Cart] (
    [Id] uniqueidentifier NOT NULL,
    [TotalAmount] float NOT NULL,
    [CustomerId] int NOT NULL,
    [CustomerId1] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_Cart] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Cart_Customers_CustomerId1] FOREIGN KEY ([CustomerId1]) REFERENCES [Customers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [CartItem] (
    [Id] uniqueidentifier NOT NULL,
    [CartId] int NOT NULL,
    [CartId1] uniqueidentifier NOT NULL,
    [CustomizeId] uniqueidentifier NOT NULL,
    [Quantity] int NOT NULL,
    [UnitPrice] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_CartItem] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CartItem_Cart_CartId1] FOREIGN KEY ([CartId1]) REFERENCES [Cart] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_CartItem_Customizes_CustomizeId] FOREIGN KEY ([CustomizeId]) REFERENCES [Customizes] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_Cart_CustomerId1] ON [Cart] ([CustomerId1]);
GO

CREATE INDEX [IX_CartItem_CartId1] ON [CartItem] ([CartId1]);
GO

CREATE UNIQUE INDEX [IX_CartItem_CustomizeId] ON [CartItem] ([CustomizeId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250522145246_update_cuz', N'8.0.8');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Cart] DROP CONSTRAINT [FK_Cart_Customers_CustomerId1];
GO

ALTER TABLE [CartItem] DROP CONSTRAINT [FK_CartItem_Cart_CartId1];
GO

ALTER TABLE [CartItem] DROP CONSTRAINT [FK_CartItem_Customizes_CustomizeId];
GO

ALTER TABLE [CartItem] DROP CONSTRAINT [PK_CartItem];
GO

DROP INDEX [IX_CartItem_CartId1] ON [CartItem];
GO

ALTER TABLE [Cart] DROP CONSTRAINT [PK_Cart];
GO

DROP INDEX [IX_Cart_CustomerId1] ON [Cart];
GO

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CartItem]') AND [c].[name] = N'CartId1');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [CartItem] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [CartItem] DROP COLUMN [CartId1];
GO

DECLARE @var5 sysname;
SELECT @var5 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Cart]') AND [c].[name] = N'CustomerId1');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [Cart] DROP CONSTRAINT [' + @var5 + '];');
ALTER TABLE [Cart] DROP COLUMN [CustomerId1];
GO

EXEC sp_rename N'[CartItem]', N'CartItems';
GO

EXEC sp_rename N'[Cart]', N'Carts';
GO

EXEC sp_rename N'[CartItems].[IX_CartItem_CustomizeId]', N'IX_CartItems_CustomizeId', N'INDEX';
GO

DECLARE @var6 sysname;
SELECT @var6 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CartItems]') AND [c].[name] = N'UnitPrice');
IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [CartItems] DROP CONSTRAINT [' + @var6 + '];');
ALTER TABLE [CartItems] ALTER COLUMN [UnitPrice] float NULL;
GO

DECLARE @var7 sysname;
SELECT @var7 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CartItems]') AND [c].[name] = N'CartId');
IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [CartItems] DROP CONSTRAINT [' + @var7 + '];');
ALTER TABLE [CartItems] ALTER COLUMN [CartId] uniqueidentifier NOT NULL;
GO

DECLARE @var8 sysname;
SELECT @var8 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Carts]') AND [c].[name] = N'CustomerId');
IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [Carts] DROP CONSTRAINT [' + @var8 + '];');
ALTER TABLE [Carts] ALTER COLUMN [CustomerId] uniqueidentifier NOT NULL;
GO

ALTER TABLE [CartItems] ADD CONSTRAINT [PK_CartItems] PRIMARY KEY ([Id]);
GO

ALTER TABLE [Carts] ADD CONSTRAINT [PK_Carts] PRIMARY KEY ([Id]);
GO

CREATE INDEX [IX_CartItems_CartId] ON [CartItems] ([CartId]);
GO

CREATE UNIQUE INDEX [IX_Carts_CustomerId] ON [Carts] ([CustomerId]);
GO

ALTER TABLE [CartItems] ADD CONSTRAINT [FK_CartItems_Carts_CartId] FOREIGN KEY ([CartId]) REFERENCES [Carts] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [CartItems] ADD CONSTRAINT [FK_CartItems_Customizes_CustomizeId] FOREIGN KEY ([CustomizeId]) REFERENCES [Customizes] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Carts] ADD CONSTRAINT [FK_Carts_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([Id]) ON DELETE CASCADE;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250522151241_Update_Customer_Cart_Relations', N'8.0.8');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250522151602_updateCart', N'8.0.8');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250522151925_AddCartTable', N'8.0.8');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250522152403_updatedata', N'8.0.8');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250522152559_update1', N'8.0.8');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250522152853_1', N'8.0.8');
GO

COMMIT;
GO

