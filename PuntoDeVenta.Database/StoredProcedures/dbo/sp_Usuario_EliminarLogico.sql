CREATE PROCEDURE [dbo].[sp_Usuario_EliminarLogico]
    @EmpresaId BIGINT,
    @UsuarioId BIGINT,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Usuario]
    SET
        [Estatus] = 0,
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [Id] = @UsuarioId
      AND [EmpresaId] = @EmpresaId
      AND [Estatus] = 1;

    SELECT @@ROWCOUNT AS [AffectedRows];
END;
