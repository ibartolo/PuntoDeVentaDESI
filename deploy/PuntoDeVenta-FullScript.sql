/* ============================================================
   PuntoDeVentaDESI - Script completo de base de datos
   Base de datos: db_9c7990_puntoventadev
   Ejecutar TODO el script en SSMS (conexion a esa base).
   ============================================================ */
USE [db_9c7990_puntoventadev];
GO
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

/* ===================== TABLAS ===================== */
GO
PRINT 'Creando tabla Empresa';
GO
CREATE TABLE [dbo].[Empresa]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [NombreComercial] NVARCHAR (250) NOT NULL,
    [RazonSocial] NVARCHAR (250) NOT NULL,
    [RFC] NVARCHAR (50) NOT NULL,
    [Responsable] NVARCHAR (250) NOT NULL,
    [Direccion] NVARCHAR (500) NOT NULL,
    [Ciudad] NVARCHAR (100) NULL,
    [Estado] NVARCHAR (100) NULL,
    [CodigoPostal] NVARCHAR (10) NULL,
    [Telefono] NVARCHAR (50) NULL,
    [CorreoContacto] NVARCHAR (250) NOT NULL,
    [FechaVigenciaInicio] DATETIME NOT NULL,
    [FechaVigenciaFin] DATETIME NOT NULL,
    [EsPeriodoPrueba] BIT CONSTRAINT [DF_Empresa_EsPeriodoPrueba] DEFAULT ((1)) NOT NULL,
    [Estatus] BIT CONSTRAINT [DF_Empresa_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    [LogoUrl] NVARCHAR (500) NULL,
    CONSTRAINT [PK_Empresa] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
PRINT 'Creando tabla Usuario';
GO
CREATE TABLE [dbo].[Usuario]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [EmpresaId] BIGINT NOT NULL,
    [NombreUsuario] NVARCHAR (25) NOT NULL,
    [ContrasenaHash] NVARCHAR (256) NOT NULL,
    [ContrasenaSalt] NVARCHAR (256) NOT NULL,
    [ContrasenaIteraciones] INT NOT NULL,
    [Correo] NVARCHAR (250) NULL,
    [Telefono] NVARCHAR (50) NULL,
    [ImagenPerfil] NVARCHAR (500) NULL,
    [Estatus] BIT CONSTRAINT [DF_Usuario_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_Usuario_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_Usuario] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Usuario_Empresa] FOREIGN KEY ([EmpresaId]) REFERENCES [dbo].[Empresa] ([Id]),
    CONSTRAINT [UQ_Usuario_NombreUsuario] UNIQUE NONCLUSTERED ([NombreUsuario] ASC),
    CONSTRAINT [CK_Usuario_NombreUsuario_NotEmpty] CHECK (LEN(LTRIM(RTRIM([NombreUsuario]))) > 0),
    CONSTRAINT [CK_Usuario_ContrasenaIteraciones_Min] CHECK ([ContrasenaIteraciones] >= 100000)
);

GO
PRINT 'Creando tabla Sucursal';
GO
CREATE TABLE [dbo].[Sucursal]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [EmpresaId] BIGINT NOT NULL,
    [Nombre] NVARCHAR (100) NOT NULL,
    [Descripcion] NVARCHAR (MAX) NULL,
    [Calle] NVARCHAR (200) NULL,
    [Ciudad] NVARCHAR (100) NULL,
    [Colonia] NVARCHAR (100) NULL,
    [CodigoPostal] NVARCHAR (10) NULL,
    [Estatus] BIT CONSTRAINT [DF_Sucursal_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_Sucursal_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_Sucursal] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Sucursal_Empresa] FOREIGN KEY ([EmpresaId]) REFERENCES [dbo].[Empresa] ([Id]),
    CONSTRAINT [CK_Sucursal_Nombre_NotEmpty] CHECK (LEN(LTRIM(RTRIM([Nombre]))) > 0),
    CONSTRAINT [CK_Sucursal_Nombre_MaxLen] CHECK (LEN([Nombre]) <= 100)
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_Sucursal_EmpresaId_Nombre_Activa]
    ON [dbo].[Sucursal]([EmpresaId] ASC, [Nombre] ASC)
    WHERE [Estatus] = 1;

GO
PRINT 'Creando tabla UsuarioSucursal';
GO
CREATE TABLE [dbo].[UsuarioSucursal]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [EmpresaId] BIGINT NOT NULL,
    [UsuarioId] BIGINT NOT NULL,
    [SucursalId] BIGINT NOT NULL,
    [Estatus] BIT CONSTRAINT [DF_UsuarioSucursal_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_UsuarioSucursal_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_UsuarioSucursal] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UsuarioSucursal_Empresa] FOREIGN KEY ([EmpresaId]) REFERENCES [dbo].[Empresa] ([Id]),
    CONSTRAINT [FK_UsuarioSucursal_Usuario] FOREIGN KEY ([UsuarioId]) REFERENCES [dbo].[Usuario] ([Id]),
    CONSTRAINT [FK_UsuarioSucursal_Sucursal] FOREIGN KEY ([SucursalId]) REFERENCES [dbo].[Sucursal] ([Id])
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_UsuarioSucursal_Usuario_Sucursal_Activo]
    ON [dbo].[UsuarioSucursal]([UsuarioId] ASC, [SucursalId] ASC)
    WHERE [Estatus] = 1;

GO
PRINT 'Creando tabla Rol';
GO
CREATE TABLE [dbo].[Rol]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [EmpresaId] BIGINT NOT NULL,
    [Nombre] NVARCHAR (50) NOT NULL,
    [Descripcion] NVARCHAR (250) NULL,
    [PuedeAutorizar] BIT CONSTRAINT [DF_Rol_PuedeAutorizar] DEFAULT ((0)) NOT NULL,
    [Estatus] BIT CONSTRAINT [DF_Rol_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_Rol_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_Rol] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Rol_Empresa] FOREIGN KEY ([EmpresaId]) REFERENCES [dbo].[Empresa] ([Id]),
    CONSTRAINT [CK_Rol_Nombre_NotEmpty] CHECK (LEN(LTRIM(RTRIM([Nombre]))) > 0),
    CONSTRAINT [CK_Rol_Nombre_MaxLen] CHECK (LEN([Nombre]) <= 50)
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_Rol_EmpresaId_Nombre_Activo]
    ON [dbo].[Rol]([EmpresaId] ASC, [Nombre] ASC)
    WHERE [Estatus] = 1;

GO
PRINT 'Creando tabla Pagina';
GO
CREATE TABLE [dbo].[Pagina]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [Nombre] NVARCHAR (100) NOT NULL,
    [NombreVisible] NVARCHAR (150) NULL,
    [Descripcion] NVARCHAR (250) NULL,
    [Tipo] NVARCHAR (20) CONSTRAINT [DF_Pagina_Tipo] DEFAULT (N'Menu') NOT NULL,
    [Direccion] NVARCHAR (250) NULL,
    [PermisosPadreId] BIGINT NULL,
    [Logo] NVARCHAR (100) NULL,
    [OrdenB] INT CONSTRAINT [DF_Pagina_OrdenB] DEFAULT ((0)) NOT NULL,
    [Estatus] BIT CONSTRAINT [DF_Pagina_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_Pagina_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_Pagina] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Pagina_Pagina_Padre] FOREIGN KEY ([PermisosPadreId]) REFERENCES [dbo].[Pagina] ([Id]),
    CONSTRAINT [CK_Pagina_Nombre_NotEmpty] CHECK (LEN(LTRIM(RTRIM([Nombre]))) > 0),
    CONSTRAINT [CK_Pagina_Nombre_MaxLen] CHECK (LEN([Nombre]) <= 100)
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_Pagina_Nombre_Activa]
    ON [dbo].[Pagina]([Nombre] ASC)
    WHERE [Estatus] = 1;

GO
PRINT 'Creando tabla UsuarioRol';
GO
CREATE TABLE [dbo].[UsuarioRol]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [UsuarioId] BIGINT NOT NULL,
    [RolId] BIGINT NOT NULL,
    [Estatus] BIT CONSTRAINT [DF_UsuarioRol_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_UsuarioRol_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_UsuarioRol] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UsuarioRol_Usuario] FOREIGN KEY ([UsuarioId]) REFERENCES [dbo].[Usuario] ([Id]),
    CONSTRAINT [FK_UsuarioRol_Rol] FOREIGN KEY ([RolId]) REFERENCES [dbo].[Rol] ([Id])
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_UsuarioRol_UsuarioId_RolId_Activo]
    ON [dbo].[UsuarioRol]([UsuarioId] ASC, [RolId] ASC)
    WHERE [Estatus] = 1;

GO
PRINT 'Creando tabla RolPaginaAccion';
GO
CREATE TABLE [dbo].[RolPaginaAccion]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [RolId] BIGINT NOT NULL,
    [PaginaId] BIGINT NOT NULL,
    [PuedeLeer] BIT CONSTRAINT [DF_RolPaginaAccion_PuedeLeer] DEFAULT ((0)) NOT NULL,
    [PuedeCrear] BIT CONSTRAINT [DF_RolPaginaAccion_PuedeCrear] DEFAULT ((0)) NOT NULL,
    [PuedeEditar] BIT CONSTRAINT [DF_RolPaginaAccion_PuedeEditar] DEFAULT ((0)) NOT NULL,
    [PuedeEliminar] BIT CONSTRAINT [DF_RolPaginaAccion_PuedeEliminar] DEFAULT ((0)) NOT NULL,
    [PuedeExportar] BIT CONSTRAINT [DF_RolPaginaAccion_PuedeExportar] DEFAULT ((0)) NOT NULL,
    [Estatus] BIT CONSTRAINT [DF_RolPaginaAccion_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_RolPaginaAccion_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_RolPaginaAccion] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_RolPaginaAccion_Rol] FOREIGN KEY ([RolId]) REFERENCES [dbo].[Rol] ([Id]),
    CONSTRAINT [FK_RolPaginaAccion_Pagina] FOREIGN KEY ([PaginaId]) REFERENCES [dbo].[Pagina] ([Id])
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_RolPaginaAccion_RolId_PaginaId_Activo]
    ON [dbo].[RolPaginaAccion]([RolId] ASC, [PaginaId] ASC)
    WHERE [Estatus] = 1;

GO
PRINT 'Creando tabla UsuarioPagina';
GO
CREATE TABLE [dbo].[UsuarioPagina]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [UsuarioId] BIGINT NULL,
    [PaginaId] BIGINT NULL,
    [Estatus] BIT CONSTRAINT [DF_UsuarioPagina_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_UsuarioPagina_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_UsuarioPagina] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UsuarioPagina_Usuario] FOREIGN KEY ([UsuarioId]) REFERENCES [dbo].[Usuario] ([Id]),
    CONSTRAINT [FK_UsuarioPagina_Pagina] FOREIGN KEY ([PaginaId]) REFERENCES [dbo].[Pagina] ([Id])
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_UsuarioPagina_UsuarioId_PaginaId_Activo]
    ON [dbo].[UsuarioPagina]([UsuarioId] ASC, [PaginaId] ASC)
    WHERE [Estatus] = 1;

GO
PRINT 'Creando tabla TokenRecuperacion';
GO
CREATE TABLE [dbo].[TokenRecuperacion]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [UsuarioId] BIGINT NOT NULL,
    [Token] NVARCHAR (200) NOT NULL,
    [FechaExpiracion] DATETIME NOT NULL,
    [Usado] BIT CONSTRAINT [DF_TokenRecuperacion_Usado] DEFAULT ((0)) NOT NULL,
    [Estatus] BIT CONSTRAINT [DF_TokenRecuperacion_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_TokenRecuperacion_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_TokenRecuperacion] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TokenRecuperacion_Usuario] FOREIGN KEY ([UsuarioId]) REFERENCES [dbo].[Usuario] ([Id]),
    CONSTRAINT [CK_TokenRecuperacion_Token_NotEmpty] CHECK (LEN(LTRIM(RTRIM([Token]))) > 0)
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_TokenRecuperacion_Token]
    ON [dbo].[TokenRecuperacion]([Token] ASC);

GO
PRINT 'Creando tabla Marca';
GO
CREATE TABLE [dbo].[Marca]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [EmpresaId] BIGINT NOT NULL,
    [Nombre] NVARCHAR (100) NOT NULL,
    [Descripcion] NVARCHAR (MAX) NULL,
    [Estatus] BIT CONSTRAINT [DF_Marca_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_Marca_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_Marca] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Marca_Empresa] FOREIGN KEY ([EmpresaId]) REFERENCES [dbo].[Empresa] ([Id]),
    CONSTRAINT [CK_Marca_Nombre_NotEmpty] CHECK (LEN(LTRIM(RTRIM([Nombre]))) > 0),
    CONSTRAINT [CK_Marca_Nombre_MaxLen] CHECK (LEN([Nombre]) <= 100)
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_Marca_EmpresaId_Nombre_Activa]
    ON [dbo].[Marca]([EmpresaId] ASC, [Nombre] ASC)
    WHERE [Estatus] = 1;

GO
PRINT 'Creando tabla Categoria';
GO
CREATE TABLE [dbo].[Categoria]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [EmpresaId] BIGINT NOT NULL,
    [CategoriaPadreId] BIGINT NULL,
    [Nombre] NVARCHAR (100) NOT NULL,
    [Descripcion] NVARCHAR (MAX) NULL,
    [Area] NVARCHAR (100) NULL,
    [Orden] INT CONSTRAINT [DF_Categoria_Orden] DEFAULT ((0)) NOT NULL,
    [Estatus] BIT CONSTRAINT [DF_Categoria_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_Categoria_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_Categoria] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Categoria_Empresa] FOREIGN KEY ([EmpresaId]) REFERENCES [dbo].[Empresa] ([Id]),
    CONSTRAINT [FK_Categoria_CategoriaPadre] FOREIGN KEY ([CategoriaPadreId]) REFERENCES [dbo].[Categoria] ([Id]),
    CONSTRAINT [CK_Categoria_Nombre_NotEmpty] CHECK (LEN(LTRIM(RTRIM([Nombre]))) > 0),
    CONSTRAINT [CK_Categoria_Nombre_MaxLen] CHECK (LEN([Nombre]) <= 100)
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_Categoria_EmpresaId_Nombre_Activa]
    ON [dbo].[Categoria]([EmpresaId] ASC, [Nombre] ASC)
    WHERE [Estatus] = 1;

GO
CREATE NONCLUSTERED INDEX [IX_Categoria_EmpresaId_CategoriaPadreId]
    ON [dbo].[Categoria]([EmpresaId] ASC, [CategoriaPadreId] ASC);

GO
PRINT 'Creando tabla Producto';
GO
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

GO
PRINT 'Creando tabla Precio';
GO
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

GO
PRINT 'Creando tabla Proveedor';
GO
CREATE TABLE [dbo].[Proveedor]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [EmpresaId] BIGINT NOT NULL,
    [Nombre] NVARCHAR (150) NOT NULL,
    [Contacto] NVARCHAR (150) NULL,
    [Correo] NVARCHAR (250) NULL,
    [Telefono] NVARCHAR (50) NULL,
    [Direccion] NVARCHAR (500) NULL,
    [Estatus] BIT CONSTRAINT [DF_Proveedor_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_Proveedor_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_Proveedor] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Proveedor_Empresa] FOREIGN KEY ([EmpresaId]) REFERENCES [dbo].[Empresa] ([Id]),
    CONSTRAINT [CK_Proveedor_Nombre_NotEmpty] CHECK (LEN(LTRIM(RTRIM([Nombre]))) > 0),
    CONSTRAINT [CK_Proveedor_Nombre_MaxLen] CHECK (LEN([Nombre]) <= 150)
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_Proveedor_EmpresaId_Nombre_Activa]
    ON [dbo].[Proveedor]([EmpresaId] ASC, [Nombre] ASC)
    WHERE [Estatus] = 1;

GO
PRINT 'Creando tabla ProductoProveedor';
GO
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

GO
PRINT 'Creando tabla Cliente';
GO
CREATE TABLE [dbo].[Cliente]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [EmpresaId] BIGINT NOT NULL,
    [Nombre] NVARCHAR (150) NOT NULL,
    [Correo] NVARCHAR (250) NULL,
    [Telefono] NVARCHAR (50) NULL,
    [Direccion] NVARCHAR (500) NULL,
    [EsPublicoGeneral] BIT CONSTRAINT [DF_Cliente_EsPublicoGeneral] DEFAULT ((0)) NOT NULL,
    [Estatus] BIT CONSTRAINT [DF_Cliente_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_Cliente_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_Cliente] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Cliente_Empresa] FOREIGN KEY ([EmpresaId]) REFERENCES [dbo].[Empresa] ([Id]),
    CONSTRAINT [CK_Cliente_Nombre_NotEmpty] CHECK (LEN(LTRIM(RTRIM([Nombre]))) > 0),
    CONSTRAINT [CK_Cliente_Nombre_MaxLen] CHECK (LEN([Nombre]) <= 150)
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_Cliente_EmpresaId_Nombre_Activa]
    ON [dbo].[Cliente]([EmpresaId] ASC, [Nombre] ASC)
    WHERE [Estatus] = 1;

GO
PRINT 'Creando tabla Compra';
GO
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

GO
PRINT 'Creando tabla CompraDetalle';
GO
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

GO
PRINT 'Creando tabla Stock';
GO
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

GO
PRINT 'Creando tabla StockMovimiento';
GO
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

GO
PRINT 'Creando tabla CajaChica';
GO
CREATE TABLE [dbo].[CajaChica]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [EmpresaId] BIGINT NOT NULL,
    [SucursalId] BIGINT NOT NULL,
    [UsuarioId] BIGINT NOT NULL,
    [MontoInicial] DECIMAL (18, 4) CONSTRAINT [DF_CajaChica_MontoInicial] DEFAULT ((0)) NOT NULL,
    [IngresosTotales] DECIMAL (18, 4) CONSTRAINT [DF_CajaChica_IngresosTotales] DEFAULT ((0)) NOT NULL,
    [SalidasTotales] DECIMAL (18, 4) CONSTRAINT [DF_CajaChica_SalidasTotales] DEFAULT ((0)) NOT NULL,
    [FechaApertura] DATETIME CONSTRAINT [DF_CajaChica_FechaApertura] DEFAULT (GETDATE()) NOT NULL,
    [FechaCierre] DATETIME NULL,
    [Estado] NVARCHAR (15) CONSTRAINT [DF_CajaChica_Estado] DEFAULT (N'Abierta') NOT NULL,
    [Estatus] BIT CONSTRAINT [DF_CajaChica_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_CajaChica_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_CajaChica] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CajaChica_Empresa] FOREIGN KEY ([EmpresaId]) REFERENCES [dbo].[Empresa] ([Id]),
    CONSTRAINT [FK_CajaChica_Sucursal] FOREIGN KEY ([SucursalId]) REFERENCES [dbo].[Sucursal] ([Id]),
    CONSTRAINT [FK_CajaChica_Usuario] FOREIGN KEY ([UsuarioId]) REFERENCES [dbo].[Usuario] ([Id]),
    CONSTRAINT [CK_CajaChica_Estado] CHECK ([Estado] IN (N'Abierta', N'Cerrada'))
);

GO
PRINT 'Creando tabla SalidaCaja';
GO
CREATE TABLE [dbo].[SalidaCaja]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [EmpresaId] BIGINT NOT NULL,
    [CajaChicaId] BIGINT NOT NULL,
    [Monto] DECIMAL (18, 4) NOT NULL,
    [Comentario] NVARCHAR (300) NULL,
    [FechaHora] DATETIME CONSTRAINT [DF_SalidaCaja_FechaHora] DEFAULT (GETDATE()) NOT NULL,
    [Justificada] BIT CONSTRAINT [DF_SalidaCaja_Justificada] DEFAULT ((0)) NOT NULL,
    [EvidenciaUrl] NVARCHAR (300) NULL,
    [TipoSalida] NVARCHAR (30) NULL,
    [Estatus] BIT CONSTRAINT [DF_SalidaCaja_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_SalidaCaja_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_SalidaCaja] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SalidaCaja_Empresa] FOREIGN KEY ([EmpresaId]) REFERENCES [dbo].[Empresa] ([Id]),
    CONSTRAINT [FK_SalidaCaja_CajaChica] FOREIGN KEY ([CajaChicaId]) REFERENCES [dbo].[CajaChica] ([Id])
);

GO
PRINT 'Creando tabla Corte';
GO
CREATE TABLE [dbo].[Corte]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [EmpresaId] BIGINT NOT NULL,
    [SucursalId] BIGINT NOT NULL,
    [UsuarioId] BIGINT NOT NULL,
    [CajaChicaId] BIGINT NOT NULL,
    [FechaCorte] DATETIME CONSTRAINT [DF_Corte_FechaCorte] DEFAULT (GETDATE()) NOT NULL,
    [MontoInicial] DECIMAL (18, 4) CONSTRAINT [DF_Corte_MontoInicial] DEFAULT ((0)) NOT NULL,
    [TotalVentas] DECIMAL (18, 4) CONSTRAINT [DF_Corte_TotalVentas] DEFAULT ((0)) NOT NULL,
    [TotalEfectivo] DECIMAL (18, 4) CONSTRAINT [DF_Corte_TotalEfectivo] DEFAULT ((0)) NOT NULL,
    [TotalTarjeta] DECIMAL (18, 4) CONSTRAINT [DF_Corte_TotalTarjeta] DEFAULT ((0)) NOT NULL,
    [TotalTransferencia] DECIMAL (18, 4) CONSTRAINT [DF_Corte_TotalTransferencia] DEFAULT ((0)) NOT NULL,
    [Salidas] DECIMAL (18, 4) CONSTRAINT [DF_Corte_Salidas] DEFAULT ((0)) NOT NULL,
    [EfectivoEsperado] DECIMAL (18, 4) CONSTRAINT [DF_Corte_EfectivoEsperado] DEFAULT ((0)) NOT NULL,
    [EfectivoContado] DECIMAL (18, 4) CONSTRAINT [DF_Corte_EfectivoContado] DEFAULT ((0)) NOT NULL,
    [Diferencia] DECIMAL (18, 4) CONSTRAINT [DF_Corte_Diferencia] DEFAULT ((0)) NOT NULL,
    [Observaciones] NVARCHAR (MAX) NULL,
    [Estatus] BIT CONSTRAINT [DF_Corte_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_Corte_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_Corte] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Corte_Empresa] FOREIGN KEY ([EmpresaId]) REFERENCES [dbo].[Empresa] ([Id]),
    CONSTRAINT [FK_Corte_Sucursal] FOREIGN KEY ([SucursalId]) REFERENCES [dbo].[Sucursal] ([Id]),
    CONSTRAINT [FK_Corte_Usuario] FOREIGN KEY ([UsuarioId]) REFERENCES [dbo].[Usuario] ([Id]),
    CONSTRAINT [FK_Corte_CajaChica] FOREIGN KEY ([CajaChicaId]) REFERENCES [dbo].[CajaChica] ([Id])
);

GO
PRINT 'Creando tabla CorteDetalle';
GO
CREATE TABLE [dbo].[CorteDetalle]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [CorteId] BIGINT NOT NULL,
    [MetodoPago] NVARCHAR (20) NOT NULL,
    [Monto] DECIMAL (18, 4) CONSTRAINT [DF_CorteDetalle_Monto] DEFAULT ((0)) NOT NULL,
    [FolioCobro] NVARCHAR (100) NULL,
    [TicketCobroUrl] NVARCHAR (300) NULL,
    [TicketSistemaUrl] NVARCHAR (300) NULL,
    [ComprobanteUrl] NVARCHAR (300) NULL,
    [Estatus] BIT CONSTRAINT [DF_CorteDetalle_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_CorteDetalle_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_CorteDetalle] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CorteDetalle_Corte] FOREIGN KEY ([CorteId]) REFERENCES [dbo].[Corte] ([Id]),
    CONSTRAINT [CK_CorteDetalle_MetodoPago] CHECK ([MetodoPago] IN (N'Efectivo', N'Tarjeta', N'Transferencia'))
);

GO
PRINT 'Creando tabla Venta';
GO
CREATE TABLE [dbo].[Venta]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [EmpresaId] BIGINT NOT NULL,
    [SucursalId] BIGINT NOT NULL,
    [UsuarioId] BIGINT NOT NULL,
    [ClienteId] BIGINT NULL,
    [CajaChicaId] BIGINT NULL,
    [Folio] NVARCHAR (50) NULL,
    [FechaVenta] DATETIME CONSTRAINT [DF_Venta_FechaVenta] DEFAULT (GETDATE()) NOT NULL,
    [MetodoPago] NVARCHAR (20) NOT NULL,
    [Subtotal] DECIMAL (18, 4) CONSTRAINT [DF_Venta_Subtotal] DEFAULT ((0)) NOT NULL,
    [Impuesto] DECIMAL (18, 4) CONSTRAINT [DF_Venta_Impuesto] DEFAULT ((0)) NOT NULL,
    [Total] DECIMAL (18, 4) CONSTRAINT [DF_Venta_Total] DEFAULT ((0)) NOT NULL,
    [Estatus] BIT CONSTRAINT [DF_Venta_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_Venta_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_Venta] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Venta_Empresa] FOREIGN KEY ([EmpresaId]) REFERENCES [dbo].[Empresa] ([Id]),
    CONSTRAINT [FK_Venta_Sucursal] FOREIGN KEY ([SucursalId]) REFERENCES [dbo].[Sucursal] ([Id]),
    CONSTRAINT [FK_Venta_Usuario] FOREIGN KEY ([UsuarioId]) REFERENCES [dbo].[Usuario] ([Id]),
    CONSTRAINT [FK_Venta_Cliente] FOREIGN KEY ([ClienteId]) REFERENCES [dbo].[Cliente] ([Id]),
    CONSTRAINT [FK_Venta_CajaChica] FOREIGN KEY ([CajaChicaId]) REFERENCES [dbo].[CajaChica] ([Id]),
    CONSTRAINT [CK_Venta_MetodoPago] CHECK ([MetodoPago] IN (N'Efectivo', N'Tarjeta', N'Transferencia'))
);

GO
PRINT 'Creando tabla VentaDetalle';
GO
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

GO
PRINT 'Creando tabla Cancelacion';
GO
CREATE TABLE [dbo].[Cancelacion]
(
    [Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [EmpresaId] BIGINT NOT NULL,
    [SucursalId] BIGINT NOT NULL,
    [VentaId] BIGINT NOT NULL,
    [Tipo] NVARCHAR (10) NOT NULL,
    [Motivo] NVARCHAR (300) NULL,
    [UsuarioCancela] NVARCHAR (25) NULL,
    [UsuarioAutoriza] NVARCHAR (25) NULL,
    [FechaCancelacion] DATETIME CONSTRAINT [DF_Cancelacion_FechaCancelacion] DEFAULT (GETDATE()) NOT NULL,
    [TotalReembolsado] DECIMAL (18, 4) CONSTRAINT [DF_Cancelacion_TotalReembolsado] DEFAULT ((0)) NOT NULL,
    [MetodoReembolso] NVARCHAR (20) NULL,
    [Estatus] BIT CONSTRAINT [DF_Cancelacion_Estatus] DEFAULT ((1)) NOT NULL,
    [CreadoPor] NVARCHAR (25) NOT NULL,
    [FechaCreacion] DATETIME CONSTRAINT [DF_Cancelacion_FechaCreacion] DEFAULT (GETDATE()) NOT NULL,
    [ModificadoPor] NVARCHAR (25) NULL,
    [FechaModificacion] DATETIME NULL,
    CONSTRAINT [PK_Cancelacion] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Cancelacion_Empresa] FOREIGN KEY ([EmpresaId]) REFERENCES [dbo].[Empresa] ([Id]),
    CONSTRAINT [FK_Cancelacion_Sucursal] FOREIGN KEY ([SucursalId]) REFERENCES [dbo].[Sucursal] ([Id]),
    CONSTRAINT [FK_Cancelacion_Venta] FOREIGN KEY ([VentaId]) REFERENCES [dbo].[Venta] ([Id]),
    CONSTRAINT [CK_Cancelacion_Tipo] CHECK ([Tipo] IN (N'Total', N'Parcial'))
);

GO
PRINT 'Creando tabla DevolucionDetalle';
GO
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

GO
/* ================= STORED PROCEDURES ================= */
GO
PRINT 'Creando SP sp_CajaChica_Abrir';
GO
CREATE PROCEDURE [dbo].[sp_CajaChica_Abrir]
    @EmpresaId BIGINT,
    @SucursalId BIGINT,
    @UsuarioId BIGINT,
    @MontoInicial DECIMAL(18,4),
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    -- Sin corte previo no se puede abrir una nueva caja chica.
    IF EXISTS
    (
        SELECT 1 FROM [dbo].[CajaChica]
        WHERE [EmpresaId] = @EmpresaId
          AND [SucursalId] = @SucursalId
          AND [UsuarioId] = @UsuarioId
          AND [Estado] = N'Abierta'
          AND [Estatus] = 1
    )
    BEGIN
        RAISERROR('Ya existe una caja chica abierta para este usuario y sucursal. Realice el corte antes de abrir otra.', 16, 1);
        RETURN;
    END;

    INSERT INTO [dbo].[CajaChica]
    ([EmpresaId], [SucursalId], [UsuarioId], [MontoInicial], [IngresosTotales], [SalidasTotales],
     [FechaApertura], [FechaCierre], [Estado], [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
    VALUES
    (@EmpresaId, @SucursalId, @UsuarioId, @MontoInicial, 0, 0,
     GETDATE(), NULL, N'Abierta', 1, @Actor, GETDATE(), NULL, NULL);

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS [Id];
END;

GO
PRINT 'Creando SP sp_CajaChica_Cerrar';
GO
CREATE PROCEDURE [dbo].[sp_CajaChica_Cerrar]
    @EmpresaId BIGINT,
    @CajaChicaId BIGINT,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[CajaChica]
    SET [Estado] = N'Cerrada',
        [FechaCierre] = GETDATE(),
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [EmpresaId] = @EmpresaId
      AND [Id] = @CajaChicaId
      AND [Estado] = N'Abierta'
      AND [Estatus] = 1;

    SELECT @@ROWCOUNT AS [AffectedRows];
END;

GO
PRINT 'Creando SP sp_CajaChica_ObtenerAbierta';
GO
CREATE PROCEDURE [dbo].[sp_CajaChica_ObtenerAbierta]
    @EmpresaId BIGINT,
    @SucursalId BIGINT,
    @UsuarioId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        C.[Id], C.[EmpresaId], C.[SucursalId], S.[Nombre] AS [SucursalNombre],
        C.[UsuarioId], U.[NombreUsuario] AS [UsuarioNombre], C.[MontoInicial], C.[IngresosTotales],
        C.[SalidasTotales], C.[FechaApertura], C.[FechaCierre], C.[Estado],
        C.[Estatus], C.[CreadoPor], C.[FechaCreacion], C.[ModificadoPor], C.[FechaModificacion]
    FROM [dbo].[CajaChica] C
    INNER JOIN [dbo].[Sucursal] S ON S.[Id] = C.[SucursalId]
    INNER JOIN [dbo].[Usuario] U ON U.[Id] = C.[UsuarioId]
    WHERE C.[EmpresaId] = @EmpresaId
      AND C.[SucursalId] = @SucursalId
      AND C.[UsuarioId] = @UsuarioId
      AND C.[Estado] = N'Abierta'
      AND C.[Estatus] = 1
    ORDER BY C.[FechaApertura] DESC;
END;

GO
PRINT 'Creando SP sp_Cancelacion_Guardar';
GO
CREATE PROCEDURE [dbo].[sp_Cancelacion_Guardar]
    @EmpresaId BIGINT,
    @SucursalId BIGINT,
    @VentaId BIGINT,
    @Tipo NVARCHAR(10),
    @Motivo NVARCHAR(300) = NULL,
    @UsuarioCancela NVARCHAR(25) = NULL,
    @UsuarioAutoriza NVARCHAR(25) = NULL,
    @TotalReembolsado DECIMAL(18,4) = 0,
    @MetodoReembolso NVARCHAR(20) = NULL,
    @DetalleJson NVARCHAR(MAX) = NULL,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @Tipo NOT IN (N'Total', N'Parcial')
    BEGIN
        RAISERROR('Tipo de cancelacion no valido.', 16, 1);
        RETURN;
    END;

    -- SYNC-12: la venta debe existir y pertenecer a la empresa (tenant) autenticada.
    IF NOT EXISTS
    (
        SELECT 1 FROM [dbo].[Venta]
        WHERE [Id] = @VentaId AND [EmpresaId] = @EmpresaId AND [Estatus] = 1
    )
    BEGIN
        RAISERROR('Venta no valida para la empresa.', 16, 1);
        RETURN;
    END;

    -- SYNC-11: la sucursal debe pertenecer a la empresa (tenant) autenticada.
    IF NOT EXISTS
    (
        SELECT 1 FROM [dbo].[Sucursal]
        WHERE [Id] = @SucursalId AND [EmpresaId] = @EmpresaId AND [Estatus] = 1
    )
    BEGIN
        RAISERROR('Sucursal no valida para la empresa.', 16, 1);
        RETURN;
    END;

    DECLARE @CancelacionId BIGINT;

    DECLARE @Detalle TABLE
    (
        VentaDetalleId BIGINT,
        ProductoId BIGINT,
        Cantidad DECIMAL(18,4),
        Importe DECIMAL(18,4),
        RegresaAStock BIT
    );

    IF @DetalleJson IS NOT NULL AND LEN(@DetalleJson) > 2
    BEGIN
        INSERT INTO @Detalle (VentaDetalleId, ProductoId, Cantidad, Importe, RegresaAStock)
        SELECT VentaDetalleId, ProductoId, Cantidad, Importe, RegresaAStock
        FROM OPENJSON(@DetalleJson)
        WITH
        (
            VentaDetalleId BIGINT '$.VentaDetalleId',
            ProductoId BIGINT '$.ProductoId',
            Cantidad DECIMAL(18,4) '$.Cantidad',
            Importe DECIMAL(18,4) '$.Importe',
            RegresaAStock BIT '$.RegresaAStock'
        );
    END

    -- SYNC-12: cantidades positivas.
    IF EXISTS (SELECT 1 FROM @Detalle WHERE [Cantidad] <= 0)
    BEGIN
        RAISERROR('La cantidad a devolver debe ser mayor a cero.', 16, 1);
        RETURN;
    END;

    -- SYNC-12: cada linea devuelta debe pertenecer a la venta indicada.
    IF EXISTS
    (
        SELECT 1
        FROM @Detalle D
        WHERE D.[VentaDetalleId] > 0
          AND NOT EXISTS
          (
              SELECT 1 FROM [dbo].[VentaDetalle] VD
              WHERE VD.[Id] = D.[VentaDetalleId] AND VD.[VentaId] = @VentaId
          )
    )
    BEGIN
        RAISERROR('El detalle de la cancelacion no pertenece a la venta indicada.', 16, 1);
        RETURN;
    END;

    -- SYNC-12: no permitir lineas de detalle duplicadas dentro de la misma cancelacion.
    IF EXISTS
    (
        SELECT D.[VentaDetalleId]
        FROM @Detalle D
        WHERE D.[VentaDetalleId] > 0
        GROUP BY D.[VentaDetalleId]
        HAVING COUNT(*) > 1
    )
    BEGIN
        RAISERROR('La cancelacion contiene lineas de detalle duplicadas.', 16, 1);
        RETURN;
    END;

    -- SYNC-12: la cantidad acumulada devuelta no puede exceder la vendida.
    IF EXISTS
    (
        SELECT 1
        FROM @Detalle D
        INNER JOIN [dbo].[VentaDetalle] VD ON VD.[Id] = D.[VentaDetalleId] AND VD.[VentaId] = @VentaId
        OUTER APPLY
        (
            SELECT SUM(DD.[Cantidad]) AS [Devuelto]
            FROM [dbo].[DevolucionDetalle] DD
            INNER JOIN [dbo].[Cancelacion] C ON C.[Id] = DD.[CancelacionId]
            WHERE DD.[VentaDetalleId] = D.[VentaDetalleId]
              AND DD.[Estatus] = 1
              AND C.[Estatus] = 1
              AND C.[EmpresaId] = @EmpresaId
        ) R
        WHERE D.[VentaDetalleId] > 0
          AND D.[Cantidad] > (VD.[Cantidad] - ISNULL(R.[Devuelto], 0))
    )
    BEGIN
        RAISERROR('La cantidad a devolver excede lo vendido o ya fue cancelada.', 16, 1);
        RETURN;
    END;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO [dbo].[Cancelacion]
        ([EmpresaId], [SucursalId], [VentaId], [Tipo], [Motivo], [UsuarioCancela], [UsuarioAutoriza],
         [FechaCancelacion], [TotalReembolsado], [MetodoReembolso],
         [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
        VALUES
        (@EmpresaId, @SucursalId, @VentaId, @Tipo, @Motivo, @UsuarioCancela, @UsuarioAutoriza,
         GETDATE(), @TotalReembolsado, @MetodoReembolso,
         1, @Actor, GETDATE(), NULL, NULL);

        SET @CancelacionId = CAST(SCOPE_IDENTITY() AS BIGINT);

        INSERT INTO [dbo].[DevolucionDetalle]
        ([CancelacionId], [VentaDetalleId], [ProductoId], [Cantidad], [Importe], [RegresaAStock],
         [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
        SELECT @CancelacionId, D.[VentaDetalleId], D.[ProductoId], D.[Cantidad], D.[Importe], D.[RegresaAStock],
               1, @Actor, GETDATE(), NULL, NULL
        FROM @Detalle D;

        -- Devolucion a stock solo de los productos marcados, dentro de la empresa (tenant).
        UPDATE S
        SET S.[Cantidad] = S.[Cantidad] + D.[Cantidad],
            S.[ModificadoPor] = @Actor,
            S.[FechaModificacion] = GETDATE()
        FROM [dbo].[Stock] S
        INNER JOIN @Detalle D ON D.[ProductoId] = S.[ProductoId]
        WHERE S.[SucursalId] = @SucursalId
          AND S.[EmpresaId] = @EmpresaId
          AND S.[Estatus] = 1
          AND D.[RegresaAStock] = 1;

        INSERT INTO [dbo].[StockMovimiento]
        ([EmpresaId], [SucursalId], [ProductoId], [TipoMovimiento], [Cantidad],
         [ExistenciaAnterior], [ExistenciaNueva], [Motivo], [ReferenciaId],
         [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
        SELECT @EmpresaId, @SucursalId, D.[ProductoId], N'Devolucion', D.[Cantidad],
               ISNULL(S.[Cantidad] - D.[Cantidad], 0), ISNULL(S.[Cantidad], D.[Cantidad]), N'Devolucion', @CancelacionId,
               1, @Actor, GETDATE(), NULL, NULL
        FROM @Detalle D
        LEFT JOIN [dbo].[Stock] S ON S.[ProductoId] = D.[ProductoId]
                                 AND S.[SucursalId] = @SucursalId
                                 AND S.[EmpresaId] = @EmpresaId
        WHERE D.[RegresaAStock] = 1;

        -- Reembolso en efectivo (justificado) se registra como salida de caja si aplica.
        IF @MetodoReembolso = N'Efectivo' AND @TotalReembolsado > 0
        BEGIN
            DECLARE @CajaChicaId BIGINT;

            SELECT TOP (1) @CajaChicaId = [Id]
            FROM [dbo].[CajaChica]
            WHERE [EmpresaId] = @EmpresaId AND [SucursalId] = @SucursalId AND [Estado] = N'Abierta' AND [Estatus] = 1
            ORDER BY [FechaApertura] DESC;

            IF @CajaChicaId IS NOT NULL
            BEGIN
                INSERT INTO [dbo].[SalidaCaja]
                ([EmpresaId], [CajaChicaId], [Monto], [Comentario], [FechaHora], [Justificada], [EvidenciaUrl], [TipoSalida],
                 [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
                VALUES
                (@EmpresaId, @CajaChicaId, @TotalReembolsado, N'Reembolso por cancelacion/devolucion', GETDATE(), 1, NULL, N'Reembolso',
                 1, @Actor, GETDATE(), NULL, NULL);

                UPDATE [dbo].[CajaChica]
                SET [SalidasTotales] = [SalidasTotales] + @TotalReembolsado,
                    [ModificadoPor] = @Actor,
                    [FechaModificacion] = GETDATE()
                WHERE [Id] = @CajaChicaId
                  AND [EmpresaId] = @EmpresaId;
            END
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH

    SELECT @CancelacionId AS [Id];
END;

GO
PRINT 'Creando SP sp_Cancelacion_Listar';
GO
CREATE PROCEDURE [dbo].[sp_Cancelacion_Listar]
    @EmpresaId BIGINT,
    @SucursalId BIGINT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        C.[Id], C.[EmpresaId], C.[SucursalId], C.[VentaId], V.[Folio] AS [VentaFolio],
        C.[Tipo], C.[Motivo], C.[UsuarioCancela], C.[UsuarioAutoriza], C.[FechaCancelacion],
        C.[TotalReembolsado], C.[MetodoReembolso],
        C.[Estatus], C.[CreadoPor], C.[FechaCreacion], C.[ModificadoPor], C.[FechaModificacion]
    FROM [dbo].[Cancelacion] C
    INNER JOIN [dbo].[Venta] V ON V.[Id] = C.[VentaId]
    WHERE C.[EmpresaId] = @EmpresaId
      AND C.[Estatus] = 1
      AND (@SucursalId = 0 OR C.[SucursalId] = @SucursalId)
    ORDER BY C.[FechaCancelacion] DESC;
END;

GO
PRINT 'Creando SP sp_Cancelacion_Obtener';
GO
CREATE PROCEDURE [dbo].[sp_Cancelacion_Obtener]
    @EmpresaId BIGINT,
    @CancelacionId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        C.[Id], C.[EmpresaId], C.[SucursalId], C.[VentaId], V.[Folio] AS [VentaFolio],
        C.[Tipo], C.[Motivo], C.[UsuarioCancela], C.[UsuarioAutoriza], C.[FechaCancelacion],
        C.[TotalReembolsado], C.[MetodoReembolso],
        C.[Estatus], C.[CreadoPor], C.[FechaCreacion], C.[ModificadoPor], C.[FechaModificacion]
    FROM [dbo].[Cancelacion] C
    INNER JOIN [dbo].[Venta] V ON V.[Id] = C.[VentaId]
    WHERE C.[EmpresaId] = @EmpresaId
      AND C.[Id] = @CancelacionId;

    SELECT
        D.[Id], D.[CancelacionId], D.[VentaDetalleId], D.[ProductoId], P.[Nombre] AS [ProductoNombre],
        D.[Cantidad], D.[Importe], D.[RegresaAStock],
        D.[Estatus], D.[CreadoPor], D.[FechaCreacion], D.[ModificadoPor], D.[FechaModificacion]
    FROM [dbo].[DevolucionDetalle] D
    INNER JOIN [dbo].[Producto] P ON P.[Id] = D.[ProductoId]
    WHERE D.[CancelacionId] = @CancelacionId
      AND D.[Estatus] = 1;
END;

GO
PRINT 'Creando SP sp_Categoria_Actualizar';
GO
CREATE PROCEDURE [dbo].[sp_Categoria_Actualizar]
    @EmpresaId BIGINT,
    @CategoriaId BIGINT,
    @CategoriaPadreId BIGINT = NULL,
    @Nombre NVARCHAR(100),
    @Descripcion NVARCHAR(MAX) = NULL,
    @Area NVARCHAR(100) = NULL,
    @Orden INT = 0,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    IF @CategoriaPadreId IS NOT NULL
       AND @CategoriaPadreId = @CategoriaId
    BEGIN
        RAISERROR('Una categoría no puede ser su propia categoría padre.', 16, 1);
        RETURN;
    END;

    IF @CategoriaPadreId IS NOT NULL
       AND NOT EXISTS
       (
           SELECT 1
           FROM [dbo].[Categoria]
           WHERE [Id] = @CategoriaPadreId
             AND [EmpresaId] = @EmpresaId
             AND [Estatus] = 1
       )
    BEGIN
        RAISERROR('Categoría padre no válida para la empresa.', 16, 1);
        RETURN;
    END;

    UPDATE [dbo].[Categoria]
    SET
        [CategoriaPadreId] = @CategoriaPadreId,
        [Nombre] = LTRIM(RTRIM(@Nombre)),
        [Descripcion] = @Descripcion,
        [Area] = @Area,
        [Orden] = @Orden,
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [EmpresaId] = @EmpresaId
      AND [Id] = @CategoriaId
      AND [Estatus] = 1;

    SELECT @@ROWCOUNT AS [AffectedRows];
END;

GO
PRINT 'Creando SP sp_Categoria_Consultar';
GO
CREATE PROCEDURE [dbo].[sp_Categoria_Consultar]
    @EmpresaId BIGINT,
    @CategoriaId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        C.[Id],
        C.[EmpresaId],
        C.[CategoriaPadreId],
        P.[Nombre] AS [CategoriaPadreNombre],
        C.[Nombre],
        C.[Descripcion],
        C.[Area],
        C.[Orden],
        C.[Estatus],
        C.[CreadoPor],
        C.[FechaCreacion],
        C.[ModificadoPor],
        C.[FechaModificacion]
    FROM [dbo].[Categoria] C
    LEFT JOIN [dbo].[Categoria] P
        ON P.[Id] = C.[CategoriaPadreId]
    WHERE C.[EmpresaId] = @EmpresaId
      AND C.[Id] = @CategoriaId;
END;

GO
PRINT 'Creando SP sp_Categoria_EliminarLogico';
GO
CREATE PROCEDURE [dbo].[sp_Categoria_EliminarLogico]
    @EmpresaId BIGINT,
    @CategoriaId BIGINT,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS
    (
        SELECT 1
        FROM [dbo].[Categoria]
        WHERE [EmpresaId] = @EmpresaId
          AND [CategoriaPadreId] = @CategoriaId
          AND [Estatus] = 1
    )
    BEGIN
        RAISERROR('No se puede eliminar una categoría con subcategorías activas.', 16, 1);
        RETURN;
    END;

    UPDATE [dbo].[Categoria]
    SET
        [Estatus] = 0,
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [EmpresaId] = @EmpresaId
      AND [Id] = @CategoriaId
      AND [Estatus] = 1;

    SELECT @@ROWCOUNT AS [AffectedRows];
END;

GO
PRINT 'Creando SP sp_Categoria_Guardar';
GO
CREATE PROCEDURE [dbo].[sp_Categoria_Guardar]
    @EmpresaId BIGINT,
    @CategoriaId BIGINT = 0,
    @CategoriaPadreId BIGINT = NULL,
    @Nombre NVARCHAR(100),
    @Descripcion NVARCHAR(MAX) = NULL,
    @Area NVARCHAR(100) = NULL,
    @Orden INT = 0,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM [dbo].[Empresa] WHERE [Id] = @EmpresaId AND [Estatus] = 1)
    BEGIN
        RAISERROR('Empresa no valida para operacion de Categoria.', 16, 1);
        RETURN;
    END;

    IF @CategoriaId IS NULL OR @CategoriaId = 0
    BEGIN
        INSERT INTO [dbo].[Categoria]
        ([EmpresaId], [CategoriaPadreId], [Nombre], [Descripcion], [Area], [Orden],
         [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
        VALUES
        (@EmpresaId, @CategoriaPadreId, LTRIM(RTRIM(@Nombre)), @Descripcion, @Area, @Orden,
         1, @Actor, GETDATE(), NULL, NULL);

        SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS [Id];
    END
    ELSE
    BEGIN
        UPDATE [dbo].[Categoria]
        SET [CategoriaPadreId] = @CategoriaPadreId,
            [Nombre] = LTRIM(RTRIM(@Nombre)),
            [Descripcion] = @Descripcion,
            [Area] = @Area,
            [Orden] = @Orden,
            [ModificadoPor] = @Actor,
            [FechaModificacion] = GETDATE()
        WHERE [EmpresaId] = @EmpresaId
          AND [Id] = @CategoriaId
          AND [Estatus] = 1;

        SELECT @CategoriaId AS [Id];
    END
END;

GO
PRINT 'Creando SP sp_Categoria_Insertar';
GO
CREATE PROCEDURE [dbo].[sp_Categoria_Insertar]
    @EmpresaId BIGINT,
    @CategoriaPadreId BIGINT = NULL,
    @Nombre NVARCHAR(100),
    @Descripcion NVARCHAR(MAX) = NULL,
    @Area NVARCHAR(100) = NULL,
    @Orden INT = 0,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[Empresa]
        WHERE [Id] = @EmpresaId
          AND [Estatus] = 1
    )
    BEGIN
        RAISERROR('Empresa no válida para operación de Categoría.', 16, 1);
        RETURN;
    END;

    IF @CategoriaPadreId IS NOT NULL
       AND NOT EXISTS
       (
           SELECT 1
           FROM [dbo].[Categoria]
           WHERE [Id] = @CategoriaPadreId
             AND [EmpresaId] = @EmpresaId
             AND [Estatus] = 1
       )
    BEGIN
        RAISERROR('Categoría padre no válida para la empresa.', 16, 1);
        RETURN;
    END;

    INSERT INTO [dbo].[Categoria]
    (
        [EmpresaId], [CategoriaPadreId], [Nombre], [Descripcion], [Area], [Orden], [Estatus],
        [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion]
    )
    VALUES
    (
        @EmpresaId, @CategoriaPadreId, LTRIM(RTRIM(@Nombre)), @Descripcion, @Area, @Orden, 1,
        @Actor, GETDATE(), NULL, NULL
    );

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS [Id];
END;

GO
PRINT 'Creando SP sp_Categoria_Listar';
GO
CREATE PROCEDURE [dbo].[sp_Categoria_Listar]
    @EmpresaId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        C.[Id],
        C.[EmpresaId],
        C.[CategoriaPadreId],
        P.[Nombre] AS [CategoriaPadreNombre],
        C.[Nombre],
        C.[Descripcion],
        C.[Area],
        C.[Orden],
        C.[Estatus],
        C.[CreadoPor],
        C.[FechaCreacion],
        C.[ModificadoPor],
        C.[FechaModificacion]
    FROM [dbo].[Categoria] C
    LEFT JOIN [dbo].[Categoria] P
        ON P.[Id] = C.[CategoriaPadreId]
    WHERE C.[EmpresaId] = @EmpresaId
      AND C.[Estatus] = 1
    ORDER BY C.[Orden] ASC, C.[Nombre] ASC;
END;

GO
PRINT 'Creando SP sp_Categoria_ListarPorPadre';
GO
CREATE PROCEDURE [dbo].[sp_Categoria_ListarPorPadre]
    @EmpresaId BIGINT,
    @CategoriaPadreId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        C.[Id],
        C.[EmpresaId],
        C.[CategoriaPadreId],
        P.[Nombre] AS [CategoriaPadreNombre],
        C.[Nombre],
        C.[Descripcion],
        C.[Area],
        C.[Orden],
        C.[Estatus],
        C.[CreadoPor],
        C.[FechaCreacion],
        C.[ModificadoPor],
        C.[FechaModificacion]
    FROM [dbo].[Categoria] C
    LEFT JOIN [dbo].[Categoria] P
        ON P.[Id] = C.[CategoriaPadreId]
    WHERE C.[EmpresaId] = @EmpresaId
      AND C.[CategoriaPadreId] = @CategoriaPadreId
      AND C.[Estatus] = 1
    ORDER BY C.[Orden] ASC, C.[Nombre] ASC;
END;

GO
PRINT 'Creando SP sp_Categoria_Obtener';
GO
CREATE PROCEDURE [dbo].[sp_Categoria_Obtener]
    @EmpresaId BIGINT,
    @CategoriaId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        C.[Id], C.[EmpresaId], C.[CategoriaPadreId], P.[Nombre] AS [CategoriaPadreNombre],
        C.[Nombre], C.[Descripcion], C.[Area], C.[Orden],
        C.[Estatus], C.[CreadoPor], C.[FechaCreacion], C.[ModificadoPor], C.[FechaModificacion]
    FROM [dbo].[Categoria] C
    LEFT JOIN [dbo].[Categoria] P ON P.[Id] = C.[CategoriaPadreId]
    WHERE C.[EmpresaId] = @EmpresaId
      AND C.[Id] = @CategoriaId;
END;

GO
PRINT 'Creando SP sp_Cliente_Actualizar';
GO
CREATE PROCEDURE [dbo].[sp_Cliente_Actualizar]
    @EmpresaId BIGINT,
    @ClienteId BIGINT,
    @Nombre NVARCHAR(150),
    @Correo NVARCHAR(250) = NULL,
    @Telefono NVARCHAR(50) = NULL,
    @Direccion NVARCHAR(500) = NULL,
    @EsPublicoGeneral BIT = 0,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Cliente]
    SET
        [Nombre] = LTRIM(RTRIM(@Nombre)),
        [Correo] = @Correo,
        [Telefono] = @Telefono,
        [Direccion] = @Direccion,
        [EsPublicoGeneral] = @EsPublicoGeneral,
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [EmpresaId] = @EmpresaId
      AND [Id] = @ClienteId
      AND [Estatus] = 1;

    SELECT @@ROWCOUNT AS [AffectedRows];
END;

GO
PRINT 'Creando SP sp_Cliente_Consultar';
GO
CREATE PROCEDURE [dbo].[sp_Cliente_Consultar]
    @EmpresaId BIGINT,
    @ClienteId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id],
        [EmpresaId],
        [Nombre],
        [Correo],
        [Telefono],
        [Direccion],
        [EsPublicoGeneral],
        [Estatus],
        [CreadoPor],
        [FechaCreacion],
        [ModificadoPor],
        [FechaModificacion]
    FROM [dbo].[Cliente]
    WHERE [EmpresaId] = @EmpresaId
      AND [Id] = @ClienteId;
END;

GO
PRINT 'Creando SP sp_Cliente_EliminarLogico';
GO
CREATE PROCEDURE [dbo].[sp_Cliente_EliminarLogico]
    @EmpresaId BIGINT,
    @ClienteId BIGINT,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS
    (
        SELECT 1
        FROM [dbo].[Cliente]
        WHERE [EmpresaId] = @EmpresaId
          AND [Id] = @ClienteId
          AND [EsPublicoGeneral] = 1
          AND [Estatus] = 1
    )
    BEGIN
        RAISERROR('El cliente "Público General" no puede eliminarse.', 16, 1);
        RETURN;
    END;

    UPDATE [dbo].[Cliente]
    SET
        [Estatus] = 0,
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [EmpresaId] = @EmpresaId
      AND [Id] = @ClienteId
      AND [Estatus] = 1;

    SELECT @@ROWCOUNT AS [AffectedRows];
END;

GO
PRINT 'Creando SP sp_Cliente_Guardar';
GO
CREATE PROCEDURE [dbo].[sp_Cliente_Guardar]
    @EmpresaId BIGINT,
    @ClienteId BIGINT = 0,
    @Nombre NVARCHAR(150),
    @Correo NVARCHAR(150) = NULL,
    @Telefono NVARCHAR(30) = NULL,
    @Direccion NVARCHAR(300) = NULL,
    @EsPublicoGeneral BIT = 0,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM [dbo].[Empresa] WHERE [Id] = @EmpresaId AND [Estatus] = 1)
    BEGIN
        RAISERROR('Empresa no valida para operacion de Cliente.', 16, 1);
        RETURN;
    END;

    IF @ClienteId IS NULL OR @ClienteId = 0
    BEGIN
        INSERT INTO [dbo].[Cliente]
        ([EmpresaId], [Nombre], [Correo], [Telefono], [Direccion], [EsPublicoGeneral],
         [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
        VALUES
        (@EmpresaId, LTRIM(RTRIM(@Nombre)), @Correo, @Telefono, @Direccion, @EsPublicoGeneral,
         1, @Actor, GETDATE(), NULL, NULL);

        SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS [Id];
    END
    ELSE
    BEGIN
        UPDATE [dbo].[Cliente]
        SET [Nombre] = LTRIM(RTRIM(@Nombre)),
            [Correo] = @Correo,
            [Telefono] = @Telefono,
            [Direccion] = @Direccion,
            [EsPublicoGeneral] = @EsPublicoGeneral,
            [ModificadoPor] = @Actor,
            [FechaModificacion] = GETDATE()
        WHERE [EmpresaId] = @EmpresaId
          AND [Id] = @ClienteId
          AND [Estatus] = 1;

        SELECT @ClienteId AS [Id];
    END
END;

GO
PRINT 'Creando SP sp_Cliente_Insertar';
GO
CREATE PROCEDURE [dbo].[sp_Cliente_Insertar]
    @EmpresaId BIGINT,
    @Nombre NVARCHAR(150),
    @Correo NVARCHAR(250) = NULL,
    @Telefono NVARCHAR(50) = NULL,
    @Direccion NVARCHAR(500) = NULL,
    @EsPublicoGeneral BIT = 0,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[Empresa]
        WHERE [Id] = @EmpresaId
          AND [Estatus] = 1
    )
    BEGIN
        RAISERROR('Empresa no válida para operación de Cliente.', 16, 1);
        RETURN;
    END;

    INSERT INTO [dbo].[Cliente]
    (
        [EmpresaId], [Nombre], [Correo], [Telefono], [Direccion], [EsPublicoGeneral], [Estatus],
        [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion]
    )
    VALUES
    (
        @EmpresaId, LTRIM(RTRIM(@Nombre)), @Correo, @Telefono, @Direccion, @EsPublicoGeneral, 1,
        @Actor, GETDATE(), NULL, NULL
    );

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS [Id];
END;

GO
PRINT 'Creando SP sp_Cliente_Listar';
GO
CREATE PROCEDURE [dbo].[sp_Cliente_Listar]
    @EmpresaId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id],
        [EmpresaId],
        [Nombre],
        [Correo],
        [Telefono],
        [Direccion],
        [EsPublicoGeneral],
        [Estatus],
        [CreadoPor],
        [FechaCreacion],
        [ModificadoPor],
        [FechaModificacion]
    FROM [dbo].[Cliente]
    WHERE [EmpresaId] = @EmpresaId
      AND [Estatus] = 1
    ORDER BY [EsPublicoGeneral] DESC, [Nombre] ASC;
END;

GO
PRINT 'Creando SP sp_Cliente_Obtener';
GO
CREATE PROCEDURE [dbo].[sp_Cliente_Obtener]
    @EmpresaId BIGINT,
    @ClienteId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id], [EmpresaId], [Nombre], [Correo], [Telefono], [Direccion], [EsPublicoGeneral],
        [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion]
    FROM [dbo].[Cliente]
    WHERE [EmpresaId] = @EmpresaId
      AND [Id] = @ClienteId;
END;

GO
PRINT 'Creando SP sp_Cliente_PublicoGeneral';
GO
CREATE PROCEDURE [dbo].[sp_Cliente_PublicoGeneral]
    @EmpresaId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        [Id], [EmpresaId], [Nombre], [Correo], [Telefono], [Direccion], [EsPublicoGeneral],
        [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion]
    FROM [dbo].[Cliente]
    WHERE [EmpresaId] = @EmpresaId
      AND [EsPublicoGeneral] = 1
      AND [Estatus] = 1
    ORDER BY [Id] ASC;
END;

GO
PRINT 'Creando SP sp_Compra_EliminarLogico';
GO
CREATE PROCEDURE [dbo].[sp_Compra_EliminarLogico]
    @EmpresaId BIGINT,
    @CompraId BIGINT,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Compra]
    SET [Estatus] = 0,
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [EmpresaId] = @EmpresaId
      AND [Id] = @CompraId
      AND [Estatus] = 1;

    UPDATE [dbo].[CompraDetalle]
    SET [Estatus] = 0,
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [CompraId] = @CompraId
      AND [Estatus] = 1;

    SELECT @@ROWCOUNT AS [AffectedRows];
END;

GO
PRINT 'Creando SP sp_Compra_Guardar';
GO
CREATE PROCEDURE [dbo].[sp_Compra_Guardar]
    @EmpresaId BIGINT,
    @SucursalId BIGINT,
    @ProveedorId BIGINT,
    @Folio NVARCHAR(50) = NULL,
    @FechaCompra DATETIME = NULL,
    @Subtotal DECIMAL(18,4) = 0,
    @Impuesto DECIMAL(18,4) = 0,
    @Total DECIMAL(18,4) = 0,
    @Observaciones NVARCHAR(MAX) = NULL,
    @DetalleJson NVARCHAR(MAX) = NULL,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS (SELECT 1 FROM [dbo].[Proveedor] WHERE [Id] = @ProveedorId AND [EmpresaId] = @EmpresaId AND [Estatus] = 1)
    BEGIN
        RAISERROR('Proveedor no valido para operacion de Compra.', 16, 1);
        RETURN;
    END;

    IF @FechaCompra IS NULL SET @FechaCompra = GETDATE();

    DECLARE @CompraId BIGINT;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO [dbo].[Compra]
        ([EmpresaId], [SucursalId], [ProveedorId], [Folio], [FechaCompra], [Subtotal], [Impuesto], [Total], [Observaciones],
         [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
        VALUES
        (@EmpresaId, @SucursalId, @ProveedorId, @Folio, @FechaCompra, @Subtotal, @Impuesto, @Total, @Observaciones,
         1, @Actor, GETDATE(), NULL, NULL);

        SET @CompraId = CAST(SCOPE_IDENTITY() AS BIGINT);

        DECLARE @Detalle TABLE
        (
            ProductoId BIGINT,
            Cantidad DECIMAL(18,4),
            CostoUnitario DECIMAL(18,4),
            Importe DECIMAL(18,4)
        );

        IF @DetalleJson IS NOT NULL AND LEN(@DetalleJson) > 2
        BEGIN
            INSERT INTO @Detalle (ProductoId, Cantidad, CostoUnitario, Importe)
            SELECT ProductoId, Cantidad, CostoUnitario, Importe
            FROM OPENJSON(@DetalleJson)
            WITH
            (
                ProductoId BIGINT '$.ProductoId',
                Cantidad DECIMAL(18,4) '$.Cantidad',
                CostoUnitario DECIMAL(18,4) '$.CostoUnitario',
                Importe DECIMAL(18,4) '$.Importe'
            );
        END

        INSERT INTO [dbo].[CompraDetalle]
        ([CompraId], [ProductoId], [Cantidad], [CostoUnitario], [Importe],
         [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
        SELECT @CompraId, D.[ProductoId], D.[Cantidad], D.[CostoUnitario], D.[Importe],
               1, @Actor, GETDATE(), NULL, NULL
        FROM @Detalle D;

        -- Ingreso de stock por cada linea (crea el registro si no existe).
        UPDATE S
        SET S.[Cantidad] = S.[Cantidad] + D.[Cantidad],
            S.[ModificadoPor] = @Actor,
            S.[FechaModificacion] = GETDATE()
        FROM [dbo].[Stock] S
        INNER JOIN @Detalle D ON D.[ProductoId] = S.[ProductoId]
        WHERE S.[SucursalId] = @SucursalId
          AND S.[Estatus] = 1;

        INSERT INTO [dbo].[Stock]
        ([EmpresaId], [SucursalId], [ProductoId], [Cantidad], [StockMinimo],
         [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
        SELECT @EmpresaId, @SucursalId, D.[ProductoId], D.[Cantidad], 0,
               1, @Actor, GETDATE(), NULL, NULL
        FROM @Detalle D
        WHERE NOT EXISTS
        (
            SELECT 1 FROM [dbo].[Stock] S
            WHERE S.[SucursalId] = @SucursalId AND S.[ProductoId] = D.[ProductoId] AND S.[Estatus] = 1
        );

        -- Bitacora de movimientos.
        INSERT INTO [dbo].[StockMovimiento]
        ([EmpresaId], [SucursalId], [ProductoId], [TipoMovimiento], [Cantidad],
         [ExistenciaAnterior], [ExistenciaNueva], [Motivo], [ReferenciaId],
         [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
        SELECT @EmpresaId, @SucursalId, D.[ProductoId], N'Ingreso', D.[Cantidad],
               ISNULL(S.[Cantidad] - D.[Cantidad], 0), ISNULL(S.[Cantidad], D.[Cantidad]), N'Compra', @CompraId,
               1, @Actor, GETDATE(), NULL, NULL
        FROM @Detalle D
        LEFT JOIN [dbo].[Stock] S ON S.[ProductoId] = D.[ProductoId] AND S.[SucursalId] = @SucursalId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH

    SELECT @CompraId AS [Id];
END;

GO
PRINT 'Creando SP sp_Compra_Listar';
GO
CREATE PROCEDURE [dbo].[sp_Compra_Listar]
    @EmpresaId BIGINT,
    @SucursalId BIGINT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        C.[Id], C.[EmpresaId], C.[SucursalId], S.[Nombre] AS [SucursalNombre],
        C.[ProveedorId], P.[Nombre] AS [ProveedorNombre], C.[Folio], C.[FechaCompra],
        C.[Subtotal], C.[Impuesto], C.[Total], C.[Observaciones],
        C.[Estatus], C.[CreadoPor], C.[FechaCreacion], C.[ModificadoPor], C.[FechaModificacion]
    FROM [dbo].[Compra] C
    INNER JOIN [dbo].[Sucursal] S ON S.[Id] = C.[SucursalId]
    INNER JOIN [dbo].[Proveedor] P ON P.[Id] = C.[ProveedorId]
    WHERE C.[EmpresaId] = @EmpresaId
      AND C.[Estatus] = 1
      AND (@SucursalId = 0 OR C.[SucursalId] = @SucursalId)
    ORDER BY C.[FechaCompra] DESC;
END;

GO
PRINT 'Creando SP sp_Compra_Obtener';
GO
CREATE PROCEDURE [dbo].[sp_Compra_Obtener]
    @EmpresaId BIGINT,
    @CompraId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        C.[Id], C.[EmpresaId], C.[SucursalId], S.[Nombre] AS [SucursalNombre],
        C.[ProveedorId], P.[Nombre] AS [ProveedorNombre], C.[Folio], C.[FechaCompra],
        C.[Subtotal], C.[Impuesto], C.[Total], C.[Observaciones],
        C.[Estatus], C.[CreadoPor], C.[FechaCreacion], C.[ModificadoPor], C.[FechaModificacion]
    FROM [dbo].[Compra] C
    INNER JOIN [dbo].[Sucursal] S ON S.[Id] = C.[SucursalId]
    INNER JOIN [dbo].[Proveedor] P ON P.[Id] = C.[ProveedorId]
    WHERE C.[EmpresaId] = @EmpresaId
      AND C.[Id] = @CompraId;

    SELECT
        D.[Id], D.[CompraId], D.[ProductoId], PR.[Nombre] AS [ProductoNombre],
        D.[Cantidad], D.[CostoUnitario], D.[Importe],
        D.[Estatus], D.[CreadoPor], D.[FechaCreacion], D.[ModificadoPor], D.[FechaModificacion]
    FROM [dbo].[CompraDetalle] D
    INNER JOIN [dbo].[Producto] PR ON PR.[Id] = D.[ProductoId]
    WHERE D.[CompraId] = @CompraId
      AND D.[Estatus] = 1;
END;

GO
PRINT 'Creando SP sp_Corte_Guardar';
GO
CREATE PROCEDURE [dbo].[sp_Corte_Guardar]
    @EmpresaId BIGINT,
    @SucursalId BIGINT,
    @UsuarioId BIGINT,
    @CajaChicaId BIGINT,
    @MontoInicial DECIMAL(18,4) = 0,
    @EfectivoContado DECIMAL(18,4) = 0,
    @Observaciones NVARCHAR(MAX) = NULL,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @FechaApertura DATETIME;
    DECLARE @Salidas DECIMAL(18,4) = 0;
    DECLARE @TotalEfectivo DECIMAL(18,4) = 0;
    DECLARE @TotalTarjeta DECIMAL(18,4) = 0;
    DECLARE @TotalTransferencia DECIMAL(18,4) = 0;
    DECLARE @TotalVentas DECIMAL(18,4) = 0;
    DECLARE @Esperado DECIMAL(18,4);
    DECLARE @Diferencia DECIMAL(18,4);
    DECLARE @CorteId BIGINT;

    SELECT @FechaApertura = [FechaApertura], @Salidas = [SalidasTotales]
    FROM [dbo].[CajaChica]
    WHERE [Id] = @CajaChicaId AND [EmpresaId] = @EmpresaId AND [Estado] = N'Abierta' AND [Estatus] = 1;

    IF @FechaApertura IS NULL
    BEGIN
        RAISERROR('Caja chica no encontrada o ya cerrada para corte.', 16, 1);
        RETURN;
    END;

    SELECT
        @TotalEfectivo = ISNULL(SUM(CASE WHEN [MetodoPago] = N'Efectivo' THEN [Total] ELSE 0 END), 0),
        @TotalTarjeta = ISNULL(SUM(CASE WHEN [MetodoPago] = N'Tarjeta' THEN [Total] ELSE 0 END), 0),
        @TotalTransferencia = ISNULL(SUM(CASE WHEN [MetodoPago] = N'Transferencia' THEN [Total] ELSE 0 END), 0)
    FROM [dbo].[Venta]
    WHERE [EmpresaId] = @EmpresaId
      AND [SucursalId] = @SucursalId
      AND [CajaChicaId] = @CajaChicaId
      AND [Estatus] = 1
      AND [FechaVenta] >= @FechaApertura;

    SET @TotalVentas = @TotalEfectivo + @TotalTarjeta + @TotalTransferencia;
    SET @Esperado = @MontoInicial + @TotalEfectivo - @Salidas;
    SET @Diferencia = @EfectivoContado - @Esperado;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO [dbo].[Corte]
        ([EmpresaId], [SucursalId], [UsuarioId], [CajaChicaId], [FechaCorte], [MontoInicial],
         [TotalVentas], [TotalEfectivo], [TotalTarjeta], [TotalTransferencia], [Salidas],
         [EfectivoEsperado], [EfectivoContado], [Diferencia], [Observaciones],
         [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
        VALUES
        (@EmpresaId, @SucursalId, @UsuarioId, @CajaChicaId, GETDATE(), @MontoInicial,
         @TotalVentas, @TotalEfectivo, @TotalTarjeta, @TotalTransferencia, @Salidas,
         @Esperado, @EfectivoContado, @Diferencia, @Observaciones,
         1, @Actor, GETDATE(), NULL, NULL);

        SET @CorteId = CAST(SCOPE_IDENTITY() AS BIGINT);

        INSERT INTO [dbo].[CorteDetalle]
        ([CorteId], [MetodoPago], [Monto], [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
        VALUES
        (@CorteId, N'Efectivo', @TotalEfectivo, 1, @Actor, GETDATE(), NULL, NULL),
        (@CorteId, N'Tarjeta', @TotalTarjeta, 1, @Actor, GETDATE(), NULL, NULL),
        (@CorteId, N'Transferencia', @TotalTransferencia, 1, @Actor, GETDATE(), NULL, NULL);

        UPDATE [dbo].[CajaChica]
        SET [Estado] = N'Cerrada',
            [FechaCierre] = GETDATE(),
            [IngresosTotales] = @TotalVentas,
            [ModificadoPor] = @Actor,
            [FechaModificacion] = GETDATE()
        WHERE [Id] = @CajaChicaId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH

    SELECT @CorteId AS [Id];
END;

GO
PRINT 'Creando SP sp_Corte_Listar';
GO
CREATE PROCEDURE [dbo].[sp_Corte_Listar]
    @EmpresaId BIGINT,
    @SucursalId BIGINT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        C.[Id], C.[EmpresaId], C.[SucursalId], S.[Nombre] AS [SucursalNombre],
        C.[UsuarioId], U.[NombreUsuario] AS [UsuarioNombre], C.[CajaChicaId], C.[FechaCorte],
        C.[MontoInicial], C.[TotalVentas], C.[TotalEfectivo], C.[TotalTarjeta], C.[TotalTransferencia],
        C.[Salidas], C.[EfectivoEsperado], C.[EfectivoContado], C.[Diferencia], C.[Observaciones],
        C.[Estatus], C.[CreadoPor], C.[FechaCreacion], C.[ModificadoPor], C.[FechaModificacion]
    FROM [dbo].[Corte] C
    INNER JOIN [dbo].[Sucursal] S ON S.[Id] = C.[SucursalId]
    INNER JOIN [dbo].[Usuario] U ON U.[Id] = C.[UsuarioId]
    WHERE C.[EmpresaId] = @EmpresaId
      AND C.[Estatus] = 1
      AND (@SucursalId = 0 OR C.[SucursalId] = @SucursalId)
    ORDER BY C.[FechaCorte] DESC;
END;

GO
PRINT 'Creando SP sp_Corte_Obtener';
GO
CREATE PROCEDURE [dbo].[sp_Corte_Obtener]
    @EmpresaId BIGINT,
    @CorteId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        C.[Id], C.[EmpresaId], C.[SucursalId], S.[Nombre] AS [SucursalNombre],
        C.[UsuarioId], U.[NombreUsuario] AS [UsuarioNombre], C.[CajaChicaId], C.[FechaCorte],
        C.[MontoInicial], C.[TotalVentas], C.[TotalEfectivo], C.[TotalTarjeta], C.[TotalTransferencia],
        C.[Salidas], C.[EfectivoEsperado], C.[EfectivoContado], C.[Diferencia], C.[Observaciones],
        C.[Estatus], C.[CreadoPor], C.[FechaCreacion], C.[ModificadoPor], C.[FechaModificacion]
    FROM [dbo].[Corte] C
    INNER JOIN [dbo].[Sucursal] S ON S.[Id] = C.[SucursalId]
    INNER JOIN [dbo].[Usuario] U ON U.[Id] = C.[UsuarioId]
    WHERE C.[EmpresaId] = @EmpresaId
      AND C.[Id] = @CorteId;

    SELECT
        D.[Id], D.[CorteId], D.[MetodoPago], D.[Monto], D.[FolioCobro],
        D.[TicketCobroUrl], D.[TicketSistemaUrl], D.[ComprobanteUrl],
        D.[Estatus], D.[CreadoPor], D.[FechaCreacion], D.[ModificadoPor], D.[FechaModificacion]
    FROM [dbo].[CorteDetalle] D
    WHERE D.[CorteId] = @CorteId
      AND D.[Estatus] = 1;
END;

GO
PRINT 'Creando SP sp_Empresa_Guardar';
GO
CREATE PROCEDURE [dbo].[sp_Empresa_Guardar]
    @NombreComercial NVARCHAR(250),
    @RazonSocial NVARCHAR(250),
    @RFC NVARCHAR(50),
    @Responsable NVARCHAR(250),
    @Direccion NVARCHAR(500),
    @Ciudad NVARCHAR(100) = NULL,
    @Estado NVARCHAR(100) = NULL,
    @CodigoPostal NVARCHAR(10) = NULL,
    @Telefono NVARCHAR(50) = NULL,
    @CorreoContacto NVARCHAR(250),
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    IF @NombreComercial IS NULL OR LTRIM(RTRIM(@NombreComercial)) = ''
    BEGIN
        RAISERROR('El Nombre Comercial es requerido.', 16, 1);
        RETURN;
    END;

    IF @RazonSocial IS NULL OR LTRIM(RTRIM(@RazonSocial)) = ''
    BEGIN
        RAISERROR('La Razón Social es requerida.', 16, 1);
        RETURN;
    END;

    IF @RFC IS NULL OR LTRIM(RTRIM(@RFC)) = ''
    BEGIN
        RAISERROR('El RFC es requerido.', 16, 1);
        RETURN;
    END;

    IF @Responsable IS NULL OR LTRIM(RTRIM(@Responsable)) = ''
    BEGIN
        RAISERROR('El Responsable es requerido.', 16, 1);
        RETURN;
    END;

    IF @Direccion IS NULL OR LTRIM(RTRIM(@Direccion)) = ''
    BEGIN
        RAISERROR('La Dirección es requerida.', 16, 1);
        RETURN;
    END;

    IF @CorreoContacto IS NULL OR LTRIM(RTRIM(@CorreoContacto)) = ''
    BEGIN
        RAISERROR('El Correo de Contacto es requerido.', 16, 1);
        RETURN;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM [dbo].[Empresa]
        WHERE [RFC] = LTRIM(RTRIM(@RFC))
          AND [Estatus] = 1
    )
    BEGIN
        RAISERROR('Ya existe una empresa activa con el mismo RFC.', 16, 1);
        RETURN;
    END;

    INSERT INTO [dbo].[Empresa]
    (
        [NombreComercial], [RazonSocial], [RFC], [Responsable], [Direccion],
        [Ciudad], [Estado], [CodigoPostal], [Telefono], [CorreoContacto],
        [FechaVigenciaInicio], [FechaVigenciaFin], [EsPeriodoPrueba], [Estatus],
        [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion], [LogoUrl]
    )
    VALUES
    (
        LTRIM(RTRIM(@NombreComercial)), LTRIM(RTRIM(@RazonSocial)), LTRIM(RTRIM(@RFC)),
        LTRIM(RTRIM(@Responsable)), LTRIM(RTRIM(@Direccion)),
        @Ciudad, @Estado, @CodigoPostal, @Telefono, LTRIM(RTRIM(@CorreoContacto)),
        GETDATE(), DATEADD(DAY, 30, GETDATE()), 1, 1,
        @Actor, GETDATE(), NULL, NULL, NULL
    );

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS [Id];
END;

GO
PRINT 'Creando SP sp_Marca_Actualizar';
GO
CREATE PROCEDURE [dbo].[sp_Marca_Actualizar]
    @EmpresaId BIGINT,
    @MarcaId BIGINT,
    @Nombre NVARCHAR(100),
    @Descripcion NVARCHAR(MAX) = NULL,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Marca]
    SET
        [Nombre] = LTRIM(RTRIM(@Nombre)),
        [Descripcion] = @Descripcion,
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [EmpresaId] = @EmpresaId
      AND [Id] = @MarcaId
      AND [Estatus] = 1;

    SELECT @@ROWCOUNT AS [AffectedRows];
END;

GO
PRINT 'Creando SP sp_Marca_Consultar';
GO
CREATE PROCEDURE [dbo].[sp_Marca_Consultar]
    @EmpresaId BIGINT,
    @MarcaId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id],
        [EmpresaId],
        [Nombre],
        [Descripcion],
        [Estatus],
        [CreadoPor],
        [FechaCreacion],
        [ModificadoPor],
        [FechaModificacion]
    FROM [dbo].[Marca]
    WHERE [EmpresaId] = @EmpresaId
      AND [Id] = @MarcaId;
END;

GO
PRINT 'Creando SP sp_Marca_EliminarLogico';
GO
CREATE PROCEDURE [dbo].[sp_Marca_EliminarLogico]
    @EmpresaId BIGINT,
    @MarcaId BIGINT,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Marca]
    SET
        [Estatus] = 0,
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [EmpresaId] = @EmpresaId
      AND [Id] = @MarcaId
      AND [Estatus] = 1;

    SELECT @@ROWCOUNT AS [AffectedRows];
END;

GO
PRINT 'Creando SP sp_Marca_Insertar';
GO
CREATE PROCEDURE [dbo].[sp_Marca_Insertar]
    @EmpresaId BIGINT,
    @Nombre NVARCHAR(100),
    @Descripcion NVARCHAR(MAX) = NULL,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[Empresa]
        WHERE [Id] = @EmpresaId
          AND [Estatus] = 1
    )
    BEGIN
        RAISERROR('Empresa no válida para operación de Marca.', 16, 1);
        RETURN;
    END;

    INSERT INTO [dbo].[Marca]
    (
        [EmpresaId], [Nombre], [Descripcion], [Estatus],
        [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion]
    )
    VALUES
    (
        @EmpresaId, LTRIM(RTRIM(@Nombre)), @Descripcion, 1,
        @Actor, GETDATE(), NULL, NULL
    );

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS [Id];
END;

GO
PRINT 'Creando SP sp_Marca_Listar';
GO
CREATE PROCEDURE [dbo].[sp_Marca_Listar]
    @EmpresaId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id],
        [EmpresaId],
        [Nombre],
        [Descripcion],
        [Estatus],
        [CreadoPor],
        [FechaCreacion],
        [ModificadoPor],
        [FechaModificacion]
    FROM [dbo].[Marca]
    WHERE [EmpresaId] = @EmpresaId
      AND [Estatus] = 1
    ORDER BY [Nombre] ASC;
END;

GO
PRINT 'Creando SP sp_Pagina_EliminarLogico';
GO
CREATE PROCEDURE [dbo].[sp_Pagina_EliminarLogico]
    @PaginaId BIGINT,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Pagina]
    SET
        [Estatus] = 0,
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [Id] = @PaginaId
      AND [Estatus] = 1;

    SELECT @@ROWCOUNT AS [AffectedRows];
END;

GO
PRINT 'Creando SP sp_Pagina_Guardar';
GO
CREATE PROCEDURE [dbo].[sp_Pagina_Guardar]
    @PaginaId BIGINT,
    @Nombre NVARCHAR(100),
    @NombreVisible NVARCHAR(150) = NULL,
    @Descripcion NVARCHAR(250) = NULL,
    @Tipo NVARCHAR(20) = N'Menu',
    @Direccion NVARCHAR(250) = NULL,
    @PermisosPadreId BIGINT = NULL,
    @Logo NVARCHAR(100) = NULL,
    @OrdenB INT = 0,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    SET @Nombre = LTRIM(RTRIM(@Nombre));
    IF @Nombre IS NULL OR LEN(@Nombre) = 0
    BEGIN
        RAISERROR('El Nombre de la Página es requerido.', 16, 1);
        RETURN;
    END;

    IF @PermisosPadreId IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM [dbo].[Pagina] WHERE [Id] = @PermisosPadreId AND [Estatus] = 1)
    BEGIN
        RAISERROR('La Página padre no es válida.', 16, 1);
        RETURN;
    END;

    IF @PaginaId = 0
    BEGIN
        IF EXISTS
        (
            SELECT 1
            FROM [dbo].[Pagina]
            WHERE [Nombre] = @Nombre
              AND [Estatus] = 1
        )
        BEGIN
            SELECT CAST(-1 AS BIGINT) AS [Id];
            RETURN;
        END;

        INSERT INTO [dbo].[Pagina]
        (
            [Nombre], [NombreVisible], [Descripcion], [Tipo], [Direccion],
            [PermisosPadreId], [Logo], [OrdenB], [Estatus],
            [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion]
        )
        VALUES
        (
            @Nombre, @NombreVisible, @Descripcion, @Tipo, @Direccion,
            @PermisosPadreId, @Logo, @OrdenB, 1,
            @Actor, GETDATE(), NULL, NULL
        );

        SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS [Id];
    END
    ELSE
    BEGIN
        UPDATE [dbo].[Pagina]
        SET
            [Nombre] = @Nombre,
            [NombreVisible] = @NombreVisible,
            [Descripcion] = @Descripcion,
            [Tipo] = @Tipo,
            [Direccion] = @Direccion,
            [PermisosPadreId] = @PermisosPadreId,
            [Logo] = @Logo,
            [OrdenB] = @OrdenB,
            [ModificadoPor] = @Actor,
            [FechaModificacion] = GETDATE()
        WHERE [Id] = @PaginaId
          AND [Estatus] = 1;

        SELECT CAST(CASE WHEN @@ROWCOUNT > 0 THEN @PaginaId ELSE 0 END AS BIGINT) AS [Id];
    END;
END;

GO
PRINT 'Creando SP sp_Pagina_Listar';
GO
CREATE PROCEDURE [dbo].[sp_Pagina_Listar]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id],
        [Nombre],
        [NombreVisible],
        [Descripcion],
        [Tipo],
        [Direccion],
        [PermisosPadreId],
        [Logo],
        [OrdenB],
        [Estatus],
        [CreadoPor],
        [FechaCreacion],
        [ModificadoPor],
        [FechaModificacion]
    FROM [dbo].[Pagina]
    WHERE [Estatus] = 1
    ORDER BY [OrdenB] ASC, [Nombre] ASC;
END;

GO
PRINT 'Creando SP sp_Pagina_Obtener';
GO
CREATE PROCEDURE [dbo].[sp_Pagina_Obtener]
    @PaginaId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id],
        [Nombre],
        [NombreVisible],
        [Descripcion],
        [Tipo],
        [Direccion],
        [PermisosPadreId],
        [Logo],
        [OrdenB],
        [Estatus],
        [CreadoPor],
        [FechaCreacion],
        [ModificadoPor],
        [FechaModificacion]
    FROM [dbo].[Pagina]
    WHERE [Id] = @PaginaId;
END;

GO
PRINT 'Creando SP sp_Pagina_PorUsuario';
GO
CREATE PROCEDURE [dbo].[sp_Pagina_PorUsuario]
    @EmpresaId BIGINT,
    @UsuarioId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        P.[Id],
        P.[Nombre],
        P.[NombreVisible],
        P.[Descripcion],
        P.[Tipo],
        P.[Direccion],
        P.[PermisosPadreId],
        P.[Logo],
        P.[OrdenB],
        P.[Estatus],
        P.[CreadoPor],
        P.[FechaCreacion],
        P.[ModificadoPor],
        P.[FechaModificacion]
    FROM [dbo].[Pagina] P
    INNER JOIN [dbo].[RolPaginaAccion] RPA
        ON P.[Id] = RPA.[PaginaId]
       AND RPA.[Estatus] = 1
       AND RPA.[PuedeLeer] = 1
    INNER JOIN [dbo].[Rol] R
        ON RPA.[RolId] = R.[Id]
       AND R.[Estatus] = 1
       AND R.[EmpresaId] = @EmpresaId
    INNER JOIN [dbo].[UsuarioRol] UR
        ON R.[Id] = UR.[RolId]
       AND UR.[Estatus] = 1
       AND UR.[UsuarioId] = @UsuarioId
    WHERE P.[Estatus] = 1

    UNION

    SELECT
        P.[Id],
        P.[Nombre],
        P.[NombreVisible],
        P.[Descripcion],
        P.[Tipo],
        P.[Direccion],
        P.[PermisosPadreId],
        P.[Logo],
        P.[OrdenB],
        P.[Estatus],
        P.[CreadoPor],
        P.[FechaCreacion],
        P.[ModificadoPor],
        P.[FechaModificacion]
    FROM [dbo].[Pagina] P
    INNER JOIN [dbo].[UsuarioPagina] UP
        ON P.[Id] = UP.[PaginaId]
       AND UP.[Estatus] = 1
       AND UP.[UsuarioId] = @UsuarioId
    WHERE P.[Estatus] = 1

    ORDER BY [OrdenB] ASC, [Nombre] ASC;
END;

GO
PRINT 'Creando SP sp_Permisos_PorUsuario';
GO
CREATE PROCEDURE [dbo].[sp_Permisos_PorUsuario]
    @NombreUsuario NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @UsuarioId BIGINT;
    DECLARE @EsAdmin BIT = 0;

    SELECT TOP (1) @UsuarioId = [Id]
    FROM [dbo].[Usuario]
    WHERE [NombreUsuario] = @NombreUsuario
      AND [Estatus] = 1;

    IF @UsuarioId IS NULL
    BEGIN
        RETURN;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM [dbo].[UsuarioRol] UR
        INNER JOIN [dbo].[Rol] R ON R.[Id] = UR.[RolId]
        WHERE UR.[UsuarioId] = @UsuarioId
          AND UR.[Estatus] = 1
          AND R.[Estatus] = 1
          AND R.[Nombre] = N'Administrador'
    )
    BEGIN
        SET @EsAdmin = 1;
    END;

    IF @EsAdmin = 1
    BEGIN
        SELECT
            P.[Id] AS [PaginaId],
            P.[Nombre] AS [PaginaNombre],
            P.[Direccion] AS [Direccion],
            CAST(1 AS BIT) AS [PuedeLeer],
            CAST(1 AS BIT) AS [PuedeCrear],
            CAST(1 AS BIT) AS [PuedeEditar],
            CAST(1 AS BIT) AS [PuedeEliminar],
            CAST(1 AS BIT) AS [PuedeExportar]
        FROM [dbo].[Pagina] P
        WHERE P.[Estatus] = 1
        ORDER BY P.[OrdenB] ASC;
        RETURN;
    END;

    SELECT
        P.[Id] AS [PaginaId],
        P.[Nombre] AS [PaginaNombre],
        P.[Direccion] AS [Direccion],
        CAST(MAX(CASE WHEN RPA.[PuedeLeer] = 1 THEN 1 ELSE 0 END) AS BIT) AS [PuedeLeer],
        CAST(MAX(CASE WHEN RPA.[PuedeCrear] = 1 THEN 1 ELSE 0 END) AS BIT) AS [PuedeCrear],
        CAST(MAX(CASE WHEN RPA.[PuedeEditar] = 1 THEN 1 ELSE 0 END) AS BIT) AS [PuedeEditar],
        CAST(MAX(CASE WHEN RPA.[PuedeEliminar] = 1 THEN 1 ELSE 0 END) AS BIT) AS [PuedeEliminar],
        CAST(MAX(CASE WHEN RPA.[PuedeExportar] = 1 THEN 1 ELSE 0 END) AS BIT) AS [PuedeExportar]
    FROM [dbo].[Pagina] P
    LEFT JOIN [dbo].[RolPaginaAccion] RPA
        ON RPA.[PaginaId] = P.[Id]
       AND RPA.[Estatus] = 1
    LEFT JOIN [dbo].[UsuarioRol] UR
        ON UR.[RolId] = RPA.[RolId]
       AND UR.[Estatus] = 1
       AND UR.[UsuarioId] = @UsuarioId
    LEFT JOIN [dbo].[UsuarioPagina] UP
        ON UP.[PaginaId] = P.[Id]
       AND UP.[Estatus] = 1
       AND UP.[UsuarioId] = @UsuarioId
    WHERE P.[Estatus] = 1
      AND (UR.[Id] IS NOT NULL OR UP.[Id] IS NOT NULL)
    GROUP BY P.[Id], P.[Nombre], P.[Direccion]
    ORDER BY P.[Id] ASC;
END;

GO
PRINT 'Creando SP sp_Permisos_Validar';
GO
CREATE PROCEDURE [dbo].[sp_Permisos_Validar]
    @UsuarioId BIGINT,
    @PaginaId BIGINT,
    @Accion NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS
    (
        SELECT 1
        FROM [dbo].[UsuarioRol] UR
        INNER JOIN [dbo].[Rol] R ON R.[Id] = UR.[RolId]
        WHERE UR.[UsuarioId] = @UsuarioId
          AND UR.[Estatus] = 1
          AND R.[Estatus] = 1
          AND R.[Nombre] = N'Administrador'
    )
    BEGIN
        SELECT CAST(1 AS INT);
        RETURN;
    END;

    DECLARE @Permitido BIT = 0;

    SELECT @Permitido = 1
    FROM [dbo].[RolPaginaAccion] RPA
    INNER JOIN [dbo].[UsuarioRol] UR ON UR.[RolId] = RPA.[RolId]
    WHERE UR.[UsuarioId] = @UsuarioId
      AND UR.[Estatus] = 1
      AND RPA.[PaginaId] = @PaginaId
      AND RPA.[Estatus] = 1
      AND
      (
          (@Accion = N'Leer' AND RPA.[PuedeLeer] = 1)
          OR (@Accion = N'Crear' AND RPA.[PuedeCrear] = 1)
          OR (@Accion = N'Editar' AND RPA.[PuedeEditar] = 1)
          OR (@Accion = N'Eliminar' AND RPA.[PuedeEliminar] = 1)
          OR (@Accion = N'Exportar' AND RPA.[PuedeExportar] = 1)
      );

    IF @Permitido = 0
    BEGIN
        IF EXISTS
        (
            SELECT 1
            FROM [dbo].[UsuarioPagina]
            WHERE [UsuarioId] = @UsuarioId
              AND [PaginaId] = @PaginaId
              AND [Estatus] = 1
        )
        BEGIN
            SET @Permitido = 1;
        END;
    END;

    SELECT CAST(ISNULL(@Permitido, 0) AS INT);
END;

GO
PRINT 'Creando SP sp_Precio_Guardar';
GO
CREATE PROCEDURE [dbo].[sp_Precio_Guardar]
    @EmpresaId BIGINT,
    @ProductoId BIGINT,
    @PrecioCompra DECIMAL(18,4),
    @PrecioVentaSugerido DECIMAL(18,4),
    @PrecioVenta DECIMAL(18,4),
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM [dbo].[Producto] WHERE [Id] = @ProductoId AND [EmpresaId] = @EmpresaId AND [Estatus] = 1)
    BEGIN
        RAISERROR('Producto no valido para operacion de Precio.', 16, 1);
        RETURN;
    END;

    -- Solo un precio activo a la vez: se cierra el vigente.
    UPDATE [dbo].[Precio]
    SET [Activo] = 0,
        [FechaFin] = GETDATE(),
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [EmpresaId] = @EmpresaId
      AND [ProductoId] = @ProductoId
      AND [Activo] = 1
      AND [Estatus] = 1;

    INSERT INTO [dbo].[Precio]
    ([EmpresaId], [ProductoId], [PrecioCompra], [PrecioVentaSugerido], [PrecioVenta], [Activo],
     [FechaInicio], [FechaFin], [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
    VALUES
    (@EmpresaId, @ProductoId, @PrecioCompra, @PrecioVentaSugerido, @PrecioVenta, 1,
     GETDATE(), NULL, 1, @Actor, GETDATE(), NULL, NULL);

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS [Id];
END;

GO
PRINT 'Creando SP sp_Precio_ObtenerActivo';
GO
CREATE PROCEDURE [dbo].[sp_Precio_ObtenerActivo]
    @EmpresaId BIGINT,
    @ProductoId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        PR.[Id], PR.[EmpresaId], PR.[ProductoId], P.[Nombre] AS [ProductoNombre],
        PR.[PrecioCompra], PR.[PrecioVentaSugerido], PR.[PrecioVenta], PR.[Activo],
        PR.[FechaInicio], PR.[FechaFin],
        PR.[Estatus], PR.[CreadoPor], PR.[FechaCreacion], PR.[ModificadoPor], PR.[FechaModificacion]
    FROM [dbo].[Precio] PR
    INNER JOIN [dbo].[Producto] P ON P.[Id] = PR.[ProductoId]
    WHERE PR.[EmpresaId] = @EmpresaId
      AND PR.[ProductoId] = @ProductoId
      AND PR.[Activo] = 1
      AND PR.[Estatus] = 1
    ORDER BY PR.[FechaInicio] DESC;
END;

GO
PRINT 'Creando SP sp_Precio_PorProducto';
GO
CREATE PROCEDURE [dbo].[sp_Precio_PorProducto]
    @EmpresaId BIGINT,
    @ProductoId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        PR.[Id], PR.[EmpresaId], PR.[ProductoId], P.[Nombre] AS [ProductoNombre],
        PR.[PrecioCompra], PR.[PrecioVentaSugerido], PR.[PrecioVenta], PR.[Activo],
        PR.[FechaInicio], PR.[FechaFin],
        PR.[Estatus], PR.[CreadoPor], PR.[FechaCreacion], PR.[ModificadoPor], PR.[FechaModificacion]
    FROM [dbo].[Precio] PR
    INNER JOIN [dbo].[Producto] P ON P.[Id] = PR.[ProductoId]
    WHERE PR.[EmpresaId] = @EmpresaId
      AND PR.[ProductoId] = @ProductoId
      AND PR.[Estatus] = 1
    ORDER BY PR.[Activo] DESC, PR.[FechaInicio] DESC;
END;

GO
PRINT 'Creando SP sp_Producto_EliminarLogico';
GO
CREATE PROCEDURE [dbo].[sp_Producto_EliminarLogico]
    @EmpresaId BIGINT,
    @ProductoId BIGINT,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Producto]
    SET [Estatus] = 0,
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [EmpresaId] = @EmpresaId
      AND [Id] = @ProductoId
      AND [Estatus] = 1;

    SELECT @@ROWCOUNT AS [AffectedRows];
END;

GO
PRINT 'Creando SP sp_Producto_Guardar';
GO
CREATE PROCEDURE [dbo].[sp_Producto_Guardar]
    @EmpresaId BIGINT,
    @ProductoId BIGINT = 0,
    @CategoriaId BIGINT = NULL,
    @MarcaId BIGINT = NULL,
    @MarcaTexto NVARCHAR(100) = NULL,
    @Nombre NVARCHAR(150),
    @Descripcion NVARCHAR(MAX) = NULL,
    @FotoUrl NVARCHAR(300) = NULL,
    @Codigo NVARCHAR(100) = NULL,
    @TipoProducto NVARCHAR(20) = N'Comprado',
    @UnidadMedida NVARCHAR(30) = NULL,
    @StockMinimo DECIMAL(18,4) = 0,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM [dbo].[Empresa] WHERE [Id] = @EmpresaId AND [Estatus] = 1)
    BEGIN
        RAISERROR('Empresa no valida para operacion de Producto.', 16, 1);
        RETURN;
    END;

    IF @ProductoId IS NULL OR @ProductoId = 0
    BEGIN
        INSERT INTO [dbo].[Producto]
        ([EmpresaId], [CategoriaId], [MarcaId], [MarcaTexto], [Nombre], [Descripcion], [FotoUrl],
         [Codigo], [TipoProducto], [UnidadMedida], [StockMinimo],
         [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
        VALUES
        (@EmpresaId, @CategoriaId, @MarcaId, @MarcaTexto, LTRIM(RTRIM(@Nombre)), @Descripcion, @FotoUrl,
         @Codigo, @TipoProducto, @UnidadMedida, @StockMinimo,
         1, @Actor, GETDATE(), NULL, NULL);

        SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS [Id];
    END
    ELSE
    BEGIN
        UPDATE [dbo].[Producto]
        SET [CategoriaId] = @CategoriaId,
            [MarcaId] = @MarcaId,
            [MarcaTexto] = @MarcaTexto,
            [Nombre] = LTRIM(RTRIM(@Nombre)),
            [Descripcion] = @Descripcion,
            [FotoUrl] = @FotoUrl,
            [Codigo] = @Codigo,
            [TipoProducto] = @TipoProducto,
            [UnidadMedida] = @UnidadMedida,
            [StockMinimo] = @StockMinimo,
            [ModificadoPor] = @Actor,
            [FechaModificacion] = GETDATE()
        WHERE [EmpresaId] = @EmpresaId
          AND [Id] = @ProductoId
          AND [Estatus] = 1;

        SELECT @ProductoId AS [Id];
    END
END;

GO
PRINT 'Creando SP sp_Producto_Listar';
GO
CREATE PROCEDURE [dbo].[sp_Producto_Listar]
    @EmpresaId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        PR.[Id], PR.[EmpresaId], PR.[CategoriaId], C.[Nombre] AS [CategoriaNombre],
        PR.[MarcaId], M.[Nombre] AS [MarcaNombre], PR.[MarcaTexto], PR.[Nombre], PR.[Descripcion],
        PR.[FotoUrl], PR.[Codigo], PR.[TipoProducto], PR.[UnidadMedida], PR.[StockMinimo],
        ISNULL(PV.[PrecioVenta], 0) AS [PrecioVenta], CAST(0 AS DECIMAL(18,4)) AS [StockDisponible],
        PR.[Estatus], PR.[CreadoPor], PR.[FechaCreacion], PR.[ModificadoPor], PR.[FechaModificacion]
    FROM [dbo].[Producto] PR
    LEFT JOIN [dbo].[Categoria] C ON C.[Id] = PR.[CategoriaId]
    LEFT JOIN [dbo].[Marca] M ON M.[Id] = PR.[MarcaId]
    LEFT JOIN [dbo].[Precio] PV ON PV.[ProductoId] = PR.[Id] AND PV.[Activo] = 1 AND PV.[Estatus] = 1
    WHERE PR.[EmpresaId] = @EmpresaId
      AND PR.[Estatus] = 1
    ORDER BY PR.[Nombre] ASC;
END;

GO
PRINT 'Creando SP sp_Producto_Obtener';
GO
CREATE PROCEDURE [dbo].[sp_Producto_Obtener]
    @EmpresaId BIGINT,
    @ProductoId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        PR.[Id], PR.[EmpresaId], PR.[CategoriaId], C.[Nombre] AS [CategoriaNombre],
        PR.[MarcaId], M.[Nombre] AS [MarcaNombre], PR.[MarcaTexto], PR.[Nombre], PR.[Descripcion],
        PR.[FotoUrl], PR.[Codigo], PR.[TipoProducto], PR.[UnidadMedida], PR.[StockMinimo],
        ISNULL(PV.[PrecioVenta], 0) AS [PrecioVenta], CAST(0 AS DECIMAL(18,4)) AS [StockDisponible],
        PR.[Estatus], PR.[CreadoPor], PR.[FechaCreacion], PR.[ModificadoPor], PR.[FechaModificacion]
    FROM [dbo].[Producto] PR
    LEFT JOIN [dbo].[Categoria] C ON C.[Id] = PR.[CategoriaId]
    LEFT JOIN [dbo].[Marca] M ON M.[Id] = PR.[MarcaId]
    LEFT JOIN [dbo].[Precio] PV ON PV.[ProductoId] = PR.[Id] AND PV.[Activo] = 1 AND PV.[Estatus] = 1
    WHERE PR.[EmpresaId] = @EmpresaId
      AND PR.[Id] = @ProductoId;
END;

GO
PRINT 'Creando SP sp_Producto_PorCodigo';
GO
CREATE PROCEDURE [dbo].[sp_Producto_PorCodigo]
    @EmpresaId BIGINT,
    @Codigo NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        PR.[Id], PR.[EmpresaId], PR.[CategoriaId], C.[Nombre] AS [CategoriaNombre],
        PR.[MarcaId], M.[Nombre] AS [MarcaNombre], PR.[MarcaTexto], PR.[Nombre], PR.[Descripcion],
        PR.[FotoUrl], PR.[Codigo], PR.[TipoProducto], PR.[UnidadMedida], PR.[StockMinimo],
        ISNULL(PV.[PrecioVenta], 0) AS [PrecioVenta], CAST(0 AS DECIMAL(18,4)) AS [StockDisponible],
        PR.[Estatus], PR.[CreadoPor], PR.[FechaCreacion], PR.[ModificadoPor], PR.[FechaModificacion]
    FROM [dbo].[Producto] PR
    LEFT JOIN [dbo].[Categoria] C ON C.[Id] = PR.[CategoriaId]
    LEFT JOIN [dbo].[Marca] M ON M.[Id] = PR.[MarcaId]
    LEFT JOIN [dbo].[Precio] PV ON PV.[ProductoId] = PR.[Id] AND PV.[Activo] = 1 AND PV.[Estatus] = 1
    WHERE PR.[EmpresaId] = @EmpresaId
      AND PR.[Estatus] = 1
      AND PR.[Codigo] = @Codigo;
END;

GO
PRINT 'Creando SP sp_ProductoProveedor_EliminarLogico';
GO
CREATE PROCEDURE [dbo].[sp_ProductoProveedor_EliminarLogico]
    @EmpresaId BIGINT,
    @ProductoProveedorId BIGINT,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[ProductoProveedor]
    SET [Estatus] = 0,
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [EmpresaId] = @EmpresaId
      AND [Id] = @ProductoProveedorId
      AND [Estatus] = 1;

    SELECT @@ROWCOUNT AS [AffectedRows];
END;

GO
PRINT 'Creando SP sp_ProductoProveedor_Guardar';
GO
CREATE PROCEDURE [dbo].[sp_ProductoProveedor_Guardar]
    @EmpresaId BIGINT,
    @ProductoId BIGINT,
    @ProveedorId BIGINT,
    @Costo DECIMAL(18,4) = NULL,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS
    (
        SELECT 1 FROM [dbo].[ProductoProveedor]
        WHERE [ProductoId] = @ProductoId AND [ProveedorId] = @ProveedorId
    )
    BEGIN
        UPDATE [dbo].[ProductoProveedor]
        SET [Costo] = @Costo,
            [Estatus] = 1,
            [ModificadoPor] = @Actor,
            [FechaModificacion] = GETDATE()
        WHERE [ProductoId] = @ProductoId
          AND [ProveedorId] = @ProveedorId;

        SELECT TOP (1) [Id] FROM [dbo].[ProductoProveedor]
        WHERE [ProductoId] = @ProductoId AND [ProveedorId] = @ProveedorId;
    END
    ELSE
    BEGIN
        INSERT INTO [dbo].[ProductoProveedor]
        ([EmpresaId], [ProductoId], [ProveedorId], [Costo],
         [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
        VALUES
        (@EmpresaId, @ProductoId, @ProveedorId, @Costo,
         1, @Actor, GETDATE(), NULL, NULL);

        SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS [Id];
    END
END;

GO
PRINT 'Creando SP sp_ProductoProveedor_ListarPorProducto';
GO
CREATE PROCEDURE [dbo].[sp_ProductoProveedor_ListarPorProducto]
    @EmpresaId BIGINT,
    @ProductoId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        PP.[Id], PP.[EmpresaId], PP.[ProductoId], PP.[ProveedorId], PR.[Nombre] AS [ProveedorNombre],
        PP.[Costo],
        PP.[Estatus], PP.[CreadoPor], PP.[FechaCreacion], PP.[ModificadoPor], PP.[FechaModificacion]
    FROM [dbo].[ProductoProveedor] PP
    INNER JOIN [dbo].[Proveedor] PR ON PR.[Id] = PP.[ProveedorId]
    WHERE PP.[EmpresaId] = @EmpresaId
      AND PP.[ProductoId] = @ProductoId
      AND PP.[Estatus] = 1
    ORDER BY PR.[Nombre] ASC;
END;

GO
PRINT 'Creando SP sp_Proveedor_Actualizar';
GO
CREATE PROCEDURE [dbo].[sp_Proveedor_Actualizar]
    @EmpresaId BIGINT,
    @ProveedorId BIGINT,
    @Nombre NVARCHAR(150),
    @Contacto NVARCHAR(150) = NULL,
    @Correo NVARCHAR(250) = NULL,
    @Telefono NVARCHAR(50) = NULL,
    @Direccion NVARCHAR(500) = NULL,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Proveedor]
    SET
        [Nombre] = LTRIM(RTRIM(@Nombre)),
        [Contacto] = @Contacto,
        [Correo] = @Correo,
        [Telefono] = @Telefono,
        [Direccion] = @Direccion,
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [EmpresaId] = @EmpresaId
      AND [Id] = @ProveedorId
      AND [Estatus] = 1;

    SELECT @@ROWCOUNT AS [AffectedRows];
END;

GO
PRINT 'Creando SP sp_Proveedor_Consultar';
GO
CREATE PROCEDURE [dbo].[sp_Proveedor_Consultar]
    @EmpresaId BIGINT,
    @ProveedorId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id],
        [EmpresaId],
        [Nombre],
        [Contacto],
        [Correo],
        [Telefono],
        [Direccion],
        [Estatus],
        [CreadoPor],
        [FechaCreacion],
        [ModificadoPor],
        [FechaModificacion]
    FROM [dbo].[Proveedor]
    WHERE [EmpresaId] = @EmpresaId
      AND [Id] = @ProveedorId;
END;

GO
PRINT 'Creando SP sp_Proveedor_EliminarLogico';
GO
CREATE PROCEDURE [dbo].[sp_Proveedor_EliminarLogico]
    @EmpresaId BIGINT,
    @ProveedorId BIGINT,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Proveedor]
    SET
        [Estatus] = 0,
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [EmpresaId] = @EmpresaId
      AND [Id] = @ProveedorId
      AND [Estatus] = 1;

    SELECT @@ROWCOUNT AS [AffectedRows];
END;

GO
PRINT 'Creando SP sp_Proveedor_Guardar';
GO
CREATE PROCEDURE [dbo].[sp_Proveedor_Guardar]
    @EmpresaId BIGINT,
    @ProveedorId BIGINT = 0,
    @Nombre NVARCHAR(150),
    @Contacto NVARCHAR(150) = NULL,
    @Correo NVARCHAR(150) = NULL,
    @Telefono NVARCHAR(30) = NULL,
    @Direccion NVARCHAR(300) = NULL,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM [dbo].[Empresa] WHERE [Id] = @EmpresaId AND [Estatus] = 1)
    BEGIN
        RAISERROR('Empresa no valida para operacion de Proveedor.', 16, 1);
        RETURN;
    END;

    IF @ProveedorId IS NULL OR @ProveedorId = 0
    BEGIN
        INSERT INTO [dbo].[Proveedor]
        ([EmpresaId], [Nombre], [Contacto], [Correo], [Telefono], [Direccion],
         [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
        VALUES
        (@EmpresaId, LTRIM(RTRIM(@Nombre)), @Contacto, @Correo, @Telefono, @Direccion,
         1, @Actor, GETDATE(), NULL, NULL);

        SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS [Id];
    END
    ELSE
    BEGIN
        UPDATE [dbo].[Proveedor]
        SET [Nombre] = LTRIM(RTRIM(@Nombre)),
            [Contacto] = @Contacto,
            [Correo] = @Correo,
            [Telefono] = @Telefono,
            [Direccion] = @Direccion,
            [ModificadoPor] = @Actor,
            [FechaModificacion] = GETDATE()
        WHERE [EmpresaId] = @EmpresaId
          AND [Id] = @ProveedorId
          AND [Estatus] = 1;

        SELECT @ProveedorId AS [Id];
    END
END;

GO
PRINT 'Creando SP sp_Proveedor_Insertar';
GO
CREATE PROCEDURE [dbo].[sp_Proveedor_Insertar]
    @EmpresaId BIGINT,
    @Nombre NVARCHAR(150),
    @Contacto NVARCHAR(150) = NULL,
    @Correo NVARCHAR(250) = NULL,
    @Telefono NVARCHAR(50) = NULL,
    @Direccion NVARCHAR(500) = NULL,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[Empresa]
        WHERE [Id] = @EmpresaId
          AND [Estatus] = 1
    )
    BEGIN
        RAISERROR('Empresa no válida para operación de Proveedor.', 16, 1);
        RETURN;
    END;

    INSERT INTO [dbo].[Proveedor]
    (
        [EmpresaId], [Nombre], [Contacto], [Correo], [Telefono], [Direccion], [Estatus],
        [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion]
    )
    VALUES
    (
        @EmpresaId, LTRIM(RTRIM(@Nombre)), @Contacto, @Correo, @Telefono, @Direccion, 1,
        @Actor, GETDATE(), NULL, NULL
    );

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS [Id];
END;

GO
PRINT 'Creando SP sp_Proveedor_Listar';
GO
CREATE PROCEDURE [dbo].[sp_Proveedor_Listar]
    @EmpresaId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id],
        [EmpresaId],
        [Nombre],
        [Contacto],
        [Correo],
        [Telefono],
        [Direccion],
        [Estatus],
        [CreadoPor],
        [FechaCreacion],
        [ModificadoPor],
        [FechaModificacion]
    FROM [dbo].[Proveedor]
    WHERE [EmpresaId] = @EmpresaId
      AND [Estatus] = 1
    ORDER BY [Nombre] ASC;
END;

GO
PRINT 'Creando SP sp_Proveedor_Obtener';
GO
CREATE PROCEDURE [dbo].[sp_Proveedor_Obtener]
    @EmpresaId BIGINT,
    @ProveedorId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id], [EmpresaId], [Nombre], [Contacto], [Correo], [Telefono], [Direccion],
        [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion]
    FROM [dbo].[Proveedor]
    WHERE [EmpresaId] = @EmpresaId
      AND [Id] = @ProveedorId;
END;

GO
PRINT 'Creando SP sp_Reporte_MasVendidos';
GO
CREATE PROCEDURE [dbo].[sp_Reporte_MasVendidos]
    @EmpresaId BIGINT,
    @FechaInicio DATETIME,
    @FechaFin DATETIME,
    @Top INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    IF @Top IS NULL OR @Top <= 0 SET @Top = 10;

    SELECT TOP (@Top)
        P.[Id] AS [ProductoId],
        P.[Nombre] AS [ProductoNombre],
        ISNULL(SUM(D.[Cantidad]), 0) AS [CantidadVendida],
        ISNULL(SUM(D.[Importe]), 0) AS [TotalVenta]
    FROM [dbo].[VentaDetalle] D
    INNER JOIN [dbo].[Venta] V ON V.[Id] = D.[VentaId]
    INNER JOIN [dbo].[Producto] P ON P.[Id] = D.[ProductoId]
    WHERE V.[EmpresaId] = @EmpresaId
      AND V.[Estatus] = 1
      AND D.[Estatus] = 1
      AND V.[FechaVenta] >= @FechaInicio
      AND V.[FechaVenta] <= @FechaFin
    GROUP BY P.[Id], P.[Nombre]
    ORDER BY [CantidadVendida] DESC;
END;

GO
PRINT 'Creando SP sp_Reporte_Utilidad';
GO
CREATE PROCEDURE [dbo].[sp_Reporte_Utilidad]
    @EmpresaId BIGINT,
    @FechaInicio DATETIME,
    @FechaFin DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        P.[Id] AS [ProductoId],
        P.[Nombre] AS [ProductoNombre],
        ISNULL(SUM(D.[Cantidad]), 0) AS [CantidadVendida],
        ISNULL(SUM(D.[Importe]), 0) AS [TotalVenta],
        ISNULL(SUM(D.[Cantidad] * ISNULL(PR.[PrecioCompra], 0)), 0) AS [TotalCosto],
        ISNULL(SUM(D.[Importe]), 0) - ISNULL(SUM(D.[Cantidad] * ISNULL(PR.[PrecioCompra], 0)), 0) AS [Utilidad]
    FROM [dbo].[VentaDetalle] D
    INNER JOIN [dbo].[Venta] V ON V.[Id] = D.[VentaId]
    INNER JOIN [dbo].[Producto] P ON P.[Id] = D.[ProductoId]
    OUTER APPLY
    (
        SELECT TOP (1) [PrecioCompra]
        FROM [dbo].[Precio]
        WHERE [ProductoId] = P.[Id] AND [Activo] = 1 AND [Estatus] = 1
        ORDER BY [FechaInicio] DESC
    ) PR
    WHERE V.[EmpresaId] = @EmpresaId
      AND V.[Estatus] = 1
      AND D.[Estatus] = 1
      AND V.[FechaVenta] >= @FechaInicio
      AND V.[FechaVenta] <= @FechaFin
    GROUP BY P.[Id], P.[Nombre]
    ORDER BY [Utilidad] DESC;
END;

GO
PRINT 'Creando SP sp_Reporte_VentasPorCajero';
GO
CREATE PROCEDURE [dbo].[sp_Reporte_VentasPorCajero]
    @EmpresaId BIGINT,
    @FechaInicio DATETIME,
    @FechaFin DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        U.[Id] AS [UsuarioId],
        U.[NombreUsuario] AS [CajeroNombre],
        COUNT(V.[Id]) AS [NumeroVentas],
        ISNULL(SUM(V.[Total]), 0) AS [Total]
    FROM [dbo].[Usuario] U
    INNER JOIN [dbo].[Venta] V ON V.[UsuarioId] = U.[Id]
        AND V.[Estatus] = 1
        AND V.[FechaVenta] >= @FechaInicio
        AND V.[FechaVenta] <= @FechaFin
    WHERE U.[EmpresaId] = @EmpresaId
    GROUP BY U.[Id], U.[NombreUsuario]
    ORDER BY [Total] DESC;
END;

GO
PRINT 'Creando SP sp_Reporte_VentasPorPeriodo';
GO
CREATE PROCEDURE [dbo].[sp_Reporte_VentasPorPeriodo]
    @EmpresaId BIGINT,
    @FechaInicio DATETIME,
    @FechaFin DATETIME,
    @SucursalId BIGINT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        V.[FechaVenta] AS [Fecha],
        V.[Folio],
        S.[Nombre] AS [SucursalNombre],
        U.[NombreUsuario] AS [CajeroNombre],
        V.[MetodoPago],
        V.[Subtotal],
        V.[Impuesto],
        V.[Total]
    FROM [dbo].[Venta] V
    INNER JOIN [dbo].[Sucursal] S ON S.[Id] = V.[SucursalId]
    INNER JOIN [dbo].[Usuario] U ON U.[Id] = V.[UsuarioId]
    WHERE V.[EmpresaId] = @EmpresaId
      AND V.[Estatus] = 1
      AND V.[FechaVenta] >= @FechaInicio
      AND V.[FechaVenta] <= @FechaFin
      AND (@SucursalId = 0 OR V.[SucursalId] = @SucursalId)
    ORDER BY V.[FechaVenta] DESC;
END;

GO
PRINT 'Creando SP sp_Reporte_VentasPorSucursal';
GO
CREATE PROCEDURE [dbo].[sp_Reporte_VentasPorSucursal]
    @EmpresaId BIGINT,
    @FechaInicio DATETIME,
    @FechaFin DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        S.[Id] AS [SucursalId],
        S.[Nombre] AS [SucursalNombre],
        COUNT(V.[Id]) AS [NumeroVentas],
        ISNULL(SUM(V.[Total]), 0) AS [Total]
    FROM [dbo].[Sucursal] S
    LEFT JOIN [dbo].[Venta] V ON V.[SucursalId] = S.[Id]
        AND V.[Estatus] = 1
        AND V.[FechaVenta] >= @FechaInicio
        AND V.[FechaVenta] <= @FechaFin
    WHERE S.[EmpresaId] = @EmpresaId
      AND S.[Estatus] = 1
    GROUP BY S.[Id], S.[Nombre]
    ORDER BY [Total] DESC;
END;

GO
PRINT 'Creando SP sp_Rol_EliminarLogico';
GO
CREATE PROCEDURE [dbo].[sp_Rol_EliminarLogico]
    @EmpresaId BIGINT,
    @RolId BIGINT,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Rol]
    SET
        [Estatus] = 0,
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [EmpresaId] = @EmpresaId
      AND [Id] = @RolId
      AND [Estatus] = 1;

    SELECT @@ROWCOUNT AS [AffectedRows];
END;

GO
PRINT 'Creando SP sp_Rol_Guardar';
GO
CREATE PROCEDURE [dbo].[sp_Rol_Guardar]
    @Id BIGINT,
    @EmpresaId BIGINT,
    @Nombre NVARCHAR(50),
    @Descripcion NVARCHAR(250) = NULL,
    @PuedeAutorizar BIT = 0,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    SET @Nombre = LTRIM(RTRIM(@Nombre));

    IF NOT EXISTS (SELECT 1 FROM [dbo].[Empresa] WHERE [Id] = @EmpresaId AND [Estatus] = 1)
    BEGIN
        SELECT CAST(0 AS BIGINT);
        RETURN;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM [dbo].[Rol]
        WHERE [EmpresaId] = @EmpresaId
          AND [Nombre] = @Nombre
          AND [Id] <> ISNULL(@Id, 0)
          AND [Estatus] = 1
    )
    BEGIN
        SELECT CAST(-1 AS BIGINT);
        RETURN;
    END;

    IF @Id IS NULL OR @Id = 0
    BEGIN
        INSERT INTO [dbo].[Rol]
        (
            [EmpresaId], [Nombre], [Descripcion], [PuedeAutorizar], [Estatus],
            [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion]
        )
        VALUES
        (
            @EmpresaId, @Nombre, @Descripcion, @PuedeAutorizar, 1,
            @Actor, GETDATE(), NULL, NULL
        );

        SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
    END
    ELSE
    BEGIN
        UPDATE [dbo].[Rol]
        SET
            [Nombre] = @Nombre,
            [Descripcion] = @Descripcion,
            [PuedeAutorizar] = @PuedeAutorizar,
            [ModificadoPor] = @Actor,
            [FechaModificacion] = GETDATE()
        WHERE [Id] = @Id
          AND [EmpresaId] = @EmpresaId
          AND [Estatus] = 1;

        SELECT CAST(@Id AS BIGINT);
    END;
END;

GO
PRINT 'Creando SP sp_Rol_Listar';
GO
CREATE PROCEDURE [dbo].[sp_Rol_Listar]
    @Usuario NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        R.[Id],
        R.[EmpresaId],
        R.[Nombre],
        R.[Descripcion],
        R.[PuedeAutorizar],
        R.[Estatus],
        R.[CreadoPor],
        R.[FechaCreacion],
        R.[ModificadoPor],
        R.[FechaModificacion]
    FROM [dbo].[Rol] R
    INNER JOIN [dbo].[Usuario] U
        ON U.[EmpresaId] = R.[EmpresaId]
    WHERE U.[NombreUsuario] = @Usuario
      AND U.[Estatus] = 1
      AND R.[Estatus] = 1
    ORDER BY R.[Nombre] ASC;
END;

GO
PRINT 'Creando SP sp_Rol_Obtener';
GO
CREATE PROCEDURE [dbo].[sp_Rol_Obtener]
    @EmpresaId BIGINT,
    @RolId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id],
        [EmpresaId],
        [Nombre],
        [Descripcion],
        [PuedeAutorizar],
        [Estatus],
        [CreadoPor],
        [FechaCreacion],
        [ModificadoPor],
        [FechaModificacion]
    FROM [dbo].[Rol]
    WHERE [EmpresaId] = @EmpresaId
      AND [Id] = @RolId;
END;

GO
PRINT 'Creando SP sp_Rol_PorUsuario';
GO
CREATE PROCEDURE [dbo].[sp_Rol_PorUsuario]
    @EmpresaId BIGINT,
    @UsuarioId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        R.[Id],
        R.[EmpresaId],
        R.[Nombre],
        R.[Descripcion],
        R.[PuedeAutorizar],
        R.[Estatus],
        R.[CreadoPor],
        R.[FechaCreacion],
        R.[ModificadoPor],
        R.[FechaModificacion]
    FROM [dbo].[Rol] R
    INNER JOIN [dbo].[UsuarioRol] UR
        ON R.[Id] = UR.[RolId]
    WHERE UR.[UsuarioId] = @UsuarioId
      AND UR.[Estatus] = 1
      AND R.[Estatus] = 1
      AND R.[EmpresaId] = @EmpresaId
    ORDER BY R.[Nombre] ASC;
END;

GO
PRINT 'Creando SP sp_RolPaginaAccion_ConteoPorRol';
GO
CREATE PROCEDURE [dbo].[sp_RolPaginaAccion_ConteoPorRol]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        R.[Id] AS [RolId],
        CAST(COUNT(RPA.[Id]) AS INT) AS [TotalPaginas]
    FROM [dbo].[Rol] R
    LEFT JOIN [dbo].[RolPaginaAccion] RPA
        ON RPA.[RolId] = R.[Id]
       AND RPA.[Estatus] = 1
    WHERE R.[Estatus] = 1
    GROUP BY R.[Id];
END;

GO
PRINT 'Creando SP sp_RolPaginaAccion_EliminarLogico';
GO
CREATE PROCEDURE [dbo].[sp_RolPaginaAccion_EliminarLogico]
    @EmpresaId BIGINT,
    @Id BIGINT,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE RPA
    SET
        RPA.[Estatus] = 0,
        RPA.[ModificadoPor] = @Actor,
        RPA.[FechaModificacion] = GETDATE()
    FROM [dbo].[RolPaginaAccion] RPA
    INNER JOIN [dbo].[Rol] R
        ON RPA.[RolId] = R.[Id]
       AND R.[EmpresaId] = @EmpresaId
    WHERE RPA.[Id] = @Id
      AND RPA.[Estatus] = 1;

    SELECT @@ROWCOUNT AS [AffectedRows];
END;

GO
PRINT 'Creando SP sp_RolPaginaAccion_EliminarPorRol';
GO
CREATE PROCEDURE [dbo].[sp_RolPaginaAccion_EliminarPorRol]
    @RolId BIGINT,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[RolPaginaAccion]
    SET
        [Estatus] = 0,
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [RolId] = @RolId
      AND [Estatus] = 1;

    SELECT @@ROWCOUNT AS [AffectedRows];
END;

GO
PRINT 'Creando SP sp_RolPaginaAccion_Guardar';
GO
CREATE PROCEDURE [dbo].[sp_RolPaginaAccion_Guardar]
    @EmpresaId BIGINT,
    @RolId BIGINT,
    @PaginaId BIGINT,
    @PuedeLeer BIT = 0,
    @PuedeCrear BIT = 0,
    @PuedeEditar BIT = 0,
    @PuedeEliminar BIT = 0,
    @PuedeExportar BIT = 0,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[Rol]
        WHERE [Id] = @RolId
          AND [EmpresaId] = @EmpresaId
          AND [Estatus] = 1
    )
    BEGIN
        RAISERROR('Rol no válido para operación de permisos.', 16, 1);
        RETURN;
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[Pagina]
        WHERE [Id] = @PaginaId
          AND [Estatus] = 1
    )
    BEGIN
        RAISERROR('Página no válida para operación de permisos.', 16, 1);
        RETURN;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM [dbo].[RolPaginaAccion]
        WHERE [RolId] = @RolId
          AND [PaginaId] = @PaginaId
          AND [Estatus] = 1
    )
    BEGIN
        UPDATE [dbo].[RolPaginaAccion]
        SET
            [PuedeLeer] = @PuedeLeer,
            [PuedeCrear] = @PuedeCrear,
            [PuedeEditar] = @PuedeEditar,
            [PuedeEliminar] = @PuedeEliminar,
            [PuedeExportar] = @PuedeExportar,
            [ModificadoPor] = @Actor,
            [FechaModificacion] = GETDATE()
        WHERE [RolId] = @RolId
          AND [PaginaId] = @PaginaId
          AND [Estatus] = 1;

        SELECT CAST([Id] AS BIGINT) AS [Id]
        FROM [dbo].[RolPaginaAccion]
        WHERE [RolId] = @RolId
          AND [PaginaId] = @PaginaId
          AND [Estatus] = 1;
    END
    ELSE
    BEGIN
        INSERT INTO [dbo].[RolPaginaAccion]
        (
            [RolId], [PaginaId], [PuedeLeer], [PuedeCrear], [PuedeEditar],
            [PuedeEliminar], [PuedeExportar], [Estatus],
            [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion]
        )
        VALUES
        (
            @RolId, @PaginaId, @PuedeLeer, @PuedeCrear, @PuedeEditar,
            @PuedeEliminar, @PuedeExportar, 1,
            @Actor, GETDATE(), NULL, NULL
        );

        SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS [Id];
    END;
END;

GO
PRINT 'Creando SP sp_RolPaginaAccion_ListarPorRol';
GO
CREATE PROCEDURE [dbo].[sp_RolPaginaAccion_ListarPorRol]
    @EmpresaId BIGINT,
    @RolId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        RPA.[Id],
        RPA.[RolId],
        RPA.[PaginaId],
        RPA.[PuedeLeer],
        RPA.[PuedeCrear],
        RPA.[PuedeEditar],
        RPA.[PuedeEliminar],
        RPA.[PuedeExportar],
        RPA.[Estatus],
        RPA.[CreadoPor],
        RPA.[FechaCreacion],
        RPA.[ModificadoPor],
        RPA.[FechaModificacion],
        P.[Nombre] AS [PaginaNombre],
        P.[Direccion] AS [Direccion]
    FROM [dbo].[RolPaginaAccion] RPA
    INNER JOIN [dbo].[Rol] R
        ON RPA.[RolId] = R.[Id]
       AND R.[Estatus] = 1
       AND R.[EmpresaId] = @EmpresaId
    INNER JOIN [dbo].[Pagina] P
        ON RPA.[PaginaId] = P.[Id]
    WHERE RPA.[RolId] = @RolId
      AND RPA.[Estatus] = 1
    ORDER BY P.[OrdenB] ASC, P.[Nombre] ASC;
END;

GO
PRINT 'Creando SP sp_RolPaginaAccion_Validar';
GO
CREATE PROCEDURE [dbo].[sp_RolPaginaAccion_Validar]
    @EmpresaId BIGINT,
    @UsuarioId BIGINT,
    @PaginaNombre NVARCHAR(100),
    @Accion NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @PaginaId BIGINT;

    SELECT @PaginaId = P.[Id]
    FROM [dbo].[Pagina] P
    WHERE P.[Nombre] = @PaginaNombre
      AND P.[Estatus] = 1;

    IF @PaginaId IS NULL
    BEGIN
        SELECT CAST(0 AS BIT) AS [TienePermiso];
        RETURN;
    END;

    SELECT CAST
    (
        CASE WHEN EXISTS
        (
            SELECT 1
            FROM [dbo].[RolPaginaAccion] RPA
            INNER JOIN [dbo].[Rol] R
                ON RPA.[RolId] = R.[Id]
               AND R.[Estatus] = 1
               AND R.[EmpresaId] = @EmpresaId
            INNER JOIN [dbo].[UsuarioRol] UR
                ON R.[Id] = UR.[RolId]
               AND UR.[Estatus] = 1
               AND UR.[UsuarioId] = @UsuarioId
            WHERE RPA.[PaginaId] = @PaginaId
              AND RPA.[Estatus] = 1
              AND
              (
                  (@Accion = N'Leer' AND RPA.[PuedeLeer] = 1)
                  OR (@Accion = N'Crear' AND RPA.[PuedeCrear] = 1)
                  OR (@Accion = N'Editar' AND RPA.[PuedeEditar] = 1)
                  OR (@Accion = N'Eliminar' AND RPA.[PuedeEliminar] = 1)
                  OR (@Accion = N'Exportar' AND RPA.[PuedeExportar] = 1)
              )
        )
        THEN 1 ELSE 0 END AS BIT
    ) AS [TienePermiso];
END;

GO
PRINT 'Creando SP sp_SalidaCaja_Listar';
GO
CREATE PROCEDURE [dbo].[sp_SalidaCaja_Listar]
    @EmpresaId BIGINT,
    @CajaChicaId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id], [EmpresaId], [CajaChicaId], [Monto], [Comentario], [FechaHora], [Justificada], [EvidenciaUrl], [TipoSalida],
        [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion]
    FROM [dbo].[SalidaCaja]
    WHERE [EmpresaId] = @EmpresaId
      AND [CajaChicaId] = @CajaChicaId
      AND [Estatus] = 1
    ORDER BY [FechaHora] DESC;
END;

GO
PRINT 'Creando SP sp_SalidaCaja_Registrar';
GO
CREATE PROCEDURE [dbo].[sp_SalidaCaja_Registrar]
    @EmpresaId BIGINT,
    @CajaChicaId BIGINT,
    @Monto DECIMAL(18,4),
    @Comentario NVARCHAR(300) = NULL,
    @Justificada BIT = 0,
    @EvidenciaUrl NVARCHAR(300) = NULL,
    @TipoSalida NVARCHAR(30) = NULL,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @Ingresos DECIMAL(18,4);
    DECLARE @Salidas DECIMAL(18,4);

    SELECT @Ingresos = [IngresosTotales], @Salidas = [SalidasTotales]
    FROM [dbo].[CajaChica]
    WHERE [Id] = @CajaChicaId AND [EmpresaId] = @EmpresaId AND [Estado] = N'Abierta' AND [Estatus] = 1;

    IF @Ingresos IS NULL
    BEGIN
        RAISERROR('Caja chica no encontrada o ya cerrada.', 16, 1);
        RETURN;
    END;

    -- Limite: las salidas no pueden superar el 50% de los ingresos totales.
    IF (@Salidas + @Monto) > (@Ingresos * 0.5)
    BEGIN
        RAISERROR('La salida supera el limite permitido (50%% de los ingresos totales).', 16, 1);
        RETURN;
    END;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO [dbo].[SalidaCaja]
        ([EmpresaId], [CajaChicaId], [Monto], [Comentario], [FechaHora], [Justificada], [EvidenciaUrl], [TipoSalida],
         [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
        VALUES
        (@EmpresaId, @CajaChicaId, @Monto, @Comentario, GETDATE(), @Justificada, @EvidenciaUrl, @TipoSalida,
         1, @Actor, GETDATE(), NULL, NULL);

        UPDATE [dbo].[CajaChica]
        SET [SalidasTotales] = [SalidasTotales] + @Monto,
            [ModificadoPor] = @Actor,
            [FechaModificacion] = GETDATE()
        WHERE [Id] = @CajaChicaId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS [Id];
END;

GO
PRINT 'Creando SP sp_Stock_ListarPorSucursal';
GO
CREATE PROCEDURE [dbo].[sp_Stock_ListarPorSucursal]
    @EmpresaId BIGINT,
    @SucursalId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        S.[Id], S.[EmpresaId], S.[SucursalId], SU.[Nombre] AS [SucursalNombre],
        S.[ProductoId], P.[Nombre] AS [ProductoNombre], S.[Cantidad], S.[StockMinimo],
        CASE WHEN S.[Cantidad] <= S.[StockMinimo] THEN 1 ELSE 0 END AS [BajoMinimo],
        S.[Estatus], S.[CreadoPor], S.[FechaCreacion], S.[ModificadoPor], S.[FechaModificacion]
    FROM [dbo].[Stock] S
    INNER JOIN [dbo].[Sucursal] SU ON SU.[Id] = S.[SucursalId]
    INNER JOIN [dbo].[Producto] P ON P.[Id] = S.[ProductoId]
    WHERE S.[EmpresaId] = @EmpresaId
      AND S.[SucursalId] = @SucursalId
      AND S.[Estatus] = 1
    ORDER BY P.[Nombre] ASC;
END;

GO
PRINT 'Creando SP sp_Stock_Movimiento';
GO
CREATE PROCEDURE [dbo].[sp_Stock_Movimiento]
    @EmpresaId BIGINT,
    @SucursalId BIGINT,
    @ProductoId BIGINT,
    @TipoMovimiento NVARCHAR(20),
    @Cantidad DECIMAL(18,4),
    @Motivo NVARCHAR(300) = NULL,
    @ReferenciaId BIGINT = NULL,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS (SELECT 1 FROM [dbo].[Producto] WHERE [Id] = @ProductoId AND [EmpresaId] = @EmpresaId AND [Estatus] = 1)
    BEGIN
        RAISERROR('Producto no valido para movimiento de stock.', 16, 1);
        RETURN;
    END;

    DECLARE @Anterior DECIMAL(18,4) = 0;
    DECLARE @Nueva DECIMAL(18,4);

    BEGIN TRY
        BEGIN TRANSACTION;

        SELECT @Anterior = [Cantidad]
        FROM [dbo].[Stock]
        WHERE [SucursalId] = @SucursalId AND [ProductoId] = @ProductoId AND [Estatus] = 1;

        IF @Anterior IS NULL
        BEGIN
            SET @Anterior = 0;
            INSERT INTO [dbo].[Stock]
            ([EmpresaId], [SucursalId], [ProductoId], [Cantidad], [StockMinimo],
             [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
            VALUES
            (@EmpresaId, @SucursalId, @ProductoId, 0, 0, 1, @Actor, GETDATE(), NULL, NULL);
        END

        SET @Nueva = @Anterior + @Cantidad;
        IF @Nueva < 0 SET @Nueva = 0;

        UPDATE [dbo].[Stock]
        SET [Cantidad] = @Nueva,
            [ModificadoPor] = @Actor,
            [FechaModificacion] = GETDATE()
        WHERE [SucursalId] = @SucursalId AND [ProductoId] = @ProductoId AND [Estatus] = 1;

        INSERT INTO [dbo].[StockMovimiento]
        ([EmpresaId], [SucursalId], [ProductoId], [TipoMovimiento], [Cantidad],
         [ExistenciaAnterior], [ExistenciaNueva], [Motivo], [ReferenciaId],
         [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
        VALUES
        (@EmpresaId, @SucursalId, @ProductoId, @TipoMovimiento, @Cantidad,
         @Anterior, @Nueva, @Motivo, @ReferenciaId,
         1, @Actor, GETDATE(), NULL, NULL);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH

    SELECT @Nueva AS [ExistenciaNueva];
END;

GO
PRINT 'Creando SP sp_Stock_Obtener';
GO
CREATE PROCEDURE [dbo].[sp_Stock_Obtener]
    @EmpresaId BIGINT,
    @SucursalId BIGINT,
    @ProductoId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        S.[Id], S.[EmpresaId], S.[SucursalId], SU.[Nombre] AS [SucursalNombre],
        S.[ProductoId], P.[Nombre] AS [ProductoNombre], S.[Cantidad], S.[StockMinimo],
        CASE WHEN S.[Cantidad] <= S.[StockMinimo] THEN 1 ELSE 0 END AS [BajoMinimo],
        S.[Estatus], S.[CreadoPor], S.[FechaCreacion], S.[ModificadoPor], S.[FechaModificacion]
    FROM [dbo].[Stock] S
    INNER JOIN [dbo].[Sucursal] SU ON SU.[Id] = S.[SucursalId]
    INNER JOIN [dbo].[Producto] P ON P.[Id] = S.[ProductoId]
    WHERE S.[EmpresaId] = @EmpresaId
      AND S.[SucursalId] = @SucursalId
      AND S.[ProductoId] = @ProductoId
      AND S.[Estatus] = 1;
END;

GO
PRINT 'Creando SP sp_StockMovimiento_Listar';
GO
CREATE PROCEDURE [dbo].[sp_StockMovimiento_Listar]
    @EmpresaId BIGINT,
    @SucursalId BIGINT = 0,
    @ProductoId BIGINT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        M.[Id], M.[EmpresaId], M.[SucursalId], SU.[Nombre] AS [SucursalNombre],
        M.[ProductoId], P.[Nombre] AS [ProductoNombre], M.[TipoMovimiento], M.[Cantidad],
        M.[ExistenciaAnterior], M.[ExistenciaNueva], M.[Motivo], M.[ReferenciaId],
        M.[Estatus], M.[CreadoPor], M.[FechaCreacion], M.[ModificadoPor], M.[FechaModificacion]
    FROM [dbo].[StockMovimiento] M
    INNER JOIN [dbo].[Sucursal] SU ON SU.[Id] = M.[SucursalId]
    INNER JOIN [dbo].[Producto] P ON P.[Id] = M.[ProductoId]
    WHERE M.[EmpresaId] = @EmpresaId
      AND (@SucursalId = 0 OR M.[SucursalId] = @SucursalId)
      AND (@ProductoId = 0 OR M.[ProductoId] = @ProductoId)
      AND M.[Estatus] = 1
    ORDER BY M.[FechaCreacion] DESC;
END;

GO
PRINT 'Creando SP sp_Sucursal_Actualizar';
GO
CREATE PROCEDURE [dbo].[sp_Sucursal_Actualizar]
    @EmpresaId BIGINT,
    @SucursalId BIGINT,
    @Nombre NVARCHAR(100),
    @Descripcion NVARCHAR(MAX) = NULL,
    @Calle NVARCHAR(200) = NULL,
    @Ciudad NVARCHAR(100) = NULL,
    @Colonia NVARCHAR(100) = NULL,
    @CodigoPostal NVARCHAR(10) = NULL,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Sucursal]
    SET
        [Nombre] = LTRIM(RTRIM(@Nombre)),
        [Descripcion] = @Descripcion,
        [Calle] = @Calle,
        [Ciudad] = @Ciudad,
        [Colonia] = @Colonia,
        [CodigoPostal] = @CodigoPostal,
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [EmpresaId] = @EmpresaId
      AND [Id] = @SucursalId
      AND [Estatus] = 1;

    SELECT @@ROWCOUNT AS [AffectedRows];
END;

GO
PRINT 'Creando SP sp_Sucursal_Consultar';
GO
CREATE PROCEDURE [dbo].[sp_Sucursal_Consultar]
    @EmpresaId BIGINT,
    @SucursalId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id],
        [EmpresaId],
        [Nombre],
        [Descripcion],
        [Calle],
        [Ciudad],
        [Colonia],
        [CodigoPostal],
        [Estatus],
        [CreadoPor],
        [FechaCreacion],
        [ModificadoPor],
        [FechaModificacion]
    FROM [dbo].[Sucursal]
    WHERE [EmpresaId] = @EmpresaId
      AND [Id] = @SucursalId;
END;

GO
PRINT 'Creando SP sp_Sucursal_EliminarLogico';
GO
CREATE PROCEDURE [dbo].[sp_Sucursal_EliminarLogico]
    @EmpresaId BIGINT,
    @SucursalId BIGINT,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Sucursal]
    SET
        [Estatus] = 0,
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [EmpresaId] = @EmpresaId
      AND [Id] = @SucursalId
      AND [Estatus] = 1;

    SELECT @@ROWCOUNT AS [AffectedRows];
END;

GO
PRINT 'Creando SP sp_Sucursal_Guardar';
GO
CREATE PROCEDURE [dbo].[sp_Sucursal_Guardar]
    @EmpresaId BIGINT,
    @SucursalId BIGINT = 0,
    @Nombre NVARCHAR(100),
    @Descripcion NVARCHAR(MAX) = NULL,
    @Calle NVARCHAR(150) = NULL,
    @Ciudad NVARCHAR(100) = NULL,
    @Colonia NVARCHAR(100) = NULL,
    @CodigoPostal NVARCHAR(10) = NULL,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM [dbo].[Empresa] WHERE [Id] = @EmpresaId AND [Estatus] = 1)
    BEGIN
        RAISERROR('Empresa no valida para operacion de Sucursal.', 16, 1);
        RETURN;
    END;

    IF @SucursalId IS NULL OR @SucursalId = 0
    BEGIN
        INSERT INTO [dbo].[Sucursal]
        ([EmpresaId], [Nombre], [Descripcion], [Calle], [Ciudad], [Colonia], [CodigoPostal],
         [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
        VALUES
        (@EmpresaId, LTRIM(RTRIM(@Nombre)), @Descripcion, @Calle, @Ciudad, @Colonia, @CodigoPostal,
         1, @Actor, GETDATE(), NULL, NULL);

        SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS [Id];
    END
    ELSE
    BEGIN
        UPDATE [dbo].[Sucursal]
        SET [Nombre] = LTRIM(RTRIM(@Nombre)),
            [Descripcion] = @Descripcion,
            [Calle] = @Calle,
            [Ciudad] = @Ciudad,
            [Colonia] = @Colonia,
            [CodigoPostal] = @CodigoPostal,
            [ModificadoPor] = @Actor,
            [FechaModificacion] = GETDATE()
        WHERE [EmpresaId] = @EmpresaId
          AND [Id] = @SucursalId
          AND [Estatus] = 1;

        SELECT @SucursalId AS [Id];
    END
END;

GO
PRINT 'Creando SP sp_Sucursal_Insertar';
GO
CREATE PROCEDURE [dbo].[sp_Sucursal_Insertar]
    @EmpresaId BIGINT,
    @Nombre NVARCHAR(100),
    @Descripcion NVARCHAR(MAX) = NULL,
    @Calle NVARCHAR(200) = NULL,
    @Ciudad NVARCHAR(100) = NULL,
    @Colonia NVARCHAR(100) = NULL,
    @CodigoPostal NVARCHAR(10) = NULL,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[Empresa]
        WHERE [Id] = @EmpresaId
          AND [Estatus] = 1
    )
    BEGIN
        RAISERROR('Empresa no válida para operación de Sucursal.', 16, 1);
        RETURN;
    END;

    INSERT INTO [dbo].[Sucursal]
    (
        [EmpresaId], [Nombre], [Descripcion], [Calle], [Ciudad], [Colonia], [CodigoPostal], [Estatus],
        [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion]
    )
    VALUES
    (
        @EmpresaId, LTRIM(RTRIM(@Nombre)), @Descripcion, @Calle, @Ciudad, @Colonia, @CodigoPostal, 1,
        @Actor, GETDATE(), NULL, NULL
    );

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS [Id];
END;

GO
PRINT 'Creando SP sp_Sucursal_Listar';
GO
CREATE PROCEDURE [dbo].[sp_Sucursal_Listar]
    @EmpresaId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id],
        [EmpresaId],
        [Nombre],
        [Descripcion],
        [Calle],
        [Ciudad],
        [Colonia],
        [CodigoPostal],
        [Estatus],
        [CreadoPor],
        [FechaCreacion],
        [ModificadoPor],
        [FechaModificacion]
    FROM [dbo].[Sucursal]
    WHERE [EmpresaId] = @EmpresaId
      AND [Estatus] = 1
    ORDER BY [Nombre] ASC;
END;

GO
PRINT 'Creando SP sp_Sucursal_Obtener';
GO
CREATE PROCEDURE [dbo].[sp_Sucursal_Obtener]
    @EmpresaId BIGINT,
    @SucursalId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id], [EmpresaId], [Nombre], [Descripcion], [Calle], [Ciudad], [Colonia], [CodigoPostal],
        [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion]
    FROM [dbo].[Sucursal]
    WHERE [EmpresaId] = @EmpresaId
      AND [Id] = @SucursalId;
END;

GO
PRINT 'Creando SP sp_Sucursal_PorUsuario';
GO
CREATE PROCEDURE [dbo].[sp_Sucursal_PorUsuario]
    @EmpresaId BIGINT,
    @UsuarioId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    -- Sucursales asignadas explicitamente al usuario.
    SELECT
        S.[Id], S.[EmpresaId], S.[Nombre], S.[Descripcion], S.[Calle], S.[Ciudad], S.[Colonia], S.[CodigoPostal],
        S.[Estatus], S.[CreadoPor], S.[FechaCreacion], S.[ModificadoPor], S.[FechaModificacion]
    FROM [dbo].[Sucursal] S
    INNER JOIN [dbo].[UsuarioSucursal] US ON US.[SucursalId] = S.[Id] AND US.[Estatus] = 1
    WHERE S.[EmpresaId] = @EmpresaId
      AND S.[Estatus] = 1
      AND US.[UsuarioId] = @UsuarioId
    ORDER BY S.[Nombre] ASC;

    -- Si no tiene asignaciones, se devuelven todas las de la empresa (compatibilidad).
    IF @@ROWCOUNT = 0
    BEGIN
        SELECT
            [Id], [EmpresaId], [Nombre], [Descripcion], [Calle], [Ciudad], [Colonia], [CodigoPostal],
            [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion]
        FROM [dbo].[Sucursal]
        WHERE [EmpresaId] = @EmpresaId
          AND [Estatus] = 1
        ORDER BY [Nombre] ASC;
    END
END;

GO
PRINT 'Creando SP sp_TokenRecuperacion_Crear';
GO
CREATE PROCEDURE [dbo].[sp_TokenRecuperacion_Crear]
    @UsuarioId BIGINT,
    @Token NVARCHAR(200),
    @FechaExpiracion DATETIME,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[Usuario]
        WHERE [Id] = @UsuarioId
          AND [Estatus] = 1
    )
    BEGIN
        RAISERROR('Usuario no válido para generar token de recuperación.', 16, 1);
        RETURN;
    END;

    -- Invalida (borrado lógico) los tokens previos vigentes del usuario.
    UPDATE [dbo].[TokenRecuperacion]
    SET
        [Estatus] = 0,
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [UsuarioId] = @UsuarioId
      AND [Usado] = 0
      AND [Estatus] = 1;

    INSERT INTO [dbo].[TokenRecuperacion]
    (
        [UsuarioId], [Token], [FechaExpiracion], [Usado], [Estatus],
        [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion]
    )
    VALUES
    (
        @UsuarioId, @Token, @FechaExpiracion, 0, 1,
        @Actor, GETDATE(), NULL, NULL
    );

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS [Id];
END;

GO
PRINT 'Creando SP sp_TokenRecuperacion_MarcarUsado';
GO
CREATE PROCEDURE [dbo].[sp_TokenRecuperacion_MarcarUsado]
    @TokenRecuperacionId BIGINT,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[TokenRecuperacion]
    SET
        [Usado] = 1,
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [Id] = @TokenRecuperacionId
      AND [Estatus] = 1;

    SELECT @@ROWCOUNT AS [AffectedRows];
END;

GO
PRINT 'Creando SP sp_TokenRecuperacion_ObtenerPorToken';
GO
CREATE PROCEDURE [dbo].[sp_TokenRecuperacion_ObtenerPorToken]
    @Token NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        T.[Id],
        T.[UsuarioId],
        T.[Token],
        T.[FechaExpiracion],
        T.[Usado],
        T.[Estatus],
        T.[CreadoPor],
        T.[FechaCreacion],
        T.[ModificadoPor],
        T.[FechaModificacion],
        U.[NombreUsuario] AS [NombreUsuario]
    FROM [dbo].[TokenRecuperacion] T
    INNER JOIN [dbo].[Usuario] U
        ON T.[UsuarioId] = U.[Id]
    WHERE T.[Token] = @Token
      AND T.[Estatus] = 1
      AND T.[Usado] = 0
      AND T.[FechaExpiracion] >= GETDATE();
END;

GO
PRINT 'Creando SP sp_TokenRecuperacion_RestablecerContrasenia';
GO
CREATE PROCEDURE [dbo].[sp_TokenRecuperacion_RestablecerContrasenia]
    @Token NVARCHAR(100),
    @ContrasenaHash NVARCHAR(256),
    @ContrasenaSalt NVARCHAR(256),
    @ContrasenaIteraciones INT,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @UsuarioId BIGINT;

    SELECT TOP (1) @UsuarioId = [UsuarioId]
    FROM [dbo].[TokenRecuperacion]
    WHERE [Token] = @Token
      AND [Usado] = 0
      AND [Estatus] = 1
      AND [FechaExpiracion] >= GETDATE();

    IF @UsuarioId IS NULL
    BEGIN
        SELECT CAST(0 AS INT);
        RETURN;
    END;

    UPDATE [dbo].[Usuario]
    SET
        [ContrasenaHash] = @ContrasenaHash,
        [ContrasenaSalt] = @ContrasenaSalt,
        [ContrasenaIteraciones] = @ContrasenaIteraciones,
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [Id] = @UsuarioId
      AND [Estatus] = 1;

    UPDATE [dbo].[TokenRecuperacion]
    SET
        [Usado] = 1,
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [Token] = @Token;

    SELECT CAST(1 AS INT);
END;

GO
PRINT 'Creando SP sp_Usuario_ActualizarContrasena';
GO
CREATE PROCEDURE [dbo].[sp_Usuario_ActualizarContrasena]
    @UsuarioId BIGINT,
    @ContrasenaHash NVARCHAR(256),
    @ContrasenaSalt NVARCHAR(256),
    @ContrasenaIteraciones INT,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Usuario]
    SET
        [ContrasenaHash] = @ContrasenaHash,
        [ContrasenaSalt] = @ContrasenaSalt,
        [ContrasenaIteraciones] = @ContrasenaIteraciones,
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [Id] = @UsuarioId
      AND [Estatus] = 1;

    SELECT @@ROWCOUNT AS [AffectedRows];
END;

GO
PRINT 'Creando SP sp_Usuario_ConsultarPorNombreUsuario';
GO
CREATE PROCEDURE [dbo].[sp_Usuario_ConsultarPorNombreUsuario]
    @NombreUsuario NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        [Id],
        [EmpresaId],
        [NombreUsuario],
        [ContrasenaHash],
        [ContrasenaSalt],
        [ContrasenaIteraciones],
        [Correo],
        [Telefono],
        [ImagenPerfil],
        [Estatus],
        [CreadoPor],
        [FechaCreacion],
        [ModificadoPor],
        [FechaModificacion]
    FROM [dbo].[Usuario]
    WHERE [NombreUsuario] = @NombreUsuario
      AND [Estatus] = 1;
END;

GO
PRINT 'Creando SP sp_Usuario_EliminarLogico';
GO
CREATE PROCEDURE [dbo].[sp_Usuario_EliminarLogico]
    @EmpresaId BIGINT,
    @UsuarioId BIGINT,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Usuario]
    SET
        [Estatus] = 0,
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [Id] = @UsuarioId
      AND [EmpresaId] = @EmpresaId
      AND [Estatus] = 1;

    SELECT @@ROWCOUNT AS [AffectedRows];
END;

GO
PRINT 'Creando SP sp_Usuario_Guardar';
GO
CREATE PROCEDURE [dbo].[sp_Usuario_Guardar]
    @Id BIGINT,
    @EmpresaId BIGINT,
    @NombreUsuario NVARCHAR(25),
    @ContrasenaHash NVARCHAR(256) = NULL,
    @ContrasenaSalt NVARCHAR(256) = NULL,
    @ContrasenaIteraciones INT = NULL,
    @Correo NVARCHAR(250) = NULL,
    @Telefono NVARCHAR(50) = NULL,
    @ImagenPerfil NVARCHAR(500) = NULL,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    SET @NombreUsuario = LTRIM(RTRIM(@NombreUsuario));

    IF EXISTS
    (
        SELECT 1
        FROM [dbo].[Usuario]
        WHERE [NombreUsuario] = @NombreUsuario
          AND [Id] <> ISNULL(@Id, 0)
    )
    BEGIN
        SELECT CAST(-1 AS BIGINT);
        RETURN;
    END;

    IF @Id IS NULL OR @Id = 0
    BEGIN
        IF @ContrasenaHash IS NULL OR @ContrasenaSalt IS NULL OR @ContrasenaIteraciones IS NULL
        BEGIN
            SELECT CAST(0 AS BIGINT);
            RETURN;
        END;

        INSERT INTO [dbo].[Usuario]
        (
            [EmpresaId], [NombreUsuario], [ContrasenaHash], [ContrasenaSalt], [ContrasenaIteraciones],
            [Correo], [Telefono], [ImagenPerfil], [Estatus],
            [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion]
        )
        VALUES
        (
            @EmpresaId, @NombreUsuario, @ContrasenaHash, @ContrasenaSalt, @ContrasenaIteraciones,
            @Correo, @Telefono, @ImagenPerfil, 1,
            @Actor, GETDATE(), NULL, NULL
        );

        SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
    END
    ELSE
    BEGIN
        UPDATE [dbo].[Usuario]
        SET
            [NombreUsuario] = @NombreUsuario,
            [Correo] = @Correo,
            [Telefono] = @Telefono,
            [ImagenPerfil] = @ImagenPerfil,
            [ContrasenaHash] = CASE WHEN @ContrasenaHash IS NULL OR @ContrasenaHash = N'' THEN [ContrasenaHash] ELSE @ContrasenaHash END,
            [ContrasenaSalt] = CASE WHEN @ContrasenaSalt IS NULL OR @ContrasenaSalt = N'' THEN [ContrasenaSalt] ELSE @ContrasenaSalt END,
            [ContrasenaIteraciones] = CASE WHEN @ContrasenaIteraciones IS NULL OR @ContrasenaIteraciones = 0 THEN [ContrasenaIteraciones] ELSE @ContrasenaIteraciones END,
            [ModificadoPor] = @Actor,
            [FechaModificacion] = GETDATE()
        WHERE [Id] = @Id
          AND [EmpresaId] = @EmpresaId
          AND [Estatus] = 1;

        SELECT CAST(@Id AS BIGINT);
    END;
END;

GO
PRINT 'Creando SP sp_Usuario_Listar';
GO
CREATE PROCEDURE [dbo].[sp_Usuario_Listar]
    @EmpresaId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        U.[Id],
        U.[EmpresaId],
        U.[NombreUsuario],
        U.[Correo],
        U.[Telefono],
        U.[ImagenPerfil],
        U.[Estatus],
        U.[CreadoPor],
        U.[FechaCreacion],
        U.[ModificadoPor],
        U.[FechaModificacion],
        E.[NombreComercial] AS [EmpresaNombre]
    FROM [dbo].[Usuario] U
    INNER JOIN [dbo].[Empresa] E ON E.[Id] = U.[EmpresaId]
    WHERE U.[EmpresaId] = @EmpresaId
      AND U.[Estatus] = 1
    ORDER BY U.[NombreUsuario] ASC;
END;

GO
PRINT 'Creando SP sp_Usuario_Obtener';
GO
CREATE PROCEDURE [dbo].[sp_Usuario_Obtener]
    @EmpresaId BIGINT,
    @UsuarioId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        U.[Id],
        U.[EmpresaId],
        U.[NombreUsuario],
        U.[Correo],
        U.[Telefono],
        U.[ImagenPerfil],
        U.[Estatus],
        U.[CreadoPor],
        U.[FechaCreacion],
        U.[ModificadoPor],
        U.[FechaModificacion],
        E.[NombreComercial] AS [EmpresaNombre]
    FROM [dbo].[Usuario] U
    INNER JOIN [dbo].[Empresa] E ON E.[Id] = U.[EmpresaId]
    WHERE U.[Id] = @UsuarioId
      AND U.[EmpresaId] = @EmpresaId;
END;

GO
PRINT 'Creando SP sp_Usuario_ObtenerParaLogin';
GO
CREATE PROCEDURE [dbo].[sp_Usuario_ObtenerParaLogin]
    @NombreUsuario NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        U.[Id],
        U.[EmpresaId],
        U.[NombreUsuario],
        U.[ContrasenaHash],
        U.[ContrasenaSalt],
        U.[ContrasenaIteraciones],
        U.[Correo],
        U.[Telefono],
        U.[ImagenPerfil],
        U.[Estatus],
        U.[CreadoPor],
        U.[FechaCreacion],
        U.[ModificadoPor],
        U.[FechaModificacion],
        E.[NombreComercial] AS [EmpresaNombre]
    FROM [dbo].[Usuario] U
    INNER JOIN [dbo].[Empresa] E
        ON U.[EmpresaId] = E.[Id]
    WHERE U.[NombreUsuario] = @NombreUsuario
      AND U.[Estatus] = 1
      AND E.[Estatus] = 1
      AND GETDATE() >= E.[FechaVigenciaInicio]
      AND GETDATE() <= E.[FechaVigenciaFin];
END;

GO
PRINT 'Creando SP sp_UsuarioPagina_EliminarLogico';
GO
CREATE PROCEDURE [dbo].[sp_UsuarioPagina_EliminarLogico]
    @Id BIGINT,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[UsuarioPagina]
    SET
        [Estatus] = 0,
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [Id] = @Id
      AND [Estatus] = 1;

    SELECT @@ROWCOUNT AS [AffectedRows];
END;

GO
PRINT 'Creando SP sp_UsuarioPagina_Guardar';
GO
CREATE PROCEDURE [dbo].[sp_UsuarioPagina_Guardar]
    @Id BIGINT,
    @UsuarioId BIGINT,
    @PaginaId BIGINT,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ExistenteId BIGINT;

    SELECT TOP (1) @ExistenteId = [Id]
    FROM [dbo].[UsuarioPagina]
    WHERE [UsuarioId] = @UsuarioId
      AND [PaginaId] = @PaginaId;

    IF @ExistenteId IS NULL
    BEGIN
        INSERT INTO [dbo].[UsuarioPagina]
        (
            [UsuarioId], [PaginaId], [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion]
        )
        VALUES
        (
            @UsuarioId, @PaginaId, 1, @Actor, GETDATE(), NULL, NULL
        );

        SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
    END
    ELSE
    BEGIN
        UPDATE [dbo].[UsuarioPagina]
        SET
            [Estatus] = 1,
            [ModificadoPor] = @Actor,
            [FechaModificacion] = GETDATE()
        WHERE [Id] = @ExistenteId;

        SELECT CAST(@ExistenteId AS BIGINT);
    END;
END;

GO
PRINT 'Creando SP sp_UsuarioPagina_ListarPorUsuario';
GO
CREATE PROCEDURE [dbo].[sp_UsuarioPagina_ListarPorUsuario]
    @EmpresaId BIGINT,
    @UsuarioId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        UP.[Id],
        UP.[UsuarioId],
        UP.[PaginaId],
        UP.[Estatus],
        UP.[CreadoPor],
        UP.[FechaCreacion],
        UP.[ModificadoPor],
        UP.[FechaModificacion]
    FROM [dbo].[UsuarioPagina] UP
    INNER JOIN [dbo].[Usuario] U
        ON UP.[UsuarioId] = U.[Id]
       AND U.[EmpresaId] = @EmpresaId
    WHERE UP.[UsuarioId] = @UsuarioId
      AND UP.[Estatus] = 1
    ORDER BY UP.[Id] ASC;
END;

GO
PRINT 'Creando SP sp_UsuarioPagina_ListarTodos';
GO
CREATE PROCEDURE [dbo].[sp_UsuarioPagina_ListarTodos]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id],
        [UsuarioId],
        [PaginaId],
        [Estatus],
        [CreadoPor],
        [FechaCreacion],
        [ModificadoPor],
        [FechaModificacion]
    FROM [dbo].[UsuarioPagina]
    WHERE [Estatus] = 1;
END;

GO
PRINT 'Creando SP sp_UsuarioPagina_Obtener';
GO
CREATE PROCEDURE [dbo].[sp_UsuarioPagina_Obtener]
    @Id BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        [Id],
        [UsuarioId],
        [PaginaId],
        [Estatus],
        [CreadoPor],
        [FechaCreacion],
        [ModificadoPor],
        [FechaModificacion]
    FROM [dbo].[UsuarioPagina]
    WHERE [Id] = @Id
      AND [Estatus] = 1;
END;

GO
PRINT 'Creando SP sp_UsuarioRol_EliminarLogico';
GO
CREATE PROCEDURE [dbo].[sp_UsuarioRol_EliminarLogico]
    @EmpresaId BIGINT,
    @UsuarioRolId BIGINT,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE UR
    SET
        UR.[Estatus] = 0,
        UR.[ModificadoPor] = @Actor,
        UR.[FechaModificacion] = GETDATE()
    FROM [dbo].[UsuarioRol] UR
    INNER JOIN [dbo].[Rol] R
        ON UR.[RolId] = R.[Id]
       AND R.[EmpresaId] = @EmpresaId
    WHERE UR.[Id] = @UsuarioRolId
      AND UR.[Estatus] = 1;

    SELECT @@ROWCOUNT AS [AffectedRows];
END;

GO
PRINT 'Creando SP sp_UsuarioRol_Guardar';
GO
CREATE PROCEDURE [dbo].[sp_UsuarioRol_Guardar]
    @EmpresaId BIGINT,
    @UsuarioId BIGINT,
    @RolId BIGINT,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[Usuario]
        WHERE [Id] = @UsuarioId
          AND [EmpresaId] = @EmpresaId
          AND [Estatus] = 1
    )
    BEGIN
        RAISERROR('Usuario no válido para asignación de Rol.', 16, 1);
        RETURN;
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[Rol]
        WHERE [Id] = @RolId
          AND [EmpresaId] = @EmpresaId
          AND [Estatus] = 1
    )
    BEGIN
        RAISERROR('Rol no válido para asignación.', 16, 1);
        RETURN;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM [dbo].[UsuarioRol]
        WHERE [UsuarioId] = @UsuarioId
          AND [RolId] = @RolId
          AND [Estatus] = 1
    )
    BEGIN
        SELECT CAST([Id] AS BIGINT) AS [Id]
        FROM [dbo].[UsuarioRol]
        WHERE [UsuarioId] = @UsuarioId
          AND [RolId] = @RolId
          AND [Estatus] = 1;
        RETURN;
    END;

    INSERT INTO [dbo].[UsuarioRol]
    (
        [UsuarioId], [RolId], [Estatus],
        [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion]
    )
    VALUES
    (
        @UsuarioId, @RolId, 1,
        @Actor, GETDATE(), NULL, NULL
    );

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS [Id];
END;

GO
PRINT 'Creando SP sp_UsuarioRol_ListarPorUsuario';
GO
CREATE PROCEDURE [dbo].[sp_UsuarioRol_ListarPorUsuario]
    @EmpresaId BIGINT,
    @UsuarioId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        UR.[Id],
        UR.[UsuarioId],
        UR.[RolId],
        UR.[Estatus],
        UR.[CreadoPor],
        UR.[FechaCreacion],
        UR.[ModificadoPor],
        UR.[FechaModificacion]
    FROM [dbo].[UsuarioRol] UR
    INNER JOIN [dbo].[Rol] R
        ON UR.[RolId] = R.[Id]
       AND R.[Estatus] = 1
       AND R.[EmpresaId] = @EmpresaId
    WHERE UR.[UsuarioId] = @UsuarioId
      AND UR.[Estatus] = 1
    ORDER BY UR.[Id] ASC;
END;

GO
PRINT 'Creando SP sp_Venta_Guardar';
GO
CREATE PROCEDURE [dbo].[sp_Venta_Guardar]
    @EmpresaId BIGINT,
    @SucursalId BIGINT,
    @UsuarioId BIGINT,
    @ClienteId BIGINT = NULL,
    @CajaChicaId BIGINT = NULL,
    @Folio NVARCHAR(50) = NULL,
    @MetodoPago NVARCHAR(20),
    @Subtotal DECIMAL(18,4) = 0,
    @Impuesto DECIMAL(18,4) = 0,
    @Total DECIMAL(18,4) = 0,
    @DetalleJson NVARCHAR(MAX) = NULL,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @MetodoPago NOT IN (N'Efectivo', N'Tarjeta', N'Transferencia')
    BEGIN
        RAISERROR('Metodo de pago no valido.', 16, 1);
        RETURN;
    END;

    -- SYNC-11: la sucursal debe pertenecer a la empresa (tenant) autenticada.
    IF NOT EXISTS
    (
        SELECT 1 FROM [dbo].[Sucursal]
        WHERE [Id] = @SucursalId AND [EmpresaId] = @EmpresaId AND [Estatus] = 1
    )
    BEGIN
        RAISERROR('Sucursal no valida para la empresa.', 16, 1);
        RETURN;
    END;

    -- SYNC-11: si se informa caja chica, debe pertenecer a la empresa.
    IF @CajaChicaId IS NOT NULL AND NOT EXISTS
    (
        SELECT 1 FROM [dbo].[CajaChica]
        WHERE [Id] = @CajaChicaId AND [EmpresaId] = @EmpresaId AND [Estatus] = 1
    )
    BEGIN
        RAISERROR('Caja chica no valida para la empresa.', 16, 1);
        RETURN;
    END;

    DECLARE @VentaId BIGINT;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO [dbo].[Venta]
        ([EmpresaId], [SucursalId], [UsuarioId], [ClienteId], [CajaChicaId], [Folio], [FechaVenta], [MetodoPago],
         [Subtotal], [Impuesto], [Total], [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
        VALUES
        (@EmpresaId, @SucursalId, @UsuarioId, @ClienteId, @CajaChicaId, @Folio, GETDATE(), @MetodoPago,
         @Subtotal, @Impuesto, @Total, 1, @Actor, GETDATE(), NULL, NULL);

        SET @VentaId = CAST(SCOPE_IDENTITY() AS BIGINT);

        DECLARE @Detalle TABLE
        (
            ProductoId BIGINT,
            Cantidad DECIMAL(18,4),
            PrecioUnitario DECIMAL(18,4),
            Importe DECIMAL(18,4)
        );

        IF @DetalleJson IS NOT NULL AND LEN(@DetalleJson) > 2
        BEGIN
            INSERT INTO @Detalle (ProductoId, Cantidad, PrecioUnitario, Importe)
            SELECT ProductoId, Cantidad, PrecioUnitario, Importe
            FROM OPENJSON(@DetalleJson)
            WITH
            (
                ProductoId BIGINT '$.ProductoId',
                Cantidad DECIMAL(18,4) '$.Cantidad',
                PrecioUnitario DECIMAL(18,4) '$.PrecioUnitario',
                Importe DECIMAL(18,4) '$.Importe'
            );
        END

        INSERT INTO [dbo].[VentaDetalle]
        ([VentaId], [ProductoId], [Cantidad], [PrecioUnitario], [Importe],
         [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
        SELECT @VentaId, D.[ProductoId], D.[Cantidad], D.[PrecioUnitario], D.[Importe],
               1, @Actor, GETDATE(), NULL, NULL
        FROM @Detalle D;

        -- Descuenta stock de la sucursal activa, siempre dentro de la empresa (tenant).
        UPDATE S
        SET S.[Cantidad] = CASE WHEN S.[Cantidad] - D.[Cantidad] < 0 THEN 0 ELSE S.[Cantidad] - D.[Cantidad] END,
            S.[ModificadoPor] = @Actor,
            S.[FechaModificacion] = GETDATE()
        FROM [dbo].[Stock] S
        INNER JOIN @Detalle D ON D.[ProductoId] = S.[ProductoId]
        WHERE S.[SucursalId] = @SucursalId
          AND S.[EmpresaId] = @EmpresaId
          AND S.[Estatus] = 1;

        INSERT INTO [dbo].[StockMovimiento]
        ([EmpresaId], [SucursalId], [ProductoId], [TipoMovimiento], [Cantidad],
         [ExistenciaAnterior], [ExistenciaNueva], [Motivo], [ReferenciaId],
         [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
        SELECT @EmpresaId, @SucursalId, D.[ProductoId], N'Venta', -D.[Cantidad],
               ISNULL(S.[Cantidad] + D.[Cantidad], D.[Cantidad]), ISNULL(S.[Cantidad], 0), N'Venta', @VentaId,
               1, @Actor, GETDATE(), NULL, NULL
        FROM @Detalle D
        LEFT JOIN [dbo].[Stock] S ON S.[ProductoId] = D.[ProductoId]
                                 AND S.[SucursalId] = @SucursalId
                                 AND S.[EmpresaId] = @EmpresaId;

        -- Registra los ingresos en la caja chica activa (validada por empresa).
        IF @CajaChicaId IS NOT NULL
        BEGIN
            UPDATE [dbo].[CajaChica]
            SET [IngresosTotales] = [IngresosTotales] + @Total,
                [ModificadoPor] = @Actor,
                [FechaModificacion] = GETDATE()
            WHERE [Id] = @CajaChicaId
              AND [EmpresaId] = @EmpresaId
              AND [Estado] = N'Abierta'
              AND [Estatus] = 1;
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH

    SELECT @VentaId AS [Id];
END;

GO
PRINT 'Creando SP sp_Venta_Listar';
GO
CREATE PROCEDURE [dbo].[sp_Venta_Listar]
    @EmpresaId BIGINT,
    @SucursalId BIGINT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        V.[Id], V.[EmpresaId], V.[SucursalId], S.[Nombre] AS [SucursalNombre],
        V.[UsuarioId], U.[NombreUsuario] AS [UsuarioNombre], V.[ClienteId], C.[Nombre] AS [ClienteNombre],
        V.[CajaChicaId], V.[Folio], V.[FechaVenta], V.[MetodoPago], V.[Subtotal], V.[Impuesto], V.[Total],
        V.[Estatus], V.[CreadoPor], V.[FechaCreacion], V.[ModificadoPor], V.[FechaModificacion]
    FROM [dbo].[Venta] V
    INNER JOIN [dbo].[Sucursal] S ON S.[Id] = V.[SucursalId]
    INNER JOIN [dbo].[Usuario] U ON U.[Id] = V.[UsuarioId]
    LEFT JOIN [dbo].[Cliente] C ON C.[Id] = V.[ClienteId]
    WHERE V.[EmpresaId] = @EmpresaId
      AND V.[Estatus] = 1
      AND (@SucursalId = 0 OR V.[SucursalId] = @SucursalId)
    ORDER BY V.[FechaVenta] DESC;
END;

GO
PRINT 'Creando SP sp_Venta_Obtener';
GO
CREATE PROCEDURE [dbo].[sp_Venta_Obtener]
    @EmpresaId BIGINT,
    @VentaId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        V.[Id], V.[EmpresaId], V.[SucursalId], S.[Nombre] AS [SucursalNombre],
        V.[UsuarioId], U.[NombreUsuario] AS [UsuarioNombre], V.[ClienteId], C.[Nombre] AS [ClienteNombre],
        V.[CajaChicaId], V.[Folio], V.[FechaVenta], V.[MetodoPago], V.[Subtotal], V.[Impuesto], V.[Total],
        V.[Estatus], V.[CreadoPor], V.[FechaCreacion], V.[ModificadoPor], V.[FechaModificacion]
    FROM [dbo].[Venta] V
    INNER JOIN [dbo].[Sucursal] S ON S.[Id] = V.[SucursalId]
    INNER JOIN [dbo].[Usuario] U ON U.[Id] = V.[UsuarioId]
    LEFT JOIN [dbo].[Cliente] C ON C.[Id] = V.[ClienteId]
    WHERE V.[EmpresaId] = @EmpresaId
      AND V.[Id] = @VentaId;

    SELECT
        D.[Id], D.[VentaId], D.[ProductoId], P.[Nombre] AS [ProductoNombre],
        D.[Cantidad], D.[PrecioUnitario], D.[Importe],
        D.[Estatus], D.[CreadoPor], D.[FechaCreacion], D.[ModificadoPor], D.[FechaModificacion]
    FROM [dbo].[VentaDetalle] D
    INNER JOIN [dbo].[Producto] P ON P.[Id] = D.[ProductoId]
    WHERE D.[VentaId] = @VentaId
      AND D.[Estatus] = 1;
END;

GO
/* ===================== SEEDS ===================== */
GO
PRINT 'Ejecutando seed seed-dev-user.sql';
GO
/*
  SOLO DESARROLLO.
  No usar en ambientes productivos.
  Credenciales reales y secretos productivos NO se versionan.
*/

DECLARE @Actor NVARCHAR(25) = N'seed-dev';
DECLARE @Ahora DATETIME = GETDATE();
DECLARE @EmpresaId BIGINT;

SELECT TOP (1) @EmpresaId = [Id]
FROM [dbo].[Empresa]
WHERE [RFC] = N'XAXX010101000';

IF @EmpresaId IS NULL
BEGIN
    INSERT INTO [dbo].[Empresa]
    (
        [NombreComercial],
        [RazonSocial],
        [RFC],
        [Responsable],
        [Direccion],
        [Ciudad],
        [Estado],
        [CodigoPostal],
        [Telefono],
        [CorreoContacto],
        [FechaVigenciaInicio],
        [FechaVigenciaFin],
        [EsPeriodoPrueba],
        [Estatus],
        [CreadoPor],
        [FechaCreacion],
        [ModificadoPor],
        [FechaModificacion],
        [LogoUrl]
    )
    VALUES
    (
        N'Empresa Demo Desarrollo',
        N'Empresa Demo Desarrollo SA de CV',
        N'XAXX010101000',
        N'Administrador Demo',
        N'Avenida Desarrollo 100',
        N'CDMX',
        N'Ciudad de México',
        N'01000',
        N'5550000000',
        N'dev@local.invalid',
        DATEADD(DAY, -1, @Ahora),
        DATEADD(YEAR, 1, @Ahora),
        1,
        1,
        @Actor,
        @Ahora,
        NULL,
        NULL,
        NULL
    );

    SET @EmpresaId = SCOPE_IDENTITY();
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Usuario] WHERE [NombreUsuario] = N'dev-admin')
BEGIN
    INSERT INTO [dbo].[Usuario]
    (
        [EmpresaId],
        [NombreUsuario],
        [ContrasenaHash],
        [ContrasenaSalt],
        [ContrasenaIteraciones],
        [Estatus],
        [CreadoPor],
        [FechaCreacion],
        [ModificadoPor],
        [FechaModificacion]
    )
    VALUES
    (
        @EmpresaId,
        N'dev-admin',
        N'43EUnE9xx0vTc1+NcbTcIeawakj6mqqsJWxyOGmV78U=',
        N'qbOYMRQm/0ToMv1SbqI3Eg==',
        150000,
        1,
        @Actor,
        @Ahora,
        NULL,
        NULL
    );
END;

GO
PRINT 'Ejecutando seed seed-security.sql';
GO
/*
  SOLO DESARROLLO.
  No usar en ambientes productivos.
  Precarga roles/páginas/permisos base del dominio POS (idempotente).
  No contiene credenciales ni secretos productivos.
*/

SET NOCOUNT ON;

DECLARE @Actor NVARCHAR(25) = N'seed-security';
DECLARE @Ahora DATETIME = GETDATE();

/* ============================================================
   1. Páginas base (globales, no dependen de empresa).
   ============================================================ */
DECLARE @Paginas TABLE
(
    [Nombre] NVARCHAR(100),
    [NombreVisible] NVARCHAR(150),
    [Descripcion] NVARCHAR(250),
    [Tipo] NVARCHAR(20),
    [Direccion] NVARCHAR(250),
    [Padre] NVARCHAR(100),
    [Logo] NVARCHAR(100),
    [OrdenB] INT
);

INSERT INTO @Paginas ([Nombre], [NombreVisible], [Descripcion], [Tipo], [Direccion], [Padre], [Logo], [OrdenB])
VALUES
    (N'Inicio', N'Inicio', N'Dashboard del sistema', N'Menu', N'/Home/Index', NULL, N'fas fa-home', 1),
    (N'Catalogos', N'Catálogos', N'Catálogos del sistema', N'SubMenu', N'#', NULL, N'fas fa-th-list', 10),
    (N'Marcas', N'Marcas', N'Catálogo de marcas', N'Menu', N'/Marca/Mark', N'Catalogos', N'fas fa-trademark', 11),
    (N'Operacion', N'Operación', N'Operación de venta', N'SubMenu', N'#', NULL, N'fas fa-cash-register', 20),
    (N'Ventas', N'Ventas', N'Punto de venta', N'Menu', N'/Venta/Index', N'Operacion', N'fas fa-shopping-cart', 21),
    (N'Inventario', N'Inventario', N'Inventario y stock', N'SubMenu', N'#', NULL, N'fas fa-boxes', 30),
    (N'Productos', N'Productos', N'Catálogo de productos', N'Menu', N'/Producto/Index', N'Inventario', N'fas fa-box', 31),
    (N'Clientes', N'Clientes', N'Catálogo de clientes', N'Menu', N'/Cliente/Index', N'Inventario', N'fas fa-users', 32),
    (N'Seguridad', N'Seguridad', N'Administración de seguridad', N'SubMenu', N'#', NULL, N'fas fa-shield-alt', 90),
    (N'Roles', N'Roles', N'Administración de roles', N'Menu', N'/Security/Role', N'Seguridad', N'fas fa-user-shield', 91),
    (N'Permisos', N'Permisos', N'Administración de permisos', N'Menu', N'/Security/Permissions', N'Seguridad', N'fas fa-key', 92),
    (N'Usuarios', N'Usuarios', N'Administración de usuarios', N'Menu', N'/User/Users', N'Seguridad', N'fas fa-user-cog', 93);

INSERT INTO [dbo].[Pagina]
(
    [Nombre], [NombreVisible], [Descripcion], [Tipo], [Direccion], [PermisosPadreId],
    [Logo], [OrdenB], [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion]
)
SELECT
    P.[Nombre],
    P.[NombreVisible],
    P.[Descripcion],
    P.[Tipo],
    P.[Direccion],
    (SELECT TOP (1) PG.[Id] FROM [dbo].[Pagina] PG WHERE PG.[Nombre] = P.[Padre] AND PG.[Estatus] = 1),
    P.[Logo],
    P.[OrdenB],
    1,
    @Actor,
    @Ahora,
    NULL,
    NULL
FROM @Paginas P
WHERE NOT EXISTS
(
    SELECT 1
    FROM [dbo].[Pagina] PG
    WHERE PG.[Nombre] = P.[Nombre]
      AND PG.[Estatus] = 1
);

/* Enlaza el padre de las páginas que ya existían sin jerarquía. */
UPDATE PG
SET PG.[PermisosPadreId] =
    (
        SELECT TOP (1) PADRE.[Id]
        FROM [dbo].[Pagina] PADRE
        WHERE PADRE.[Nombre] = P.[Padre]
          AND PADRE.[Estatus] = 1
    )
FROM [dbo].[Pagina] PG
INNER JOIN @Paginas P
    ON PG.[Nombre] = P.[Nombre]
WHERE P.[Padre] IS NOT NULL
  AND PG.[PermisosPadreId] IS NULL;

/* ============================================================
   2. Rol "Administrador" por empresa (precargado, editable).
   ============================================================ */
INSERT INTO [dbo].[Rol]
(
    [EmpresaId], [Nombre], [Descripcion], [PuedeAutorizar], [Estatus],
    [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion]
)
SELECT
    E.[Id],
    N'Administrador',
    N'Control total del sistema',
    1,
    1,
    @Actor,
    @Ahora,
    NULL,
    NULL
FROM [dbo].[Empresa] E
WHERE E.[Estatus] = 1
  AND NOT EXISTS
  (
      SELECT 1
      FROM [dbo].[Rol] R
      WHERE R.[EmpresaId] = E.[Id]
        AND R.[Nombre] = N'Administrador'
        AND R.[Estatus] = 1
  );

/* ============================================================
   3. Concede todos los permisos de página al rol Administrador.
   ============================================================ */
INSERT INTO [dbo].[RolPaginaAccion]
(
    [RolId], [PaginaId], [PuedeLeer], [PuedeCrear], [PuedeEditar], [PuedeEliminar], [PuedeExportar],
    [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion]
)
SELECT
    R.[Id],
    P.[Id],
    1, 1, 1, 1, 1,
    1,
    @Actor,
    @Ahora,
    NULL,
    NULL
FROM [dbo].[Rol] R
CROSS JOIN [dbo].[Pagina] P
WHERE R.[Nombre] = N'Administrador'
  AND R.[Estatus] = 1
  AND P.[Estatus] = 1
  AND NOT EXISTS
  (
      SELECT 1
      FROM [dbo].[RolPaginaAccion] RPA
      WHERE RPA.[RolId] = R.[Id]
        AND RPA.[PaginaId] = P.[Id]
        AND RPA.[Estatus] = 1
  );

/* ============================================================
   4. Asigna el rol Administrador al usuario de desarrollo.
   ============================================================ */
INSERT INTO [dbo].[UsuarioRol]
(
    [UsuarioId], [RolId], [Estatus],
    [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion]
)
SELECT
    U.[Id],
    R.[Id],
    1,
    @Actor,
    @Ahora,
    NULL,
    NULL
FROM [dbo].[Usuario] U
INNER JOIN [dbo].[Rol] R
    ON R.[EmpresaId] = U.[EmpresaId]
   AND R.[Nombre] = N'Administrador'
   AND R.[Estatus] = 1
WHERE U.[NombreUsuario] = N'dev-admin'
  AND U.[Estatus] = 1
  AND NOT EXISTS
  (
      SELECT 1
      FROM [dbo].[UsuarioRol] UR
      WHERE UR.[UsuarioId] = U.[Id]
        AND UR.[RolId] = R.[Id]
        AND UR.[Estatus] = 1
  );

GO
PRINT 'Ejecutando seed seed-dev-security.sql';
GO
/*
  SOLO DESARROLLO.
  Seed de seguridad: páginas/menús base, rol Administrador y asignación al usuario dev-admin.
  No usar en ambientes productivos.
*/

DECLARE @Actor NVARCHAR(25) = N'seed-dev';
DECLARE @Ahora DATETIME = GETDATE();
DECLARE @EmpresaId BIGINT;

SELECT TOP (1) @EmpresaId = [Id]
FROM [dbo].[Empresa]
WHERE [RFC] = N'XAXX010101000';

IF @EmpresaId IS NULL
BEGIN
    RETURN;
END;

/* ---------- Páginas / menús base (idempotente por Nombre) ---------- */
DECLARE @Paginas TABLE ([Nombre] NVARCHAR(50), [NombreVisible] NVARCHAR(100), [Tipo] NVARCHAR(20), [Direccion] NVARCHAR(250), [Logo] NVARCHAR(50), [OrdenB] INT, [Padre] NVARCHAR(50));

INSERT INTO @Paginas ([Nombre], [NombreVisible], [Tipo], [Direccion], [Logo], [OrdenB], [Padre]) VALUES
    (N'Inicio',     N'Inicio',     N'Menu', N'/Home/Index',       N'fa-home',          1,  NULL),
    (N'Catalogos',  N'Catálogos',  N'Menu', NULL,                 N'fa-th-list',       2,  NULL),
    (N'Ventas',     N'Ventas',     N'Menu', NULL,                 N'fa-cash-register', 3,  NULL),
    (N'Inventario', N'Inventario', N'Menu', NULL,                 N'fa-boxes',         4,  NULL),
    (N'Caja',       N'Caja',       N'Menu', NULL,                 N'fa-coins',         5,  NULL),
    (N'Reportes',   N'Reportes',   N'Menu', NULL,                 N'fa-chart-line',    6,  NULL),
    (N'Seguridad',  N'Seguridad',  N'Menu', NULL,                 N'fa-shield-alt',    7,  NULL),
    (N'Marcas',     N'Marcas',     N'SubMenu', N'/Marca/Mark',   N'fa-trademark',     1,  N'Catalogos'),
    (N'Roles',      N'Roles',      N'SubMenu', N'/Security/Role', N'fa-user-tag',     1,  N'Seguridad'),
    (N'Permisos',   N'Permisos',   N'SubMenu', N'/Security/Permisos', N'fa-key',     2,  N'Seguridad'),
    (N'Usuarios',   N'Usuarios',   N'SubMenu', N'/User/Users',   N'fa-users',         3,  N'Seguridad');

/* Padres */
INSERT INTO [dbo].[Pagina] ([Nombre], [NombreVisible], [Descripcion], [Tipo], [Direccion], [PermisosPadreId], [Logo], [OrdenB], [Estatus], [CreadoPor], [FechaCreacion])
SELECT P.[Nombre], P.[NombreVisible], NULL, P.[Tipo], P.[Direccion], NULL, P.[Logo], P.[OrdenB], 1, @Actor, @Ahora
FROM @Paginas P
WHERE P.[Padre] IS NULL
  AND NOT EXISTS (SELECT 1 FROM [dbo].[Pagina] X WHERE X.[Nombre] = P.[Nombre]);

/* Hijos */
INSERT INTO [dbo].[Pagina] ([Nombre], [NombreVisible], [Descripcion], [Tipo], [Direccion], [PermisosPadreId], [Logo], [OrdenB], [Estatus], [CreadoPor], [FechaCreacion])
SELECT P.[Nombre], P.[NombreVisible], NULL, P.[Tipo], P.[Direccion], PAD.[Id], P.[Logo], P.[OrdenB], 1, @Actor, @Ahora
FROM @Paginas P
LEFT JOIN [dbo].[Pagina] PAD ON PAD.[Nombre] = P.[Padre]
WHERE P.[Padre] IS NOT NULL
  AND NOT EXISTS (SELECT 1 FROM [dbo].[Pagina] X WHERE X.[Nombre] = P.[Nombre]);

/* ---------- Roles base (idempotente) ---------- */
IF NOT EXISTS (SELECT 1 FROM [dbo].[Rol] WHERE [EmpresaId] = @EmpresaId AND [Nombre] = N'Administrador')
BEGIN
    INSERT INTO [dbo].[Rol] ([EmpresaId], [Nombre], [Descripcion], [PuedeAutorizar], [Estatus], [CreadoPor], [FechaCreacion])
    VALUES (@EmpresaId, N'Administrador', N'Acceso total al sistema', 1, 1, @Actor, @Ahora);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Rol] WHERE [EmpresaId] = @EmpresaId AND [Nombre] = N'Vendedor')
BEGIN
    INSERT INTO [dbo].[Rol] ([EmpresaId], [Nombre], [Descripcion], [PuedeAutorizar], [Estatus], [CreadoPor], [FechaCreacion])
    VALUES (@EmpresaId, N'Vendedor', N'Operación de ventas', 0, 1, @Actor, @Ahora);
END;

/* ---------- Permisos del rol Administrador sobre todas las páginas ---------- */
DECLARE @RolAdminId BIGINT;
SELECT TOP (1) @RolAdminId = [Id] FROM [dbo].[Rol] WHERE [EmpresaId] = @EmpresaId AND [Nombre] = N'Administrador';

INSERT INTO [dbo].[RolPaginaAccion] ([RolId], [PaginaId], [PuedeLeer], [PuedeCrear], [PuedeEditar], [PuedeEliminar], [PuedeExportar], [Estatus], [CreadoPor], [FechaCreacion])
SELECT @RolAdminId, P.[Id], 1, 1, 1, 1, 1, 1, @Actor, @Ahora
FROM [dbo].[Pagina] P
WHERE P.[Estatus] = 1
  AND NOT EXISTS (SELECT 1 FROM [dbo].[RolPaginaAccion] RPA WHERE RPA.[RolId] = @RolAdminId AND RPA.[PaginaId] = P.[Id]);

/* ---------- Permisos del rol Vendedor (Ventas + Catálogos) ---------- */
DECLARE @RolVendedorId BIGINT;
SELECT TOP (1) @RolVendedorId = [Id] FROM [dbo].[Rol] WHERE [EmpresaId] = @EmpresaId AND [Nombre] = N'Vendedor';

INSERT INTO [dbo].[RolPaginaAccion] ([RolId], [PaginaId], [PuedeLeer], [PuedeCrear], [PuedeEditar], [PuedeEliminar], [PuedeExportar], [Estatus], [CreadoPor], [FechaCreacion])
SELECT @RolVendedorId, P.[Id], 1, 0, 0, 0, 0, 1, @Actor, @Ahora
FROM [dbo].[Pagina] P
WHERE P.[Nombre] IN (N'Inicio', N'Catalogos', N'Ventas', N'Marcas')
  AND P.[Estatus] = 1
  AND NOT EXISTS (SELECT 1 FROM [dbo].[RolPaginaAccion] RPA WHERE RPA.[RolId] = @RolVendedorId AND RPA.[PaginaId] = P.[Id]);

/* ---------- Asignar Administrador al usuario dev-admin ---------- */
DECLARE @DevUsuarioId BIGINT;
SELECT TOP (1) @DevUsuarioId = [Id] FROM [dbo].[Usuario] WHERE [NombreUsuario] = N'dev-admin';

IF @DevUsuarioId IS NOT NULL AND @RolAdminId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [dbo].[UsuarioRol] WHERE [UsuarioId] = @DevUsuarioId AND [RolId] = @RolAdminId AND [Estatus] = 1)
    BEGIN
        INSERT INTO [dbo].[UsuarioRol] ([UsuarioId], [RolId], [Estatus], [CreadoPor], [FechaCreacion])
        VALUES (@DevUsuarioId, @RolAdminId, 1, @Actor, @Ahora);
    END;
END;

GO
PRINT 'Ejecutando seed seed-cliente-publico-general.sql';
GO
/*
  SOLO DESARROLLO.
  No usar en ambientes productivos.
  Precarga el cliente especial "Público General" por empresa (idempotente).
  No contiene credenciales ni secretos productivos.
*/

SET NOCOUNT ON;

DECLARE @Actor NVARCHAR(25) = N'seed-cliente';
DECLARE @Ahora DATETIME = GETDATE();

INSERT INTO [dbo].[Cliente]
(
    [EmpresaId], [Nombre], [Correo], [Telefono], [Direccion], [EsPublicoGeneral], [Estatus],
    [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion]
)
SELECT
    E.[Id],
    N'Público General',
    NULL,
    NULL,
    NULL,
    1,
    1,
    @Actor,
    @Ahora,
    NULL,
    NULL
FROM [dbo].[Empresa] E
WHERE E.[Estatus] = 1
  AND NOT EXISTS
  (
      SELECT 1
      FROM [dbo].[Cliente] C
      WHERE C.[EmpresaId] = E.[Id]
        AND C.[Nombre] = N'Público General'
        AND C.[Estatus] = 1
  );

GO
PRINT 'Ejecutando seed seed-pos.sql';
GO
-- Seed de modulos POS (idempotente).
-- Crea el cliente especial "Publico General" por empresa activa si no existe.

IF NOT EXISTS
(
    SELECT 1
    FROM [dbo].[Cliente] C
    INNER JOIN [dbo].[Empresa] E ON E.[Id] = C.[EmpresaId]
    WHERE C.[EsPublicoGeneral] = 1
      AND C.[Estatus] = 1
      AND E.[Estatus] = 1
)
BEGIN
    INSERT INTO [dbo].[Cliente]
    ([EmpresaId], [Nombre], [Correo], [Telefono], [Direccion], [EsPublicoGeneral],
     [Estatus], [CreadoPor], [FechaCreacion], [ModificadoPor], [FechaModificacion])
    SELECT
        E.[Id], N'Publico General', NULL, NULL, NULL, 1,
        1, N'seed', GETDATE(), NULL, NULL
    FROM [dbo].[Empresa] E
    WHERE E.[Estatus] = 1
      AND NOT EXISTS
      (
          SELECT 1 FROM [dbo].[Cliente] C
          WHERE C.[EmpresaId] = E.[Id] AND C.[EsPublicoGeneral] = 1
      );
END;

GO
/* ===== Credencial inicial conocida: dev-admin / Admin123! ===== */
PRINT 'Estableciendo credencial dev-admin / Admin123!';
GO
DECLARE @Ahora DATETIME = GETDATE();
DECLARE @EmpresaId BIGINT = (SELECT TOP (1) [Id] FROM [dbo].[Empresa] ORDER BY [Id]);
IF @EmpresaId IS NULL
BEGIN
    INSERT INTO [dbo].[Empresa] ([NombreComercial],[RazonSocial],[RFC],[Responsable],[Direccion],[Ciudad],[Estado],[CodigoPostal],[Telefono],[CorreoContacto],[FechaVigenciaInicio],[FechaVigenciaFin],[EsPeriodoPrueba],[Estatus],[CreadoPor],[FechaCreacion],[ModificadoPor],[FechaModificacion],[LogoUrl])
    VALUES (N'Empresa Demo',N'Empresa Demo SA de CV',N'XAXX010101000',N'Administrador',N'Domicilio 1',N'CDMX',N'CDMX',N'01000',N'5550000000',N'admin@local.invalid',DATEADD(DAY,-1,@Ahora),DATEADD(YEAR,1,@Ahora),0,1,N'seed',@Ahora,NULL,NULL,NULL);
    SET @EmpresaId = SCOPE_IDENTITY();
END
IF EXISTS (SELECT 1 FROM [dbo].[Usuario] WHERE [NombreUsuario] = N'dev-admin')
    UPDATE [dbo].[Usuario] SET [ContrasenaHash]=N'FW8toA7zL/wSAHO0UDpD0T9MM740hFBJjbNVPrC+2k8=', [ContrasenaSalt]=N'oRg8xuWT6gc7kaI5NULtzw==', [ContrasenaIteraciones]=150000, [Estatus]=1 WHERE [NombreUsuario]=N'dev-admin';
ELSE
    INSERT INTO [dbo].[Usuario] ([EmpresaId],[NombreUsuario],[ContrasenaHash],[ContrasenaSalt],[ContrasenaIteraciones],[Estatus],[CreadoPor],[FechaCreacion],[ModificadoPor],[FechaModificacion])
    VALUES (@EmpresaId,N'dev-admin',N'FW8toA7zL/wSAHO0UDpD0T9MM740hFBJjbNVPrC+2k8=',N'oRg8xuWT6gc7kaI5NULtzw==',150000,1,N'seed',@Ahora,NULL,NULL);
GO
INSERT INTO [dbo].[UsuarioRol] ([UsuarioId],[RolId],[Estatus],[CreadoPor],[FechaCreacion],[ModificadoPor],[FechaModificacion])
SELECT U.[Id], R.[Id], 1, N'seed', GETDATE(), NULL, NULL
FROM [dbo].[Usuario] U
INNER JOIN [dbo].[Rol] R ON R.[EmpresaId]=U.[EmpresaId] AND R.[Nombre]=N'Administrador' AND R.[Estatus]=1
WHERE U.[NombreUsuario]=N'dev-admin' AND U.[Estatus]=1
  AND NOT EXISTS (SELECT 1 FROM [dbo].[UsuarioRol] UR WHERE UR.[UsuarioId]=U.[Id] AND UR.[RolId]=R.[Id] AND UR.[Estatus]=1);
GO
