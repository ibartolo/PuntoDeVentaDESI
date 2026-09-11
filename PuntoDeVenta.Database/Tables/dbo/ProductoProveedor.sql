CREATE TABLE [dbo].[ProductoProveedor]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [EmpresaId] BIGINT NOT NULL,
    [ProductoId] BIGINT NOT NULL,
    [ProveedorId] BIGINT NOT NULL,
    [Costo] DECIMAL (18, 4) NULL,
    [Estatus] BIT CONSTRAINT [DF_ProductoProveedor_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_ProductoProveedor_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_ProductoProveedor] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ProductoProveedor_Empresa] FOREIGN KEY ([EmpresaId]) REFERENCES [dbo].[Empresa] ([Id]),
    CONSTRAINT [FK_ProductoProveedor_Producto] FOREIGN KEY ([ProductoId]) REFERENCES [dbo].[Producto] ([Id]),
    CONSTRAINT [FK_ProductoProveedor_Proveedor] FOREIGN KEY ([ProveedorId]) REFERENCES [dbo].[Proveedor] ([Id])
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_ProductoProveedor_Producto_Proveedor_Activo]
    ON [dbo].[ProductoProveedor]([ProductoId] ASC, [ProveedorId] ASC)
    WHERE [Estatus] = 1;
