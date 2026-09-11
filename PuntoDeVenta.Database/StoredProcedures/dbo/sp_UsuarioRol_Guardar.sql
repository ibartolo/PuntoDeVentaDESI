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
