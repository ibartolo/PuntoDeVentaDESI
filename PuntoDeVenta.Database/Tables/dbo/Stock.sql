CREATE TABLE [dbo].[Stock]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [EmpresaId] BIGINT NOT NULL,
    [SucursalId] BIGINT NOT NULL,
    [ProductoId] BIGINT NOT NULL,
    [Cantidad] DECIMAL (18, 4) CONSTRAINT [DF_Stock_Cantidad] DEFAULT ((0)) NOT NULL,
    [StockMinimo] DECIMAL (18, 4) CONSTRAINT [DF_Stock_StockMinimo] DEFAULT ((0)) NOT NULL,
    [Estatus] BIT CONSTRAINT [DF_Stock_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_Stock_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_Stock] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Stock_Empresa] FOREIGN KEY ([EmpresaId]) REFERENCES [dbo].[Empresa] ([Id]),
    CONSTRAINT [FK_Stock_Sucursal] FOREIGN KEY ([SucursalId]) REFERENCES [dbo].[Sucursal] ([Id]),
    CONSTRAINT [FK_Stock_Producto] FOREIGN KEY ([ProductoId]) REFERENCES [dbo].[Producto] ([Id])
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_Stock_Sucursal_Producto_Activo]
    ON [dbo].[Stock]([SucursalId] ASC, [ProductoId] ASC)
    WHERE [Estatus] = 1;
