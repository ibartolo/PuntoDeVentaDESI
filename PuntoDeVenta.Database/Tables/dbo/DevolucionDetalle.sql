CREATE TABLE [dbo].[DevolucionDetalle]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [CancelacionId] BIGINT NOT NULL,
    [VentaDetalleId] BIGINT NOT NULL,
    [ProductoId] BIGINT NOT NULL,
    [Cantidad] DECIMAL (18, 4) NOT NULL,
    [Importe] DECIMAL (18, 4) NOT NULL,
    [RegresaAStock] BIT CONSTRAINT [DF_DevolucionDetalle_RegresaAStock] DEFAULT ((0)) NOT NULL,
    [Estatus] BIT CONSTRAINT [DF_DevolucionDetalle_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_DevolucionDetalle_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_DevolucionDetalle] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_DevolucionDetalle_Cancelacion] FOREIGN KEY ([CancelacionId]) REFERENCES [dbo].[Cancelacion] ([Id]),
    CONSTRAINT [FK_DevolucionDetalle_VentaDetalle] FOREIGN KEY ([VentaDetalleId]) REFERENCES [dbo].[VentaDetalle] ([Id]),
    CONSTRAINT [FK_DevolucionDetalle_Producto] FOREIGN KEY ([ProductoId]) REFERENCES [dbo].[Producto] ([Id])
);
