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
