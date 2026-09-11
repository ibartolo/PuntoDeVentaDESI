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
