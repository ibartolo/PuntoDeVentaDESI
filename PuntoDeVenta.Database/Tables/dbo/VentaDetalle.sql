CREATE TABLE [dbo].[VentaDetalle]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [VentaId] BIGINT NOT NULL,
    [ProductoId] BIGINT NOT NULL,
    [Cantidad] DECIMAL (18, 4) NOT NULL,
    [PrecioUnitario] DECIMAL (18, 4) NOT NULL,
    [Importe] DECIMAL (18, 4) NOT NULL,
    [Estatus] BIT CONSTRAINT [DF_VentaDetalle_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_VentaDetalle_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_VentaDetalle] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_VentaDetalle_Venta] FOREIGN KEY ([VentaId]) REFERENCES [dbo].[Venta] ([Id]),
    CONSTRAINT [FK_VentaDetalle_Producto] FOREIGN KEY ([ProductoId]) REFERENCES [dbo].[Producto] ([Id])
);
