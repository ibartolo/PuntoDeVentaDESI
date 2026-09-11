CREATE PROCEDURE [dbo].[sp_UsuarioPagina_EliminarLogico]
    @Id BIGINT,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[UsuarioPagina]
    SET
        [Estatus] = 0,
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [Id] = @Id
      AND [Estatus] = 1;

    SELECT @@ROWCOUNT AS [AffectedRows];
END;
