CREATE TABLE [dbo].[Producto]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [EmpresaId] BIGINT NOT NULL,
    [CategoriaId] BIGINT NULL,
    [MarcaId] BIGINT NULL,
    [MarcaTexto] NVARCHAR (100) NULL,
    [Nombre] NVARCHAR (150) NOT NULL,
    [Descripcion] NVARCHAR (MAX) NULL,
    [FotoUrl] NVARCHAR (300) NULL,
    [Codigo] NVARCHAR (100) NULL,
    [TipoProducto] NVARCHAR (20) CONSTRAINT [DF_Producto_TipoProducto] DEFAULT (N'Comprado') NOT NULL,
    [UnidadMedida] NVARCHAR (30) NULL,
    [StockMinimo] DECIMAL (18, 4) CONSTRAINT [DF_Producto_StockMinimo] DEFAULT ((0)) NOT NULL,
    [Estatus] BIT CONSTRAINT [DF_Producto_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_Producto_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_Producto] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Producto_Empresa] FOREIGN KEY ([EmpresaId]) REFERENCES [dbo].[Empresa] ([Id]),
    CONSTRAINT [FK_Producto_Categoria] FOREIGN KEY ([CategoriaId]) REFERENCES [dbo].[Categoria] ([Id]),
    CONSTRAINT [FK_Producto_Marca] FOREIGN KEY ([MarcaId]) REFERENCES [dbo].[Marca] ([Id]),
    CONSTRAINT [CK_Producto_Nombre_NotEmpty] CHECK (LEN(LTRIM(RTRIM([Nombre]))) > 0),
    CONSTRAINT [CK_Producto_TipoProducto] CHECK ([TipoProducto] IN (N'Comprado', N'Fabricado'))
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_Producto_EmpresaId_Codigo_Activo]
    ON [dbo].[Producto]([EmpresaId] ASC, [Codigo] ASC)
    WHERE [Estatus] = 1 AND [Codigo] IS NOT NULL;
