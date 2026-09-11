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
