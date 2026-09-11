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
