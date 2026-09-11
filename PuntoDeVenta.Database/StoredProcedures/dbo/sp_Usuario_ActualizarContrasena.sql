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
