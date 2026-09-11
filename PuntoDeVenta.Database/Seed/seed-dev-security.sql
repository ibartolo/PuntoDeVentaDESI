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
