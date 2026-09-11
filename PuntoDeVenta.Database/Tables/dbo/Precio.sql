CREATE TABLE [dbo].[Precio]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [EmpresaId] BIGINT NOT NULL,
    [ProductoId] BIGINT NOT NULL,
    [PrecioCompra] DECIMAL (18, 4) CONSTRAINT [DF_Precio_PrecioCompra] DEFAULT ((0)) NOT NULL,
    [PrecioVentaSugerido] DECIMAL (18, 4) CONSTRAINT [DF_Precio_PrecioVentaSugerido] DEFAULT ((0)) NOT NULL,
    [PrecioVenta] DECIMAL (18, 4) CONSTRAINT [DF_Precio_PrecioVenta] DEFAULT ((0)) NOT NULL,
    [Activo] BIT CONSTRAINT [DF_Precio_Activo] DEFAULT ((1)) NOT NULL,
    [FechaInicio] DATETIME CONSTRAINT [DF_Precio_FechaInicio] DEFAULT (GETDATE()) NOT NULL,
    [FechaFin] DATETIME NULL,
    [Estatus] BIT CONSTRAINT [DF_Precio_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_Precio_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_Precio] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Precio_Empresa] FOREIGN KEY ([EmpresaId]) REFERENCES [dbo].[Empresa] ([Id]),
    CONSTRAINT [FK_Precio_Producto] FOREIGN KEY ([ProductoId]) REFERENCES [dbo].[Producto] ([Id])
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_Precio_Producto_Activo]
    ON [dbo].[Precio]([ProductoId] ASC)
    WHERE [Activo] = 1 AND [Estatus] = 1;
