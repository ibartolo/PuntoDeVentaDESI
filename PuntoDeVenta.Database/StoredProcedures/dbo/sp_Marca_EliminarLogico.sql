CREATE PROCEDURE [dbo].[sp_Marca_EliminarLogico]
    @EmpresaId BIGINT,
    @MarcaId BIGINT,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Marca]
    SET
        [Estatus] = 0,
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [EmpresaId] = @EmpresaId
      AND [Id] = @MarcaId
      AND [Estatus] = 1;

    SELECT @@ROWCOUNT AS [AffectedRows];
END;
