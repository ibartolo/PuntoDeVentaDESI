CREATE TABLE [dbo].[Compra]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [EmpresaId] BIGINT NOT NULL,
    [SucursalId] BIGINT NOT NULL,
    [ProveedorId] BIGINT NOT NULL,
    [Folio] NVARCHAR (50) NULL,
    [FechaCompra] DATETIME CONSTRAINT [DF_Compra_FechaCompra] DEFAULT (GETDATE()) NOT NULL,
    [Subtotal] DECIMAL (18, 4) CONSTRAINT [DF_Compra_Subtotal] DEFAULT ((0)) NOT NULL,
    [Impuesto] DECIMAL (18, 4) CONSTRAINT [DF_Compra_Impuesto] DEFAULT ((0)) NOT NULL,
    [Total] DECIMAL (18, 4) CONSTRAINT [DF_Compra_Total] DEFAULT ((0)) NOT NULL,
    [Observaciones] NVARCHAR (MAX) NULL,
    [Estatus] BIT CONSTRAINT [DF_Compra_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_Compra_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_Compra] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Compra_Empresa] FOREIGN KEY ([EmpresaId]) REFERENCES [dbo].[Empresa] ([Id]),
    CONSTRAINT [FK_Compra_Sucursal] FOREIGN KEY ([SucursalId]) REFERENCES [dbo].[Sucursal] ([Id]),
    CONSTRAINT [FK_Compra_Proveedor] FOREIGN KEY ([ProveedorId]) REFERENCES [dbo].[Proveedor] ([Id])
);
