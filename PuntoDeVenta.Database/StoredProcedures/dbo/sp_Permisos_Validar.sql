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
