CREATE PROCEDURE [dbo].[sp_Compra_EliminarLogico]
    @EmpresaId BIGINT,
    @CompraId BIGINT,
    @Actor NVARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Compra]
    SET [Estatus] = 0,
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [EmpresaId] = @EmpresaId
      AND [Id] = @CompraId
      AND [Estatus] = 1;

    UPDATE [dbo].[CompraDetalle]
    SET [Estatus] = 0,
        [ModificadoPor] = @Actor,
        [FechaModificacion] = GETDATE()
    WHERE [CompraId] = @CompraId
      AND [Estatus] = 1;

    SELECT @@ROWCOUNT AS [AffectedRows];
END;
