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
