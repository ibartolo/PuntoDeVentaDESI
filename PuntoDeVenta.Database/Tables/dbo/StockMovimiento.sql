CREATE TABLE [dbo].[StockMovimiento]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [EmpresaId] BIGINT NOT NULL,
    [SucursalId] BIGINT NOT NULL,
    [ProductoId] BIGINT NOT NULL,
    [TipoMovimiento] NVARCHAR (20) NOT NULL,
    [Cantidad] DECIMAL (18, 4) NOT NULL,
    [ExistenciaAnterior] DECIMAL (18, 4) NOT NULL,
    [ExistenciaNueva] DECIMAL (18, 4) NOT NULL,
    [Motivo] NVARCHAR (300) NULL,
    [ReferenciaId] BIGINT NULL,
    [Estatus] BIT CONSTRAINT [DF_StockMovimiento_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_StockMovimiento_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_StockMovimiento] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_StockMovimiento_Empresa] FOREIGN KEY ([EmpresaId]) REFERENCES [dbo].[Empresa] ([Id]),
    CONSTRAINT [FK_StockMovimiento_Sucursal] FOREIGN KEY ([SucursalId]) REFERENCES [dbo].[Sucursal] ([Id]),
    CONSTRAINT [FK_StockMovimiento_Producto] FOREIGN KEY ([ProductoId]) REFERENCES [dbo].[Producto] ([Id]),
    CONSTRAINT [CK_StockMovimiento_Tipo] CHECK ([TipoMovimiento] IN (N'Ingreso', N'Ajuste', N'Devolucion', N'Venta'))
);
