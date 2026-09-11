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
