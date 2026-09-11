CREATE TABLE [dbo].[CompraDetalle]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [CompraId] BIGINT NOT NULL,
    [ProductoId] BIGINT NOT NULL,
    [Cantidad] DECIMAL (18, 4) NOT NULL,
    [CostoUnitario] DECIMAL (18, 4) NOT NULL,
    [Importe] DECIMAL (18, 4) NOT NULL,
    [Estatus] BIT CONSTRAINT [DF_CompraDetalle_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_CompraDetalle_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_CompraDetalle] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CompraDetalle_Compra] FOREIGN KEY ([CompraId]) REFERENCES [dbo].[Compra] ([Id]),
    CONSTRAINT [FK_CompraDetalle_Producto] FOREIGN KEY ([ProductoId]) REFERENCES [dbo].[Producto] ([Id])
);
